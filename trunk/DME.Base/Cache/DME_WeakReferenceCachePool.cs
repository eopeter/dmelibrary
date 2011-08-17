using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace DME.Base.Cache
{
    /// <summary>
    /// 弱引用缓存类
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DME_WeakReferenceCachePool<TKey, TItem> where TItem : class
    {
        #region The Cleaner class

        private class Cleaner : CriticalFinalizerObject
        {
            #region Instance Data

            private DME_WeakReferenceCachePool<TKey, TItem> _owner;

            #endregion

            #region Constructor & Finalizer

            public Cleaner(DME_WeakReferenceCachePool<TKey, TItem> owner)
            {
                this._owner = owner;
            }

            ~Cleaner()
            {
                if (this._owner._autoCleanAbandonedItems)
                {
                    this._owner.CleanAbandonedItems();
                    GC.ReRegisterForFinalize(this);
                }
            }

            #endregion
        }

        #endregion

        #region Instance Data

        private const int LOCK_TIMEOUT_MSECS = 500;
        private Dictionary<TKey, WeakReference> _cachePool;
        private bool _autoCleanAbandonedItems;
        private ReaderWriterLockSlim _cacheLock;

        #endregion

        #region Constructor & Finalizer

        public DME_WeakReferenceCachePool() : this(true) { }

        public DME_WeakReferenceCachePool(bool autoCleanAbandonedItems)
        {
            this._cacheLock = new ReaderWriterLockSlim();
            this._cachePool = new Dictionary<TKey, WeakReference>();
            this._autoCleanAbandonedItems = autoCleanAbandonedItems;
            if (this._autoCleanAbandonedItems)
            {
                new Cleaner(this);
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        ~DME_WeakReferenceCachePool()
        {
            this._autoCleanAbandonedItems = false;
        }

        #endregion

        #region Properties

        public bool AutoCleanAbandonedItems
        {
            get
            {
                return this._autoCleanAbandonedItems;
            }
        }

        public TItem this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                if (this._cacheLock.TryEnterReadLock(LOCK_TIMEOUT_MSECS))
                {
                    try
                    {
                        WeakReference weakReference;
                        if (_cachePool.TryGetValue(key, out weakReference))
                        {
                            return (TItem)weakReference.Target;
                        }
                    }
                    finally
                    {
                        this._cacheLock.ExitReadLock();
                    }
                }

                return null;
            }
            set
            {
                this.Add(key, value);
            }
        }

        #endregion

        #region Methods

        public void Add(TKey key, TItem item)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (this._cacheLock.TryEnterWriteLock(LOCK_TIMEOUT_MSECS))
            {
                try
                {
                    _cachePool[key] = new WeakReference(item);
                }
                finally
                {
                    this._cacheLock.ExitWriteLock();
                }
            }
        }

        public void Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (this._cacheLock.TryEnterWriteLock(LOCK_TIMEOUT_MSECS))
            {
                try
                {
                    this._cachePool.Remove(key);
                }
                finally
                {
                    this._cacheLock.ExitWriteLock();
                }
            }
        }

        public void Clear()
        {
            if (this._cacheLock.TryEnterWriteLock(LOCK_TIMEOUT_MSECS))
            {
                try
                {
                    this._cachePool.Clear();
                }
                finally
                {
                    this._cacheLock.ExitWriteLock();
                }
            }
        }

        public void CleanAbandonedItems()
        {
            if (this._cacheLock.TryEnterWriteLock(LOCK_TIMEOUT_MSECS))
            {
                try
                {
                    Dictionary<TKey, WeakReference> newCachePool = new Dictionary<TKey, WeakReference>();
                    foreach (KeyValuePair<TKey, WeakReference> keyValuePair in _cachePool)
                    {
                        if (keyValuePair.Value.IsAlive)
                        {
                            newCachePool[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }

                    this._cachePool = newCachePool;
                }
                finally
                {
                    this._cacheLock.ExitWriteLock();
                }
            }
        }

        #endregion
    }
}
