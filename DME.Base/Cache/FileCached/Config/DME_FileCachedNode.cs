using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.FileCached
{
    /// <summary>
    /// 
    /// </summary>
    public class DME_FileCachedNode
    {
        private string _name;
        private string _fileCachePath;
        private string _keyPrefix = "";
        private int _cacheSubDirs = 1000;

        /// <summary>
        /// 节点名
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// 主机地址
        /// </summary>
        public string FileCachePath
        {
            get
            {
                return _fileCachePath;
            }
            set
            {
                _fileCachePath = value;
            }
        }
        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeyPrefix
        {
            get
            {
                return _keyPrefix;
            }
            set
            {
                _keyPrefix = value;
            }
        }
        /// <summary>
        /// 最大文件目录数
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
    }
}
