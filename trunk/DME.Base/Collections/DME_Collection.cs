using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Collections
{
    /// <summary>
    /// 集合操作类
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
    public static class DME_Collection
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
        /// <summary>从集合中选取符合条件的元素</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IList<TObject> Find<TObject>(IEnumerable<TObject> source, Predicate<TObject> predicate)
        {
            IList<TObject> list = new List<TObject>();
            ActionOnSpecification(source, delegate(TObject ele) { list.Add(ele); }, predicate);
            return list;
        }

        /// <summary> 返回符合条件的第一个元素</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TObject FindFirstSpecification<TObject>(IEnumerable<TObject> source, Predicate<TObject> predicate)
        {
            foreach (TObject element in source)
            {
                if (predicate(element))
                {
                    return element;
                }
            }

            return default(TObject);
        }

        /// <summary>集合中是否包含满足predicate条件的元素</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public static bool ContainsSpecification<TObject>(IEnumerable<TObject> source, Predicate<TObject> predicate, out TObject specification)
        {
            specification = default(TObject);
            foreach (TObject element in source)
            {
                if (predicate(element))
                {
                    specification = element;
                    return true;
                }
            }

            return false;
        }

        /// <summary>集合中是否包含满足predicate条件的元素</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool ContainsSpecification<TObject>(IEnumerable<TObject> source, Predicate<TObject> predicate)
        {
            TObject specification;
            return ContainsSpecification<TObject>(source, predicate, out specification);
        }

        #endregion

        /// <summary>对集合中满足predicate条件的元素执行action。如果没有条件，predicate传入null</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        public static void ActionOnSpecification<TObject>(IEnumerable<TObject> collection, Action<TObject> action, Predicate<TObject> predicate)
        {
            if (collection == null)
            {
                return;
            }

            if (predicate == null)
            {
                foreach (TObject obj in collection)
                {
                    action(obj);
                }

                return;
            }

            foreach (TObject obj in collection)
            {
                if (predicate(obj))
                {
                    action(obj);
                }
            }
        }

        /// <summary>对集合中的每个元素执行action</summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static void ActionOnEach<TObject>(IEnumerable<TObject> collection, Action<TObject> action)
        {
            ActionOnSpecification<TObject>(collection, action, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ary"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] GetPart<T>(T[] ary, int startIndex, int count)
        {
            return GetPart<T>(ary, startIndex, count, false);
        }

        public static T[] GetPart<T>(T[] ary, int startIndex, int count, bool reverse)
        {
            if (startIndex >= ary.Length)
            {
                return null;
            }

            if (ary.Length < startIndex + count)
            {
                count = ary.Length - startIndex;
            }

            T[] result = new T[count];

            if (!reverse)
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] = ary[startIndex + i];
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] = ary[ary.Length - startIndex - 1 - i];
                }
            }

            return result;
        }

        /// <summary>从已排序的列表中，采用二分查找找到目标在列表中的位置。
        /// 如果刚好有个元素与目标相等，则返回true，且minIndex会被赋予该元素的位置；否则，返回false，且minIndex会被赋予比目标小且最接近目标的元素的位置</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortedList"></param>
        /// <param name="target"></param>
        /// <param name="minIndex"></param>
        /// <returns></returns>
        public static bool BinarySearch<T>(IList<T> sortedList, T target, out int minIndex) where T : IComparable
        {
            if (target.CompareTo(sortedList[0]) == 0)
            {
                minIndex = 0;
                return true;
            }

            if (target.CompareTo(sortedList[0]) < 0)
            {
                minIndex = -1;
                return false;
            }

            if (target.CompareTo(sortedList[sortedList.Count - 1]) == 0)
            {
                minIndex = sortedList.Count - 1;
                return true;
            }

            if (target.CompareTo(sortedList[sortedList.Count - 1]) > 0)
            {
                minIndex = sortedList.Count - 1;
                return false;
            }

            int left = 0;
            int right = sortedList.Count - 1;

            while (right - left > 1)
            {
                int middle = (left + right) / 2;

                if (target.CompareTo(sortedList[middle]) == 0)
                {
                    minIndex = middle;
                    return true;
                }

                if (target.CompareTo(sortedList[middle]) < 0)
                {
                    right = middle;
                }
                else
                {
                    left = middle;
                }
            }

            minIndex = left;
            return false;
        }

        /// <summary>高效地求两个List元素的交集</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<T> GetIntersection<T>(List<T> list1, List<T> list2) where T : IComparable
        {
            List<T> largList = list1.Count > list2.Count ? list1 : list2;
            List<T> smallList = largList == list1 ? list2 : list1;

            largList.Sort();

            int minIndex = 0;

            List<T> result = new List<T>();
            foreach (T tmp in smallList)
            {
                if (BinarySearch<T>(largList, tmp, out minIndex))
                {
                    result.Add(tmp);
                }
            }

            return result;
        }

        /// <summary>高效地求两个List元素的并集</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<T> GetUnion<T>(IList<T> list1, IList<T> list2)
        {
            SortedDictionary<T, int> result = new SortedDictionary<T, int>();
            foreach (T tmp in list1)
            {
                if (!result.ContainsKey(tmp))
                {
                    result.Add(tmp, 0);
                }
            }

            foreach (T tmp in list2)
            {
                if (!result.ContainsKey(tmp))
                {
                    result.Add(tmp, 0);
                }
            }

            return (List<T>)DME_CollectionConverter.CopyAllToList<T>(result.Keys);
        } 
    }
}
