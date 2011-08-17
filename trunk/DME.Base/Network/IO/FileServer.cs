using System;
using System.IO;
using DME.Base.Network.Sockets;
using DME.Base.Network.Tcp;

namespace DME.Base.Network.IO
{
    /// <summary>
    /// 文件服务端
    /// </summary>
    public class DME_FileServer : DME_NetServer
    {
        #region 属性
        private String _SavedPath;
        /// <summary>保存路径</summary>
        public String SavedPath
        {
            get { return _SavedPath ?? (_SavedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")); }
            set { _SavedPath = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 已重载。
        /// </summary>
        protected override void EnsureCreateServer()
        {
            Name = "文件服务";

            DME_TcpServer svr = new DME_TcpServer(Address, Port);
            svr.Accepted += new EventHandler<DME_NetEventArgs>(server_Accepted);
            // 允许同时处理多个数据包
            svr.NoDelay = false;
            // 使用线程池来处理事件
            svr.UseThreadPool = true;
        }
        #endregion

        #region 事件
        void server_Accepted(object sender, DME_NetEventArgs e)
        {
            DME_TcpClient session = e.UserToken as DME_TcpClient;
            if (session == null) return;

            //session.NoDelay = false;
            SetEvent(session);
        }

        void SetEvent(DME_TcpClient session)
        {
            session.Received += delegate(Object sender, DME_NetEventArgs e)
            {
                DME_TcpClient tc = sender as DME_TcpClient;

                Stream stream = null;
                if (!tc.Data.Contains("Stream"))
                {
                    stream = new MemoryStream();
                    tc.Data["Stream"] = stream;
                }
                else
                {
                    stream = tc.Data["Stream"] as Stream;
                }
                
                // 把数据写入流
                e.WriteTo(stream);

                // 数据太少时等下一次，不过基本上不可能。5是FileFormat可能的最小长度
                if (stream.Length < 5) return;

                DME_FileFormat format = DME_FileFormat.Load(stream);
            };
            session.Error += new EventHandler<DME_NetEventArgs>(OnError);
        }
        #endregion
    }
}
