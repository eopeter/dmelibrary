using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using DME.Base.IO;
using System.Reflection;
using System.Threading;
using DME.Base.Helper;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace DME.Web.Helper
{
    //注意：由于此为静态类，其下全是静态方法，因此其中所有引用的Page实例必须在方法内部声明
    //否则点击网页后退按钮后，Page实例将会丢失，将会抛出“服务器无法刷新已完成的响应。”的错误！
    /// <summary>
    /// 提供运行时异常的日志记录
    /// </summary>
    /// <example>
    /// <p>此功能是DME.Web提供的特色功能，也是最推荐使用的功能。使用起来非常方便，在项目中引用DME.Web之后只需要配置Global.asax即可。</p>
    /// <code><![CDATA[
    /// protected void Application_Error(object sender, EventArgs e)
    /// {
    ///     if (HttpContext.Current.AllErrors.Length > 0)
    ///     { 
    ///         foreach (Exception ex in HttpContext.Current.AllErrors) 
    ///         {
    ///             DME.Web.Helper.DMEWeb_ErrorReporter.RecordErrors(ex); 
    ///         }
    ///     }
    /// }]]></code>
    /// <p>这样设置之后，项目中一旦产生了运行时异常将自动被以日志形式记录下来。
    /// 默认的日志类型是.htm文件，记录在~/Errors/目录下（需要有写入权限），
    /// 您也可以通过在Global.asax中配置来设置日志类型、存储路径，甚至关闭此功能。</p>
    /// <code><![CDATA[
    /// protected void Application_Start(object sender, EventArgs e)  {
    ///     // 以下为不设置时的默认值，您可以根据实际情况自行设置其他值。
    ///     DME.Web.Helper.ErrorReporter.LogType = LogType.Htm;
    ///     DME.Web.Helper.ErrorReporter.LogPath = "~/Errors/";
    /// }  
    /// ]]></code>
    /// <p>如果您希望能以Web页面的形式访问到系统记录的异常日志，只需要配置web.config中httpHandlers一节即可：</p>
    /// <code><![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <configuration>
    ///     <system.web>
    ///         <httpHandlers>
    ///             <add verb="*" path="Errors/List.aspx" type="DME.Web.Common.DMEWeb_ServerErrorList,DME.Web"/>
    ///         </httpHandlers>
    ///     </system.web>
    /// </configuration>
    /// ]]></code>
    /// <p>其中“Errors/List.aspx”可以是您希望的任意路径，不必存在这个.aspx文件。</p>
    /// </example>
    public static partial class DMEWeb_ErrorReporter
    {
        private static string _LogDir = string.Empty;
        private static string _LogPassword = string.Empty;
        private static bool _isRecord = true;
        private static LogType _type = LogType.Htm;
        /// <summary>锁</summary>
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        
        /// <summary>
        /// 是否将捕捉的运行时异常以日志形式保存下来。默认为 true。
        /// </summary>
        public static bool IsRecordErrors 
        {
            get { return _isRecord; } 
            set { _isRecord = value; } 
        }
        /// <summary>
        /// 错误日志的保存类型
        /// </summary>
        public static LogType LogType 
        { 
            get { return _type; } 
            set { _type = value; } 
        }

        /// <summary>
        /// 日志目录
        /// </summary>
        public static String LogPath
        {

            get
            {
                if (!DME_Validation.IsNull(_LogDir))
                {
                    return _LogDir;
                }

                _LogDir = DME.Base.DME_LibraryConfig.DME_LogPath;
                return _LogDir;
            }
        }

        public static String LogPassword
        {
            get
            {
                if (!DME_Validation.IsNull(_LogPassword))
                {
                    return _LogPassword;
                }

                _LogPassword = DME.Base.DME_LibraryConfig.DME_LogPassword;
                return _LogPassword;
            }
        }

        /// <summary>
        /// 递归指定异常的内部异常（包括该异常本身）
        /// </summary>
        /// <param name="ex">指定的异常</param>
        /// <param name="exStack">包所有内部异常的集合</param>
        /// <remarks>
        /// 当前异常先于其内部异常被插入到 Stack 集合。
        /// </remarks>
        private static void GetErrorStack(Exception ex, ref Stack<Exception> exStack)
        {
            if (ex == null) return;
            exStack.Push(ex);

            if (ex.InnerException != null)
            {
                exStack.Push(ex.InnerException);
                GetErrorStack(ex.InnerException, ref exStack);
            }
        }

        private static string BuildErrorLogContent(Exception ex, LogType logType)
        {
            HttpContext Page = HttpContext.Current;

            StringBuilder sb = new StringBuilder();

            //通过递归生成一个包含所有异常的堆
            Stack<Exception> exStack = new Stack<Exception>();
            GetErrorStack(ex, ref  exStack);

            if (logType == LogType.Html || logType == LogType.Htm)
            {
                #region HTML记录日志

                #region 初始化一些信息
                if (ex == null) { ex = Page.Error; }
                if (ex == null)
                {
                    ex = new Exception("DME.Web.Helper.DMEWeb_ErrorReporter.ShowServerError(ex) 引发异常：HttpContext.Current.Error或ex为空引用。");
                    ex.Source = "DME.Web.Helper.DMEWeb_ErrorReporter.ShowServerError(ex)";
                    ex.HelpLink = DMEWeb_Info.HelpLink;
                }
                if (string.IsNullOrEmpty(ex.HelpLink)) ex.HelpLink = DMEWeb_Info.HelpLink;
                #endregion

                sb.Append(ShowServerErrorHeader(ex, Page));


                sb.Append(ShowServerErrorBody(exStack, Page));
                sb.Append(ShowServerErrorFooter(ex, Page));

                #endregion
            }
            else
            {
                #region 文本记录日志

                sb.AppendLine("=============================================================================");
                sb.AppendLine(string.Format("\tRecorded by DME.Web Version {0}, Do Maker Exchange", DMEWeb_Info.Version));
                sb.AppendLine(string.Format("\tPublic Url: {0}", DMEWeb_Info.HelpLink));
                sb.AppendLine();
                sb.AppendLine("\t.Net Framework: " + Environment.Version.ToString());
                sb.AppendLine("\tEnvironment.OSVersion: " + Environment.OSVersion.VersionString);
                sb.AppendLine(string.Format("\tDateTime: {0}", DateTime.Now.ToString()));
                sb.AppendLine("=============================================================================");

                int tmpN = exStack.Count;
                //用于日志列表页的隐藏信息
                string tmpDetail = "";

                foreach (Exception e in exStack)
                {
                    sb.AppendLine();
                    sb.AppendLine(string.Format("Stack<Exception>[{0}]:", --tmpN));
                    sb.AppendLine(string.Format("{0}", e.Message));
                    sb.AppendLine();
                    sb.AppendLine(string.Format("类型（Exception.GetType()）：{0}", ex.GetType()));
                    sb.AppendLine(string.Format("来源（Exception.Source）：{0}", ex.Source));
                    MethodBase mb = ex.TargetSite;
                    if (mb != null)
                    {
                        sb.AppendLine(string.Format("类对象（Exception.TargetSite.ReflectedType）：{0}", mb.ReflectedType));
                    }
                    sb.AppendLine();
                    sb.AppendLine(string.Format("请求的 URL：{0}{1}", Page.Request.UserHostName, Page.Request.RawUrl));
                    sb.AppendLine(e.StackTrace);
                    sb.AppendLine();
                    sb.AppendLine("-------------------------------------------------------------------------------------------------------------------");

                    tmpDetail = string.Format("{0}: {1}", e.GetType(), e.Message.Replace(System.Environment.NewLine, ""));
                }

                //<ErrorMessage>标签在ServerErrorList中有使用。
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("<!-- 用于日志列表页的隐藏信息");
                sb.AppendLine(string.Format("<ErrorMessage>{0}</ErrorMessage>", tmpDetail));
                sb.AppendLine("用于日志列表页的隐藏信息 -->");

                sb.AppendLine();
                #endregion
            }

            return sb.ToString();
        }

        /// <summary>
        /// 以日志形式记录运行时异常
        /// </summary>
        /// <param name="ex"></param>
        public static void RecordErrors(Exception ex)
        {
            RecordErrors(BuildErrorLogContent(ex, LogType), LogType);
        }
        private static void RecordErrors(string logContent, LogType logType)
        {
            if (IsRecordErrors)
            {
                string logFileExt;
                switch (logType)
                {
                    case LogType.Txt:
                        logFileExt = "txt"; break;
                    case LogType.Html:
                        logFileExt = "html"; break;
                    case LogType.Htm:
                        logFileExt = "htm"; break;
                    case LogType.Log:
                        logFileExt = "log"; break;
                    default:
                        logFileExt = "uolib"; break;
                }

                HttpContext Page = HttpContext.Current;
                if (Page == null)
                {
                    HttpRequest request = new HttpRequest("PageIsNull.htm", "http://127.0.0.1/", string.Empty);
                    HttpResponse response = new HttpResponse(null);
                    Page = new HttpContext(request, response);
                }

                string logFile = Path.Combine(LogPath, string.Format("Err_{0}.{1}", DateTime.Now.ToString("yyyy-MM-dd_HHmmss_ffffff"), logFileExt));

                DirectoryInfo di = new DirectoryInfo(LogPath);
                if (!di.Exists) { di.Create(); }
                FileInfo fi = new FileInfo(logFile);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.AutoFlush = true;
                    sw.Write(logContent);
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 输出当前抛出的错误并执行Response.End()方法。
        /// </summary>
        /// <param name="ex"></param>
        public static void ShowServerError(Exception ex)
        {
            ShowServerError(ex, true);
        }
        /// <summary>
        /// 输出当前抛出的错误。
        /// </summary>
        /// <param name="ex">当前要输出的异常对象</param>
        /// <param name="isResponseEnd">是否调用Page.Response.End();</param>
        public static void ShowServerError(Exception ex, bool isResponseEnd)
        {
            HttpContext Page = HttpContext.Current;
            if (Page == null)
            {
                HttpRequest request = new HttpRequest("PageIsNull.htm", "http://127.0.0.1/", string.Empty);
                HttpResponse response = new HttpResponse(null);
                Page = new HttpContext(request, response);
            }

            string logContent = BuildErrorLogContent(ex, LogType);
            RecordErrors(logContent, LogType);

            Page.Response.Write(logContent);
            Page.Server.ClearError();
            if (isResponseEnd == true)
            {
                try
                {
                    Page.Response.End();
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
            }
        }

        #region 为 ShowServerError 提供支持的部分
        private static StringBuilder ShowServerErrorHeader(Exception ex, HttpContext Page)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.AppendLine("<head>");

            sb.AppendLine("<!--=============================================================================");
            sb.AppendLine(string.Format("\tRecorded by uoLib Version {0}, udnz.com", DMEWeb_Info.Version));
            sb.AppendLine(string.Format("\tPublic Url: {0}", DMEWeb_Info.HelpLink));
            sb.AppendLine();
            sb.AppendLine("\t.Net Framework: " + Environment.Version.ToString());
            sb.AppendLine("\tEnvironment.OSVersion: " + Environment.OSVersion.VersionString);
            sb.AppendLine(string.Format("\tDateTime: {0}", DateTime.Now.ToString()));
            sb.AppendLine("=============================================================================-->");

            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.AppendLine("<meta name=\"robots\" content=\"none\" />");

            sb.Append("<title>\"");
            if (Page == null)
            {
                sb.Append("(Special Error: HttpContext.Current == null)");
            }
            else
            {
                sb.Append(Page.Request.ApplicationPath);
            }
            sb.Append("\"");
            sb.Append(" 应用程序中的服务器错误.[Responsed by uoLib V" + DMEWeb_Info.Version.ToString() + ", Do Maker Exchange]");
            sb.AppendLine("</title>");

            sb.AppendLine("<style>");
            sb.AppendLine("body,td{font-size:12px;line-height:150%;font-family:Arial;background-color:#EFEFEF;}");
            sb.AppendLine("H1 { font-family:\"Verdana\";font-weight:normal;font-size:18pt;color:red }");
            sb.AppendLine("H2 { font-family:\"Verdana\";font-weight:normal;font-size:12pt;color:maroon;line-height:150%;}");
            sb.AppendLine("H2 i { font-size:12px; font-style:italic;}");
            sb.AppendLine("pre {font-family:Courier New,\"Lucida Console\";font-size:11px;line-height:normal; background-color:#FFFFEE; padding:10px;}");
            sb.AppendLine("hr {height:1px;}");
            sb.AppendLine("li {margin:0 auto;}");
            sb.AppendLine("a {text-decoration:none;font-family:\"Verdana\";}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.Append("<h1>\"");
            if (Page == null)
            {
                sb.Append("(Special Error: HttpContext.Current == null)");
            }
            else
            {
                sb.Append(Page.Request.ApplicationPath);
            }
            sb.AppendLine("\" 应用程序中的服务器错误。</h1>");
            return sb;
        }
        private static StringBuilder ShowServerErrorBody(Stack<Exception> exStack, HttpContext Page)
        {
            StringBuilder sb = new StringBuilder();

            int tmpN = exStack.Count;
            //用于日志列表页的隐藏信息
            string tmpDetail = "";

            foreach (Exception e in exStack)
            {
                sb.Append("<h2>");
                sb.Append(string.Format("<i>Stack&lt;Exception&gt;[{0}]:</i><br />", --tmpN));
                sb.Append(Page.Server.HtmlEncode(e.Message));
                sb.AppendLine("</h2>");

                MethodBase mb = e.TargetSite;
                if (mb != null)
                {
                    StringBuilder psStr = new StringBuilder();
                    ParameterInfo[] ps = mb.GetParameters();
                    if (ps.Length > 0)
                    {
                        foreach (ParameterInfo p in ps)
                        {
                            psStr.Append(p.ToString());
                            if (p != null && p.DefaultValue != null && !string.IsNullOrEmpty(p.DefaultValue.ToString())) { psStr.Append("[" + p.DefaultValue.ToString() + "]"); }
                            psStr.Append(", ");
                        }
                        psStr.Remove(psStr.Length - 2, 2);
                    }
                    sb.Append("<strong>类型（Exception.GetType()） </strong>：");
                    sb.Append(e.GetType().ToString());

                    sb.Append("<br /><strong>来源（Exception.Source） </strong>：");
                    sb.Append(e.Source);

                    sb.Append("<br /><strong>方法（Exception.TargetSite） </strong>：");
                    Regex re = new Regex("\\(.*\\)", RegexOptions.IgnoreCase);
                    sb.Append(re.Replace(mb.ToString(), string.Format("({0})", psStr.ToString())));

                    sb.Append("<br /><strong>类对象（Exception.TargetSite.ReflectedType） </strong>：");
                    sb.Append(mb.ReflectedType.ToString());
                }

                sb.Append("<br />");

                //sb.Append("<strong>Stack Trace</strong>：<br /><br /><pre>" + EnhancedStackTrace(ex) + "</pre><hr />");

                sb.Append("<pre>[Exception]" + e.StackTrace + "</pre>");
                if (Page != null)
                {
                    sb.Append("<strong>请求的 URL</strong>：");
                    sb.Append(Page.Request.ServerVariables["HTTP_HOST"]);
                    sb.Append(Page.Request.RawUrl);
                }
                sb.Append("<br /><strong>服务器时间</strong>：");
                sb.Append(System.DateTime.Now.ToString());
                sb.AppendLine("<hr />");

                tmpDetail = string.Format("{0}: {1}", e.GetType(), e.Message.Replace(System.Environment.NewLine, ""));
            }

            //<ErrorMessage>标签在ServerErrorList中有使用。
            sb.AppendLine("<!-- 用于日志列表页的隐藏信息");
            sb.AppendLine(string.Format("<ErrorMessage>{0}</ErrorMessage>", tmpDetail));
            sb.AppendLine("用于日志列表页的隐藏信息 -->");

            return sb;
        }
        private static StringBuilder ShowServerErrorFooter(Exception ex, HttpContext Page)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table cellspacing=\"0\" cellpadding=\"3\" border=\"0\" width=\"100%\">");
            sb.Append("<tr><td style=\"width:35%\"><strong>服务端信息：</strong></td><td style=\"width:65%\"><strong>客户端信息：</strong></td>");
            sb.Append("</tr><tr>");
            sb.Append("<td style=\"padding-left:15px;\" valign=\"top\">");
            sb.Append(".Net framework：" + Environment.Version.ToString());
            sb.Append("<br />操作系统：" + Environment.OSVersion.VersionString);
            if (Page != null) sb.Append("<br />运行环境：" + Page.Request.ServerVariables.Get("SERVER_SOFTWARE"));
            sb.Append("</td>");
            sb.Append("<td style=\"padding-left:15px;\" valign=\"top\">");
            if (Page == null || Page.Request.UrlReferrer == null)
            {
                sb.Append("来源：(Null)");
            }
            else
            {
                sb.Append("来源：" + Page.Request.UrlReferrer.ToString());
            }
            if (Page != null) sb.Append("<br />客户端：" + Page.Request.UserAgent);
            if (Page != null) sb.Append("<br />IP地址：" + DMEWeb_String.GetIP());
            sb.Append("</tr></table><hr /><br /><br />");
            sb.Append("<br /><p style=\"color:#555\">");
            sb.Append("请将本页信息保存或截屏，并提交给本站管理员。(" + DMEWeb_Info.AdminEmail + ")");
            sb.Append("<br /><a href=\"" + DMEWeb_Info.HelpLink + "\" target=\"_blank\">");
            sb.Append("程序库内部版本号：");
            sb.Append(DMEWeb_Info.Version.ToString());
            sb.Append("</a>");
            sb.Append("<br />参考链接：<a href=\"" + ex.HelpLink + "\" target=\"_blank\">" + ex.HelpLink + "</a>");
            sb.AppendLine("</p>");
            sb.AppendLine("<!-- Responsed by uoLib(udnz.com) -->");
            sb.AppendLine("</body></html>");

            return sb;
        }
        #region 查错堆栈
        private static string EnhancedStackTrace(Exception ex)
        {
            return EnhancedStackTrace(new StackTrace(ex, true));
        }
        private static string EnhancedStackTrace(StackTrace st)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append("---- Stack Trace ----");
            sb.Append(Environment.NewLine);
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                MemberInfo mi = sf.GetMethod();
                sb.Append(StackFrameToString(sf));
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
        private static string StackFrameToString(StackFrame sf)
        {
            StringBuilder sb = new StringBuilder();
            int intParam;
            MemberInfo mi = sf.GetMethod();
            sb.Append("   ");
            sb.Append(mi.DeclaringType.Namespace);
            sb.Append(".");
            sb.Append(mi.DeclaringType.Name);
            sb.Append(".");
            sb.Append(mi.Name);
            // -- build method params     
            sb.Append("(");
            intParam = 0;
            foreach (ParameterInfo param in sf.GetMethod().GetParameters())
            {
                intParam += 1;
                sb.Append(param.Name);
                sb.Append(" As ");
                sb.Append(param.ParameterType.Name);
            }
            sb.Append(")");
            sb.Append(Environment.NewLine);
            // -- if source code is available, append location info      
            sb.Append("       ");
            if (string.IsNullOrEmpty(sf.GetFileName()))
            {
                sb.Append("(unknown file)");
                //-- native code offset is always available    
                sb.Append(": N ");
                sb.Append(String.Format("{0:#00000}", sf.GetNativeOffset()));
            }
            else
            {
                sb.Append(System.IO.Path.GetFileName(sf.GetFileName()));
                sb.Append(": line ");
                sb.Append(String.Format("{0:#0000}", sf.GetFileLineNumber()));
                sb.Append(", col ");
                sb.Append(String.Format("{0:#00}", sf.GetFileColumnNumber()));
                if (sf.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                {
                    sb.Append(", IL ");
                    sb.Append(String.Format("{0:#0000}", sf.GetILOffset()));
                }
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// 异常日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// .txt 后缀的文本文件
        /// </summary>
        Txt = 0,
        /// <summary>
        /// .html 后缀的网页文件
        /// </summary>
        Html,
        /// <summary>
        /// .htm 后缀的网页文件
        /// </summary>
        Htm,
        /// <summary>
        /// .log 后缀的文本文件
        /// </summary>
        Log,
    }
}
