using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.Network.Sockets;
using System.Net.Sockets;
using System.Net;

namespace DME.Base.Network.Tcp
{
    /// <summary>
    /// 增强TCP客户端
    /// </summary>
    public class DME_TcpClient : DME_SocketClient
    {
        #region 属性
        /// <summary>
        /// 已重载。
        /// </summary>
        public override ProtocolType ProtocolType
        {
            get { return ProtocolType.Tcp; }
        }

        private IPEndPoint _RemoteEndPoint;
        /// <summary>远程终结点</summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return _RemoteEndPoint; }
            set { _RemoteEndPoint = value; }
        }
        #endregion

        #region 接收
        /// <summary>
        /// 接收数据。已重载。接收到0字节表示连接断开！
        /// </summary>
        /// <param name="e"></param>
        protected override void OnReceive(DME_NetEventArgs e)
        {
            if (e.BytesTransferred > 0)
                base.OnReceive(e);
            else
            {
                //// 关闭前回收
                //Push(e);
                //Close();

                OnError(e, null);
            }
        }
        #endregion
    }
}
