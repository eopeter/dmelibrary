using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    /// <summary>
    /// 表达式接口
    /// </summary>
    public interface DMEWeb_IExpression : DMEWeb_ICloneableElement<DMEWeb_IExpression>
    {
        /// <summary>
        /// 获取表达式的值
        /// </summary>
        /// <returns></returns>
        object GetValue();
    }
}
