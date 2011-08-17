using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DME.Base.Cache.RedisCached
{
    public class DME_RedisConfig
    {
        private bool _isCache = true;
        private int _sendTimeout = -1;
        private Dictionary<string, DME_RedisNode> _nodes = new Dictionary<string, DME_RedisNode>();

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
        public int SendTimeout
        {
            get
            {
                return _sendTimeout;
            }
            set
            {
                _sendTimeout = value;
            }
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        public Dictionary<string, DME_RedisNode> Nodes
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
