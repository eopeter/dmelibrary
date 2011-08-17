using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.Helper;

namespace DME.Base.Common
{
    /// <summary>
    /// 随机类
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
    public sealed class DME_Random
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

        /// <summary>生成随机数字</summary>
        /// <param name="length">生成长度</param>
        /// <returns>随机数</returns>
        public string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>生成随机数字</summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>随机数</returns>
        public string Number(int Length, bool Sleep)
        {
            if (Sleep)
            {
                System.Threading.Thread.Sleep(3);
            }
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }

        /// <summary>生成随机字母与数字</summary>
        /// <param name="IntStr">生成长度</param>
        /// <returns></returns>
        public string Str(int Length)
        {
            return Str(Length, false);
        }

        /// <summary>生成随机字母与数字</summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public string Str(int Length, bool Sleep)
        {
            if (Sleep)
            {
                System.Threading.Thread.Sleep(3);
            }
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        /// <summary> 生成随机纯字母随机数 </summary>
        /// <param name="IntStr">生成长度</param>
        /// <returns></returns>
        public string Str_char(int Length)
        {
            return Str_char(Length, false);
        }

        /// <summary>生成随机纯字母随机数</summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public string Str_char(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        /// <summary> 获取由数字构成的随机字符串。首位不为“0”。</summary>
        /// <param name="len">字符串长度</param>
        /// <returns>随机字符串</returns>
        public string GetRandomNum(int len)
        {
            if (len <= 0) { return null; }
            StringBuilder sb = new StringBuilder();
            //使用Guid.NewGuid().GetHashCode()作为种子，可以确保Random在极短时间产生的随机数尽可能做到不重复    
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            do { sb.Append(rand.Next(0, 9)); }
            while (sb.Length < len);

            //第一位为 0 就去掉再补一位
            if (sb[0] == '0')
            {
                sb.Remove(0, 1);
                sb.Insert(0, rand.Next(1, 9));
            }

            return sb.ToString();
        }

        /// <summary>指定字串中生成任意位随机数,如果指定为空字串则默认为:"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz </summary>
        /// <param name="arrChar">指定的字串</param>
        /// <param name="len">字符串长度</param>
        /// <returns>指定字串中任意位随机数</returns>
        public string GetRnd(string arrChar, int len)
        {
            if (DME_Validation.IsNull(arrChar))
            {
                arrChar = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            }
            int k = arrChar.Length;
            string strCode = "";
            Random rand = new Random();
            for (int i = 1; i <= len; i++)
            {
                int j = Convert.ToInt32(rand.Next(k));
                strCode += arrChar.Substring(j, 1);
            }
            return strCode;

        }

        #endregion
    }
}
