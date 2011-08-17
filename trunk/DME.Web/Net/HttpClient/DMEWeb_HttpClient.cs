using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Net;
using DME.Base.Helper;

namespace DME.Web.Net
{
    public class DMEWeb_HttpClient
    {

        #region �¼�
        public event EventHandler<DMEWeb_StatusUpdateEventArgs> StatusUpdate;

        private void OnStatusUpdate(DMEWeb_StatusUpdateEventArgs e)
        {
            EventHandler<DMEWeb_StatusUpdateEventArgs> temp = StatusUpdate;
            if (temp != null)
                temp(this, e);
        }
        #endregion

        #region ����
        #region Proxy
        /// <summary>
        /// Ҫ���͵�Form����Ϣ
        /// </summary>
        private DMEWeb_Proxy proxy = null;
        public DMEWeb_Proxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }
        #endregion

        #region KeepContext
        /// <summary>
        /// �Ƿ��Զ��ڲ�ͬ������䱣��Cookie, Referer
        /// </summary>
        private bool keepContext;
        public bool KeepContext
        {
            get { return keepContext; }
            set { keepContext = value; }
        }
        #endregion

        #region DefaultLanguage
        /// <summary>
        /// �����Ļ�Ӧ������
        /// </summary>
        private string defaultLanguage = "zh-CN";
        public string DefaultLanguage
        {
            get { return defaultLanguage; }
            set { defaultLanguage = value; }
        }
        #endregion

        #region DefaultEncoding
        /// <summary>
        /// GetString()������ܴ�HTTPͷ��Meta��ǩ�л�ȡ������Ϣ,��ʹ�ô˱�������ȡ�ַ���
        /// </summary>
        private Encoding defaultEncoding = Encoding.UTF8;
        public Encoding DefaultEncoding
        {
            get { return defaultEncoding; }
            set { defaultEncoding = value; }
        }
        #endregion

        #region Verb
        /// <summary>
        /// ָʾ����Get������Post����
        /// </summary>
        private DMEWeb_HttpVerb verb = DMEWeb_HttpVerb.GET;
        public DMEWeb_HttpVerb Verb
        {
            get { return verb; }
            set { verb = value; }
        }
        #endregion

        #region Files
        /// <summary>
        /// Ҫ�ϴ����ļ�.�����Ϊ�����Զ�תΪPost����
        /// </summary>
        private readonly List<DMEWeb_HttpUploadingFile> files = new List<DMEWeb_HttpUploadingFile>();
        public List<DMEWeb_HttpUploadingFile> Files
        {
            get { return files; }
        }
        #endregion

        #region PostingData
        /// <summary>
        /// Ҫ���͵�Form����Ϣ
        /// </summary>
        private readonly Dictionary<string, string> postingData = new Dictionary<string, string>();
        public Dictionary<string, string> PostingData
        {
            get { return postingData; }
        }
        #endregion

        #region PostXmlData
        /// <summary>
        /// Ҫ���͵�Xml
        /// </summary>
        private string postXmlData = string.Empty;
        public string PostXmlData
        {
            get { return postXmlData; }
            set { postXmlData = value; }
        }
        #endregion

        #region Url
        /// <summary>
        /// ��ȡ������������Դ�ĵ�ַ
        /// </summary>
        private string url = string.Empty;
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        #endregion

        #region ResponseHeaders
        /// <summary>
        /// �����ڻ�ȡ��Ӧ��,��ʱ��¼��Ӧ��HTTPͷ
        /// </summary>
        private WebHeaderCollection responseHeaders;
        public WebHeaderCollection ResponseHeaders
        {
            get { return responseHeaders; }
        }
        #endregion

        #region Accept
        /// <summary>
        /// ��ȡ��������������Դ����
        /// </summary>
        private string accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
        public string Accept
        {
            get { return accept; }
            set { accept = value; }
        }
        #endregion

        #region UserAgent
        /// <summary>
        /// ��ȡ�����������е�HttpͷUser-Agent��ֵ
        /// </summary>
        private string userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }
        #endregion

        #region Context
        /// <summary>
        /// ��ȡ������Cookie��Referer
        /// </summary>
        private DMEWeb_HttpClientContext context;
        public DMEWeb_HttpClientContext Context
        {
            get { return context; }
            set { context = value; }
        }
        #endregion

        #region StartPoint
        /// <summary>
        /// ��ȡ�����û�ȡ���ݵ���ʼ��,���ڶϵ�����,���߳����ص�
        /// </summary>
        private int startPoint;
        public int StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }
        #endregion

        #region EndPoint
        /// <summary>
        /// ��ȡ�����û�ȡ���ݵĽ�����,���ڶϵ�����,���³����ص�.
        /// ���Ϊ0,��ʾ��ȡ��Դ��StartPoint��ʼ��ʣ������
        /// </summary>
        private int endPoint;
        public int EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }
        #endregion
        #endregion

        #region ����
        #region public HttpClient()
        /// <summary>
        /// �����µ�HttpClientʵ��
        /// </summary>
        public DMEWeb_HttpClient()
            : this(null)
        {
        }
        #endregion

        #region public DMEWeb_HttpClient(string url)
        /// <summary>
        /// �����µ�HttpClientʵ��
        /// </summary>
        /// <param name="url">Ҫ��ȡ����Դ�ĵ�ַ</param>
        public DMEWeb_HttpClient(string url)
            : this(url, null)
        {
        }
        #endregion

        #region public HttpClient(string url, DMEWeb_HttpClientContext context)
        /// <summary>
        /// �����µ�HttpClientʵ��
        /// </summary>
        /// <param name="url">Ҫ��ȡ����Դ�ĵ�ַ</param>
        /// <param name="context">Cookie��Referer</param>
        public DMEWeb_HttpClient(string url, DMEWeb_HttpClientContext context)
            : this(url, context, false)
        {
        }
        #endregion

        #region public DMEWeb_HttpClient(string url, DMEWeb_HttpClientContext context, bool keepContext)
        /// <summary>
        /// �����µ�HttpClientʵ��
        /// </summary>
        /// <param name="url">Ҫ��ȡ����Դ�ĵ�ַ</param>
        /// <param name="context">Cookie��Referer</param>
        /// <param name="keepContext">�Ƿ��Զ��ڲ�ͬ������䱣��Cookie, Referer</param>
        public DMEWeb_HttpClient(string url, DMEWeb_HttpClientContext context, bool keepContext)
        {
            this.url = url;
            this.context = context;
            this.keepContext = keepContext;
            if (this.context == null)
                this.context = new DMEWeb_HttpClientContext();
        }
        #endregion
        #endregion

        #region ����
        #region private string GetEncodingFromHeaders()
        private string GetEncodingFromHeaders()
        {
            string encoding = null;
            string contentType = responseHeaders["Content-Type"];
            if (contentType != null)
            {
                int i = contentType.IndexOf("charset=");
                if (i != -1)
                {
                    encoding = contentType.Substring(i + 8);
                }
            }
            return encoding;
        }
        #endregion

        #region private string GetEncodingFromBody(byte[] data)
        private string GetEncodingFromBody(byte[] data)
        {
            string encodingName = null;
            string dataAsAscii = Encoding.ASCII.GetString(data);
            if (dataAsAscii != null)
            {
                int i = dataAsAscii.IndexOf("charset=");
                if (i != -1)
                {
                    int j = dataAsAscii.IndexOf("\"", i);
                    if (j != -1)
                    {
                        int k = i + 8;
                        encodingName = dataAsAscii.Substring(k, (j - k) + 1);
                        char[] chArray = new char[2] { '>', '"' };
                        encodingName = encodingName.TrimEnd(chArray);
                    }
                }
            }
            return encodingName;
        }
        #endregion

        #region private HttpWebRequest CreateRequest()
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns>web�������</returns>
        private HttpWebRequest CreateRequest()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.AllowAutoRedirect = false;
            req.CookieContainer = new CookieContainer();
            req.Headers.Add("Accept-Language", defaultLanguage);
            req.Accept = accept;
            req.UserAgent = userAgent;
            req.KeepAlive = false;
            if (!DME_Validation.IsNull(proxy))
            {
                req.Timeout = 60000;
                req.UseDefaultCredentials = true;
                req.Proxy = proxy.GetProxy();
                
            }
          
            if (context.Cookies != null)
            {
                req.CookieContainer.Add(context.Cookies);
            }

            if (!string.IsNullOrEmpty(context.Referer))
            {
                req.Referer = context.Referer;
            }

            if (verb == DMEWeb_HttpVerb.HEAD)
            {
                req.Method = "HEAD";
                return req;
            }

            if (postingData.Count > 0 || files.Count > 0)
            {
                verb = DMEWeb_HttpVerb.POST;
            }

            if (verb == DMEWeb_HttpVerb.POSTXML)
            {
                if (DME_Validation.IsNull(postXmlData))
                {
                    postXmlData = "<?xml version=\"1.0\"?><root />";
                }
                byte[] buffer = Encoding.UTF8.GetBytes(postXmlData);
                req.Method = "POST";
                req.ContentType = "application/xml";
                req.ContentLength = buffer.Length;
                Stream PostData = req.GetRequestStream();
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
            }

            if (verb == DMEWeb_HttpVerb.POST)
            {
                req.Method = "POST";

                MemoryStream memoryStream = new MemoryStream();
                StreamWriter writer = new StreamWriter(memoryStream);

                if (files.Count > 0)
                {
                    string newLine = "\r\n";
                    string boundary = Guid.NewGuid().ToString().Replace("-", "");
                    req.ContentType = "multipart/form-data; boundary=" + boundary;

                    foreach (string key in postingData.Keys)
                    {
                        writer.Write("--" + boundary + newLine);
                        writer.Write("Content-Disposition: form-data; name=\"{0}\"{1}{1}", key, newLine);
                        writer.Write(postingData[key] + newLine);
                    }

                    foreach (DMEWeb_HttpUploadingFile file in files)
                    {
                        writer.Write("--" + boundary + newLine);
                        writer.Write(
                            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}",
                            file.FieldName,
                            file.FileName,
                            newLine
                            );
                        writer.Write("Content-Type: application/octet-stream" + newLine + newLine);
                        writer.Flush();
                        memoryStream.Write(file.Data, 0, file.Data.Length);
                        writer.Write(newLine);
                        writer.Write("--" + boundary + newLine);
                    }
                }
                else
                {
                    req.ContentType = "application/x-www-form-urlencoded";
                    StringBuilder sb = new StringBuilder();
                    foreach (string key in postingData.Keys)
                    {
                        sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(postingData[key]));
                    }
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                    }
                    writer.Write(sb.ToString());
                }

                writer.Flush();

                using (Stream stream = req.GetRequestStream())
                {
                    memoryStream.WriteTo(stream);
                }
                writer.Close();
                memoryStream.Close();
            }

            if (startPoint != 0 && endPoint != 0)
            {
                req.AddRange(startPoint, endPoint);
            }
            else if (startPoint != 0 && endPoint == 0)
            {
                req.AddRange(startPoint);
            }

            return req;
        }
        #endregion

        #region public void AttachFile(string fileName, string fieldName)
        /// <summary>
        /// �����������Ҫ�ϴ����ļ�
        /// </summary>
        /// <param name="fileName">Ҫ�ϴ����ļ�·��</param>
        /// <param name="fieldName">�ļ��ֶε�����(�൱��&lt;input type=file name=fieldName&gt;)���fieldName)</param>
        public void AttachFile(string fileName, string fieldName)
        {
            DMEWeb_HttpUploadingFile file = new DMEWeb_HttpUploadingFile(fileName, fieldName);
            files.Add(file);
        }
        #endregion

        #region public void AttachFile(byte[] data, string fileName, string fieldName)
        /// <summary>
        /// �����������Ҫ�ϴ����ļ�
        /// </summary>
        /// <param name="data">Ҫ�ϴ����ļ�����</param>
        /// <param name="fileName">�ļ���</param>
        /// <param name="fieldName">�ļ��ֶε�����(�൱��&lt;input type=file name=fieldName&gt;)���fieldName)</param>
        public void AttachFile(byte[] data, string fileName, string fieldName)
        {
            DMEWeb_HttpUploadingFile file = new DMEWeb_HttpUploadingFile(data, fileName, fieldName);
            files.Add(file);
        }
        #endregion

        #region public void Reset()
        /// <summary>
        /// ���PostingData, Files, StartPoint, EndPoint, ResponseHeaders, ����Verb����ΪGet.
        /// �ڷ���һ������������Ϣ�������,������ô˷������ֹ�������Ӧ������ʹ��һ�����󲻻��ܵ�Ӱ��.
        /// </summary>
        public void Reset()
        {
            verb = DMEWeb_HttpVerb.GET;
            files.Clear();
            postingData.Clear();
            responseHeaders = null;
            startPoint = 0;
            endPoint = 0;
            proxy = null;
        }
        #endregion    


        private Cookie Set_HeadersCookie(string Set_CookieStr,string host)
        {

            if (DME_Validation.IsNull(Set_CookieStr))
            {
                return null;
            }

            string[] groups = Set_CookieStr.Split(';');
            Cookie cke = new Cookie();
            if (!DME_Validation.IsNull(groups[0]))
            {
                string[] tempstr = groups[0].Trim().Split('=');
                if (tempstr.Length > 1)
                {
                    cke.Name = tempstr[0].Trim();
                    cke.Value = tempstr[1].Trim();
                }
                else
                {
                    return null;
                }
            }
            foreach (string group in groups)
            {
                string groupOk = group.Trim();
                if (!DME_Validation.IsNull(groupOk))
                {
                    string[] keyVal = groupOk.Split('=');
                    switch (keyVal[0].ToLower())
                    {
                        case "domain":
                            cke.Domain = keyVal[1].Trim();
                            break;
                        case "path":
                            cke.Path = keyVal[1].Trim();
                            break;                      
                    }
                }
            }
            if (DME_Validation.IsNull(cke.Domain))
            {
                cke.Domain = host;
            }
            return cke;
        }

        #region public HttpWebResponse GetResponse()
        /// <summary>
        /// ����һ���µ�����,�����ػ�õĻ�Ӧ
        /// ���ô˷�����Զ���ᴥ��StatusUpdate�¼�.
        /// </summary>
        /// <returns>��Ӧ��HttpWebResponse</returns>
        public HttpWebResponse GetResponse()
        {
            HttpWebRequest req = CreateRequest();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            responseHeaders = res.Headers;
            string host = req.Headers.Get("Host");
            if (keepContext && !DME_Validation.IsNull(responseHeaders.Get("Set-Cookie")))
            {
                string[] Set_Cookie = DME_String.SplitString(responseHeaders.Get("Set-Cookie"), ",");
                CookieCollection New_Cookies = new CookieCollection();

                foreach (string str in Set_Cookie)
                {
                    if (Set_HeadersCookie(str, host) != null)
                    {
                        New_Cookies.Add(Set_HeadersCookie(str, host));
                    }
                }


                if (context.Cookies != null)
                {
                    foreach (Cookie ck in New_Cookies)
                    {
                        if (res.Cookies[ck.Name] != null)
                        {
                            res.Cookies[ck.Name].Value = ck.Value;
                            
                        }
                        else
                        {
                            res.Cookies.Add(ck);
                        }
                    }
                    
                    foreach (Cookie ck in res.Cookies)
                    {
                        if (context.Cookies[ck.Name] != null)
                        {
                            context.Cookies[ck.Name].Value = ck.Value;
                        }
                        else
                        {
                            context.Cookies.Add(ck);
                        }
                    }
                }
                else
                {
                    context.Cookies = New_Cookies;
                }
                context.Referer = url;
            }
            return res;
        }
        #endregion

        #region public Stream GetStream()
        /// <summary>
        /// ����һ���µ�����,�����ػ�Ӧ���ݵ���
        /// ���ô˷�����Զ���ᴥ��StatusUpdate�¼�.
        /// </summary>
        /// <returns>������Ӧ�������ݵ���</returns>
        public Stream GetStream()
        {
            return GetResponse().GetResponseStream();
        }
        #endregion

        #region public byte[] GetBytes()
        /// <summary>
        /// ����һ���µ�����,�����ֽ�������ʽ���ػ�Ӧ������
        /// ���ô˷����ᴥ��StatusUpdate�¼�
        /// </summary>
        /// <returns>������Ӧ�������ݵ��ֽ�����</returns>
        public byte[] GetBytes()
        {
            HttpWebResponse res = GetResponse();
            int length = (int)res.ContentLength;

            MemoryStream memoryStream = new MemoryStream();
            byte[] buffer = new byte[0x100];
            Stream rs = res.GetResponseStream();
            for (int i = rs.Read(buffer, 0, buffer.Length); i > 0; i = rs.Read(buffer, 0, buffer.Length))
            {
                memoryStream.Write(buffer, 0, i);
                OnStatusUpdate(new DMEWeb_StatusUpdateEventArgs((int)memoryStream.Length, length));
            }
            rs.Close();
            return memoryStream.ToArray();
        }
        #endregion

        #region public string GetString()
        /// <summary>
        /// ����һ���µ�����,��Httpͷ,��Html Meta��ǩ,��DefaultEncodingָʾ�ı�����Ϣ�Ի�Ӧ�������
        /// ���ô˷����ᴥ��StatusUpdate�¼�
        /// </summary>
        /// <returns>�������ַ���</returns>
        public string GetString()
        {
            byte[] data = GetBytes();
            string encodingName = GetEncodingFromHeaders();

            if (encodingName == null)
            {
                encodingName = GetEncodingFromBody(data);
            }
            Encoding encoding;
            if (encodingName == null)
                encoding = defaultEncoding;
            else
            {
                try
                {
                    encoding = Encoding.GetEncoding(encodingName);
                }
                catch (ArgumentException)
                {
                    encoding = defaultEncoding;
                }
            }
            return encoding.GetString(data);
        }
        #endregion

        #region public string GetString(Encoding encoding)
        /// <summary>
        /// ����һ���µ�����,�Ի�Ӧ������������ָ���ı�����н���
        /// ���ô˷����ᴥ��StatusUpdate�¼�
        /// </summary>
        /// <param name="encoding">ָ���ı���</param>
        /// <returns>�������ַ���</returns>
        public string GetString(Encoding encoding)
        {
            byte[] data = GetBytes();
            return encoding.GetString(data);
        }
        #endregion

        #region public int HeadContentLength()
        /// <summary>
        /// ����һ���µ�Head����,��ȡ��Դ�ĳ���
        /// ����������PostingData, Files, StartPoint, EndPoint, Verb
        /// </summary>
        /// <returns>���ص���Դ����</returns>
        public int HeadContentLength()
        {
            Reset();
            DMEWeb_HttpVerb lastVerb = verb;
            verb = DMEWeb_HttpVerb.HEAD;
            using (HttpWebResponse res = GetResponse())
            {
                verb = lastVerb;
                return (int)res.ContentLength;
            }
        }
        #endregion

        #region public void SaveAsFile(string fileName)
        /// <summary>
        /// ����һ���µ�����,�ѻ�Ӧ���������ݱ��浽�ļ�
        /// ���ô˷����ᴥ��StatusUpdate�¼�
        /// ���ָ�����ļ�����,���ᱻ����
        /// </summary>
        /// <param name="fileName">Ҫ������ļ�·��</param>
        public void SaveAsFile(string fileName)
        {
            SaveAsFile(fileName, DMEWeb_FileExistsAction.Overwrite);
        }
        #endregion

        #region public bool SaveAsFile(string fileName, DMEWeb_FileExistsAction existsAction)
        /// <summary>
        /// ����һ���µ�����,�ѻ�Ӧ���������ݱ��浽�ļ�
        /// ���ô˷����ᴥ��StatusUpdate�¼�
        /// </summary>
        /// <param name="fileName">Ҫ������ļ�·��</param>
        /// <param name="existsAction">ָ�����ļ�����ʱ��ѡ��</param>
        /// <returns>�Ƿ���Ŀ���ļ�д��������</returns>
        public bool SaveAsFile(string fileName, DMEWeb_FileExistsAction existsAction)
        {
            byte[] data = GetBytes();
            switch (existsAction)
            {
                case DMEWeb_FileExistsAction.Overwrite:
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write)))
                        writer.Write(data);
                    return true;

                case DMEWeb_FileExistsAction.Append:
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write)))
                        writer.Write(data);
                    return true;

                default:
                    if (!File.Exists(fileName))
                    {
                        using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
                            writer.Write(data);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
        }
        #endregion
        #endregion

    }
}
