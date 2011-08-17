using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DME.Base.Cache.FileCached
{
    /// <summary>
    /// 
    /// </summary>
    public class DME_FileCachedSection
    {
        public static DME_FileCachedConfig GetConfig()
        {
            return ParseConfig(DME_LibraryConfig.DME_FileCachedConfig);
        }
        private static DME_FileCachedConfig ParseConfig(Dictionary<string, string> section)
        {
            DME_FileCachedConfig config = new DME_FileCachedConfig();
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
                    }
                    else
                    {
                        DME_FileCachedNode node = new DME_FileCachedNode();
                        node.Name = item.Key;
                        if (dic.ContainsKey("FileCachePath"))
                        {
                            node.FileCachePath = dic["FileCachePath"];
                        }
                        if (dic.ContainsKey("KeyPrefix"))
                        {
                            node.KeyPrefix = dic["KeyPrefix"];
                        }
                        if (dic.ContainsKey("CacheSubDirs"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["CacheSubDirs"]))
                            {

                                node.CacheSubDirs = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["CacheSubDirs"]); ;
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
