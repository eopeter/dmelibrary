using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using DME.Base.Network.Common;

namespace DME.Base.Network.Sockets
{
    /// <summary>
    /// 针对APM异步模型进行封装的网络服务器
    /// </summary>
    public class DME_SocketServer : DME_SocketBase
    {
        #region 属性
        #endregion

        #region 构造
        /// <summary>基础Socket对象</summary>
        public Socket Server
        {
            get
            {
                if (Socket == null) EnsureCreate();
                return Socket;
            }
            set { Socket = value; }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public DME_SocketServer(IPAddress address, Int32 port)
        {
            Address = address;
            Port = port;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public DME_SocketServer(String hostname, Int32 port)
        {
            Address = DME_NetHelper.ParseAddress(hostname);
            Port = port;
        }
        #endregion

        #region 开始停止
        private Boolean _Active;
        /// <summary>是否活动</summary>
        public Boolean Active
        {
            get { return _Active; }
            private set { _Active = value; }
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void Start()
        {
            if (Active) throw new InvalidOperationException("服务已经开始！");

            OnStart();

            Active = true;
        }

        /// <summary>
        /// 开始时调用的方法
        /// </summary>
        protected virtual void OnStart()
        {
            Bind();
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            if (!Active) throw new InvalidOperationException("服务没有开始！");

            OnStop();

            Active = false;
        }

        /// <summary>
        /// 停止时调用的方法
        /// </summary>
        protected virtual void OnStop()
        {
            Close();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 已重载。服务器不会因为普通错误而关闭Socket停止服务
        /// </summary>
        /// <param name="e"></param>
        /// <param name="ex"></param>
        protected override void OnError(DME_NetEventArgs e, Exception ex)
        {
            //base.OnError(e, ex);
            try
            {
                ProcessError(e, ex);
            }
            finally
            {
                if (e.SocketError == SocketError.OperationAborted) Close();
            }
        }
        #endregion
    }
}
