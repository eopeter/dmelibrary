using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.Network.Common;

namespace DME.Base.Network.Sockets
{
    /// <summary>
    /// 网络基类，提供资源释放和日志输出的统一处理
    /// </summary>
    public abstract class DME_Netbase : DME_DisposeBase
    {
        #region 日志
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(String msg)
        {
            if (DME_NetHelper.Debug) DME_NetHelper.WriteLog(msg);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLog(String format, params Object[] args)
        {
            if (DME_NetHelper.Debug) DME_NetHelper.WriteLog(format, args);
        }
        #endregion
    }
}
