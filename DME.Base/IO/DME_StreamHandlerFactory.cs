using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using DME.Base.Log;

namespace DME.Base.IO
{
    /// <summary>
    /// 数据流处理器工厂
    /// </summary>
    public abstract class DME_StreamHandlerFactory : DME_IStreamHandlerFactory
    {
        #region 接口
        /// <summary>
        /// 获取处理器
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract DME_IStreamHandler GetHandler(Stream stream);
        #endregion

        #region 构造
        static DME_StreamHandlerFactory()
        {
            LoadConfig();
        }
        #endregion

        #region 工厂
        static Dictionary<String, List<DME_IStreamHandlerFactory>> maps = new Dictionary<String, List<DME_IStreamHandlerFactory>>();
        /// <summary>
        /// 注册数据流处理器工厂。
        /// 数据流到达时将进入指定通道的每一个工厂，直到工厂可以返回数据流处理器为止。
        /// 不同通道名称的工厂互不干扰。
        /// </summary>
        /// <param name="name">通道名称，用于区分数据流总线</param>
        /// <param name="factory"></param>
        public static void RegisterFactory(String name, DME_IStreamHandlerFactory factory)
        {
            lock (maps)
            {
                //if (!maps.Contains(factory)) maps.Add(factory);

                if (!maps.ContainsKey(name)) maps.Add(name, new List<DME_IStreamHandlerFactory>());
                List<DME_IStreamHandlerFactory> list = maps[name];

                // 相同实例或者相同工厂类只能有一个
                foreach (DME_IStreamHandlerFactory item in list)
                {
                    if (item == factory || item.GetType() == factory.GetType()) return;
                }

                list.Add(factory);
            }
        }
        #endregion

        #region 配置
        const String factoryKey = "DME.StreamHandlerFactory_";

        /// <summary>
        /// 获取配置文件指定的工厂
        /// </summary>
        /// <returns></returns>
        static Dictionary<String, Type[]> GetFactory()
        {
            IDictionary<string, string> nvcs = DME_LibraryConfig.DMEDb_StreamHandlerFactoryConfig;
            if (nvcs == null || nvcs.Count < 1) return null;

            Dictionary<String, Type[]> dic = new Dictionary<String, Type[]>();
            // 遍历设置项
            foreach (KeyValuePair<string, string> appName in nvcs)
            {
                // 必须以指定名称开始
                if (!appName.Key.StartsWith(factoryKey, StringComparison.Ordinal)) continue;

                // 总线通道名称
                String name = appName.Key.Substring(factoryKey.Length + 1);

                String str = appName.Value;
                if (String.IsNullOrEmpty(str)) continue;

                String[] ss = str.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                List<Type> list = new List<Type>();
                foreach (String item in ss)
                {
                    Type type = Type.GetType(item);
                    list.Add(type);
                }

                dic.Add(name, list.ToArray());
            }
            return dic.Count > 0 ? dic : null; ;
        }

        /// <summary>
        /// 从配置文件中加载工厂
        /// </summary>
        static void LoadConfig()
        {
            try
            {
                Dictionary<String, Type[]> ts = GetFactory();
                if (ts == null || ts.Count < 1) return;

                foreach (String item in ts.Keys)
                {
                    foreach (Type type in ts[item])
                    {
                        DME_IStreamHandlerFactory factory = Activator.CreateInstance(type) as DME_IStreamHandlerFactory;
                        RegisterFactory(item, factory);
                    }
                }
            }
            catch (Exception ex)
            {
                
                DME_Log.WriteLine("从配置文件加载数据流工厂出错！" + ex.ToString());
            }
        }
        #endregion

        #region 处理数据流
        /// <summary>
        /// 处理数据流。Http、Tcp、Udp等所有数据流都将到达这里，多种传输方式汇聚于此，由数据流总线统一处理！
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        public static void Process(String name, Stream stream)
        {
            if (maps == null || maps.Count < 1) return;

            DME_IStreamHandler handler = null;
            DME_IStreamHandlerFactory[] fs = maps[name].ToArray();
            // 倒序遍历工厂，后来者优先
            for (int i = fs.Length - 1; i >= 0; i--)
            {
                // 把数据流分给每一个工厂，看看谁有能力处理数据流，有能力者返回数据流处理器
                handler = fs[i].GetHandler(stream);
                if (handler != null) break;
            }
            if (handler == null) return;

            // 由该处理器处理数据流
            handler.Process(stream);
        }
        #endregion
    }
}
