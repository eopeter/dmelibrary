using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Task
{
    /// <summary> 
    /// 任务管理中心 
    /// 使用它可以管理一个或则多个同时运行的任务 
    /// </summary> 
    public static class DME_TaskScheduler
    {
        private static List<DME_Task> taskScheduler;
        /// <summary>锁</summary>
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 任务的总数
        /// </summary>
        public static int Count
        {
            get { return taskScheduler.Count; }
        }

        /// <summary>
        /// 构造
        /// </summary>
        static DME_TaskScheduler()
        {
            taskScheduler = new List<DME_Task>();
        }

        /// <summary>
        /// 查找任务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DME_Task Find(string name)
        {
            return taskScheduler.Find(task => task.Name == name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerator<DME_Task> GetEnumerator()
        {
            return taskScheduler.GetEnumerator();
        }

        /// <summary> 
        /// 终止任务 
        /// </summary> 
        public static void TerminateAllTask()
        {
            lock (taskScheduler)
            {
                taskScheduler.ForEach(task => task.Close());
                taskScheduler.Clear();
                taskScheduler.TrimExcess();
            }
        }

        /// <summary>
        /// 注册任务
        /// </summary>
        /// <param name="task"></param>
        internal static void Register(DME_Task task)
        {
            cacheLock.EnterWriteLock();
            try
            {
                taskScheduler.Add(task);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
        /// <summary>
        /// 注销任务
        /// </summary>
        /// <param name="task"></param>
        internal static void Deregister(DME_Task task)
        {
            cacheLock.EnterWriteLock();
            try
            {
                taskScheduler.Remove(task);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
    }
}
