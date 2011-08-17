using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemoryCached
{
    public class DME_MemoryCachedSection
    {
        public static DME_MemoryCachedConfig GetConfig()
        {
            return ParseConfig(DME_LibraryConfig.DMEDb_MemoryCachedConfig);
        }
        private static DME_MemoryCachedConfig ParseConfig(Dictionary<string, string> section)
        {
            DME_MemoryCachedConfig config = new DME_MemoryCachedConfig();
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
                        if (dic.ContainsKey("IntervalMinutes"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["IntervalMinutes"]))
                            {
                                config.IntervalMinutes = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["IntervalMinutes"]);
                            }
                        }
                        if (dic.ContainsKey("ScavangeMinutes"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumeric(dic["ScavangeMinutes"]))
                            {
                                config.ScavangeMinutes = DME.Base.Helper.DME_TypeParse.StringToInt32(dic["ScavangeMinutes"]);
                            }
                        }
                        if (dic.ContainsKey("MaxCount"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumber(dic["MaxCount"]))
                            {
                                config.MaxCount = DME.Base.Helper.DME_TypeParse.StringToInt64(dic["MaxCount"]);
                            }
                        }
                        if (dic.ContainsKey("MaxSize"))
                        {
                            if (DME.Base.Helper.DME_Validation.IsNumber(dic["MaxSize"]))
                            {
                                string[] maxSizes = dic["MaxSize"].Split('*');
                                config.MaxSize = 1;
                                foreach (string max in maxSizes)
                                {
                                    config.MaxSize *= DME.Base.Helper.DME_TypeParse.StringToInt64(max);
                                }
                            }
                        }
                    }
                    else
                    {
                        DME_MemoryCachedNode node = new DME_MemoryCachedNode();
                        node.Name = item.Key;
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
