using System;

namespace DME.DataBase.DataProvider.Adapter
{
	/// <summary>
	/// ���ܴ����������ʹ�øö�ǰ����ȷ����Ӧ�����ݱ��������Ͳ���ʱ���������
	/// </summary>
	public class DMEDb_IBCommand
	{
		string _insertCmd=string.Empty ;
		string _updateCmd=string.Empty ;
		string _tableName=string.Empty ;
		string _selectCmd=string.Empty ;
		string _deleteCmd=string.Empty ;
        string _guidpk = string.Empty;
		int _id=0;

		/// <summary>
		/// Ĭ�Ϲ��캯��
		/// </summary>
		public DMEDb_IBCommand()
		{
		
		}

		/// <summary>
		/// ָ��һ�����ݱ��ʼ������
		/// </summary>
		/// <param name="tableName"></param>
        public DMEDb_IBCommand(string tableName)
		{
			_tableName=tableName;
		}

		/// <summary>
		/// ������������
		/// </summary>
		public string InsertCommand
		{
			get{return _insertCmd ;}
			set{_insertCmd =value; }
		}

		/// <summary>
		/// ������������
		/// </summary>
		public string UpdateCommand
		{
			get{return _updateCmd ;}
			set{_updateCmd =value;}
		}

		/// <summary>
		/// ѡ����������
		/// </summary>
		public string SelectCommand
		{
			get{return _selectCmd ;}
			set{_selectCmd =value;}
		}

		/// <summary>
		/// ɾ����������
		/// </summary>
		public string DeleteCommand
		{
			get{return _deleteCmd ;}
			set{_deleteCmd =value;}
		}

		/// <summary>
		/// ������
		/// </summary>
		public string TableName
		{
			get{return _tableName ;}
			set{_tableName =value;}
		}

		/// <summary>
		/// �����ʶ���������ݿ��������
		/// </summary>
		public int InsertedID
		{
			get{return _id;}
			set{_id=value;}
		}

       
        /// <summary>
        /// GUID ��������
        /// </summary>
        public string GuidPrimaryKey
        {
            get { return _guidpk; }
            set { _guidpk = value; }
        }
	}
}
