using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DME.Web.Template
{
    /// <summary>
    /// 属性集合
    /// </summary>
    public class DMEWeb_AttributeCollection : IEnumerable<DMEWeb_Attribute>
    {
         /// <summary>
        /// 构造默认模板属性
        /// </summary>
        /// <param name="ownerElement"></param>
        internal DMEWeb_AttributeCollection(DMEWeb_Element ownerElement)
        {
            this.OwnerElement = ownerElement;
            _Dictionary = new Dictionary<string, DMEWeb_Attribute>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 构造一定容量的默认模板属性
        /// </summary>
        /// <param name="ownerElement"></param>
        /// <param name="capacity"></param>
        internal DMEWeb_AttributeCollection(DMEWeb_Element ownerElement, int capacity)
        {
            this.OwnerElement = ownerElement;
            _Dictionary = new Dictionary<string, DMEWeb_Attribute>(capacity, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 宿主标签
        /// </summary>
        internal DMEWeb_Element OwnerElement { get; private set; }

        /// <summary>
        /// 存放容器
        /// </summary>
        private Dictionary<string, DMEWeb_Attribute> _Dictionary;

        /// <summary>
        /// 返回某个索引位置的属性元素
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>如果存在此索引位置属性元素,则返回其值或者返回null</returns>
        public DMEWeb_Attribute this[int index]
        {
            get
            {
                if (index >= 0 && index < _Dictionary.Count)
                {

                    IEnumerator<DMEWeb_Attribute> enumer = _Dictionary.Values.GetEnumerator();
                    int i = 0;
                    while (enumer.MoveNext())
                    {
                        if (i == index)
                        {
                            return enumer.Current;
                        }

                        i++;
                    }                    
                }
                return null;
            }
        }

        /// <summary>
        /// 返回某个名称的属性元素
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns>如果存在此属性,则返回其元素或者返回null</returns>
        public DMEWeb_Attribute this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return null;

                DMEWeb_Attribute value;
                bool success = _Dictionary.TryGetValue(name, out value);
                if (!success) value = null;

                return value;
            }
        }

        /// <summary>
        /// 返回某个名称的属性值
        /// </summary>
        /// <param name="name">属性的名称</param>
        /// <returns>如果存在此属性则返回其值,否则返回字符串空值</returns>
        public string GetValue(string name)
        {
            return GetValue(name, string.Empty);
        }

        /// <summary>
        /// 返回某个名称的属性值
        /// </summary>
        /// <param name="name">属性的名称</param>
        /// <param name="defaultValue">如果不存在则属性则返回此默认值</param>
        /// <returns>如果存在此属性则返回其值,否则返回默认值</returns>
        public string GetValue(string name, string defaultValue)
        {
            DMEWeb_Attribute attribute = this[name];
            if (attribute == null || attribute.Value == null) return defaultValue;
            object value = attribute.Value.GetValue();
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 添加某个属性值
        /// </summary>
        /// <param name="item">属性元素</param>
        internal void Add(DMEWeb_Attribute item)
        {
            if (item == null) return;

            item.OwnerElement = this.OwnerElement;

            //判断是否存在某个值,如果存在则更新
            if (Contains(item.Name))
            {
                _Dictionary[item.Name] = item;
            }
            else
            {
                _Dictionary.Add(item.Name, item);
            }            
        }

        /// <summary>
        /// 添加某个属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        internal void Add(string name, string text)
        {
            DMEWeb_Attribute attribute = new DMEWeb_Attribute(this.OwnerElement, name, text);
            this.Add(attribute);
            this.OnAdding(attribute);
        }

        /// <summary>
        /// 清空所有属性值
        /// </summary>
        internal void Clear()
        {
            _Dictionary.Clear();
        }

        /// <summary>
        /// 判断是否存在某个属性
        /// </summary>
        /// <param name="name">要判断的属性名称</param>
        /// <returns>存在则返回true否则返回false</returns>
        public bool Contains(string name)
        {
            return _Dictionary.ContainsKey(name);
        }

        /// <summary>
        /// 返回属性数目
        /// </summary>
        public int Count
        {
            get
            {
                return _Dictionary.Count;
            }
        }

        #region 事件处理
        /// <summary>
        /// 添加新属性时的触发事件参数
        /// </summary>
        internal class DMEWeb_AttributeAddingEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            public DMEWeb_AttributeAddingEventArgs(DMEWeb_Attribute item)
            {
                this.Item = item;
            }
            /// <summary>
            /// 添加添加的项目
            /// </summary>
            public DMEWeb_Attribute Item { get; private set; }
        }
        /// <summary>
        /// 添加新属性时的触发事件
        /// </summary>
        internal event EventHandler<DMEWeb_AttributeAddingEventArgs> Adding;
        /// <summary>
        /// 添加新属性时触发事件
        /// </summary>
        /// <param name="item"></param>
        protected void OnAdding(DMEWeb_Attribute item)
        {
            EventHandler<DMEWeb_AttributeAddingEventArgs> e = this.Adding;
            if (e != null) e(this, new DMEWeb_AttributeAddingEventArgs(item));
        }
        #endregion

        #region IEnumerable<Attribute> 成员
        /// <summary>
        /// 返回当前对象的迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DMEWeb_Attribute> GetEnumerator()
        {
            return this._Dictionary.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员
        /// <summary>
        /// 返回当前对象的迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Dictionary.Values.GetEnumerator();
        }

        #endregion
    }
}
