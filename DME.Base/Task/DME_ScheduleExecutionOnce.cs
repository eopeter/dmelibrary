using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Task
{
    /// <summary>
    /// 计划在某一未来的时间执行一个操作一次，如果这个时间比现在的时间小，就变成了立即执行的方式 
    /// </summary>
    public struct DME_ScheduleExecutionOnce : DME_ISchedule
    {
        private DateTime schedule;

        /// <summary>
        /// 返回最初计划执行时间 
        /// </summary>
        public DateTime ExecutionTime
        {
            get { return schedule; }
            set { schedule = value; }
        }
        /// <summary> 
        /// 得到该计划还有多久才能运行 
        /// </summary> 
        public long DueTime
        {
            get
            {
                long ms = (schedule.Ticks - DateTime.Now.Ticks) / 10000;
                if (ms < 0)
                {
                    ms = 0;
                }
                return ms;
            }
        }
        /// <summary>
        /// 周期时间
        /// </summary>
        public long Period
        {
            get { return Timeout.Infinite; }
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="time">计划开始执行的时间</param> 
        public DME_ScheduleExecutionOnce(DateTime time)
        {
            schedule = time;
        }
    }
}
