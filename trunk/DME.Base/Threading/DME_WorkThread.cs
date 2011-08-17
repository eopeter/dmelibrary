using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Threading
{
    /// <summary>
    /// 多线程通过操作类库
    /// </summary>
    public class DME_WorkThread
    {
        #region 内部事件
        /// <summary>
        /// 线程结束事件
        /// </summary>
        public event EventHandler OnEndThread;
        #endregion

        #region 字段和属性
        //多线程操作类
        private DME_MutiThread threads;
        /// <summary>
        /// 多线程操作类
        /// </summary>
        public DME_MutiThread Threads
        {
            get { return threads; }
            set { threads = value; }
        }

        //工作函数被执行的次数
        private int execTimes;
        /// <summary>
        /// 工作函数被执行的次数
        /// </summary>
        public int ExecTimes
        {
            get { return execTimes; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        public DME_WorkThread()
        {
            this.execTimes = 0;
        }
        #endregion

        #region 类的方法
        /// <summary>
        /// 工作函数
        /// </summary>
        public virtual void Works()
        {
            if (this.threads == null)
            {
                throw new Exception("组线程类无法初始化!");
            }
            while (true)
            {
                Monitor.Enter(this);//锁定，保持同步
                Console.WriteLine(Thread.CurrentThread.Name + "正在执行第" + ExecTimes + "次操作!");
                if (this.execTimes == 1000)
                {
                    this.threads.CloseThreads();
                }
                this.execTimes++;
                Monitor.Exit(this);//取消锁定
                Thread.Sleep(50);
            }

        }

        /// <summary>
        /// 开始工作
        /// </summary>
        /// <param name="ThreadSum"></param>
        public virtual void StartWork(int ThreadSum) 
        {
            this.threads = new DME_MutiThread(this, ThreadSum);
            this.Threads.StartThreads();
        }

        /// <summary>
        /// 结束工作
        /// </summary>
        public virtual void EndWork() 
        {
            this.Threads.CloseThreads();
        }

        /// <summary>
        /// 关闭线程的回调函数
        /// </summary>
        /// <param name="AsyncResult"></param>
        public void CloseThreadsCallBackMethon(IAsyncResult AsyncResult)
        {
            this.OnEndThread(this, new EventArgs());
            DME_MutiThread.Close close = (DME_MutiThread.Close)AsyncResult.AsyncState;
            close.EndInvoke(AsyncResult);
        }
        #endregion
    }
}
