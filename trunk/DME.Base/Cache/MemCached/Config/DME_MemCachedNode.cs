using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemCached
{
    public class DME_MemCachedNode
    {
        private string _name;
        private string _hosts;
        private string _keyPrefix = "";

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
        public string Hosts
        {
            get
            {
                return _hosts;
            }
            set
            {
                _hosts = value;
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
    }
}
