using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.FileCached
{
    /// <summary>
    /// 
    /// </summary>
    public class DME_FileCachedConfig
    {
        private bool _isCache = true;
        private Dictionary<string, DME_FileCachedNode> _nodes = new Dictionary<string, DME_FileCachedNode>();

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
        /// 节点列表
        /// </summary>
        public Dictionary<string, DME_FileCachedNode> Nodes
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
