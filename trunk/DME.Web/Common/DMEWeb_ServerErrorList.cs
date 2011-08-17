using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using DME.Web.Helper;
using DME.Base.Helper;
using System.Web.SessionState;

namespace DME.Web.Common
{
    /// <summary>
    /// 提供异常日志的Web浏览功能
    /// </summary>
    /// <example>
    /// <p>此功能是uoLib提供的特色功能，也是最推荐使用的功能。使用起来非常方便，在项目中引用uoLib之后只需要配置Global.asax即可。</p>
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
    /// <p>这样设置之后，项目中一旦产生了运行时异常将自动被以日志形式记录下来。默认的日志类型是.htm文件，记录在~/Errors/目录下（需要有写入权限），您也可以通过配置<see cref="uoLib.Web.Debugger.Configuration"/>来设置日志类型、存储路径，甚至关闭此功能。</p>
    /// <p>如果您希望能以Web页面的形式访问到系统记录的异常日志，只需要配置web.config中httpHandlers一节即可：</p>
    /// <code><![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <configuration>
    ///     <system.web>
    ///         <httpHandlers>
    ///             <add verb="*" path="/ErrorList.aspx" type="DME.Web.Common.DMEWeb_ServerErrorList,DME.Web"/>
    ///         </httpHandlers>
    ///     </system.web>
    /// </configuration>]]></code>
    /// <p>其中“ErrorList.aspx”不必存在这个.aspx文件。</p>
    /// </example>
    public class DMEWeb_ServerErrorList : IHttpHandler, IRequiresSessionState
    {
        Regex AllLogRegex = new Regex(@"^Err_[\d]{4}-[\d]{2}-[\d]{2}_[\d]{6}_[\d]{6}\.(?:htm|txt)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #region IHttpHandler 成员

        /// <summary>
        /// 是否可以被多线程同时使用
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string pageContent = string.Empty;
            string act = context.Request.QueryString["act"];
            string file = context.Request.QueryString["file"];

            string errfolder = DMEWeb_ErrorReporter.LogPath;
            string filePath; DirectoryInfo di;

            //当网站在虚拟目录下，而errfolder以“/”开头指向了主网站，则context.Server.MapPath(errfolder)可能会产生异常
            //主要原因是 errfolder 指向了虚拟目录以外的网址
            filePath = Path.Combine(errfolder, file ?? string.Empty);
            di = new DirectoryInfo(errfolder);

            if (act == "checkLogin")
            {
                string psw = context.Request.Form["ErrorList.Psw"];
                try
                {
                    if (psw == DMEWeb_ErrorReporter.LogPassword)
                    {
                        context.Session.Add("ErrorList.IsLogin", 1);
                        context.Session.Timeout = 60;
                    }
                    context.Response.Redirect(context.Request.UrlReferrer.AbsolutePath);
                }
                catch
                {
                    context.Response.Write("设置登录凭据失败，请在配置中禁用密码保护功能。");
                    context.Response.End();
                }
            }
            else if (act == "logOut")
            {
                try
                {
                    context.Session.Remove("ErrorList.IsLogin");
                    context.Response.Redirect(context.Request.UrlReferrer.AbsolutePath);
                }
                catch
                {
                    context.Response.Write("设置登录凭据失败，请在配置中禁用密码保护功能。");
                    context.Response.End();
                }
            }
            else if (act == "del" && !DME_Validation.IsNull(file))
            {
                FileInfo fi = new FileInfo(filePath);
                try
                {
                    if (fi.Exists) { fi.Delete(); }
                }
                catch (System.UnauthorizedAccessException)
                {
                    pageContent = string.Format("<p class=\"att\">对路径“{0}”的访问被拒绝。</p>", filePath);
                }
            }
            else if (act == "lock" && !DME_Validation.IsNull(file))
            {
                FileInfo fi = new FileInfo(filePath);
                try
                {
                    if (fi.Exists)
                    {
                        if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            fi.Attributes &= FileAttributes.Archive | FileAttributes.Compressed | FileAttributes.Device | FileAttributes.Directory | FileAttributes.Encrypted | FileAttributes.Hidden | FileAttributes.Normal | FileAttributes.NotContentIndexed | FileAttributes.Offline | FileAttributes.ReparsePoint | FileAttributes.SparseFile | FileAttributes.System | FileAttributes.Temporary;
                        }
                        else
                        {
                            fi.Attributes |= FileAttributes.ReadOnly;
                        }
                    }
                }
                catch (System.UnauthorizedAccessException)
                {
                    pageContent = string.Format("<p class=\"att\">对路径“{0}”的访问被拒绝。</p>", filePath);
                }
            }
            else if (act == "lockAll")
            {
                try
                {
                    FileInfo[] fis = di.GetFiles();
                    if (fis != null && fis.Length > 0)
                    {
                        foreach (FileInfo f in fis)
                        {
                            //若文件已锁定，则忽略
                            if ((f.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) continue;
                            if (AllLogRegex.IsMatch(f.Name)) f.Attributes |= FileAttributes.ReadOnly;
                        }
                    }
                    context.Response.Redirect(context.Request["SCRIPT_NAME"]);
                }
                catch (Exception ex)
                {
                    pageContent = string.Format("<p class=\"att\">发生错误：{0}</p>", ex.Message);
                }
            }
            else if (act == "unlockAll")
            {
                try
                {
                    FileInfo[] fis = di.GetFiles();
                    if (fis != null && fis.Length > 0)
                    {
                        foreach (FileInfo f in fis)
                        {
                            if ((f.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly && AllLogRegex.IsMatch(f.Name))
                                f.Attributes &= FileAttributes.Archive | FileAttributes.Compressed | FileAttributes.Device | FileAttributes.Directory | FileAttributes.Encrypted | FileAttributes.Hidden | FileAttributes.Normal | FileAttributes.NotContentIndexed | FileAttributes.Offline | FileAttributes.ReparsePoint | FileAttributes.SparseFile | FileAttributes.System | FileAttributes.Temporary;
                        }
                    }
                    context.Response.Redirect(context.Request["SCRIPT_NAME"]);
                }
                catch (Exception ex)
                {
                    pageContent = string.Format("<p class=\"att\">发生错误：{0}</p>", ex.Message);
                }
            }
            else if (act == "delAll")
            {
                try
                {
                    FileInfo[] fis = di.GetFiles();
                    if (fis != null && fis.Length > 0)
                    {
                        foreach (FileInfo f in fis)
                        {
                            //若文件已锁定，则忽略
                            if ((f.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) continue;

                            if (AllLogRegex.IsMatch(f.Name))
                            {
                                f.Delete();
                            }
                        }
                    }
                    context.Response.Redirect(context.Request["SCRIPT_NAME"]);
                }
                catch (Exception ex)
                {
                    pageContent = string.Format("<p class=\"att\">发生错误：{0}</p>", ex.Message);
                }
            }
            else
            {
                pageContent = ShowBody(context).ToString();
            }


            if (!string.IsNullOrEmpty(pageContent))
            {
                StringBuilder sb = ShowHeader(context);

                sb.Append(pageContent);

                sb.Append(ShowFooter(context));
                context.Response.Write(sb);
            }
            else
            {
                context.Response.Redirect(context.Request.ServerVariables["URL"]);
            }
        }
        #endregion

        private static StringBuilder ShowHeader(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.AppendLine("<head>");

            sb.AppendLine("<!--=============================================================================");
            sb.AppendLine(string.Format("\tRecorded by DME.Web Version {0}, Do Maker Exchange", DMEWeb_Info.Version));
            sb.AppendLine(string.Format("\tPublic Url: {0}", DMEWeb_Info.HelpLink));
            sb.AppendLine();
            sb.AppendLine("\t.Net Framework: " + Environment.Version.ToString());
            sb.AppendLine("\tEnvironment.OSVersion: " + Environment.OSVersion.VersionString);
            sb.AppendLine(string.Format("\tDateTime: {0}", DateTime.Now.ToString()));
            sb.AppendLine("=============================================================================-->");

            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.AppendLine("<meta name=\"robots\" content=\"index,nofollow\" />");

            sb.AppendLine("<title>错误日志列表 [Responsed by uoLib V" + DMEWeb_Info.Version.ToString() + ", udnz.com]</title>");

            sb.AppendLine("<style>");
            sb.AppendLine("body{padding:0;margin:0;}");
            sb.AppendLine("body,p,li{font-size:12px;line-height:150%;font-family:Arial;}");
            sb.AppendLine("p{padding-left:2em;}");
            sb.AppendLine(".att {color:#F00;}");
            sb.AppendLine(".detail {color:#999;}");
            sb.AppendLine(".TOP {padding:10px 30px; background-color:#0000AA;}");
            sb.AppendLine("H1 { font-family:\"微软雅黑\",\"宋体\";font-weight:normal;font-size:16pt;color:#FFF;line-height:150%;}");
            sb.AppendLine("a {text-decoration:none;font-family:\"Verdana\";color:#000}");
            sb.AppendLine("a:visited {color:#777;}");
            sb.AppendLine("a:hover {text-decoration:none;color:#F00;border-bottom:1px solid #F00;padding-bottom:2px;}");
            sb.AppendLine(".mainContainer ol{margin:0 6em;}");
            sb.AppendLine(".mainContainer ol li{border-bottom:1px dotted #CCC;}");
            sb.AppendLine(".operLink a {color:#00F}");
            sb.AppendLine(".locked,.locked a,.locked .operLink a,a.locked {color:#06A}");
            sb.AppendLine(".del,a.del {color:#F00}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.Append(string.Format("<div class=\"TOP\"><h1>\"{0}\" 应用程序中的错误日志</h1></div>", context.Request.ApplicationPath));
            sb.AppendLine("");
            return sb;
        }
        private static StringBuilder ShowBody(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            bool isLogin = CheckLogin(context);
            if (isLogin)
            {
                #region 显示日志文件列表
                sb.Append("<div class=\"mainContainer\">");
                if (DMEWeb_ErrorReporter.IsRecordErrors == false)
                {
                    sb.Append("<p class=\"att\">已停止记录错误日志。</p>");
                }
                else
                {
                    string errfolder = DMEWeb_ErrorReporter.LogPath;
                    DirectoryInfo di = new DirectoryInfo(errfolder);
                    if (!di.Exists)
                    {
                        sb.Append(string.Format("<p class=\"att\">日志存储目录不存在。（{0}）</p>", di.FullName));
                    }
                    else
                    {
                        try
                        {
                            FileInfo[] files = di.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                            if (files == null || files.Length == 0)
                            {
                                sb.Append("<p>尚无错误日志。</p>");
                            }
                            else
                            {
                                string operLine = string.Format("<p>本页展示的错误日志由 <a href=\"{0}\" target=\"_blank\"><strong style=\"color:#000000\">DME.Web</strong></a> 自动记录，您可以在Global.asax中配置日志的存储路径、文件类型或关闭日志记录。"
                                                        + "<span class=\"operLink\">[ <a href=\"{0}\" target=\"_blank\">查看帮助</a> | "
                                                        + "<a href=\"?act=delAll\" onclick=\"return confirm('要删除所有未锁定的记录吗？');\">删除所有记录</a> | "
                                                        + "<a href=\"?act=lockAll\" onclick=\"return confirm('要锁定所有记录吗？');\">全部锁定</a> | "
                                                        + "<a href=\"?act=unlockAll\" onclick=\"return confirm('要解锁所有记录吗？');\">全部解锁</a>"
                                                        + " ]</span></p>",
                                                        DMEWeb_Info.HelpLink);

                                sb.Append(operLine);

                                sb.Append("<ol>");

                                //倒序，因此专门用一个 StringBuilder
                                StringBuilder listSb = new StringBuilder();
                                bool isLocked;
                                foreach (FileInfo fi in files)
                                {
                                    isLocked = (fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;

                                    listSb.Insert(0, "</li>");
                                    string detail = "";
                                    string fileSize = "";
                                    using (StreamReader sr = new StreamReader(fi.FullName, Encoding.UTF8))
                                    {
                                        string tmp = sr.ReadToEnd();
                                        sr.Close();
                                        //<ErrorMessage>标签来自于ShowServerError。
                                        Regex re = new Regex("<ErrorMessage>(.*)</ErrorMessage>");
                                        if (re.IsMatch(tmp)) { detail = re.Match(tmp).Result("$1"); }
                                        fileSize = "(" + DME_Files.FormatFileSize(fi.Length) + ")";
                                    }

                                    string opLink;
                                    if (isLocked)
                                    {
                                        opLink = string.Format("<a href=\"?act=lock&file={0}\">解锁</a>", fi.Name);
                                    }
                                    else
                                    {
                                        opLink = string.Format("<a href=\"?act=del&file={0}\" class=\"del\">删除</a> | <a href=\"?act=lock&file={0}\" class=\"locked\">锁定</a>", fi.Name);
                                    }

                                    listSb.Insert(0, string.Format("<a href=\"{1}{0}\" target=\"_blank\">{0}</a> - [ {4} ] {3} <span class=\"detail\">{2}</span>",
                                        fi.Name, errfolder.Replace(DME_Path.MapPath(@"\"), ""), context.Server.HtmlEncode(detail), fileSize, opLink));

                                    if (isLocked)
                                    {
                                        listSb.Insert(0, "<li class=\"locked\">");
                                    }
                                    else
                                    {
                                        listSb.Insert(0, "<li>");
                                    }
                                }

                                sb.Append(listSb);
                                sb.Append("</ol>");

                                sb.Append("<br /><br />");
                                sb.Append("<p><span class=\"del\">删除</span> - 删除该文件，不再显示。&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"locked\">解锁</span> - 已锁定的文件，只读。&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"locked\">锁定</span> - 将文件设为只读，批量删除时不删除该记录。<br />批量操作仅对由本程序生成的错误日志生效。<br /><br /></p>");
                                if (files != null && files.Length > 25) sb.Append(operLine);
                            }
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            sb.Append(string.Format("<p class=\"att\">对路径“{0}”的访问被拒绝。</p>", di.FullName));
                        }
                    }
                }
                sb.Append("</div>");
                #endregion
            }
            else
            {
                sb.Append("<div class=\"mainContainer\">");
                sb.Append("<form action=\"?act=checkLogin\" method=\"POST\">");
                sb.Append("<p>");
                sb.Append("请输入管理密码：<br />");
                sb.Append("<input type=\"password\" name=\"ErrorList.Psw\" />");
                sb.Append("<input type=\"submit\" name=\"ErrorList.BuCheck\" value=\"登录\" /><br />");
                sb.Append("</p>");
                sb.Append("</form>");
                sb.Append("</div>");
            }

            return sb;
        }
        private static bool CheckLogin(HttpContext context)
        {
            try
            {
                bool isOk = context.Session["ErrorList.IsLogin"].ToString() == "1";
                return isOk;
            }
            catch
            {
                return false;
            }
        }
        private static StringBuilder ShowFooter(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<p><a href=\"{0}\" target=\"_blank\">Responsed by DME.Web V{1}, Do Maker Exchange</a></p>", DMEWeb_Info.HelpLink, DMEWeb_Info.Version));
            sb.Append("<br /><br />");
            sb.AppendLine("</body></html>");

            return sb;
        }
    }
}
