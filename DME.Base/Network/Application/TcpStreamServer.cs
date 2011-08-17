using System;
using DME.Base.Network.Tcp;
using DME.Base.Network.Sockets;
using DME.Base.IO;


namespace DME.Base.Network.Application
{
    /// <summary>
    /// 基于Tcp的数据流服务器
    /// </summary>
    public class DME_TcpStreamServer : DME_TcpNetServer
    {
        private String _StreamHandlerFactoryName;
        /// <summary>数据流工厂名称</summary>
        public String StreamHandlerFactoryName
        {
            get { return _StreamHandlerFactoryName; }
            set { _StreamHandlerFactoryName = value; }
        }

        /// <summary>
        /// 已重载。
        /// </summary>
        protected override void EnsureCreateServer()
        {
            if (String.IsNullOrEmpty(StreamHandlerFactoryName)) throw new Exception("未指定数据流工厂名称！");

            Name = "数据流服务器（TCP）";

            base.EnsureCreateServer();
        }

        /// <summary>
        /// 已重载。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnReceived(object sender, DME_NetEventArgs e)
        {
            if (e.BytesTransferred > 0) DME_StreamHandlerFactory.Process(StreamHandlerFactoryName, e.GetStream());
        }
    }
}