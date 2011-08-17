using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DME.Base.Collections
{
    /// <summary>
    /// Enum工具类
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
    public static class DME_Enum
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
        /// <summary>将Enum的所有枚举值放到IList中，以绑定到如ComoboBox等控件</summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IList<string> ConvertEnumToFieldDescriptionList(Type enumType)
        {
            IList<string> resultList = new List<string>();

            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo fi in fields)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DME_EnumDescription), false);
                if ((attrs != null) && (attrs.Length > 0))
                {
                    DME_EnumDescription des = (DME_EnumDescription)attrs[0];
                    resultList.Add(des.Description);
                }
                else
                {
                    if (fi.Name != "value__")
                    {
                        resultList.Add(fi.Name);
                    }
                }
            }

            return resultList;
        }

        /// <summary>获取Enum的所有Field的文本表示。</summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IList<string> ConvertEnumToFieldTextList(Type enumType)
        {
            IList<string> resultList = new List<string>();

            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo fi in fields)
            {
                if (fi.Name != "value__")
                {
                    resultList.Add(fi.Name);
                }
            }

            return resultList;
        }
        /// <summary>ParseEnumValue 与ConvertEnumToList结合使用，将ComoboBox等控件中选中的string转换为枚举值</summary>
        /// <param name="enumType"></param>
        /// <param name="filedValOrDesc"></param>
        /// <returns></returns>

        public static object ParseEnumValue(Type enumType, string filedValOrDesc)
        {
            if ((enumType == null) || (filedValOrDesc == null))
            {
                return null;
            }

            return Enum.Parse(enumType, filedValOrDesc);
        }        
        #endregion
    }
}
