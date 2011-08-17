using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace DME.Base.Helper
{
    /// <summary>
    /// 类型转换类
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
    public static class DME_TypeParse
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
        /// <summary>字符转成8位整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>8位整数</returns>
        public static sbyte StringToSByte(string num)
        {
            sbyte _iNum = 0;
            SByte.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成8位无符号整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>8位无符号整数</returns>
        public static byte StringToByte(string num)
        {
            byte _iNum = 0;
            Byte.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary> BytesToSByte</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static sbyte[] BytesToSBytes(byte[] value)
        {
            sbyte[] mySByte = new sbyte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] > 127)
                {
                    mySByte[i] = (sbyte)(value[i] - 256);
                }
                else
                {
                    mySByte[i] = (sbyte)value[i];
                }
            }
            return mySByte;
        }

        /// <summary>字符转成16位整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>16位整数</returns>
        public static short StringToInt16(string num)
        {
            short _iNum = 0;
            Int16.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成16位无符号整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>16位无符号整数</returns>
        public static ushort StringToUInt16(string num)
        {
            ushort _iNum = 0;
            UInt16.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成32位整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>32位整数</returns>
        public static int StringToInt32(string num)
        {
            int _iNum = 0;
            Int32.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成32位无符号整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>32位无符号整数</returns>
        public static uint StringToUInt32(string num)
        {
            uint _iNum = 0;
            UInt32.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成64位整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>64位整数</returns>
        public static long StringToInt64(string num)
        {
            long _iNum = 0;
            Int64.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成64位无符号整数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>64位无符号整数</returns>
        public static ulong StringToUInt64(string num)
        {
            ulong _iNum = 0;
            UInt64.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成十进制数,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>十进制数</returns>
        public static decimal StringToDecimal(string num)
        {
            decimal _iNum = 0;
            Decimal.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成单精度 32 位数字,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>单精度 32 位数字</returns>
        public static Single StringToSingle(string num)
        {
            Single _iNum = 0;
            Single.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>字符转成单双精度 64 位数字,如果转换异常则返回整数0</summary>
        /// <param name="num">字符数字</param>
        /// <returns>双精度 64 位数字</returns>
        public static double StringToDouble(string num)
        {
            double _iNum = 0;
            Double.TryParse(num, out _iNum);
            return _iNum;
        }

        /// <summary>将字符[true|false]转成Boolean,如果转换异常则返回整数false</summary>
        /// <param name="str">字符(true|false)</param>
        /// <returns>true|false</returns>
        public static bool StringToBoolean(string str)
        {
            bool _Str = false;
            Boolean.TryParse(str, out _Str);
            return _Str;
        }

        /// <summary>将字符转成DateTime,如果转换异常则返回 今天</summary>
        /// <param name="str">字符</param>
        /// <returns>DateTime</returns>
        public static DateTime StringToDateTime(string str)
        {
            DateTime _Str;
            if (DateTime.TryParse(str, out _Str))
            {
                return _Str;
            }
            else
            {
                return DateTime.Today;
            }
        }

        /// <summary>将字符转成Char,如果转换异常则返回空</summary>
        /// <param name="str">字符</param>
        /// <returns>Char</returns>
        public static char StringToChar(string str)
        {
            char _Str;
            Char.TryParse(str, out _Str);
            return _Str;
        }

        /// <summary>正整数转换字节数组</summary>
        /// <param name="i">正整数</param>
        /// <returns>字节数组</returns>
        public static byte[] UintToBytes(uint i)
        {
            return System.BitConverter.GetBytes(i);
        }

        /// <summary>字节数组转换整数</summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始位置</param>
        /// <returns>正整数</returns>
        public static uint BytesToUint(byte[] bytes,int index)
        {
            return System.BitConverter.ToUInt32(bytes, index);
        }

        /// <summary>字节数组转换字符</summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始位置</param>
        /// <returns>字符</returns>
        public static char BytesToChar(byte[] bytes,int index)
        {
            return System.BitConverter.ToChar(bytes, index);
        }

        /// <summary>字符转换字节数组</summary>
        /// <param name="Char">字符</param>
        /// <returns>字节数组</returns>
        public static byte[] CharToBytes(char Char)
        {
            return System.BitConverter.GetBytes(Char);
        }

        /// <summary>字节数组转换字符数组</summary>
        /// <param name="encoding">编码 如：Encoding.UTF8</param>
        /// <param name="bytes">字节数组</param>
        /// <returns>字符数组</returns>
        public static char[] BytesToChars(Encoding encoding,byte[] bytes)
        {
            return encoding.GetChars(bytes);
           
        }

        /// <summary>字节数组转换字符数组</summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>字符数组</returns>
        public static char[] BytesToChars(byte[] bytes)
        {
            return Encoding.Default.GetChars(bytes);

        }

        /// <summary>字符数组转换字节数组</summary>
        /// <param name="encoding">字符编码</param>
        /// <param name="chars">字符数组</param>
        /// <returns>字节数组</returns>
        public static byte[] CharsToBytes(Encoding encoding, char[] chars)
        {
            return encoding.GetBytes(chars);
        }

        /// <summary>字符数组转换字节数组</summary>
        /// <param name="chars">字符数组</param>
        /// <returns>字节数组</returns>
        public static byte[] CharsToBytes(char[] chars)
        {
            return Encoding.Default.GetBytes(chars);
        }

        /// <summary>字节数组转换字符串</summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>字符串</returns>
        public static string BytesToString(byte[] bytes)
        {
            char[] chars = BytesToChars(bytes);
            return new string(chars);

        }

        /// <summary>字节数组转换字符串</summary>
        /// <param name="encoding">字符编码</param>
        /// <param name="bytes">字节数组</param>
        /// <returns>字符串</returns>
        public static string BytesToString(Encoding encoding, byte[] bytes)
        {
            char[] chars = BytesToChars(encoding, bytes);
            return new string(chars);

        }

        /// <summary>字符串转换字节数组</summary>
        /// <param name="chars">字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] StringToBytes(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        /// <summary>字符串转换字节数组</summary>
        /// <param name="encoding">编码 如：Encoding.UTF8</param>
        /// <param name="str">字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] StringToBytes(Encoding encoding, string str)
        {
            return encoding.GetBytes(str);
        }
      
        /// <summary>字符串转换字符数组</summary>
        /// <param name="str">字符串</param>
        /// <returns>字符数组<</returns>
        public static char[] StringToChars(string str)
        {
            return str.ToCharArray();
        }

        /// <summary>字符串转换正整数</summary>
        /// <param name="str">字符串</param>
        /// <returns>正整数<</returns>
        public static uint StringToUint(string str)
        {
            byte[] bytes = StringToBytes(str);
            return BytesToUint(bytes,0);
        }

        /// <summary>正整数转换字符串</summary>
        /// <param name="i">正整数</param>
        /// <returns>字符串<</returns>
        public static string UintToString(uint i)
        {
            byte[] bytes = UintToBytes(i);
            char[] chars = BytesToChars(bytes);
            return new string(chars);
            
        }

        /// <summary>正整数转换字符数组</summary>
        /// <param name="i">正整数</param>
        /// <returns>字符数组</returns>
        public static char[] UintToChars(uint i)
        {
            byte[] bytes = UintToBytes(i);
            return BytesToChars(bytes);
        }

        /// <summary>字符数组转换整数</summary>
        /// <param name="chars">字符数组</param>
        /// <returns>正整数</returns>
        public static uint CharsToUint(char[] chars)
        {
            byte[] bytes = CharsToBytes(chars);
            return BytesToUint(bytes, 0);
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

        /// <summary>由16进制转为汉字字符串（如：B2E2 -> 测 ）</summary>
        /// <param name="source"></param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string HexToString(string source, Encoding encoding)
        {
            byte[] oribyte = new byte[source.Length / 2];
            string pattern = "[^a-fA-F0-9]";
            Regex regex = new Regex(pattern);
            if (!regex.Match(source).Success)
            {
                for (int i = 0; i < source.Length; i += 2)
                {
                    string str = Convert.ToInt32(source.Substring(i, 2), 16).ToString();
                    oribyte[i / 2] = Convert.ToByte(source.Substring(i, 2), 16);
                }
                return encoding.GetString(oribyte);
            }
            return null;
        }

        ///<summary>字符串转为16进制字符串（如：测 -> B2E2 ）</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StringToHex(string source, Encoding encoding)
        {
            int i = source.Length;
            string temp;
            string end = "";
            byte[] array = new byte[2];
            int i1, i2;
            for (int j = 0; j < i; j++)
            {
                temp = source.Substring(j, 1);
                array = encoding.GetBytes(temp);
                if (array.Length.ToString() == "1")
                {
                    i1 = Convert.ToInt32(array[0]);
                    end += Convert.ToString(i1, 16);
                }
                else
                {
                    i1 = Convert.ToInt32(array[0]);
                    i2 = Convert.ToInt32(array[1]);
                    end += Convert.ToString(i1, 16);
                    end += Convert.ToString(i2, 16);
                }
            }
            return end.ToUpper();
        }

        /// <summary>字节数组转换为16进制表示的字符串</summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "");
        }

        /// <summary>16进制表示的字符串转换为字节数组/summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(String src)
        {
            int l = src.Length / 2;
            String str;
            byte[] ret = new byte[l];

            for (int i = 0; i < l; i++)
            {
                str = src.Substring(i * 2, 2);
                ret[i] = Convert.ToByte(str, 16);
            }
            return ret;
        }

        /// <summary> 将int型表示的IP还原成正常IPv4格式。</summary>
        /// <param name="intIPAddress">int型表示的IP</param>
        /// <returns></returns>
        public static string NumberToIP(int intIPAddress)
        {
            int tempIPAddress;
            //将目标整形数字intIPAddress转换为IP地址字符串?
            //-1062731518?192.168.1.2 
            //-1062731517?192.168.1.3 
            if (intIPAddress >= 0)
            {
                tempIPAddress = intIPAddress;
            }
            else
            {
                tempIPAddress = intIPAddress + 1;
            }
            int s1 = tempIPAddress / 256 / 256 / 256;
            int s21 = s1 * 256 * 256 * 256;
            int s2 = (tempIPAddress - s21) / 256 / 256;
            int s31 = s2 * 256 * 256 + s21;
            int s3 = (tempIPAddress - s31) / 256;
            int s4 = tempIPAddress - s3 * 256 - s31;
            if (intIPAddress < 0)
            {
                s1 = 255 + s1;
                s2 = 255 + s2;
                s3 = 255 + s3;
                s4 = 255 + s4;
            }
            string strIPAddress = s1.ToString() + "." + s2.ToString() + "." + s3.ToString() + "." + s4.ToString();
            return strIPAddress;
        }

        /// <summary>将IPv4格式的字符串转换为int型表示</summary>
        /// <param name="strIPAddress">IPv4格式的字符</param>
        /// <returns></returns>
        public static int IPToNumber(string strIPAddress)
        {
            //将目标IP地址字符串strIPAddress转换为数字 
            string[] arrayIP = strIPAddress.Split('.');
            int sip1 = Int32.Parse(arrayIP[0]);
            int sip2 = Int32.Parse(arrayIP[1]);
            int sip3 = Int32.Parse(arrayIP[2]);
            int sip4 = Int32.Parse(arrayIP[3]);
            int tmpIpNumber;
            tmpIpNumber = sip1 * 256 * 256 * 256 + sip2 * 256 * 256 + sip3 * 256 + sip4;
            return tmpIpNumber;
        }

        /// <summary>获取class的声明名称</summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetClassSimpleName(Type t)
        {
            string[] parts = t.ToString().Split('.');
            return parts[parts.Length - 1].ToString();
        }

        /// <summary>获取类型的完全名称，如"DME.Base.Helper.DME_TypeParse,DME.Base"</summary>
        /// <param name="destType"></param>
        /// <returns></returns> 
        public static string GetTypeRegularName(Type destType)
        {
            string assName = destType.Assembly.FullName.Split(',')[0];

            return string.Format("{0},{1}", destType.ToString(), assName);

        }

        /// <summary>获取类型的完全名称</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTypeRegularName(object obj)
        {
            Type destType = obj.GetType();
            return GetTypeRegularName(destType);
        }

        /// <summary 通过类型的完全名称获取类型，regularName如"DME.Base.Helper.DME_TypeParse,DME.Base"</summary>
        /// <param name="regularName"></param>
        /// <returns></returns>  
        public static Type GetTypeByRegularName(string regularName)
        {
            return DME.Base.Reflection.DME_ReflectionHelper.GetType(regularName);
        } 

        /// <summary>ChangeType 对System.Convert.ChangeType进行了增强，支持(0,1)到bool的转换，字符串->枚举、int->枚举、字符串->Type</summary>
        /// <param name="targetType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static object ChangeType(Type targetType, object val)
        {
            #region null
            if (val == null)
            {
                return null;
            }
            #endregion

            #region Same Type
            if (targetType == val.GetType())
            {
                return val;
            }
            #endregion

            #region bool 1,0
            if (targetType == typeof(bool))
            {
                if (val.ToString() == "0")
                {
                    return false;
                }

                if (val.ToString() == "1")
                {
                    return true;
                }
            }
            #endregion

            #region Enum
            if (targetType.IsEnum)
            {
                int intVal = 0;
                bool suc = int.TryParse(val.ToString(), out intVal);
                if (!suc)
                {
                    return Enum.Parse(targetType, val.ToString());
                }
                else
                {
                    return val;
                }
            }
            #endregion

            #region Type
            if (targetType == typeof(Type))
            {
                return DME.Base.Reflection.DME_ReflectionHelper.GetType(val.ToString());
            }
            #endregion

            //将double赋值给数值型的DataRow的字段是可以的，但是通过反射赋值给object的非double的其它数值类型的属性，却不行        
            return System.Convert.ChangeType(val, targetType);

        }

        /// <summary> 将 Stream 转成 byte[] </summary> 
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary> 将 byte[] 转成 Stream </summary> 
        public static Stream BytesToStream(byte[] bytes) 
        { 
            Stream stream = new MemoryStream(bytes); 
            return stream; 
        } 
        #endregion

    }
}
