using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DME.Windows.Helper
{
    public partial class DMEWin_ServiceInstaller
    {
        #region Private Variables

        private string _servicePath;
        private string _serviceName;
        private string _serviceDisplayName;

        #endregion Private Variables

        #region DLLImport

        [DllImport("advapi32.dll")]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

        [DllImport("Advapi32.dll")]
        private static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
        int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
        string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport("advapi32.dll")]
        private static extern void CloseServiceHandle(IntPtr SCHANDLE);

        [DllImport("advapi32.dll")]
        private static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

        [DllImport("advapi32.dll")]
        private static extern int DeleteService(IntPtr SVHANDLE);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        #endregion DLLImport

        /*
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// <param name="args">命令参数</param>
        [STAThread]
        static void Main(string[] args)
        {
            string svcPath;
            string svcName;
            string svcDispName;
            //服务程序的路径
            svcPath = @"C:\Service.exe";
            svcDispName = "MyService";
            svcName = "MyService";
            ServiceInstaller si = new ServiceInstaller();
            si.InstallService(svcPath, svcName, svcDispName);
            Console.Read();
        }
        */

        /// <summary>
        /// 安装服务程序并运行
        /// </summary>
        /// <param name="svcPath">程序路径</param>
        /// <param name="svcName">服务名称</param>
        /// <param name="svcDispName">显示服务名称</param>
        /// <returns>服务是否安装成功</returns>
        public static bool InstallService(string svcPath, string svcName, string svcDispName)
        {
            #region Constants declaration.
            int SC_MANAGER_CREATE_SERVICE = 0x0002;
            int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            //int SERVICE_DEMAND_START = 0x00000003;
            int SERVICE_ERROR_NORMAL = 0x00000001;
            int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            int SERVICE_QUERY_CONFIG = 0x0001;
            int SERVICE_CHANGE_CONFIG = 0x0002;
            int SERVICE_QUERY_STATUS = 0x0004;
            int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            int SERVICE_START = 0x0010;
            int SERVICE_STOP = 0x0020;
            int SERVICE_PAUSE_CONTINUE = 0x0040;
            int SERVICE_INTERROGATE = 0x0080;
            int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
             SERVICE_QUERY_CONFIG |
             SERVICE_CHANGE_CONFIG |
             SERVICE_QUERY_STATUS |
             SERVICE_ENUMERATE_DEPENDENTS |
             SERVICE_START |
             SERVICE_STOP |
             SERVICE_PAUSE_CONTINUE |
             SERVICE_INTERROGATE |
             SERVICE_USER_DEFINED_CONTROL);
            int SERVICE_AUTO_START = 0x00000002;
            #endregion Constants declaration.
            try
            {
                IntPtr sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt32() != 0)
                {
                    IntPtr sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                    if (sv_handle.ToInt32() == 0)
                    {
                        CloseServiceHandle(sc_handle);
                        return false;
                    }
                    else
                    {
                        //试尝启动服务
                        int i = StartService(sv_handle, 0, null);
                        if (i == 0)
                        {
                            return false;
                        }
                        CloseServiceHandle(sc_handle);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 卸载服务程序
        /// </summary>
        /// <param name="svcName">服务名称</param>
        /// <returns>服务是否卸载成功</returns>
        public static bool UnInstallService(string svcName)
        {
            int GENERIC_WRITE = 0x40000000;
            IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() != 0)
            {
                int DELETE = 0x10000;
                IntPtr svc_hndl = OpenService(sc_hndl, svcName, DELETE);
                if (svc_hndl.ToInt32() != 0)
                {
                    int i = DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        CloseServiceHandle(sc_hndl);
                        return true;
                    }
                    else
                    {
                        CloseServiceHandle(sc_hndl);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 写入Windows事件日志
        /// </summary>
        /// <param name="EventName">事件名称</param>
        /// <param name="EventContent">事件内容</param>
        public static void ServiceEventLog(string EventName, string EventContent)
        {
            StringBuilder sb = new StringBuilder();
            // sb.Append(string.Format(".Net Version: {0}", Environment.Version.ToString()));
            // sb.Append(string.Format("Current Identity: {0}", WindowsIdentity.GetCurrent().Name));
            sb.Append(string.Format("【{0}】\r\n{1}", EventName, EventContent));

            Console.WriteLine(sb.ToString());

            // Create the source, if it does not already exist.
            if (!EventLog.SourceExists("DME"))
            {
                EventLog.CreateEventSource("DME", "DME.DME.Windows.Helper.DMEWin_ServiceInstaller.ServiceEventLog");
                // Console.WriteLine("CreatingEventSource");
            }

            // Create an EventLog instance and assign its source.
            EventLog ServiceLog = new EventLog();
            ServiceLog.Source = "DME";

            // Write an informational entry to the event log.    
            // ServiceLog.WriteEntry("Writing to event log.");
            ServiceLog.WriteEntry(sb.ToString());

            // Close EventLog.
            ServiceLog.Close();
        }
    }
}
