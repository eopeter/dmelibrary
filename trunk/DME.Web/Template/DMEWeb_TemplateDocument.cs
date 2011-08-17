using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Runtime.Remoting.Messaging;

namespace DME.Web.Template
{
    /// <summary>
    /// 模板文档
    /// </summary>
    public class DMEWeb_TemplateDocument : DMEWeb_Template
    {

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        private DMEWeb_TemplateDocument(DMEWeb_TemplateDocumentConfig documentConfig)
        {
            this.DocumentConfig = documentConfig;
        }
        /// <summary>
        /// 采用默认的文档配置并根据TextRader数据进行解析
        /// </summary>
        /// <param name="reader"></param>
        public DMEWeb_TemplateDocument(TextReader reader) : this(reader, DMEWeb_TemplateDocumentConfig.Default) { }
        /// <summary>
        /// 根据TextRader数据进行解析
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="documentConfig"></param>
        public DMEWeb_TemplateDocument(TextReader reader, DMEWeb_TemplateDocumentConfig documentConfig)
        {
            this.DocumentConfig = documentConfig;
            this.ParseString(reader.ReadToEnd());
        }
        /// <summary>
        /// 采用默认的文档配置并根据文件内容进行解析
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="charset"></param>
        public DMEWeb_TemplateDocument(string fileName, Encoding charset) : this(fileName, charset, DMEWeb_TemplateDocumentConfig.Default) { }
        /// <summary>
        /// 根据文件内容进行解析
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="charset"></param>
        /// <param name="documentConfig"></param>
        public DMEWeb_TemplateDocument(string fileName, Encoding charset, DMEWeb_TemplateDocumentConfig documentConfig)
        {
            string text = System.IO.File.ReadAllText(fileName, charset);
            this.File = Path.GetFullPath(fileName);
            this.Charset = charset;
            this.AddFileDependency(this.File);
            this.DocumentConfig = documentConfig;
            this.ParseString(this, text);
        }
        /// <summary>
        /// 采用默认的文档配置并根据字符串进行解析
        /// </summary>
        /// <param name="text"></param>
        public DMEWeb_TemplateDocument(string text) : this(text, DMEWeb_TemplateDocumentConfig.Default) { }
        /// <summary>
        /// 根据字符串进行解析
        /// </summary>
        /// <param name="text"></param>
        /// <param name="documentConfig"></param>
        public DMEWeb_TemplateDocument(string text, DMEWeb_TemplateDocumentConfig documentConfig)
        {
            this.DocumentConfig = documentConfig;
            this.ParseString(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentElement"></param>
        /// <param name="text"></param>
        /// <param name="documentConfig"></param>
        internal DMEWeb_TemplateDocument(DMEWeb_Template documentElement, string text, DMEWeb_TemplateDocumentConfig documentConfig) : this(documentElement, documentElement, text, documentConfig) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentElement"></param>
        /// <param name="container"></param>
        /// <param name="text"></param>
        /// <param name="documentConfig"></param>
        internal DMEWeb_TemplateDocument(DMEWeb_Template documentElement, DMEWeb_Tag container, string text, DMEWeb_TemplateDocumentConfig documentConfig)
        {
            this.DocumentConfig = documentConfig;
            this.AppendChild(documentElement);
            this.ChildTemplates.Add(documentElement);
            this.ParseString(documentElement, container, text);
        }
        #endregion

        #region 属性定义
        ///// <summary>
        ///// 模板文本数据
        ///// </summary>
        /////public string Text { get; private set; }

        /// <summary>
        /// 根文档模板
        /// </summary>
        public DMEWeb_Template DocumentElement
        {
            get
            {
                return this.TagContainer;
            }
            set
            {
                this.TagContainer = value;
            }
        }

        /// <summary>
        /// 返回此模板块的宿主模板文档
        /// </summary>
        public override DMEWeb_TemplateDocument OwnerDocument
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// 模板文档的配置参数
        /// </summary>
        public DMEWeb_TemplateDocumentConfig DocumentConfig { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private const string CallContext_RenderingTag_Key = "DME.Web.Template.DMEWeb_TemplateDocument.CurrentRenderingTag";
        /// <summary>
        /// 
        /// </summary>
        private const string CallContext_RenderingDocument_Key = "DME.Web.Template.DMEWeb_TemplateDocument.CurrentRenderingDocument";
        /// <summary>
        /// 返回当前正在呈现数据的标签
        /// </summary>
        public DMEWeb_Tag CurrentRenderingTag
        {
            get
            {
                return CallContext.GetData(CallContext_RenderingTag_Key) as DMEWeb_Tag;
            }
            private set
            {
                if (value == null)
                {
                    CallContext.FreeNamedDataSlot(CallContext_RenderingTag_Key);
                }
                else
                {
                    CallContext.SetData(CallContext_RenderingTag_Key, value);
                }
            }
        }
        /// <summary>
        /// 当前正在呈现数据的文档
        /// </summary>
        public static DMEWeb_TemplateDocument CurrentRenderingDocument
        {
            get
            {
                return CallContext.GetData(CallContext_RenderingDocument_Key) as DMEWeb_TemplateDocument;
            }
            private set
            {
                if (value == null)
                {
                    CallContext.FreeNamedDataSlot(CallContext_RenderingDocument_Key);
                }
                else
                {
                    CallContext.SetData(CallContext_RenderingDocument_Key, value);
                }
            }
        }
        #endregion

        #region 方法定义
        /// <summary>
        /// 获取此模板文档的呈现数据
        /// </summary>
        public string GetRenderText()
        {
            using (StringWriter writer = new StringWriter())
            {
                this.Render(writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 注册当前呈现的标签
        /// </summary>
        /// <param name="tag"></param>
        internal void RegisterCurrentRenderingTag(DMEWeb_Tag tag)
        {
            DMEWeb_TemplateDocument.CurrentRenderingDocument = tag == null ? null : tag.OwnerDocument;
            this.CurrentRenderingTag = tag;
        }
        #endregion
        
        #region 解析字符串
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="text"></param>
        private void ParseString(string text)
        {
            this.ParseString(this, text);
        }
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="text"></param>
        private void ParseString(DMEWeb_Template ownerTemplate, string text)
        {
            this.ParseString(ownerTemplate, ownerTemplate, text);
        }
        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="ownerTemplate">宿主模板</param>
        /// <param name="container">标签的容器</param>
        /// <param name="text">模板文本数据</param>
        private void ParseString(DMEWeb_Template ownerTemplate, DMEWeb_Tag container, string text)
        {
            //设置根文档模板
            this.DocumentElement = ownerTemplate;
            //this.Text = text;

            if (string.IsNullOrEmpty(text)) return;

            int charOffset = 0, offset = 0;
            bool isClosedTag;
            Match match = null;

            //标签堆栈
            Stack<DMEWeb_Tag> tagStack = new Stack<DMEWeb_Tag>();
            tagStack.Push(container);

            try
            {
                while (offset < text.Length)
                {
                    if (DMEWeb_ParserHelper.IsVariableTagStart(text, offset) && (match = DMEWeb_ParserRegex.VarTagRegex.Match(text, offset)).Success)  //匹配到模板变量
                    {
                        //构建文本节点
                        DMEWeb_ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, match.Index - charOffset);
                        //构建模板变量
                        DMEWeb_ParserHelper.CreateVariableTag(ownerTemplate, container, match);
                    }
                    else if (DMEWeb_ParserHelper.IsTagStart(text, offset) && (match = DMEWeb_ParserRegex.TagRegex.Match(text, offset)).Success)  //匹配到某种类型的标签
                    {
                        //构建文本节点
                        DMEWeb_ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, match.Index - charOffset);
                        //构建标签
                        DMEWeb_Tag tag = DMEWeb_ParserHelper.CreateTag(ownerTemplate, match, out isClosedTag);

                        //将标签加入堆栈
                        tagStack.Push(tag);
                        //将解析权交给标签
                        bool flag = tag.ProcessBeginTag(ownerTemplate, container, tagStack, text, ref match, isClosedTag);
                        //非已闭合标签或者是单标签则处理标签的结束标签
                        if (flag)
                        {
                            tag.ProcessEndTag(ownerTemplate, tag, tagStack, text, ref match);
                        }
                        if (tagStack.Count > 0 && tagStack.Peek() == tag && isClosedTag)
                        {
                            //闭合标签则回滚一级
                            tagStack.Pop();
                        }
                        //取得容器
                        if (tagStack.Count > 0) container = tagStack.Peek();
                    }
                    else if (DMEWeb_ParserHelper.IsCloseTagStart(text, offset) && (match = DMEWeb_ParserRegex.EndTagRegex.Match(text, offset)).Success)            //匹配到某个结束标签
                    {
                        //取得标签名称
                        string name = match.Groups["tagname"].Value;
                        //非匹配的结束标签.则模板有错
                        throw new DMEWeb_ParserException("无效的结束标签");
                    }
                    else if (DMEWeb_ParserHelper.IsVTExpressionStart(text, offset))
                    {
                        char s = DMEWeb_ParserHelper.ReadChar(text, offset + DMEWeb_ParserHelper.VTExpressionHead.Length);
                        int startOffset = offset + DMEWeb_ParserHelper.VTExpressionHead.Length + 1;
                        int lastOffset = text.IndexOf(s, offset + DMEWeb_ParserHelper.VTExpressionHead.Length + 1);
                        if (lastOffset == -1) throw new DMEWeb_ParserException(string.Format("无法找到VT表达式\"{0}\"的结束标记", DMEWeb_ParserHelper.VTExpressionHead));
                        string code = text.Substring(startOffset, lastOffset - startOffset);
                        if (code.Length > 0)
                        {
                            //构建文本节点
                            DMEWeb_ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, offset - charOffset);
                            //解析表达式里的代码
                            new DMEWeb_TemplateDocument(ownerTemplate, container, code, ownerTemplate.OwnerDocument.DocumentConfig);
                        }
                        offset = lastOffset + 1;
                        charOffset = offset;
                        continue;
                    }
                    else if (DMEWeb_ParserHelper.IsCommentTagStart(text, offset))
                    {
                        //构建文本节点
                        DMEWeb_ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, offset - charOffset);

                        //找到注释的起始标记"<!--vt[",则直接查找结束标记"]-->"
                        offset = text.IndexOf(DMEWeb_ParserHelper.CommentTagEnd, offset + DMEWeb_ParserHelper.CommentTagStart.Length);
                        if (offset == -1) throw new DMEWeb_ParserException("无法找到注释的结束标记");
                        offset += DMEWeb_ParserHelper.CommentTagEnd.Length;
                        charOffset = offset;
                        continue;
                    }
                    //处理偏移位置
                    if (match != null && match.Success)
                    {
                        charOffset = offset = match.Index + match.Length;
                        match = null;
                    }
                    else
                    {
                        offset++;
                    }
                }
                //处理文本字符
                DMEWeb_ParserHelper.CreateTextNode(ownerTemplate, container, text, charOffset, text.Length - charOffset);
                if (tagStack.Count > 1)
                {
                    //堆栈里还有其它元素.则有错误
                    throw new DMEWeb_ParserException(string.Format("{0}标签未闭合", tagStack.Pop()));
                }
            }
            catch (DMEWeb_ParserException ex)
            {
                //如果错误中不包含行号与列号.则计算行号与列号
                if (!ex.HaveLineAndColumnNumber && match != null && match.Success)
                {
                    //获取当前出错时正在解析的模板文件
                    string file = string.Empty;
                   DMEWeb_Tag tag = container;
                    while (string.IsNullOrEmpty(file) && tag != null)
                    {
                        if (tag is DMEWeb_Template)
                        {
                            file = ((DMEWeb_Template)tag).File;
                        }
                        else if (tag is DMEWeb_IncludeTag)
                        {
                            file = ((DMEWeb_IncludeTag)tag).File;
                        }
                        tag = tag.Parent;
                    }
                    if (string.IsNullOrEmpty(file))
                    {
                        throw new DMEWeb_ParserException(DMEWeb_Utility.GetLineAndColumnNumber(text, match.Index), match.ToString(), ex.Message);
                    }
                    else
                    {
                        throw new DMEWeb_ParserException(file, DMEWeb_Utility.GetLineAndColumnNumber(text, match.Index), match.ToString(), ex.Message);
                    }
                }
                else
                {
                    //继续抛出错误
                    throw;
                }
            }
            finally
            {
                //清空堆栈
                tagStack.Clear();
                tagStack = null;
            }
        }
        #endregion

        #region 从文件缓存中构建模板文档对象
                /// <summary>
        /// 从文件缓存中构建模板文档对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static DMEWeb_TemplateDocument FromFileCache(string fileName, Encoding charset)
        {
            return FromFileCache(fileName, charset, DMEWeb_TemplateDocumentConfig.Default);
        }
        /// <summary>
        /// 从文件缓存中构建模板文档对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="charset"></param>
        /// <param name="documentConfig"></param>
        /// <returns></returns>
        public static DMEWeb_TemplateDocument FromFileCache(string fileName, Encoding charset, DMEWeb_TemplateDocumentConfig documentConfig)
        {
            Cache cache = HttpRuntime.Cache;
            if (documentConfig == null) documentConfig = DMEWeb_TemplateDocumentConfig.Default;

            //没有缓存则直接返回实例
            if (cache == null) return new DMEWeb_TemplateDocument(fileName, charset, documentConfig);
            fileName = Path.GetFullPath(fileName);

            string cacheKey = string.Format("TEMPLATE_DOCUMENT_CACHE_ITEM_{0}_{1}_{2}", documentConfig.TagOpenMode, documentConfig.CompressText, fileName);
            DMEWeb_TemplateDocument document = cache.Get(cacheKey) as DMEWeb_TemplateDocument;
            if (document == null)
            {
                document = new DMEWeb_TemplateDocument(fileName, charset, documentConfig);
                cache.Insert(cacheKey, document, new CacheDependency(document.FileDependencies));
            }
            //返回副本
            return document.Clone();
        }
        #endregion

        #region 克隆模板文档对象
        /// <summary>
        /// 克隆模板文档对象
        /// </summary>
        /// <returns></returns>
        private DMEWeb_TemplateDocument Clone()
        {
            return (DMEWeb_TemplateDocument)Clone(null);
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
            DMEWeb_TemplateDocument tag = new DMEWeb_TemplateDocument(this.DocumentConfig);

            //优先拷贝变量
            foreach (DMEWeb_Variable var in this.Variables)
            {
                tag.Variables.Add(var.Clone(tag));
            }

            tag.Id = this.Id;
            tag.Name = this.Name;
            foreach (var att in this.Attributes)
            {
                tag.Attributes.Add(att.Clone(tag));
            }

            tag.Charset = this.Charset;
            tag.File = this.File;
            tag.fileDependencies = this.fileDependencies;
            tag.Visible = this.Visible;

            foreach (DMEWeb_Element element in this.InnerElements)
            {
                DMEWeb_Element item = element.Clone(tag);
                tag.AppendChild(item);

                if (element is DMEWeb_Template && this.DocumentElement == element) tag.DocumentElement = (DMEWeb_Template)item;
            }

            if (tag.DocumentElement == null) tag.DocumentElement = tag;
            return tag;
        }
        #endregion
    }
}
