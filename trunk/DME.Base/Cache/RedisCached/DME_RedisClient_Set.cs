using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.RedisCached
{
    /// <summary>
    /// Set操作
    /// </summary>
    public partial class DME_RedisClient
    {
        #region Set commands

        #region 增加元素
        /// <summary>
        /// 增加元素到SETS序列,如果元素（member）不存在则添加成功 1，否则失败 0;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool AddToSet(string key, byte[] member)
        {
            if (!CheckKey(key))
            {
                return false;
            }
            return redisSocket.SendDataExpectInt(member, "SADD {0} {1}\r\n", key, member.Length) > 0 ? true : false;
        }
        /// <summary>
        /// 增加元素到SETS序列,如果元素（member）不存在则添加成功 1，否则失败 0;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool AddToSetString(string key, string member)
        {
            return AddToSet(key, Encoding.UTF8.GetBytes(member));
        }
        /// <summary>
        /// 增加元素到SETS序列,如果元素（member）不存在则添加成功 1，否则失败 0;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool AddToSetObject(string key, object member)
        {
            return AddToSetString(key, DME.Base.Helper.DME_Serialize.SerializeToBin(member));
        }
        #endregion 增加元素

        #region 获取
        /// <summary>
        /// 统计某个SETS的序列的元素数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int CardinalityOfSet(string key)
        {
            if (!CheckKey(key))
            {
                return 0;
            }
            return redisSocket.SendDataExpectInt(null, "SCARD {0}\r\n", key);
        }

        /// <summary>
        /// 获知指定成员是否存在于集合中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsMemberOfSet(string key, byte[] member)
        {
            if (!CheckKey(key))
            {
                return false;
            }
            return redisSocket.SendDataExpectInt(member, "SISMEMBER {0} {1}\r\n", key, member.Length) > 0 ? true : false;
        }
        /// <summary>
        /// 获知指定成员是否存在于集合中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool IsMemberOfSetString(string key, string member)
        {
            return IsMemberOfSet(key, Encoding.UTF8.GetBytes(member));
        }
        /// <summary>
        /// 获知指定成员是否存在于集合中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool IsMemberOfSetObject(string key, object member)
        {
            return IsMemberOfSetString(key, DME.Base.Helper.DME_Serialize.SerializeToBin(member));
        }

        /// <summary>
        /// 返回某个序列的所有元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[][] GetMembersOfSet(string key)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            return redisSocket.SendDataCommandExpectMultiBulkReply(null, "SMEMBERS {0}\r\n", key);
        }
        /// <summary>
        /// 返回某个序列的所有元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> GetMembersOfSetString(string key)
        {
            byte[][] data = GetMembersOfSet(key);
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
        /// 返回某个序列的所有元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<object> GetMembersOfSetObject(string key)
        {
            List<string> strList = GetMembersOfSetString(key);
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
        /// 随机返回某个序列的元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] GetRandomMemberOfSet(string key)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            return redisSocket.SendExpectData(null, "SRANDMEMBER {0}\r\n", key);
        }
        /// <summary>
        /// 随机返回某个序列的元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetRandomMemberOfSetString(string key)
        {
            byte[] data = GetRandomMemberOfSet(key);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        /// 随机返回某个序列的元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetRandomMemberOfSetObject(string key)
        {
            string data = GetRandomMemberOfSetString(key);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return DME.Base.Helper.DME_Serialize.DeserializeFromBin(data);
        }

        /// <summary>
        /// 从集合中随机弹出一个成员
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] PopRandomMemberOfSet(string key)
        {
            if (!CheckKey(key))
            {
                return null;
            }
            return redisSocket.SendExpectData(null, "SPOP {0}\r\n", key);
        }
        /// <summary>
        /// 从集合中随机弹出一个成员
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string PopRandomMemberOfSetString(string key)
        {
            byte[] data = PopRandomMemberOfSet(key);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        /// 从集合中随机弹出一个成员
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object PopRandomMemberOfSetObject(string key)
        {
            string data = PopRandomMemberOfSetString(key);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return DME.Base.Helper.DME_Serialize.DeserializeFromBin(data);
        }
        #endregion 获取

        #region 删除
        /// <summary>
        /// 删除SETS序列的某个元素，如果元素不存在则失败0，否则成功 1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool RemoveFromSet(string key, byte[] member)
        {
            if (!CheckKey(key))
            {
                return false;
            }
            return redisSocket.SendDataExpectInt(member, "SREM {0} {1}\r\n", key, member.Length) > 0 ? true : false;
        }
        /// <summary>
        /// 删除SETS序列的某个元素，如果元素不存在则失败0，否则成功 1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool RemoveFromSetString(string key, string member)
        {
            return RemoveFromSet(key, Encoding.UTF8.GetBytes(member));
        }
        /// <summary>
        /// 删除SETS序列的某个元素，如果元素不存在则失败0，否则成功 1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool RemoveFromSetObject(string key, object member)
        {
            return RemoveFromSetString(key, DME.Base.Helper.DME_Serialize.SerializeToBin(member));
        }
        #endregion 删除

        #region 并集
        /// <summary>
        /// 返回 key1, key2, …, keyN(keys) 的并集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private byte[][] GetUnionOfSets(params string[] keys)
        {
            if (!IsCache)
            {
                return null;
            }
            if (keys == null)
                throw new ArgumentNullException();

            return redisSocket.SendDataCommandExpectMultiBulkReply(null, "SUNION " + string.Join(" ", keys) + "\r\n");

        }
        /// <summary>
        ///  返回 key1, key2, …, keyN(keys) 的并集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<string> GetUnionOfSetsString(params string[] keys)
        {
            byte[][] data = GetUnionOfSets(keys);
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
        ///  返回 key1, key2, …, keyN(keys) 的并集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<object> GetUnionOfSetsObject(params string[] keys)
        {
            List<string> strList = GetUnionOfSetsString(keys);
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
        /// 将 keys 的并集存入 dstkey
        /// </summary>
        /// <param name="destKey"></param>
        /// <param name="keys"></param>
        public void StoreUnionOfSets(string destKey, params string[] keys)
        {
            if (!IsCache)
            {
                return;
            }
            redisSocket.StoreSetCommands("SUNIONSTORE", destKey, keys);
        }
        #endregion 并集

        #region 交集
        /// <summary>
        /// 返回 key1, key2, …, keyN(keys) 的交集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private byte[][] GetIntersectionOfSets(params string[] keys)
        {
            if (!IsCache)
            {
                return null;
            }
            if (keys == null)
                throw new ArgumentNullException();

            return redisSocket.SendDataCommandExpectMultiBulkReply(null, "SINTER " + string.Join(" ", keys) + "\r\n");
        }
        /// <summary>
        ///  返回 key1, key2, …, keyN(keys) 的交集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<string> GetIntersectionOfSetsString(params string[] keys)
        {
            byte[][] data = GetIntersectionOfSets(keys);
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
        ///  返回 key1, key2, …, keyN(keys) 的交集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<object> GetIntersectionOfSetsObject(params string[] keys)
        {
            List<string> strList = GetIntersectionOfSetsString(keys);
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
        /// 将 key1, key2, …, keyN(keys) 中的交集存入 dstkey
        /// </summary>
        /// <param name="destKey"></param>
        /// <param name="keys"></param>
        public void StoreIntersectionOfSets(string destKey, params string[] keys)
        {
            if (!IsCache)
            {
                return;
            }
            redisSocket.StoreSetCommands("SINTERSTORE", destKey, keys);
        }
        #endregion 交集

        #region 差集
        /// <summary>
        /// 依据 key2, …, keyN 求 key1 的差集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private byte[][] GetDifferenceOfSets(params string[] keys)
        {
            if (!IsCache)
            {
                return null;
            }
            if (keys == null)
                throw new ArgumentNullException();

            return redisSocket.SendDataCommandExpectMultiBulkReply(null, "SDIFF " + string.Join(" ", keys) + "\r\n");

        }
        /// <summary>
        ///  依据 key2, …, keyN 求 key1 的差集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<string> GetDifferenceOfSetsString(params string[] keys)
        {
            byte[][] data = GetDifferenceOfSets(keys);
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
        ///  依据 key2, …, keyN 求 key1 的差集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<object> GetDifferenceOfSetsObject(params string[] keys)
        {
            List<string> strList = GetDifferenceOfSetsString(keys);
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
        /// 依据 key2, …, keyN 求 key1(keys) 的差集并存入 dstkey
        /// </summary>
        /// <param name="destKey"></param>
        /// <param name="keys"></param>
        public void StoreDifferenceOfSets(string destKey, params string[] keys)
        {
            if (!IsCache)
            {
                return;
            }
            redisSocket.StoreSetCommands("SDIFFSTORE", destKey, keys);
        }
        #endregion 差集

        #region 移动
        /// <summary>
        /// 把一个SETS序列的某个元素 移动到 另外一个SETS序列
        /// </summary>
        /// <param name="srcKey"></param>
        /// <param name="destKey"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool MoveMemberToSet(string srcKey, string destKey, byte[] member)
        {
            if (!IsCache)
            {
                return false;
            }
            return redisSocket.SendDataExpectInt(member, "SMOVE {0} {1} {2}\r\n", srcKey, destKey, member.Length) > 0 ? true : false;
        }
        /// <summary>
        /// 把一个SETS序列的某个元素 移动到 另外一个SETS序列
        /// </summary>
        /// <param name="srcKey"></param>
        /// <param name="destKey"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool MoveMemberToSetString(string srcKey, string destKey, string member)
        {
            return MoveMemberToSet(srcKey, destKey, Encoding.UTF8.GetBytes(member));
        }
        /// <summary>
        /// 把一个SETS序列的某个元素 移动到 另外一个SETS序列
        /// </summary>
        /// <param name="srcKey"></param>
        /// <param name="destKey"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool MoveMemberToSetObject(string srcKey, string destKey, object member)
        {
            return MoveMemberToSetString(srcKey, destKey, DME.Base.Helper.DME_Serialize.SerializeToBin(member));
        }
        #endregion 移动

        #endregion
    }
}
