using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.Network.Sockets;

namespace DME.Base.Network.Tcp
{
    /// <summary>
    /// Tcp实现的网络服务器基类
    /// </summary>
    public class DME_TcpNetServer : DME_NetServer
    {
        /// <summary>
        /// 已重载。
        /// </summary>
        protected override void EnsureCreateServer()
        {
            DME_TcpServer svr = new DME_TcpServer(Address, Port);
            svr.Accepted += new EventHandler<DME_NetEventArgs>(OnAccepted);
        }

        /// <summary>
        /// 接受连接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAccepted(Object sender, DME_NetEventArgs e)
        {
            DME_TcpClient session = e.UserToken as DME_TcpClient;
            if (session == null) return;

            session.Received += OnReceived;
            session.Error += new EventHandler<DME_NetEventArgs>(OnError);
        }

        /// <summary>
        /// 收到数据时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnReceived(Object sender, DME_NetEventArgs e) { }
    }
}
