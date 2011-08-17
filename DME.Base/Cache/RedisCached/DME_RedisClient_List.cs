using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.RedisCached
{
    /// <summary>
    /// List操作
    /// </summary>
    public partial class DME_RedisClient
    {
        #region List commands
        /// <summary>
        /// 从自定的范围内返回序列的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private byte[][] ListRange(string key, int start, int end)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            return redisSocket.SendDataCommandExpectMultiBulkReply(null, "LRANGE {0} {1} {2}\r\n", key, start, end);
        }
        /// <summary>
        /// 从自定的范围内返回序列的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<string> ListRangeString(string key, int start, int end)
        {
            byte[][] data = ListRange(key, start, end);
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
        /// 从自定的范围内返回序列的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<object> ListRangeObject(string key, int start, int end)
        {
            List<string> strList = ListRangeString(key, start, end);
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

        /// <summary>
        /// 从 List 尾部添加一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void RightPushString(string key, string value)
        {
            if (!CheckKeyAndValue(key, value))
            {
                return;
            }
            redisSocket.SendExpectSuccess("RPUSH {0} {1}\r\n{2}\r\n", key, value.Length, value);
        }

        /// <summary>
        /// 从 List 尾部添加一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void RightPushObject(string key, object value)
        {
            RightPushString(key, DME.Base.Helper.DME_Serialize.SerializeToBin(value));
        }

        /// <summary>
        /// 从 List 头部添加一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void LeftPushString(string key, string value)
        {
            if (!CheckKeyAndValue(key, value))
            {
                return;
            }
            redisSocket.SendExpectSuccess("LPUSH {0} {1}\r\n{2}\r\n", key, value.Length, value);
        }

        /// <summary>
        /// 从 List 头部添加一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void LeftPushObject(string key, object value)
        {
            LeftPushString(key, DME.Base.Helper.DME_Serialize.SerializeToBin(value));
        }

        /// <summary>
        /// 返回一个 List 的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int ListLength(string key)
        {
            if (!CheckKey(key))
            {
                return 0;
            }
            return redisSocket.SendExpectInt("LLEN {0}\r\n", key);
        }

        /// <summary>
        /// 返回某个位置的序列值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private byte[] ListIndex(string key, int index)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            redisSocket.SendCommand("LINDEX {0} {1}\r\n", key, index);
            return redisSocket.ReadData();
        }
        /// <summary>
        /// 返回某个位置的序列值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ListIndexString(string key, int index)
        {
            byte[] data = ListIndex(key, index);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        /// 返回某个位置的序列值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public object ListIndexObject(string key, int index)
        {
            string data = ListIndexString(key, index);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return DME.Base.Helper.DME_Serialize.DeserializeFromBin(data);
        }

        /// <summary>
        ///  弹出 List 的第一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] LeftPop(string key)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            redisSocket.SendCommand("LPOP {0}\r\n", key);
            return redisSocket.ReadData();
        }
        /// <summary>
        ///  弹出 List 的第一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string LeftPopString(string key)
        {
            byte[] data = LeftPop(key);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        ///  弹出 List 的第一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object LeftPopObject(string key)
        {
            string data = LeftPopString(key);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return DME.Base.Helper.DME_Serialize.DeserializeFromBin(data);
        }

        /// <summary>
        ///  弹出 List 的最后一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] RightPop(string key)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            redisSocket.SendCommand("RPOP {0}\r\n", key);
            return redisSocket.ReadData();
        }
        /// <summary>
        ///  弹出 List 的最后一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string RightPopString(string key)
        {
            byte[] data = RightPop(key);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        ///  弹出 List 的最后一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object RightPopObject(string key)
        {
            string data = RightPopString(key);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return DME.Base.Helper.DME_Serialize.DeserializeFromBin(data);
        }
        #endregion
    }
}
