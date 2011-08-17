using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Cache.MemoryCached
{
    //TODO: Better method to scavange - perhaps every 1 minute and once
    //TODO: Problem with Remove - Deadlock //Fixed
    //TODO: optimization 
    //      we have three problems with the cache:
    //* Its doing GC.Collect everytime //Fixed
    //* Its possibly causing deadlocks //Fixed
    //* Its locking readers out//FIXED - needs testing if ReaderWriterLock is effective to use. Depends on read/write ratio.
    static class DME_CacheItemDictionary
    {
        //Dictionary to hold the CacheItems
        public static Dictionary<string, DME_CacheItem> ItemDictionary =
            new Dictionary<string, DME_CacheItem>();

        /* The initial size of the queue and the maximum allowed growth quotient
         * when dividing the current queue size with the initial. When the max
         * quotinent is reached the queue will truncate to its initial size by
         * removing the oldest item consecutively.
         * When a key is removed from the queue its CacheItem is disposed from
         * the Cache. Thus the cache size should be big enough to ensure the
         * item may be disposed.
         */
        private static System.Collections.Queue keyQueue =
            new System.Collections.Queue();

        private static DateTime _lastScavangeTime;
        private static ReaderWriterLockSlim _readWriteLock;

        private static int _intervalMinutes = 1;
        private static int _scavangeMinutes = 60;
        private static long _maxCount = 1000000;
        private static long _maxSize = 100 * 1024 * 1024;//100*1M

        public static long Size;

        /// <summary>
        /// 每隔多少分钟扫描一次
        /// </summary>
        public static int IntervalMinutes
        {
            get
            {
                return _intervalMinutes;
            }
            set
            {
                _intervalMinutes = value;
            }
        }
        /// <summary>
        /// 处理多少分钟之前的元素
        /// </summary>
        public static int ScavangeMinutes
        {
            get
            {
                return _scavangeMinutes;
            }
            set
            {
                _scavangeMinutes = value;
            }
        }
        /// <summary>
        /// 最大可缓存元素数
        /// </summary>
        public static long MaxCount
        {
            get
            {
                return _maxCount;
            }
            set
            {
                _maxCount = value;
            }
        }
        /// <summary>
        /// 最大可使用缓存大小
        /// </summary>
        public static long MaxSize
        {
            get
            {
                return _maxSize;
            }
            set
            {
                _maxSize = value;
            }
        }
        static DME_CacheItemDictionary()
        {
            _lastScavangeTime = DateTime.Now;
            _readWriteLock = new ReaderWriterLockSlim();
        }
        /// <summary>
        /// 是否存在Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(string key)
        {
            if (!ItemDictionary.ContainsKey(key))
            {
                return false;
            }
            _readWriteLock.EnterReadLock();
            try
            {
                _Remove(key);
            }
            catch
            {
                return false;
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }

            return ItemDictionary.ContainsKey(key);
        }
        /// <summary>
        /// 获取元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DME_CacheItem Get(string key)
        {
            _readWriteLock.EnterReadLock();
            //Monitor.Enter(ItemDictionary);
            try
            {
                return ItemDictionary[key];
            }
            finally
            {
                _readWriteLock.ExitReadLock();
                //Monitor.Exit(ItemDictionary);
            }
        }

        public static void Add(string key, DME_CacheItem item)
        {
            if (ItemDictionary.Count >= MaxCount || Size >= MaxSize)
            {
                Clear();
            }

            //Monitor.Enter(ItemDictionary);
            _readWriteLock.EnterWriteLock();
            try
            {
                if (ItemDictionary.ContainsKey(key))
                {
                    ItemDictionary[key].timestamp = DateTime.Now;
                    ItemDictionary[key].expiryDate = item.expiryDate;
                    ItemDictionary[key].value = item.value;
                }
                else
                {
                    Scavange();

                    //Monitor.Enter(keyQueue);
                    //try
                    //{
                        keyQueue.Enqueue(key);
                    //}
                    //finally
                    //{
                    //    Monitor.Exit(keyQueue);
                    //}

                    ItemDictionary.Add(key, new DME_CacheItem(DateTime.Now, item.value,item.expiryDate));
                    Size += item.value.Length;
                }
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
                //Monitor.Exit(ItemDictionary);
            }
        }

        /// <summary>
        /// Evaluates all cacheItems associated with this cache item to determine if it 
        /// should be considered expired. Evaluation stops as soon as any CacheItem is still valid. 
        /// </summary>
        /// <returns>True if cache should be considered expired</returns>
        //public static bool HasExpired()
        //{
        //    foreach (KeyValuePair<string, CacheItem> keyValuePair in ItemDictionary)
        //    {
        //        if (keyValuePair.Value.expiryDate.CompareTo(DateTime.Now) > 0)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// 每隔1分钟将minutes分钟以前添加的元素扫描一次
        /// </summary>
        /// <param name="minutes"></param>
        private static void Scavange()
        {
            //Only scavange every 1 minute
            TimeSpan difference = DateTime.Now - _lastScavangeTime;
            if (difference.Minutes < IntervalMinutes)
                return;

            _lastScavangeTime = DateTime.Now;

            //NOTE: There is already a lock from Add method, which calls this method.

            //Monitor.Enter(keyQueue);
            int keyQueueCount = keyQueue.Count;
            //try
            //{
                for (int i = 0; i < keyQueueCount; i++)
                {
                    if (ItemDictionary.ContainsKey((string)keyQueue.Peek()))
                    {
                        if (ItemDictionary[(string)keyQueue.Peek()].timestamp.CompareTo(
                            DateTime.Now.AddMinutes(ScavangeMinutes * -1)) < 0)
                        {
                           DME_CacheItem cahceItem = ItemDictionary[(string)keyQueue.Peek()];
                            _Remove((string)keyQueue.Dequeue());
                            cahceItem.Dispose();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        keyQueue.Dequeue();
                    }
                }
            //}
            //finally
            //{
            //    //Monitor.Exit(keyQueue);
            //}
            DME_GCWrapper.Collect();
        }

        public static void Remove(string key)
        {
            _readWriteLock.EnterWriteLock();
            //Monitor.Enter(ItemDictionary);
            //Monitor.Enter(keyQueue);
            try
            {
                //_Remove(key);
                if (ItemDictionary.ContainsKey(key))
                {
                    Size -= ItemDictionary[key].value.Length;
                    ItemDictionary.Remove(key);
                }
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
                //Monitor.Exit(keyQueue);
                //Monitor.Exit(ItemDictionary);
            }
        }

        private static void _Remove(string key)
        {
            if (ItemDictionary.ContainsKey(key))
            {
                if (ItemDictionary[key].expiryDate.CompareTo(DateTime.Now) < 0)
                {
                    Size -= ItemDictionary[key].value.Length;
                    ItemDictionary.Remove(key);
                }
                else
                {
                    if (!keyQueue.Contains(key))
                        keyQueue.Enqueue(key);
                }
            }
        }

        /* Clear() will only remove object references from the cache
         * and queue. Capacity will remain unchanged.
         */
        public static void Clear()
        {
            _readWriteLock.EnterWriteLock();
            //Monitor.Enter(keyQueue);
            //Monitor.Enter(ItemDictionary);
            try
            {
                Size = 0;
                keyQueue.Clear();
                ItemDictionary.Clear();
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
                //Monitor.Exit(ItemDictionary);
                //Monitor.Exit(keyQueue);
            }
        }
    }
}
