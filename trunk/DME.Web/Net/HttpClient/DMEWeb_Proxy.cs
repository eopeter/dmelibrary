using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.Helper;
using System.Net;

namespace DME.Web.Net
{
    /// <summary>
    /// http 代理
    /// </summary>
    public class DMEWeb_Proxy
    {
        private string _address = string.Empty;
        private string _port = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">代理地址</param>
        /// <param name="port">代理端口</param>
        /// <param name="username">代理认证用户名</param>
        /// <param name="password">代理认证密码</param>
        public DMEWeb_Proxy(string address, string port, string username,string password)
        {
            this._address = address;
            this._port = port;
            this._username = username;
            this._password = password;
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public System.Net.WebProxy GetProxy()
        {
            System.Net.WebProxy _proxy = null;
            if (!DME_Validation.IsNull(_address) && !DME_Validation.IsNull(_port))
            {
                    _proxy = new System.Net.WebProxy();
                    _proxy.Address = new Uri("http://" + _address + ":" + _port); 
                    if (!DME_Validation.IsNull(_username))
                    {
                        _proxy.Credentials = new NetworkCredential(_username, _password);
                    }
            }

            return _proxy;
        }
    }
}
