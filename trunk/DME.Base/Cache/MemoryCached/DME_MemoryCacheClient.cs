using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemoryCached
{
    public class DME_MemoryCacheClient
    {
        private static Dictionary<string, DME_MemoryCache> instances = new Dictionary<string, DME_MemoryCache>();
        private static DME_MemoryCache defaultInstance = null;

        public static void Setup(string name, string keyPrefix)
        {
            if (instances.ContainsKey(name))
            {
                throw new Exception("Trying to configure MemoryCachedClient instance \"" + name + "\" twice.");
            }
            instances[name] = new DME_MemoryCache(keyPrefix);
        }


        public static DME_MemoryCache GetInstance()
        {
            return defaultInstance ?? (defaultInstance = GetInstance("Default"));
        }

        public static DME_MemoryCache GetInstance(string name)
        {
            DME_MemoryCache c;
            if (instances.TryGetValue(name, out c))
            {
                return c;
            }
            else
            {
                DME_MemoryCachedConfig config = DME_MemoryCachedSection.GetConfig();

                if (config != null && config.Nodes.ContainsKey(name))
                {
                    Setup(name, config.Nodes[name].KeyPrefix);
                    c = GetInstance(name);
                    c.IsCache = config.IsCache;
                    c.IntervalMinutes = config.IntervalMinutes;
                    c.ScavangeMinutes = config.ScavangeMinutes;
                    c.MaxCount = config.MaxCount;
                    c.MaxSize = config.MaxSize * 1024;
                    return c;
                }
                throw new Exception("Unable to find MemoryCachedClient instance \"" + name + "\".");
            }
        }
    }
}
