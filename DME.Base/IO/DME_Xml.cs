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
    /// xml�ļ�������
    /// 
    /// �޸ļ�¼
    ///
    ///		2010.12.18 �汾��1.0 lance ������
    /// 
    /// �汾��1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DME_Xml : DME_DisposeBase
    {
        #region ˽�б���
        private XmlDocument _doc;
        private XmlNode _xn;
        private string _xmlPath;
        #endregion

        #region ���б���
        #endregion

        #region ����
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

        #region ����
        #endregion

        #region ����
        /// <summary> ���ؽڵ���󼯺�</summary>
        /// <param name="node">�ڵ�</param>
        /// <returns>XmlNodeList</returns>
        public XmlDocument Document
        {
            get{return _doc;}
        }
        #endregion

        #region ˽�к���
        
        #endregion

        #region ��������
        /// <summary>���ô˷���������Ҫ����Execute������</summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="attribute">���������ǿ�ʱ���ظ�����ֵ�����򷵻ش���ֵ</param>
        /// <returns></returns>
        public string Select(string node, string attribute)
        {
            string value = String.Empty; ;
            _xn = GetNode(node);
            value = (DME_Validation.IsNull(attribute) ? _xn.InnerText : _xn.Attributes[attribute].Value);
            
            return value;
        }

        /// <summary>��������</summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="element">Ԫ�������ǿ�ʱ������Ԫ�أ������ڸ�Ԫ���в�������</param>
        /// <param name="attribute">���������ǿ�ʱ�����Ԫ������ֵ���������Ԫ��ֵ</param>
        /// <param name="value">ֵ</param>
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

        /// <summary>��������</summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="element">Ԫ�������ǿ�ʱ������Ԫ�أ������ڸ�Ԫ���в�������</param>
        /// <param name="attribute">�����飬��ʽ:IP=127.0.0.1; DataSource=TestMap ;User=sa ;Password=chenqi;DataBase=TestMap</param>
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

        /// <summary>����CDATA��ʽ����</summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="element">Ԫ�������ǿ�ʱ������Ԫ�أ������ڸ�Ԫ���в�������</param>
        /// <param name="attribute">���������ǿ�ʱ�����Ԫ������ֵ���������Ԫ��ֵ</param>
        /// <param name="value">ֵ</param>
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

        /// <summary>��������</summary>
        /// <param name="node">�ڵ�</param>
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

        /// <summary>�޸�����</summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="attribute">���������ǿ�ʱ�޸ĸýڵ�����ֵ�������޸Ľڵ�ֵ</param>
        /// <param name="value">ֵ</param>
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

        /// <summary>�޸�CDATA��ʽ����</summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="attribute">���������ǿ�ʱ�޸ĸýڵ�����ֵ�������޸Ľڵ�ֵ</param>
        /// <param name="value">ֵ</param>
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

        /// <summary>ɾ������</summary>
        /// <param name="node">�ڵ�����ǷǸ��ڵ��ʹ�ö��ڵ���ʽ��nodelayer1>nodelayer2>nodelayer3</param>
        /// <param name="attribute">���������ǿ�ʱɾ���ýڵ�����ֵ������ɾ���ڵ�ֵ</param>
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

        /// <summary>���ؽڵ�</summary>
        /// <param name="node">�ڵ���ʽ</param>
        /// <returns></returns>
        public XmlNode GetNode(string node)
        { 
            if (DME_Validation.IsNull(node))
            {
                throw new ArgumentNullException(string.Format("{0} �ڵ�Ϊ��", node));
            }
            string[] nodelayers = node.Split('>');
            XmlNode xn = _doc.SelectSingleNode(nodelayers[0]);
            for (int i = 1; i < nodelayers.Length; i++)
            {
                if (DME_Validation.IsNull(nodelayers[i]))
                {

                    throw new ArgumentException(String.Format("��{0}���ڵ�Ϊ��", i + 1));
                }
                else
                {
                    xn = xn.SelectSingleNode(nodelayers[i]);
                }
            }
            return xn;
        }

        /// <summary>����XML</summary>
        /// <returns></returns>
        public DME_Xml Save()
        {
            return Save(_xmlPath);
        }

        /// <summary>����XML</summary>
        /// <param name="XmlPath">Xml·��</param>
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

        /// <summary> ��������,���ص�ǰ�ڵ�������¼��ڵ�,��䵽һ��DataSet��</summary>
        /// <param name="node">�ڵ��·��</param>
        /// <returns></returns>
        public DataSet GetXmlData(string node)
        {
            DataSet ds = new DataSet();
            StringReader read = new StringReader(GetNode(node).OuterXml);
            ds.ReadXml(read);
            read.Dispose();
            return ds;
        }

        /// <summary>����XML��ʽ</summary>
        /// <param name="rootname"></param>
        /// <returns></returns>
        public DME_Xml Create(string rootname)
        {
            _doc.LoadXml(string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?><{0}/>", rootname));
            return this;
        }
        /// <summary>����XML��ʽ</summary>
        /// <param name="rootname"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public DME_Xml Create(string rootname,DME_Encoding encoding)
        {
            _doc.LoadXml(string.Format(@"<?xml version=""1.0"" encoding=""{0}""?><{1}/>",DME_EnumDescription.GetFieldText(encoding), rootname));
            return this;
        }

        /// <summary> ���ؽڵ���󼯺�</summary>
        /// <param name="node">�ڵ�</param>
        /// <returns>XmlNodeList</returns>
        public XmlNodeList GetXmlNodeList(string node,string itme)
        {
            _xn = GetNode(node);
            XmlNodeList _NodeList = _xn.SelectNodes(itme);
            return _NodeList;
        }

        

        /// <summary>��ʾXML</summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string Echo(string node)
        {
            _xn = GetNode(node);
            return _xn.OuterXml;
        }
        /// <summary>
        /// ��������ʵ����Դ�ͷ��߼�
        /// </summary>
        /// <param name="disposing">��Dispose���ã��ͷ�������Դ�����������������ã��ͷŷ��й���Դ��</param>
        protected override void OnDispose(Boolean disposing)
        {
            if (disposing)
            {
                // �ͷ��й���Դ

                _doc = null;
                _xn = null;
            }

            base.OnDispose(disposing);
        }
        #endregion
    }

}
