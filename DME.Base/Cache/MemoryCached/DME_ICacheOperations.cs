using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemoryCached
{
    public interface DME_ICacheOperations
    {
        object Get(string key);

        /// <summary>
        /// Adds new CacheItem to cache. If another item already exists with the same key, that item is removed before
        /// the new item is added. If any failure occurs during this process, the cache will not contain the item being added. 
        /// Items added with this method have no set expiry date and can be removed by any Scavange.
        /// </summary>
        /// <param name="value">Object to add to the cache (Unknown object pointer)</param>
        void Set(string key, object value);

        void Set(string key, object value, int expirySeconds);

        /// <summary>
        /// Adds new CacheItem to cache. If another item already exists with the same key, that item is removed before
        /// the new item is added. If any failure occurs during this process, the cache will not contain the item being added. 
        /// Items added with this method have a set expiry date and can not be removed until they have expired.
        /// </summary>
        /// <param name="value">Object to add to the cache (Unknown object pointer)</param>
        /// /// <param name="expiryDate">Date of expiry</param>
        void Set(string key, object value, DateTime expiryDate);

        /// <summary>
        /// Removes the current item from the cache. If no item exists with that key, this method does nothing.
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// Revoke all objects from the Object Cache
        /// </summary>
        /// <returns>Number of objects revoked</returns>
        int RemoveAll();

        //void Scavange(double minutes);

        bool ContainsKey(string key);

        long Size { get; }
    }
}
