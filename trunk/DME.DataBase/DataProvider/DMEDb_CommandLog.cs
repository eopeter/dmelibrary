using System;
using System.IO ;
using System.Data ;
using DME.DataBase;

namespace DME.DataBase.DataProvider.Data
{
	/// <summary>
	/// 命令对象日志2008.7.18 增加线程处理,2011.5.9 增加执行时间记录
	/// </summary>
	public class DMEDb_CommandLog
	{
		//日志相关
        private static  string _dataLogFile;
        /// <summary>
        /// 获取或者设置日志文件的路径，可以带Web相对路径
        /// </summary>
        public static string DataLogFile
        {
            get {
                if (DME.Base.Helper.DME_Validation.IsNull(_dataLogFile))
                {

                    _dataLogFile = DME.Base.DME_LibraryConfig.DMEDb_DataLogPath;
                    DME.Base.Helper.DME_Files.InitFolder(_dataLogFile);
                    if (!DME.Base.Helper.DME_Validation.IsNull(_dataLogFile))
                    {
                        _dataLogFile = System.IO.Path.Combine(_dataLogFile, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
                        _saveCommandLog = DME.Base.DME_LibraryConfig.DMEDb_SaveCommandLog;
                    }
                }
                return _dataLogFile;
            }
            set { _dataLogFile = value; }
        }

        private static bool _saveCommandLog;
        /// <summary>
        /// 是否记录日志文件
        /// </summary>
        public  static  bool SaveCommandLog
        {
            get {
                string temp = DataLogFile;//必须先调用下，以计算_saveCommandLog
                return _saveCommandLog;
            }
            set { _saveCommandLog = value; }
        }

        private static long _logExecutedTime = -1;
        /// <summary>
        /// 需要记录的时间，只有该值等于0会记录所有查询，否则只记录大于该时间的查询。单位毫秒。
        /// </summary>
        public static long LogExecutedTime
        {
            get {
                if (_logExecutedTime ==-1)
                {
                    string temp = DME.Base.DME_LibraryConfig.DMEDb_LogExecutedTime;
                    if(string.IsNullOrEmpty (temp ))
                        _logExecutedTime =0;
                    else
                        long.TryParse(temp, out _logExecutedTime);
                }
                return _logExecutedTime;
            }
            set { _logExecutedTime = value; }
        }

		private static DMEDb_CommandLog _Instance;
        private static object lockObj = new object();
        private System.Diagnostics.Stopwatch watch = null;
        

		/// <summary>
		/// 获取单例对象
		/// </summary>
		public static DMEDb_CommandLog Instance
		{
			get
			{
                if (_Instance == null)
                {
                    lock (lockObj)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new DMEDb_CommandLog();
                        }
                    }
                }
				return _Instance;
			}
		}

		/// <summary>
		/// 默认构造函数
		/// </summary>
		public DMEDb_CommandLog()
		{
            
		}

        /// <summary>
        /// 是否开启执行时间记录
        /// </summary>
        /// <param name="startStopwatch"></param>
        public DMEDb_CommandLog(bool startStopwatch)
        {
            if (startStopwatch)
            {
                watch = new System.Diagnostics.Stopwatch();
                watch.Start();            
            }
        }

        /// <summary>
        /// 重新开始记录执行时间
        /// </summary>
        public void ReSet()
        {
            if (watch != null)
                watch.Reset();
        }

        /// <summary>
        /// 获取当前执行的实际SQL语句
        /// </summary>
        public string CommandText
        {
            private set;
            get;
        }
       
		/// <summary>
        /// 写命令日志和执行时间（如果开启的话）
		/// </summary>
        /// <param name="command">命令对象</param>
		/// <param name="who">调用命令的源名称</param>
        /// <param name="elapsedMilliseconds">执行时间</param>
		public void WriteLog(IDbCommand command,string who,out long elapsedMilliseconds)
		{
            CommandText = command.CommandText;
            elapsedMilliseconds = 0;
            if (SaveCommandLog)
            {
                if (watch != null)
                {
                    elapsedMilliseconds = watch.ElapsedMilliseconds;
                    if ((LogExecutedTime > 0 && elapsedMilliseconds > LogExecutedTime) || LogExecutedTime == 0)
                    {
                        RecordCommandLog(command, who);
                        WriteLog("Execueted Time(ms):" + elapsedMilliseconds + "\r\n", who);
                    }
                }
                else
                {
                    RecordCommandLog(command, who);
                }
            }
		}

		/// <summary>
		///写入日志消息
		/// </summary>
		/// <param name="msg">消息</param>
		/// <param name="who">发送者</param>
		public void WriteLog(string msg,string who)
		{
			if(SaveCommandLog)
				WriteLog ("//"+DateTime.Now.ToString ()+ " @"+who+" ："+msg+"\r\n");
		}

        /// <summary>
        /// 写错误日志，将使用 DataLogFile 配置键的文件名写文件，不受SaveCommandLog 影响，除非 DataLogFile 未设置或为空。
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="errmsg">调用命令的源名称</param>
        public void WriteErrLog(IDbCommand command, string errmsg)
        {
            if (!string.IsNullOrEmpty(DataLogFile))
            {
                errmsg += ":Error";
                RecordCommandLog(command, errmsg);
            }
        }

		/// <summary>
		/// 获取日志文本
		/// </summary>
		/// <returns>日志文本</returns>
		public string GetLog()
		{
			StreamReader sr= File.OpenText(DataLogFile );
			string text=sr.ReadToEnd ();
			sr.Close();
			return text;
		}

		/// <summary>
		/// 记录命令信息
		/// </summary>
		/// <param name="command">命令对象</param>
        /// <param name="who">执行人</param>
		private void RecordCommandLog(IDbCommand command,string who)
		{
			string temp="//"+DateTime.Now.ToString ()+ " @"+who+" 执行命令：\r\nSQL=\""+command.CommandText+"\"\r\n//命令类型："+command.CommandType.ToString ();
			if(command.Transaction !=null)
				temp=temp.Replace ("执行命令","执行事务");
			WriteLog(temp);
			if(command.Parameters.Count >0)
			{
				WriteLog("//"+command.Parameters.Count+"个命令参数：");
				for(int i=0;i<command.Parameters.Count ;i++)
				{
					IDataParameter p=(IDataParameter)command.Parameters[i];
					WriteLog ("Parameter[\""+p.ParameterName+"\"]\t=\t\""+Convert.ToString ( p.Value)+"\"  \t\t\t//DbType=" +p.DbType.ToString ());
				}
			}
			

		}

		/// <summary>
		/// 写入日志
		/// </summary>
		/// <param name="log"></param>
		private void WriteLog(string log)
		{
            lock (lockObj)
            {
                StreamWriter sw = File.AppendText(DataLogFile); ;
                sw.WriteLine(log);
                sw.Close();
            }
		}
	}
}
