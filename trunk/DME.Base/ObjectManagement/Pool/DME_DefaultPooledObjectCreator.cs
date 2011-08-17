using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.ObjectManagement.Pool
{
    /// <summary>
    /// 直接使用被池化类型的默认构造函数创建对象。
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
    public class DME_DefaultPooledObjectCreator<TObject> : DME_IPooledObjectCreator<TObject> where TObject : class
    {
        #region DME_IPooledObjectCreator<TObject> 成员

        public TObject Create()
        {
            return Activator.CreateInstance<TObject>();
        }

        public void Reset(TObject obj)
        {

        }

        #endregion
    }
}
