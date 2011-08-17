using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    /// <summary>
    /// 变量函数委托
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object DMEWeb_VariableFunction(object args);

    /// <summary>
    /// 变量函数集合
    /// </summary>
    public class DMEWeb_VariableFunctionCollection : Dictionary<string, DMEWeb_VariableFunction>
    {
        /// <summary>
        /// 
        /// </summary>
        public DMEWeb_VariableFunctionCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
            
        }

        /// <summary>
        /// 添加与方法名同名的变量函数
        /// </summary>
        /// <param name="function"></param>
        public void Add(DMEWeb_VariableFunction function)
        {
            if (function != null)
                this.Add(function.Method.Name, function);
        }
    }
}
