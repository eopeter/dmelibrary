using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.FileCached
{
    /// <summary>
    /// 文件缓存实体
    /// </summary>
    [Serializable]
    public class DME_FileCache_Model
    {
        private DateTime _CreateTime;
        private int _ExpirationTime;
        private Object _Model = null;


        #region Property
        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }
        public int ExpirationTime
        {
            get { return _ExpirationTime; }
            set { _ExpirationTime = value; }
        }

        public Object Model
        {
            get { return _Model; }
            set { _Model = value; }
        }
        #endregion
    }
}
