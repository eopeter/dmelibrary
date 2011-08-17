using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.FileCached
{
    public class DME_FileCacheClient
    {
        private static Dictionary<string, DME_FileCache> instances = new Dictionary<string, DME_FileCache>();
        private static DME_FileCache defaultInstance = null;

        public static void Setup(string name, string fileCachePath)
        {
            if (instances.ContainsKey(name))
            {
                throw new Exception("Trying to configure FileCachedClient instance \"" + name + "\" twice.");
            }
            instances[name] = new DME_FileCache(fileCachePath);
        }


        public static DME_FileCache GetInstance()
        {
            return defaultInstance ?? (defaultInstance = GetInstance("Default"));
        }

        public static DME_FileCache GetInstance(string name)
        {
            DME_FileCache c;
            if (instances.TryGetValue(name, out c))
            {
                return c;
            }
            else
            {
                DME_FileCachedConfig config = DME_FileCachedSection.GetConfig();

                if (config != null && config.Nodes.ContainsKey(name))
                {
                    Setup(name, config.Nodes[name].FileCachePath);
                    c = GetInstance(name);
                    c.IsCache = config.IsCache;
                    c.KeyPrefix = config.Nodes[name].KeyPrefix;
                    c.CacheSubDirs = config.Nodes[name].CacheSubDirs;
                    return c;
                }
                throw new Exception("Unable to find FileCachedClient instance \"" + name + "\".");
            }
        }
    }
}
