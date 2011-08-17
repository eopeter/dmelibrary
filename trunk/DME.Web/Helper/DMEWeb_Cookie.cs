using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace DME.Web.Helper
{
    public static class DMEWeb_Cookie
    {
        /// 创建COOKIE对象并赋Value值
        /// <summary>
        /// 创建COOKIE对象并赋Value值，修改COOKIE的Value值也用此方法，因为对COOKIE修改必须重新设Expires
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        public static void SetObj(string strCookieName, int iExpires, string strValue)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            objCookie.Value = HttpUtility.UrlEncode(strValue.Trim(), System.Text.Encoding.GetEncoding(65001));
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// 创建COOKIE对象并赋多个KEY键值
        /// <summary>
        /// 创建COOKIE对象并赋多个KEY键值
        /// 设键/值如下：
        /// NameValueCollection myCol = new NameValueCollection();
        /// myCol.Add("red", "rojo");
        /// myCol.Add("green", "verde");
        /// myCol.Add("blue", "azul");
        /// myCol.Add("red", "rouge");   结果“red:rojo,rouge；green:verde；blue:azul”
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="KeyValue">键/值对集合</param>
        public static void SetObj(string strCookieName, int iExpires, NameValueCollection KeyValue)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            foreach (String key in KeyValue.AllKeys)
            {
                objCookie[key] = HttpUtility.UrlEncode(KeyValue[key].Trim(), System.Text.Encoding.GetEncoding(65001));
            }
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// 创建COOKIE对象并赋Value值
        /// <summary>
        /// 创建COOKIE对象并赋Value值，修改COOKIE的Value值也用此方法，因为对COOKIE修改必须重新设Expires
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="strDomain">作用域</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        public static void SetObj(string strCookieName, int iExpires, string strValue, string strDomain)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            objCookie.Value = HttpUtility.UrlEncode(strValue.Trim(), System.Text.Encoding.GetEncoding(65001));
            objCookie.Domain = strDomain.Trim();
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// 创建COOKIE对象并赋多个KEY键值
        /// <summary>
        /// 创建COOKIE对象并赋多个KEY键值
        /// 设键/值如下：
        /// NameValueCollection myCol = new NameValueCollection();
        /// myCol.Add("red", "rojo");
        /// myCol.Add("green", "verde");
        /// myCol.Add("blue", "azul");
        /// myCol.Add("red", "rouge");   结果“red:rojo,rouge；green:verde；blue:azul”
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="strDomain">作用域</param>
        /// <param name="KeyValue">键/值对集合</param>
        public static void SetObj(string strCookieName, int iExpires, NameValueCollection KeyValue, string strDomain)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            foreach (String key in KeyValue.AllKeys)
            {
                objCookie[key] = HttpUtility.UrlEncode(KeyValue[key].Trim(), System.Text.Encoding.GetEncoding(65001));
            }
            objCookie.Domain = strDomain.Trim();
            if (iExpires > 0)
            {
                if (iExpires == 1)
                {
                    objCookie.Expires = DateTime.MaxValue;
                }
                else
                {
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                }
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// 读取Cookie某个对象的Value值
        /// <summary>
        /// 读取Cookie某个对象的Value值，返回Value值，如果对象本就不存在，则返回字符串"CookieNonexistence"
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <returns>Value值，如果对象本就不存在，则返回空字符串</returns>
        public static string GetValue(string strCookieName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)   
              {
                  return "";
              }   
              else   
              {
                  return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[strCookieName].Value, System.Text.Encoding.GetEncoding(65001));
              }
        }

        /// 读取Cookie某个对象的某个Key键的键值
        /// <summary>
        /// 读取Cookie某个对象的某个Key键的键值，返回Key键值
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <returns>Key键值，如果对象本就不存在，则返回空字符串，如果Key键不存在，则返回空字符串</returns>
        public static string GetValue(string strCookieName,string strKeyName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return "";
            }
            else
            {
                string strObjValue = HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[strCookieName].Value, System.Text.Encoding.GetEncoding(65001));
                string strKeyName2 = strKeyName+"=";
                if (strObjValue.IndexOf(strKeyName2) == -1)
                {
                    return "";
                }
                else
                {
                    return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[strCookieName][strKeyName], System.Text.Encoding.GetEncoding(65001));
                }
            }
        }

        /// 修改某个COOKIE对象某个Key键的键值 或 给某个COOKIE对象添加Key键 都调用本方法
        /// <summary>
        /// 修改某个COOKIE对象某个Key键的键值 或 给某个COOKIE对象添加Key键 都调用本方法，
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="KeyValue">Key键值</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)。注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <returns>如果对象本就不存在，false，如果操作成功返回true。</returns>
        public static bool Edit(string strCookieName, string strKeyName, string KeyValue,int iExpires)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return false;
            }
            else
            {
                HttpCookie objCookie = HttpContext.Current.Request.Cookies[strCookieName];
                objCookie[strKeyName] = HttpUtility.UrlEncode(KeyValue.Trim(), System.Text.Encoding.GetEncoding(65001));
                if (iExpires > 0)
                {
                    if (iExpires == 1)
                    {
                        objCookie.Expires = DateTime.MaxValue;
                    }
                    else
                    {
                        objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                    }
                }
                HttpContext.Current.Response.Cookies.Add(objCookie);
                return true;
            }
        }

        /// 删除COOKIE对象
        /// <summary>
        /// 删除COOKIE对象
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        public static void Del(string strCookieName)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());       
            objCookie.Expires = DateTime.Now.AddYears(-5);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// 删除某个COOKIE对象某个Key子键
        /// <summary>
        /// 删除某个COOKIE对象某个Key子键
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)。注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <returns>如果对象本就不存在，则返回false，如果操作成功返回true。</returns>
        public static bool Del(string strCookieName, string strKeyName, int iExpires)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return false;
            }
            else
            {
                HttpCookie objCookie = HttpContext.Current.Request.Cookies[strCookieName];
                objCookie.Values.Remove(strKeyName);
                if (iExpires > 0)
                {
                    if (iExpires == 1)
                    {
                        objCookie.Expires = DateTime.MaxValue;
                    }
                    else
                    {
                        objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                    }
                }
                HttpContext.Current.Response.Cookies.Add(objCookie);
                return true;
            }
        }
    }
}
