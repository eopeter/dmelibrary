using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DME.Base.Cache.FileCached
{
    /// <summary>
    /// 二进制文件缓存
    /// </summary>
    public class DME_FileCache
    {
        private string _cachePath;
        private bool _isCache = true;
        private string _keyPrefix = "";
        private int _cacheSubDirs = 1000;

        public static object lockObj = new object();

        #region 构造函数
        public DME_FileCache(string path)
        {
            _cachePath = path;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 文件路径
        /// </summary>
        public string CachePath
        {
            get
            {
                return _cachePath;
            }
            set
            {
                _cachePath = value;
            }
        }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache
        {
            set
            {
                _isCache = value;
            }
            get
            {
                return _isCache;
            }
        }
        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeyPrefix
        {
            set
            {
                _keyPrefix = value;
            }
            get
            {
                return _keyPrefix;
            }
        }
        /// <summary>
        /// 最大文件目录数,0为不分子目录
        /// </summary>
        public int CacheSubDirs
        {
            get
            {
                return _cacheSubDirs;
            }
            set
            {
                _cacheSubDirs = value;
            }
        }
        #endregion

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string key, object value)
        {
            return Set(key,value,0);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="extime"></param>
        /// <returns></returns>
        public bool Set(string key,object value, int extime)
        {
            if (!IsCache)
            {
                return true;
            }

            string filepath = GetFilePath(key);
            string xpath = DME.Base.Helper.DME_Path.GetDirectoryName(filepath);
            lock (lockObj)
            {
                DME.Base.Helper.DME_Files.InitFolder(xpath);
                try
                {

                    DME_FileCache_Model fmodel = new DME_FileCache_Model();
                    fmodel.CreateTime = DateTime.Now;
                    fmodel.ExpirationTime = extime;
                    fmodel.Model = value;

                    DME.Base.Helper.DME_Serialize.SerializeToBinary(fmodel, filepath);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (!IsCache)
            {
                return null;
            }
            string filepath = GetFilePath(key);
            if (!DME.Base.Helper.DME_Files.FileExists(filepath))
            {
                return null;
            }

            //string filepath = Path.Combine(BasePath, string.Format("{0}.xml", key));
            DME_FileCache_Model fmodel = DME.Base.Helper.DME_Serialize.DeserializeFromBinary(typeof(DME_FileCache_Model), filepath) as DME_FileCache_Model;
            if(fmodel.ExpirationTime==0)
            {
                return fmodel.Model;
            }
            TimeSpan ts=DateTime.Now-fmodel.CreateTime;
            if (ts.TotalSeconds >= fmodel.ExpirationTime)
            {
                Remove(key);
                return null;
            }
            return fmodel.Model;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (!IsCache)
            {
                return;
            }

            string filepath = GetFilePath(key);
            lock (lockObj)
            {
                try
                {
                    FileInfo f = new FileInfo(filepath);
                    if (f.Exists)
                    {
                        f.Attributes = FileAttributes.Normal;
                        f.Delete();                       
                    }
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }
        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public void RemoveAll()
        {
            if (!IsCache)
            {
                return;
            }
            string cachePath = DME.Base.Helper.DME_Path.GetDirectoryName(CachePath);
            lock (lockObj)
            {
                DME.Base.Helper.DME_Files.DeleteFolder(cachePath, false);
            }

        }
        /// <summary>
        /// 获取缓存路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetFilePath(string key)
        {
            //key = KeyPrefix + key;
            string filepath = "";
            
            //前缀
            if (!string.IsNullOrEmpty(KeyPrefix))
            {
                filepath += "\\" + KeyPrefix;
            }

            // 绝对值取模
            if (CacheSubDirs != 0)
            {
                var n = Math.Abs(key.GetHashCode()) % CacheSubDirs;
                filepath += "\\" + n.ToString();
            }
            
            //按"."分割key,以划分目录
            string[] strs = key.Split('.');
            for (int i = 0; i < strs.Length;i++)
            {
                filepath += "\\" + strs[i];
            }

            filepath = DME.Base.Helper.DME_Path.MapPath(CachePath + filepath + ".cache");
            return filepath;
        }
    }
}
