using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;
using DME.Base.Helper;
using DME.Wimdows.Win32API;

namespace DME.Wimdows.IO
{
    /// <summary>
    /// Ini文件操作类
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
    public class DMEWin_IniFiles
    {
        #region 私有变量
        /// <summary>文件的路径</summary>
        private string m_path;
        #endregion

        #region 公有变量
        /// <summary>一个在INI文件中的段的最大大小。</summary>
        public const int MaxSectionSize = 32767; // 32 KB 
        #endregion

        #region 构造
        public DMEWin_IniFiles(string path)
        {
            m_path = DME_Path.GetFullPath(path);
        }
        #endregion

        #region 析构
        #endregion

        #region 属性
        public string Path
        {
            get
            {
                return m_path;
            }
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="valLength"></param>
        /// <returns></returns>
        private static string[] ConvertNullSeperatedStringToStringArray(IntPtr ptr, int valLength)
        {
            string[] retval;
            if (valLength == 0)
            {
                //Return an empty array. 
                retval = new string[0];
            }
            else
            {
                //Convert the buffer into a string.  Decrease the length 
                //by 1 so that we remove the second null off the end. 
                string buff = Marshal.PtrToStringAuto(ptr, valLength - 1);

                //Parse the buffer into an array of strings by searching for nulls. 
                retval = buff.Split('\0');
            }

            return retval;
        }

        private void WriteValueInternal(string sectionName, string keyName, string value)
        {

            if (!DMEWin_Win32API.WritePrivateProfileString(sectionName, keyName, value, m_path))
            {

                throw new System.ComponentModel.Win32Exception();

            }

        }

        public void WriteValue(string sectionName, string keyName, string value)
        {

            if (sectionName == null)

                throw new ArgumentNullException("sectionName");



            if (keyName == null)

                throw new ArgumentNullException("keyName");



            if (value == null)

                throw new ArgumentNullException("value");



            WriteValueInternal(sectionName, keyName, value);

        }


        #endregion

        #region 公开函数

        public string GetString(string sectionName,string keyName,string defaultValue)
        {
            if (DME_Validation.IsNull(sectionName))
            {

                throw new ArgumentNullException("方法【DME_IniFiles.GetString】参数【sectionName】不能为空。");

            }
            if (keyName == null)
            {

                throw new ArgumentNullException("方法【DME_IniFiles.GetString】参数【keyName】不能为空。");
            }
            StringBuilder retval = new StringBuilder(DMEWin_IniFiles.MaxSectionSize);
            DMEWin_Win32API.GetPrivateProfileString(sectionName, keyName, defaultValue, retval, DMEWin_IniFiles.MaxSectionSize, m_path);
            return retval.ToString();
        }

        public int GetInt16(string sectionName,string keyName,short defaultValue)
        {
            int retval = GetInt32(sectionName, keyName, defaultValue);
            return Convert.ToInt16(retval);
        }

        public int GetInt32(string sectionName,string keyName,int defaultValue)
        {
            if (sectionName == null)
            {
                throw new ArgumentNullException("sectionName");
            }
            if (keyName == null)
            {
                throw new ArgumentNullException("keyName");

            }
            return DMEWin_Win32API.GetPrivateProfileInt(sectionName, keyName, defaultValue, m_path);
        }

        public double GetDouble(string sectionName,string keyName,double defaultValue)
        {
            string retval = GetString(sectionName, keyName, "");
            if (retval == null || retval.Length == 0)
            {
                return defaultValue;
            }
            return Convert.ToDouble(retval, CultureInfo.InvariantCulture);
        }

        public List<KeyValuePair<string, string>> GetSectionValuesAsList(string sectionName)
        {

            List<KeyValuePair<string, string>> retval;

            string[] keyValuePairs;

            string key, value;

            int equalSignPos;
            if (sectionName == null)
            {
                throw new ArgumentNullException("sectionName");

            }

            //Allocate a buffer for the returned section names. 
            IntPtr ptr = Marshal.AllocCoTaskMem(DMEWin_IniFiles.MaxSectionSize);
            try
            {
                //Get the section key/value pairs into the buffer. 
                int len = DMEWin_Win32API.GetPrivateProfileSection(sectionName, ptr, DMEWin_IniFiles.MaxSectionSize, m_path);

                keyValuePairs = ConvertNullSeperatedStringToStringArray(ptr, len);

            }

            finally
            {
                //Free the buffer 
                Marshal.FreeCoTaskMem(ptr);
            }

            //Parse keyValue pairs and add them to the list. 
            retval = new List<KeyValuePair<string, string>>(keyValuePairs.Length);

            for (int i = 0; i < keyValuePairs.Length; ++i)
            {
                //Parse the "key=value" string into its constituent parts 
                equalSignPos = keyValuePairs[i].IndexOf('=');
                key = keyValuePairs[i].Substring(0, equalSignPos);
                value = keyValuePairs[i].Substring(equalSignPos + 1, keyValuePairs[i].Length - equalSignPos - 1);
                retval.Add(new KeyValuePair<string, string>(key, value));
            }

            return retval;
        }

        public Dictionary<string, string> GetSectionValues(string sectionName)
        {
            List<KeyValuePair<string, string>> keyValuePairs;
            Dictionary<string, string> retval;
            keyValuePairs = GetSectionValuesAsList(sectionName);

            //Convert list into a dictionary. 
            retval = new Dictionary<string, string>(keyValuePairs.Count);
            foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
            {
                //Skip any key we have already seen. 
                if (!retval.ContainsKey(keyValuePair.Key))
                {
                    retval.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return retval;
        }

        public string[] GetKeyNames(string sectionName)
        {
            int len;
            string[] retval;
            if (sectionName == null)
            {
                throw new ArgumentNullException("sectionName");

            }

            //Allocate a buffer for the returned section names. 
            IntPtr ptr = Marshal.AllocCoTaskMem(DMEWin_IniFiles.MaxSectionSize);
            try
            {
                //Get the section names into the buffer. 
                len = DMEWin_Win32API.GetPrivateProfileString(sectionName, null, null, ptr, DMEWin_IniFiles.MaxSectionSize, m_path);
                retval = ConvertNullSeperatedStringToStringArray(ptr, len);

            }

            finally
            {
               //Free the buffer 
                Marshal.FreeCoTaskMem(ptr);
            }
            return retval;
        }

        public string[] GetSectionNames()
        {
            string[] retval;
            int len;
            //Allocate a buffer for the returned section names. 
            IntPtr ptr = Marshal.AllocCoTaskMem(DMEWin_IniFiles.MaxSectionSize);
            try
            {
                //Get the section names into the buffer. 
                len = DMEWin_Win32API.GetPrivateProfileSectionNames(ptr, DMEWin_IniFiles.MaxSectionSize, m_path);

                retval = ConvertNullSeperatedStringToStringArray(ptr, len);

            }

            finally
            {

                //Free the buffer 

                Marshal.FreeCoTaskMem(ptr);

            }

            return retval;

        }

        public void WriteValue(string sectionName, string keyName, short value)
        {

            WriteValue(sectionName, keyName, (int)value);

        }

        public void WriteValue(string sectionName, string keyName, int value)
        {

            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));

        }

        public void WriteValue(string sectionName, string keyName, float value)
        {

            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));

        }

        public void WriteValue(string sectionName, string keyName, double value)
        {

            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));

        }

        public void DeleteKey(string sectionName, string keyName)
        {
            if (sectionName == null)
            {
                throw new ArgumentNullException("sectionName");
            }

            if (keyName == null)
            {
                throw new ArgumentNullException("keyName");

            }

            WriteValueInternal(sectionName, keyName, null);

        }

        public void DeleteSection(string sectionName)
        {

            if (sectionName == null)
            {
                throw new ArgumentNullException("sectionName");
            }
            WriteValueInternal(sectionName, null, null);

        }
        #endregion      
    }
}
