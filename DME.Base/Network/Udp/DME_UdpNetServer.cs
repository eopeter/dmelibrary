using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.Network.Sockets;

namespace DME.Base.Network.Udp
{
    /// <summary>
    /// Udp实现的网络服务器基类
    /// </summary>
    public abstract class DME_UdpNetServer : DME_NetServer
    {
        /// <summary>
        /// 已重载。
        /// </summary>
        protected override void EnsureCreateServer()
        {
            DME_UdpServer svr = new DME_UdpServer(Address, Port);
            svr.Received += new EventHandler<DME_NetEventArgs>(OnReceived);

            Server = svr;
        }

        /// <summary>
        /// 数据到达时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnReceived(object sender, DME_NetEventArgs e) { }
    }
}
