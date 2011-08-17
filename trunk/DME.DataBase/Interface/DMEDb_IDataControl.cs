using System;
using System.Data;

namespace DME.DataBase.Common
{
	/// <summary>
	/// ����ӳ��ؼ��ӿ�
	/// </summary>
    public interface DMEDb_IDataControl
	{
		
		/// <summary>
		/// �����ݿ������������������
		/// </summary>
		string LinkProperty
		{
			get;
			set;
		}
		
		/// <summary>
		/// �����ݹ����ı���
		/// </summary>
		string LinkObject
		{
			get;
			set;
		}

		/// <summary>
		/// �Ƿ�ͨ����������֤Ĭ��Ϊtrue
		/// </summary>
		bool IsValid
		{
			get;
		}

//		/// <summary>
//		/// ��������
//		/// </summary>
//		DbType DataType
//		{
//			get;
//			set;
//		}

		/// <summary>
		/// ��������
		/// </summary>
		TypeCode SysTypeCode
		{
			get;
			set;
		}

		/// <summary>
		/// ֻ�����
		/// </summary>
		bool ReadOnly
		{
			get;
			set;
		}

		/// <summary>
		/// �Ƿ�ͻ�����֤
		/// </summary>
//		bool isClientValidation
//		{
//			get;
//			set;
//		}

		/// <summary>
		/// �Ƿ������ֵ
		/// </summary>
		bool isNull
		{
			get;
//			set;
		}

		/// <summary>
		/// �Ƿ�������
		/// </summary>
		bool PrimaryKey
		{
			get;
			set;
        }

//		object 

//		/// <summary>
//		/// �ͻ�����֤�ű�
//		/// </summary>
//		string ClientValidationFunctionString
//		{
//			get;
//			set;
//		}

		/// <summary>
		/// ����ֵ
		/// </summary>
		/// <param name="obj"></param>
		void SetValue(object value);

		/// <summary>
		/// ��ȡֵ
		/// </summary>
		/// <returns></returns>
		object GetValue();

		/// <summary>
		/// �������֤
		/// </summary>
		/// <returns></returns>
		bool Validate();

	}
}
