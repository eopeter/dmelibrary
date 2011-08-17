using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base.Task
{
    /// <summary>
    /// 计划立即执行任务 
    /// </summary>
    public struct DME_ImmediateExecution : DME_ISchedule
    {
        /// <summary>
        /// 返回最初计划执行时间 
        /// </summary>
        public DateTime ExecutionTime
        {
            get { return DateTime.Now; }
            set { }
        }
        /// <summary>
        /// 得到该计划还有多久才能运行 
        /// </summary>
        public long DueTime
        {
            get { return 0; }
        }
        /// <summary>
        /// 周期时间
        /// </summary>
        public long Period
        {
            get { return Timeout.Infinite; }
        }
    }
}
