using System;
using System.Data;
using System.Data.OleDb;

namespace DME.DataBase.DataProvider.Data
{
   /// <summary>
   /// Access 数据库访问类
   /// </summary>
    public sealed class DMEDb_Access : DMEDb_OleDb 
    {

        /// <summary>
        /// 获取当前数据库类型的枚举
        /// </summary>
        public override DME.DataBase.Common.DMEDb_DBMSType CurrentDBMSType
        {
            get { return DME.DataBase.Common.DMEDb_DBMSType.Access; }
        }
    }
    
}
