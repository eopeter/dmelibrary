﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using DME.Base.Log;

namespace DME.Base.Network.Common
{
    /// <summary>
    /// 网络工具类
    /// </summary>
    public static class DME_NetHelper
    {
        #region 日志输出
        private static Boolean? _Debug;
        /// <summary>
        /// 是否调试
        /// </summary>
        public static Boolean Debug
        {
            get
            {
                if (_Debug != null) return _Debug.Value;
                _Debug = DME_LibraryConfig.DME_Debug;
                return _Debug.Value;
            }
            set { _Debug = value; }
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(String msg)
        {
            DME_Log.WriteLine(msg);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLog(String format, params Object[] args)
        {
            DME_Log.WriteLine(format, args);
        }
        #endregion

        #region 辅助函数
        /// <summary>
        /// 分析地址
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static IPAddress ParseAddress(String hostname)
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
            int index = 0;
            while ((index < hostAddresses.Length) && (hostAddresses[index].AddressFamily != AddressFamily.InterNetwork))
            {
                index++;
            }
            if (hostAddresses.Length > 0 && index < hostAddresses.Length) return hostAddresses[index];

            return null;
        }

        /// <summary>
        /// 获取本地IPV4列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetIPV4()
        {
            IPAddress[] IPList = Dns.GetHostAddresses(Dns.GetHostName());
            List<IPAddress> list = new List<IPAddress>();
            foreach (IPAddress item in IPList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork) list.Add(item);
            }

            return list;
        }
        #endregion
    }
}
