/*
 * 该类的作用
 * 
 * 使用下面的方法创建数据访问实例,可以在App.config中作如下配置：
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

 * 修改者：         时间：2010-3-24                
 * 修改说明：在参数设置的时候，如果有null值的参数，将在数据库设置NULL值。
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
	/// 公共数据访问基础类
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

        private int transCount;//事务计数器
//		//日志相关
//		private string DataLogFile ;
//		private bool SaveCommandLog;
		/// <summary>
		/// 默认构造函数
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
        /// 根据数据库实例获取数据库类型枚举
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
		/// 创建公共数据访问类的实例
		/// </summary>
		/// <param name="providerAssembly">提供这程序集名称</param>
		/// <param name="providerType">提供者类型</param>
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
                throw new InvalidOperationException("当前指定的的提供程序不是 DMEDb_AdoHelper 抽象类的具体实现类，请确保应用程序进行了正确的配置（如connectionStrings 配置节的 providerName 属性）。");
			}
		}

        /// <summary>
        /// 当前数据库的类型枚举
        /// </summary>
        public abstract DMEDb_DBMSType CurrentDBMSType{get;}
        /// <summary>
        /// 获取最近一次执行查询的所耗费的时间，单位：毫秒
        /// </summary>
        public long ElapsedMilliseconds
        {
            get { return _elapsedMilliseconds; }
        }

        private string _insertKey;
        /// <summary>
        /// 在插入具有自增列的数据后，获取刚才自增列的数据的方式，默认使用SQLSERVER的方式 @@IDENTITY，在其它具体数据库实现类可能需要重写该属性或者运行时动态指定。
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
		/// 数据连结字符串
		/// </summary>
		public string ConnectionString
		{
			get{
                return _connString;
            }
			set{
                _connString=value;
                //处理 相对路径，假定 ~ 路径格式就是Web程序的相对路径
                //if(!string.IsNullOrEmpty (value) && _connString.IndexOf ('~')>0)
                //{
                //    if (appRootPath == "")
                //    {
                //        string EscapedCodeBase = Assembly.GetExecutingAssembly().EscapedCodeBase;
                //        Uri u = new Uri(EscapedCodeBase);
                //        string path = Path.GetDirectoryName(u.LocalPath);
                //        if (path.Length > 4)
                //            appRootPath = path.Substring(0, path.Length - 3);// 去除 \bin，获取根目录
                //    }
                //    _connString = _connString.Replace("~", appRootPath);
                //}
                DMEDb_CommonUtil.ReplaceWebRootPath(ref _connString);
            }
		}

		/// <summary>
		/// 数据操作的错误信息，请在每次查询后检查该信息。
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
        /// 在事务执行期间，更新过程如果出现错误，是否自动回滚事务。默认为是。
        /// </summary>
        public bool OnErrorRollback
        {
            get { return _onErrorRollback; }
            set { _onErrorRollback = value; }
        }

        /// <summary>
        /// 查询出现错误是否是将错误抛出。默认为是。
        /// 如果设置为否，将简化调用程序的异常处理，但是请检查每次更新后受影响的结果数和错误信息来决定你的程序逻辑。
        /// 如果在事务执行期间，期望出现错误后立刻结束处理，请设置本属性为 是。
        /// </summary>
        public bool OnErrorThrow
        {
            get { return _onErrorThrow; }
            set { _onErrorThrow = value; }
        }

		/// <summary>
		/// 获取事务的数据连结对象
		/// </summary>
		/// <returns>数据连结对象</returns>
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
        /// 获取数据库连接对象实例
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetDbConnection()
        {
            return this.GetConnection();
        }

		/// <summary>
		/// 获取数据连结对象实例
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		/// <returns>数据连结对象</returns>
		public IDbConnection GetConnection(string connectionString)
		{
			this.ConnectionString=connectionString;
			return this.GetConnection ();
		}

		/// <summary>
		/// 获取数据适配器实例
		/// </summary>
		/// <returns>数据适配器</returns>
		protected abstract IDbDataAdapter  GetDataAdapter(IDbCommand command);

		/// <summary>
		/// 获取或者设置事务对象
		/// </summary>
		public IDbTransaction Transaction
		{
			get{return _transation;}
			set{_transation=value;}
		}

        /// <summary>
        /// 获取参数名的标识字符，默认为SQLSERVER格式，如果其它数据库则可能需要重写该属性
        /// </summary>
        public virtual string GetParameterChar
        {
            get { return "@"; }
        }

		/// <summary>
		/// 获取一个新参数对象
		/// </summary>
		/// <returns>特定于数据源的参数对象</returns>
		public abstract IDataParameter GetParameter();

		/// <summary>
		/// 获取一个新参数对象
		/// </summary>
		/// <param name="paraName">参数名字</param>
		/// <param name="dbType">数据库数据类型</param>
		/// <param name="size">字段大小</param>
		/// <returns>特定于数据源的参数对象</returns>
		public abstract IDataParameter GetParameter(string paraName,System.Data.DbType dbType,int size);

        /// <summary>
        /// 获取一个新参数对象
        /// </summary>
        /// <param name="paraName">参数名字</param>
        /// <param name="dbType">>数据库数据类型</param>
        /// <returns>特定于数据源的参数对象</returns>
        public IDataParameter GetParameter(string paraName, DbType dbType)
        {
            IDataParameter para = this.GetParameter();
            para.ParameterName = paraName;
            para.DbType = dbType;
            return para;
        }

		/// <summary>
		/// 根据参数名和值返回参数一个新的参数对象
		/// </summary>
		/// <param name="paraName">参数名</param>
		/// <param name="Value">参数值</param>
		/// <returns>特定于数据源的参数对象</returns>
		public IDataParameter GetParameter(string paraName ,object Value)
		{
			IDataParameter para=this.GetParameter ();
			para.ParameterName=paraName;
			para.Value =Value;
			return para;
		}

		/// <summary>
		/// 获取一个新参数对象
		/// </summary>
		/// <param name="paraName">参数名</param>
		/// <param name="dbType">参数值</param>
		/// <param name="size">参数大小</param>
		/// <param name="paraDirection">参数输出类型</param>
		/// <returns>特定于数据源的参数对象</returns>
		public IDataParameter GetParameter(string paraName,System.Data.DbType dbType,int size,System.Data.ParameterDirection paraDirection)
		{
			IDataParameter para=this.GetParameter (paraName,dbType,size);
			para.Direction=paraDirection;
			return para;
		}

        /// <summary>
        /// 获取一个新参数对象
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="size">参数值的长度</param>
        /// <param name="paraDirection">参数的输入输出类型</param>
        /// <param name="precision">参数值参数的精度</param>
        /// <param name="scale">参数的小数位位数</param>
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
        /// 返回此 SqlConnection 的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">集合名称，可以为空</param>
        /// <param name="restrictionValues">请求的架构的一组限制值，可以为空</param>
        /// <returns>数据库架构信息表</returns>
        public abstract DataTable GetSchema(string collectionName, string[] restrictionValues);

      /// <summary>
        /// 获取存储过程、函数的定义内容，如果子类支持，需要在子类中重写
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <returns></returns>
        public virtual string GetSPDetail(string spName)
        {
            return "";
        }

        /// <summary>
        /// 获取视图定义，如果子类支持，需要在子类中重写
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns></returns>
        public virtual string GetViweDetail(string viewName)
        {
            return "";
        }

		/// <summary>
		/// 打开连接并开启事务
		/// </summary>
		public void BeginTransaction ()
		{
            transCount++;
			this.ErrorMessage ="";
			_connection=GetConnection();//在子类中将会获取连接对象实例
			if(_connection.State!=ConnectionState.Open )
				_connection.Open ();
            if(transCount ==1)
			    _transation=_connection.BeginTransaction ();
            DMEDb_CommandLog.Instance.WriteLog("打开连接并开启事务", "DMEDb_AdoHelper");
		}

		/// <summary>
		/// 提交事务并关闭连接
		/// </summary>
		public void Commit()
		{
            transCount--;
			if(_transation!=null && _transation.Connection!=null && transCount ==0) 
				_transation.Commit ();
            if (_connection != null && _connection.State == ConnectionState.Open && transCount == 0)
                _connection.Close();
            DMEDb_CommandLog.Instance.WriteLog("提交事务并关闭连接", "DMEDb_AdoHelper");
		}

		/// <summary>
        /// 回滚事务并关闭连接
		/// </summary>
		public void Rollback()
		{
			if(_transation!=null && _transation.Connection!=null) 
				_transation.Rollback ();
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
            DMEDb_CommandLog.Instance.WriteLog("回滚事务并关闭连接", "DMEDb_AdoHelper");
		}

        private bool _sqlServerCompatible = true;
        /// <summary>
        /// SQL SERVER 兼容性设置，默认为兼容。该特性可以将SQLSERVER的语句移植到其它其它类型的数据库，例如字段分隔符号，日期函数等。
        /// </summary>
        public bool SqlServerCompatible
        {
            get { return _sqlServerCompatible; }
            set { _sqlServerCompatible = value; }
        }
        /// <summary>
        /// 对应SQL语句进行其它的处理，例如将SQLSERVER的字段名外的中括号替换成数据库特定的字符。该方法会在执行查询前调用，默认情况下不进行任何处理。
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        protected virtual  string  PrepareSQL(ref string SQL)
        {
            return SQL;
        }
		/// <summary>
		/// 完善命令对象,处理命令对象关联的事务和连接，如果未打开连接这里将打开它
		/// </summary>
		/// <param name="cmd">命令对象</param>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">命令类型</param>
		/// <param name="parameters">参数数组</param>
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
                            //为存储过程带回返回值
                            cmd.Parameters.Add(parameters[i]);
                        }
                    }
                  
			if(cmd.Connection.State!=ConnectionState.Open )
				cmd.Connection.Open ();
			//增加日志处理
			//dth,2008.4.8
			//
//			if(SaveCommandLog )
//				RecordCommandLog(cmd);
			//CommandLog.Instance.WriteLog(cmd,"AdoHelper");
		}

//		/// <summary>
//		/// 记录命令信息
//		/// </summary>
//		/// <param name="command"></param>
//		private void RecordCommandLog(IDbCommand command)
//		{
//			WriteLog("'"+DateTime.Now.ToString ()+ " @AdoHelper 执行命令：\rSQL=\""+command.CommandText+"\"\r'命令类型："+command.CommandType.ToString ());
//			if(command.Parameters.Count >0)
//			{
//				WriteLog("'共有　"+command.Parameters.Count+"　个命令参数：");
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
//		/// 写入日志
//		/// </summary>
//		/// <param name="log"></param>
//		private void WriteLog(string log)
//		{
//			StreamWriter sw=File.AppendText (this.DataLogFile );;
//			sw.WriteLine (log);
//			sw.Close ();
//		}

		/// <summary>
        /// 执行不返回值的查询，如果此查询出现了错误并且设置 OnErrorThrow 属性为 是，将抛出错误；否则将返回 -1，此时请检查ErrorMessage属性；
        /// 如果此查询在事务中并且出现了错误，将根据 OnErrorRollback 属性设置是否自动回滚事务。
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">命令类型</param>
		/// <param name="parameters">参数数组</param>
		/// <returns>受影响的行数</returns>
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
				//如果开启事务，则由上层调用者决定何时提交事务
			}
			catch(Exception ex)
			{
				ErrorMessage =ex.Message ;
                bool inTransaction = cmd.Transaction==null? false:true ;

				//如果开启事务，那么此处应该回退事务
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
        /// 执行不返回值的查询，如果此查询出现了错误，将返回 -1，此时请检查ErrorMessage属性；
        /// 如果此查询在事务中并且出现了错误，将根据 OnErrorRollback 属性设置是否自动回滚事务。
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>受影响的行数</returns>
		public int ExecuteNonQuery(string SQL)
		{
			return ExecuteNonQuery( SQL,CommandType.Text ,null);
		}

		/// <summary>
		/// 执行插入数据的查询，仅限于Access，SqlServer
		/// </summary>
		/// <param name="SQL">插入数据的SQL</param>
		/// <param name="commandType">命令类型</param>
		/// <param name="parameters">参数数组</param>
		/// <param name="ID">要传出的本次操作的新插入数据行的主键ID值</param>
		/// <returns>本次查询受影响的行数</returns>
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
				//如果在内部开启了事务则提交事务，否则外部调用者决定何时提交事务
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
		/// 执行插入数据的查询
		/// </summary>
		/// <param name="SQL">插入数据的SQL</param>
		/// <param name="ID">要传出的本次操作的新插入数据行的主键ID值</param>
		/// <returns>本次查询受影响的行数</returns>
		public int ExecuteInsertQuery(string SQL,ref object ID)
		{
			return  ExecuteInsertQuery( SQL,CommandType.Text ,null, ref ID);
		}

		/// <summary>
		/// 执行返回单一值得查询
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">命令类型</param>
		/// <param name="parameters">参数数组</param>
		/// <returns>查询结果</returns>
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
				//如果开启事务，则由上层调用者决定何时提交事务
			}
			catch(Exception ex)
			{
				ErrorMessage =ex.Message ;
				//如果开启事务，那么此处应该回退事务
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
		/// 执行返回单一值得查询
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>查询结果</returns>
		public object ExecuteScalar(string SQL)
		{
			return ExecuteScalar( SQL,CommandType.Text ,null);
		}

		/// <summary>
		/// 执行返回数据集的查询
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">命令类型</param>
		/// <param name="parameters">参数数组</param>
		/// <returns>数据集</returns>
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
        /// 执行返回数据架构的查询，注意，不返回任何行
        /// </summary>
        /// <param name="SQL">SQL</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>数据架构</returns>
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
		/// 执行返回数据集的查询
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>数据集</returns>
		public DataSet ExecuteDataSet(string SQL)
		{
			return  ExecuteDataSet( SQL, CommandType.Text  ,null);
		}


		/// <summary>
		/// 返回单一行的数据阅读器
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>数据阅读器</returns>
		public IDataReader ExecuteDataReaderWithSingleRow(string SQL)
		{
			IDataParameter[] paras={};
			//在有事务的时候不能关闭连接
            return ExecuteDataReaderWithSingleRow(SQL, paras);
		}

        /// <summary>
        /// 返回单一行的数据阅读器
        /// </summary>
        /// <param name="SQL">SQL</param>
        /// <param name="paras">参数</param>
        /// <returns>数据阅读器</returns>
        public IDataReader ExecuteDataReaderWithSingleRow(string SQL, IDataParameter[] paras)
        {
            //在有事务的时候不能关闭连接
            if (this.Transaction != null)
                return ExecuteDataReader(ref SQL, CommandType.Text, CommandBehavior.SingleRow, ref paras);
            else
                return ExecuteDataReader(ref SQL, CommandType.Text, CommandBehavior.SingleRow | CommandBehavior.CloseConnection, ref paras);
        }

		/// <summary>
		/// 根据查询返回数据阅读器对象，在非事务过程中，阅读完以后会自动关闭数据库连接
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <returns>数据阅读器</returns>
		public IDataReader ExecuteDataReader(string SQL)
		{
			IDataParameter[] paras={};
            return ExecuteDataReader(ref SQL, CommandType.Text, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, ref paras);
		}

		/// <summary>
		/// 根据查询返回数据阅读器对象
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="cmdBehavior">对查询和返回结果有影响的说明</param>
		/// <returns>数据阅读器</returns>
		public IDataReader ExecuteDataReader(string SQL,CommandBehavior cmdBehavior)
		{
			IDataParameter[] paras={};
			return ExecuteDataReader(ref SQL, CommandType.Text , cmdBehavior,ref paras);
		}

		/// <summary>
		/// 根据查询返回数据阅读器对象
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">命令类型</param>
		/// <param name="parameters">参数数组</param>
		/// <returns>数据阅读器</returns>
		public IDataReader ExecuteDataReader(string SQL,CommandType commandType,IDataParameter[] parameters)
		{
            return ExecuteDataReader(ref SQL, commandType, CommandBehavior.SingleResult | CommandBehavior.CloseConnection, ref parameters);
		}

		/// <summary>
		/// 根据查询返回数据阅读器对象
		/// </summary>
		/// <param name="SQL">SQL</param>
		/// <param name="commandType">命令类型</param>
		/// <param name="cmdBehavior">对查询和返回结果有影响的说明</param>
		/// <param name="parameters">参数数组</param>
		/// <returns>数据阅读器</returns>
		protected virtual IDataReader ExecuteDataReader(ref string SQL, CommandType commandType, CommandBehavior cmdBehavior,ref IDataParameter[] parameters)
		{
			IDbConnection conn=GetConnection();
			IDbCommand cmd=conn.CreateCommand ();
			CompleteCommand(cmd,ref SQL,ref commandType,ref parameters);

            DMEDb_CommandLog cmdLog = new DMEDb_CommandLog(true);

			IDataReader reader=null;
			try
			{
				//如果命令对象的事务对象为空，那么强制在读取完数据后关闭阅读器的数据库连接 2008.3.20
				if(cmd.Transaction ==null && cmdBehavior==CommandBehavior.Default )
					cmdBehavior=CommandBehavior.CloseConnection ;
				reader = cmd.ExecuteReader (cmdBehavior);
			}
			catch(Exception ex)
			{
				ErrorMessage=ex.Message ;
				//只有出现了错误而且没有开启事务，可以关闭连结
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
