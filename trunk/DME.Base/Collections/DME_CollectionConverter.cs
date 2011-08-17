using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Collections
{
    /// <summary>
    /// 用于转换集合内的元素或集合类型
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
    public static class DME_CollectionConverter
    {

        #region 私有变量
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
        #endregion

        #region 公开函数
        /// <summary> 将source中的每个元素转换为TResult类型</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <returns></returns>   
        public static IList<TResult> ConvertAll<TObject, TResult>(IEnumerable<TObject> source, Func<TObject, TResult> converter)
        {
            return ConvertSpecification<TObject, TResult>(source, converter, null);
        }

        /// <summary>将source中的符合predicate条件元素转换为TResult类型</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>    
        public static IList<TResult> ConvertSpecification<TObject, TResult>(IEnumerable<TObject> source, Func<TObject, TResult> converter, Predicate<TObject> predicate)
        {
            IList<TResult> list = new List<TResult>();
            DME_Collection.ActionOnSpecification<TObject>(source, delegate(TObject ele) { list.Add(converter(ele)); }, predicate);
            return list;
        }

        /// <summary>将source中的符合predicate条件的第一个元素转换为TResult类型</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TResult ConvertFirstSpecification<TObject, TResult>(IEnumerable<TObject> source, Func<TObject, TResult> converter, Predicate<TObject> predicate)
        {
            TObject target = DME_Collection.FindFirstSpecification<TObject>(source, predicate);

            if (target == null)
            {
                return default(TResult);
            }

            return converter(target);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<TObject> CopyAllToList<TObject>(IEnumerable<TObject> source)
        {
            IList<TObject> copy = new List<TObject>();
            DME_Collection.ActionOnEach<TObject>(source, delegate(TObject t) { copy.Add(t); });
            return copy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IList<TObject> CopySpecificationToList<TObject>(IEnumerable<TObject> source, Predicate<TObject> predicate)
        {
            IList<TObject> copy = new List<TObject>();
            DME_Collection.ActionOnSpecification<TObject>(source, delegate(TObject t) { copy.Add(t); }, predicate);
            return copy;
        }

        /// <summary>将子类对象集合转换为基类对象集合</summary>
        /// <typeparam name="TBase"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<TBase> ConvertListUpper<TBase, T>(IList<T> list) where T : TBase
        {
            IList<TBase> baseList = new List<TBase>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                baseList.Add(list[i]);
            }

            return baseList;
        }

        /// <summary>将基类对象集合强制转换为子类对象集合</summary>
        /// <typeparam name="TBase"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseList"></param>
        /// <returns></returns>
        public static IList<T> ConvertListDown<TBase, T>(IList<TBase> baseList) where T : TBase
        {
            IList<T> list = new List<T>(baseList.Count);
            for (int i = 0; i < baseList.Count; i++)
            {
                list.Add((T)baseList[i]);
            }

            return list;
        }

        /// <summary>将数组转换为IList</summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="ary"></param>
        /// <returns></returns>
        public static IList<TElement> ConvertArrayToList<TElement>(TElement[] ary)
        {
            if (ary == null)
            {
                return null;
            }

            return DME_Collection.Find<TElement>(ary, null);
        }

        /// <summary>将IList转换为数组</summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TElement[] ConvertListToArray<TElement>(IList<TElement> list)
        {
            if (list == null)
            {
                return null;
            }

            TElement[] ary = new TElement[list.Count];
            for (int i = 0; i < ary.Length; i++)
            {
                ary[i] = list[i];
            }

            return ary;
        }
        #endregion
    }
}
