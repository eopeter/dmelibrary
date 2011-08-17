using System;
using DME.Base.Network.Tcp;
using DME.Base.Network.Sockets;


namespace DME.Base.Network.Application
{
    /// <summary>
    /// Tcp实现的Echo服务
    /// </summary>
    public class DME_TcpEchoServer : DME_TcpNetServer
    {
        /// <summary>
        /// 已重载。
        /// </summary>
        protected override void EnsureCreateServer()
        {
            Name = "Echo服务（TCP）";

            base.EnsureCreateServer();

            DME_TcpServer svr = Server as DME_TcpServer;
            // 允许同时处理多个数据包
            svr.NoDelay = false;
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
            DME_TcpClient tc = sender as DME_TcpClient;

            try
            {
                if (e.BytesTransferred > 1024)
                {
                    WriteLog("{0}的数据包大于1k，抛弃！", tc.RemoteEndPoint);
                }
                else
                {
                    WriteLog("{0} [{1}] {2}", tc.RemoteEndPoint, e.BytesTransferred, e.GetString());

                    if (tc != null && tc.Client.Connected) tc.Send(e.Buffer, e.Offset, e.BytesTransferred);
                }
            }
            finally
            {
                tc.Close();
            }
        }
    }
}