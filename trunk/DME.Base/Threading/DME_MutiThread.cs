using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Threading
{
    public class DME_MutiThread
    {
         #region 字段和属性
        /// <summary>
        /// 线程数组
        /// </summary>
        private Thread[] workThreads;

        public Thread[] WorkThreads
        {
            get { return workThreads; }
        }

        /// <summary>
        /// 线程数量
        /// </summary>
        private int threadSum;

        public int ThreadSum
        {
            get { return threadSum; }
        }

        /// <summary>
        /// 线程状态
        /// </summary>
        private bool threadState;

        public bool ThreadState
        {
            get { return threadState; }
            set { threadState = value; }
        }

        /// <summary>
        /// 工作具体操作的类
        /// </summary>
        private DME_WorkThread works;

        public DME_WorkThread Works
        {
            get { return works; }
            set { works = value; }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime endTime;

        public DateTime EndTime
        {
            get { return endTime; }
        }

        #endregion

        #region 构造方法
        public DME_MutiThread() 
        {
            this.threadState = false;
            this.works = null;
            this.threadSum = 0;
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Works"></param>
        /// <param name="ThreadSum"></param>
        public DME_MutiThread(DME_WorkThread Works, int ThreadSum) 
        {
            this.threadState = false;
            this.works = Works;
            this.threadSum = ThreadSum > 0 ? ThreadSum : 1;
            this.workThreads = new Thread[this.threadSum];
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 初始化线程
        /// </summary>
        private void InitAllThreads() 
        {
            for (int i = 0; i < this.workThreads.Length; i++)
            {
                this.workThreads[i] = new Thread(new ThreadStart(this.works.Works));
                this.workThreads[i].Name = "线程<" + i + ">";
            }
        }

        /// <summary>
        /// 开始执行工作
        /// </summary>
        public void StartThreads() 
        {
            this.InitAllThreads();
            this.startTime = DateTime.Now;
            for (int i = 0; i < this.workThreads.Length; i++)
            {
                this.workThreads[i].Start();
            }
            this.threadState = true;
        }

        /// <summary>
        /// 检查线程状态
        /// </summary>
        /// <returns></returns>
        public bool CheckThreadState() 
        {
            bool flag = false;
            for (int i = 0; i < this.workThreads.Length; i++) 
            {
                if (this.workThreads[i].IsAlive) 
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        #endregion

        #region 委托关闭所有线程
        /// <summary>
        /// 关闭线程的委托
        /// </summary>
        public delegate void Close();

        /// <summary>
        /// 关闭所有线程
        /// </summary>
        private void CloseAllThreads() 
        {
            for (int i = 0; i < this.workThreads.Length; i++)
            {
                this.workThreads[i].Abort();
            }
            this.threadState = false;
            this.endTime = DateTime.Now;
        }

        /// <summary>
        /// 外部调用关闭线程的方法
        /// </summary>
        public void CloseThreads() 
        {
            Close close = new Close(CloseAllThreads);
            AsyncCallback AsyncCallBackMethon = new AsyncCallback(this.works.CloseThreadsCallBackMethon);
            close.BeginInvoke(AsyncCallBackMethon, close);
            Thread.CurrentThread.Join();
        }
        #endregion
    }
}
