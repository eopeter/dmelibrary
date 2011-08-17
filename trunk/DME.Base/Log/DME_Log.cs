using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DME.Base.Helper;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using DME.Base.IO;

namespace DME.Base.Log
{
    /// <summary>
    /// 日志类
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DME_Log
    {
        #region 私有变量
        private static StreamWriter LogWriter;
        private static String _LogDir;
        private static Timer AutoCloseWriterTimer;
        /// <summary>锁</summary>
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 是否当前进程的第一次写日志
        /// </summary>
        private static Boolean isFirst = false;
        #endregion

        #region 公有变量
        /// <summary>
        /// 写日志事件。绑定该事件后，DME_Log将不再把日志写到日志文件中去。
        /// </summary>
        public static event EventHandler<DME_WriteLogEventArgs> OnWriteLog;
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        private static Boolean? _Debug;
        /// <summary>是否调试。如果代码指定了值，则只会使用代码指定的值，否则每次都读取配置。</summary>
        public static Boolean Debug
        {
            get
            {
                if (_Debug != null) return _Debug.Value;
                return DME_LibraryConfig.DME_Debug;
            }
            set { _Debug = value; }
        }
        /// <summary>
        /// 日志目录
        /// </summary>
        public static String LogPath
        {
            
            get
            {              
                if (!DME_Validation.IsNull(_LogDir))
                {
                    return _LogDir;
                }

                _LogDir = DME_LibraryConfig.DME_LogPath;
                return _LogDir;
            }
        }
        #endregion

        #region 私有函数
        /// <summary>停止日志</summary>
        private static void CloseWriter(Object obj)
        {
            if (LogWriter == null) return;
            cacheLock.EnterWriteLock();
            try
            {
                if (LogWriter == null) return;
                LogWriter.Close();
                LogWriter.Dispose();
                LogWriter = null;
            }
            catch { }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
        /// <summary>使用线程池线程异步执行日志写入动作</summary>
        /// <param name="obj"></param>
        private static void PerformWriteLog(Object obj)
        {
            cacheLock.EnterWriteLock();
            try
            {
                // 初始化日志读写器
                if (LogWriter == null) InitLog();
                // 写日志
                LogWriter.WriteLine((String)obj);
                // 声明自动关闭日志读写器的定时器。无限延长时间，实际上不工作
                if (AutoCloseWriterTimer == null) AutoCloseWriterTimer = new Timer(new TimerCallback(CloseWriter), null, Timeout.Infinite, Timeout.Infinite);
                // 改变定时器为5秒后触发一次。如果5秒内有多次写日志操作，估计定时器不会触发，直到空闲五秒为止
                AutoCloseWriterTimer.Change(5000, Timeout.Infinite);
            }
            catch { }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
        private static void InitLog()
        {
            String path = LogPath;
            if (!DME_Files.FolderExists(path))
            {
                DME_Files.CreateFolder(path);
            }

            String logfile = System.IO.Path.Combine(path,DateTime.Now.ToString("yyyy_MM_dd") + ".log");
            int i = 0;
            while (i < 10)
            {
                try
                {
                    LogWriter = new StreamWriter(logfile, true, Encoding.UTF8);
                    LogWriter.AutoFlush = true;
                    break;
                }
                catch
                {
                    if (logfile.EndsWith("_" + i + ".log"))
                        logfile = logfile.Replace("_" + i + ".log", "_" + (++i) + ".log");
                    else
                        logfile = logfile.Replace(@".log", @"_0.log");
                }
            }
            if (i >= 10) throw new Exception("无法写入日志！");
            if (!isFirst)
            {
                isFirst = true;
                Process process = Process.GetCurrentProcess();
                String name = String.Empty;
                Assembly asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    if (DME_Validation.IsNull(name))
                    {
                        AssemblyTitleAttribute att = Attribute.GetCustomAttribute(asm, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
                        if (att != null) name = att.Title;
                    }
                    if (DME_Validation.IsNull(name))
                    {
                        AssemblyProductAttribute att = Attribute.GetCustomAttribute(asm, typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                        if (att != null) name = att.Product;
                    }

                    if (DME_Validation.IsNull(name))
                    {
                        AssemblyDescriptionAttribute att = Attribute.GetCustomAttribute(asm, typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
                        if (att != null) name = att.Description;
                    }
                }
                if (DME_Validation.IsNull(name))
                {
                    try
                    {
                        name = process.MachineName;
                    }
                    catch { }
                }
                // 通过判断LogWriter.BaseStream.Length，解决有时候日志文件为空但仍然加空行的问题
                if (File.Exists(logfile) && LogWriter.BaseStream.Length > 0) LogWriter.WriteLine();
                LogWriter.WriteLine("#Software: {0}", name);
                LogWriter.WriteLine("#ProcessID: {0}", process.Id);
                LogWriter.WriteLine("#BaseDirectory: {0}", AppDomain.CurrentDomain.BaseDirectory);
                LogWriter.WriteLine("#Date: {0:yyyy-MM-dd}", DateTime.Now);
                LogWriter.WriteLine("#Fields: 【Time】-【ThreadID】-【IsPoolThread】-【ThreadName】-【Message】");
            }
        }
        #endregion

        #region 公开函数
        /// <summary> 输出日志</summary>
        /// <param name="msg">信息</param>
        public static void WriteLine(String msg)
        {
            DME_WriteLogEventArgs e = new DME_WriteLogEventArgs(msg);
            if (OnWriteLog != null)
            {
                OnWriteLog(null, e);
                return;
            }

            String s = LogPath;
            bool b = Debug;

            PerformWriteLog(e.ToString());
        }

        /// <summary>堆栈调试。
        /// 输出堆栈信息，用于调试时处理调用上下文。
        /// 本方法会造成大量日志，请慎用。
        /// </summary>
        public static void DebugStack()
        {
            DebugStack(int.MaxValue);
        }

        /// <summary>堆栈调试。</summary>
        /// <param name="maxNum">最大捕获堆栈方法数</param>
        public static void DebugStack(int maxNum)
        {
            int skipFrames = 1;
            if (maxNum == int.MaxValue) skipFrames = 2;
            StackTrace st = new StackTrace(skipFrames, true);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("调用堆栈：");
            int count = Math.Min(maxNum, st.FrameCount);
            for (int i = 0; i < count; i++)
            {
                StackFrame sf = st.GetFrame(i);
                sb.AppendFormat("{0}->{1}", sf.GetMethod().DeclaringType.FullName, sf.GetMethod().ToString());
                if (i < count - 1) sb.AppendLine();
            }
            WriteLine(sb.ToString());
        }

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(String format, params Object[] args)
        {
            //处理时间的格式化
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] != null && args[i].GetType() == typeof(DateTime)) args[i] = ((DateTime)args[i]).ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
            }
            WriteLine(String.Format(format, args));
        }
        #endregion
    }
}
