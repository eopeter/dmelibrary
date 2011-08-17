
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;

namespace DME.Web.Template
{
    /// <summary>
    /// 变量元素.如变量: {$:member.name} 或带前缀与属性的变量: {$:#.member.name htmlencode='true'}
    /// </summary>
    public class DMEWeb_VariableTag : DMEWeb_Element, DMEWeb_IAttributesElement
    {
        /// <summary>
        /// 带变量字段的初始化
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="varExp"></param>
        internal DMEWeb_VariableTag(DMEWeb_Template ownerTemplate, DMEWeb_VariableExpression varExp)
            : base(ownerTemplate)
        {
            //注册添加属性时触发事件.用于设置自身的某些属性值
            this.Attributes = new DMEWeb_AttributeCollection(this);
            this.Attributes.Adding += OnAddingAttribute;
            this.VarExpression = varExp;
            this.CallFunctions = new List<DMEWeb_IExpression>();
        }

        #region 属性定义
        /// <summary>
        /// 此标签的属性集合
        /// </summary>
        public DMEWeb_AttributeCollection Attributes { get; protected set; }

        /// <summary>
        /// 变量元素中的变量表达式
        /// </summary>
        public DMEWeb_VariableExpression VarExpression { get; private set; }

        /// <summary>
        /// 是否需要对输出数据进行HTML数据格式化
        /// </summary>
        public DMEWeb_Attribute HtmlEncode
        {
            get
            {
                return this.Attributes["HtmlEncode"];
            }
        }

        /// <summary>
        /// 是否需要对输出数据进行JS脚本格式化
        /// </summary>
        public DMEWeb_Attribute JsEncode
        {
            get
            {
                return this.Attributes["JsEncode"];
            }
        }

        /// <summary>
        /// 是否需要对输出数据进行XML数据格式化
        /// </summary>
        public DMEWeb_Attribute XmlEncode
        {
            get
            {
                return this.Attributes["XmlEncode"];
            }
        }

        /// <summary>
        /// 是否需要对输出数据进行URL地址编码
        /// </summary>
        public DMEWeb_Attribute UrlEncode
        {
            get
            {
                return this.Attributes["UrlEncode"];
            }
        }

        /// <summary>
        /// 是否需要对输出数据进行文本数据格式化(先HtmlEncode格式化,再将"空格"转换为"&amp;nbsp;", "\n"转换为"&lt;br /&gt;")
        /// </summary>
        public DMEWeb_Attribute TextEncode
        {
            get
            {
                return this.Attributes["TextEncode"];
            }
        }

        /// <summary>
        /// 是否需要对输出数据进行文本压缩(删除换行符和换行符前后的空格)
        /// </summary>
        public DMEWeb_Attribute CompressText
        {
            get
            {
                return this.Attributes["CompressText"];
            }
        }

        /// <summary>
        /// 数据的输出长度
        /// </summary>
        public DMEWeb_Attribute Length
        {
            get
            {
                return this.Attributes["Length"];
            }
        }

        /// <summary>
        /// 附加文本(此属性只能配合Length属性使用.即当文本有被裁剪时才附加此文本)
        /// </summary>
        public DMEWeb_Attribute AppendText
        {
            get
            {
                return this.Attributes["AppendText"];
            }
        }

        /// <summary>
        /// 数据的编码
        /// </summary>
        public DMEWeb_Attribute Charset
        {
            get
            {
                return this.Attributes["Charset"];
            }
        }

        /// <summary>
        /// 数据输出时的格式化表达式
        /// </summary>
        public DMEWeb_Attribute Format
        {
            get
            {
                return this.Attributes["Format"];
            }
        }

        /// <summary>
        /// 数据输出时是否删除HTML代码
        /// </summary>
        public DMEWeb_Attribute RemoveHtml
        {
            get
            {
                return this.Attributes["RemoveHtml"];
            }
        }

        /// <summary>
        /// 要调用的函数列表
        /// </summary>
        public List<DMEWeb_IExpression> CallFunctions { get; protected set; }
        #endregion

        #region 添加标签属性时的触发事件函数.用于设置自身的某些属性值
        /// <summary>
        /// 添加标签属性时的触发事件函数.用于设置自身的某些属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddingAttribute(object sender, DMEWeb_AttributeCollection.DMEWeb_AttributeAddingEventArgs e)
        {
            switch (e.Item.Name.ToLower())
            {
                case "call":
                    //要调用的方法
                    this.CallFunctions.Add(e.Item.Value);
                    break;
            }
        }
        #endregion

        #region 呈现本元素的数据
        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        public override void Render(System.IO.TextWriter writer)
        {
            DMEWeb_VariableTag tag = this;
            object value = this.VarExpression.GetValue();

            //调用自定函数处理此变量的值
            if (this.CallFunctions.Count > 0)
            {
                foreach (var exp in this.CallFunctions)
                {
                    object method = exp.GetValue();
                    if (!DMEWeb_Utility.IsNothing(method))
                    {
                        DMEWeb_UserDefinedFunction func;
                        if (this.OwnerTemplate.UserDefinedFunctions.TryGetValue(method.ToString(), out func))
                        {
                            value = func(new object[] { value });
                        }
                    }
                }
            }

            if (DMEWeb_Utility.IsNothing(value)) return;

            bool formated = false;
            string text = string.Empty;
            string format = this.Format == null ? string.Empty : this.Format.GetTextValue();
            if (value is string)
            {
                //字符串
                text = (string)value;
            }
            else
            {                
                //非字符串.则判断处理format
                if (!string.IsNullOrEmpty(format))
                {
                    IFormattable iformat = value as IFormattable;
                    if (iformat != null)
                    {
                        text = iformat.ToString(format, CultureInfo.InvariantCulture);
                        formated = true;
                    }
                }
                //非IFormattable接口.则直接取字符串处理
                if (!formated)
                {
                    //如果数据是IEnumerable,IEnumerator接口则进行数据拼凑
                    IEnumerator ie = null;
                    if (value is IEnumerable)
                    {
                        ie = ((IEnumerable)value).GetEnumerator();
                    }
                    else if (value is IEnumerator)
                    {
                        ie = (IEnumerator)value;
                    }
                    if (ie != null)
                    {
                        StringBuilder buffer = new StringBuilder();
                        ie.Reset();
                        while (ie.MoveNext())
                        {
                            if (buffer.Length != 0) buffer.Append(",");
                            buffer.Append(ie.Current);
                        }
                        text = buffer.ToString();
                    }
                    else
                    {
                        text = value.ToString();
                    }
                }
            }

            if (text.Length > 0)
            {
                bool removeHtml = this.RemoveHtml == null ? false : DMEWeb_Utility.ConverToBoolean(this.RemoveHtml.GetTextValue());
                bool textEncode = this.TextEncode == null ? false : DMEWeb_Utility.ConverToBoolean(this.TextEncode.GetTextValue());
                bool htmlEncode = this.HtmlEncode == null ? false : DMEWeb_Utility.ConverToBoolean(this.HtmlEncode.GetTextValue());
                bool xmlEncode = this.XmlEncode == null ? false : DMEWeb_Utility.ConverToBoolean(this.XmlEncode.GetTextValue());
                bool jsEncode = this.JsEncode == null ? false : DMEWeb_Utility.ConverToBoolean(this.JsEncode.GetTextValue());
                bool urlEncode = this.UrlEncode == null ? false : DMEWeb_Utility.ConverToBoolean(this.UrlEncode.GetTextValue());
                bool compressText = this.CompressText == null ? false : DMEWeb_Utility.ConverToBoolean(this.CompressText.GetTextValue());

                int length = this.Length == null ? 0 : DMEWeb_Utility.ConverToInt32(this.Length.GetTextValue());
                string appendText = this.AppendText == null ? string.Empty : this.AppendText.GetTextValue();
                Encoding charset = this.Charset == null ? this.OwnerTemplate.Charset : DMEWeb_Utility.GetEncodingFromCharset(this.Charset.GetTextValue(), this.OwnerTemplate.Charset);

                if (removeHtml) text = DMEWeb_Utility.RemoveHtmlCode(text);
                if (length > 0) text = DMEWeb_Utility.CutString(text, length, charset, appendText);
                if (textEncode)
                {
                    text = DMEWeb_Utility.TextEncode(text);
                }
                else if (htmlEncode)
                {
                    text = HttpUtility.HtmlEncode(text);
                }
                if (xmlEncode) text = DMEWeb_Utility.XmlEncode(text);
                if (jsEncode) text = DMEWeb_Utility.JsEncode(text);
                if (urlEncode) text = HttpUtility.UrlEncode(text, charset);
                if (compressText) text = DMEWeb_Utility.CompressText(text);

                if (!formated && !string.IsNullOrEmpty(format))
                {
                    text = string.Format(format, text);
                }
                

                writer.Write(text);
            }
        }
        #endregion

        #region 输出变量元素的原字符数据
        /// <summary>
        /// 输出变量元素的原字符串数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("{$:");
            buffer.Append(this.VarExpression.ToString());
            foreach (DMEWeb_Attribute attribute in this.Attributes)
            {
                buffer.AppendFormat(" {0}=\"{1}\"", attribute.Name, HttpUtility.HtmlEncode(attribute.Text));
            }
            buffer.Append("}");
            return buffer.ToString();
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
            DMEWeb_VariableTag tag = new DMEWeb_VariableTag(ownerTemplate, (DMEWeb_VariableExpression)this.VarExpression.Clone(ownerTemplate));
            foreach (var att in this.Attributes)
            {
                tag.Attributes.Add(att.Clone(ownerTemplate));
            }
            foreach (DMEWeb_IExpression exp in this.CallFunctions)
            {
                tag.CallFunctions.Add(exp.Clone(ownerTemplate));
            }
            return tag;
        }
        #endregion
    }
}
