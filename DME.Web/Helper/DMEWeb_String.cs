using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using DME.Base.Helper;

namespace DME.Web.Helper
{
    /// <summary>
    /// 字符处理类
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
    public static class DMEWeb_String
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
        /// <summary> 高亮显示 </summary>   
        /// <param name="str">原字符串</param>   
        /// <param name="findstr">查找字符串</param>   
        /// <param name="cssclass">Style</param>   
        /// <returns>string</returns>   
        public static string OutHighlightText(string str, string findstr, string cssclass)
        {
            if (findstr != "")
            {
                string text1 = "<span class=\"" + cssclass + "\">%s</span>";
                str = str.Replace(findstr, text1.Replace("%s", findstr));
            }
            return str;
        }

        /// <summary>
        /// 返回浏览者的IP地址。若无法取得，则返回空字符串。
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ip = HttpContext.Current.Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR");
            if (DME_Validation.IsNull(ip) == true)
            {
                ip = HttpContext.Current.Request.ServerVariables.Get("REMOTE_ADDR");
            }
            if (DME_Validation.IsNull(ip) == true)
            {
                ip = HttpContext.Current.Request.UserHostAddress;
            }
            return ip;
        }
        #endregion
    }
}
