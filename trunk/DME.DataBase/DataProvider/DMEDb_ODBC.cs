using System;
using System.Data ;
using System.Data.Odbc ;

namespace DME.DataBase.DataProvider.Data
{
	/// <summary>
	/// ODBC ���ݷ�����
	/// </summary>
    public sealed class DMEDb_Odbc : DMEDb_AdoHelper
	{
		/// <summary>
		/// Ĭ�Ϲ��캯��
		/// </summary>
		public DMEDb_Odbc()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
        /// <summary>
        /// ��ȡ��ǰ���ݿ����͵�ö��
        /// </summary>
        public override DME.DataBase.Common.DMEDb_DBMSType CurrentDBMSType
        {
            get { return DME.DataBase.Common.DMEDb_DBMSType.UNKNOWN; }
        }

		/// <summary>
		/// �������Ҵ����ݿ�����
		/// </summary>
		/// <returns>���ݿ�����</returns>
		protected override IDbConnection GetConnection()
		{
			IDbConnection conn=base.GetConnection ();
			if(conn==null)
			{
				conn=new OdbcConnection (base.ConnectionString );
				//conn.Open ();
			}
			return conn;
		}

		/// <summary>
		/// ��ȡ����������ʵ��
		/// </summary>
		/// <returns>����������</returns>
		protected override IDbDataAdapter  GetDataAdapter(IDbCommand command)
		{
			IDbDataAdapter ada=new OdbcDataAdapter ((OdbcCommand )command);
			return ada;
		}

		/// <summary>
		/// ��ȡһ���²�������
		/// </summary>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public override IDataParameter GetParameter()
		{
			return new OdbcParameter ();
		}

		/// <summary>
		///  ��ȡһ���²�������
		/// </summary>
		/// <param name="paraName">������</param>
		/// <param name="dbType">������������</param>
		/// <param name="size">������С</param>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public override IDataParameter GetParameter(string paraName,System.Data.DbType dbType,int size)
		{
			OdbcParameter para=new OdbcParameter();
			para.ParameterName=paraName;
			para.DbType=dbType;
			para.Size=size;
			return para;
		}

        /// <summary>
        /// ���ش� OdbcConnection ������Դ�ļܹ���Ϣ��
        /// </summary>
        /// <param name="collectionName">��������</param>
        /// <param name="restrictionValues">����ļܹ���һ������ֵ</param>
        /// <returns>���ݿ�ܹ���Ϣ��</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (OdbcConnection conn = (OdbcConnection)this.GetConnection())
            {
                conn.Open();
                if (restrictionValues == null && string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema();
                else if (restrictionValues == null && !string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema(collectionName);
                else
                    return conn.GetSchema(collectionName, restrictionValues);
            }
            
        }
	}
}
