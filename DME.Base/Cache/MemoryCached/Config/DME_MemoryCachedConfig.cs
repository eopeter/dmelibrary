using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemoryCached
{
    public class DME_MemoryCachedConfig
    {
        private bool _isCache = true;
        private int _intervalMinutes = 1;
        private int _scavangeMinutes = 60;
        private long _maxCount = 1000000;
        private long _maxSize = 100 * 1024;//100*1024K
        private Dictionary<string, DME_MemoryCachedNode> _nodes = new Dictionary<string, DME_MemoryCachedNode>();

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
        /// 每隔多少分钟扫描一次
        /// </summary>
        public int IntervalMinutes
        {
            get
            {
                return _intervalMinutes;
            }
            set
            {
                _intervalMinutes = value;
            }
        }
        /// <summary>
        /// 处理多少分钟之前的元素
        /// </summary>
        public int ScavangeMinutes
        {
            get
            {
                return _scavangeMinutes;
            }
            set
            {
                _scavangeMinutes = value;
            }
        }
        /// <summary>
        /// 最大可缓存元素数
        /// </summary>
        public long MaxCount
        {
            get
            {
                return _maxCount;
            }
            set
            {
                _maxCount = value;
            }
        }
        /// <summary>
        /// 最大可使用缓存大小
        /// </summary>
        public long MaxSize
        {
            get
            {
                return _maxSize;
            }
            set
            {
                _maxSize = value;
            }
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        public Dictionary<string, DME_MemoryCachedNode> Nodes
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
