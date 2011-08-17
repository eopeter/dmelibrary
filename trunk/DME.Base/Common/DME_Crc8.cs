using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DME.Base.Common
{
    /// <summary>
    /// 文件Crc8校验类
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
    public class DME_Crc8
    {
        #region 私有变量
        /// <summary>Crc 8 位校验表</summary>
        private byte[] Crc8_Table = new byte[] 
            { 
               0,94,188,226,97,63,221,131,194,156,126,32,163,253,31,65, 
               157,195,33,127,252,162,64,30, 95,1,227,189,62,96,130,220, 
               35,125,159,193,66,28,254,160,225,191,93,3,128,222,60,98, 
               190,224,2,92,223,129,99,61,124,34,192,158,29,67,161,255, 
               70,24,250,164,39,121,155,197,132,218,56,102,229,187,89,7,             
               219,133,103,57,186,228,6,88,25,71,165,251,120,38,196,154, 
               101,59,217,135,4,90,184,230,167,249,27,69,198,152,122,36,                         
               248,166,68,26,153,199,37,123,58,100,134,216,91,5,231,185,             
               140,210,48,110,237,179,81,15,78,16,242,172,47,113,147,205, 
               17,79,173,243,112,46,204,146,211,141,111,49,178,236,14,80, 
               175,241,19,77,206,144,114,44,109,51,209,143,12,82,176,238, 
               50,108,142,208,83,13,239,177,240,174,76,18,145,207,45,115, 
               202,148,118,40,171,245,23,73,8,86,180,234,105,55,213,139, 
               87,9,235,181,54,104,138,212,149,203, 41,119,244,170,72,22, 
               233,183,85,11,136,214,52,106,43,117,151,201,74,20,246,168, 
               116,42,200,150,21,75,169,247,182,232,10,84,215,137,107,53 
            };
        private const int COUNT = 102400;
        private uint crc = 0;
        private string mFile;
        #endregion

        #region 公有变量

        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        /// <summary>返回 Crc8校验结果</summary>
        public uint CrcValue
        {
            get
            {
                return crc;
            }
            set
            {
                crc = value;
            }
        }

        /// <summary>校验的文件路径</summary>
        public string CheckFile
        {
            set
            {
                mFile = value;
            }
        }

        /// <summary>Crc校验前设置校验值</summary> 
        public void Reset()
        {
            crc = 0;
        }
        #endregion

        #region 私有函数
        /// <summary>8 位 Crc 校验 产生校验码 需要被校验码和校验码 </summary>
        /// <param name="Crc"></param>
        /// <param name="OldCrc"></param>
        private void Crc(byte crc, byte OldCrc)
        {
            crc = Crc8_Table[OldCrc ^ crc];
        }

        // 8 位 Crc 校验 产生校验码 只要被校验码 
        private void Crc(int bval)
        {
            crc = Crc8_Table[crc ^ bval];
        }

        // 8 位 Crc 校验 产生校验码 只要被校验的字节数组 
        private void Crc(byte[] buffer)
        {

            Crc(buffer, 0, buffer.Length);
        }

        // 8 位 Crc 校验 产生校验码 要被校验的字节数组、起始结果位置和字节长度 
        private void Crc(byte[] buf, int off, int len)
        {
            if (buf == null)
            {
                throw new ArgumentNullException("buf");
            }

            if (off < 0 || len < 0 || off + len > buf.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (int i = off; i < len; i++)
            {
                Crc(buf[i]);
            }
        }
        #endregion

        #region 公开函数

        public void Crc(byte[] buf, int off, int len, uint crcOld)
        {
            crc = crcOld;
            Crc(buf, off, len);
        }

        //获取校验码
        public static uint CrcGet(string mFile)
        {
            uint crcCode = 0;
            int blockLength = 0;
            FileStream fs = null;
            try
            {
                byte[] data = new byte[COUNT];
                DME_Crc8 crc = new DME_Crc8();
                fs = File.OpenRead(mFile);

                blockLength = fs.Read(data, 0, COUNT);
                while ((blockLength = fs.Read(data, 0, COUNT)) > 0)
                {
                    crc.Crc(data, 0, blockLength, crcCode);
                    crcCode = crc.CrcValue;
                }
                fs.Close();
            }
            catch (Exception)
            {
                throw new CrcGetException();
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return crcCode;
        }
        #endregion
    }

    /// <summary>文件Crc码获取异常类</summary>
    public class CrcGetException : Exception
    {

    }
}
