using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.RedisCached
{
    public class DME_RedisClientFactory
    {
        private static Dictionary<string, DME_RedisClient> instances = new Dictionary<string, DME_RedisClient>();
        private static DME_RedisClient defaultInstance = null;

        public static void Setup(string name, DME_RedisNode node, int sendTimeout)
        {
            if (instances.ContainsKey(name))
            {
                throw new Exception("Trying to configure RedisClient instance \"" + name + "\" twice.");
            }
            instances[name] = new DME_RedisClient(node.Host, node.Port, node.PassWord, sendTimeout);
        }


        public static DME_RedisClient GetInstance()
        {
            return defaultInstance ?? (defaultInstance = GetInstance("Default"));
        }

        public static DME_RedisClient GetInstance(string name)
        {
            DME_RedisClient c;
            if (instances.TryGetValue(name, out c))
            {
                return c;
            }
            else
            {
                DME_RedisConfig config = DME_RedisSection.GetConfig();

                if (config != null && config.Nodes.ContainsKey(name))
                {
                    Setup(name, config.Nodes[name], config.SendTimeout);
                    c = GetInstance(name);
                    c.IsCache = config.IsCache;
                    c.SelectDB(config.Nodes[name].DefaultDB);
                    return c;
                }
                throw new Exception("Unable to find RedisClient instance \"" + name + "\".");
            }
        }
    }
}
