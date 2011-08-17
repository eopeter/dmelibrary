using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.DEncrypt
{
    /// <summary>
    /// Base24加密解密类
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
    public sealed class DME_Base24
    {
        #region 私有变量
        private static DME_Base24 defaultInstance;
        private string map;
        #endregion

        #region 公有变量
        public const string DefaultMap = "BCDFGHJKMPQRTVWXY2346789";
        #endregion

        #region 构造
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DME_Base24()
        {
            this.map = DefaultMap;
        }
        #endregion

        #region 析构
        #endregion

        #region 属性
        /// <summary>
        /// 获取 base 24 编码的默认实现
        /// </summary>
        public static DME_Base24 Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new DME_Base24();
                }
                return defaultInstance;
            }
        }

        /// <summary>
        /// 获取或设置进行 base 24 编码的字符映射表
        /// </summary>
        /// <exception cref="ArgumentException">map不符合规范：长度24不允许重复字符</exception>
        public string Map
        {
            get { return this.map; }
            set
            {
                if (value == null || value.Length != 24)
                {
                    throw new ArgumentException("map必须是长度24的字符串");
                }
                for (byte i = 1; i < 24; i++)
                {
                    for (byte j = 0; j < i; j++)
                    {
                        if (value[i] == value[j])
                        {
                            throw new ArgumentException("map中不能含有重复字符");
                        }
                    }
                }
                this.map = value;
            }
        }
        #endregion

        #region 私有函数
        #endregion

        #region 公开函数
        /// <summary>将 8 位无符号整数数组的值转换为它的等效 System.String 表示形式（使用 base 24 数字编码）。</summary>
        /// <param name="bytes">一个 8 位无符号整数数组。</param>
        /// <returns>bytes 内容的 System.String 表示形式，以基数为 24 的数字表示。</returns>
        /// <exception cref="ArgumentNullException">bytes 为 null。</exception>
        public string GetString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes is null");
            }
            string text = string.Empty;
            for (int i = bytes.Length - 1; i >= 0; i -= 4)
            {
                uint data = 0;
                for (int j = i < 3 ? i : 3; j >= 0; j--)
                {
                    data = (data << 8) + bytes[i - j];
                }

                string t = string.Empty;
                while (data > 0)
                {
                    int d = (int)(data % 24);
                    t = map[d] + t;
                    data = data / 24;
                }
                text = t.PadLeft(7, map[0]) + text;
            }
            return text;
        }

        /// <summary>将指定的 System.String（它将二进制数据编码为 base 24 数字）转换成等效的 8 位无符号整数数组。</summary>
        /// <param name="text">System.String。</param>
        /// <returns>等效于 text 的 8 位无符号整数数组。</returns>
        /// <exception cref="ArgumentNullException">text 为 null。</exception>
        /// <exception cref="FormatException">text 包含非 base 24 字符 或 text 包含不可转换的 base 24 字符序列</exception>
        public byte[] GetBytes(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text is null");
            }
            int len = text.Length / 7 + (text.Length % 7 == 0 ? 0 : 1);
            byte[] bytes = new byte[len * 4];
            int pos = bytes.Length - 1;
            for (int i = text.Length - 1; i >= 0; i -= 7, pos -= 4)
            {
                uint data = 0;
                for (int j = i < 6 ? i : 6; j >= 0; j--)
                {
                    int d = map.IndexOf(text[i - j]);
                    if (d == -1)
                    {
                        throw new FormatException("text 包含非 base 24 字符");
                    }
                    try
                    {
                        data = checked(data * 24 + (uint)d);
                    }
                    catch (OverflowException)
                    {
                        throw new FormatException("text 包含不可转换的 base 24 字符序列");
                    }
                }

                byte[] t = BitConverter.GetBytes(data);
                for (int j = 0; j < 4; j++)
                {
                    bytes[pos - j] = t[j];
                }
            }
            return bytes;
        }
        #endregion
    }
}
