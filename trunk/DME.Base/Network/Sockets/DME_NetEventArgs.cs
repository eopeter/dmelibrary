﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace DME.Base.Network.Sockets
{
    /// <summary>
    /// 网络事件参数
    /// </summary>
    public class DME_NetEventArgs : SocketAsyncEventArgs
    {
        #region 属性
        private Int32 _Times;
        /// <summary>使用次数</summary>
        public Int32 Times
        {
            get { return _Times; }
            set { _Times = value; }
        }

        private Boolean _Used;
        /// <summary>使用中</summary>
        public Boolean Used
        {
            get { return _Used; }
            set { _Used = value; }
        }
        #endregion

        #region 事件
        private Boolean hasEvent;
        private Delegate _Completed;
        /// <summary>
        /// 完成事件。该事件只能设置一个。
        /// </summary>
        public new event EventHandler<DME_NetEventArgs> Completed
        {
            add
            {
                if (_Completed != null) throw new Exception("重复设置了事件！");
                _Completed = value;
                // 考虑到该对象将会作为池对象使用，不需要频繁的add和remove事件，所以一次性挂接即可
                if (!hasEvent)
                {
                    hasEvent = true;
                    base.Completed += OnCompleted;
                }
            }
            remove
            {
                _Completed = null;
            }
        }

        //protected override void OnCompleted(SocketAsyncEventArgs e)
        //{
        //    if (_Completed != null) (_Completed as EventHandler<NetEventArgs>)(sender, e as NetEventArgs);
        //}

        private void OnCompleted(Object sender, SocketAsyncEventArgs e)
        {
            if (_Completed != null) (_Completed as EventHandler<DME_NetEventArgs>)(sender, e as DME_NetEventArgs);
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 从接收缓冲区拿字符串，UTF-8编码
        /// </summary>
        /// <returns></returns>
        public String GetString()
        {
            if (Buffer == null || Buffer.Length < 1 || BytesTransferred < 1) return null;

            return Encoding.UTF8.GetString(Buffer, Offset, BytesTransferred);
        }

        /// <summary>
        /// Socket数据流。每个网络事件参数带有一个，防止多次声明流对象
        /// </summary>
        private DME_SocketStream socketStream;

        /// <summary>
        /// 从接收缓冲区获取一个流，该流可用于读取已接收数据，写入数据时向远端发送数据
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            if (Buffer == null || Buffer.Length < 1 || BytesTransferred < 1) return null;

            if (socketStream == null)
            {
                Stream ms = new MemoryStream(Buffer, Offset, BytesTransferred);
                socketStream = new DME_SocketStream(AcceptSocket, ms, RemoteEndPoint);
            }
            else
            {
                socketStream.Reset(AcceptSocket, Offset, BytesTransferred);
            }

            return socketStream;
        }

        /// <summary>
        /// 讲接收缓冲区中的数据写入流
        /// </summary>
        /// <param name="stream"></param>
        public void WriteTo(Stream stream)
        {
            if (Buffer == null || Buffer.Length < 1 || BytesTransferred < 1) return;

            stream.Write(Buffer, Offset, BytesTransferred);
        }

        /// <summary>
        /// 已重载。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}]{1}", LastOperation, GetString());
        }
        #endregion
    }
}
