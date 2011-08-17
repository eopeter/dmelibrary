using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DME.App.Common
{
    /// <summary>
    /// Application工具类
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
    public sealed class DMEApp_AppEnv
    {
        #region 私有变量
        private static System.Threading.Mutex MutexForSingletonExe = null;
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        /// <summary>
        /// 取当前进程的完整路径，包含文件名(进程名)。
        /// result: X:\xxx\xxx\xxx.exe (.exe文件所在的目录+.exe文件名)
        /// </summary>
        public string AssemblyLocation
        {
            get
            {
                return this.GetType().Assembly.Location;

            }
        }

        /// <summary>
        /// //获取新的 Process 组件并将其与当前活动的进程关联的主模块的完整路径，包含文件名(进程名)。
        /// result: X:\xxx\xxx\xxx.exe (.exe文件所在的目录+.exe文件名)
        /// </summary>
        public string ModuleFileName
        {
            get
            {
                return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            }
        }

        /// <summary>
        /// 获取和设置当前目录（即该进程从中启动的目录）的完全限定路径
        /// result: X:\xxx\xxx (.exe文件所在的目录)
        /// </summary>
        public string CurrentDirectory
        {
            get
            {
                return System.Environment.CurrentDirectory;
            }
        }

        /// <summary>
        /// 获取当前 Thread 的当前应用程序域的基目录，它由程序集冲突解决程序用来探测程序集
        /// result: X:\xxx\xxx\ (.exe文件所在的目录+"\")
        /// </summary>
        public string BaseDirectory
        {
            get
            {
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 获取和设置包含该应用程序的目录的名称。(推荐)
        /// result: X:\xxx\xxx\ (.exe文件所在的目录+"\")
        /// </summary>
        public string ApplicationBase
        {
            get
            {
                return System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }

        /// <summary>
        /// 获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称。
        /// result: X:\xxx\xxx (.exe文件所在的目录)
        /// </summary>
        public string StartupPath
        {
            get
            {
                return System.Windows.Forms.Application.StartupPath;

            }
        }
        /// <summary>
        /// 获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。
        /// result: X:\xxx\xxx\xxx.exe (.exe文件所在的目录+.exe文件名)
        /// </summary>
        public string ExecutablePath
        {
            get
            {
                return System.Windows.Forms.Application.ExecutablePath;
            }
        }
        #endregion

        #region 私有函数
        #endregion

        #region 公开函数
        /// <summary>启动一个应用程序/进程</summary>
        /// <param name="appFilePath">文件路径</param>   
        public void StartApplication(string appFilePath)
        {
            Process downprocess = new Process();
            downprocess.StartInfo.FileName = appFilePath;
            downprocess.Start();
        }

        /// <summary> 目标应用程序是否已经启动。通常用于判断单实例应用。</summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public bool IsAppInstanceExist(string instanceName)
        {
            bool createdNew = false;
            MutexForSingletonExe = new System.Threading.Mutex(false, instanceName, out createdNew);
            return (!createdNew);
        }

        /// <summary>在浏览器中打开wsUrl链接</summary>
        /// <param name="url"></param>
        public void OpenUrl(string url)
        {
            Process.Start(url);
        }

        
        #endregion
    }
}
