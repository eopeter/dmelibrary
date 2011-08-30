using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace DME.Windows.Common
{
    /// <summary>
    /// 计算机硬件获取类。
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
    public class DMEWin_ComputerHelper
    {
        public IList<string> CpuID;
        public IList<string> MotherboardID;
        public IDictionary<string,string> MacAndIP;
        public IList<string> DiskID;
        public IList<string> LoginUserName;
        public string ComputerName;
        public IList<string> SystemType;
        public IList<string> TotalPhysicalMemory; //单位：M
        private static DMEWin_ComputerHelper _instance;
        public static DMEWin_ComputerHelper Instance()
        {
            if (_instance == null)
                _instance = new DMEWin_ComputerHelper();
            return _instance;
        }
        public DMEWin_ComputerHelper()
        {
            CpuID = GetCpuID();
            MotherboardID = GetMotherboardID();
            MacAndIP = GetMacAndIP();
            DiskID = GetDiskID();
            LoginUserName = GetUserName();
            SystemType = GetSystemType();
            TotalPhysicalMemory = GetTotalPhysicalMemory();
            ComputerName = GetComputerName();
        }
        IList<string> GetCpuID()
        {
            IList<string> cpuInfo = new List<string>();//cpu序列号
            try
            {
                //获取CPU序列号代码
                
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo.Add(mo.Properties["ProcessorId"].Value.ToString());
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return cpuInfo;
            }
            finally
            {
            }

        }
        IList<string> GetMotherboardID()
        {
            IList<string> motherboardid = new List<string>();
            try
            {
                //获取主板ID
                ManagementObjectSearcher mc = new ManagementObjectSearcher("select * from Win32_baseboard");
                foreach (ManagementObject mo in mc.Get())
                {
                    motherboardid.Add(mo["SerialNumber"].ToString());
                    break;
                }
                mc = null;
                return motherboardid;
            }
            catch
            {
                return motherboardid;
            }
            finally
            {
            }
        }
        IDictionary<string, string> GetMacAndIP()
        {
            IDictionary<string, string> macip = new Dictionary<string,string>();
            try
            {
                //获取网卡硬件地址
                
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        macip.Add(((System.Array)(mo.Properties["IpAddress"].Value)).GetValue(0).ToString(), mo["MacAddress"].ToString());
                        break;
                    }
                }
                moc = null;
                mc = null;
                return macip;
            }
            catch
            {
                return macip;
            }
            finally
            {
            }

        }
        IList<string> GetDiskID()
        {
            IList<string> HDid = new List<string>();
            try
            {
                //获取硬盘ID
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid.Add(mo.Properties["Model"].Value.ToString());
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return HDid;
            }
            finally
            {
            }

        }
        /// <summary>
        /// 操作系统的登录用户名
        /// </summary>
        /// <returns></returns>
        IList<string> GetUserName()
        {
            IList<string> st = new List<string>();
            try
            {
                
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st.Add(mo["UserName"].ToString());

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return st;
            }
            finally
            {
            }

        }
        /// <summary>
        /// PC类型
        /// </summary>
        /// <returns></returns>
        IList<string> GetSystemType()
        {
            IList<string> st = new List<string>();
            try
            {
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st.Add(mo["SystemType"].ToString());

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return st;
            }
            finally
            {
            }

        }

        /// <summary>
        /// 物理内存
        /// </summary>
        /// <returns></returns>
        IList<string> GetTotalPhysicalMemory()
        {
            IList<string> st = new List<string>();
            try
            {
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st.Add(mo["TotalPhysicalMemory"].ToString());

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return st;
            }
            finally
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }
    }
}
