using System;
using System.Data;
using System.Data.SqlServerCe;

namespace DME.DataBase.DataProvider.Data
{
    /// <summary>
    /// SqlServerCe 数据处理
    /// </summary>
    public sealed class DMEDb_SqlServerCe : DMEDb_AdoHelper
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DMEDb_SqlServerCe()
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
            get { return DME.DataBase.Common.DMEDb_DBMSType.SqlServerCe; }
        }

        /// <summary>
        /// 创建并且打开数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        protected override IDbConnection GetConnection()
        {
            IDbConnection conn = base.GetConnection();
            if (conn == null)
            {
                conn = new SqlCeConnection(base.ConnectionString);
                //conn.Open ();
            }
            return conn;
        }

        /// <summary>
        /// 获取数据适配器实例
        /// </summary>
        /// <returns>数据适配器</returns>
        protected override IDbDataAdapter GetDataAdapter(IDbCommand command)
        {
            IDbDataAdapter ada = new SqlCeDataAdapter((SqlCeCommand)command);
            return ada;
        }

        /// <summary>
        /// 获取一个新参数对象
        /// </summary>
        /// <returns>特定于数据源的参数对象</returns>
        public override IDataParameter GetParameter()
        {
            return new SqlCeParameter();
        }

        /// <summary>
        ///  获取一个新参数对象
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="dbType">参数数据类型</param>
        /// <param name="size">参数大小</param>
        /// <returns>特定于数据源的参数对象</returns>
        public override IDataParameter GetParameter(string paraName, System.Data.DbType dbType, int size)
        {
            SqlCeParameter para = new SqlCeParameter();
            para.ParameterName = paraName;
            para.DbType = dbType;
            para.Size = size;
            return para;
        }

        /// <summary>
        /// 返回此 SqlConnection 的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="restrictionValues">请求的架构的一组限制值</param>
        /// <returns>数据库架构信息表</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            //using (SqlCeConnection conn = (SqlCeConnection)this.GetConnection())
            //{
            //    //下面的方法默认不支持。
            //    //conn.Open();
            //    //if (restrictionValues == null && string.IsNullOrEmpty(collectionName))
            //    //    return conn.GetSchema();
            //    //else if (restrictionValues == null && !string.IsNullOrEmpty(collectionName))
            //    //    return conn.GetSchema(collectionName);
            //    //else
            //    //    return conn.GetSchema(collectionName, restrictionValues);
            //}

            string sql = "select * from INFORMATION_SCHEMA.";
            collectionName = collectionName.ToUpper();
            switch (collectionName)
            {
                case "COLUMNS":
                case "INDEXES":
                case "KEY_COLUMN_USAGE":
                case "PROVIDER_TYPES":
                case "TABLES":
                case "TABLE_CONSTRAINTS":
                case "REFERENTIAL_CONSTRAINTS":
                    sql = sql + collectionName;
                    break;
                default:
                    throw new NotSupportedException("SQLCE 不支持该架构集合");
            }

            DataSet ds = this.ExecuteDataSet(sql);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (restrictionValues != null && restrictionValues.Length > 0)
                {
                    DataTable dtObj = dt.Clone();
                    foreach (DataRow dr in dt.Rows)
                    {
                        bool flag = true;
                        for (int i = 0; i < restrictionValues.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(restrictionValues[i]))
                            {
                                if (dr[i].ToString() != restrictionValues[i])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            dtObj.Rows.Add(dr.ItemArray);
                        }
                    }
                    return dtObj;
                }
                else
                {
                    return dt;
                }

            }
            throw new Exception("SQLCE 查询表架构错误，架构表为空：" + sql);

        }

      

    }
}
