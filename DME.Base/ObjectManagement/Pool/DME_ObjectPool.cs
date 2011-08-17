using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.ObjectManagement.Pool
{

    /// <summary>
    /// 对象池实现类
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DME_ObjectPool<TObject> : DME_IObjectPool<TObject> where TObject : class
    {
        
        #region 私有变量
        private IList<TObject> idleList = new List<TObject>();
        private IDictionary<TObject, TObject> busyDictionary = new Dictionary<TObject, TObject>();
        private object locker = new object();
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        #region MinObjectCount 对象池中最少同时存在的对象数。
        /// <summary>
        /// 对象池中最少同时存在的对象数。
        /// </summary>
        private int minObjectCount = 0;
        public int MinObjectCount
        {
            get { return minObjectCount; }
            set { minObjectCount = value; }
        }
        #endregion

        #region MaxObjectCount 对象池中最多同时存在的对象数。
        /// <summary>
        /// 对象池中最多同时存在的对象数。
        /// </summary>
        private int maxObjectCount = int.MaxValue;
        public int MaxObjectCount
        {
            get { return maxObjectCount; }
            set { maxObjectCount = value; }
        }
        #endregion

        #region DetectSpanInMSecs
        /// <summary>
        /// 当池中没有空闲的对象且数量已达到MaxObjectCount时，如果这时发生Rent调用，则检测空闲对象的时间间隔。
        /// </summary>
        private int detectSpanInMSecs = 10;
        public int DetectSpanInMSecs
        {
            get { return detectSpanInMSecs; }
            set { detectSpanInMSecs = value; }
        }
        #endregion

        #region PooledObjectCreator
        private DME_IPooledObjectCreator<TObject> pooledObjectCreator = new DME_DefaultPooledObjectCreator<TObject>();
        public DME_IPooledObjectCreator<TObject> PooledObjectCreator
        {
            set { pooledObjectCreator = value ?? new DME_DefaultPooledObjectCreator<TObject>(); }
        }
        #endregion 
        #endregion

        #region 私有函数
        #endregion

        #region 公开函数
        #endregion

        #region DME_IObjectPool<TObject> 成员


        public void Initialize()
        {
            if (this.minObjectCount < 0)
            {
                throw new Exception("The MinObjectCount must be greater than 0 !");
            }

            if (this.minObjectCount > this.maxObjectCount)
            {
                throw new Exception("The MinObjectCount can't be greater than MaxObjectCount !");
            }

            if (this.detectSpanInMSecs < 0)
            {
                throw new Exception("The DetectSpanInMSecs must be greater than 0 !");
            }

            for (int i = 0; i < this.minObjectCount; i++)
            {
                TObject obj = this.pooledObjectCreator.Create();
                this.idleList.Add(obj);
            }
        }

        public TObject Rent()
        {
            lock (this.locker)
            {
                if ((this.idleList.Count == 0) && (this.busyDictionary.Count < this.maxObjectCount))
                {
                    TObject obj = this.pooledObjectCreator.Create();
                    this.busyDictionary.Add(obj, obj);
                    return obj;
                }

                while (this.idleList.Count == 0)
                {
                    System.Threading.Thread.Sleep(this.detectSpanInMSecs);
                }

                TObject objToRent = this.idleList[0];
                this.idleList.RemoveAt(0);
                this.busyDictionary.Add(objToRent, objToRent);
                return objToRent;
            }
        }

        public void GiveBack(TObject obj)
        {
            lock (this.locker)
            {
                if (!this.busyDictionary.ContainsKey(obj))
                {
                    return;
                }

                this.pooledObjectCreator.Reset(obj);
                this.busyDictionary.Remove(obj);
                this.idleList.Add(obj);
            }
        }

        #endregion
    }
}
