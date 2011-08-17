﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DME.Base.DEncrypt
{
    /// <summary>
    /// RSA加密解密类。
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
    public static class DME_RSA
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
        /// <summary>生成公私钥</summary>
        /// <param name="PrivateKeyPath">私钥文件路径</param>
        /// <param name="PublicKeyPath">公钥文件路径</param>
        public static bool RSAKey(string PrivateKeyPath, string PublicKeyPath)
        {
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                CreatePrivateKeyXML(PrivateKeyPath, provider.ToXmlString(true));
                CreatePublicKeyXML(PublicKeyPath, provider.ToXmlString(false));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>获取公私钥</summary>
        /// <param name="PrivateKey">私钥</param>
        /// <param name="PublicKey">公钥</param>
        public static void GetRSAKey(out string PrivateKey, out string PublicKey)
        {
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                PrivateKey = provider.ToXmlString(true);
                PublicKey = provider.ToXmlString(false);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>对原始数据进行MD5加密</summary>
        /// <param name="m_strSource">待加密数据</param>
        /// <returns>返回机密后的数据</returns>
        public static string GetHash(string m_strSource)
        {
            HashAlgorithm algorithm = HashAlgorithm.Create("MD5");
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            byte[] inArray = algorithm.ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }

        /// <summary>RSA加密</summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="encryptString">MD5加密后的数据</param>
        /// <returns>RSA公钥加密后的数据</returns>
        public static string Encrypt(string xmlPublicKey, string encryptString)
        {
            string str2;
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xmlPublicKey);
                byte[] bytes = new UnicodeEncoding().GetBytes(encryptString);
                str2 = Convert.ToBase64String(provider.Encrypt(bytes, false));
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str2;
        }

        /// <summary>RSA解密</summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="decryptString">待解密的数据</param>
        /// <returns>解密后的结果</returns>
        public static string Decrypt(string xmlPrivateKey, string decryptString)
        {
            string str2;
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xmlPrivateKey);
                byte[] rgb = Convert.FromBase64String(decryptString);
                byte[] buffer2 = provider.Decrypt(rgb, false);
                str2 = new UnicodeEncoding().GetString(buffer2);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str2;
        }

        /// <summary>对MD5加密后的密文进行签名</summary>
        /// <param name="p_strKeyPrivate">私钥</param>
        /// <param name="m_strHashbyteSignature">MD5加密后的密文</param>
        /// <returns>签名后的值</returns>
        public static string SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature)
        {
            byte[] rgbHash = Convert.FromBase64String(m_strHashbyteSignature);
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(p_strKeyPrivate);
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("MD5");
            byte[] inArray = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(inArray);
        }

        /// <summary>签名验证</summary>
        /// <param name="p_strKeyPublic">公钥</param>
        /// <param name="p_strHashbyteDeformatter">待验证的数据</param>
        /// <param name="p_strDeformatterData">签名<</param>
        /// <returns>签名是否符合</returns>
        public static bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {
            try
            {
                byte[] rgbHash = Convert.FromBase64String(p_strHashbyteDeformatter);
                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(p_strKeyPublic);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
                deformatter.SetHashAlgorithm("MD5");
                byte[] rgbSignature = Convert.FromBase64String(p_strDeformatterData);
                if (deformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>创建公钥文件</summary>
        /// <param name="path"></param>
        /// <param name="publickey"></param>
        public static void CreatePublicKeyXML(string path, string publickey)
        {
            try
            {
                FileStream publickeyxml = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(publickeyxml);
                sw.WriteLine(publickey);
                sw.Close();
                publickeyxml.Close();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary> 创建私钥文件</summary>
        /// <param name="path"></param>
        /// <param name="privatekey"></param>
        public static void CreatePrivateKeyXML(string path, string privatekey)
        {
            try
            {
                FileStream privatekeyxml = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(privatekeyxml);
                sw.WriteLine(privatekey);
                sw.Close();
                privatekeyxml.Close();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>读取公钥</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadPublicKey(string path)
        {
            StreamReader reader = new StreamReader(path);
            string publickey = reader.ReadToEnd();
            reader.Close();
            return publickey;
        }

        /// <summary>读取私钥</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadPrivateKey(string path)
        {
            StreamReader reader = new StreamReader(path);
            string privatekey = reader.ReadToEnd();
            reader.Close();
            return privatekey;
        }
        #endregion
    }
}
