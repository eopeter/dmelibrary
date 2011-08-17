using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace DME.Base.DEncrypt
{
    /// <summary>
    ///  MD5加密解密类。
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
    public static class DME_MD5
    {
        #region 私有变量
        private const int BITS_TO_A_BYTE = 8;
        private const int BYTES_TO_A_WORD = 4;
        private const int BITS_TO_A_WORD = 32;

        private static int[] m_lOnBits;
        private static int[] m_l2Power;

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
        /// 
        /// </summary>
        /// <param name="SourceStr"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static int GetASC(string SourceStr, Encoding encoding)
        {
            Byte[] BytesofString = encoding.GetBytes(SourceStr.Substring(0, 1));
            if (BytesofString.Length == 1)
            {
                return BytesofString[0];
            }
            else
            {
                int tmpNum = BytesofString[0] * 0x100 + BytesofString[1];
                if (tmpNum > 0x8000) tmpNum -= 0x10000;
                return tmpNum;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lValue"></param>
        /// <param name="iShiftBits"></param>
        /// <returns></returns>
        private static int LShift(int lValue, int iShiftBits)
        {
            if (iShiftBits == 0) return lValue;
            if (iShiftBits == 31)
            {
                if (lValue % 2 == 1)
                    return Convert.ToInt32(0x80000000 - 0x100000000);
                else
                    return 0;
            }
            long tmpLShift;
            if ((lValue & m_l2Power[31 - iShiftBits]) != 0)
            {
                tmpLShift = ((lValue & m_lOnBits[31 - (iShiftBits + 1)]) * m_l2Power[iShiftBits]) | Convert.ToInt32(0x80000000 - 0x100000000);
            }
            else
                tmpLShift = (lValue & m_lOnBits[31 - iShiftBits]) * m_l2Power[iShiftBits];
            tmpLShift %= 0x100000000;
            if (tmpLShift >= 0x80000000) tmpLShift -= 0x100000000;
            return Convert.ToInt32(tmpLShift);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lValue"></param>
        /// <param name="iShiftBits"></param>
        /// <returns></returns>
        private static int RShift(int lValue, int iShiftBits)
        {
            if (iShiftBits == 0) return lValue;
            if (iShiftBits == 31)
            {
                if ((lValue & 0x80000000) != 0)
                    return 1;
                else
                    return 0;
            }
            int tmpRShift = (lValue & 0x7FFFFFFE) / m_l2Power[iShiftBits];
            if ((lValue & 0x80000000) != 0)
            {
                tmpRShift |= (0x40000000 / m_l2Power[iShiftBits - 1]);
            }
            return tmpRShift;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lValue"></param>
        /// <param name="iShiftBits"></param>
        /// <returns></returns>
        private static int RotateLeft(int lValue, int iShiftBits)
        {
            return (LShift(lValue, iShiftBits) | RShift(lValue, (32 - iShiftBits)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lX"></param>
        /// <param name="lY"></param>
        /// <returns></returns>
        private static int AddUnsigned(int lX, int lY)
        {
            long lX4, lY4, lX8, lY8, lResult;

            lX8 = lX & 0x80000000;
            lY8 = lY & 0x80000000;
            lX4 = lX & 0x40000000;
            lY4 = lY & 0x40000000;

            lResult = (lX & 0x3FFFFFFF) + (lY & 0x3FFFFFFF);

            if ((lX4 & lY4) != 0)
            {
                lResult = lResult ^ 0x80000000 ^ lX8 ^ lY8;
            }
            else
            {
                if ((lX4 | lY4) != 0)
                {
                    if ((lResult & 0x40000000) != 0)
                        lResult = lResult ^ 0xC0000000 ^ lX8 ^ lY8;
                    else
                        lResult = lResult ^ 0x40000000 ^ lX8 ^ lY8;
                }
                else
                {
                    lResult = lResult ^ lX8 ^ lY8;
                }

            }

            lResult %= 0x100000000;
            if (lResult >= 0x80000000) lResult -= 0x100000000;
            return Convert.ToInt32(lResult);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static int md5_F(int x, int y, int z)
        {
            return (x & y) | ((x ^ -1) & z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static int md5_G(int x, int y, int z)
        {
            return (x & z) | (y & (z ^ -1));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static int md5_H(int x, int y, int z)
        {
            return (x ^ y ^ z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static int md5_I(int x, int y, int z)
        {
            return (y ^ (x | (z ^ -1)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="x"></param>
        /// <param name="s"></param>
        /// <param name="ac"></param>
        private static void md5_FF(ref int a, int b, int c, int d, int x, int s, int ac)
        {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(md5_F(b, c, d), x), ac));
            a = RotateLeft(a, s);
            a = AddUnsigned(a, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="x"></param>
        /// <param name="s"></param>
        /// <param name="ac"></param>
        private static void md5_GG(ref int a, int b, int c, int d, int x, int s, int ac)
        {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(md5_G(b, c, d), x), ac));
            a = RotateLeft(a, s);
            a = AddUnsigned(a, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="x"></param>
        /// <param name="s"></param>
        /// <param name="ac"></param>
        private static void md5_HH(ref int a, int b, int c, int d, int x, int s, int ac)
        {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(md5_H(b, c, d), x), ac));
            a = RotateLeft(a, s);
            a = AddUnsigned(a, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="x"></param>
        /// <param name="s"></param>
        /// <param name="ac"></param>
        private static void md5_II(ref int a, int b, int c, int d, int x, int s, int ac)
        {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(md5_I(b, c, d), x), ac));
            a = RotateLeft(a, s);
            a = AddUnsigned(a, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static int[] ConvertToWordArray(string sMessage, Encoding encoding)
        {
            int lMessageLength, lNumberOfWords, lBytePosition, lByteCount, lWordCount;
            const int MODULUS_BITS = 512;
            const int CONGRUENT_BITS = 448;

            lMessageLength = sMessage.Length;
            lNumberOfWords = (((lMessageLength + ((MODULUS_BITS - CONGRUENT_BITS) / BITS_TO_A_BYTE)) / (MODULUS_BITS / BITS_TO_A_BYTE)) + 1) * (MODULUS_BITS / BITS_TO_A_WORD);
            int[] lWordArray = new int[lNumberOfWords];
            lBytePosition = 0;
            lByteCount = 0;

            while (lByteCount < lMessageLength)
            {
                lWordCount = lByteCount / BYTES_TO_A_WORD;
                lBytePosition = (lByteCount % BYTES_TO_A_WORD) * BITS_TO_A_BYTE;
                lWordArray[lWordCount] |= LShift(GetASC(sMessage.Substring(lByteCount, 1), encoding), lBytePosition);
                lByteCount++;
            }

            lWordCount = lByteCount / BYTES_TO_A_WORD;
            lBytePosition = (lByteCount % BYTES_TO_A_WORD) * BITS_TO_A_BYTE;
            lWordArray[lWordCount] |= LShift(0x80, lBytePosition);
            lWordArray[lNumberOfWords - 2] = LShift(lMessageLength, 3);
            lWordArray[lNumberOfWords - 1] = RShift(lMessageLength, 29);
            return lWordArray;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lValue"></param>
        /// <returns></returns>
        private static string WordToHex(int lValue)
        {
            string WordToHextmp, tmpStr;
            WordToHextmp = "";
            int lByte;

            for (int lCount = 0; lCount <= 3; lCount++)
            {
                lByte = RShift(lValue, lCount * BITS_TO_A_BYTE) & m_lOnBits[BITS_TO_A_BYTE - 1];
                tmpStr = "0" + Convert.ToString(lByte, 16);
                WordToHextmp += tmpStr.Substring(tmpStr.Length - 2, 2);
            }
            return WordToHextmp;
        } 
        #endregion

        #region 公开函数
        /// <summary>兼容 ASP 版的 MD5 32位 算法</summary>
        /// <param name="sMessage">要加密的字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt_asp(string sMessage, Encoding encoding)
        {
            m_lOnBits = new int[31] { 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 65535, 131071, 262143, 524287, 1048575, 2097151, 4194303, 8388607, 16777215, 33554431, 67108863, 134217727, 268435455, 536870911, 1073741823, 2147483647 };
            m_l2Power = new int[31] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432, 67108864, 134217728, 268435456, 536870912, 1073741824 };

            const int S11 = 7;
            const int S12 = 12;
            const int S13 = 17;
            const int S14 = 22;
            const int S21 = 5;
            const int S22 = 9;
            const int S23 = 14;
            const int S24 = 20;
            const int S31 = 4;
            const int S32 = 11;
            const int S33 = 16;
            const int S34 = 23;
            const int S41 = 6;
            const int S42 = 10;
            const int S43 = 15;
            const int S44 = 21;

            int k, AA, BB, CC, DD, a, b, c, d;

            int[] x = ConvertToWordArray(sMessage, encoding);

            a = 0x67452301;
            b = Convert.ToInt32(0xEFCDAB89 - 0x100000000);
            c = Convert.ToInt32(0x98BADCFE - 0x100000000);
            d = 0x10325476;

            for (k = 0; k < x.Length; k += 16)
            {
                AA = a;
                BB = b;
                CC = c;
                DD = d;

                md5_FF(ref a, b, c, d, x[k + 0], S11, Convert.ToInt32(0xD76AA478 - 0x100000000));
                md5_FF(ref d, a, b, c, x[k + 1], S12, Convert.ToInt32(0xE8C7B756 - 0x100000000));
                md5_FF(ref c, d, a, b, x[k + 2], S13, 0x242070DB);
                md5_FF(ref b, c, d, a, x[k + 3], S14, Convert.ToInt32(0xC1BDCEEE - 0x100000000));
                md5_FF(ref a, b, c, d, x[k + 4], S11, Convert.ToInt32(0xF57C0FAF - 0x100000000));
                md5_FF(ref d, a, b, c, x[k + 5], S12, 0x4787C62A);
                md5_FF(ref c, d, a, b, x[k + 6], S13, Convert.ToInt32(0xA8304613 - 0x100000000));
                md5_FF(ref b, c, d, a, x[k + 7], S14, Convert.ToInt32(0xFD469501 - 0x100000000));
                md5_FF(ref a, b, c, d, x[k + 8], S11, 0x698098D8);
                md5_FF(ref d, a, b, c, x[k + 9], S12, Convert.ToInt32(0x8B44F7AF - 0x100000000));
                md5_FF(ref c, d, a, b, x[k + 10], S13, Convert.ToInt32(0xFFFF5BB1 - 0x100000000));
                md5_FF(ref b, c, d, a, x[k + 11], S14, Convert.ToInt32(0x895CD7BE - 0x100000000));
                md5_FF(ref a, b, c, d, x[k + 12], S11, 0x6B901122);
                md5_FF(ref d, a, b, c, x[k + 13], S12, Convert.ToInt32(0xFD987193 - 0x100000000));
                md5_FF(ref c, d, a, b, x[k + 14], S13, Convert.ToInt32(0xA679438E - 0x100000000));
                md5_FF(ref b, c, d, a, x[k + 15], S14, 0x49B40821);

                md5_GG(ref a, b, c, d, x[k + 1], S21, Convert.ToInt32(0xF61E2562 - 0x100000000));
                md5_GG(ref d, a, b, c, x[k + 6], S22, Convert.ToInt32(0xC040B340 - 0x100000000));
                md5_GG(ref c, d, a, b, x[k + 11], S23, 0x265E5A51);
                md5_GG(ref b, c, d, a, x[k + 0], S24, Convert.ToInt32(0xE9B6C7AA - 0x100000000));
                md5_GG(ref a, b, c, d, x[k + 5], S21, Convert.ToInt32(0xD62F105D - 0x100000000));
                md5_GG(ref d, a, b, c, x[k + 10], S22, 0x2441453);
                md5_GG(ref c, d, a, b, x[k + 15], S23, Convert.ToInt32(0xD8A1E681 - 0x100000000));
                md5_GG(ref b, c, d, a, x[k + 4], S24, Convert.ToInt32(0xE7D3FBC8 - 0x100000000));
                md5_GG(ref a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
                md5_GG(ref d, a, b, c, x[k + 14], S22, Convert.ToInt32(0xC33707D6 - 0x100000000));
                md5_GG(ref c, d, a, b, x[k + 3], S23, Convert.ToInt32(0xF4D50D87 - 0x100000000));
                md5_GG(ref b, c, d, a, x[k + 8], S24, 0x455A14ED);
                md5_GG(ref a, b, c, d, x[k + 13], S21, Convert.ToInt32(0xA9E3E905 - 0x100000000));
                md5_GG(ref d, a, b, c, x[k + 2], S22, Convert.ToInt32(0xFCEFA3F8 - 0x100000000));
                md5_GG(ref c, d, a, b, x[k + 7], S23, 0x676F02D9);
                md5_GG(ref b, c, d, a, x[k + 12], S24, Convert.ToInt32(0x8D2A4C8A - 0x100000000));

                md5_HH(ref a, b, c, d, x[k + 5], S31, Convert.ToInt32(0xFFFA3942 - 0x100000000));
                md5_HH(ref d, a, b, c, x[k + 8], S32, Convert.ToInt32(0x8771F681 - 0x100000000));
                md5_HH(ref c, d, a, b, x[k + 11], S33, 0x6D9D6122);
                md5_HH(ref b, c, d, a, x[k + 14], S34, Convert.ToInt32(0xFDE5380C - 0x100000000));
                md5_HH(ref a, b, c, d, x[k + 1], S31, Convert.ToInt32(0xA4BEEA44 - 0x100000000));
                md5_HH(ref d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
                md5_HH(ref c, d, a, b, x[k + 7], S33, Convert.ToInt32(0xF6BB4B60 - 0x100000000));
                md5_HH(ref b, c, d, a, x[k + 10], S34, Convert.ToInt32(0xBEBFBC70 - 0x100000000));
                md5_HH(ref a, b, c, d, x[k + 13], S31, 0x289B7EC6);
                md5_HH(ref d, a, b, c, x[k + 0], S32, Convert.ToInt32(0xEAA127FA - 0x100000000));
                md5_HH(ref c, d, a, b, x[k + 3], S33, Convert.ToInt32(0xD4EF3085 - 0x100000000));
                md5_HH(ref b, c, d, a, x[k + 6], S34, 0x4881D05);
                md5_HH(ref a, b, c, d, x[k + 9], S31, Convert.ToInt32(0xD9D4D039 - 0x100000000));
                md5_HH(ref d, a, b, c, x[k + 12], S32, Convert.ToInt32(0xE6DB99E5 - 0x100000000));
                md5_HH(ref c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
                md5_HH(ref b, c, d, a, x[k + 2], S34, Convert.ToInt32(0xC4AC5665 - 0x100000000));

                md5_II(ref a, b, c, d, x[k + 0], S41, Convert.ToInt32(0xF4292244 - 0x100000000));
                md5_II(ref d, a, b, c, x[k + 7], S42, 0x432AFF97);
                md5_II(ref c, d, a, b, x[k + 14], S43, Convert.ToInt32(0xAB9423A7 - 0x100000000));
                md5_II(ref b, c, d, a, x[k + 5], S44, Convert.ToInt32(0xFC93A039 - 0x100000000));
                md5_II(ref a, b, c, d, x[k + 12], S41, 0x655B59C3);
                md5_II(ref d, a, b, c, x[k + 3], S42, Convert.ToInt32(0x8F0CCC92 - 0x100000000));
                md5_II(ref c, d, a, b, x[k + 10], S43, Convert.ToInt32(0xFFEFF47D - 0x100000000));
                md5_II(ref b, c, d, a, x[k + 1], S44, Convert.ToInt32(0x85845DD1 - 0x100000000));
                md5_II(ref a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
                md5_II(ref d, a, b, c, x[k + 15], S42, Convert.ToInt32(0xFE2CE6E0 - 0x100000000));
                md5_II(ref c, d, a, b, x[k + 6], S43, Convert.ToInt32(0xA3014314 - 0x100000000));
                md5_II(ref b, c, d, a, x[k + 13], S44, 0x4E0811A1);
                md5_II(ref a, b, c, d, x[k + 4], S41, Convert.ToInt32(0xF7537E82 - 0x100000000));
                md5_II(ref d, a, b, c, x[k + 11], S42, Convert.ToInt32(0xBD3AF235 - 0x100000000));
                md5_II(ref c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
                md5_II(ref b, c, d, a, x[k + 9], S44, Convert.ToInt32(0xEB86D391 - 0x100000000));

                a = AddUnsigned(a, AA);
                b = AddUnsigned(b, BB);
                c = AddUnsigned(c, CC);
                d = AddUnsigned(d, DD);
            }

            return (WordToHex(a) + WordToHex(b) + WordToHex(c) + WordToHex(d));
        }

        /// <summary>兼容 ASP 版的 MD5 32位 算法</summary>
        /// <param name="sMessage">要加密的字符串</param>
        /// <returns></returns>
        public static string Encrypt_asp(string sMessage)
        {
            return Encrypt_asp(sMessage,Encoding.Default);
        }

        /// <summary>MD5 16位加密</summary>
        /// <param name="sMessage">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt16(string sMessage)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(sMessage)), 4, 8);
            t2 = t2.Replace("-", "");
            t2 = t2.ToLower();
            return t2;
        }

        /// <summary>MD5 32位加密</summary>
        /// <param name="sMessage">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt32(string sMessage)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(sMessage));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2")); //就是这里的处理
            }

            return tmp.ToString();
        }

        #endregion
    }
}
