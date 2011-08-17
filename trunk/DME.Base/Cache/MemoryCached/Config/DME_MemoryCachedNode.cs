using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemoryCached
{
    public class DME_MemoryCachedNode
    {
        private string _name;
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
