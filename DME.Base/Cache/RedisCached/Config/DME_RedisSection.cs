using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.RedisCached
{
    public class DME_RedisSection
    {
        public static DME_RedisConfig GetConfig()
        {
            return ParseConfig(DME_LibraryConfig.DMEDb_RedisCachedConfig);
        }
        private static DME_RedisConfig ParseConfig(Dictionary<string, string> section)
        {
            DME_RedisConfig config = new DME_RedisConfig();
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
                        if (dic.ContainsKey("SendTimeout"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["SendTimeout"]))
                            {
                                config.SendTimeout = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["SendTimeout"]);
                            }
                        }
                    }
                    else
                    {
                        DME_RedisNode node = new DME_RedisNode();
                        node.Name = item.Key;
                        if (dic.ContainsKey("Host"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsIPv4(dic["Host"]))
                            {
                                node.Host = dic["Host"];
                            }                           
                        }
                        if (dic.ContainsKey("Port"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["Port"]))
                            {
                                node.Port = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["Port"]);
                            }
                        }
                        if (dic.ContainsKey("PassWord"))
                        {
                            node.PassWord = dic["PassWord"];
                        }
                        if (dic.ContainsKey("DefaultDB"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["DefaultDB"]))
                            {
                                node.DefaultDB = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["DefaultDB"]);
                            }
                        }
                        config.Nodes.Add(node.Name, node);
                    }
                }

            }
            return config;
        }
    }
}
