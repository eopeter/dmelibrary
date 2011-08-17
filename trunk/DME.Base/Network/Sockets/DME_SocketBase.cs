﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using DME.Base.Network.Common;
using DME.Base.Threading;
using System.Threading;

namespace DME.Base.Network.Sockets
{
    /// <summary>
    /// Socket基类
    /// </summary>
    public abstract class DME_SocketBase : DME_Netbase
    {
        #region 属性
        private Socket _Socket;
        /// <summary>套接字</summary>
        internal protected Socket Socket
        {
            get { return _Socket; }
            set { _Socket = value; }
        }

        private ProtocolType _ProtocolType = ProtocolType.Tcp;
        /// <summary>协议类型</summary>
        public virtual ProtocolType ProtocolType
        {
            get { return _ProtocolType; }
        }

        private Int32 _BufferSize = 10240;
        /// <summary>缓冲区大小</summary>
        public Int32 BufferSize
        {
            get { return _BufferSize; }
            set { _BufferSize = value; }
        }

        private IPAddress _Address = IPAddress.Any;
        /// <summary>监听本地地址</summary>
        public IPAddress Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private Int32 _Port;
        /// <summary>监听端口</summary>
        public Int32 Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private Boolean _NoDelay = true;
        /// <summary>禁用接收延迟，收到数据后马上建立异步读取再处理本次数据</summary>
        public Boolean NoDelay
        {
            get { return _NoDelay; }
            set { _NoDelay = value; }
        }

        private Boolean _UseThreadPool;
        /// <summary>是否使用线程池处理事件。建议仅在事件处理非常耗时时使用线程池来处理。</summary>
        public Boolean UseThreadPool
        {
            get { return _UseThreadPool; }
            set { _UseThreadPool = value; }
        }
        #endregion

        #region 扩展属性
        /// <summary>允许将套接字绑定到已在使用中的地址。</summary>
        public Boolean ReuseAddress
        {
            get
            {
                if (Socket == null) return false;

                return (Boolean)Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress);
            }
            set
            {
                if (Socket != null) Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, value);
            }
        }

        private IDictionary _Data;
        /// <summary>数据字典</summary>
        public IDictionary Data
        {
            get { return _Data ?? (_Data = new Dictionary<Object, Object>()); }
            //set { _Data = value; }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 确保创建基础Socket对象
        /// </summary>
        protected virtual void EnsureCreate()
        {
            if (Socket != null) return;

            switch (ProtocolType)
            {
                case ProtocolType.Tcp:
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType);
                    break;
                case ProtocolType.Udp:
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType);
                    break;
                default:
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Unknown, ProtocolType);
                    break;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 绑定本地终结点
        /// </summary>
        public void Bind()
        {
            if (Socket != null && !Socket.IsBound) Socket.Bind(new IPEndPoint(Address, Port));
        }
        #endregion

        #region 套接字事件参数池
        private static DME_ObjectPool<DME_NetEventArgs> _Pool;
        /// <summary>套接字事件参数池</summary>
        public static DME_ObjectPool<DME_NetEventArgs> Pool
        {
            get
            {
                if (_Pool == null) _Pool = new DME_ObjectPool<DME_NetEventArgs>();
                return _Pool;
            }
        }

        /// <summary>
        /// 从池里拿一个对象
        /// </summary>
        /// <returns></returns>
        public DME_NetEventArgs Pop()
        {
            DME_NetEventArgs e = Pool.Pop();
            if (e.Used) throw new Exception("才刚出炉，怎么可能使用中呢？");
            e.AcceptSocket = Socket;
            e.UserToken = this;
            e.Completed += OnCompleted;
            if (e.Buffer == null)
            {
                Byte[] buffer = new Byte[BufferSize];
                e.SetBuffer(buffer, 0, buffer.Length);
            }

            e.Times++;
            e.Used = true;

            return e;
        }

        /// <summary>
        /// 把对象归还到池里
        /// </summary>
        /// <remarks>
        /// 网络事件参数使用原则：
        /// 1，得到者负责回收（通过方法参数得到）
        /// 2，正常执行时自己负责回收，异常时顶级负责回收
        /// 3，把回收责任交给别的方法
        /// </remarks>
        /// <param name="e"></param>
        public void Push(DME_NetEventArgs e)
        {
            if (e == null) return;
            if (!e.Used) throw new Exception("准备回炉，怎么可能已经不再使用呢？");

            e.UserToken = null;
            e.AcceptSocket = null;
            e.Completed -= OnCompleted;

            e.Used = false;

            Pool.Push(e);
        }
        #endregion

        #region 释放资源
        /// <summary>
        /// 关闭网络操作
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 子类重载实现资源释放逻辑
        /// </summary>
        /// <param name="disposing">从Dispose调用（释放所有资源）还是析构函数调用（释放非托管资源）</param>
        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);

            if (Socket != null)
            {
                //#if DEBUG
                //                WriteLog("{0}关闭套接字！{1}", GetType().Name, disposing ? "Dispose" : "Finalize");
                //#endif

                if (Socket != null)
                {
                    // 先用Shutdown禁用Socket（发送未完成发送的数据），再用Close关闭，这是一种比较优雅的关闭Socket的方法
                    if (DME_NetHelper.Debug)
                    {
                        try
                        {
                            Socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException ex2)
                        {
                            if (ex2.ErrorCode != 10057) WriteLog(ex2.ToString());
                        }
                        catch (ObjectDisposedException) { }
                        catch (Exception ex3)
                        {
                            WriteLog(ex3.ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            Socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException ex2)
                        {
                            if (ex2.ErrorCode != 10057) WriteLog(ex2.ToString());
                        }
                        catch { }
                    }
                    Socket.Close();
                    Socket = null;
                }
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 完成事件，将在工作线程中被调用，不要占用太多时间。
        /// </summary>
        public event EventHandler<DME_NetEventArgs> Completed;

        private void OnCompleted(Object sender, SocketAsyncEventArgs e)
        {
            if (e is DME_NetEventArgs)
                RaiseComplete(e as DME_NetEventArgs);
            else
                throw new InvalidOperationException("所有套接字事件参数必须来自于事件参数池Pool！");
        }

        /// <summary>
        /// 触发完成事件。
        /// 可能由工作线程（事件触发）调用，也可能由用户线程通过线程池线程调用。
        /// 作为顶级，将会处理所有异常并调用OnError，其中OnError有能力回收参数e。
        /// </summary>
        /// <param name="e"></param>
        protected void RaiseComplete(DME_NetEventArgs e)
        {
            try
            {
                if (Completed != null) Completed(this, e);

                OnComplete(e);
                // 这里可以改造为使用多线程处理事件
                //ThreadPoolCallback(OnCompleted, e);
            }
            catch (Exception ex)
            {
                OnError(e, ex);
                // 都是在线程池线程里面了，不要往外抛出异常
                //throw;
            }
        }

        /// <summary>
        /// 异步触发完成事件处理程序
        /// </summary>
        /// <param name="e"></param>
        protected void RaiseCompleteAsync(DME_NetEventArgs e)
        {
            if (UseThreadPool)
            {
                ThreadPool.QueueUserWorkItem(delegate(Object state)
                {
                    RaiseComplete(state as DME_NetEventArgs);
                }, e);
            }
            else
            {
                RaiseComplete(e);
            }
        }

        /// <summary>
        /// 完成事件分发中心。
        /// 正常执行时OnComplete必须保证回收参数e，异常时RaiseComplete将能够代为回收
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnComplete(DME_NetEventArgs e) { }

        /// <summary>
        /// 错误发生/断开连接时
        /// </summary>
        public event EventHandler<DME_NetEventArgs> Error;

        /// <summary>
        /// 错误发生/断开连接时。拦截Error事件中的所有异常，不外抛，防止因为Error异常导致多次调用OnError
        /// </summary>
        /// <param name="e"></param>
        /// <param name="ex"></param>
        protected void ProcessError(DME_NetEventArgs e, Exception ex)
        {
            if (Error != null)
            {
                if (ex != null)
                {
                    if (e == null) e = Pop();
                    e.UserToken = ex;
                }

                try
                {
                    Error(this, e);
                }
                catch (Exception ex2)
                {
                    WriteLog(ex2.ToString());
                }
            }

            // 不管有没有外部事件，都要归还网络事件参数，那是对象池的东西，不是你的
            if (e != null) Push(e);
        }

        /// <summary>
        /// 错误发生时。负责调用Error事件以及回收网络事件参数
        /// </summary>
        /// <remarks>OnError除了会调用ProcessError外，还会关闭Socket</remarks>
        /// <param name="e"></param>
        /// <param name="ex"></param>
        protected virtual void OnError(DME_NetEventArgs e, Exception ex)
        {
            try
            {
                ProcessError(e, ex);
            }
            finally
            {
                Close();
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 在线程池里面执行指定委托。内部会处理异常并调用OnError
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="e"></param>
        protected void ThreadPoolCallback(Action<DME_NetEventArgs> callback, DME_NetEventArgs e)
        {
            if (UseThreadPool)
            {
                ThreadPool.QueueUserWorkItem(delegate(Object state)
                {
                    try
                    {
                        callback(state as DME_NetEventArgs);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            OnError(state as DME_NetEventArgs, ex);
                        }
                        catch { }
                        // 都是在线程池线程里面了，不要往外抛出异常
                        //throw;
                    }
                }, e);
            }
            else
            {
                callback(e);
            }
        }

        ///// <summary>
        ///// 在线程池里调用事件处理并负责回收事件参数对象
        ///// </summary>
        ///// <param name="handler"></param>
        ///// <param name="e"></param>
        //protected void ThreadPoolInvoke(EventHandler<NetEventArgs> handler, NetEventArgs e)
        //{
        //    if (handler == null) return;

        //    ThreadPool.QueueUserWorkItem(delegate(Object state)
        //    {
        //        Object[] objs = (Object[])state;
        //        EventHandler<NetEventArgs> handler2 = objs[0] as EventHandler<NetEventArgs>;
        //        NetEventArgs e2 = objs[1] as NetEventArgs;
        //        try
        //        {
        //            handler2(this, e2);
        //        }
        //        finally
        //        {
        //            // 回收
        //            Push(e2);
        //        }
        //    }, new Object[] { handler, e });
        //}
        #endregion
    }
}
