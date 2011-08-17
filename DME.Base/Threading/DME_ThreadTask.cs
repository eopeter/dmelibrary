using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Threading
{
    class DME_ThreadTask
    {
        private Int32 _ID;
        /// <summary>唯一编号</summary>
        public Int32 ID
        {
            get { return _ID; }
            private set { _ID = value; }
        }

        private WaitCallback _Method;
        /// <summary>任务方法</summary>
        public WaitCallback Method
        {
            get { return _Method; }
            set { _Method = value; }
        }

        private Object _Argument;
        /// <summary>任务参数</summary>
        public Object Argument
        {
            get { return _Argument; }
            set { _Argument = value; }
        }

        private WaitCallback _AbortMethod;
        /// <summary>取消任务时执行的方法</summary>
        public WaitCallback AbortMethod
        {
            get { return _AbortMethod; }
            set { _AbortMethod = value; }
        }

        private WaitCallback _CompleteMethod;
        /// <summary>完成任务后执行的方法</summary>
        public WaitCallback CompleteMethod
        {
            get { return _CompleteMethod; }
            set { _CompleteMethod = value; }
        }

        private static Object newID_Lock = new object();
        private static Int32 _newID;
        /// <summary>
        /// 取一个新编号
        /// </summary>
        private static Int32 newID
        {
            get
            {
                lock (newID_Lock)
                {
                    _newID++;
                    return _newID;
                }
            }
        }

        /// <summary>
        /// 构造一个线程任务
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="argument">任务参数</param>
        public DME_ThreadTask(WaitCallback method, Object argument)
        {
            Method = method;
            Argument = argument;
            ID = newID;
            AbortMethod = null;
            CompleteMethod = null;
        }

        /// <summary>
        /// 构造一个线程任务
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="argument">任务参数</param>
        /// <param name="completeMethod">完成任务后执行的方法</param>
        public DME_ThreadTask(WaitCallback method, Object argument, WaitCallback completeMethod)
        {
            Method = method;
            Argument = argument;
            ID = newID;
            AbortMethod = null;
            CompleteMethod = completeMethod;
        }

        /// <summary>
        /// 构造一个线程任务
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="abortMethod">任务被取消时执行的方法</param>
        /// <param name="argument">任务参数</param>
        public DME_ThreadTask(WaitCallback method, WaitCallback abortMethod, Object argument)
        {
            Method = method;
            Argument = argument;
            ID = newID;
            AbortMethod = abortMethod;
            CompleteMethod = null;
        }

        /// <summary>
        /// 构造一个线程任务
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="abortMethod">任务被取消时执行的方法</param>
        /// <param name="completeMethod">完成任务后执行的方法</param>
        /// <param name="argument">任务参数</param>
        public DME_ThreadTask(WaitCallback method, WaitCallback abortMethod, WaitCallback completeMethod, Object argument)
        {
            Method = method;
            Argument = argument;
            ID = newID;
            AbortMethod = abortMethod;
            CompleteMethod = completeMethod;
        }
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum DME_TaskState
    {
        /// <summary>
        /// 未处理
        /// </summary>
        Unstarted = 0,

        /// <summary>
        /// 正在处理
        /// </summary>
        Running = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        Finished = 2
    }
}
