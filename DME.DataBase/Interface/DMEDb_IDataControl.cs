using System;
using System.Data;

namespace DME.DataBase.Common
{
	/// <summary>
	/// 数据映射控件接口
	/// </summary>
    public interface DMEDb_IDataControl
	{
		
		/// <summary>
		/// 与数据库数据项相关联的数据
		/// </summary>
		string LinkProperty
		{
			get;
			set;
		}
		
		/// <summary>
		/// 与数据关联的表名
		/// </summary>
		string LinkObject
		{
			get;
			set;
		}

		/// <summary>
		/// 是否通过服务器验证默认为true
		/// </summary>
		bool IsValid
		{
			get;
		}

//		/// <summary>
//		/// 数据类型
//		/// </summary>
//		DbType DataType
//		{
//			get;
//			set;
//		}

		/// <summary>
		/// 数据类型
		/// </summary>
		TypeCode SysTypeCode
		{
			get;
			set;
		}

		/// <summary>
		/// 只读标记
		/// </summary>
		bool ReadOnly
		{
			get;
			set;
		}

		/// <summary>
		/// 是否客户端验证
		/// </summary>
//		bool isClientValidation
//		{
//			get;
//			set;
//		}

		/// <summary>
		/// 是否允许空值
		/// </summary>
		bool isNull
		{
			get;
//			set;
		}

		/// <summary>
		/// 是否是主键
		/// </summary>
		bool PrimaryKey
		{
			get;
			set;
        }

//		object 

//		/// <summary>
//		/// 客户端验证脚本
//		/// </summary>
//		string ClientValidationFunctionString
//		{
//			get;
//			set;
//		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="obj"></param>
		void SetValue(object value);

		/// <summary>
		/// 获取值
		/// </summary>
		/// <returns></returns>
		object GetValue();

		/// <summary>
		/// 服务端验证
		/// </summary>
		/// <returns></returns>
		bool Validate();

	}
}
