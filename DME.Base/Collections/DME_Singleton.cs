using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Collections
{
    /// <summary>
    /// 通用单例模式
    /// business   b = Singleton<business >.Instance; 
    ///等同于business   b =new business ();的使用方法.区别在于通地 Singleton<business >.Instance;永远只会是一个.
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
    public class DME_Singleton<T> where T:new()
    {
        private DME_Singleton(){}
        public static T Instance{
            get{
                return SingletonCreator.instance;
            }
        }

        class SingletonCreator
        {
            internal static readonly T instance = new T();
        }
        
    }
}
