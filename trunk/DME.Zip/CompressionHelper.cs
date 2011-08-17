﻿using System;
using System.Text;
using System.IO;
using DME.Zip.BZip2;
using DME.Zip.GZip;
using DME.Zip.Zip;

namespace DME.Zip
{
    /// <summary>
    /// 使用 SharpZipLib 进行压缩的辅助类，简化压缩字节数组和字符串的操作。
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


    public enum CompressionType
    {
        //// <summary>
        /// GZip 压缩格式
        /// </summary>
        GZip,
        //// <summary>
        /// BZip2 压缩格式
        /// </summary>
        BZip2,
        //// <summary>
        /// Zip 压缩格式
        /// </summary>
        Zip
    }

    public class CompressionHelper
    {

        #region 成员变量
        /// <summary>
        /// 压缩供应者，默认为 GZip。
        /// </summary>
        public static CompressionType CompressionProvider = CompressionType.GZip;
        #endregion

        #region Public methods

        /// <summary>
        /// 从原始字节数组生成已压缩的字节数组。
        /// </summary>
        /// <param name="bytesToCompress">原始字节数组。</param>
        /// <returns>返回已压缩的字节数组</returns>
        public static byte[] Compress(byte[] bytesToCompress)
        {
            MemoryStream ms = new MemoryStream();
            Stream s = OutputStream(ms);
            s.Write(bytesToCompress, 0, bytesToCompress.Length);
            s.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 从原始字符串生成已压缩的字符串。
        /// </summary>
        /// <param name="stringToCompress">原始字符串。</param>
        /// <returns>返回已压缩的字符串。</returns>
        public static string Compress(string stringToCompress)
        {
            byte[] compressedData = CompressToByte(stringToCompress);
            string strOut = Convert.ToBase64String(compressedData);
            return strOut;
        }

        /// <summary>
        /// 从原始字符串生成已压缩的字节数组。
        /// </summary>
        /// <param name="stringToCompress">原始字符串。</param>
        /// <returns>返回已压缩的字节数组。</returns>
        public static byte[] CompressToByte(string stringToCompress)
        {
            byte[] bytData = Encoding.Unicode.GetBytes(stringToCompress);
            return Compress(bytData);
        }

        /// <summary>
        /// 从已压缩的字符串生成原始字符串。
        /// </summary>
        /// <param name="stringToDecompress">已压缩的字符串。</param>
        /// <returns>返回原始字符串。</returns>
        public string DeCompress(string stringToDecompress)
        {
            string outString = string.Empty;
            if (stringToDecompress == null)
            {
                throw new ArgumentNullException("stringToDecompress", "You tried to use an empty string");
            }

            try
            {
                byte[] inArr = Convert.FromBase64String(stringToDecompress.Trim());
                outString = Encoding.Unicode.GetString(DeCompress(inArr));
            }
            catch (NullReferenceException nEx)
            {
                return nEx.Message;
            }

            return outString;
        }

        /// <summary>
        /// 从已压缩的字节数组生成原始字节数组。
        /// </summary>
        /// <param name="bytesToDecompress">已压缩的字节数组。</param>
        /// <returns>返回原始字节数组。</returns>
        public static byte[] DeCompress(byte[] bytesToDecompress)
        {
            byte[] writeData = new byte[4096];
            Stream s2 = InputStream(new MemoryStream(bytesToDecompress));
            MemoryStream outStream = new MemoryStream();

            while (true)
            {
                int size = s2.Read(writeData, 0, writeData.Length);
                if (size > 0)
                {
                    outStream.Write(writeData, 0, size);
                }
                else
                {
                    break;
                }
            }
            s2.Close();
            byte[] outArr = outStream.ToArray();
            outStream.Close();
            return outArr;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 从给定的流生成压缩输出流。
        /// </summary>
        /// <param name="inputStream">原始流。</param>
        /// <returns>返回压缩输出流。</returns>
        private static Stream OutputStream(Stream inputStream)
        {
            switch (CompressionProvider)
            {
                case CompressionType.BZip2:
                    return new BZip2OutputStream(inputStream);

                case CompressionType.GZip:
                    return new GZipOutputStream(inputStream);

                case CompressionType.Zip:
                    return new ZipOutputStream(inputStream);

                default:
                    return new GZipOutputStream(inputStream);
            }
        }

        /// <summary>
        /// 从给定的流生成压缩输入流。
        /// </summary>
        /// <param name="inputStream">原始流。</param>
        /// <returns>返回压缩输入流。</returns>
        private static Stream InputStream(Stream inputStream)
        {
            switch (CompressionProvider)
            {
                case CompressionType.BZip2:
                    return new BZip2InputStream(inputStream);

                case CompressionType.GZip:
                    return new GZipInputStream(inputStream);

                case CompressionType.Zip:
                    return new ZipInputStream(inputStream);

                default:
                    return new GZipInputStream(inputStream);
            }
        }

        #endregion
    }
}
