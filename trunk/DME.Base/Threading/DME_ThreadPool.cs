using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DME.Base.Log;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;

namespace DME.Base.Threading
{
    /// <summary>
    /// 线程池。所有静态方法和实例方法均是线程安全。
    /// </summary>
    public sealed class DME_ThreadPool : IDisposable
    {
        /// <summary>锁</summary>
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        #region 属性
        #region 基本属性
        private Int32 _MaxThreads;
        /// <summary>最大线程数</summary>
        public Int32 MaxThreads
        {
            get { return _MaxThreads; }
            set { _MaxThreads = value; }
        }

        private Int32 _MinThreads;
        /// <summary>最小线程数</summary>
        public Int32 MinThreads
        {
            get { return _MinThreads; }
            set { _MinThreads = value; }
        }

        private String _Name;
        /// <summary>线程池名称</summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private Exception _LastError;
        /// <summary>最后的异常</summary>
        public Exception LastError
        {
            get { return _LastError; }
            set { _LastError = value; }
        }
        #endregion

        #region 线程
        

        /// <summary>
        /// 使用volatile关键字，等到对象创建完成
        /// </summary>
        private volatile Thread _ManagerThread;
        /// <summary>维护线程</summary>
        private Thread ManagerThread
        {
            get
            {
                if (_ManagerThread == null)
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        if (_ManagerThread == null)
                        {
                            Thread thread = new Thread(Work);
                            thread.Name = Name + "线程池维护线程";
                            thread.IsBackground = true;
                            thread.Priority = ThreadPriority.Highest;//最高优先级
                            _ManagerThread = thread;
                        }
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                }
                return _ManagerThread;
            }
        }

        /// <summary>
        /// 第一个任务到来时初始化线程池
        /// </summary>
        private void Init()
        {
            if (ManagerThread.IsAlive) return;
            if ((ManagerThread.ThreadState & ThreadState.Unstarted) != ThreadState.Unstarted) return;

            ManagerThread.Start();

            WriteLog("初始化线程池：" + Name + " 最大：" + MaxThreads + " 最小：" + MinThreads);
        }

        private List<DME_Thread> _Threads;
        /// <summary>线程组。适用该资源时，记得加上线程锁lockObj</summary>
        private List<DME_Thread> Threads
        {
            get
            {
                if (_Threads == null) _Threads = new List<DME_Thread>();
                return _Threads;
            }
        }

        private Int32 _ThreadCount;
        /// <summary>当前线程数</summary>
        public Int32 ThreadCount
        {
            get { return _ThreadCount; }
            private set { _ThreadCount = value; }
        }

        private Int32 _RunningCount;
        /// <summary>正在处理任务的线程数</summary>
        public Int32 RunningCount
        {
            get { return _RunningCount; }
            private set { _RunningCount = value; }
        }

        private AutoResetEvent _Event = new AutoResetEvent(false);
        /// <summary>事件量</summary>
        private AutoResetEvent Event
        {
            get { return _Event; }
        }

        #endregion
        #endregion

        #region 任务队列
        private SortedList<Int32, DME_ThreadTask> _Tasks;
        /// <summary>任务队列</summary>
        private SortedList<Int32, DME_ThreadTask> Tasks
        {
            get
            {
                if (_Tasks == null) _Tasks = new SortedList<Int32, DME_ThreadTask>();
                return _Tasks;
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 构造一个线程池
        /// </summary>
        /// <param name="name">线程池名</param>
        private DME_ThreadPool(String name)
        {
            Name = name;

            //最大线程数为4×处理器个数
            MaxThreads = 10 * Environment.ProcessorCount;
            MinThreads = 2 * Environment.ProcessorCount;
        }

        private static Dictionary<String, DME_ThreadPool> _cache = new Dictionary<string, DME_ThreadPool>();
        /// <summary>
        /// 创建线程池。一个名字只能创建一个线程池。线程安全。
        /// </summary>
        /// <param name="name">线程池名</param>
        /// <returns></returns>
        public static DME_ThreadPool Create(String name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(name, "线程池名字不能为空！");
            if (_cache.ContainsKey(name)) return _cache[name];
            cacheLock.EnterWriteLock();
            try
            {
                if (_cache.ContainsKey(name)) return _cache[name];
                DME_ThreadPool pool = new DME_ThreadPool(name);
                _cache.Add(name, pool);
                return pool;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
        #endregion

        #region 队列操作
        /// <summary>
        /// 把用户工作项放入队列
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <returns>任务编号</returns>
        public Int32 Queue(WaitCallback method)
        {
            return Queue(method, null);
        }

        /// <summary>
        /// 把用户工作项放入队列
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="argument">任务参数</param>
        /// <returns>任务编号</returns>
        public Int32 Queue(WaitCallback method, Object argument)
        {
            return Queue(new DME_ThreadTask(method, argument));
        }

        /// <summary>
        /// 把用户工作项放入队列
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="argument">任务参数</param>
        /// <param name="completeMethod">完成任务后执行的方法</param>
        /// <returns>任务编号</returns>
        public Int32 Queue(WaitCallback method, Object argument, WaitCallback completeMethod)
        {
            return Queue(new DME_ThreadTask(method, argument, completeMethod));
        }

        /// <summary>
        /// 把用户工作项放入队列。指定任务被取消时执行的方法，该方法仅针对尚未被线程开始调用时的任务有效
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="abortMethod">任务被取消时执行的方法</param>
        /// <param name="argument">任务参数</param>
        /// <returns>任务编号</returns>
        public Int32 Queue(WaitCallback method, WaitCallback abortMethod, Object argument)
        {
            return Queue(new DME_ThreadTask(method, abortMethod, argument));
        }

        /// <summary>
        /// 把用户工作项放入队列。指定任务被取消时执行的方法，该方法仅针对尚未被线程开始调用时的任务有效
        /// </summary>
        /// <param name="method">任务方法</param>
        /// <param name="abortMethod">任务被取消时执行的方法</param>
        /// <param name="completeMethod">完成任务后执行的方法</param>
        /// <param name="argument">任务参数</param>
        /// <returns>任务编号</returns>
        public Int32 Queue(WaitCallback method, WaitCallback abortMethod, WaitCallback completeMethod, Object argument)
        {
            return Queue(new DME_ThreadTask(method, abortMethod, completeMethod,argument));
        }

        /// <summary>
        /// 把用户工作项放入队列
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns>任务编号</returns>
        private Int32 Queue(DME_ThreadTask task)
        {
            //加锁，防止冲突
            cacheLock.EnterWriteLock();
            try
            {
                Tasks.Add(task.ID, task);                
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }

            //初始化线程池
            if (ManagerThread == null || !ManagerThread.IsAlive) Init();

            //通知管理线程，任务到达
            Event.Set();

            return task.ID;
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns>任务状态</returns>
        public DME_TaskState Abort(Int32 id)
        {
            // 重点：
            // 这里使用了锁，很危险，所以仅仅在锁里面删除任务，任务的善后处理在锁外面完成

            // 要取消的任务
            DME_ThreadTask task = null;
            // 任务状态
            DME_TaskState state = DME_TaskState.Finished;

            #region 检查任务是否还在队列里面
            if (Tasks.ContainsKey(id))
            {
                //加锁，防止冲突
                cacheLock.EnterWriteLock();
                try
                {
                    if (Tasks.ContainsKey(id))
                    {
                        task = Tasks[id];
                        Tasks.Remove(id);
                        state = DME_TaskState.Unstarted;
                    }
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            #endregion

            #region 检查任务是否正在处理
            if (task == null && Threads.Count > 0)
            {
                cacheLock.EnterWriteLock();
                try
                {
                    if (Threads.Count > 0)
                    {
                        foreach (DME_Thread item in Threads)
                        {
                            if (item.Task != null && item.Task.ID == id)
                            {
                                task = item.Task;
                                Boolean b = item.Running;
                                item.Abort(true);
                                if (b)
                                    state = DME_TaskState.Running;
                                else
                                    state = DME_TaskState.Finished;
                            }
                        }
                    }
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            #endregion

            if (task == null) state = DME_TaskState.Finished;

            // 处理任务结束时的事情
            if (task != null && task.AbortMethod != null)
            {
                try { task.AbortMethod(task.Argument); }
                catch { }
            }

            return state;
        }

        /// <summary>
        /// 取消所有未开始任务
        /// </summary>
        /// <remarks>这里不要调用上面Abort取消单个任务，否则可能会造成死锁</remarks>
        public void AbortAllTask()
        {
            // 重点：
            // 这里使用了锁，很危险，所以仅仅在锁里面删除任务，任务的善后处理在锁外面完成

            if (Tasks == null || Tasks.Count < 1) return;
            List<DME_ThreadTask> list = null;

            cacheLock.EnterWriteLock();
            try
            {
                if (Tasks == null || Tasks.Count < 1) return;
                list = new List<DME_ThreadTask>();
                foreach (DME_ThreadTask item in Tasks.Values)
                {
                    list.Add(item);
                }
                Tasks.Clear();
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }

            if (list == null || list.Count < 1) return;

            foreach (DME_ThreadTask item in list)
            {
                if (item.AbortMethod != null)
                {
                    try { item.AbortMethod(item.Argument); }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 取消所有进行中任务
        /// </summary>
        /// <remarks>这里不要调用上面Abort取消单个任务，否则可能会造成死锁</remarks>
        public void AbortAllThread()
        {
            // 重点：
            // 这里使用了锁，很危险，所以仅仅在锁里面删除任务，任务的善后处理在锁外面完成

            if (Threads == null || Threads.Count < 1) return;
            List<DME_ThreadTask> list = null;
            cacheLock.EnterWriteLock();
            try
            {
                if (Threads == null || Threads.Count < 1) return;

                list = new List<DME_ThreadTask>();
                foreach (DME_Thread item in Threads)
                {
                    if (item.Running)
                    {
                        list.Add(item.Task);
                        item.Abort(true);
                    }
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }

            if (list == null || list.Count < 1) return;

            foreach (DME_ThreadTask item in list)
            {
                if (item.AbortMethod != null)
                {
                    try { item.AbortMethod(item.Argument); }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 取消所有任务
        /// </summary>
        /// <remarks>这里不要调用上面Abort取消单个任务，否则可能会造成死锁</remarks>
        public void AbortAll()
        {
            AbortAllTask();
            AbortAllThread();
        }

        /// <summary>
        /// 查询任务状态
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns>任务状态</returns>
        public DME_TaskState Query(Int32 id)
        {
            if (Tasks == null || Tasks.Count < 1) return DME_TaskState.Unstarted;

            //检查任务是否还在队列里面
            if (Tasks.ContainsKey(id)) return DME_TaskState.Unstarted;

            //检查任务是否正在处理
            if (Threads == null || Threads.Count < 1) return DME_TaskState.Finished;

            cacheLock.EnterReadLock();
            try
            {
                if (Threads == null || Threads.Count < 1) return DME_TaskState.Finished;
                foreach (DME_Thread item in Threads)
                {
                    if (item.Task != null && item.Task.ID == id)
                    {
                        if (item.Running)
                            return DME_TaskState.Running;
                        else
                            return DME_TaskState.Finished;
                    }
                }
                return DME_TaskState.Finished;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 查询任务个数
        /// </summary>
        /// <returns></returns>
        public Int32 QueryCount()
        {
            cacheLock.EnterReadLock();
            try
            {
                return Tasks.Count;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 等待所有任务完成，并指定是否在等待之前退出同步域。
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns>是否在等待之前退出同步域</returns>
        public Boolean WaitAll(Int32 millisecondsTimeout)
        {
            Stopwatch watch = Stopwatch.StartNew();

            Int32 Interval = 10;
            while (true)
            {
                if (RunningCount < 1)
                {
                    try
                    {
                        if (Tasks.Count < 1) break;
                    }
                    catch (Exception ex)
                    {
                        WriteLog("取任务数异常！" + ex.ToString());
                    }
                }
                if (watch.ElapsedMilliseconds >= millisecondsTimeout) return false;

                Thread.Sleep(Interval);
            }
            return true;
        }
        #endregion

        #region 维护
        /// <summary>
        /// 调度包装
        /// </summary>
        private void Work()
        {
            while (true)
            {
                try
                {
                    //等待事件量，超时1秒
                    Event.WaitOne(1000, true);
                    Event.Reset();
                    cacheLock.EnterWriteLock();
                    try
                    {
                        #region 线程维护与统计
                        Int32 freecount = 0;
                        //清理死线程
                        for (int i = Threads.Count - 1; i >= 0; i--)
                        {
                            if (Threads[i] == null)
                            {
                                Threads.RemoveAt(i);
                                DME_Log.WriteLine(Name + "线程池的线程对象为空，设计错误！");
                            }
                            else if (!Threads[i].IsAlive)
                            {
                                Threads[i].Dispose();
                                DME_Log.WriteLine(Threads[i].Name + "处于非活动状态，设计错误！");
                                Threads.RemoveAt(i);
                            }
                            else if (!Threads[i].Running)
                                freecount++;
                        }
                        //正在处理任务的线程数
                        RunningCount = Threads.Count - freecount;

                        WriteLog("总数：" + Threads.Count + "  可用：" + freecount + " 任务数：" + Tasks.Count);

                        Int32 count = MinThreads - freecount;
                        //保留最小线程数个线程
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                DME_Thread thread = AddThread();
                                if (thread != null) Threads.Add(thread);
                            }
                        }
                        else if (count < 0)//过多活动线程，清理不活跃的
                        {
                            for (int i = Threads.Count - 1; i >= 0 && count < 0; i--)
                            {
                                if (Threads[i].CanRelease)
                                {
                                    Threads[i].Dispose();
                                    Threads.RemoveAt(i);
                                    count++;
                                }
                            }
                        }
                        #endregion
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }

                    //检查任务，分派线程
                    if (Tasks.Count > 0)
                    {
                        cacheLock.EnterWriteLock();
                        try
                        {
                            while (Tasks.Count > 0)
                            {
                                //借一个线程
                                DME_Thread thread = Open();
                                if (thread == null) break;
                                WriteLog("借得线程" + thread.Name);

                                //拿出一个任务
                                Int32 id = Tasks.Keys[0];
                                thread.Task = Tasks[id];
                                Tasks.RemoveAt(0);
                                thread.Start();
                            }
                        }
                        finally
                        {
                            cacheLock.ExitWriteLock();
                        }
                    }
                }
                catch (ThreadInterruptedException ex)
                {
                    LastError = ex;
                    break;
                }
                catch (ThreadAbortException ex)
                {
                    LastError = ex;

                    break;
                }
                catch (Exception ex)
                {
                    LastError = ex;
                    DME_Log.WriteLine(ex.ToString());
                }
            }
            AbortAll();
        }

        /// <summary>
        /// 添加线程。本方法不是线程安全，调用者需要自己维护线程安全
        /// </summary>
        /// <returns></returns>
        private DME_Thread AddThread()
        {
            //保证活动线程数不超过最大线程数
            if (Threads.Count >= MaxThreads) return null;

            DME_Thread thread = new DME_Thread();
            thread.Name = String.Format("{0}线程池{1,3}号线程", Name, ThreadCount);
            thread.OnTaskFinished += new EventHandler<EventArgs>(thread_OnTaskFinished);

            ThreadCount++;

            WriteLog("新建线程：" + thread.Name);
            return thread;
        }

        void thread_OnTaskFinished(object sender, EventArgs e)
        {
            DME_Thread thread = sender as DME_Thread;
            if (thread.Task.CompleteMethod != null)
            {
                try { thread.Task.CompleteMethod(thread.Task.Argument); }
                catch { }
            }
            Close(thread);

            //通知管理线程，任务完成
            Event.Set();
        }
        #endregion

        #region 线程调度
        /// <summary>
        /// 借用线程 本方法不是线程安全，调用者需要自己维护线程安全
        /// </summary>
        /// <returns></returns>
        private DME_Thread Open()
        {
            //cacheLock.EnterWriteLock();
            //try
            //{
                foreach (DME_Thread item in Threads)
                {
                    if (item != null && item.IsAlive && !item.Running) return item;
                }

                //没有空闲线程，加一个
                if (Threads.Count < MaxThreads)
                {
                    DME_Thread thread = AddThread();
                    Threads.Add(thread);

                    RunningCount++;

                    return thread;
                }
                else
                    WriteLog("已达到最大线程数！");
            //}
            //finally
            //{
            //    cacheLock.ExitWriteLock();
            //}
            return null;
        }

        /// <summary>
        /// 归还线程
        /// </summary>
        /// <param name="thread"></param>
        private void Close(DME_Thread thread)
        {
            if (thread == null) return;
            WriteLog("归还线程" + thread.Name);

            RunningCount--;

            //看这个线程是活的还是死的，死的需要清除
            if (!thread.IsAlive)
            {
                if (Threads.Contains(thread))
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        if (Threads.Contains(thread))
                        {
                            Threads.Remove(thread);
                            DME_Log.WriteLine("归还" + thread.Name + "时发现，线程被关闭了，设计错误！");
                        }
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                }
                thread.Dispose();
            }
        }
        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            WriteLog(Name + "线程池释放资源");
            if (Threads != null && Threads.Count > 0)
            {
                cacheLock.EnterWriteLock();
                try
                {
                    if (Threads != null && Threads.Count > 0)
                    {
                        for (int i = Threads.Count - 1; i >= 0; i--)
                        {
                            if (Threads[i] != null)
                            {
                                Threads[i].Dispose();
                            }
                            Threads.RemoveAt(i);
                        }
                    }
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            if (ManagerThread != null && ManagerThread.IsAlive) ManagerThread.Abort();

            if (_Event != null) _Event.Close();
            cacheLock.Dispose();
        }

        /// <summary>
        /// 析构
        /// </summary>
        ~DME_ThreadPool()
        {
            Dispose(false);
        }
        #endregion

        #region 辅助函数
        private static void WriteLog(String msg)
        {
            if (DME_Log.Debug) DME_Log.WriteLine("线程：" + Thread.CurrentThread.Name + " 信息：" + msg);
        }

        /// <summary>
        /// 已重载。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}线程池，线程数：{1}，任务数：{2}", Name, Threads.Count, Tasks.Count);
        }
        #endregion
    }
}
