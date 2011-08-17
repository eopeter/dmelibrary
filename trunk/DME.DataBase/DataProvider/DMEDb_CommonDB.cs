/*
 * ���������
 * 
 * ʹ������ķ����������ݷ���ʵ��,������App.config�����������ã�
 <add key="SqlServerConnectionString" value="Data Source=localhost;Initial catalog=DAABAF;user id=daab;password=daab" />
       <add key="SqlServerHelperAssembly" value="CommonDataProvider.Data"></add>
       <add key="SqlServerHelperType" value="CommonDataProvider.Data.SqlServer"></add>
       <add key="OleDbConnectionString" value="Provider=SQLOLEDB;Data Source=localhost;Initial catalog=DAABAF;user id=daab;password=daab" />
       <add key="OleDbHelperAssembly" value="CommonDataProvider.Data"></add>
       <add key="OleDbHelperType" value="CommonDataProvider.Data.OleDb"></add>
       <add key="OdbcConnectionString" value="DRIVER={SQL Server};SERVER=localhost;DATABASE=DAABAF;UID=daab;PWD=daab;" />
       <add key="OdbcHelperAssembly" value="CommonDataProvider.Data"></add>
       <add key="OdbcHelperType" value="CommonDataProvider.Data.Odbc"></add>
       <add key="OracleConnectionString" value="User ID=DAAB;Password=DAAB;Data Source=spinvis_flash;" />
       <add key="OracleHelperAssembly" value="CommonDataProvider.Data"></add>
       <add key="OracleHelperType" value="CommonDataProvider.Data.Oracle"></add>
        * 
       <add key="SQLiteConnectionString" value="Data Source=spinvis_flash;" />
       <add key="SQLiteHelperAssembly" value="CommonDataProvider.Data"></add>
       <add key="SQLiteHelperType" value="CommonDataProvider.Data.SQLite"></add>

 * �޸��ߣ�         ʱ�䣺2010-3-24                
 * �޸�˵�����ڲ������õ�ʱ�������nullֵ�Ĳ������������ݿ�����NULLֵ��
 * ========================================================================
*/


using System;
using System.Data ;
using System.IO ;
using System.Reflection ;
using DME.DataBase.Common;
using DME.DataBase;

namespace DME.DataBase.DataProvider.Data
{
	/// <summary>
	/// �������ݷ��ʻ�����
	/// </summary>
	public abstract class DMEDb_CommonDB
	{
		private string _connString=string.Empty ;
		private string _errorMessage=string.Empty ;
        private bool _onErrorRollback = true;
        private bool _onErrorThrow = true ;
		private IDbConnection _connection=null;
		private IDbTransaction _transation=null;
        private long _elapsedMilliseconds = 0;

        private  string appRootPath = "";

        private int transCount;//���������
//		//��־���
//		private string DataLogFile ;
//		private bool SaveCommandLog;
		/// <summary>
		/// Ĭ�Ϲ��캯��
		/// </summary>
        public DMEDb_CommonDB()
		{
//			DataLogFile=System.Configuration .ConfigurationSettings .AppSettings ["DataLogFile"];
//			string temp=System.Configuration .ConfigurationSettings .AppSettings ["SaveCommandLog"];
//			if(temp!=null && DataLogFile!=null && DataLogFile!="")
//			{
//				if(temp.ToUpper() =="TRUE") SaveCommandLog=true ;else SaveCommandLog=false;
//			}
		}

        /// <summary>
        /// �������ݿ�ʵ����ȡ���ݿ�����ö��
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static DMEDb_DBMSType GetDBMSType(DMEDb_CommonDB db)
        {
            if (db != null)
            {
                if (db is DMEDb_Access )
                    return DMEDb_DBMSType.Access ;
                if (db is DMEDb_SqlServer)
                    return DMEDb_DBMSType.SqlServer;
                if (db is DMEDb_Oracle)
                    return DMEDb_DBMSType.Oracle;
                if (db is DMEDb_OleDb)
                    return DMEDb_DBMSType.UNKNOWN;
                if (db is DMEDb_Odbc)
                    return DMEDb_DBMSType.UNKNOWN;
            }
            return DMEDb_DBMSType.UNKNOWN;
        }

        /// <summary>
		/// �����������ݷ������ʵ��
		/// </summary>
		/// <param name="providerAssembly">�ṩ���������</param>
		/// <param name="providerType">�ṩ������</param>
		/// <returns></returns>
        public static DMEDb_AdoHelper CreateInstance(string providerAssembly, string providerType)
		{
			Assembly assembly = Assembly.Load( providerAssembly );
			object provider = assembly.CreateInstance( providerType );

            if (provider is DMEDb_AdoHelper)
			{
                return provider as DMEDb_AdoHelper;
			}
			else
			{
                throw new InvalidOperationException("��ǰָ���ĵ��ṩ������ DMEDb_AdoHelper ������ľ���ʵ���࣬��ȷ��Ӧ�ó����������ȷ�����ã���connectionStrings ���ýڵ� providerName ���ԣ���");
			}
		}

        /// <summary>
        /// ��ǰ���ݿ������ö��
        /// </summary>
        public abstract DMEDb_DBMSType CurrentDBMSType{get;}
        /// <summary>
        /// ��ȡ���һ��ִ�в�ѯ�����ķѵ�ʱ�䣬��λ������
        /// </summary>
        public long ElapsedMilliseconds
        {
            get { return _elapsedMilliseconds; }
        }

        private string _insertKey;
        /// <summary>
        /// �ڲ�����������е����ݺ󣬻�ȡ�ղ������е����ݵķ�ʽ��Ĭ��ʹ��SQLSERVER�ķ�ʽ @@IDENTITY���������������ݿ�ʵ���������Ҫ��д�����Ի�������ʱ��ָ̬����
        /// </summary>
        public virtual string InsertKey
        {
            get {
                if (string.IsNullOrEmpty(_insertKey))
                    return "SELECT @@IDENTITY";
                else
                    return _insertKey;
            }
            set {
                _insertKey = value;
            }
        }

		/// <summary>
		/// ���������ַ���
		/// </summary>
		public string ConnectionString
		{
			get{
                return _connString;
            }
			set{
                _connString=value;
                //���� ���·�����ٶ� ~ ·����ʽ����Web��������·��
                //if(!string.IsNullOrEmpty (value) && _connString.IndexOf ('~')>0)
                //{
                //    if (appRootPath == "")
                //    {
                //        string EscapedCodeBase = Assembly.GetExecutingAssembly().EscapedCodeBase;
                //        Uri u = new Uri(EscapedCodeBase);
                //        string path = Path.GetDirectoryName(u.LocalPath);
                //        if (path.Length > 4)
                //            appRootPath = path.Substring(0, path.Length - 3);// ȥ�� \bin����ȡ��Ŀ¼
                //    }
                //    _connString = _connString.Replace("~", appRootPath);
                //}
                DMEDb_CommonUtil.ReplaceWebRootPath(ref _connString);
            }
		}

		/// <summary>
		/// ���ݲ����Ĵ�����Ϣ������ÿ�β�ѯ�������Ϣ��
		/// </summary>
		public string ErrorMessage
		{
			get{return _errorMessage;}
			set{
				if(value!=null && value!="")
					_errorMessage+=";"+value;
				else
					_errorMessage=value;
			}
		}

        /// <summary>
        /// ������ִ���ڼ䣬���¹���������ִ����Ƿ��Զ��ع�����Ĭ��Ϊ�ǡ�
        /// </summary>
        public bool OnErrorRollback
        {
            get { return _onErrorRollback; }
            set { _onErrorRollback = value; }
        }

        /// <summary>
        /// ��ѯ���ִ����Ƿ��ǽ������׳���Ĭ��Ϊ�ǡ�
        /// �������Ϊ�񣬽��򻯵��ó�����쳣������������ÿ�θ��º���Ӱ��Ľ�����ʹ�����Ϣ��������ĳ����߼���
        /// ���������ִ���ڼ䣬�������ִ�������̽������������ñ�����Ϊ �ǡ�
        /// </summary>
        public bool OnErrorThrow
        {
            get { return _onErrorThrow; }
            set { _onErrorThrow = value; }
        }

		/// <summary>
		/// ��ȡ����������������
		/// </summary>
		/// <returns>�����������</returns>
		protected virtual IDbConnection GetConnection() //
		{
			if(Transaction !=null)
			{
				IDbTransaction trans=Transaction ;
				if (trans.Connection!=null)
					return trans.Connection ;
			}
			return null;
		}

        /// <summary>
        /// ��ȡ���ݿ����Ӷ���ʵ��
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetDbConnection()
        {
            return this.GetConnection();
        }

		/// <summary>
		/// ��ȡ�����������ʵ��
		/// </summary>
		/// <param name="connectionString">�����ַ���</param>
		/// <returns>�����������</returns>
		public IDbConnection GetConnection(string connectionString)
		{
			this.ConnectionString=connectionString;
			return this.GetConnection ();
		}

		/// <summary>
		/// ��ȡ����������ʵ��
		/// </summary>
		/// <returns>����������</returns>
		protected abstract IDbDataAdapter  GetDataAdapter(IDbCommand command);

		/// <summary>
		/// ��ȡ���������������
		/// </summary>
		public IDbTransaction Transaction
		{
			get{return _transation;}
			set{_transation=value;}
		}

        /// <summary>
        /// ��ȡ�������ı�ʶ�ַ���Ĭ��ΪSQLSERVER��ʽ������������ݿ��������Ҫ��д������
        /// </summary>
        public virtual string GetParameterChar
        {
            get { return "@"; }
        }

		/// <summary>
		/// ��ȡһ���²�������
		/// </summary>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public abstract IDataParameter GetParameter();

		/// <summary>
		/// ��ȡһ���²�������
		/// </summary>
		/// <param name="paraName">��������</param>
		/// <param name="dbType">���ݿ���������</param>
		/// <param name="size">�ֶδ�С</param>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public abstract IDataParameter GetParameter(string paraName,System.Data.DbType dbType,int size);

        /// <summary>
        /// ��ȡһ���²�������
        /// </summary>
        /// <param name="paraName">��������</param>
        /// <param name="dbType">>���ݿ���������</param>
        /// <returns>�ض�������Դ�Ĳ�������</returns>
        public IDataParameter GetParameter(string paraName, DbType dbType)
        {
            IDataParameter para = this.GetParameter();
            para.ParameterName = paraName;
            para.DbType = dbType;
            return para;
        }

		/// <summary>
		/// ���ݲ�������ֵ���ز���һ���µĲ�������
		/// </summary>
		/// <param name="paraName">������</param>
		/// <param name="Value">����ֵ</param>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public IDataParameter GetParameter(string paraName ,object Value)
		{
			IDataParameter para=this.GetParameter ();
			para.ParameterName=paraName;
			para.Value =Value;
			return para;
		}

		/// <summary>
		/// ��ȡһ���²�������
		/// </summary>
		/// <param name="paraName">������</param>
		/// <param name="dbType">����ֵ</param>
		/// <param name="size">������С</param>
		/// <param name="paraDirection">�����������</param>
		/// <returns>�ض�������Դ�Ĳ�������</returns>
		public IDataParameter GetParameter(string paraName,System.Data.DbType dbType,int size,System.Data.ParameterDirection paraDirection)
		{
			IDataParameter para=this.GetParameter (paraName,dbType,size);
			para.Direction=paraDirection;
			return para;
		}

        /// <summary>
        /// ��ȡһ���²�������
        /// </summary>
        /// <param name="paraName">������</param>
        /// <param name="dbType">��������</param>
        /// <param name="size">����ֵ�ĳ���</param>
        /// <param name="paraDirection">�����������������</param>
        /// <param name="precision">����ֵ�����ľ���</param>
        /// <param name="scale">������С��λλ��</param>
        /// <returns></returns>
        public IDataParameter GetParameter(string paraName, System.Data.DbType dbType, int size, System.Data.ParameterDirection paraDirection, byte  precision, byte  scale)
        {
            IDbDataParameter para = (IDbDataParameter)this.GetParameter(paraName, dbType, size);
            para.Direction = paraDirection;
            para.Precision = precision;
            para.Scale = scale;
            return para;
        }

        /// <summary>
        /// ���ش� SqlConnection ������Դ�ļܹ���Ϣ��
        /// </summary>
        /// <param name="collectionName">�������ƣ�����Ϊ��</param>
        /// <param name="restrictionValues">����ļܹ���һ������ֵ������Ϊ��</param>
        /// <returns>���ݿ�ܹ���Ϣ��</returns>
        public abstract DataTable GetSchema(string collectionName, string[] restrictionValues);

      /// <summary>
        /// ��ȡ�洢���̡������Ķ������ݣ��������֧�֣���Ҫ����������д
        /// </summary>
        /// <param name="spName">�洢��������</param>
        /// <returns></returns>
        public virtual string GetSPDetail(string spName)
        {
            return "";
        }

        /// <summary>
        /// ��ȡ��ͼ���壬�������֧�֣���Ҫ����������д
        /// </summary>
        /// <param name="viewName">��ͼ����</param>
        /// <returns></returns>
        public virtual string GetViweDetail(string viewName)
        {
            return "";
        }

		/// <summary>
		/// �����Ӳ���������
		/// </summary>
		public void BeginTransaction ()
		{
            transCount++;
			this.ErrorMessage ="";
			_connection=GetConnection();//�������н����ȡ���Ӷ���ʵ��
			if(_connection.State!=ConnectionState.Open )
				_connection.Open ();
            if(transCount ==1)
			    _transation=_connection.BeginTransaction ();
            DMEDb_CommandLog.Instance.WriteLog("�����Ӳ���������", "DMEDb_AdoHelper");
		}

		/// <summary>
		/// �ύ���񲢹ر�����
		/// </summary>
		public void Commit()
		{
            transCount--;
			if(_transation!=null && _transation.Connection!=null && transCount ==0) 
				_transation.Commit ();
            if (_connection != null && _connection.State == ConnectionState.Open && transCount == 0)
                _connection.Close();
            DMEDb_CommandLog.Instance.WriteLog("�ύ���񲢹ر�����", "DMEDb_AdoHelper");
		}

		/// <summary>
        /// �ع����񲢹ر�����
		/// </summary>
		public void Rollback()
		{
			if(_transation!=null && _transation.Connection!=null) 
				_transation.Rollback ();
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
            DMEDb_CommandLog.Instance.WriteLog("�ع����񲢹ر�����", "DMEDb_AdoHelper");
		}

        private bool _sqlServerCompatible = true;
        /// <summary>
        /// SQL SERVER ���������ã�Ĭ��Ϊ���ݡ������Կ��Խ�SQLSERVER�������ֲ�������������͵����ݿ⣬�����ֶηָ����ţ����ں����ȡ�
        /// </summary>
        public bool SqlServerCompatible
        {
            get { return _sqlServerCompatible; }
            set { _sqlServerCompatible = value; }
        }
        /// <summary>
        /// ��ӦSQL�����������Ĵ������罫SQLSERVER���ֶ�������������滻�����ݿ��ض����ַ����÷�������ִ�в�ѯǰ���ã�Ĭ������²������κδ���
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        protected virtual  string  PrepareSQL(ref string SQL)
        {
            return SQL;
        }
		/// <summary>
		/// �����������,������������������������ӣ����δ���������ｫ����
		/// </summary>
		/// <param name="cmd">�������</param>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">��������</param>
		/// <param name="parameters">��������</param>
		private void CompleteCommand(IDbCommand cmd,ref string SQL,ref CommandType commandType,ref IDataParameter[] parameters)
		{
            cmd.CommandText =SqlServerCompatible ? PrepareSQL(ref  SQL):SQL ;
			cmd.CommandType =commandType;
			cmd.Transaction =this.Transaction ;

			if(parameters!=null)
				for(int i=0;i<parameters.Length ;i++)
                    if (parameters[i] != null)
                    {
                        if (commandType != CommandType.StoredProcedure)
                        {
                            IDataParameter para = (IDataParameter)((ICloneable)parameters[i]).Clone();
                            if (para.Value == null)
                                para.Value = DBNull.Value;
                            cmd.Parameters.Add(para);
                        }
                        else
                        {
                            //Ϊ�洢���̴��ط���ֵ
                            cmd.Parameters.Add(parameters[i]);
                        }
                    }
                  
			if(cmd.Connection.State!=ConnectionState.Open )
				cmd.Connection.Open ();
			//������־����
			//dth,2008.4.8
			//
//			if(SaveCommandLog )
//				RecordCommandLog(cmd);
			//CommandLog.Instance.WriteLog(cmd,"AdoHelper");
		}

//		/// <summary>
//		/// ��¼������Ϣ
//		/// </summary>
//		/// <param name="command"></param>
//		private void RecordCommandLog(IDbCommand command)
//		{
//			WriteLog("'"+DateTime.Now.ToString ()+ " @AdoHelper ִ�����\rSQL=\""+command.CommandText+"\"\r'�������ͣ�"+command.CommandType.ToString ());
//			if(command.Parameters.Count >0)
//			{
//				WriteLog("'���С�"+command.Parameters.Count+"�������������");
//				for(int i=0;i<command.Parameters.Count ;i++)
//				{
//					IDataParameter p=(IDataParameter)command.Parameters[i];
//					WriteLog ("Parameter["+p.ParameterName+"]=\""+p.Value.ToString ()+"\"  'DbType=" +p.DbType.ToString ());
//				}
//			}
//			WriteLog ("\r\n");
//
//		}
//
//		/// <summary>
//		/// д����־
//		/// </summary>
//		/// <param name="log"></param>
//		private void WriteLog(string log)
//		{
//			StreamWriter sw=File.AppendText (this.DataLogFile );;
//			sw.WriteLine (log);
//			sw.Close ();
//		}

		/// <summary>
        /// ִ�в�����ֵ�Ĳ�ѯ������˲�ѯ�����˴��������� OnErrorThrow ����Ϊ �ǣ����׳����󣻷��򽫷��� -1����ʱ����ErrorMessage���ԣ�
        /// ����˲�ѯ�������в��ҳ����˴��󣬽����� OnErrorRollback ���������Ƿ��Զ��ع�����
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">��������</param>
		/// <param name="parameters">��������</param>
		/// <returns>��Ӱ�������</returns>
		public virtual int ExecuteNonQuery(string SQL,CommandType commandType,IDataParameter[] parameters)
		{
            ErrorMessage = "";
			IDbConnection conn=GetConnection();
			IDbCommand cmd=conn.CreateCommand ();
			CompleteCommand(cmd,ref SQL,ref commandType,ref parameters);

            DMEDb_CommandLog cmdLog = new DMEDb_CommandLog(true);

			int result=-1;
			try
			{
				result=cmd.ExecuteNonQuery ();
				//����������������ϲ�����߾�����ʱ�ύ����
			}
			catch(Exception ex)
			{
				ErrorMessage =ex.Message ;
                bool inTransaction = cmd.Transaction==null? false:true ;

				//�������������ô�˴�Ӧ�û�������
				if(cmd.Transaction!=null && OnErrorRollback )
					cmd.Transaction.Rollback ();

                cmdLog.WriteErrLog(cmd, "DMEDb_AdoHelper:" + ErrorMessage);
                if (OnErrorThrow)
                {
                    throw new DMEDb_QueryException(ex.Message, cmd.CommandText, commandType, parameters, inTransaction, conn.ConnectionString);
                }
			}
			finally
			{
				if(cmd.Transaction==null && conn.State ==ConnectionState.Open )
					conn.Close ();
			}

            cmdLog.WriteLog(cmd, "DMEDb_AdoHelper", out _elapsedMilliseconds);

			return result;
		}

		/// <summary>
        /// ִ�в�����ֵ�Ĳ�ѯ������˲�ѯ�����˴��󣬽����� -1����ʱ����ErrorMessage���ԣ�
        /// ����˲�ѯ�������в��ҳ����˴��󣬽����� OnErrorRollback ���������Ƿ��Զ��ع�����
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>��Ӱ�������</returns>
		public int ExecuteNonQuery(string SQL)
		{
			return ExecuteNonQuery( SQL,CommandType.Text ,null);
		}

		/// <summary>
		/// ִ�в������ݵĲ�ѯ��������Access��SqlServer
		/// </summary>
		/// <param name="SQL">�������ݵ�SQL</param>
		/// <param name="commandType">��������</param>
		/// <param name="parameters">��������</param>
		/// <param name="ID">Ҫ�����ı��β������²��������е�����IDֵ</param>
		/// <returns>���β�ѯ��Ӱ�������</returns>
		public virtual int ExecuteInsertQuery(string SQL,CommandType commandType,IDataParameter[] parameters,ref object ID)
		{
			IDbConnection conn=GetConnection();
			IDbCommand cmd=conn.CreateCommand ();
			CompleteCommand(cmd,ref SQL,ref commandType,ref parameters);

            DMEDb_CommandLog cmdLog = new DMEDb_CommandLog(true);

			bool inner=false;
			int result=-1;
			ID=0;
			try
			{
				if(cmd.Transaction ==null)
				{
					inner=true;
					cmd.Transaction=conn.BeginTransaction ();
				}
			
				result=cmd.ExecuteNonQuery ();
                cmd.CommandText = this.InsertKey;// "SELECT @@IDENTITY";
				ID=cmd.ExecuteScalar ();
				//������ڲ��������������ύ���񣬷����ⲿ�����߾�����ʱ�ύ����
				if(inner)
				{
					cmd.Transaction.Commit ();
					cmd.Transaction=null;
				}
			}
			catch(Exception ex)
			{
				ErrorMessage=ex.Message ;
                bool inTransaction = cmd.Transaction == null ? false : true;
				if(cmd.Transaction!=null)
					cmd.Transaction.Rollback ();
				if(inner)
					cmd.Transaction=null;

                cmdLog.WriteErrLog(cmd, "DMEDb_AdoHelper:" + ErrorMessage);
                if (OnErrorThrow)
                {
                    throw new DMEDb_QueryException(ex.Message, cmd.CommandText , commandType, parameters, inTransaction, conn.ConnectionString);
                }

			}
			finally
			{
				if(cmd.Transaction==null && conn.State ==ConnectionState.Open )
					conn.Close ();
			}

            cmdLog.WriteLog(cmd, "DMEDb_AdoHelper", out _elapsedMilliseconds);

			return result;
		}

		/// <summary>
		/// ִ�в������ݵĲ�ѯ
		/// </summary>
		/// <param name="SQL">�������ݵ�SQL</param>
		/// <param name="ID">Ҫ�����ı��β������²��������е�����IDֵ</param>
		/// <returns>���β�ѯ��Ӱ�������</returns>
		public int ExecuteInsertQuery(string SQL,ref object ID)
		{
			return  ExecuteInsertQuery( SQL,CommandType.Text ,null, ref ID);
		}

		/// <summary>
		/// ִ�з��ص�һֵ�ò�ѯ
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">��������</param>
		/// <param name="parameters">��������</param>
		/// <returns>��ѯ���</returns>
		public virtual object ExecuteScalar(string SQL,CommandType commandType,IDataParameter[] parameters)
		{
			IDbConnection conn=GetConnection();
			IDbCommand cmd=conn.CreateCommand ();
			CompleteCommand(cmd,ref SQL,ref commandType,ref parameters);

            DMEDb_CommandLog cmdLog = new DMEDb_CommandLog(true);

			object result=null;
			try
			{
				result=cmd.ExecuteScalar ();
				//����������������ϲ�����߾�����ʱ�ύ����
			}
			catch(Exception ex)
			{
				ErrorMessage =ex.Message ;
				//�������������ô�˴�Ӧ�û�������
                //if(cmd.Transaction!=null)
                //    cmd.Transaction.Rollback ();

                bool inTransaction = cmd.Transaction == null ? false : true;
                cmdLog.WriteErrLog(cmd, "DMEDb_AdoHelper:" + ErrorMessage);
                if (OnErrorThrow)
                {
                    throw new DMEDb_QueryException(ex.Message, cmd.CommandText, commandType, parameters, inTransaction, conn.ConnectionString);
                }
			}
			finally
			{
				if(cmd.Transaction==null && conn.State ==ConnectionState.Open )
					conn.Close ();
			}

            cmdLog.WriteLog(cmd, "DMEDb_AdoHelper", out _elapsedMilliseconds);

			return result;
		}

		/// <summary>
		/// ִ�з��ص�һֵ�ò�ѯ
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>��ѯ���</returns>
		public object ExecuteScalar(string SQL)
		{
			return ExecuteScalar( SQL,CommandType.Text ,null);
		}

		/// <summary>
		/// ִ�з������ݼ��Ĳ�ѯ
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">��������</param>
		/// <param name="parameters">��������</param>
		/// <returns>���ݼ�</returns>
		public virtual DataSet ExecuteDataSet(string SQL,CommandType commandType,IDataParameter[] parameters)
		{
			IDbConnection conn=GetConnection();
			IDbCommand cmd=conn.CreateCommand ();
			CompleteCommand(cmd,ref SQL,ref commandType,ref parameters);
			IDataAdapter ada=GetDataAdapter(cmd);

            DMEDb_CommandLog cmdLog = new DMEDb_CommandLog(true);

			DataSet ds=new DataSet ();
			try
			{
				ada.Fill(ds);//FillSchema(ds,SchemaType.Mapped )
			}
			catch(Exception ex)
			{
				ErrorMessage=ex.Message ;
                bool inTransaction = cmd.Transaction == null ? false : true;
                cmdLog.WriteErrLog(cmd, "DMEDb_AdoHelper:" + ErrorMessage);
                if (OnErrorThrow)
                {
                    throw new DMEDb_QueryException(ex.Message, cmd.CommandText, commandType, parameters, inTransaction, conn.ConnectionString);
                }
			}
			finally
			{
				if(cmd.Transaction==null && conn.State ==ConnectionState.Open )
					conn.Close ();
			}

            cmdLog.WriteLog(cmd, "DMEDb_AdoHelper", out _elapsedMilliseconds);

			return ds;
		}

        /// <summary>
        /// ִ�з������ݼܹ��Ĳ�ѯ��ע�⣬�������κ���
        /// </summary>
        /// <param name="SQL">SQL</param>
        /// <param name="commandType">��������</param>
        /// <param name="parameters">��������</param>
        /// <returns>���ݼܹ�</returns>
        public virtual DataSet ExecuteDataSetSchema(string SQL, CommandType commandType, IDataParameter[] parameters)
        {
            IDbConnection conn = GetConnection();
            IDbCommand cmd = conn.CreateCommand();
            CompleteCommand(cmd, ref SQL, ref commandType, ref parameters);
            IDataAdapter ada = GetDataAdapter(cmd);

            DataSet ds = new DataSet();
            try
            {
                ada.FillSchema(ds, SchemaType.Mapped);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                bool inTransaction = cmd.Transaction == null ? false : true;
                DMEDb_CommandLog.Instance.WriteErrLog(cmd, "DMEDb_AdoHelper:" + ErrorMessage);
                if (OnErrorThrow)
                {
                    throw new DMEDb_QueryException(ex.Message, cmd.CommandText, commandType, parameters, inTransaction, conn.ConnectionString);
                }
            }
            finally
            {
                if (cmd.Transaction == null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return ds;
        }

		/// <summary>
		/// ִ�з������ݼ��Ĳ�ѯ
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>���ݼ�</returns>
		public DataSet ExecuteDataSet(string SQL)
		{
			return  ExecuteDataSet( SQL, CommandType.Text  ,null);
		}


		/// <summary>
		/// ���ص�һ�е������Ķ���
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>�����Ķ���</returns>
		public IDataReader ExecuteDataReaderWithSingleRow(string SQL)
		{
			IDataParameter[] paras={};
			//���������ʱ���ܹر�����
            return ExecuteDataReaderWithSingleRow(SQL, paras);
		}

        /// <summary>
        /// ���ص�һ�е������Ķ���
        /// </summary>
        /// <param name="SQL">SQL</param>
        /// <param name="paras">����</param>
        /// <returns>�����Ķ���</returns>
        public IDataReader ExecuteDataReaderWithSingleRow(string SQL, IDataParameter[] paras)
        {
            //���������ʱ���ܹر�����
            if (this.Transaction != null)
                return ExecuteDataReader(ref SQL, CommandType.Text, CommandBehavior.SingleRow, ref paras);
            else
                return ExecuteDataReader(ref SQL, CommandType.Text, CommandBehavior.SingleRow | CommandBehavior.CloseConnection, ref paras);
        }

		/// <summary>
		/// ���ݲ�ѯ���������Ķ��������ڷ���������У��Ķ����Ժ���Զ��ر����ݿ�����
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>�����Ķ���</returns>
		public IDataReader ExecuteDataReader(string SQL)
		{
			IDataParameter[] paras={};
            return ExecuteDataReader(ref SQL, CommandType.Text, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, ref paras);
		}

		/// <summary>
		/// ���ݲ�ѯ���������Ķ�������
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="cmdBehavior">�Բ�ѯ�ͷ��ؽ����Ӱ���˵��</param>
		/// <returns>�����Ķ���</returns>
		public IDataReader ExecuteDataReader(string SQL,CommandBehavior cmdBehavior)
		{
			IDataParameter[] paras={};
			return ExecuteDataReader(ref SQL, CommandType.Text , cmdBehavior,ref paras);
		}

		/// <summary>
		/// ���ݲ�ѯ���������Ķ�������
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">��������</param>
		/// <param name="parameters">��������</param>
		/// <returns>�����Ķ���</returns>
		public IDataReader ExecuteDataReader(string SQL,CommandType commandType,IDataParameter[] parameters)
		{
            return ExecuteDataReader(ref SQL, commandType, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, ref parameters);
		}

		/// <summary>
		/// ���ݲ�ѯ���������Ķ�������
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">��������</param>
		/// <param name="cmdBehavior">�Բ�ѯ�ͷ��ؽ����Ӱ���˵��</param>
		/// <param name="parameters">��������</param>
		/// <returns>�����Ķ���</returns>
		protected virtual IDataReader ExecuteDataReader(ref string SQL, CommandType commandType, CommandBehavior cmdBehavior,ref IDataParameter[] parameters)
		{
			IDbConnection conn=GetConnection();
			IDbCommand cmd=conn.CreateCommand ();
			CompleteCommand(cmd,ref SQL,ref commandType,ref parameters);

            DMEDb_CommandLog cmdLog = new DMEDb_CommandLog(true);

			IDataReader reader=null;
			try
			{
				//������������������Ϊ�գ���ôǿ���ڶ�ȡ�����ݺ�ر��Ķ��������ݿ����� 2008.3.20
				if(cmd.Transaction ==null && cmdBehavior==CommandBehavior.Default )
					cmdBehavior=CommandBehavior.CloseConnection ;
				reader = cmd.ExecuteReader (cmdBehavior);
			}
			catch(Exception ex)
			{
				ErrorMessage=ex.Message ;
				//ֻ�г����˴������û�п������񣬿��Թر�����
				if(cmd.Transaction==null && conn.State ==ConnectionState.Open )
					conn.Close ();
				
                bool inTransaction = cmd.Transaction == null ? false : true;
                cmdLog.WriteErrLog(cmd, "DMEDb_AdoHelper:" + ErrorMessage);
                if (OnErrorThrow)
                {
                    throw new DMEDb_QueryException(ex.Message, cmd.CommandText, commandType, parameters, inTransaction, conn.ConnectionString);
                }
			}

            cmdLog.WriteLog(cmd, "DMEDb_AdoHelper", out _elapsedMilliseconds);

			return reader;
		}
		
	}
}
