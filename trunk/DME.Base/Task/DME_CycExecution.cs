using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Task
{
    /// <summary>
    /// 周期性的执行计划
    /// </summary>
    public struct DME_CycExecution : DME_ISchedule
    {
        private DateTime schedule;
        private TimeSpan period;

        /// <summary>
        /// 返回最初计划执行时间 
        /// </summary>
        public DateTime ExecutionTime
        {
            get { return schedule; }
            set { schedule = value; }
        }

        /// <summary>
        ///  得到该计划还有多久才能运行 
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
            get { return period.Ticks / 10000; }
        }

        /// <summary> 
        /// 构造函数,马上开始运行 
        /// </summary> 
        /// <param name="period">周期时间</param> 
        public DME_CycExecution(TimeSpan period)
        {
            this.schedule = DateTime.Now;
            this.period = period;
        }
        /// <summary> 
        /// 构造函数，在一个将来时间开始运行 
        /// </summary> 
        /// <param name="shedule">计划执行的时间</param> 
        /// <param name="period">周期时间</param> 
        public DME_CycExecution(DateTime shedule, TimeSpan period)
        {
            this.schedule = shedule;
            this.period = period;
        }
    }
}
