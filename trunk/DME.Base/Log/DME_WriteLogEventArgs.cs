using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Log
{
    /// <summary>
    /// 写日志事件
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DME_WriteLogEventArgs : EventArgs
    {
        #region 私有变量
        private String _Message;
        private Exception _Exception;
        private DateTime _Time;
        private Int32 _ThreadID;
        private Boolean _IsPoolThread;
        private String _ThreadName;
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">日志</param>
        public DME_WriteLogEventArgs(String message) : this(message, null) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">日志</param>
        /// <param name="exception">异常</param>
        public DME_WriteLogEventArgs(String message, Exception exception)
        {
            Message = message;
            Exception = exception;
            Init();
        }
        #endregion

        #region 析构
        #endregion

        #region 属性
        /// <summary>日志信息</summary>
        public String Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        /// <summary>异常</summary>
        public Exception Exception
        {
            get { return _Exception; }
            set { _Exception = value; }
        }
        /// <summary>时间</summary>
        public DateTime Time
        {
            get { return _Time; }
            set { _Time = value; }
        }
        /// <summary>线程编号</summary>
        public Int32 ThreadID
        {
            get { return _ThreadID; }
            set { _ThreadID = value; }
        }
        /// <summary>是否线程池线程</summary>
        public Boolean IsPoolThread
        {
            get { return _IsPoolThread; }
            set { _IsPoolThread = value; }
        }
        /// <summary>线程名</summary>
        public String ThreadName
        {
            get { return _ThreadName; }
            set { _ThreadName = value; }
        }
        #endregion

        #region 私有函数
        private void Init()
        {
            Time = DateTime.Now;
            ThreadID = Thread.CurrentThread.ManagedThreadId;
            IsPoolThread = Thread.CurrentThread.IsThreadPoolThread;
            ThreadName = Thread.CurrentThread.Name;
        }
        #endregion

        #region 公开函数
        /// <summary>
        /// 已重载。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("【{0:HH:mm:ss.fff}】-【{1}】-【{2}】-【{3}】-【{4}】", Time, ThreadID, IsPoolThread ? 'Y' : 'N', String.IsNullOrEmpty(ThreadName) ? "-" : ThreadName, Message);
        }
        #endregion
    }
}
