using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Net;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Web.Services;
using System.Web.Services.Description;

namespace DME.Web.Common
{
    /// <summary>
    /// WebService工具类
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DMEWeb_WebService
    {
        #region 私有变量
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        #endregion

        #region 私有函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wsUrl"></param>
        /// <returns></returns>
        private string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        } 

        private IDictionary<string, Type> WSProxyTypeDictionary = new Dictionary<string, Type>();

        /// <summary> 获取目标Web服务对应的代理类型</summary>
        /// <param name="wsUrl">目标Web服务的url</param>
        /// <param name="classname">Web服务的class名称，如果不需要指定，则传入null</param>      
        public Type GetWsProxyType(string wsUrl, string classname)
        {
            string @namespace = "ESBasic.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = GetWsClassName(wsUrl);
            }
            string cacheKey = wsUrl + "@" + classname;
            if (WSProxyTypeDictionary.ContainsKey(cacheKey))
            {
                return WSProxyTypeDictionary[cacheKey];
            }


            //获取WSDL
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(wsUrl + "?WSDL");
            ServiceDescription sd = ServiceDescription.Read(stream);
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(@namespace);

            //生成客户端代理类代码
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();
            ICodeCompiler icc = csc.CreateCompiler();

            //设定编译参数
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");

            //编译代理类
            CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
            if (true == cr.Errors.HasErrors)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }

            //生成代理实例，并调用方法
            System.Reflection.Assembly assembly = cr.CompiledAssembly;
            Type wsProxyType = assembly.GetType(@namespace + "." + classname, true, true);

            lock (WSProxyTypeDictionary)
            {
                if (!WSProxyTypeDictionary.ContainsKey(cacheKey))
                {
                    WSProxyTypeDictionary.Add(cacheKey, wsProxyType);
                }
            }
            return wsProxyType;
        }
        #endregion

        #region 公开函数
        /// <summary>
        /// InvokeWebService 动态调用web服务
        /// </summary>
        /// <param name="wsUrl">WebService 地址</param>
        /// <param name="methodname">方法名</param>
        /// <param name="args">参数，仅仅支持简单类型</param>		
        public object InvokeWebService(string wsUrl, string methodname, object[] args)
        {
            return InvokeWebService(wsUrl, null, methodname, args);
        }

        /// <summary>
        /// InvokeWebService 动态调用web服务
        /// </summary>
        public object InvokeWebService(string wsUrl, string classname, string methodname, object[] args)
        {
            try
            {
                Type wsProxyType = GetWsProxyType(wsUrl, classname);
                object obj = Activator.CreateInstance(wsProxyType);
                MethodInfo mi = wsProxyType.GetMethod(methodname);

                return mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
