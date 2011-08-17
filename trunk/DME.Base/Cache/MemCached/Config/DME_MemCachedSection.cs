using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemCached
{
    public class DME_MemCachedSection
    {
        public static DME_MemCachedConfig GetConfig()
        {
            return ParseConfig(DME_LibraryConfig.DMEDb_MemCachedConfig);
        }
        private static DME_MemCachedConfig ParseConfig(Dictionary<string, string> section)
        {
            DME_MemCachedConfig config = new DME_MemCachedConfig();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> item in section)
            {
                dic = DME.Base.Helper.DME_String.AnalyzeConfigString(item.Value);
                if (!DME.Base.Helper.DME_Validation.IsNull(dic))
                {
                    if (item.Key == "setting")
                    {
                        if (dic.ContainsKey("IsCache"))
                        {
                            if (!DME.Base.Helper.DME_Validation.IsNull(dic["IsCache"]))
                            {
                                config.IsCache = DME.Base.Helper.DME_TypeParse.StringToBoolean(dic["IsCache"]);
                            }
                        }
                        if (dic.ContainsKey("SendReceiveTimeout"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["SendReceiveTimeout"]))
                            {
                                config.SendReceiveTimeout = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["SendReceiveTimeout"]);
                            }
                        }
                        if (dic.ContainsKey("MinPoolSize"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["MinPoolSize"]))
                            {
                                config.MinPoolSize = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["MinPoolSize"]);
                            }
                        }
                        if (dic.ContainsKey("MaxPoolSize"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumber(dic["MaxPoolSize"]))
                            {
                                config.MaxPoolSize = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["MaxPoolSize"]);
                            }
                        }
                    }
                    else
                    {
                        DME_MemCachedNode node = new DME_MemCachedNode();
                        node.Name = item.Key;
                        if (dic.ContainsKey("Hosts"))
                        {
                            node.Hosts = dic["Hosts"];
                        }
                        if (dic.ContainsKey("KeyPrefix"))
                        {
                            node.KeyPrefix = dic["KeyPrefix"];
                        }
                        config.Nodes.Add(node.Name, node);
                    }
                }

            }
            return config;
        }
    }
}
