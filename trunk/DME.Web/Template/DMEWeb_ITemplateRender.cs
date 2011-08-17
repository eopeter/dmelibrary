using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    /// <summary>
    /// 模板块数据的解析处理接口
    /// </summary>
    public interface DMEWeb_ITemplateRender
    {
        /// <summary>
        /// 预处理解析模板数据
        /// </summary>
        /// <param name="template"></param>
        void PreRender(DMEWeb_Template template);
    }

    /// <summary>
    /// 预处理模板数据的方法属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
    public class DMEWeb_TemplateRenderMethodAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public DMEWeb_TemplateRenderMethodAttribute() { }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
