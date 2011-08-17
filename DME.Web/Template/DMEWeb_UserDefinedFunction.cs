using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    /// <summary>
    /// 自定义函数委托
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object DMEWeb_UserDefinedFunction(object[] args);

    /// <summary>
    /// 自定义函数集合
    /// </summary>
    public class DMEWeb_UserDefinedFunctionCollection
        : Dictionary<string, DMEWeb_UserDefinedFunction>
    {
        /// <summary>
        /// 
        /// </summary>
        public DMEWeb_UserDefinedFunctionCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {

        }

        /// <summary>
        /// 添加与方法名同名的用户自定义函数
        /// </summary>
        /// <param name="function"></param>
        public void Add(DMEWeb_UserDefinedFunction function)
        {
            if (function != null)
                this.Add(function.Method.Name, function);
        }

        /// <summary>
        /// 添加某个用户自定义函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="function"></param>
        /// <remarks>重写此函数主要是为便于可重复添加多次同名的自定义函数(但只有最后一次有效)</remarks>
        public new void Add(string key, DMEWeb_UserDefinedFunction function)
        {
            lock (this)
            {
                if (base.ContainsKey(key))
                {
                    base[key] = function;
                }
                else
                {
                    base.Add(key, function);
                }
            }
        }
    }
}
