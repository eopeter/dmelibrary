using System;

namespace DME.DataBase.DataProvider.Adapter
{
	/// <summary>
	/// 智能窗体命令对象，使用该对前必须确保对应的数据表有主建和插入时候的自增列
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
		/// 默认构造函数
		/// </summary>
		public DMEDb_IBCommand()
		{
		
		}

		/// <summary>
		/// 指定一个数据表初始化该类
		/// </summary>
		/// <param name="tableName"></param>
        public DMEDb_IBCommand(string tableName)
		{
			_tableName=tableName;
		}

		/// <summary>
		/// 插入数据命令
		/// </summary>
		public string InsertCommand
		{
			get{return _insertCmd ;}
			set{_insertCmd =value; }
		}

		/// <summary>
		/// 更新数据命令
		/// </summary>
		public string UpdateCommand
		{
			get{return _updateCmd ;}
			set{_updateCmd =value;}
		}

		/// <summary>
		/// 选择数据命令
		/// </summary>
		public string SelectCommand
		{
			get{return _selectCmd ;}
			set{_selectCmd =value;}
		}

		/// <summary>
		/// 删除数据命令
		/// </summary>
		public string DeleteCommand
		{
			get{return _deleteCmd ;}
			set{_deleteCmd =value;}
		}

		/// <summary>
		/// 表名称
		/// </summary>
		public string TableName
		{
			get{return _tableName ;}
			set{_tableName =value;}
		}

		/// <summary>
		/// 插入标识，用于数据库的自增列
		/// </summary>
		public int InsertedID
		{
			get{return _id;}
			set{_id=value;}
		}

       
        /// <summary>
        /// GUID 主键名称
        /// </summary>
        public string GuidPrimaryKey
        {
            get { return _guidpk; }
            set { _guidpk = value; }
        }
	}
}
