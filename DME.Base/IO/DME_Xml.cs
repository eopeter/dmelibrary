using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using DME.Base.Helper;
using DME.Base.Log;
using DME.Base.Collections;

namespace DME.Base.IO
{
    /// <summary>
    /// xml文件操作类
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DME_Xml : DME_DisposeBase
    {
        #region 私有变量
        private XmlDocument _doc;
        private XmlNode _xn;
        private string _xmlPath;
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlPath"></param>
        public DME_Xml(string xmlPath)
        {
            _xmlPath = xmlPath;
            _doc = new XmlDocument();
            if (DME_Files.FileExists(xmlPath))
            {
               _doc.Load(xmlPath);              
            }
            else
            {
                _doc.LoadXml(xmlPath);
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        public DME_Xml()
        {
            _doc = new XmlDocument();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public DME_Xml(Stream stream)
        {
            _doc = new XmlDocument();
            _doc.Load(stream);
        }


        #endregion

        #region 析构
        #endregion

        #region 属性
        /// <summary> 返回节点对象集合</summary>
        /// <param name="node">节点</param>
        /// <returns>XmlNodeList</returns>
        public XmlDocument Document
        {
            get{return _doc;}
        }
        #endregion

        #region 私有函数
        
        #endregion

        #region 公开函数
        /// <summary>调用此方法后不再需要调用Execute方法。</summary>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns></returns>
        public string Select(string node, string attribute)
        {
            string value = String.Empty; ;
            _xn = GetNode(node);
            value = (DME_Validation.IsNull(attribute) ? _xn.InnerText : _xn.Attributes[attribute].Value);
            
            return value;
        }

        /// <summary>插入数据</summary>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public DME_Xml Insert(string node, string element, string attribute, string value)
        {
            _xn = GetNode(node);
            if (DME_Validation.IsNull(element))
            {
                if (!DME_Validation.IsNull(attribute))
                {
                    XmlElement xe = (XmlElement)_xn;
                    xe.SetAttribute(attribute, value);
                }
            }
            else
            {
                XmlElement xe = _doc.CreateElement(element);
                if (DME_Validation.IsNull(attribute))
                {
                    xe.InnerText = value;
                }
                else
                {
                    xe.SetAttribute(attribute, value);
                }
                _xn.AppendChild(xe);
            }
            return this;
        }

        /// <summary>插入属性</summary>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性组，格式:IP=127.0.0.1; DataSource=TestMap ;User=sa ;Password=chenqi;DataBase=TestMap</param>
        /// <returns></returns>
        public DME_Xml Insert_Attribute(string node, string element, string attribute)
        {
            _xn = GetNode(node);
            string[] groups = null;
            string groupOk = string.Empty;
            string[] keyVal = null;
            if (DME_Validation.IsNull(element))
            {
                if (!DME_Validation.IsNull(attribute))
                {
                    groups = DME_String.SplitString(attribute, ";");
                    XmlElement xe = (XmlElement)_xn;
                    foreach (string group in groups)
                    {
                        groupOk = group.Trim();
                        if (!DME_Validation.IsNull(groupOk))
                        {
                            keyVal = DME_String.SplitString(groupOk, "=");
                            if (keyVal.Length == 2)
                            {
                                xe.SetAttribute(keyVal[0].Trim(), keyVal[1].Trim());
                            }
                        }
                    }
                }
            }
            else
            {
                XmlElement xe = _doc.CreateElement(element);
                if (!DME_Validation.IsNull(attribute))
                {
                    groups = DME_String.SplitString(attribute, ";");
                    foreach (string group in groups)
                    {
                        groupOk = group.Trim();
                        if (!DME_Validation.IsNull(groupOk))
                        {
                            keyVal = DME_String.SplitString(groupOk, "=");
                            if (keyVal.Length == 2)
                            {
                                xe.SetAttribute(keyVal[0].Trim(), keyVal[1].Trim());
                            }
                        }
                    }
                }
                _xn.AppendChild(xe);
            }
            return this;
        }

        /// <summary>插入CDATA格式数据</summary>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public DME_Xml Insert_Cdata(string node, string element, string attribute, string value)
        {
            _xn = GetNode(node);
            if (DME_Validation.IsNull(element))
            {
                if (!DME_Validation.IsNull(attribute))
                {
                    XmlElement xe = (XmlElement)_xn;
                    xe.SetAttribute(attribute, value);
                }
            }
            else
            {
                XmlElement xe = _doc.CreateElement(element);
                if (DME_Validation.IsNull(attribute))
                {
                    xe.AppendChild(_doc.CreateCDataSection(value));
                }
                else
                {
                    xe.SetAttribute(attribute, value);
                }
                _xn.AppendChild(xe);
            }
            return this;
        }

        /// <summary>插入数据</summary>
        /// <param name="node">节点</param>
        /// <param name="xmlnode"></param>
        /// <returns></returns>
        public DME_Xml Insert(string node, XmlNode xmlnode)
        {
            if (!DME_Validation.IsNull(xmlnode))
            {
                _xn = GetNode(node);
                XmlNode NewNode = _doc.ImportNode(xmlnode, true);
                _xn.AppendChild(NewNode);
            }
            return this;
        }

        /// <summary>修改数据</summary>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public DME_Xml Update(string node, string attribute, string value)
        {
            _xn = GetNode(node);
            XmlElement xe = (XmlElement)_xn;
            if (DME_Validation.IsNull(attribute))
            {
                xe.InnerText = value;
            }
            else
            {
                xe.SetAttribute(attribute, value);
            }
            return this;
        }

        /// <summary>修改CDATA格式数据</summary>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public DME_Xml Update_Cdata(string node, string attribute, string value)
        {
            _xn = GetNode(node);
            XmlElement xe = (XmlElement)_xn;
            if (DME_Validation.IsNull(attribute))
            {
                xe.InnerText = "";
                xe.AppendChild(_doc.CreateCDataSection(value));
            }
            else
            {
                xe.SetAttribute(attribute, value);
            }
            return this;
        }

        /// <summary>删除数据</summary>
        /// <param name="node">节点如果是非根节点可使用多层节点表达式：nodelayer1>nodelayer2>nodelayer3</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <returns></returns>
        public DME_Xml Delete(string node, string attribute)
        {
            _xn = GetNode(node);
            XmlElement xe = (XmlElement)_xn;
            if (DME_Validation.IsNull(attribute))
            {
                _xn.ParentNode.RemoveChild(_xn);
            }
            else
            {
                xe.RemoveAttribute(attribute);
            }
            return this;
        }

        /// <summary>返回节点</summary>
        /// <param name="node">节点表达式</param>
        /// <returns></returns>
        public XmlNode GetNode(string node)
        { 
            if (DME_Validation.IsNull(node))
            {
                throw new ArgumentNullException(string.Format("{0} 节点为空", node));
            }
            string[] nodelayers = node.Split('>');
            XmlNode xn = _doc.SelectSingleNode(nodelayers[0]);
            for (int i = 1; i < nodelayers.Length; i++)
            {
                if (DME_Validation.IsNull(nodelayers[i]))
                {

                    throw new ArgumentException(String.Format("第{0}级节点为空", i + 1));
                }
                else
                {
                    xn = xn.SelectSingleNode(nodelayers[i]);
                }
            }
            return xn;
        }

        /// <summary>保存XML</summary>
        /// <returns></returns>
        public DME_Xml Save()
        {
            return Save(_xmlPath);
        }

        /// <summary>保存XML</summary>
        /// <param name="XmlPath">Xml路径</param>
        /// <returns></returns>
        public DME_Xml Save(string XmlPath)
        {
            string dir = DME_Path.GetDirectoryName(XmlPath);
            if (!DME_Files.FolderExists(dir))
            {
                DME_Files.CreateFolder(dir);
            }
            _doc.Save(XmlPath);
            return this;
        }

        /// <summary> 查找数据,返回当前节点的所有下级节点,填充到一个DataSet中</summary>
        /// <param name="node">节点的路径</param>
        /// <returns></returns>
        public DataSet GetXmlData(string node)
        {
            DataSet ds = new DataSet();
            StringReader read = new StringReader(GetNode(node).OuterXml);
            ds.ReadXml(read);
            read.Dispose();
            return ds;
        }

        /// <summary>创健XML格式</summary>
        /// <param name="rootname"></param>
        /// <returns></returns>
        public DME_Xml Create(string rootname)
        {
            _doc.LoadXml(string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?><{0}/>", rootname));
            return this;
        }
        /// <summary>创健XML格式</summary>
        /// <param name="rootname"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public DME_Xml Create(string rootname,DME_Encoding encoding)
        {
            _doc.LoadXml(string.Format(@"<?xml version=""1.0"" encoding=""{0}""?><{1}/>",DME_EnumDescription.GetFieldText(encoding), rootname));
            return this;
        }

        /// <summary> 返回节点对象集合</summary>
        /// <param name="node">节点</param>
        /// <returns>XmlNodeList</returns>
        public XmlNodeList GetXmlNodeList(string node,string itme)
        {
            _xn = GetNode(node);
            XmlNodeList _NodeList = _xn.SelectNodes(itme);
            return _NodeList;
        }

        

        /// <summary>显示XML</summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string Echo(string node)
        {
            _xn = GetNode(node);
            return _xn.OuterXml;
        }
        /// <summary>
        /// 子类重载实现资源释放逻辑
        /// </summary>
        /// <param name="disposing">从Dispose调用（释放所有资源）还是析构函数调用（释放非托管资源）</param>
        protected override void OnDispose(Boolean disposing)
        {
            if (disposing)
            {
                // 释放托管资源

                _doc = null;
                _xn = null;
            }

            base.OnDispose(disposing);
        }
        #endregion
    }

}
