using System;
using System.Data ;
using System.Data.OracleClient;

namespace DME.DataBase.DataProvider.Data
{
	/// <summary>
	/// OracleServer ���ݴ���
	/// </summary>
    public sealed class DMEDb_Oracle : DMEDb_AdoHelper
	{
		/// <summary>
		/// Ĭ�Ϲ��캯��
		/// </summary>
		public DMEDb_Oracle()
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
            get { return DME.DataBase.Common.DMEDb_DBMSType.Oracle; }
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
				conn=new OracleConnection (base.ConnectionString );
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
			IDbDataAdapter ada=new OracleDataAdapter ((OracleCommand )command);
			return ada;
		}

		/// <summary>
		/// ��ȡһ���²�������
		/// </summary>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public override IDataParameter GetParameter()
		{
			return new OracleParameter ();
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
			OracleParameter para=new OracleParameter();
			para.ParameterName=paraName;
			para.DbType=dbType;
			para.Size=size;
			return para;
		}

        /// <summary>
        /// ���ش� OracleConnection ������Դ�ļܹ���Ϣ��
        /// </summary>
        /// <param name="collectionName">��������</param>
        /// <param name="restrictionValues">����ļܹ���һ������ֵ</param>
        /// <returns>���ݿ�ܹ���Ϣ��</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (OracleConnection conn = (OracleConnection)this.GetConnection())
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

        /// <summary>
        /// Ԥ����SQL��䣬����в��ܰ���"["��"]"���������ţ������Ҫ����ʹ�ò�������ѯ��
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        protected override string PrepareSQL(ref string SQL)
        {
            return SQL.Replace("[", "\"").Replace("]", "\"");
        }
//		/// <summary>
//		/// ִ�в�����ֵ�ò�ѯ
//		/// </summary>
//		/// <param name="SQL">SQL</param>
//		/// <returns>��Ӱ�������</returns>
//		public override int ExecuteNonQuery(string SQL)
//		{
//			OracleConnection conn=new OracleConnection (base.ConnectionString );
//			OracleCommand cmd=new OracleCommand (SQL,conn);
//			conn.Open ();
//			int result=0;
//			try
//			{
//				result=cmd.ExecuteNonQuery ();
//			}
//			catch(Exception ex)
//			{
//				base.ErrorMessage =ex.Message ;
//			}
//			finally
//			{
//				if(conn.State ==ConnectionState.Open )
//					conn.Close ();
//			}
//			return result;
//		}
//
//		/// <summary>
//		/// ִ�в������ݵĲ�ѯ
//		/// </summary>
//		/// <param name="SQL">�������ݵ�SQL</param>
//		/// <param name="ID">Ҫ�����ı��β������²��������е�����IDֵ</param>
//		/// <returns>���β�ѯ��Ӱ�������</returns>
//		public override int ExecuteInsertQuery(string SQL,ref int ID)
//		{
//			OracleConnection conn=new OracleConnection (base.ConnectionString );
//			OracleCommand cmd=new OracleCommand (SQL,conn);
//			OracleTransaction trans=null;//=conn.BeginTransaction ();
//			conn.Open ();
//			int result=0;
//			ID=0;
//			try
//			{
//				trans=conn.BeginTransaction ();
//				cmd.Transaction =trans;
//				result=cmd.ExecuteNonQuery ();
//				cmd.CommandText ="SELECT @@IDENTITY";
//				//ID=(int)(cmd.ExecuteScalar ());//����
//				object obj=cmd.ExecuteScalar ();
//				ID=Convert.ToInt32 (obj);
//				trans.Commit ();
//			}
//			catch(Exception ex)
//			{
//				base.ErrorMessage=ex.Message ;
//				if(trans!=null)
//					trans.Rollback ();
//			}
//			finally
//			{
//				if(conn.State ==ConnectionState.Open )
//					conn.Close ();
//			}
//			return result;
//		}
//
//		/// <summary>
//		/// ִ�з������ݼ��Ĳ�ѯ
//		/// </summary>
//		/// <param name="SQL">SQL</param>
//		/// <returns>���ݼ�</returns>
//		public override DataSet ExecuteDataSet(string SQL)
//		{
//			OracleConnection conn=new OracleConnection (base.ConnectionString );
//			OracleDataAdapter ada =new OracleDataAdapter (SQL,conn);
//			DataSet ds=new DataSet ();
//			try
//			{
//				ada.Fill (ds);
//			}
//			catch(Exception ex)
//			{
//				base.ErrorMessage=ex.Message ;
//			}
//			finally
//			{
//				if(conn.State ==ConnectionState.Open )
//					conn.Close ();
//			}
//			return ds;
//		}
//
//		/// <summary>
//		/// ���ص�һ�е������Ķ���
//		/// </summary>
//		/// <param name="SQL">SQL</param>
//		/// <returns>�����Ķ���</returns>
//		public override IDataReader ExecuteDataReaderWithSingleRow(string SQL)
//		{
//			OracleConnection conn=new OracleConnection (base.ConnectionString );
//			OracleCommand cmd=new OracleCommand (SQL,conn);
//			IDataReader reader=null;
//			try
//			{
//				conn.Open ();
//				return cmd.ExecuteReader (CommandBehavior.SingleRow | CommandBehavior.CloseConnection );
//			}
//			catch(Exception ex)
//			{
//				base.ErrorMessage=ex.Message ;
//				if(conn.State ==ConnectionState.Open )
//					conn.Close ();
//			}
//			return reader;
//			
//		}

	}
}
