using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace DME.Base.Cache.RedisCached
{
    /// <summary>
    /// Redis客户端
    /// </summary>
    public partial class DME_RedisClient
    {
        DME_RedisSocket redisSocket;
        private bool _isCache = true;

        #region 构造函数
        public DME_RedisClient(string host, int port,string passWord, int sendTimeout)
        {
            redisSocket = new DME_RedisSocket(host, port, passWord, sendTimeout);
        }

        public DME_RedisClient(string host): this(host, 6379,null,-1)
        {
        }

        public DME_RedisClient()
            : this("localhost", 6379, null, -1)
        {
        }
        #endregion

        #region 属性
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
        #endregion

        #region 辅助函数
        /// <summary>
        /// 检查Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool CheckKey(string key)
        {
            if (!IsCache)
            {
                return false;
            }

            if (key == null)
            {
                throw new ArgumentNullException("Key may not be null.");
            }
            if (key.Length == 0)
            {
                throw new ArgumentException("Key may not be empty.");
            }
            if (key.Contains(" ") || key.Contains("\n") || key.Contains("\r") || key.Contains("\t") || key.Contains("\f") || key.Contains("\v"))
            {
                throw new ArgumentException("Key may not contain whitespace or control characters.");
            }

            return true;
        }
        /// <summary>
        /// 检查Key及Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckKeyAndValue(string key, object value)
        {
            bool keycheck = CheckKey(key);
            if (!keycheck)
            {
                return false;
            }
            if (value == null)
            {
                throw new ArgumentNullException("Value may not be null.");
            }
            if (GetSize(value) > 1073741824)
            {
                throw new ArgumentException("Value exceeds 1G");
            }
            return true;
        }
        /// <summary>
        /// 获取值大小
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private long GetSize(object obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            return ms.Length;
        }

        #endregion

        #region Set(存储)
        /// <summary>
        /// 存储位流
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool Set(string key, byte[] value)
        {
            if (!CheckKeyAndValue(key, value))
            {
                return false;
            }
            if (!redisSocket.SendDataCommand(value, "SET {0} {1}\r\n", key, value.Length))
                throw new Exception("Unable to connect");
            return redisSocket.ExpectSuccess();
        }

        /// <summary>
        /// 存储字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetString(string key, string value)
        {
            return SetString(key, value, 0);
        }

        /// <summary>
        /// 存储字符串，带过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool SetString(string key, string value, int seconds)
        {
            bool result = Set(key, Encoding.UTF8.GetBytes(value));
            if (seconds > 0)
            {
                result = Expire(key, seconds);
            }
            return result;
        }

        /// <summary>
        /// 存储对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetObject(string key, object value)
        {
            return SetObject(key, value,0);
        }

        /// <summary>
        /// 存储对象，带过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds">过期时间</param>
        /// <returns></returns>
        public bool SetObject(string key, object value, int seconds)
        {
            return SetString(key, DME.Base.Helper.DME_Serialize.SerializeToBin(value), seconds);
        }

        /// <summary>
        /// 创建值，如key已存在，则失败
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SetNX(string key, byte[] value)
        {
            if (!CheckKeyAndValue(key, value))
            {
                return false;
            }
            return redisSocket.SendDataExpectInt(value, "SETNX {0} {1}\r\n", key, value.Length) > 0 ? true : false;
        }

        /// <summary>
        /// 创建值，如key已存在，则失败
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetNX(string key, string value)
        {
            return SetNX(key, Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// 设置多个值
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public bool Set(IDictionary<string, string> dict)
        {
            return Set(dict.ToDictionary(k => k.Key, v => Encoding.UTF8.GetBytes(v.Value)));
        }

        /// <summary>
        /// 设置多个值
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private bool Set(IDictionary<string, byte[]> dict)
        {
            if (!IsCache)
            {
                return false;
            }
            if (dict == null)
                throw new ArgumentNullException("dict");

            var nl = Encoding.UTF8.GetBytes("\r\n");

            var ms = new MemoryStream();
            foreach (var key in dict.Keys)
            {
                var val = dict[key];

                var kLength = Encoding.UTF8.GetBytes("$" + key.Length + "\r\n");
                var k = Encoding.UTF8.GetBytes(key + "\r\n");
                var vLength = Encoding.UTF8.GetBytes("$" + val.Length + "\r\n");
                ms.Write(kLength, 0, kLength.Length);
                ms.Write(k, 0, k.Length);
                ms.Write(vLength, 0, vLength.Length);
                ms.Write(val, 0, val.Length);
                ms.Write(nl, 0, nl.Length);
            }

            redisSocket.SendDataCommand(ms.ToArray(), "*" + (dict.Count * 2 + 1) + "\r\n$4\r\nMSET\r\n");
            return redisSocket.ExpectSuccess();
        }
        #endregion Set

        #region Get(获取)
        /// <summary>
        /// 获取位流
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] Get(string key)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            return redisSocket.SendExpectData(null, "GET " + key + "\r\n");
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            byte[] data = Get(key);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetObject(string key)
        {
            string data = GetString(key);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return DME.Base.Helper.DME_Serialize.DeserializeFromBin(data);
        }

        /// <summary>
        /// 获得的key的值然后SET这个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte[] GetSet(string key, byte[] value)
        {
            if (!CheckKeyAndValue(key, value))
            {
                return null;
            }

            if (!redisSocket.SendDataCommand(value, "GETSET {0} {1}\r\n", key, value.Length))
                throw new Exception("Unable to connect");

            return redisSocket.ReadData();
        }

        /// <summary>
        /// 获得的key的值然后SET这个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetSet(string key, string value)
        {
            byte[] data = GetSet(key, Encoding.UTF8.GetBytes(value));
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 一次性返回多个键的值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private byte[][] Gets(params string[] keys)
        {
            if (!IsCache)
            {
                return null;
            }
            if (keys == null)
                throw new ArgumentNullException("key1");
            if (keys.Length == 0)
                throw new ArgumentException("keys");
            return redisSocket.SendDataCommandExpectMultiBulkReply(null, "MGET {0}\r\n", string.Join(" ", keys));
        }
        /// <summary>
        /// 一次性返回多个键的值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<string> GetsString(params string[] keys)
        {
            byte[][] data = Gets(keys);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            List<string> returnData = new List<string>();
            foreach (byte[] item in data)
            {
                returnData.Add(Encoding.UTF8.GetString(item));
            }
            return returnData;
        }
        /// <summary>
        /// 一次性返回多个键的值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<object> GetsObject(params string[] keys)
        {
            List<string> strList = GetsString(keys);
            if (strList == null || strList.Count == 0)
            {
                return null;
            }
            List<object> returnData = new List<object>();
            foreach (string item in strList)
            {
                returnData.Add(DME.Base.Helper.DME_Serialize.DeserializeFromBin(item));
            }
            return returnData;
        }
        #endregion Get

        #region Sort
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private byte[][] Sort(DME_SortOptions options)
        {
            return redisSocket.SendDataCommandExpectMultiBulkReply(null, options.ToCommand() + "\r\n");
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public List<string> SortString(DME_SortOptions options)
        {
            byte[][] data = Sort(options);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            List<string> returnData = new List<string>();
            foreach (byte[] item in data)
            {
                returnData.Add(Encoding.UTF8.GetString(item));
            }
            return returnData;
        }
        #endregion Sort

        #region Key
        /// <summary>
        /// key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            if (!CheckKey(key))
            {
                return false;
            }
            return redisSocket.SendExpectInt("EXISTS " + key + "\r\n") == 1;
        }
        /// <summary>
        /// 更改Key名，新键如果存在将被覆盖
        /// </summary>
        /// <param name="oldKeyname"></param>
        /// <param name="newKeyname"></param>
        /// <returns></returns>
        public bool Rename(string oldKeyname, string newKeyname)
        {
            if (oldKeyname == null)
                throw new ArgumentNullException("oldKeyname");
            if (newKeyname == null)
                throw new ArgumentNullException("newKeyname");
            return redisSocket.SendGetString("RENAME {0} {1}\r\n", oldKeyname, newKeyname)[0] == '+';
        }
        /// <summary>
        /// 设置Key在seconds秒后删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool Expire(string key, int seconds)
        {
            if (!CheckKey(key))
            {
                return false;
            }
            return redisSocket.SendExpectInt("EXPIRE {0} {1}\r\n", key, seconds) == 1;
        }
        /// <summary>
        /// 在某个时刻失效
        /// </summary>
        /// <param name="key"></param>
        /// <param name="at"></param>
        /// <returns></returns>
        public bool ExpireAt(string key, DateTime at)
        {
            if (!CheckKey(key))
            {
                return false;
            }

            DateTime unixEpoch = new DateTime(1970, 1, 1);
            long time = Convert.ToInt64((unixEpoch - at).TotalSeconds);
            return redisSocket.SendExpectInt("EXPIREAT {0} {1}\r\n", key, time) == 1;
        }
        /// <summary>
        /// 失效剩余时间（秒）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int TimeToLive(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return redisSocket.SendExpectInt("TTL {0}\r\n", key);
        }
        /// <summary>
        /// 返回某个key元素的数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DME_KeyType TypeOf(string key)
        {
            if (!CheckKey(key))
            {
                throw new ArgumentNullException("key");
            }

            switch (redisSocket.SendExpectString("TYPE {0}\r\n", key))
            {
                case "none":
                    return DME_KeyType.None;
                case "string":
                    return DME_KeyType.String;
                case "set":
                    return DME_KeyType.Set;
                case "list":
                    return DME_KeyType.List;
            }
            throw new DME_ResponseException("Invalid value");
        }
        /// <summary>
        /// 随机获得一个已经存在的key，如果当前数据库为空，则返回空字符串
        /// </summary>
        /// <returns></returns>
        public string RandomKey()
        {
            return redisSocket.SendExpectString("RANDOMKEY\r\n");
        }

        /// <summary>
        /// 返回所有的key列表
        /// </summary>
        /// <returns></returns>
        public string[] Keys()
        {
            string commandResponse = Encoding.UTF8.GetString(redisSocket.SendExpectData(null, "KEYS *\r\n"));
            if (commandResponse.Length < 1)
            {
                return new string[0];
            }
            else
            {
                return commandResponse.Split(' ');
            }
        }
        /// <summary>
        /// 返回匹配的key列表
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string[] GetKeys(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException("key");
            var keys = redisSocket.SendExpectData(null, "KEYS {0}\r\n", pattern);
            if (keys.Length == 0)
                return new string[0];
            return Encoding.UTF8.GetString(keys).Split(' ');
        }
        #endregion Key

        #region Remove
        /// <summary>
        /// 移除某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (!CheckKey(key))
            {
                return false;
            }
            return redisSocket.SendExpectInt("DEL " + key + "\r\n", key) == 1;
        }
        /// <summary>
        /// 移除多个Key
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Remove(params string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            return redisSocket.SendExpectInt("DEL " + string.Join(" ", args) + "\r\n");
        }
        #endregion Key

        #region Inc Dec
        /// <summary>
        /// 自增键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Increment(string key)
        {
            if (!CheckKey(key))
            {
                return 0;
            }
            return redisSocket.SendExpectInt("INCR " + key + "\r\n");
        }
        /// <summary>
        /// 令键值自增指定数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Increment(string key, int count)
        {
            if (!CheckKey(key))
            {
                return 0;
            }
            return redisSocket.SendExpectInt("INCRBY {0} {1}\r\n", key, count);
        }
        /// <summary>
        /// 自减键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Decrement(string key)
        {
            if (!CheckKey(key))
            {
                return 0;
            }
            return redisSocket.SendExpectInt("DECR " + key + "\r\n");
        }
        /// <summary>
        /// 令键值自减指定数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Decrement(string key, int count)
        {
            if (!CheckKey(key))
            {
                return 0;
            }
            return redisSocket.SendExpectInt("DECRBY {0} {1}\r\n", key, count);
        }
        #endregion Inc Dec

        #region DB
        /// <summary>
        /// 选择数据库
        /// </summary>
        /// <param name="db"></param>
        public void SelectDB(int db)
        {
            redisSocket.SendExpectSuccess("SELECT {0}\r\n", db);
        }
        /// <summary>
        /// 返回当前数据库的key的总数
        /// </summary>
        /// <returns></returns>
        public int DbSize()
        {
            return redisSocket.SendExpectInt("DBSIZE\r\n");
        }
        /// <summary>
        /// 同步保存数据到磁盘
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            return redisSocket.SendGetString("SAVE\r\n");
        }
        /// <summary>
        /// 异步保存数据到磁盘
        /// </summary>
        public void BackgroundSave()
        {
            redisSocket.SendGetString("BGSAVE\r\n");
        }
        /// <summary>
        /// 同步保存到服务器并关闭 Redis 服务器
        /// </summary>
        public void Shutdown()
        {
            redisSocket.SendGetString("SHUTDOWN\r\n");
        }
        /// <summary>
        /// 清空所有数据库中的所有键
        /// </summary>
        public void FlushAll()
        {
            redisSocket.SendGetString("FLUSHALL\r\n");
        }
        /// <summary>
        /// 清空当前数据库中的所有键
        /// </summary>
        public void FlushDB()
        {
            redisSocket.SendGetString("FLUSHDB\r\n");
        }

        /// <summary>
        /// 最后保存时间
        /// </summary>
        /// <returns></returns>
        public DateTime LastSave()
        {
            //1970-1-1 00:00:00
            long UnixEpoch = 621355968000000000L;
            int t = redisSocket.SendExpectInt("LASTSAVE\r\n");

            //Console.WriteLine(DateTime.Now.Subtract(TimeSpan.FromSeconds(t)).ToString());
            //获取当前时区
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;

            return new DateTime(UnixEpoch).AddHours(offset) + TimeSpan.FromSeconds(t);
        }
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetInfo()
        {
            byte[] r = redisSocket.SendExpectData(null, "INFO\r\n");
            var dict = new Dictionary<string, string>();

            foreach (var line in Encoding.UTF8.GetString(r).Split('\n'))
            {
                int p = line.IndexOf(':');
                if (p == -1)
                    continue;
                dict.Add(line.Substring(0, p), line.Substring(p + 1));
            }
            return dict;
        }
        #endregion DB
    }
}
