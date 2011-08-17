using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Collections
{
    /// <summary>
    /// 弱引用
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
    public class DME_WeakReference<T> : WeakReference
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public DME_WeakReference() : base(null) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="target"></param>
        public DME_WeakReference(T target) : base(target) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trackResurrection"></param>
        public DME_WeakReference(T target, bool trackResurrection) : base(target, trackResurrection) { }

        /// <summary>
        /// 目标引用对象
        /// </summary>
        public new T Target
        {
            get { return (T)base.Target; }
            set { base.Target = value; }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static implicit operator T(DME_WeakReference<T> obj)
        {
            if (obj != null && obj.Target != null) return obj.Target;
            return default(T);
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static implicit operator DME_WeakReference<T>(T target)
        {
            return new DME_WeakReference<T>(target);
        }
    }
}
