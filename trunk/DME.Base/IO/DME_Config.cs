using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Linq;
using DME.Base.Helper;
using System.Threading;
namespace DME.Base.IO
{
    /// <summary>
    /// config文件操作类
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
    public class DME_Config : DME_DisposeBase
    {
        #region 私有变量

        private ExeConfigurationFileMap fileMap = null;
        private Configuration config = null;
        private AppSettingsSection AppSettings = null;
        private ConnectionStringsSection ConnectionStrings = null;
        private  ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        /// <summary></summary>
        public DME_Config()
        {
            
            if (!DME_String.StringCompare(DME_Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile), "Web.config", false))
            {
                Initialization(DME_Path.MapPath(@"\app.config"));
            }
            else
            {
                Initialization(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            }            
        }

        /// <summary></summary>
        /// <param name="ConfigPath">相对路径</param>
        public DME_Config(string ConfigPath)
        {
            Initialization(DME_Path.MapPath(ConfigPath));
        }
        #endregion

        #region 析构
        #endregion

        #region 属性
        #endregion

        #region 私有函数
        private void Initialization(string ConfigPath)
        {
            fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = ConfigPath;
            config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            AppSettings = (AppSettingsSection)config.GetSection("appSettings");
            ConnectionStrings = (ConnectionStringsSection)config.GetSection("connectionStrings");  
            
                
        }

        /// <summary>判断key是否存在</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsAppOnKey(string key)
        {
            if (AppSettings.Settings.AllKeys.Contains(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>判断name是否存在</summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsConnectionOnKey(string name)
        {
            if (ConnectionStrings.ConnectionStrings[name] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 公开函数
        #region AppSettings节点
        /// <summary>GetSection 用于改更AppSettings节点对像</summary>
        /// <param name="GroupsName"></param>
        /// <param name="SectionsName"></param>
        /// <param name="Create">如果节点不存在是否创建</param>
        public void GetAppSection(string GroupsName, string SectionsName, bool Create)
        {
            if (DME_Validation.IsNull(SectionsName))
            {
                return;
            }

            if (DME_Validation.IsNull(GroupsName))
            {
                if (!DME_Validation.IsNull(config.GetSection(SectionsName)))
                {
                    AppSettings = (AppSettingsSection)config.GetSection(SectionsName);
                }
                else
                {
                    if (Create)
                    {
                        AppSettings = new AppSettingsSection();
                        config.Sections.Add(SectionsName, AppSettings);
                    }
                }
            }
            else
            {
                if (!DME_Validation.IsNull(config.GetSection(string.Format(@"{0}/{1}", GroupsName, SectionsName))))
                {
                    AppSettings = (AppSettingsSection)config.GetSection(string.Format(@"{0}/{1}", GroupsName, SectionsName));
                }
                else
                {
                    if (Create)
                    {
                        AppSettings = new AppSettingsSection();
                        if (config.GetSectionGroup(GroupsName) != null)
                        {
                            config.SectionGroups[GroupsName].Sections.Add(SectionsName, AppSettings);
                        }
                        else
                        {
                            config.SectionGroups.Add(GroupsName, new ConfigurationSectionGroup());
                            config.SectionGroups[GroupsName].Sections.Add(SectionsName, AppSettings);
                        }

                    }
                }
            }

        }

        /// <summary>根据AppSettings键值获取配置文件</summary>
        /// <param name="key">键值</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="Create">如果节点不存在是否创建</param>
        /// <returns></returns>
        public string GetAppConfig(string key, string defaultValue, bool Create)
        {
            string val = defaultValue;
            if (IsAppOnKey(key))
            {
                
                 val = AppSettings.Settings[key].Value;
            }
            else
            {
                if (Create)
                {
                    AppSettings.Settings.Add(key, val);
                    this.Save();
                }
            }
            return val;
        }

        /// <summary>获取所有配置文件</summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllAppConfig()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string key in AppSettings.Settings.AllKeys)
            {
                dict.Add(key, AppSettings.Settings[key].Value);
            }
            return dict;
        }

        /// <summary>写配置文件,如果节点不存在则自动创建</summary>
        /// <param name="key">键值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetAppConfig(string key, string value)
        {
            try
            {
                if (!IsAppOnKey(key))
                {
                    AppSettings.Settings.Add(key, value);
                }
                else
                {
                    AppSettings.Settings[key].Value = value;
                }
                this.Save();
                return true;
            }
            catch { return false; }
        }

        /// <summary>写配置文件(用键值创建),如果节点不存在则自动创建</summary>
        /// <param name="dict">键值集合</param>
        /// <returns></returns>
        public bool SetAppConfig(Dictionary<string, string> dict)
        {
            try
            {
                if (dict == null || dict.Count == 0)
                {
                    return false;
                }
                foreach (string key in dict.Keys)
                {
                    if (!IsAppOnKey(key))
                    {
                        AppSettings.Settings.Add(key, dict[key]);
                    }
                    else
                    {
                        AppSettings.Settings[key].Value = dict[key];
                    }
                }
                this.Save();
                return true;
            }
            catch { return false; }
        }

        /// <summary>删除appSettings节点</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveAppSetting(string key)
        {
            if (AppSettings.Settings[key] != null)
            {
                AppSettings.Settings.Remove(key);
                this.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ConnectionString节点

        /// <summary>GetConnectionSection 用于改更ConnectionStrings节点对像</summary>
        /// <param name="GroupsName"></param>
        /// <param name="SectionsName"></param>
        /// <param name="Create">如果节点不存在是否创建</param>
        public void GetConnectionSection(string GroupsName, string SectionsName, bool Create)
        {
            if (DME_Validation.IsNull(SectionsName))
            {
                return;
            }
            if (DME_Validation.IsNull(GroupsName))
            {
                if (!DME_Validation.IsNull(config.GetSection(SectionsName)))
                {
                    ConnectionStrings = (ConnectionStringsSection)config.GetSection(SectionsName);
                }
                else
                {
                    if (Create)
                    {
                        ConnectionStrings = new ConnectionStringsSection();
                        config.Sections.Add(SectionsName, ConnectionStrings);
                    }
                }
            }
            else
            {
                if (!DME_Validation.IsNull(config.GetSection(string.Format(@"{0}/{1}", GroupsName, SectionsName))))
                {
                    ConnectionStrings = (ConnectionStringsSection)config.GetSection(string.Format(@"{0}/{1}", GroupsName, SectionsName));
                }
                else
                {
                    if (Create)
                    {
                        ConnectionStrings = new ConnectionStringsSection();
                        if (config.GetSectionGroup(GroupsName) != null)
                        {
                            config.SectionGroups[GroupsName].Sections.Add(SectionsName, ConnectionStrings);
                        }
                        else
                        {
                            
                            config.SectionGroups.Add(GroupsName, new ConfigurationSectionGroup());
                            config.SectionGroups[GroupsName].Sections.Add(SectionsName, ConnectionStrings);
                        }
                    }
                }
            }
        }

        /// <summary>根据ConnectionStrings键值获取配置文件</summary>
        /// <param name="name">键值</param>
        /// <param name="default_connectionString">默认值</param>
        /// <param name="default_providerName">默认值</param>
        /// <param name="Create">如果节点不存在是否创建</param>
        /// <returns></returns>
        public void GetConnectionConfig(string name, ref string default_connectionString, ref string default_providerName, bool Create)
        {
            if (IsConnectionOnKey(name))
            {
                default_connectionString = ConnectionStrings.ConnectionStrings[name].ConnectionString;
                default_providerName = ConnectionStrings.ConnectionStrings[name].ProviderName;
            }
            else
            {
                if (Create)
                {
                    ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(name, default_connectionString, default_providerName));
                    this.Save();
                }
            }
        }

        /// <summary>获取所有配置文件</summary>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> GetConnectionConfig()
        {
            Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> dct = new Dictionary<string, string>();
            ConnectionStringSettingsCollection conCollection = ConnectionStrings.ConnectionStrings;
            foreach (ConnectionStringSettings conSetting in conCollection)
            {
                dct.Add("name", conSetting.Name);
                dct.Add("connectionString", conSetting.ConnectionString);
                dct.Add("providerName", conSetting.ProviderName);
                dict.Add(conSetting.Name, dct);
                dct.Clear();
            }
            return dict;
        }

        /// <summary>写配置文件,如果节点不存在则自动创建</summary>
        /// <param name="name">键值</param>
        /// <param name="connectionString">值</param>
        /// <param name="providerName">值</param>
        /// <returns></returns>
        public bool SetConnectionConfig(string name, string connectionString, string providerName)
        {
            try
            {
                if (!IsConnectionOnKey(name))
                {
                    ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString, providerName));
                }
                else
                {
                    ConnectionStrings.ConnectionStrings[name].ConnectionString = connectionString;
                    ConnectionStrings.ConnectionStrings[name].ProviderName = providerName;
                }
                this.Save();
                return true;
            }
            catch { return false; }
        }

        /// <summary>写配置文件,如果节点不存在则自动创建</summary>
        /// <param name="name">键值</param>
        /// <param name="connectionString">值</param>
        /// <param name="providerName">值</param>
        /// <returns></returns>
        public bool SetConnectionConfig(Dictionary<string, Dictionary<string, string>> dict)
        {
            try
            {
                if (dict == null || dict.Count == 0)
                {
                    return false;
                }
                foreach (string key in dict.Keys)
                {
                    if (!IsConnectionOnKey(key))
                    {
                        ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(dict[key]["name"], dict[key]["connectionString"], dict[key]["providerName"]));                       
                    }
                    else
                    {
                        ConnectionStrings.ConnectionStrings[dict[key]["name"]].ConnectionString = dict[key]["connectionString"];
                        ConnectionStrings.ConnectionStrings[dict[key]["name"]].ProviderName = dict[key]["providerName"];
                    }
                }
                this.Save();
                return true;
            }
            catch { return false; }
        }

        /// <summary>删除ConnectionStrings节点</summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveConnectionConfig(string name)
        {
            if (ConnectionStrings.ConnectionStrings[name] != null)
            {
                ConnectionStrings.ConnectionStrings.Remove(name);
                this.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            cacheLock.EnterWriteLock();
            try
            {
                config.Save();
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 子类重载实现资源释放逻辑
        /// </summary>
        /// <param name="disposing">从Dispose调用（释放所有资源）还是析构函数调用（释放非托管资源）</param>
        protected override void OnDispose(Boolean disposing)
        {
            base.OnDispose(disposing);
            if (disposing)
            {
                // 释放托管资源
                fileMap = null;
                AppSettings = null;
                ConnectionStrings = null;
                config = null;                
            }
            cacheLock.Dispose();
            // 释放非托管资源
        }
        #endregion



    }
}
