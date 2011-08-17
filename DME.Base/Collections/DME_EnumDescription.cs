using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DME.Base.Collections
{
    /// <summary>
    /// 用于描述枚举的特性
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
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class DME_EnumDescription : Attribute
    {
        private static IDictionary<string, IList<DME_EnumDescription>> EnumDescriptionCache = new Dictionary<string, IList<DME_EnumDescription>>(); //EnumType.FullName - IList<EnumDescription>

		#region Ctor
        public DME_EnumDescription(string _description) :this(_description ,null)
		{
		}

        public DME_EnumDescription(string _description, object _tag)
        {
            this.description = _description;
            this.tag = _tag;
        }
		#endregion

		#region property
        #region Description
        private string description = "";
        public string Description
        {
            get
            {
                return this.description;
            }
        }	 
        #endregion	

        #region EnumValue
        private object enumValue = null;
        public object EnumValue
        {
            get
            {
                return this.enumValue;
            }
        } 
        #endregion

        #region Tag
        private object tag = null;
        public object Tag
        {
            get
            {
                return this.tag;
            }
        } 
        #endregion

        #region ToString
        public override string ToString()
        {
            return this.description;
        } 
        #endregion		
		
		#endregion

        #region DoGetFieldTexts
        /// <summary>
        /// DoGetFieldTexts 得到枚举类型定义的所有枚举值的描述文本		
        /// </summary>	
        private static IList<DME_EnumDescription> DoGetFieldTexts(Type enumType)
        {
            if (!DME_EnumDescription.EnumDescriptionCache.ContainsKey(enumType.FullName))
            {
                FieldInfo[] fields = enumType.GetFields();
                IList<DME_EnumDescription> list = new List<DME_EnumDescription>();
                foreach (FieldInfo fi in fields)
                {
                    object[] eds = fi.GetCustomAttributes(typeof(DME_EnumDescription), false);
                    if (eds.Length == 1)
                    {
                        DME_EnumDescription enumDescription = (DME_EnumDescription)eds[0];
                        enumDescription.enumValue = fi.GetValue(null);
                        list.Add(enumDescription);
                    }
                }

                DME_EnumDescription.EnumDescriptionCache.Add(enumType.FullName, list);
            }

            return DME_EnumDescription.EnumDescriptionCache[enumType.FullName];
        }
        #endregion

        #region GetEnumDescriptionText

        /// <summary>获取枚举类型的描述文本</summary>
	    /// <param name="enumType"></param>
	    /// <returns></returns>
        public static string GetEnumDescriptionText(Type enumType)
		{
            DME_EnumDescription[] enumDescriptionAry = (DME_EnumDescription[])enumType.GetCustomAttributes(typeof(DME_EnumDescription), false);
            if (enumDescriptionAry.Length < 1)
            {
                return string.Empty;
            }

            return enumDescriptionAry[0].Description;
        }
        #endregion

        #region GetEnumTag

        /// <summary>获取枚举类型携带的Tag。</summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static object GetEnumTag(Type enumType)
        {
            DME_EnumDescription[] eds = (DME_EnumDescription[])enumType.GetCustomAttributes(typeof(DME_EnumDescription), false);
            if (eds.Length != 1) return string.Empty;
            return eds[0].Tag;
        } 
        #endregion		
		
        #region GetFieldText

        /// <summary>获得指定枚举值的描述文本。</summary>
		/// <param name="enumValue"></param>
		/// <returns></returns>
        public static string GetFieldText(object enumValue)
        {
            IList<DME_EnumDescription> list = DME_EnumDescription.DoGetFieldTexts(enumValue.GetType());
            if (list == null)
            {
                return null;
            }

            return DME_CollectionConverter.ConvertFirstSpecification<DME_EnumDescription, string>(list, delegate(DME_EnumDescription ed) { return ed.Description; }, delegate(DME_EnumDescription ed) { return ed.enumValue.ToString() == enumValue.ToString(); });
        } 
        #endregion

        #region GetFieldTag
        /// <summary> 获得指定枚举值的Tag。</summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static object GetFieldTag(object enumValue)
        {
            IList<DME_EnumDescription> list = DME_EnumDescription.DoGetFieldTexts(enumValue.GetType());
            if (list == null)
            {
                return null;
            }

            return DME_CollectionConverter.ConvertFirstSpecification<DME_EnumDescription, object>(list, delegate(DME_EnumDescription ed) { return ed.Tag; }, delegate(DME_EnumDescription ed) { return ed.enumValue.ToString() == enumValue.ToString(); });
        }     
        #endregion

        #region GetEnumValueByTag

        /// <summary>根据描述Tag获取对应的枚举值</summary>
        /// <param name="enumType"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static object GetEnumValueByTag(Type enumType, object tag)
        {
            IList<DME_EnumDescription> list = DME_EnumDescription.DoGetFieldTexts(enumType);
            if (list == null)
            {
                return null;
            }

            return DME_CollectionConverter.ConvertFirstSpecification<DME_EnumDescription, object>(list, delegate(DME_EnumDescription des) { return des.enumValue; }, delegate(DME_EnumDescription des) { return des.tag.ToString() == tag.ToString(); });
        } 
        #endregion
		
    }
}
