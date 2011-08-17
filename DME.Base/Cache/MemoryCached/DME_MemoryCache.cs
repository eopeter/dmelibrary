using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DME.Base.Cache.MemoryCached
{
    public class DME_MemoryCache : DME_ICacheOperations
    {
        public DME_MemoryCache()
        {
        }
        public DME_MemoryCache(string prefix)
        {
            _keyPrefix = prefix;
        }
        private string _keyPrefix="";
        private bool _isCache = true;
        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeyPrefix
        {
            get
            {
                return _keyPrefix;
            }
            set
            {
                _keyPrefix = value;
            }
        }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache
        {
            set
            {
                _isCache = value;
            }
            get
            {
                return _isCache;
            }
        }
        public long Size
        {
            get { return DME_CacheItemDictionary.Size; }
        }

        public int Count
        {
            get { return DME_CacheItemDictionary.ItemDictionary.Count; }
        }

        /// <summary>
        /// 每隔多少分钟扫描一次
        /// </summary>
        public int IntervalMinutes
        {
            get
            {
                return DME_CacheItemDictionary.IntervalMinutes;
            }
            set
            {
                DME_CacheItemDictionary.IntervalMinutes = value;
            }
        }
        /// <summary>
        /// 处理多少分钟之前的元素
        /// </summary>
        public int ScavangeMinutes
        {
            get
            {
                return DME_CacheItemDictionary.ScavangeMinutes;
            }
            set
            {
                DME_CacheItemDictionary.ScavangeMinutes = value;
            }
        }
        /// <summary>
        /// 最大可缓存元素数
        /// </summary>
        public long MaxCount
        {
            get
            {
                return DME_CacheItemDictionary.MaxCount;
            }
            set
            {
                DME_CacheItemDictionary.MaxCount = value;
            }
        }
        /// <summary>
        /// 最大可使用缓存大小
        /// </summary>
        public long MaxSize
        {
            get
            {
                return DME_CacheItemDictionary.MaxSize;
            }
            set
            {
                DME_CacheItemDictionary.MaxSize = value;
            }
        }

        public object Get(string key)
        {
            if (!IsCache)
            {
                return null;
            }
            key = KeyPrefix + key;
            if (DME_CacheItemDictionary.ContainsKey(key))
            {
                DME_CacheItem cacheItem = DME_CacheItemDictionary.Get(key);
                return DME_SerializationUtility.ToObject(cacheItem.value);
            }
            else
            {
                return null;
            }
        }

        public void Set(string key, object value)
        {
            Set(key, value, DateTime.Now.AddYears(1));
        }

        public void Set(string key, object value, int expirySeconds)
        {
            Set(key, value, DateTime.Now.AddSeconds(expirySeconds));
        }

        public void Set(string key, object value, TimeSpan expiryTimeSpan)
        {
            Set(key, value, DateTime.Now.Add(expiryTimeSpan));
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiryDate"></param>
        public void Set(string key, object value, DateTime expiryDate)
        {
            if (!IsCache)
            {
                return;
            }

            key = KeyPrefix + key;

            if (!value.GetType().IsSerializable)
                throw new SerializationException("Object is not serializable");

            DME_CacheItemDictionary.Add(key,
                new DME_CacheItem(DateTime.Now, DME_SerializationUtility.ToBytes(value), expiryDate));
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (!IsCache)
            {
                return;
            }
            key = KeyPrefix + key;
            DME_CacheItemDictionary.Remove(key);
        }
        /// <summary>
        /// 移除所有缓存
        /// </summary>
        /// <returns></returns>
        public int RemoveAll()
        {
            if (!IsCache)
            {
                return 0;
            }

            int interfaceCount = 0;

            interfaceCount = DME_CacheItemDictionary.ItemDictionary.Count;
            DME_CacheItemDictionary.Clear();

            return interfaceCount;
        }

        //public void Scavange(double minutes)
        //{
        //    //CacheItemDictionary.Scavange(minutes);
        //}

        public bool ContainsKey(string key)
        {
            if (!IsCache)
            {
                return false;
            }
            key = KeyPrefix + key;
            return DME_CacheItemDictionary.ContainsKey(key);
        }
    }
}
