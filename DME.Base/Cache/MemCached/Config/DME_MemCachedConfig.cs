using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemCached
{
    public class DME_MemCachedConfig
    {
        private bool _isCache = true;
        private int _sendReceiveTimeout = 2000;
        private int _minPoolSize = 5;
        private int _maxPoolSize = 10;
        private Dictionary<string, DME_MemCachedNode> _nodes = new Dictionary<string, DME_MemCachedNode>();

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache
        {
            get
            {
                return _isCache;
            }
            set
            {
                _isCache = value;
            }
        }
        /// <summary>
        /// 发送超时时间
        /// </summary>
        public int SendReceiveTimeout
        {
            get
            {
                return _sendReceiveTimeout;
            }
            set
            {
                _sendReceiveTimeout = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MinPoolSize
        {
            get
            {
                return _minPoolSize;
            }
            set
            {
                _minPoolSize = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MaxPoolSize
        {
            get
            {
                return _maxPoolSize;
            }
            set
            {
                _maxPoolSize = value;
            }
        }
        /// <summary>
        /// 节点列表
        /// </summary>
        public Dictionary<string, DME_MemCachedNode> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }
    }
}
