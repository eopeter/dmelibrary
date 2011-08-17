using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Task
{
    public class DME_Task : IDisposable
    {
        #region Fields
        private string taskName;
        private Timer timer;
        private TimerCallback execTask;
        private DME_ISchedule schedule;
        private DateTime lastExecuteTime;
        private DateTime nextExecuteTime;
        #endregion

        #region Properties
        /// <summary> 
        /// 任务名称 
        /// </summary> 
        public string Name
        {
            set { taskName = value; }
            get { return taskName; }
        }
        /// <summary> 
        /// 执行任务的计划 
        /// </summary> 
        public DME_ISchedule Shedule
        {
            get { return schedule; }
        }
        /// <summary> 
        /// 该任务最后一次执行的时间 
        /// </summary> 
        public DateTime LastExecuteTime
        {
            get { return lastExecuteTime; }
        }
        /// <summary> 
        /// 任务下一次执行时间 
        /// </summary> 
        public DateTime NextExecuteTime
        {
            get { return nextExecuteTime; }
        }
        #endregion

        #region Methods
        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="callback"></param> 
        /// <param name="schedule">为每个任务制定一个执行计划</param> 
        public DME_Task(TimerCallback callback, DME_ISchedule schedule)
        {
            if (callback == null || schedule == null)
            {
                throw new ArgumentNullException();
            }
            this.execTask = callback;
            this.schedule = schedule;
            execTask += new TimerCallback(Execute);
            DME_TaskScheduler.Register(this);
        }

        /// <summary> 
        /// 任务内容 
        /// </summary> 
        /// <param name="state">任务函数参数</param> 
        private void Execute(object state)
        {
            lastExecuteTime = DateTime.Now;
            if (schedule.Period == Timeout.Infinite)
            {
                nextExecuteTime = DateTime.MaxValue; //下次运行的时间不存在 
            }
            else
            {
                TimeSpan period = new TimeSpan(schedule.Period * 1000);
                nextExecuteTime = lastExecuteTime + period;
            }
            if (!(schedule is DME_CycExecution))
            {
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Start(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execTaskState"></param>
        public void Start(object execTaskState)
        {
            if (timer == null)
            {
                timer = new Timer(execTask, execTaskState, schedule.DueTime, schedule.Period);
            }
            else
            {
                timer.Change(schedule.DueTime, schedule.Period);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshSchedule()
        {
            if (timer != null)
            {
                timer.Change(schedule.DueTime, schedule.Period);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public void Close()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (execTask != null)
            {
                taskName = null;
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                execTask = null;
                DME_TaskScheduler.Deregister(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return taskName;
        }
        #endregion
    }
}
