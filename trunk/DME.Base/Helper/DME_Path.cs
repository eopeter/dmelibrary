using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Helper
{
    /// <summary>
    /// 获取路径
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
    public static class DME_Path
    {
        #region 私有变量
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        #endregion

        #region 私有函数
        #endregion

        #region 公开函数
        /// <summary>获取完全路径</summary>
        /// <param name="srtPath"></param>
        /// <returns></returns>
        public static string GetFullPath(string srtPath)
        {
            return System.IO.Path.GetFullPath(srtPath);
        }

        /// <summary>获取完全路径中的目录</summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string fullPath)
        {
            return System.IO.Path.GetDirectoryName(fullPath);
        }

        /// <summary>获取程序运行目录</summary>
        /// <returns></returns>
        public static string GetRunDirectory()
        {
            string DirectoryName = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            if (DirectoryName.IndexOf(":") == 0)
            {
                return DirectoryName + @"\";
            }
            else
            {
                return DirectoryName + "/";
            }
        }

        /// <summary>获取完全路径中的根目录</summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetPathRoot(string fullPath)
        {
            return System.IO.Path.GetPathRoot(fullPath);
        }

        /// <summary>获取完全路径中的文件名</summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetFileName(string fullPath)
        {
            return System.IO.Path.GetFileName(fullPath);
        }

        /// <summary>获取完全路径中的扩展名</summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetExtension(string fullPath)
        {
            return System.IO.Path.GetExtension(fullPath);
        }

        /// <summary>获取完全路径中的没有扩展名的文件名</summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string fullPath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(fullPath);
        }

        /// <summary></summary>
        /// <param name="srtPath"></param>
        /// <returns></returns>
        public static string MapPath(string srtPath)
        {            
            return GetFullPath(GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) + srtPath.Replace("~",""));
        }
        
        #endregion
    }
}
