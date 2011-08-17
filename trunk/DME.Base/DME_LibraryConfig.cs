using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.IO;
using System.Threading;
using DME.Base.Helper;
using System.Configuration;

namespace DME.Base
{
    /// <summary>
    /// DME_Library
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
    public class DME_LibraryConfig
    {
        private static DME_Config config = new DME_Config("/DME_Library.config");
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 日志目录
        /// </summary>
        public static String DME_LogPath
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_Log", true);
                    return DME_Path.MapPath(config.GetAppConfig("LogPath", @"/Logs/Log/", true));
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_Log", true);
                    config.SetAppConfig("LogPath", value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 是否调试
        /// </summary>
        public static Boolean DME_Debug
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    String str = String.Empty;
                    config.GetAppSection("DME_Library", "DME_Log", true);
                    str = config.GetAppConfig("Debug", "False", true);
                    if (str.ToLower() == "true") return true;
                    if (str.ToLower() == "false") return false;
                    return false;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    String str = String.Empty;
                    config.GetAppSection("DME_Library", "DME_Log", true);
                    if (value)
                    {
                        str = "true";
                    }
                    else
                    {
                        str = "false";
                    }
                    config.SetAppConfig("Debug", str);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 日志密码
        /// </summary>
        public static String DME_LogPassword
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_Log", true);
                    return config.GetAppConfig("LogPassword", "DME123456", true);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_Log", true);
                    config.SetAppConfig("LogPassword",value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 数据库日志目录
        /// </summary>
        public static String DMEDb_DataLogPath
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    return DME_Path.MapPath(config.GetAppConfig("DataLogPath", "/Logs/DataLog/", true));
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    config.SetAppConfig("DataLogPath",value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 需要记录的时间，只有该值等于0会记录所有查询，否则只记录大于该时间的查询。单位毫秒。
        /// </summary>
        public static String DMEDb_LogExecutedTime
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    return DME_Path.MapPath(config.GetAppConfig("LogExecutedTime", "0", true));
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    config.SetAppConfig("LogExecutedTime", value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static String DMEDb_SqlMapFile
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    return DME_Path.MapPath(config.GetAppConfig("SqlMapFile", "", true));
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    config.SetAppConfig("SqlMapFile", value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 是否保存数据库Command记录
        /// </summary>
        public static Boolean DMEDb_SaveCommandLog
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    String str = String.Empty;
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    str = config.GetAppConfig("SaveCommandLog", "False", true);
                    if (str.ToLower() == "true") return true;
                    if (str.ToLower() == "false") return false;
                    return false;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    String str = String.Empty;
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    if (value)
                    {
                        str = "true";
                    }
                    else
                    {
                        str = "false";
                    }
                    config.SetAppConfig("SaveCommandLog", str);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
                 
            }
        }

        /// <summary>
        /// 数据库EngineType
        /// </summary>
        public static String DMEDb_EngineType
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    
                    return config.GetAppConfig("EngineType", "", true);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_DataBase", true);
                    config.SetAppConfig("EngineType", value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DMEDb_GetDataBaseConnection(string name)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();
            cacheLock.EnterWriteLock();
            try
            {
                config.GetConnectionSection("DME_Library", "DME_DataBase_Connection", true);
                string connectionString = string.Empty;
                string providerName = string.Empty;
                config.GetConnectionConfig(name, ref connectionString, ref providerName, true);
                dc.Add("connectionString", connectionString);
                dc.Add("providerName", providerName);
                return dc;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="EngineType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DMEDb_GetDBHelperStr(string EngineType)
        {
            string assembly = null;
            string type = null;
            Dictionary<string, string> dc = new Dictionary<string, string>();
            cacheLock.EnterWriteLock();
            try
            {
                config.GetAppSection("DME_Library", "DME_DataBase", true);
                switch (EngineType.ToUpper())
                {
                    case "SQLSERVER":
                        assembly = config.GetAppConfig("SqlServerHelperAssembly", "DME.DataBase", true);
                        type = config.GetAppConfig("SqlServerHelperType", "DME.DataBase.DataProvider.Data.SqlServer", true);
                        break;
                    case "SQLSERVERCE":
                        assembly = config.GetAppConfig("SqlServerCeHelperAssembly", "DME.DataBase", true);
                        type = config.GetAppConfig("SqlServerCeHelperType", "DME.DataBase.DataProvider.Data.SqlServerCe", true);
                        break;
                    case "OLEDB":
                        assembly = config.GetAppConfig("OleDbHelperAssembly", "DME.DataBase", true);
                        type = config.GetAppConfig("OleDbHelperType", "DME.DataBase.DataProvider.Data.OleDb", true);
                        break;
                    case "ACCESS":
                        assembly = config.GetAppConfig("AccessHelperAssembly", "DME.DataBase", true);
                        type = config.GetAppConfig("AccessHelperType", "DME.DataBase.DataProvider.Data.Access", true);
                        break;
                    case "ODBC":
                        assembly = config.GetAppConfig("OdbcHelperAssembly", "DME.DataBase", true);
                        type = config.GetAppConfig("OdbcHelperType", "DME.DataBase.DataProvider.Data.Odbc", true);
                        break;
                    case "ORACLE":
                        assembly = config.GetAppConfig("OracleHelperAssembly", "DME.DataBase", true);
                        type = config.GetAppConfig("OracleHelperType", "DME.DataBase.DataProvider.Data.Odbc", true);
                        break;
                    case "SQLITE":
                        assembly = config.GetAppConfig("SQLiteHelperAssembly", "DME.DataBase.DataProvider.Data.SQLite", true);
                        type = config.GetAppConfig("SQLiteHelperType", "DME.DataBase.DataProvider.Data.SQLite", true);
                        break;
                    default:
                        assembly = config.GetAppConfig("SqlServerHelperAssembly", "DME.DataBase", true);
                        type = config.GetAppConfig("SqlServerHelperType", "DME.DataBase.DataProvider.Data.SqlServer", true);
                        break;
                }
                dc.Add("assembly", assembly);
                dc.Add("type", type);
                return dc;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string DMEDb_GetConnectionString(string ConnectionStringItem)
        {
            cacheLock.EnterWriteLock();
            try
            {
                config.GetAppSection("DME_Library", "DME_DataBase", true);
                return config.GetAppConfig(ConnectionStringItem, "", true);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        public static void DMEDb_SetDataBaseConnection(string name, string connectionString, string providerName)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();
            cacheLock.EnterWriteLock();
            try
            {
                config.GetConnectionSection("DME_Library", "DME_DataBase_Connection", true);
                config.SetConnectionConfig(name, connectionString, providerName);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// FileCachedConfig
        /// </summary>
        public static Dictionary<string, string> DME_FileCachedConfig
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_FileCached", true);
                    config.GetAppConfig("setting", @"IsCache=true", true);
                    config.GetAppConfig("Default", @"FileCachePath=/Temp/FileCache/;KeyPrefix=;CacheSubDirs=1000", true);
                    return config.GetAllAppConfig();
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }               
            }
        }

        /// <summary>
        /// MemoryCachedConfig
        /// </summary>
        public static Dictionary<string, string> DMEDb_MemoryCachedConfig
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_MemoryCached", true);
                    config.GetAppConfig("setting", @"IsCache=true;IntervalMinutes=1;ScavangeMinutes=60;MaxCount=1000000;MaxSize=100*1024", true);
                    config.GetAppConfig("Default", @"KeyPrefix=", true);
                    return config.GetAllAppConfig();
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// MemCachedConfig
        /// </summary>
        public static Dictionary<string, string> DMEDb_MemCachedConfig
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_MemCached", true);
                    config.GetAppConfig("setting", @"IsCache=true;SendReceiveTimeout=2000;MinPoolSize=5;MaxPoolSize=10", true);
                    config.GetAppConfig("Default", @"Hosts=127.0.0.1:11211;KeyPrefix=Default.001.", true);
                    return config.GetAllAppConfig();
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// RedisCachedConfig
        /// </summary>
        public static Dictionary<string, string> DMEDb_RedisCachedConfig
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_RedisCached", true);
                    config.GetAppConfig("setting", @"IsCache=true;SendTimeout=-1", true);
                    config.GetAppConfig("Default", @"Hosts=127.0.0.1;Port=6379;PassWord=;DefaultDB=0", true);
                    return config.GetAllAppConfig();
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 数据流处理器工厂
        /// </summary>
        public static Dictionary<string, string> DMEDb_StreamHandlerFactoryConfig
        {
            get
            {
                cacheLock.EnterWriteLock();
                try
                {
                    config.GetAppSection("DME_Library", "DME_StreamHandlerFactory", true);
                    return config.GetAllAppConfig();
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }
       
    }
}
