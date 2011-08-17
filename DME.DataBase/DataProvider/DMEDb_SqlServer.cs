using System;
using System.Data ;
using System.Data.SqlClient ;

namespace DME.DataBase.DataProvider.Data
{
	/// <summary>
	/// SqlServer ���ݴ���
	/// </summary>
    public sealed class DMEDb_SqlServer : DMEDb_AdoHelper
	{
		/// <summary>
		/// Ĭ�Ϲ��캯��
		/// </summary>
		public DMEDb_SqlServer()
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
            get { return DME.DataBase.Common.DMEDb_DBMSType.SqlServer; }
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
				conn=new SqlConnection (base.ConnectionString );
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
			IDbDataAdapter ada=new SqlDataAdapter ((SqlCommand )command);
			return ada;
		}

		/// <summary>
		/// ��ȡһ���²�������
		/// </summary>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public override IDataParameter GetParameter()
		{
			return new SqlParameter ();
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
			SqlParameter para=new SqlParameter();
			para.ParameterName=paraName;
			para.DbType=dbType;
			para.Size=size;
			return para;
		}

        /// <summary>
        /// ���ش� SqlConnection ������Դ�ļܹ���Ϣ��
        /// </summary>
        /// <param name="collectionName">��������</param>
        /// <param name="restrictionValues">����ļܹ���һ������ֵ</param>
        /// <returns>���ݿ�ܹ���Ϣ��</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (SqlConnection conn = (SqlConnection)this.GetConnection())
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
        /// ��ȡ�洢���̵Ķ�������
        /// </summary>
        /// <param name="spName">�洢��������</param>
        public override  string GetSPDetail(string spName)
        {
            string value = "";
            DataSet ds = this.ExecuteDataSet("sp_helptext", CommandType.StoredProcedure, 
                new IDataParameter[] { this.GetParameter("@objname", spName) });
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    value += dt.Rows[i][0].ToString();
                }
            }
            else
                value = "nothing";
            return value;
        }

        /// <summary>
        /// ��ȡ��ͼ���壬�������֧�֣���Ҫ����������д
        /// </summary>
        /// <param name="viewName">��ͼ����</param>
        /// <returns></returns>
        public override  string GetViweDetail(string viewName)
        {
            return GetSPDetail(viewName);
        }
		
		#region ��������Ĳ�ѯ

//		/// <summary>
//		/// ִ�в�����ֵ�ò�ѯ
//		/// </summary>
//		/// <param name="SQL">SQL</param>
//		/// <returns>��Ӱ�������</returns>
//		public override int ExecuteNonQuery(string SQL)
//		{
//			SqlConnection conn=(SqlConnection)GetConnection();// new SqlConnection (base.ConnectionString );
//			SqlCommand cmd=new SqlCommand (SQL,conn);
//			
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
//			SqlConnection conn=new SqlConnection (base.ConnectionString );
//			SqlCommand cmd=new SqlCommand (SQL,conn);
//			SqlTransaction trans=null;//=conn.BeginTransaction ();
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
//			SqlConnection conn=new SqlConnection (base.ConnectionString );
//			SqlDataAdapter ada =new SqlDataAdapter (SQL,conn);
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
//			SqlConnection conn=new SqlConnection (base.ConnectionString );
//			SqlCommand cmd=new SqlCommand (SQL,conn);
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

#endregion

        /// <summary>
        /// SQL��������
        /// </summary>
        /// <param name="sourceReader">����Դ��DataReader</param>
        /// <param name="connectionString">Ŀ�����ݿ�������ַ���</param>
        /// <param name="destinationTableName">Ҫ�����Ŀ�������</param>
        /// <param name="batchSize">ÿ����������Ĵ�С</param>
        public static void BulkCopy(IDataReader sourceReader,string connectionString, string destinationTableName,int batchSize)
        {
            // Ŀ�� 
            using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            {
                // ������ 
                destinationConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    bulkCopy.BatchSize = batchSize;

                    bulkCopy.DestinationTableName = destinationTableName;
                    bulkCopy.WriteToServer(sourceReader);
                }
            }
            sourceReader.Close();
        }

        /// <summary>
        /// SQL��������
        /// </summary>
        /// <param name="sourceTable">����Դ��</param>
        /// <param name="connectionString">Ŀ�����ݿ�������ַ���</param>
        /// <param name="destinationTableName">Ҫ�����Ŀ�������</param>
        /// <param name="batchSize">ÿ����������Ĵ�С</param>
        public static void BulkCopy(DataTable sourceTable, string connectionString, string destinationTableName, int batchSize)
        {
            using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            {
                // ������ 
                destinationConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    
                    bulkCopy.BatchSize = batchSize;
                    
                    bulkCopy.DestinationTableName = destinationTableName;
                    bulkCopy.WriteToServer(sourceTable);
                }
            }
        }

	}
}
