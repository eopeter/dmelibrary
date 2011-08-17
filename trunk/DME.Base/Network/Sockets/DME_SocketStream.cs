﻿using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.IO;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace DME.Base.Network.Sockets
{
    /// <summary>
    /// Socket流
    /// </summary>
    public class DME_SocketStream : DME_ReadWriteStream
    {
        #region 属性
        private Socket _Socket;
        /// <summary>套接字</summary>
        public Socket Socket
        {
            get { return _Socket; }
            internal set { _Socket = value; }
        }

        private IPEndPoint _RemoteEndPoint;
        /// <summary>远程地址</summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return _RemoteEndPoint; }
            set { _RemoteEndPoint = value; }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="socket"></param>
        public DME_SocketStream(Socket socket) : this(socket, Stream.Null) { }

        /// <summary>
        /// 使用Socket和输入流初始化一个Socket流，该流将从输入流中读取数据，并把输出的数据写入到Socket中
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="inputStream"></param>
        public DME_SocketStream(Socket socket, Stream inputStream)
            : base(inputStream, Stream.Null)
        {
            Socket = socket;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remote"></param>
        public DME_SocketStream(Socket socket, EndPoint remote) : this(socket, Stream.Null, remote) { }

        /// <summary>
        /// 使用Socket和输入流初始化一个Socket流，该流将从输入流中读取数据，并把输出的数据写入到Socket中
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="inputStream"></param>
        /// <param name="remote"></param>
        public DME_SocketStream(Socket socket, Stream inputStream, EndPoint remote)
            : base(inputStream, Stream.Null)
        {
            Socket = socket;
            RemoteEndPoint = remote as IPEndPoint;
        }
        #endregion

        #region 重载
        /// <summary>
        /// 读取数据，如果初始化时指定了输入流，则从输入流读取数据，否则从Socket中读取数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (InputStream == Stream.Null)
                return Socket.Receive(buffer, offset, count, SocketFlags.None);
            else
                return base.Read(buffer, offset, count);
        }

        /// <summary>
        /// 写入数据，经Socket向网络发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (RemoteEndPoint.Address != IPAddress.Any && RemoteEndPoint.Port != 0)
                Socket.SendTo(buffer, offset, count, SocketFlags.None, RemoteEndPoint);
            else
                Socket.Send(buffer, offset, count, SocketFlags.None);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 重新设置流
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Reset(Socket socket, Int32 offset, Int32 length)
        {
            Socket = socket;
            InputStream.Position = offset;
            InputStream.SetLength(length);
        }
        #endregion
    }
}
