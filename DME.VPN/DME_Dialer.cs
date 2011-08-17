using System;
using System.Collections.Generic;
using System.Text;
using DotRas;
using System.Net;
using DME.Base;
using System.ComponentModel;

namespace DME.Dialer
{
    public class DME_Dialer : DME_DisposeBase
    {
        private string _DialerName = string.Empty;
        private string _UserName = string.Empty;
        private string _Password = string.Empty;
        private string _ServerIP = string.Empty;
        private string _PhoneNumber = string.Empty;
        private RasPhoneBook _AllUsersPhoneBook = null;
        private RasDialer _Dialer = null;
        private RasHandle _RasHandle = null;
        public delegate void StateChangedEventHander(object sender, DME.Dialer.StateChangedEventArgs e);
        public event StateChangedEventHander StateChanged;

        public delegate void DialCompletedEventHander(object sender, DME.Dialer.DialCompletedEventArgs e);
        public event DialCompletedEventHander DialCompleted;

        public delegate void IsDisconnectEventHander(object sender, DME.Dialer.IsDisconnectEventArgs e);
        public event IsDisconnectEventHander IsDisconnect;

        private delegate void Check();
        /// <summary>
        /// 拨号名称
        /// </summary>
        public string DialerName
        {
            get { return _DialerName; }
            set { _DialerName = value; }
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP
        {
            get { return _ServerIP; }
            set 
            {
                if (!DME.Base.Helper.DME_Validation.IsIPv4(value))
                {
                    _ServerIP = string.Empty;
                }
                else
                {
                    _ServerIP = value;
                }
            }
        }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set
            {
                if (!DME.Base.Helper.DME_Validation.IsPhone(value))
                {
                    _PhoneNumber = string.Empty;
                }
                else
                {
                    _PhoneNumber = value;
                }
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public DME_Dialer()
        {
            _AllUsersPhoneBook = new RasPhoneBook();
            _Dialer = new RasDialer();
            this._Dialer.StateChanged += new System.EventHandler<DME.Dialer.StateChangedEventArgs>(this.Dialer_StateChanged);
            this._Dialer.DialCompleted += new System.EventHandler<DME.Dialer.DialCompletedEventArgs>(this.Dialer_DialCompleted);
            
        }

        public DME_Dialer(string DialerName, string UserName, string Password, string ServerIP, string PhoneNumber):this()
        {
            this.DialerName = DialerName;
            this.UserName = UserName;
            this.Password = Password;
            this.ServerIP = ServerIP;
            this.PhoneNumber = PhoneNumber;            
        }

        /// <summary>
        /// 创建VPN
        /// </summary>
        /// <returns></returns>
        public void CreateVPN()
        {

            this._AllUsersPhoneBook.Open();

            // 如果已经该名称的VPN已经存在，则更新这个VPN服务器地址
            if (_AllUsersPhoneBook.Entries.Contains(_DialerName))
            {
                _AllUsersPhoneBook.Entries[_DialerName].PhoneNumber = ServerIP;
                // 不管当前VPN是否连接，服务器地址的更新总能成功，如果正在连接，则需要VPN重启后才能起作用
                _AllUsersPhoneBook.Entries[_DialerName].Update();

            }
            // 创建一个新VPN
            else
            {
                RasEntry entry = RasEntry.CreateVpnEntry(_DialerName, ServerIP, RasVpnStrategy.Default, RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn));
                this._AllUsersPhoneBook.Entries.Add(entry);
            }

        }

        /// <summary>
        /// 创建Modem拔号
        /// </summary>
        /// <returns></returns>
        public void CreateModem()
        {

            this._AllUsersPhoneBook.Open();

            // 如果已经该名称的Modem已经存在，则更新这个Modem电话号码
            if (_AllUsersPhoneBook.Entries.Contains(_DialerName))
            {
                _AllUsersPhoneBook.Entries[_DialerName].PhoneNumber = _PhoneNumber;
                _AllUsersPhoneBook.Entries[_DialerName].Update();

            }
            // 创建一个新Modem
            else
            {
                RasEntry entry = RasEntry.CreateDialUpEntry(_DialerName, _PhoneNumber, RasDevice.GetDeviceByName("直接并行",RasDeviceType.Modem));
                this._AllUsersPhoneBook.Entries.Add(entry);
            }

        }

#if (WINXP || WIN2K8 || WIN7)
        /// <summary>
        /// 创建PPPOE
        /// </summary>
        /// <returns></returns>
        public void CreatePPPOE()
        {

            this._AllUsersPhoneBook.Open();

            // 如果已经该名称的VPN已经存在，则更新这个VPN服务器地址
            if (_AllUsersPhoneBook.Entries.Contains(_DialerName))
            {
               //如果存在什么都不做

            }
            // 创建一个新PPPOE
            else
            {
                RasEntry entry = RasEntry.CreateBroadbandEntry(_DialerName, RasDevice.GetDeviceByName("(PPPOE)", RasDeviceType.PPPoE));
                this._AllUsersPhoneBook.Entries.Add(entry);
            }

        }
#endif

        /// <summary>
        /// 连接
        /// </summary>
        public void Connection()
        {
            this._Dialer.EntryName = _DialerName;
            this._Dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            this._Dialer.Credentials = new NetworkCredential(_UserName, _Password);
            this._RasHandle = this._Dialer.DialAsync();
            Check my = check;
            IAsyncResult asyncResult = my.BeginInvoke(null, my);
        }

        /// <summary>
        /// 断开
        /// </summary>
        public void Disconnect()
        {
            if (this._Dialer.IsBusy)
            {
                // The connection attempt has not been completed, cancel the attempt.
                this._Dialer.DialAsyncCancel();
            }
            else
            {
                // The connection attempt has completed, attempt to find the connection in the active connections.
                RasConnection connection = RasConnection.GetActiveConnectionByHandle(this._RasHandle);
                if (connection != null)
                {
                    // The connection has been found, disconnect it.
                    connection.HangUp();
                }
            }
        }

        public void Delete()
        {
            this._AllUsersPhoneBook.Entries[_DialerName].Remove();
        }

        /// <summary>
        /// 获取所有连接
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetAllConnection()
        {
            IDictionary<string, string> dc = new Dictionary<string, string>();

            for (int i = 0; i < _AllUsersPhoneBook.Entries.Count; i++)
            {
                dc.Add(_AllUsersPhoneBook.Entries[i].Name, _AllUsersPhoneBook.Entries[i].Device.DeviceType);
            }

            return dc;
        }

        /// <summary>
        /// 获取当前正在连接中的连接
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetActiveConnection()
        {
            IDictionary<string,string> dc = new Dictionary<string,string>();
            foreach (RasConnection connection in RasConnection.GetActiveConnections())
            {
                dc.Add(connection.EntryName,connection.Device.DeviceType);
            }

            return dc;
        }
        /// <summary>
        /// 获取当前正在连接中的IP与ServerIP
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetIpAddresses()
        {
            IDictionary<string, string> dc = new Dictionary<string, string>();
            foreach (RasConnection connection in RasConnection.GetActiveConnections())
            {
                if (connection.EntryId == _AllUsersPhoneBook.Entries[_DialerName].Id)
                {
                    RasIPInfo ipAddresses = (RasIPInfo)connection.GetProjectionInfo(RasProjectionType.IP);
                    if (ipAddresses != null)
                    {
                        dc.Add("IP", ipAddresses.IPAddress.ToString());
                        dc.Add("ServerIP", ipAddresses.ServerIPAddress.ToString());
                    }
                }
            }
            return dc;
        }

        /// <summary>
        /// 子类重载实现资源释放逻辑
        /// </summary>
        /// <param name="disposing">从Dispose调用（释放所有资源）还是析构函数调用（释放非托管资源）</param>
        protected override void OnDispose(Boolean disposing)
        {
            if (disposing)
            {
                // 释放托管资源

                _AllUsersPhoneBook.Dispose();
                _Dialer.Dispose();
            }

            base.OnDispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dialer_StateChanged(object sender, DME.Dialer.StateChangedEventArgs e)
        {
            if (StateChanged != null)
            {
                this.StateChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dialer_DialCompleted(object sender, DME.Dialer.DialCompletedEventArgs e)
        {
            if (DialCompleted != null)
            {
                this.DialCompleted(this, e);
            }
            
        }

        
        private void check()
        {
            while (true)
            {
                if (!this._Dialer.IsBusy)
                {
                    RasConnection connection = RasConnection.GetActiveConnectionByHandle(this._RasHandle);
                    if (connection == null)
                    {
                        if (IsDisconnect != null)
                        {
                            this.IsDisconnect(this, new IsDisconnectEventArgs(true));                           
                        }
                        break;
                    }
                }
                System.Threading.Thread.Sleep(5000);
            }
        }
    }

    /// <summary>
    /// Provides data for the <see cref="RasDialer.StateChanged"/> event.
    /// </summary>
    public class StateChangedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.StateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="callbackId">The application defined value that was specified during dialing.</param>
        /// <param name="subEntryId">The one-based index for the phone book entry associated with this connection.</param>
        /// <param name="handle">The handle of the connection.</param>
        /// <param name="state">The state the remote access connection is about to enter.</param>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="errorMessage">The error message of the <paramref name="errorCode"/> that occurred.</param>
        /// <param name="extendedErrorCode">The extended error code (if any) that occurred.</param>
        internal StateChangedEventArgs(IntPtr callbackId, int subEntryId, RasHandle handle, RasConnectionState state, int errorCode, string errorMessage, int extendedErrorCode)
        {
            this.CallbackId = callbackId;
            this.SubEntryId = subEntryId;
            this.Handle = handle;
            this.State = state;
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.ExtendedErrorCode = extendedErrorCode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application defined callback id.
        /// </summary>
        public IntPtr CallbackId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the one-based index for the phone book entry associated with this connection.
        /// </summary>
        public int SubEntryId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the handle of the connection.
        /// </summary>
        public RasHandle Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state the remote access connection is about to enter.
        /// </summary>
        public RasConnectionState State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error code (if any) that occurred.
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error message for the <see cref="ErrorCode"/> that occurred.
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the extended error code (if any) that occurred.
        /// </summary>
        public int ExtendedErrorCode
        {
            get;
            private set;
        }

        #endregion
    }

    /// <summary>
    /// Provides data for the <see cref="RasDialer.DialCompleted"/> event.
    /// </summary>
    public class DialCompletedEventArgs : AsyncCompletedEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.DialCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="handle">The handle whose connection attempt completed.</param>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="cancelled"><b>true</b> if the asynchronous operation was cancelled, otherwise <b>false</b>.</param>
        /// <param name="timedOut"><b>true</b> if the operation timed out, otherwise <b>false</b>.</param>
        /// <param name="connected"><b>true</b> if the connection attempt successfully connected, otherwise <b>false</b>.</param>
        /// <param name="userState">The optional user-supplied state object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "cancelled", Justification = "The name is ok. Matching the argument name in the base constructor.")]
        public DialCompletedEventArgs(RasHandle handle, Exception error, bool cancelled, bool timedOut, bool connected, object userState)
            : base(error, cancelled, userState)
        {
            this.Handle = handle;
            this.TimedOut = timedOut;
            this.Connected = connected;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the handle whose connection attempt completed.
        /// </summary>
        public RasHandle Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the asynchronous dial attempt timed out.
        /// </summary>
        public bool TimedOut
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the connection attempt successfully connected.
        /// </summary>
        public bool Connected
        {
            get;
            private set;
        }

        #endregion
    }

    public class IsDisconnectEventArgs : EventArgs
    {
        private bool _Disconnect = false;
        public bool Disconnect { get { return _Disconnect; } }
        public IsDisconnectEventArgs(bool Disconnect)
        {
            _Disconnect = Disconnect;
        }
    }
}
