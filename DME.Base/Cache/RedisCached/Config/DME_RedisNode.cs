using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.RedisCached
{
    public class DME_RedisNode
    {
        private string _name;
        private string _host;
        private int _port = 6379;
        private string _passWord = string.Empty;
        private int _defaultDB = 0;

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
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
            }
        }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }
        /// <summary>
        /// 验证密码
        /// </summary>
        public string PassWord
        {
            get
            {
                return _passWord;
            }
            set
            {
                _passWord = value;
            }
        }
        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DefaultDB
        {
            get
            {
                return _defaultDB;
            }
            set
            {
                _defaultDB = value;
            }
        }
    }
}
