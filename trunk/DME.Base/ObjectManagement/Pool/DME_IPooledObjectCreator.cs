using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.ObjectManagement.Pool
{
    /// <summary>
    /// 池化对象创建者。用于创建被池缓存的对象。并能清除对象的状态。
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
    public interface DME_IPooledObjectCreator<TObject> where TObject : class
    {
        TObject Create();
        void Reset(TObject obj);
    }
}
