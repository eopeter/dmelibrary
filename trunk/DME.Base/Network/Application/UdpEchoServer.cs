using System.Net;
using DME.Base.Network.Udp;
using DME.Base.Network.Sockets;


namespace DME.Base.Network.Application
{
    /// <summary>
    /// Udp实现的Echo服务
    /// </summary>
    public class DME_UdpEchoServer : DME_UdpNetServer
    {
        /// <summary>
        /// 已重载。
        /// </summary>
        protected override void EnsureCreateServer()
        {
            Name = "Echo服务（UDP）";

            base.EnsureCreateServer();

            DME_UdpServer svr = Server as DME_UdpServer;
            // 允许同时处理多个数据包
            svr.NoDelay = true;
            // 使用线程池来处理事件
            svr.UseThreadPool = true;
        }

        /// <summary>
        /// 已重载。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnReceived(object sender, DME_NetEventArgs e)
        {
            if (e.BytesTransferred > 1024)
            {
                WriteLog("{0}的数据包大于1k，抛弃！", e.RemoteEndPoint);
                return;
            }

            //WriteLog("{0} {1}", e.RemoteEndPoint, Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred));
            WriteLog("{0} [{1}] {2}", e.RemoteEndPoint, e.BytesTransferred, e.GetString());

            if ((e.RemoteEndPoint as IPEndPoint).Address != IPAddress.Any)
            {
                DME_UdpServer us = sender as DME_UdpServer;
                us.Send(e.Buffer, e.Offset, e.BytesTransferred, e.RemoteEndPoint);
                // 这里发送完成后不需要关闭Socket，因为这是UdpServer的Socket
            }
        }
    }
}