using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using DotRas;
using System.Text.RegularExpressions;

namespace DME.Dialer
{
    public class DME_VPNHelper
    {
         private string iPToPing;
        private string vPNName;
        private string userName;
        private string passWord;

        // 系统路径 C:\windows\system32\
        private static string WinDir = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\";

        // rasdial.exe
        private static string RasDialFileName = "rasdial.exe";

        // VPN路径 C:\windows\system32\rasdial.exe
        private static string VPNPROCESS = WinDir + RasDialFileName;

        /// <summary>
        /// VPN地址
        /// </summary>
        public string IPToPing 
        {
            get { return iPToPing; }
            set { iPToPing = value; }
        }


        /// <summary>
        /// VPN名称
        /// </summary>
 
        public string VPNName 
        {
            get { return vPNName; }
            set { vPNName = value; }
        }

        /// <summary>
        /// VPN用户名
        /// </summary>
 
        public string UserName 
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// VPN密码
        /// </summary>
 
        public string PassWord 
        {
            get { return passWord; }
            set { passWord = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DME_VPNHelper()
        {

        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="_vpnIP"></param>
        /// <param name="_vpnName"></param>
        /// <param name="_userName"></param>
        /// <param name="_passWord"></param>
        public DME_VPNHelper(string _vpnIP, string _vpnName, string _userName, string _passWord)
        {
            this.IPToPing = _vpnIP;
            this.VPNName = _vpnName;
            this.UserName = _userName;
            this.PassWord = _passWord;
        }

        /// <summary>
        /// 尝试连接VPN(默认VPN)
        /// </summary>
        /// <returns></returns>
        public void TryConnectVPN()
        {
            this.TryConnectVPN(this.VPNName, this.UserName, this.PassWord);
        }

        /// <summary>
        /// 尝试断开连接(默认VPN)
        /// </summary>
        /// <returns></returns>
        public void TryDisConnectVPN()
        {
            this.TryDisConnectVPN(this.VPNName);
        }

        /// <summary>
        /// 创建或更新一个默认的VPN连接
        /// </summary>
        public void CreateOrUpdateVPN()
        {
            this.CreateOrUpdateVPN(this.VPNName, this.IPToPing);
        }

        /// <summary>
        /// 尝试删除连接(默认VPN)
        /// </summary>
        /// <returns></returns>
        public void TryDeleteVPN()
        {
            this.TryDeleteVPN(this.VPNName);
        }
        /// <summary>
        /// 尝试连接VPN(指定VPN名称，用户名，密码)
        /// </summary>
        /// <returns></returns>
        public void TryConnectVPN(string connVpnName, string connUserName, string connPassWord)
        {
            try
            {
                string args = string.Format("{0} {1} {2}", connVpnName, connUserName, connUserName);

                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);

                myProcess.CreateNoWindow = true;

                myProcess.UseShellExecute = false;

                Process.Start(myProcess);

            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
            }
        }

        /// <summary>
        /// 尝试断开VPN(指定VPN名称)
        /// </summary>
        /// <returns></returns>
        public void TryDisConnectVPN(string disConnVpnName)
        {
            try
            {
                string args = string.Format(@"{0} /d", disConnVpnName);

                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);

                myProcess.CreateNoWindow = true;

                myProcess.UseShellExecute = false;

                Process.Start(myProcess);

            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
            }
        }

        /// <summary>
        /// 创建或更新一个VPN连接(指定VPN名称，及IP)
        /// </summary>
        public void CreateOrUpdateVPN(string updateVPNname, string updateVPNip)
        {
            RasDialer dialer = new RasDialer();
            
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();

            allUsersPhoneBook.Open();

            // 如果已经该名称的VPN已经存在，则更新这个VPN服务器地址
            if (allUsersPhoneBook.Entries.Contains(updateVPNname))
            {
                allUsersPhoneBook.Entries[updateVPNname].PhoneNumber = updateVPNip;
                // 不管当前VPN是否连接，服务器地址的更新总能成功，如果正在连接，则需要VPN重启后才能起作用
                allUsersPhoneBook.Entries[updateVPNname].Update();
            }
            // 创建一个新VPN
            else
            {
                //RasEntry entry = RasEntry.CreateVpnEntry(updateVPNname, updateVPNip, RasVpnStrategy.PptpFirst, RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn));
                //RasEntry entry = RasEntry.CreateDialUpEntry(updateVPNname,,"88888888",RasDevice.GetDeviceByName("(PPPOE)",RasDeviceType.PPPoE));
                RasEntry entry = RasEntry.CreateBroadbandEntry("aaaaa", RasDevice.GetDeviceByName("(PPPOE)", RasDeviceType.PPPoE));
                allUsersPhoneBook.Entries.Add(entry);

                dialer.EntryName = updateVPNname;

                dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            }
        }

        /// <summary>
        /// 删除指定名称的VPN
        /// 如果VPN正在运行，一样会在电话本里删除，但是不会断开连接，所以，最好是先断开连接，再进行删除操作
        /// </summary>
        /// <param name="delVpnName"></param>
        public void TryDeleteVPN(string delVpnName)
        {
            RasDialer dialer = new RasDialer();
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
            allUsersPhoneBook.Open();
            if (allUsersPhoneBook.Entries.Contains(delVpnName))
            {
                allUsersPhoneBook.Entries.Remove(delVpnName);
            }
        }

        /// <summary>
        /// 获取当前正在连接中的VPN名称
        /// </summary>
        public List<string> GetCurrentConnectingVPNNames()
        {
            List<string> ConnectingVPNList = new List<string>();

            //Process proIP = new Process();

            //proIP.StartInfo.FileName = "cmd.exe ";
            //proIP.StartInfo.UseShellExecute = false;
            //proIP.StartInfo.RedirectStandardInput = true;
            //proIP.StartInfo.RedirectStandardOutput = true;
            //proIP.StartInfo.RedirectStandardError = true;
            //proIP.StartInfo.CreateNoWindow = true;//不显示cmd窗口 
            //proIP.Start();

            //proIP.StandardInput.WriteLine(RasDialFileName);
            //proIP.StandardInput.WriteLine("exit");

            //// 命令行运行结果
            //string strResult = proIP.StandardOutput.ReadToEnd();
            //proIP.Close();

            //Regex regger = new Regex("(?<=已连接\r\n)(.*\n)*(?=命令已完成)", RegexOptions.Multiline);

            //// 如果匹配，则说有正在连接的VPN
            //if (regger.IsMatch(strResult))
            //{
            //    string[] list = regger.Match(strResult).Value.ToString().Split('\n');
            //    for (int index = 0; index < list.Length; index++)
            //    {
            //        if (list[index] != string.Empty)
            //            ConnectingVPNList.Add(list[index].Replace("\r", ""));
            //    }
            //}
            //// 没有正在连接的VPN，则直接返回一个空List<string>

            foreach (RasConnection connection in RasConnection.GetActiveConnections())
            {

                //this.comboBox1.Items.Add(new ComboBoxItem(connection.EntryName, connection.EntryId));
                ConnectingVPNList.Add(connection.EntryName);
            }


            return ConnectingVPNList;
        }
    }
}
