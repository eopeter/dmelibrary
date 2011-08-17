using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace DME.Base.Helper
{
    /// <summary>
    /// 验证类.
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
    public static class DME_Validation
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
        /// <summary>
        /// 判断字符串compare 在 input字符串中出现的次数
        /// * 1、通过“:”来分割字符串看得到的字符串数组长度是否小于等于8
        /// * 2、判断输入的IPV6字符串中是否有“::”。
        /// * 3、如果没有“::”采用 ^([\da-f]{1,4}:){7}[\da-f]{1,4}$ 来判断
        /// * 4、如果有“::” ，判断"::"是否止出现一次
        /// * 5、如果出现一次以上 返回false
        /// * 6、^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="compare">用于比较的字符串</param>
        /// <returns>字符串compare 在 input字符串中出现的次数</returns>
        private static int GetStringCount(string input, string compare)
        {
            int index = input.IndexOf(compare);
            if (index != -1)
            {
                return 1 + GetStringCount(input.Substring(index + compare.Length), compare);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 公开函数
        /// <summary> 检查是否为空（null 或是""） </summary>
        /// <param name="obj">待验证对象</param>
        /// <returns>true or false</returns>
        public static bool IsNull(object obj)
        {
            if (obj == null)
            {
                return true;
            }

            string typeName = obj.GetType().Name;
            switch (typeName)
            {
                case "String[]":
                    string[] list = (string[])obj;
                    if (list.Length == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    string str = obj.ToString().Trim();
                    if (str == null || str == "")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
        }

        /// <summary> 验证是否匹配指定正则表达式</summary>
        /// <param name="str">待验证的字符串</param>
        /// <param name="re">正则表达式</param>
        /// <returns>true or false</returns>
        public static bool IsMatchRegex(string str, Regex re)
        {
            if (IsNull(str.Trim())) { return false; }
            else
            {
                return re.IsMatch(str);
            }
        }

        /// <summary> 验证是否为正整数</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]*$");
        }

        /// <summary> 验证是否为Double</summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object Expression)
        {
            if (Expression != null)
            {
                return Regex.IsMatch(Expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");
            }
            return false;
        }

        /// <summary> 判断对象是否为Int32类型的数字 </summary>
        /// <param name="Expression">待验证对象</param>
        /// <returns>true or false</returns>
        public static bool IsNumeric(object Expression)
        {
            if (Expression != null)
            {
                string str = Expression.ToString();
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>验证输入字符串为数字</summary>
        /// <param name="strln">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsNumber(String str)
        {
            return Regex.IsMatch(str, "^([0]|([1-9]+\\d{0,}?))(.[\\d]+)?$");
        }

        /// <summary>验证是否包含特殊字符，常用于用户名字符串的过滤。</summary>
        /// <param name="name">待验证的字符串</param>
        /// <returns>true or false</returns>
        public static bool IsValidName(string name)
        {
            if (IsNull(name.Trim())) { return false; }
            else
            {
                IEnumerator ie = name.GetEnumerator();
                while (ie.MoveNext())
                {
                    if (@"$!<>?#%@~`&*(){};.:+=\'/|"" 		".IndexOf((char)ie.Current) != -1)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary> 验证邮箱地址的合法性 </summary>
        /// <param name="email">待验证的字符串</param>
        /// <returns>true or false</returns>
        public static bool IsEmail(string email)
        {
            if (IsNull(email.Trim())) { return false; }
            else
            {
                Regex re = new Regex(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$", RegexOptions.IgnoreCase);
                return IsMatchRegex(email, re);
            }
        }

        /// <summary> 验证输入字符串为电话号码</summary>
        /// <param name="P_str_phone">输入字符串</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsPhone(string strln)
        {
            return Regex.IsMatch(strln, @"(^(\d{2,4}[-_－—]?)?\d{3,8}([-_－—]?\d{3,8})?([-_－—]?\d{1,7})?$)|(^0?1[35]\d{9}$)");
            //弱一点的验证：  @"\d{3,4}-\d{7,8}"         
        }

        /// <summary> 验证输入字符串为18位的手机号码</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsMobile(string strln)
        {
            return Regex.IsMatch(strln, @"^1[0123456789]\d{9}$", RegexOptions.IgnoreCase);
        }

        /// <summary> 验证是否是有效传真号码 </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsFax(string strln)
        {
            return Regex.IsMatch(strln, @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        /// <summary> 验证身份证是否有效</summary>
        /// <param name="strln"></param>
        /// <returns></returns>
        public static bool IsIDCard(string strln)
        {
            if (strln.Length == 18)
            {
                bool check = IsIDCard18(strln);
                return check;
            }
            else if (strln.Length == 15)
            {
                bool check = IsIDCard15(strln);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary> 验证输入字符串为18位的身份证号码 </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsIDCard18(string strln)
        {
            long n = 0;
            if (long.TryParse(strln.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(strln.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(strln.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = strln.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = strln.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != strln.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary> 验证输入字符串为15位的身份证号码 </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsIDCard15(string strln)
        {
            long n = 0;
            if (long.TryParse(strln, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(strln.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = strln.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }

        /// <summary> 验证是否为http|https|ftp|rtsp|mms协议的URL地址 </summary>
        /// <param name="url">待验证的字符串</param>
        /// <returns>true or false</returns>
        public static bool IsUrl(string url)
        {
            if (IsNull(url.Trim())) { return false; }
            else
            {
                Regex re = new Regex(@"^(https|http|ftp|rtsp|mms)?://((([0-9a-z_!~*'().&=+$%-]+:)?[0-9a-z_!~*'().&=+$%-]+)@)?(([0-9]{1,3}\.){3}[0-9]{1,3}|[^`~!@#$%\^&*\\(\)=\+_\[\]{}\|;:\.'"",<>/\?]+|([0-9a-z_!~*'()-]+\.)*([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\.[a-z]{2,6})(:[0-9]{1,4})?((/?)|(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$", RegexOptions.IgnoreCase);
                return re.IsMatch(url);
            }
        }

        /// <summary>检查字符串中是否包含指定关键字</summary>
        /// <param name="str">待检查的字符串，为空或null时将返回false;</param>
        /// <param name="badWords">指定关键字，为空或null时将返回false;</param>
        /// <param name="comparisonType">
        /// StringComparison枚举
        /// CurrentCulture 使用区域敏感排序规则和当前区域比较字符串。 
        /// CurrentCultureIgnoreCase 使用区域敏感排序规则、当前区域来比较字符串，同时忽略被比较字符串的大小写。 
        /// InvariantCulture 使用区域敏感排序规则和固定区域比较字符串。 
        /// InvariantCultureIgnoreCase 使用区域敏感排序规则、固定区域来比较字符串，同时忽略被比较字符串的大小写。 
        /// Ordinal 使用序号排序规则比较字符串。 
        /// OrdinalIgnoreCase 使用序号排序规则并忽略被比较字符串的大小写，对字符串进行比较。 
        /// </param>
        /// <returns>
        /// true or false
        /// </returns>
        public static bool IsContainBadWord(string str, string[] badWords, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (badWords == null || badWords.Length < 1) return false;

            str = str.ToLower();
            foreach (string s in badWords)
            {
                if (str.IndexOf(s.ToLower(), 0, comparisonType) > -1) return true;
            }

            return false;
        }

        /// <summary>验证是否只含有汉字</summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsOnllyChinese(string strln)
        {
            return Regex.IsMatch(strln, @"^[\u4e00-\u9fa5]+$");
        }

        /// <summary>验证是否为合法的RGB颜色字符串</summary>
        /// <param name="color">RGB颜色，如：#00ccff | #039 | ffffcc</param>
        /// <returns></returns>
        public static bool IsRGBColor(string color)
        {
            if (IsNull(color.Trim())) { return false; }
            else
            {
                Regex re = new Regex(@"^#?([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$", RegexOptions.IgnoreCase);
                return re.IsMatch(color);
            }
        }

        /// <summary> 判断文件流是否为UTF8字符集</summary>
        /// <param name="sbInputStream">文件流</param>
        /// <returns>判断结果</returns>
        public static bool IsUTF8(FileStream sbInputStream)
        {
            int i;
            byte cOctets;  // octets to go in this UTF-8 encoded character 
            byte chr;
            bool bAllAscii = true;
            long iLen = sbInputStream.Length;

            cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                chr = (byte)sbInputStream.ReadByte();

                if ((chr & 0x80) != 0) bAllAscii = false;

                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        }
                        while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0) return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    cOctets--;
                }
            }

            if (cOctets > 0)
            {
                return false;
            }

            if (bAllAscii)
            {
                return false;
            }

            return true;

        }

        /// <summary> 判断一个字符串是否为字母加数字 Regex("[a-zA-Z0-9]?"</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsWordAndNum(string source)
        {
            Regex regex = new Regex("[a-zA-Z0-9]?");
            return regex.Match(source).Success;
        }

        /// <summary> 验证字符串是否符合IPv4格式 </summary>
        /// <param name="ip">待验证的字符串</param>
        /// <returns>true or false</returns>
        public static bool IsIPv4(string ip)
        {
            if (DME_Validation.IsNull(ip))
            {
                return false;
            }
            else
            {
                Regex re = new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$", RegexOptions.IgnoreCase);
                return re.IsMatch(ip);
            }
        }

        /// <summary>判断输入的字符串是否是合法的IPV6 地址</summary>
        /// <param name="ip">待验证的字符串</param>
        /// <returns>true or false</returns>
        public static bool IsIPv6(string ip)
        {
            string pattern = "";
            string temp = ip;
            string[] strs = temp.Split(':');
            if (strs.Length > 8)
            {
                return false;
            }
            int count = GetStringCount(ip, "::");
            if (count > 1)
            {
                return false;
            }
            else if (count == 0)
            {
                pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";

                Regex regex = new Regex(pattern);
                return regex.IsMatch(ip);
            }
            else
            {
                pattern = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
                Regex regex1 = new Regex(pattern);
                return regex1.IsMatch(ip);
            }

        }

        /// <summary> 判断是否为base64字符串</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }
        #endregion
    }
}
