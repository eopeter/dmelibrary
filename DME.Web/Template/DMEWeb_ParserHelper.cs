﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DME.Web.Template
{
    /// <summary>
    /// 解析器的帮助类
    /// </summary>
    internal static class DMEWeb_ParserHelper
    {
        #region 解析判断函数块
        /// <summary>
        /// 注释标签的起始标记
        /// </summary>
        public const string CommentTagStart = "<!--vt[";
        /// <summary>
        /// 注释标签的结束标记
        /// </summary>
        public const string CommentTagEnd = "]-->";
        /// <summary>
        /// VT表达头的标记
        /// </summary>
        public const string VTExpressionHead = "vt=";

        /// <summary>
        /// 读取某个偏移位置的字符.如果超出则返回特殊字符"\0x0"
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static char ReadChar(string text, int offset)
        {
            if (offset < text.Length) return text[offset];
            return (char)0;
        }
        /// <summary>
        /// 判断c是否是c1,c2中的一个
        /// </summary>
        /// <param name="c"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private static bool IsChars(char c, char c1, char c2)
        {
            return c == c1 || c == c2;
        }
        /// <summary>
        /// 判断是否是变量标签的开始
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static bool IsVariableTagStart(string text, int offset)
        {
            return (ReadChar(text, offset) == '{' && ReadChar(text, offset + 1) == '$' && ReadChar(text, offset + 2) == ':');
        }
        /// <summary>
        /// 判断是否是某种标签的开始
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static bool IsTagStart(string text, int offset)
        {
            return (ReadChar(text, offset) == '<' &&
                IsChars(ReadChar(text, offset + 1), 'v', 'V') &&
                IsChars(ReadChar(text, offset + 2), 't', 'T') && 
                ReadChar(text, offset + 3) == ':');
        }
        /// <summary>
        /// 判断是否是某种结束标签的开始
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static bool IsCloseTagStart(string text, int offset)
        {
            return (ReadChar(text, offset) == '<' && 
                ReadChar(text, offset + 1) == '/' &&
                IsChars(ReadChar(text, offset + 2), 'v', 'V') &&
                IsChars(ReadChar(text, offset + 3), 't', 'T') && 
                ReadChar(text, offset + 4) == ':');
        }

        /// <summary>
        /// 判断是否是VT表达式的开始. vt="&lt;vt: 或者 vt='&lt;vt:
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static bool IsVTExpressionStart(string text, int offset)
        {
            return (IsChars(ReadChar(text, offset), 'v', 'V') &&
                    IsChars(ReadChar(text, offset + 1), 't', 'T') &&
                    ReadChar(text, offset + 2) == '=' &&
                    IsChars(ReadChar(text, offset + 3), '"', '\'') &&
                    IsTagStart(text, offset + 4));
        }

        /// <summary>
        /// 判断是否是注解标签的开始.注解标签的定义: &lt;!--vt[.....]--&gt;
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static bool IsCommentTagStart(string text, int offset)
        {
            return (ReadChar(text, offset) == '<' && 
                ReadChar(text, offset + 1) == '!' && 
                ReadChar(text, offset + 2) == '-' &&
                ReadChar(text, offset + 3) == '-' &&
                IsChars(ReadChar(text, offset + 4), 'v', 'V') &&
                IsChars(ReadChar(text, offset + 5), 't', 'T') && 
                ReadChar(text, offset + 6) == '[');
        }
        #endregion

        /// <summary>
        /// 解析元素的属性列表
        /// </summary>
        /// <param name="element"></param>
        /// <param name="match"></param>
        internal static void ParseElementAttributes(DMEWeb_IAttributesElement element, Match match)
        {
            //处理属性
            CaptureCollection attrNames = match.Groups["attrname"].Captures;
            CaptureCollection attrVals = match.Groups["attrval"].Captures;

            for (int i = 0; i < attrNames.Count; i++)
            {
                string attrName = attrNames[i].Value;
                string attrVal = HttpUtility.HtmlDecode(attrVals[i].ToString());

                //加入属性列表
                if (!string.IsNullOrEmpty(attrName))
                    element.Attributes.Add(attrName, attrVal);
            }
        }

        /// <summary>
        /// 构建文本节点元素
        /// </summary>
        /// <param name="ownerTemplate">宿主模板</param>
        /// <param name="container">标签的容器</param>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        internal static void CreateTextNode(DMEWeb_Template ownerTemplate, DMEWeb_Tag container, string text, int offset, int length)
        {
            if (length > 0)
            {
                string content = text.Substring(offset, length);
                if (ownerTemplate.OwnerDocument.DocumentConfig != null
                    && ownerTemplate.OwnerDocument.DocumentConfig.CompressText)
                {
                    //压缩文本
                    content = DMEWeb_Utility.CompressText(content);
                }                
                if (content.Length > 0)
                {
                    //不建立空行文本节点
                    if(content.TrimStart('\r','\n','\t').Length != 0)
                        container.AppendChild(new DMEWeb_TextNode(ownerTemplate, content));
                }
            }
        }

        /// <summary>
        /// 从匹配项中建构建变量实例
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="match"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        internal static DMEWeb_Variable CreateVariable(DMEWeb_Template ownerTemplate, Match match, out string prefix)
        {
            prefix = match.Groups["prefix"].Success ? match.Groups["prefix"].Value : null;
            string name = match.Groups["name"].Value;

            ownerTemplate = DMEWeb_Utility.GetOwnerTemplateByPrefix(ownerTemplate, prefix);
            if (ownerTemplate == null) throw new DMEWeb_ParserException(string.Format("变量的宿主模板#{0}不存在", prefix));

            DMEWeb_Variable variable = DMEWeb_Utility.GetVariableOrAddNew(ownerTemplate, name);
            return variable;
        }
        /// <summary>
        /// 从文本(如#.name或name)中构建变量标识对象
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static DMEWeb_VariableIdentity CreateVariableIdentity(DMEWeb_Template ownerTemplate, string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            Match match = DMEWeb_ParserRegex.VarIdRegex.Match(text);
            if (match.Success)
            {
                string prefix;
                DMEWeb_Variable variable = CreateVariable(ownerTemplate, match, out prefix);
                return new DMEWeb_VariableIdentity(ownerTemplate, variable, prefix);
            }
            else
            {
                //非变量
                throw new DMEWeb_ParserException(string.Format("变量标识\"{0}\"的定义格式错误", text));
            }
        }

        /// <summary>
        /// 构建变量的字段列表
        /// </summary>
        /// <param name="variableId"></param>
        /// <param name="match"></param>
        /// <param name="needCacheData"></param>
        /// <returns></returns>
        internal static DMEWeb_VariableExpression CreateVariableExpression(DMEWeb_VariableIdentity variableId, Match match, bool needCacheData)
        {
            //解析变量字段列表
            DMEWeb_VariableExpression field = new DMEWeb_VariableExpression(variableId, needCacheData);
            CaptureCollection fields = match.Groups["field"].Captures;
            CaptureCollection methods = match.Groups["method"].Captures;
            DMEWeb_VariableExpression item = field;
            for (var i = 0; i < fields.Count; i++)
            {
                string fieldName = fields[i].Value;
                DMEWeb_VariableExpression exp = new DMEWeb_VariableExpression(item, fieldName, !string.IsNullOrEmpty(methods[i].Value));
                item = exp;
            }

            return field;
        }

        /// <summary>
        /// 从变量表达式文本(如:name.age)中构建变量表达式
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="expressionText"></param>
        /// <param name="needCacheData"></param>
        /// <returns></returns>
        internal static DMEWeb_VariableExpression CreateVariableExpression(DMEWeb_Template ownerTemplate, string expressionText, bool needCacheData)
        {
            if (string.IsNullOrEmpty(expressionText)) return null;

            Match match = DMEWeb_ParserRegex.VarExpRegex.Match(expressionText);
            if (match.Success)
            {
                string prefix;
                DMEWeb_Variable variable = CreateVariable(ownerTemplate, match, out prefix);
                DMEWeb_VariableIdentity variableId = new DMEWeb_VariableIdentity(ownerTemplate, variable, prefix);

                return CreateVariableExpression(variableId, match, needCacheData);
            }
            else
            {
                //非变量表达式
                throw new DMEWeb_ParserException(string.Format("变量表达式\"{0}\"的定义格式错误", expressionText));
            }
        }
        /// <summary>
        /// 从表达式文本中构造表达式.如果表达式是以$字符开头.并且不是以$$字符开头.则认为是变量表达式.否则则认为是常量表达式
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="expressionText"></param>
        /// <returns></returns>
        internal static DMEWeb_IExpression CreateExpression(DMEWeb_Template ownerTemplate, string expressionText)
        {
            if (string.IsNullOrEmpty(expressionText)) return new DMEWeb_ConstantExpression(expressionText);

            if (expressionText.StartsWith("$"))
            {
                expressionText = expressionText.Remove(0, 1);
                if (expressionText.StartsWith("$"))
                {
                    //$$字符开头.则认为是常量表达式
                    return new DMEWeb_ConstantExpression(expressionText);
                }
                else
                {
                    //变量表达式
                    return CreateVariableExpression(ownerTemplate, expressionText, false);
                }
            }
            else
            {
                //常量表达式
                return DMEWeb_Utility.IsInteger(expressionText) ?
                      new DMEWeb_ConstantExpression(DMEWeb_Utility.ConverToInt32(expressionText))
                    : new DMEWeb_ConstantExpression(expressionText);
            }
        }

        /// <summary>
        /// 构建变量标签元素
        /// </summary>
        /// <param name="ownerTemplate">宿主模板</param>
        /// <param name="container">标签的容器</param>
        /// <param name="match"></param>
        internal static DMEWeb_VariableTag CreateVariableTag(DMEWeb_Template ownerTemplate, DMEWeb_Tag container, Match match)
        {
            string prefix;
            DMEWeb_Variable variable = CreateVariable(ownerTemplate, match, out prefix);
            DMEWeb_VariableIdentity variableId = new DMEWeb_VariableIdentity(ownerTemplate, variable, prefix);

            //变量标签元素则需要缓存表达式的值
            DMEWeb_VariableExpression varExp = CreateVariableExpression(variableId, match, true);

            DMEWeb_VariableTag tag = new DMEWeb_VariableTag(ownerTemplate, varExp);
            //解析属性列表
            ParseElementAttributes(tag, match);
            container.AppendChild(tag);

            return tag;
        }

        /// <summary>
        /// 构建标签元素
        /// </summary>
        /// <param name="ownerTemplate">宿主模板</param>
        /// <param name="match"></param>
        /// <param name="isClosedTag">是否是自闭合标签</param>
        /// <returns></returns>
        internal static DMEWeb_Tag CreateTag(DMEWeb_Template ownerTemplate, Match match, out bool isClosedTag)
        {
            string tagName = match.Groups["tagname"].Value;
            isClosedTag = match.Groups["closed"].Success;

            DMEWeb_Tag tag = DMEWeb_TagFactory.FromTagName(ownerTemplate, tagName);
            if (tag == null) throw new DMEWeb_ParserException(string.Format("不能识别的元素标签\"{0}\"", tagName));

            ParseElementAttributes(tag, match);

            return tag;
        }
    }
}
