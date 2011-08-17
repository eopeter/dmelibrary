using System;
using System.Data ;
using System.Configuration ;
using DME.DataBase.DataProvider.Data;
using DME.DataBase.Common;
using DME.Base.Helper;
using System.Collections.Generic;

namespace DME.DataBase.DataProvider.Adapter
{
    /// <summary>
    /// Ӧ�ó������ݷ���ʵ�����ṩ����ģʽ�͹���ģʽ����ʵ�����󣬸���Ӧ�ó��������ļ��Զ������ض������ݷ��ʶ���
    /// 2008.5.23 ���Ӷ�̬���ݼ����¹���,7.24�����̰߳�ȫ�ľ�̬ʵ����
    /// 2009.4.1  ����SQLite ���ݿ�֧�֡�
    /// 2010.1.6  ���� connectionStrings ����֧��
    /// </summary>
    public class DMEDb_MyDB
    {
        private static DMEDb_AdoHelper _instance = null;
        private string _msg = string.Empty;
        private static object lockObj = new object();

        #region ��ȡ��̬ʵ��

        /// <summary>
        /// ���ݷ��ʾ�̬ʵ������������������п��ܴ��ڲ������ʣ�����ʹ�ø����ԣ����Ǵ�������Ķ�̬ʵ������
        /// </summary>
        public static DMEDb_AdoHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = DMEDb_MyDB.GetDBHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region ��ȡ��̬ʵ������
        /// <summary>
        /// ͨ�������ļ�������ݷ��ʶ���ʵ����
        /// ����Ӧ�ó��������ļ��д��� EngineType ����ֵΪ[DB]��ͬʱ��Ҫ���� [DB]HelperAssembly��[DB]HelperType ��[DB]ConnectionString ����[DB]ֵΪSQLSERVER/OLEDB/ODBC/ORACLE ֮һ
        /// ���δָ�� EngineType ������ʹ�� connectionStrings ���ýڵĵ�һ������������Ϣ������ָ�� providerName������ʹ���������ʽ��
        /// providerName="DME.DataBase.DataProvider.Data.SqlServer,DME.DataBase"
        /// Ҳ����ֱ��ʹ�� �������ʽ��
        /// providerName="SqlServer" ����Ȼ��������ʽ���ṩ�������Ĭ�Ͼ��� DME.DataBase.CommonDataProvider.Data ��
        /// ����ж����Ĭ��ȡ name="default"
        /// </summary>
        /// <returns>���ݷ��ʶ���ʵ��</returns>
        public static DMEDb_AdoHelper GetDBHelper()
        {
 
            string engineType = DME.Base.DME_LibraryConfig.DMEDb_EngineType;
            DMEDb_AdoHelper helper = null;
            if (DME_Validation.IsNull(engineType))
            {
                //�� connectionStrings ��ȡ
                Dictionary<string, string> dc = new Dictionary<string, string>();
                dc = DME.Base.DME_LibraryConfig.DMEDb_GetDataBaseConnection("default");
                if ((!DME_Validation.IsNull(dc["connectionString"])) && (!DME_Validation.IsNull(dc["providerName"])))
                {
                    helper = GetDBHelperByProviderString(dc["providerName"], dc["connectionString"]);                    
                }
                else
                {
                    throw new Exception("δָ����EngineType�����ü���Ҳδ�ڡ�DME_DataBase_Connection�����ýڡ�default������������Ϣ");
                }
            }
            else
            {
                helper = GetDBHelper(engineType);
                helper.ConnectionString = GetConnectionString();
            }
            return helper;
        }

        /// <summary>
        /// �� connectionStrings ���ýڻ�ȡָ�� �����������Ƶ����ݷ��ʶ���ʵ��
        /// </summary>
        /// <param name="name">������������</param>
        /// <returns></returns>
        public static DMEDb_AdoHelper GetDBHelperByConnectionName(string name)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();
            dc = DME.Base.DME_LibraryConfig.DMEDb_GetDataBaseConnection(name); ;
            if ((!DME_Validation.IsNull(dc["connectionString"])) && (!DME_Validation.IsNull(dc["providerName"])))
            {
            }
            else
            {
                throw new Exception("δ�� connectionStrings ���ý��ҵ�ָ���� �������ƣ�" + name);
            }

            return GetDBHelperByConnectionSetting(dc);
        }

        private static DMEDb_AdoHelper GetDBHelperByConnectionSetting(Dictionary<string,string> connSetting)
        {
            return GetDBHelperByProviderString(connSetting["connectionString"], connSetting["providerName"]);
        }

        /// <summary>
        /// �����ṩ���������ַ����������ַ����������ṩ����ʵ��
        /// </summary>
        /// <param name="providerName">�����������ַ�������ʽΪ���ṩ����ȫ����,��������</param>
        /// <param name="connectionString">�����ַ���</param>
        /// <returns></returns>
        public static DMEDb_AdoHelper GetDBHelperByProviderString(string providerName, string connectionString)
        {
            string[] providerInfo = providerName.Split(',');
            string helperAssembly;
            string helperType;

            if (providerInfo.Length == 1)
            {
                helperAssembly = "DME.DataBase";
                helperType = "DME.DataBase.DataProvider.Data." + providerName;
            }
            else
            {
                helperAssembly = providerInfo[1].Trim();
                helperType = providerInfo[0].Trim();
            }
            return GetDBHelper(helperAssembly, helperType, connectionString);
        }
        /// <summary>
        /// ͨ��ָ�������ݿ����ͣ�ֵΪSQLSERVER/OLEDB/ODBC/ORACLE ֮һ���������ַ�������һ���µ����ݷ��ʶ���
        /// ��Ҫ����[DB]HelperAssembly��[DB]HelperType ����[DB]ֵΪSQLSERVER/OLEDB/ODBC/ORACLE ֮һ
        /// </summary>
        /// <param name="EngineType">���ݿ����ͣ�ֵΪSQLSERVER/OLEDB/ODBC/ORACLE ֮һ��</param>
        /// <param name="ConnectionString">�����ַ���</param>
        /// <returns>���ݷ��ʶ���</returns>
        public static DMEDb_AdoHelper GetDBHelper(string EngineType, string ConnectionString)
        {
            DMEDb_AdoHelper helper = GetDBHelper(EngineType);
            helper.ConnectionString = ConnectionString;
            return helper;
        }

        /// <summary>
        /// �������ݿ����ϵͳö�����ͺ������ַ�������һ���µ����ݷ��ʶ���ʵ��
        /// </summary>
        /// <param name="DbmsType">���ݿ�����ý�飬��ACCESS/MYSQL/ORACLE/SQLSERVER/SYSBASE/UNKNOWN </param>
        /// <param name="ConnectionString">�����ַ���</param>
        /// <returns>���ݷ��ʶ���</returns>
        public static DMEDb_AdoHelper GetDBHelper(DMEDb_DBMSType DbmsType, string ConnectionString)
        {
            string EngineType = "";
            switch (DbmsType)
            {
                case DMEDb_DBMSType.Access:
                    EngineType = "OleDb"; break;
                case DMEDb_DBMSType.MySql:
                    EngineType = "Odbc"; break;
                case DMEDb_DBMSType.Oracle:
                    EngineType = "Oracle"; break;
                case DMEDb_DBMSType.SqlServer:
                    EngineType = "SqlServer"; break;
                case DMEDb_DBMSType.SqlServerCe :
                    EngineType = "SqlServerCe"; break;
                case DMEDb_DBMSType.Sysbase:
                    EngineType = "OleDb"; break;
                case DMEDb_DBMSType.SQLite:
                    EngineType = "SQLite"; break;
                case DMEDb_DBMSType.UNKNOWN:
                    EngineType = "Odbc"; break;
            }
            DMEDb_AdoHelper helper = GetDBHelper(EngineType);
            helper.ConnectionString = ConnectionString;
            return helper;
        }

        /// <summary>
        /// ���ݳ������ƺ����ݷ��ʶ������ʹ���һ���µ����ݷ��ʶ���ʵ����
        /// </summary>
        /// <param name="HelperAssembly">��������</param>
        /// <param name="HelperType">���ݷ��ʶ�������</param>
        /// <param name="ConnectionString">�����ַ���</param>
        /// <returns>���ݷ��ʶ���</returns>
        public static DMEDb_AdoHelper GetDBHelper(string HelperAssembly, string HelperType, string ConnectionString)
        {
            DMEDb_AdoHelper helper = null;// CommonDB.CreateInstance(HelperAssembly, HelperType);
            if (HelperAssembly == "DME.DataBase")
            {
                switch (HelperType)
                {
                    case "DME.DataBase.DataProvider.Data.SqlServer": helper = new DMEDb_SqlServer(); break;
                    case "DME.DataBase.DataProvider.Data.Oracle": helper = new DMEDb_Oracle(); break;
                    case "DME.DataBase.DataProvider.Data.OleDb": helper = new DMEDb_OleDb(); break;
                    case "DME.DataBase.DataProvider.Data.Odbc": helper = new DMEDb_Odbc(); break;
                    case "DME.DataBase.DataProvider.Data.Access": helper = new DMEDb_Access(); break;
                    case "DME.DataBase.DataProvider.Data.SqlServerCe": helper = new DMEDb_SqlServerCe(); break;
                    default: helper = new DMEDb_SqlServer(); break;
                }
            }
            else
            {
                helper = DMEDb_CommonDB.CreateInstance(HelperAssembly, HelperType);
            }
            helper.ConnectionString = ConnectionString;
            return helper;
        }


        /// <summary>
        /// ������ݷ��ʶ���ʵ����EngineTypeֵΪSQLSERVER/OLEDB/ODBC/ORACLE ֮һ��Ĭ��ʹ�� PWMIS.CommonDataProvider.Data.SqlServer
        /// </summary>
        /// <param name="EngineType">���ݿ���������</param>
        /// <returns>���ݷ��ʶ���ʵ��</returns>
        private static DMEDb_AdoHelper GetDBHelper(string EngineType)
        {
            Dictionary<string, string> dc = DME.Base.DME_LibraryConfig.DMEDb_GetDBHelperStr(EngineType);
            return GetDBHelper(dc["assembly"], dc["type"], null);// CommonDB.CreateInstance(assembly, type);
        }

        #endregion

        #region ������̬����

        /// <summary>
        /// ������ݷ��������ַ���������Ӧ�ó��������ļ��д��� EngineType��[DB]HelperAssembly��[DB]HelperType ,[DB]ConnectionString����ֵΪSQLSERVER/OLEDB/ODBC/ORACLE ֮һ
        /// ���û���ҵ� [DB]ConnectionString ����Ҳ����ֱ��ʹ�� ConnectionString ��
        /// </summary>
        /// <returns>���ݷ��������ַ���</returns>
        public static string GetConnectionString()
        {

            string connectionString = null;

            switch (DME.Base.DME_LibraryConfig.DMEDb_EngineType.ToUpper())
            {
                case "SQLSERVER":
                    connectionString = DME.Base.DME_LibraryConfig.DMEDb_GetConnectionString("SqlServerConnectionString");
                    break;
                case "OLEDB":
                    connectionString = DME.Base.DME_LibraryConfig.DMEDb_GetConnectionString("OleDbConnectionString");
                    break;
                case "ODBC":
                    connectionString = DME.Base.DME_LibraryConfig.DMEDb_GetConnectionString("OdbcConnectionString");
                    break;
                case "ORACLE":
                    connectionString = DME.Base.DME_LibraryConfig.DMEDb_GetConnectionString("OracleConnectionString");
                    break;
                case "SQLITE":
                    connectionString = DME.Base.DME_LibraryConfig.DMEDb_GetConnectionString("SQLiteConnectionString");
                    break;
            }
            if(string.IsNullOrEmpty(connectionString))
                connectionString = DME.Base.DME_LibraryConfig.DMEDb_GetConnectionString("ConnectionString");

            return connectionString;
        }

        /// <summary>
        /// �������ݼ�(���ò�����ʽ)�����ݱ����ָ����������ôִ�и��²���������ִ�в��������
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <returns>��ѯ�����Ӱ�������</returns>
        public static int UpdateDataSet(DataSet ds)
        {
            int count = 0;
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.PrimaryKey.Length > 0)
                {
                    count += UpdateDataTable(dt, GetSqlUpdate(dt));
                }
                else
                {
                    count += UpdateDataTable(dt, GetSqlInsert(dt));
                }// end if
            }//end for
            return count;
        }//end function

        /// <summary>
        /// �������ݼ�����ָ���ı��У����ݱ��е�ָ���е�ֵ������Դ��ɾ������
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="tableName">������</param>
        /// <param name="columnName">����</param>
        /// <returns>��ѯ��Ӱ�������</returns>
        public static int DeleteDataSet(DataSet ds, string tableName, string columnName)
        {
            DataTable dt = ds.Tables[tableName];

            DMEDb_CommonDB DB = DMEDb_MyDB.GetDBHelper();
            string ParaChar = GetDBParaChar(DB);
            int count = 0;

            string sqlDelete = "DELETE FROM " + tableName + " WHERE " + columnName + "=" + ParaChar + columnName;

            DB.BeginTransaction();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    IDataParameter[] paras = { DB.GetParameter(ParaChar + columnName, dr[columnName]) };
                    count += DB.ExecuteNonQuery(sqlDelete, CommandType.Text, paras);
                    if (DB.ErrorMessage != "")
                        throw new Exception(DB.ErrorMessage);
                    if (count >= dt.Rows.Count) break;
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return count;

        }

        #endregion

        #region ������̬ʵ������
        /// <summary>
        /// ��ȡ��ǰ������Ϣ
        /// </summary>
        public string Message
        {
            get { return _msg; }
        }
        /// <summary>
        /// �������ݼ��������ݷ��ʶ���
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="DB">���ݷ��ʶ���</param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet ds, DMEDb_CommonDB DB)
        {
            int count = 0;
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.PrimaryKey.Length > 0)
                {
                    count += UpdateDataTable(dt, GetSqlUpdate(dt), DB);
                    _msg = "�Ѿ����¼�¼" + count + "��";
                }
                else
                {
                    count += UpdateDataTable(dt, GetSqlInsert(dt), DB);
                    _msg = "�Ѿ������¼" + count + "��";
                }// end if
            }//end for
            return count;
        }//end function

        /// <summary>
        /// �������ݼ�����ָ���ı��У����ݱ��е�ָ���е�ֵ������Դ��ɾ������,�����ݷ��ʶ���
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="tableName">������</param>
        /// <param name="columnName">����</param>
        /// <param name="DB">���ݷ��ʶ���</param>
        /// <returns></returns>
        public int DeleteDataSet(DataSet ds, string tableName, string columnName, DMEDb_CommonDB DB)
        {
            DataTable dt = ds.Tables[tableName];
            string ParaChar = GetDBParaChar(DB);
            int count = 0;
            string sqlDelete = "DELETE FROM " + tableName + " WHERE " + columnName + "=" + ParaChar + columnName;
            foreach (DataRow dr in dt.Rows)
            {
                IDataParameter[] paras = { DB.GetParameter(ParaChar + columnName, dr[columnName]) };
                count += DB.ExecuteNonQuery(sqlDelete, CommandType.Text, paras);
                if (DB.ErrorMessage != "")
                    throw new Exception(DB.ErrorMessage);
                if (count >= dt.Rows.Count) break;
            }
            return count;
        }

        /// <summary>
        /// ����������Ϣ������Դ��ѯ���ݱ����ݼ���
        /// </summary>
        /// <param name="tableName">����Դ�еı�����</param>
        /// <param name="pkNames">������������</param>
        /// <param name="pkValues">����ֵ���飬�������������Ӧ</param>
        /// <returns>���ݼ�</returns>
        public DataSet SelectDataSet(string tableName, string[] pkNames, object[] pkValues)
        {
            return SelectDataSet("*", tableName, pkNames, pkValues);
        }

        /// <summary>
        /// ����������Ϣ������Դ��ѯ���ݱ����ݼ���
        /// </summary>
        /// <param name="fields">�ֶ��б�</param>
        /// <param name="tableName">����Դ�еı�����</param>
        /// <param name="pkNames">������������</param>
        /// <param name="pkValues">����ֵ���飬�������������Ӧ</param>
        /// <param name="DB">���ݷ��ʶ���</param>
        /// <returns></returns>
        public DataSet SelectDataSet(string fields, string tableName, string[] pkNames, object[] pkValues, DMEDb_CommonDB DB)
        {
            string ParaChar = GetDBParaChar(DB);
            string sqlSelect = "SELECT " + fields + " FROM " + tableName + " WHERE 1=1 ";
            IDataParameter[] paras = new IDataParameter[pkNames.Length];
            for (int i = 0; i < pkNames.Length; i++)
            {
                sqlSelect += " And " + pkNames[i] + "=" + ParaChar + pkNames[i];
                paras[i] = DB.GetParameter(ParaChar + pkNames[i], pkValues[i]);
            }
            DataSet ds = DB.ExecuteDataSet(sqlSelect, CommandType.Text, paras);
            ds.Tables[0].TableName = tableName;
            return ds;
        }

        /// <summary>
        /// ����������Ϣ������Դ��ѯ���ݱ����ݼ���
        /// </summary>
        /// <param name="fields">�ֶ��б�</param>
        /// <param name="tableName">����Դ�еı�����</param>
        /// <param name="pkNames">������������</param>
        /// <param name="pkValues">����ֵ���飬�������������Ӧ</param>
        /// <returns>���ݼ�</returns>
        public DataSet SelectDataSet(string fields, string tableName, string[] pkNames, object[] pkValues)
        {
            DMEDb_CommonDB DB = DMEDb_MyDB.GetDBHelper();
            string ParaChar = GetDBParaChar(DB);
            string sqlSelect = "SELECT " + fields + " FROM " + tableName + " WHERE 1=1 ";
            IDataParameter[] paras = new IDataParameter[pkNames.Length];
            for (int i = 0; i < pkNames.Length; i++)
            {
                sqlSelect += " And " + pkNames[i] + "=" + ParaChar + pkNames[i];
                paras[i] = DB.GetParameter(ParaChar + pkNames[i], pkValues[i]);
            }
            DataSet ds = DB.ExecuteDataSet(sqlSelect, CommandType.Text, paras);
            ds.Tables[0].TableName = tableName;
            return ds;
        }

        /// <summary>
        /// �������ݼ��е��ֶε�����Դ��
        /// </summary>
        /// <param name="sDs">Դ���ݼ�</param>
        /// <param name="tableName">Ҫ���µı�</param>
        /// <param name="fieldName">Ҫ���µ��ֶ�</param>
        /// <param name="fieldValue">�ֶε�ֵ</param>
        /// <param name="pkName">��������</param>
        /// <param name="DB">���ݷ��ʶ���</param>
        /// <returns></returns>
        public int UpdateField(DataSet sDs, string tableName, string fieldName, object fieldValue, string pkName, DMEDb_CommonDB DB)
        {
            DataSet ds = sDs.Copy();
            DataTable dt = ds.Tables[tableName];
            fieldName = fieldName.ToUpper();
            pkName = pkName.ToUpper();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string colName = dt.Columns[i].ColumnName.ToUpper();
                if (colName == fieldName || colName == pkName)
                    continue;
                dt.Columns.Remove(colName);
                i = 0;//����Ԫ��λ�ÿ����Ѿ�Ǩ�ƣ�������Ҫ���´�ͷ��ʼ����
            }
            dt.PrimaryKey = new DataColumn[] { dt.Columns[pkName] };
            foreach (DataRow dr in dt.Rows)
            {
                dr[fieldName] = fieldValue;
            }

            int updCount = UpdateDataSet(ds, DB);
            return updCount;
        }

        #endregion

        #region �ڲ�����
        /// <summary>
        /// ��ȡ�ض����ݿ�����ַ�
        /// </summary>
        /// <param name="DB">���ݿ�����</param>
        /// <returns></returns>
        private static string GetDBParaChar(DMEDb_CommonDB DB)
        {
            return DB is DMEDb_Oracle ? ":" : "@";
        }

        /// <summary>
        /// �������ݱ�����Դ��
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        private static int UpdateDataTable(DataTable dt, string SQL)
        {
            DMEDb_CommonDB DB = DMEDb_MyDB.GetDBHelper();
            string ParaChar = GetDBParaChar(DB);
            SQL = SQL.Replace("@@", ParaChar);
            int count = 0;
            DB.BeginTransaction();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    IDataParameter[] paras = new IDataParameter[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        paras[i] = DB.GetParameter(ParaChar + dt.Columns[i].ColumnName, dr[i]);
                    }
                    count += DB.ExecuteNonQuery(SQL, CommandType.Text, paras);
                    if (DB.ErrorMessage != "")
                        throw new Exception(DB.ErrorMessage);
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return count;

        }

        /// <summary>
        /// �������ݱ������ݷ��ʶ���
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="SQL"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        private int UpdateDataTable(DataTable dt, string SQL, DMEDb_CommonDB DB)
        {
            string ParaChar = GetDBParaChar(DB);
            SQL = SQL.Replace("@@", ParaChar);
            int count = 0;

            foreach (DataRow dr in dt.Rows)
            {
                IDataParameter[] paras = new IDataParameter[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    paras[i] = DB.GetParameter(ParaChar + dt.Columns[i].ColumnName, dr[i]);
                }
                count += DB.ExecuteNonQuery(SQL, CommandType.Text, paras);
                if (DB.ErrorMessage != "")
                    throw new Exception(DB.ErrorMessage);
            }

            return count;

        }


        /// <summary>
        /// Ϊ���ݱ����ɸ���SQL��䣬��������@@ǰ׺[����������]
        /// </summary>
        /// <param name="dt">���ݱ�</param>
        /// <returns></returns>
        private static string GetSqlUpdate(DataTable dt)
        {
            string sqlUpdate = "UPDATE " + dt.TableName + " SET ";
            if (dt.PrimaryKey.Length > 0)
            {
                DataColumn[] pks = dt.PrimaryKey;
                foreach (DataColumn dc in dt.Columns)
                {
                    bool isPk = false;
                    for (int i = 0; i < pks.Length; i++)
                        if (dc == pks[i])
                        {
                            isPk = true;
                            break;
                        }
                    //����������
                    if (!isPk)
                        sqlUpdate += dc.ColumnName + "=@@" + dc.ColumnName + ",";
                }
                sqlUpdate = sqlUpdate.TrimEnd(',') + " WHERE 1=1 ";
                foreach (DataColumn dc in dt.PrimaryKey)
                {
                    sqlUpdate += "And " + dc.ColumnName + "=@@" + dc.ColumnName + ",";
                }
                sqlUpdate = sqlUpdate.TrimEnd(',');
                return sqlUpdate;

            }
            else
            {
                throw new Exception("��" + dt.TableName + "û��ָ���������޷�����Update��䣡");
            }
        }

        /// <summary>
        /// Ϊ���ݱ����ɲ���SQL��䣬��������@@ǰ׺
        /// </summary>
        /// <param name="dt">���ݱ�</param>
        /// <returns></returns>
        private static string GetSqlInsert(DataTable dt)
        {
            string Items = "";
            string ItemValues = "";
            string sqlInsert = "INSERT INTO " + dt.TableName;

            foreach (DataColumn dc in dt.Columns)
            {
                Items += dc.ColumnName + ",";
                ItemValues += "@@" + dc.ColumnName + ",";
            }
            sqlInsert += "(" + Items.TrimEnd(',') + ") Values(" + ItemValues.TrimEnd(',') + ")";
            return sqlInsert;
        }

        #endregion

        public DMEDb_DBMSType DBMSType
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public DMEDb_SQLPage SQLPage
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

    }
}

