using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Reflection
{
    /// <summary>
    /// 快速索引器接口的默认实现
    /// </summary>
    public class DME_FastIndexAccessor : DME_IIndexAccessor
    {
        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引，反射实现。
        /// 派生实体类可重写该索引，以避免发射带来的性能损耗
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public virtual Object this[String name]
        {
            get
            {
                //尝试匹配属性
                DME_PropertyInfo property = DME_PropertyInfo.Create(this.GetType(), name);
                if (property != null) return property.GetValue(this);

                //尝试匹配字段
                DME_FieldInfo field = DME_FieldInfo.Create(this.GetType(), name);
                if (field != null) return field.GetValue(this);

                throw new ArgumentException("类[" + this.GetType().FullName + "]中不存在[" + name + "]属性或字段。");
            }
            set
            {
                //尝试匹配属性
                DME_PropertyInfo property = DME_PropertyInfo.Create(this.GetType(), name);
                if (property != null)
                {
                    property.SetValue(this, value);
                    return;
                }

                //尝试匹配字段
                DME_FieldInfo field = DME_FieldInfo.Create(this.GetType(), name);
                if (field != null)
                {
                    field.SetValue(this, value);
                    return;
                }

                throw new ArgumentException("类[" + this.GetType().FullName + "]中不存在[" + name + "]属性或字段。");
            }
        }
    }
}
