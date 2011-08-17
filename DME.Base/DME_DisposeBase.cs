using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DME.Base
{
    /// <summary>
    /// 具有销毁资源处理的抽象基类
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
    public abstract class DME_DisposeBase : IDisposable
    {
        #region 私有变量
        private Int32 disposed = 0;
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        #endregion

        #region 私有函数
        /// <summary>
        /// 释放资源，参数表示是否由Dispose调用。该方法保证OnDispose只被调用一次！
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(Boolean disposing)
        {
            if (disposed != 0) return;
            if (Interlocked.CompareExchange(ref disposed, 1, 0) != 0) return;

            try
            {
                OnDispose(disposing);
            }
            catch (Exception ex)
            {
                throw new Exception("设计错误，OnDispose中尽可能的不要抛出异常！" + ex.ToString());
            }

            // 只有基类的OnDispose被调用，才有可能是2
            if (Interlocked.CompareExchange(ref disposed, 3, 2) != 2) throw new Exception("设计错误，OnDispose应该只被调用一次！代码不应该直接调用OnDispose，而应该调用Dispose。");
        }

        /// <summary>
        /// 子类重载实现资源释放逻辑时必须首先调用基类方法
        /// </summary>
        /// <param name="disposing">从Dispose调用（释放所有资源）还是析构函数调用（释放非托管资源）</param>
        protected virtual void OnDispose(Boolean disposing)
        {
            // 只有从Dispose中调用，才有可能是1
            if (Interlocked.CompareExchange(ref disposed, 2, 1) != 1) throw new Exception("设计错误，OnDispose应该只被调用一次！代码不应该直接调用OnDispose，而应该调用Dispose。");

            if (disposing)
            {
                // 释放托管资源

                // 告诉GC，不要调用析构函数
                GC.SuppressFinalize(this);
            }

            // 释放非托管资源
        }
        #endregion

        #region 公开函数
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>是否已经释放</summary>
        public Boolean Disposed
        {
            get { return disposed > 0; }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DME_DisposeBase()
        {
            // 如果忘记调用Dispose，这里会释放非托管资源
            // 如果曾经调用过Dispose，因为GC.SuppressFinalize(this)，不会再调用该析构函数
            Dispose(false);
        }
        #endregion
    }
}
