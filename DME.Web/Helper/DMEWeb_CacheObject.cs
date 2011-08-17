using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using DME.Base;
using System.Web;

namespace DME.Web.Helper
{
    /*
* .NET缓存 absoluteExpiration、slidingExpiration两个参数的疑惑
* 
* absoluteExpiration：用于设置绝对过期时间，它表示只要时间一到就过期，所以类型为System.DateTime，当给这个参数设置了一个时间时，slidingExpiration参数的值就只能为Cache.NoSlidingExpiration，否则出错；

slidingExpiration：用于设置可调过期时间，它表示当离最后访问超过某个时间段后就过期，所以类型为System.TimeSpan，当给这个参数设置了一个时间段时，absoluteExpiration的值就只能为Cache.NoAbsoluteExpiration，否则出错；

两个使用实例

Cache.Add("name", content, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), System.Web.Caching.CacheItemPriority.Normal, null);

Cache.Add("name", content, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
 */

    /// <summary>
    /// 对象缓存类
    /// </summary>
    public class DMEWeb_CacheObject
    {

        private static object lockObject = new object();
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="context">当前执行上下文</param>
        /// <param name="cacheKey">缓存名称</param>
        /// <param name="cacheDependencies">缓存依赖类型</param>
        /// <param name="absoluteExpiration">过期时间</param>
        /// <param name="slidingExpiration">用于设置可调过期时间，它表示当离最后访问超过某个时间段后就过期</param>
        /// <param name="func">要执行的委托方法</param>
        /// <returns>缓存对象</returns>
        public static object Cache(
            HttpContext context,
                string cacheKey,
                CacheDependency cacheDependencies,
                DateTime absoluteExpiration,
                TimeSpan slidingExpiration,
                Func<object> func)
        {
            var cache = context.Cache;
            var content = cache.Get(cacheKey);

            if (content == null)
            {
                lock (lockObject) //线程安全的添加缓存
                {
                    content = cache.Get(cacheKey);
                    if (content == null)
                    {
                        content = func();
                        cache.Insert(cacheKey, content, cacheDependencies, absoluteExpiration, slidingExpiration);
                    }
                }
            }
            return content;
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="context">当前执行上下文</param>
        /// <param name="chacheKey">缓存名称</param>
        /// <param name="absoluteExpiration">过期时间</param>
        /// <param name="func">要执行的方法委托</param>
        /// <returns>缓存对象</returns>
        public static object Cache(HttpContext context,
            string chacheKey,
             DateTime absoluteExpiration,
            Func<object> func)
        {
            return Cache(context, chacheKey, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, func);
        }

    }
}
