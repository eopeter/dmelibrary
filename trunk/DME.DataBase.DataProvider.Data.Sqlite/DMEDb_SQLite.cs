using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DME.DataBase.DataProvider.Data
{
    /// <summary>
    /// SQLite 数据访问类 dth,2009.4.1
    /// </summary>
    public sealed class DMEDb_SQLite : DMEDb_AdoHelper   
    {
    /// <summary>
		/// 默认构造函数
		/// </summary>
        public DMEDb_SQLite()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

        /// <summary>
        /// 获取当前数据库类型的枚举
        /// </summary>
        public override DME.DataBase.Common.DMEDb_DBMSType CurrentDBMSType
        {
            get { return DME.DataBase.Common.DMEDb_DBMSType.SQLite; }
        }
		/// <summary>
		/// 创建并且打开数据库连接
		/// </summary>
		/// <returns>数据库连接</returns>
		protected override IDbConnection GetConnection()
		{
			IDbConnection conn=base.GetConnection ();
			if(conn==null)
			{
				conn=new SQLiteConnection (base.ConnectionString );
				//conn.Open ();
			}
			return conn;
		}

		/// <summary>
		/// 获取数据适配器实例
		/// </summary>
		/// <returns>数据适配器</returns>
		protected override IDbDataAdapter  GetDataAdapter(IDbCommand command)
		{
			IDbDataAdapter ada=new SQLiteDataAdapter ((SQLiteCommand )command);
			return ada;
		}

		/// <summary>
		/// 获取一个新参数对象
		/// </summary>
		/// <returns>特定于数据源的参数对象</returns>
		public override IDataParameter GetParameter()
		{
			return new SQLiteParameter ();
		}

		/// <summary>
		///  获取一个新参数对象
		/// </summary>
		/// <param name="paraName">参数名</param>
		/// <param name="dbType">参数数据类型</param>
		/// <param name="size">参数大小</param>
		/// <returns>特定于数据源的参数对象</returns>
		public override IDataParameter GetParameter(string paraName,System.Data.DbType dbType,int size)
		{
			SQLiteParameter para=new SQLiteParameter();
			para.ParameterName=paraName;
			para.DbType=dbType;
			para.Size=size;
			return para;
		}

        /// <summary>
        /// 更新数据（为SQLite重写的支持多线程并发写入功能）
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public override int ExecuteNonQuery(string connectionString, CommandType commandType, string SQL)
        {
            //根据connectionString 缓存每一个写入锁
            return base.ExecuteNonQuery(connectionString, commandType, SQL);
        }

        /// <summary>
        /// 更新数据（为SQLite重写的支持多线程并发写入功能）
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <param name="SQL"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override int ExecuteNonQuery(string connectionString, CommandType commandType, string SQL, IDataParameter[] parameters)
        {
            return base.ExecuteNonQuery(connectionString, commandType, SQL, parameters);
        }


        /// <summary>
        /// 返回此 SQLiteConnection 的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="restrictionValues">请求的架构的一组限制值</param>
        /// <returns>数据库架构信息表</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (SQLiteConnection conn = (SQLiteConnection)this.GetConnection())
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
