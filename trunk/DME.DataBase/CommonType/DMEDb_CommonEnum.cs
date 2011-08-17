using System;
using System.Collections.Generic;
using System.Text;

namespace DME.DataBase.Common
{
    /// <summary>
    /// 数据库管理系统枚举
    /// </summary>
    public enum DMEDb_DBMSType
    {
        Access,
        SqlServer,
        SqlServerCe,
        Oracle,
        DB2,
        Sysbase,
        MySql,
        SQLite,
        PostgreSQL,
        UNKNOWN=999
    }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public enum DMEDb_DataSourceType
    { 
        OleDb,
        SqlServer,
        SqlServerCe,
        SQLite,
        Oracle,
        Odbc,
        TextFile,
        XML
    }
}
