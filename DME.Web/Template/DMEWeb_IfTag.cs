﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DME.Web.Template
{
    /// <summary>
    /// If条件标签,如: &lt;vt:if var="member.age" value="20" compare="&lt;="&gt;..&lt;vt:elseif value="30"&gt;..&lt;/vt:if&gt;
    /// </summary>
    public class DMEWeb_IfTag : DMEWeb_IfConditionTag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal DMEWeb_IfTag(DMEWeb_Template ownerTemplate)
            : base(ownerTemplate)
        {
            this.ElseIfs = new DMEWeb_ElementCollection<DMEWeb_IfConditionTag>();
        }

        #region 属性定义
        /// <summary>
        /// ElseIf节点列表
        /// </summary>
        public DMEWeb_ElementCollection<DMEWeb_IfConditionTag> ElseIfs { get; protected set; }

        /// <summary>
        /// Else节点
        /// </summary>
        private DMEWeb_ElseTag _Else;
        /// <summary>
        /// Else节点
        /// </summary>
        public DMEWeb_ElseTag Else
        {
            get { return _Else; }
            internal set
            {
                if (value != null) value.Parent = this;
                _Else = value;
            }
        }
        #endregion

        #region 方法定义
        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="conditionTag"></param>
        internal virtual void AddElseCondition(DMEWeb_IfConditionTag conditionTag)
        {
            conditionTag.Parent = this;
            this.ElseIfs.Add(conditionTag);
        }
        #endregion

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "if"; }
        }

        /// <summary>
        /// 返回此标签是否是单一标签.即是不需要配对的结束标签
        /// </summary>
        internal override bool IsSingleTag
        {
            get { return false; }
        }

        /// <summary>
        /// 根据Id获取某个子元素标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override DMEWeb_Tag GetChildTagById(string id)
        {
            DMEWeb_Tag tag = base.GetChildTagById(id);

            //如果在自身元素里找不到.则从ElseIf和Else标签里找
            if (tag == null)
            {
                foreach (DMEWeb_IfConditionTag elseif in this.ElseIfs)
                {
                    if (id.Equals(elseif.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tag = elseif;
                    }
                    else
                    {
                        tag = elseif.GetChildTagById(id);                        
                    }
                    if (tag != null)break;
                }

                if (tag == null && this.Else != null)
                {
                    if (id.Equals(this.Else.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tag = this.Else;
                    }
                    else
                    {
                        tag = this.Else.GetChildTagById(id);
                    }
                }
            }

            return tag;
        }

        /// <summary>
        /// 根据name获取所有同名的子元素标签
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override DMEWeb_ElementCollection<DMEWeb_Tag> GetChildTagsByName(string name)
        {
            DMEWeb_ElementCollection<DMEWeb_Tag> tags = base.GetChildTagsByName(name);
            //处理ElseIf标签列表
            foreach (DMEWeb_IfConditionTag elseif in this.ElseIfs)
            {
                if (name.Equals(elseif.Name, StringComparison.InvariantCultureIgnoreCase)) tags.Add(elseif);
                tags.AddRange(elseif.GetChildTagsByName(name));
            }
            //处理Else标签
            if (this.Else != null)
            {
                if (name.Equals(this.Else.Name, StringComparison.InvariantCultureIgnoreCase)) tags.Add(this.Else);
                tags.AddRange(this.Else.GetChildTagsByName(name));
            }
            return tags;
        }

        /// <summary>
        /// 根据标签名获取所有同标签名的子元素标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public override DMEWeb_ElementCollection<DMEWeb_Tag> GetChildTagsByTagName(string tagName)
        {
            DMEWeb_ElementCollection<DMEWeb_Tag> tags = base.GetChildTagsByTagName(tagName);
            //处理ElseIf标签列表
            foreach (DMEWeb_IfConditionTag elseif in this.ElseIfs)
            {
                if (tagName.Equals(elseif.TagName, StringComparison.InvariantCultureIgnoreCase)) tags.Add(elseif);
                tags.AddRange(elseif.GetChildTagsByTagName(tagName));
            }
            //处理Else标签
            if (this.Else != null)
            {
                if (tagName.Equals(this.Else.TagName, StringComparison.InvariantCultureIgnoreCase)) tags.Add(this.Else);
                tags.AddRange(this.Else.GetChildTagsByTagName(tagName));
            }
            return tags;
        }
        #endregion

        #region 呈现本元素的数据
        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderTagData(System.IO.TextWriter writer)
        {
            if (this.IsTestSuccess())
            {
                //优先测试自身条件
                base.RenderTagData(writer);
            }
            else
            {
                //其它其它条件节点
                bool flag = false;
                foreach (DMEWeb_IfConditionTag condition in this.ElseIfs)
                {
                    if (condition.IsTestSuccess())
                    {
                        condition.Render(writer);
                        flag = true;
                        break;
                    }
                }

                //如果其它条件节点都不符合.则输出else节点数据
                if (!flag && this.Else != null) this.Else.Render(writer);
            }
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
            if (this.VarExpression == null) throw new DMEWeb_ParserException(string.Format("{0}标签中缺少var属性", this.TagName));
            if (this.Values.Count == 0) throw new DMEWeb_ParserException(string.Format("{0}标签中缺少value属性", this.TagName));

            //闭合标签则不进行数据处理
            if (!isClosedTag)
            {
                container.AppendChild(this);
            }
            return !isClosedTag;
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
            DMEWeb_IfTag tag = new DMEWeb_IfTag(ownerTemplate);
            this.CopyTo((DMEWeb_IfConditionTag)tag);
            tag.Else = this.Else == null ? null : (DMEWeb_ElseTag)(this.Else.Clone(ownerTemplate));

            foreach (DMEWeb_IfConditionTag elseTag in this.ElseIfs)
            {
                tag.AddElseCondition((DMEWeb_IfConditionTag)elseTag.Clone(ownerTemplate));
            }
            return tag;
        }
        #endregion
    }
}
