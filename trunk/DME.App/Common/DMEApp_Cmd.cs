using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DME.App.Common
{
    /// <summary>
    /// CMD执行命令类
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
    public class DMEApp_Cmd
    {
        #region 私有变量
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        #endregion

        #region 私有函数
        #endregion

        #region 公开函数
        /// <summary>执行cmd.exe命令</summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>命令输出文本</returns>
        public string ExeCommand(string commandText)
        {
            return ExeCommand(new string[] { commandText });
        }

        /// <summary>执行多条cmd.exe命令</summary>
        /// <param name="commandTexts">命令文本数组</param>
        /// <returns>命令输出文本</returns>
        public string ExeCommand(string[] commandTexts)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            string strOutput = null;
            try
            {
                p.Start();
                foreach (string item in commandTexts)
                {
                    p.StandardInput.WriteLine(item);
                }
                p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();
                //strOutput = Encoding.UTF8.GetString(Encoding.Default.GetBytes(strOutput));
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }

        /// <summary>启动外部Windows应用程序，隐藏程序界面</summary>
        /// <param name="appName">应用程序路径名称</param>
        /// <returns> true表示成功，false表示失败</returns>
        public bool StartApp(string appName)
        {
            return StartApp(appName, ProcessWindowStyle.Hidden);
        }

        /// <summary>启动外部应用程序</summary>
        /// <param name="appName">应用程序路径名称</param>
        /// <param name="style">进程窗口模式</param>
        /// <returns>true表示成功，false表示失败</returns>
        public bool StartApp(string appName, ProcessWindowStyle style)
        {
            return StartApp(appName, null, style);
        }

        /// <summary>启动外部应用程序</summary>
        /// <param name="appName">应用程序路径名称</param>
        /// <param name="arguments">启动参数</param>
        /// <param name="style">进程窗口模式</param>
        /// <returns> true表示成功，false表示失败</returns>
        public bool StartApp(string appName, string arguments, ProcessWindowStyle style)
        {
            bool blnRst = false;
            Process p = new Process();
            p.StartInfo.FileName = appName;//exe,bat and so on
            p.StartInfo.WindowStyle = style;
            p.StartInfo.Arguments = arguments;
            try
            {
                p.Start();
                p.WaitForExit();
                p.Close();
                blnRst = true;
            }
            catch
            {
            }
            return blnRst;
        }

        /// <summary>实现压缩</summary>
        /// <param name="s">要压缩的目录路径</param>
        /// <param name="d">压缩后的文件路径</param>
        /// <param name="rarpath">rar.ext的文件路径</param>
        /// <example>rar("e:/www.svnhost.cn/", "e:/www.svnhost.cn.rar");</example>
        public void Rar(string s, string d,string rarpath)
        {
            ExeCommand(rarpath + " a \"" + d + "\" \"" + s + "\" -ep1");
        }

        /// <summary>实现解压缩</summary>
        /// <param name="s">压缩文件路径</param>
        /// <param name="d">解压的目录路径</param>
        /// <param name="rarpath">rar.ext的文件路径</param>
        public void UnRar(string s, string d, string rarpath)
        {
            ExeCommand(rarpath + " x \"" + s + "\" \"" + d + "\" -o+");
        }
        #endregion
    }
}
