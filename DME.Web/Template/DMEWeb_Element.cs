
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DME.Web.Template
{
    /// <summary>
    /// 元素
    /// </summary>
    public abstract class DMEWeb_Element
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate">宿主模板</param>
        protected DMEWeb_Element(DMEWeb_Template ownerTemplate)
        {
            this.OwnerTemplate = ownerTemplate;
        }

        /// <summary>
        /// 此元素的宿主模板
        /// </summary>
        public DMEWeb_Template OwnerTemplate { get; protected set; }

        /// <summary>
        /// 此元素的宿主模板文档
        /// </summary>
        public virtual DMEWeb_TemplateDocument OwnerDocument
        {
            get
            {
                return this.OwnerTemplate == null ? null : this.OwnerTemplate.OwnerDocument;
            }
        }
        /// <summary>
        /// 此元素的父级标签
        /// </summary>
        public DMEWeb_Tag Parent { get; internal set; }

        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Render(TextWriter writer);
        /// <summary>
        /// 克隆元素
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        internal abstract DMEWeb_Element Clone(DMEWeb_Template ownerTemplate);
    }
}
