using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DME.Base.Helper;

namespace DME.Base.DEncrypt
{
    /// <summary>
    /// 实现Base64加密解密
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
    public static class DME_Base64
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
        /// <summary>Base64加密</summary>
        /// <param name="encode">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string Encode(Encoding encode, string source)
        {
            byte[] bytes = encode.GetBytes(source);
            string Encode = string.Empty;
            try
            {
                Encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                Encode = source;
            }
            return Encode;
        }

        /// <summary>Base64加密，采用utf8编码方式加密</summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string Encode(string source)
        {
            return Encode(Encoding.UTF8, source);
        }

        /// <summary>Base64解密</summary>
        /// <param name="encode">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Decode(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        /// <summary> Base64解密，采用utf8编码方式解密</summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Decode(string result)
        {
            return Decode(Encoding.UTF8, result);
        }

        /// <summary> 判断是否为base64字符串</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }

        /// <summary>将Byte[]转换成Base64编码文本</summary>
        /// <param name="binBuffer">Byte[]</param>
        /// <returns>Base64编码文本</returns>
        public static string BytesToBase64(byte[] binBuffer)
        {
            int base64ArraySize = (int)Math.Ceiling(binBuffer.Length / 3d) * 4;
            char[] charBuffer = new char[base64ArraySize];
            Convert.ToBase64CharArray(binBuffer, 0, binBuffer.Length, charBuffer, 0);
            string s = new string(charBuffer);
            return s;
        }

        /// <summary>将Base64编码文本转换成Byte[]</summary>
        /// <param name="base64">Base64编码文本</param>
        /// <returns>Byte[]</returns>
        public static Byte[] Base64ToBytes(string base64)
        {
            char[] charBuffer = base64.ToCharArray();
            byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
            return bytes;
        }
        #endregion
    }
}
