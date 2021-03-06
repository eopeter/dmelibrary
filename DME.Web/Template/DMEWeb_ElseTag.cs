﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DME.Web.Template
{
    /// <summary>
    /// Else标签..只适用于if标签内.如&lt;vt:if var="member.age" value="20" compare="&lt;="&gt;..&lt;vt:else&gt;..&lt;/vt:if&gt;
    /// </summary>
    public class DMEWeb_ElseTag : DMEWeb_IfConditionTag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal DMEWeb_ElseTag(DMEWeb_Template ownerTemplate)
            : base(ownerTemplate)
        {}

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "else"; }
        }

        /// <summary>
        /// 返回此标签是否是单一标签.即是不需要配对的结束标签
        /// </summary>
        internal override bool IsSingleTag
        {
            get { return true; }
        }
        #endregion
        
        #region 重写IfConditionTag方法
        /// <summary>
        /// else节点不支持比较值
        /// </summary>
        public override DMEWeb_ElementCollection<DMEWeb_IExpression> Values
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// else节点不支持条件变量
        /// </summary>
        public override DMEWeb_VariableExpression VarExpression
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 永远返回true
        /// </summary>
        /// <returns></returns>
        internal override bool IsTestSuccess()
        {
            return true;
        }
        #endregion

        #region 解析标签数据
        /// <summary>
        /// 解析标签数据
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
            //判断标签的容器是否为IfTag标签
            if (!(container is DMEWeb_IfTag)) throw new DMEWeb_ParserException(string.Format("未找到和{0}标签对应的{1}标签", this.TagName, this.EndTagName));

            DMEWeb_IfTag ifTag = (DMEWeb_IfTag)container;
            if (ifTag.Else != null) throw new DMEWeb_ParserException(string.Format("{0}标签不能定义多个{1}标签", this.EndTagName, this.TagName));

            //加入到If标签的Else节点
            ifTag.Else = this;

            return true;
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
            DMEWeb_ElseTag tag = new DMEWeb_ElseTag(ownerTemplate);
            this.CopyTo((DMEWeb_IfConditionTag)tag);
            return tag;
        }
        #endregion
    }
}
