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

        #region 事件
        public event EventHandler<DMEWeb_StatusUpdateEventArgs> StatusUpdate;

        private void OnStatusUpdate(DMEWeb_StatusUpdateEventArgs e)
        {
            EventHandler<DMEWeb_StatusUpdateEventArgs> temp = StatusUpdate;
            if (temp != null)
                temp(this, e);
        }
        #endregion

        #region 属性
        #region Proxy
        /// <summary>
        /// 要发送的Form表单信息
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
        /// 是否自动在不同的请求间保留Cookie, Referer
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
        /// 期望的回应的语言
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
        /// GetString()如果不能从HTTP头或Meta标签中获取编码信息,则使用此编码来获取字符串
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
        /// 指示发出Get请求还是Post请求
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
        /// 要上传的文件.如果不为空则自动转为Post请求
        /// </summary>
        private readonly List<DMEWeb_HttpUploadingFile> files = new List<DMEWeb_HttpUploadingFile>();
        public List<DMEWeb_HttpUploadingFile> Files
        {
            get { return files; }
        }
        #endregion

        #region PostingData
        /// <summary>
        /// 要发送的Form表单信息
        /// </summary>
        private readonly Dictionary<string, string> postingData = new Dictionary<string, string>();
        public Dictionary<string, string> PostingData
        {
            get { return postingData; }
        }
        #endregion

        #region PostXmlData
        /// <summary>
        /// 要发送的Xml
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
        /// 获取或设置请求资源的地址
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
        /// 用于在获取回应后,暂时记录回应的HTTP头
        /// </summary>
        private WebHeaderCollection responseHeaders;
        public WebHeaderCollection ResponseHeaders
        {
            get { return responseHeaders; }
        }
        #endregion

        #region Accept
        /// <summary>
        /// 获取或设置期望的资源类型
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
        /// 获取或设置请求中的Http头User-Agent的值
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
        /// 获取或设置Cookie及Referer
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
        /// 获取或设置获取内容的起始点,用于断点续传,多线程下载等
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
        /// 获取或设置获取内容的结束点,用于断点续传,多下程下载等.
        /// 如果为0,表示获取资源从StartPoint开始的剩余内容
        /// </summary>
        private int endPoint;
        public int EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }
        #endregion
        #endregion

        #region 构造
        #region public HttpClient()
        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        public DMEWeb_HttpClient()
            : this(null)
        {
        }
        #endregion

        #region public DMEWeb_HttpClient(string url)
        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        /// <param name="url">要获取的资源的地址</param>
        public DMEWeb_HttpClient(string url)
            : this(url, null)
        {
        }
        #endregion

        #region public HttpClient(string url, DMEWeb_HttpClientContext context)
        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        /// <param name="url">要获取的资源的地址</param>
        /// <param name="context">Cookie及Referer</param>
        public DMEWeb_HttpClient(string url, DMEWeb_HttpClientContext context)
            : this(url, context, false)
        {
        }
        #endregion

        #region public DMEWeb_HttpClient(string url, DMEWeb_HttpClientContext context, bool keepContext)
        /// <summary>
        /// 构造新的HttpClient实例
        /// </summary>
        /// <param name="url">要获取的资源的地址</param>
        /// <param name="context">Cookie及Referer</param>
        /// <param name="keepContext">是否自动在不同的请求间保留Cookie, Referer</param>
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

        #region 方法
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
        /// 创建请求
        /// </summary>
        /// <returns>web请求对象</returns>
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
        /// 在请求中添加要上传的文件
        /// </summary>
        /// <param name="fileName">要上传的文件路径</param>
        /// <param name="fieldName">文件字段的名称(相当于&lt;input type=file name=fieldName&gt;)里的fieldName)</param>
        public void AttachFile(string fileName, string fieldName)
        {
            DMEWeb_HttpUploadingFile file = new DMEWeb_HttpUploadingFile(fileName, fieldName);
            files.Add(file);
        }
        #endregion

        #region public void AttachFile(byte[] data, string fileName, string fieldName)
        /// <summary>
        /// 在请求中添加要上传的文件
        /// </summary>
        /// <param name="data">要上传的文件内容</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fieldName">文件字段的名称(相当于&lt;input type=file name=fieldName&gt;)里的fieldName)</param>
        public void AttachFile(byte[] data, string fileName, string fieldName)
        {
            DMEWeb_HttpUploadingFile file = new DMEWeb_HttpUploadingFile(data, fileName, fieldName);
            files.Add(file);
        }
        #endregion

        #region public void Reset()
        /// <summary>
        /// 清空PostingData, Files, StartPoint, EndPoint, ResponseHeaders, 并把Verb设置为Get.
        /// 在发出一个包含上述信息的请求后,必须调用此方法或手工设置相应属性以使下一次请求不会受到影响.
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
        /// 发出一次新的请求,并返回获得的回应
        /// 调用此方法永远不会触发StatusUpdate事件.
        /// </summary>
        /// <returns>相应的HttpWebResponse</returns>
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
        /// 发出一次新的请求,并返回回应内容的流
        /// 调用此方法永远不会触发StatusUpdate事件.
        /// </summary>
        /// <returns>包含回应主体内容的流</returns>
        public Stream GetStream()
        {
            return GetResponse().GetResponseStream();
        }
        #endregion

        #region public byte[] GetBytes()
        /// <summary>
        /// 发出一次新的请求,并以字节数组形式返回回应的内容
        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <returns>包含回应主体内容的字节数组</returns>
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
        /// 发出一次新的请求,以Http头,或Html Meta标签,或DefaultEncoding指示的编码信息对回应主体解码
        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <returns>解码后的字符串</returns>
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
        /// 发出一次新的请求,对回应的主体内容以指定的编码进行解码
        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <param name="encoding">指定的编码</param>
        /// <returns>解码后的字符串</returns>
        public string GetString(Encoding encoding)
        {
            byte[] data = GetBytes();
            return encoding.GetString(data);
        }
        #endregion

        #region public int HeadContentLength()
        /// <summary>
        /// 发出一次新的Head请求,获取资源的长度
        /// 此请求会忽略PostingData, Files, StartPoint, EndPoint, Verb
        /// </summary>
        /// <returns>返回的资源长度</returns>
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
        /// 发出一次新的请求,把回应的主体内容保存到文件
        /// 调用此方法会触发StatusUpdate事件
        /// 如果指定的文件存在,它会被覆盖
        /// </summary>
        /// <param name="fileName">要保存的文件路径</param>
        public void SaveAsFile(string fileName)
        {
            SaveAsFile(fileName, DMEWeb_FileExistsAction.Overwrite);
        }
        #endregion

        #region public bool SaveAsFile(string fileName, DMEWeb_FileExistsAction existsAction)
        /// <summary>
        /// 发出一次新的请求,把回应的主体内容保存到文件
        /// 调用此方法会触发StatusUpdate事件
        /// </summary>
        /// <param name="fileName">要保存的文件路径</param>
        /// <param name="existsAction">指定的文件存在时的选项</param>
        /// <returns>是否向目标文件写入了数据</returns>
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
