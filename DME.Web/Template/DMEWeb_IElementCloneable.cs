using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    /// <summary>
    /// 支持元素的深度克隆的接口定义
    /// </summary>
    public interface DMEWeb_ICloneableElement<T>
    {
        /// <summary>
        /// 克隆元素
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        T Clone(DMEWeb_Template ownerTemplate);
    }
}
