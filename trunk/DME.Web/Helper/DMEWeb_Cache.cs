using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Collections;
using System.Text.RegularExpressions;

namespace DME.Web.Helper
{
    /// <summary>
    /// 缓存管理
    /// </summary>
    public class DMEWeb_Cache
    {
        private DMEWeb_Cache() { }
        //>> Based on Factor = 5 default value
        public static readonly int DayFactor = 17280;
        public static readonly int HourFactor = 720;
        public static readonly int MinuteFactor = 12;
        public static readonly double SecondFactor = 0.2;
        private static readonly Cache _cache;
        private static int Factor = 5;
        public static void ReSetFactor(int cacheFactor)
        {
            Factor = cacheFactor;
        }

        /// <summary>
        /// Static initializer should ensure we only have to look up the current cache
        /// instance once.
        /// </summary>
        static DMEWeb_Cache()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                _cache = context.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }
        }

        /// <summary>
        /// 清空Cash对象
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            ArrayList al = new ArrayList();
            while (CacheEnum.MoveNext())
            {
                al.Add(CacheEnum.Key);
            }
            foreach (string key in al)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 根据正则表达式的模式移除Cache
        /// </summary>
        /// <param name="pattern">模式</param>
        public static void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (CacheEnum.MoveNext())
            {
                if (regex.IsMatch(CacheEnum.Key.ToString()))
                    _cache.Remove(CacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// 根据键值移除Cache
        /// </summary>
        /// <param name="key">键</param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// 把对象加载到Cache
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">对象</param>
        public static void Insert(string key, object obj)
        {
            Insert(key, obj, null, 1);
        }

        /// <summary>
        /// 把对象加载到Cache,附加缓存依赖信息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">对象</param>
        /// <param name="dep">缓存依赖</param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, dep, MinuteFactor * 3);
        }

        /// <summary>
        /// 把对象加载到Cache,附加过期时间信息
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="seconds">缓存时间(秒)</param>
        public static void Insert(string key, object obj, int seconds)
        {
            Insert(key, obj, null, seconds);
        }

        /// <summary>
        /// 把对象加载到Cache,附加过期时间信息和优先级
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="seconds">缓存时间(秒)</param>
        /// <param name="priority">优先级</param>
        public static void Insert(string key, object obj, int seconds, CacheItemPriority priority)
        {
            Insert(key, obj, null, seconds, priority);
        }

        /// <summary>
        /// 把对象加载到Cache,附加缓存依赖和过期时间(多少秒后过期)
        /// (默认优先级为Normal)
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="dep">缓存依赖</param>
        /// <param name="seconds">缓存时间(秒)</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds)
        {
            Insert(key, obj, dep, seconds, CacheItemPriority.Normal);
        }

        /// <summary>
        /// 把对象加载到Cache,附加缓存依赖和过期时间(多少秒后过期)及优先级
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="dep">缓存依赖</param>
        /// <param name="seconds">缓存时间(秒)</param>
        /// <param name="priority">优先级</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds, CacheItemPriority priority)
        {
            if (obj != null)
            {
                _cache.Insert(key, obj, dep, DateTime.Now.AddSeconds(Factor * seconds), TimeSpan.Zero, priority, null);
            }
        }

        /// <summary>
        /// 把对象加到缓存并忽略优先级
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="secondFactor">时间</param>
        public static void MicroInsert(string key, object obj, int secondFactor)
        {
            if (obj != null)
            {
                _cache.Insert(key, obj, null, DateTime.Now.AddSeconds(Factor * secondFactor), TimeSpan.Zero);
            }
        }

        /// <summary>
        /// 把对象加到缓存,并把过期时间设为最大值
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        public static void Max(string key, object obj)
        {
            Max(key, obj, null);
        }

        /// <summary>
        /// 把对象加到缓存,并把过期时间设为最大值,附加缓存依赖信息
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="dep">缓存依赖</param>
        public static void Max(string key, object obj, CacheDependency dep)
        {
            if (obj != null)
            {
                _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
            }
        }

        /// <summary>
        /// 插入持久性缓存
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        public static void Permanent(string key, object obj)
        {
            Permanent(key, obj, null);
        }

        /// <summary>
        /// 插入持久性缓存,附加缓存依赖
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="obj">对象</param>
        /// <param name="dep">缓存依赖</param>
        public static void Permanent(string key, object obj, CacheDependency dep)
        {
            if (obj != null)
            {
                _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
            }
        }

        /// <summary>
        /// 根据键获取被缓存的对象
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return _cache[key];
        }

        /// <summary>
        /// Return int of seconds * SecondFactor
        /// </summary>
        public static int SecondFactorCalculate(int seconds)
        {
            // Insert method below takes integer seconds, so we have to round any fractional values
            return Convert.ToInt32(Math.Round((double)seconds * SecondFactor));
        }
    }
}
