using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DME.Web.Helper
{
    public class DMEWeb_ServerInfo
    {

        /// <summary>
        /// 服务器名
        /// </summary>
        public static string MachineName
        {
            get { return HttpContext.Current.Server.MachineName; }
        }

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public static string IP
        {
            get{return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];}
        }

        /// <summary>
        /// HTTP访问端口
        /// </summary>
        public static string Proxy
        {
            get { return HttpContext.Current.Request.ServerVariables["SERVER_PORT"]; }
        }

        /// <summary>
        /// 服务器域名
        /// </summary>
        public static string ServerName
        {
            get { return HttpContext.Current.Request.ServerVariables["SERVER_NAME"]; }
        }

        /// <summary>
        ///  .NET解释引擎版本
        /// </summary>
        public static string ServerNet
        {
            get { return ".NET CLR" + Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." + Environment.Version.Revision; }
        }

        /// <summary>
        /// 服务器操作系统版本
        /// </summary>
        public static string ServerOS
        {
            get { return Environment.OSVersion.ToString(); }
        }

        /// <summary>
        /// 服务器IIS版本
        /// </summary>
        public static string ServerSoft
        {
            get { return HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"]; }
        }

        /// <summary>
        /// 虚拟服务绝对路径
        /// </summary>
        public static string ServerPath
        {
            get { return HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"]; }
        }

        /// <summary>
        /// 执行文件的绝对路径
        /// </summary>
        public static string ServerFilePath
        {
            get { return HttpContext.Current.Request.ServerVariables["PATH_TRANSLATED"]; }
        }

        /// <summary>
        ///  虚拟目录Session总数 
        /// </summary>
        public static string ServerSessions
        {
            get { return HttpContext.Current.Session.Contents.Count.ToString(); }
        }

        /// <summary>
        ///  虚拟目录Application总数 
        /// </summary>
        public static string ServerApplication
        {
            get { return HttpContext.Current.Application.Contents.Count.ToString(); }
        }

        /// <summary>
        ///  域名主机
        /// </summary>
        public static string ServerHost
        {
            get { return HttpContext.Current.Request.ServerVariables["HTTP_HOST"]; }
        }

        /// <summary>
        ///  服务器区域语言
        /// </summary>
        public static string ServerLanguage
        {
            get { return HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"]; }
        }

        /// <summary>
        ///  服务器时区
        /// </summary>
        public static string ServerArea
        {
            get { return (DateTime.Now - DateTime.UtcNow).TotalHours > 0 ? "+" + (DateTime.Now - DateTime.UtcNow).TotalHours.ToString() : (DateTime.Now - DateTime.UtcNow).TotalHours.ToString(); }
        }

        /// <summary>
        ///  用户信息
        /// </summary>
        public static string ServerUserAgent
        {
            get { return HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"]; }
        }
        
        /// <summary>
        /// CPU个数
        /// </summary>
        public static string CpuSum
        {
            get { return Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"); }
        }

        /// <summary>
        /// CPU类型
        /// </summary>
        public static string CpuTyp
        {
            get { return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"); }
        } 

        /// <summary>
        ///  脚本超时时间
        /// </summary>
        public static string ServerTimeOut
        {
            get { return HttpContext.Current.Server.ScriptTimeout.ToString(); }
        }

        /// <summary>
        ///  开机运行时长 
        /// </summary>
        public static string ServerStart
        {
            get { return ((Double)System.Environment.TickCount / 3600000).ToString("N2"); }
        }

        /// <summary>
        ///  进程开始时间 
        /// </summary>
        public static string ServerPrStart
        {
            get
            {
                string temp;
                try
                {
                    temp = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToString();
                }
                catch
                {
                    temp = "未知";
                }
                return temp;
            }
        }

        /// <summary>
        ///  AspNet CPU时间
        /// </summary>
        public static string AspNetCPU
        {
            get
            {
                string temp;
                try
                {
                    temp = ((TimeSpan)System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime).TotalSeconds.ToString("N0");
                }
                catch
                {
                    temp = "未知";
                }
                return temp;
            }
        }

        /// <summary>
        ///  AspNet 内存占用
        /// </summary>
        public static string AspNetM
        {
            get
            {
                string temp;
                try
                {
                    temp = ((Double)System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2");
                }
                catch
                {
                    temp = "未知";
                }
                return temp;
            }
        }
        
    }
}
