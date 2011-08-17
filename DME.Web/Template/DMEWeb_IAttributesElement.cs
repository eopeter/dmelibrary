using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    
    /// <summary>
    /// 定义可包含属性的元素
    /// </summary>
    public interface DMEWeb_IAttributesElement
    {
        /// <summary>
        /// 返回元素属性集合
        /// </summary>
        DMEWeb_AttributeCollection Attributes { get; }
    }
}
