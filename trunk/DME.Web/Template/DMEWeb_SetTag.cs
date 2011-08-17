using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DME.Web.Template
{
    /// <summary>
    /// 变量赋值标签, 如:&lt;vt:set var="page" value="1" /&gt;
    /// </summary>
    public class DMEWeb_SetTag : DMEWeb_Tag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal DMEWeb_SetTag(DMEWeb_Template ownerTemplate)
            : base(ownerTemplate)
        {
            this.Values = new DMEWeb_ElementCollection<DMEWeb_IExpression>();
        }

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "set"; }
        }
        /// <summary>
        /// 返回此标签是否是单一标签.即是不需要配对的结束标签
        /// </summary>
        internal override bool IsSingleTag
        {
            get { return false; }
        }
        #endregion

        #region 属性定义
        /// <summary>
        /// 变量的值
        /// </summary>
        public DMEWeb_ElementCollection<DMEWeb_IExpression> Values { get; protected set; }

        /// <summary>
        /// 要对其赋值的变量
        /// </summary>
        public DMEWeb_VariableIdentity Variable { get; protected set; }

        /// <summary>
        /// 格式化
        /// </summary>
        public DMEWeb_Attribute Format
        {
            get
            {
                return this.Attributes["Format"];
            }
        }

        /// <summary>
        /// 是否输出此标签的结果值
        /// </summary>
        public bool Output { get; protected set; }
        #endregion

        #region 添加标签属性时的触发函数.用于设置自身的某些属性值
        /// <summary>
        /// 添加标签属性时的触发函数.用于设置自身的某些属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        protected override void OnAddingAttribute(string name, DMEWeb_Attribute item)
        {
            switch (name)
            {
                case "value":
                    this.Values.Add(item.Value);
                    break;
                case "var":
                    this.Variable = DMEWeb_ParserHelper.CreateVariableIdentity(this.OwnerTemplate, item.Text);
                    break;
                case "output":
                    this.Output = DMEWeb_Utility.ConverToBoolean(item.Text);
                    break;
            }
        }
        #endregion

        #region 呈现本元素的数据
        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderTagData(System.IO.TextWriter writer)
        {
            object value = null;
            string format = this.Format == null ? string.Empty : this.Format.GetTextValue();
            if (string.IsNullOrEmpty(format))
            {
                value = this.Values[0].GetValue();
            }
            else
            {
                List<object> param = new List<object>();
                foreach (DMEWeb_IExpression ie in this.Values)
                {
                    param.Add(ie.GetValue());
                }
                value = string.Format(format, param.ToArray());
            }
            if (this.Variable != null) this.Variable.Value = value;

            if (this.Output && value != null) writer.Write(value);
            base.RenderTagData(writer);
        }
        #endregion

        #region 开始解析标签数据
        /// <summary>
        /// 开始解析标签数据
        /// </summary>
        /// <param name="ownerTemplate">宿主模板</param>
        /// <param name="container">标签的容器</param>
        /// <param name="tagStack">标签堆栈</param>
        /// <param name="text"></param>
        /// <param name="match"></param>
        /// <param name="isClosedTag">是否闭合标签</param>
        /// <returns>如果需要继续处理EndTag则返回true.否则请返回false</returns>
        internal override bool ProcessBeginTag(DMEWeb_Template ownerTemplate, DMEWeb_Tag container, Stack<DMEWeb_Tag> tagStack, string text, ref Match match, bool isClosedTag)
        {
            if (this.Variable == null && !this.Output) throw new DMEWeb_ParserException(string.Format("{0}标签中如果未定义Output属性为true则必须定义var属性", this.TagName));
            if (this.Values.Count < 1) throw new DMEWeb_ParserException(string.Format("{0}标签中缺少value属性", this.TagName));
            if (this.Values.Count > 1 && this.Format == null) throw new DMEWeb_ParserException(string.Format("{0}标签如果已定义多个value属性,则也必须定义format属性", this.TagName));

            return base.ProcessBeginTag(ownerTemplate, container, tagStack, text, ref match, isClosedTag);
        }
        #endregion

        #region 克隆当前元素到新的宿主模板
        /// <summary>
        /// 克隆当前元素到新的宿主模板
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        internal override DMEWeb_Element Clone(DMEWeb_Template ownerTemplate)
        {
            DMEWeb_SetTag tag = new DMEWeb_SetTag(ownerTemplate);
            this.CopyTo(tag);
            tag.Variable = this.Variable == null ? null : this.Variable.Clone(ownerTemplate);
            tag.Output = this.Output;
            foreach (DMEWeb_IExpression exp in this.Values)
            {
                tag.Values.Add((DMEWeb_IExpression)(exp.Clone(ownerTemplate)));
            }
            return tag;
        }
        #endregion
    }
}
