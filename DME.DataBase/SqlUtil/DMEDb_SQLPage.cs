/*简单查询分页方案 算法说明
 * SQL SERVER ：
		 * 如果有这样简单的查询语句：
		 * SELECT SelectFields FROM TABLE @@Where ORDER BY OrderFields DESC
		 * 那么分页方案可以采用下面的方式：
		 * 
		 * 第一页：
		 * SELECT TOP @@PageSize SelectFields FROM TABLE @@Where ORDER BY OrderFields DESC;
		 * 中间页：
		 * SELECT Top @@PageSize * FROM
			             (SELECT Top @@PageSize * FROM
				           (
                             SELECT Top @@Page_Size_Number
							 SelectFields FROM TABLE @@Where ORDER BY OrderFields DESC
						   ) P_T0 ORDER BY OrderFields ASC
						 ) P_T1 ORDER BY OrderFields DESC
		 * 最后页：
		 * SELECT * FROM (	 
	                      Select Top @@LeftSize 
						  SelectFields FROM TABLE @@Where ORDER BY OrderFields ASC
						 ) ORDER BY OrderFields DESC
						 
						  
		 * 函数 MakePageSQLStringByDBNAME 在此基础上实现了更为复杂的分页处理，这里的复杂时说查询
		 * 包含大量的子查询或者连接查询，因此评价查询复杂与否采用下面的标准：
		 * 
		 * 只包含一个 SELECT 谓词；
		 * 没有 INNER JOIN，RIGHT JOIN，LEFT JOIN 等表连接谓词；
		 * 谓词 FROM 后只能有一个表名；
		 * 
		 * 否则，视为该查询为一个复杂查询，采用复杂查询分页方案；
Oracle ：
基本的分页原理利用Oracle内指的 rownum 伪列，它是一个递增序列，但是它在Order by 之前生成，通常
采用下面的分页语句：
select * from
 (select rownum r_n,temptable.* from  
   ( @@SourceSQL ) temptable
 ) temptable2 where r_n between @@RecStart  and @@RecEnd
其中：
@@SourceSQL :当前任意复杂的SQL语句
@@RecStart:记录开始的点，等于 ((tCurPage -1) * tPageSize +1) 
@@RecEnd  :记录结束的点，等于 (tCurPage * tPageSize) 
		 

** 约束：
使用该分页方法要求 SQL语句本身必须满足下列条件：
1，最外层的查询不能含有 TOP 谓词(最好不要使用TOP，可以避免Oracle 不兼容的问题)；
2，最外层查询必须含有 ORDER BY 语句（Oracle除外）；
3，不能含有下列替换参数(区分大小写)：@@PageSize,@@Page_Size_Number,@@LeftSize,@@Where
4，SQL必须符合 SQL-92 以上标准，且 最外层ORDER BY 语句之后不能有其他语句，
5，如果使用SQLSERVER 以外的数据库系统，请在Web.config配置节里面注明 EngineType 的值；
Group by 等放在Order by 之前；
**
 *
 * =====================主键高效分页方式=========================================
 * 海量数据库的查询优化及分页算法方案 http://edu.codepub.com/2009/0522/4437_3.php
 * 
 * 我们知道，几乎任何字段，我们都可以通过max(字段)或min(字段)来提取某个字段中的最大或最小值，所以如果这个字段不重复，那么就可以利用这些不重复的字段的max或min作为分水岭，使其成为分页算法中分开每页的参照物。在这里，我们可以用操作符“>”或“<”号来完成这个使命，使查询语句符合SARG形式。如：
Select top 10 * from table1 where id>200
　　于是就有了如下分页方案：
select top 页大小 *
from table1
where id>
      (select max (id) from
      (select top ((页码-1)*页大小) id from table1 order by id) as T
       )    
  order by id

		 */
using System;

namespace DME.DataBase.Common
{
    /// <summary>
    /// SQL SERVER 分页处理，自动识别标准SQL语句并生成适合分页的SQL语句
    /// </summary>
    public class DMEDb_SQLPage
    {
        //public static string DBType=System.Configuration.ConfigurationSettings .AppSettings ["EngineType"];
        public static DMEDb_DBMSType DbmsType = DMEDb_DBMSType.SqlServer;

        public DMEDb_SQLPage()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 生成SQL分页语句，记录总数为0表示生成统计语句
        /// </summary>
        /// <param name="dbmsType">数据库类型</param>
        /// <param name="strSQLInfo">原始SQL语句</param>
        /// <param name="strWhere">在分页前要替换的字符串，用于分页前的筛选</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="PageNumber">页码</param>
        /// <param name="AllCount">记录总数，如果是0则生成统计记录数量的查询</param>
        /// <returns>生成SQL分页语句</returns>
        public static string MakeSQLStringByPage(DMEDb_DBMSType dbmsType, string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount)
        {
            //根据不同的数据库引擎调用不同生成器
            string SQL = string.Empty;
            switch (dbmsType)
            {
                case DMEDb_DBMSType.Access:
                case DMEDb_DBMSType.SqlServer:
                    SQL = MakePageSQLStringByMSSQL(strSQLInfo, strWhere, PageSize, PageNumber, AllCount);
                    break;
                case DMEDb_DBMSType.Oracle:
                    SQL = MakePageSQLStringByOracle(strSQLInfo, strWhere, PageSize, PageNumber, AllCount);
                    break;
                case DMEDb_DBMSType.MySql:
                case DMEDb_DBMSType.SQLite:
                    SQL = MakePageSQLStringByMySQL(strSQLInfo, strWhere, PageSize, PageNumber, AllCount);
                    break;
                case DMEDb_DBMSType.PostgreSQL:
                    SQL = MakePageSQLStringByPostgreSQL(strSQLInfo, strWhere, PageSize, PageNumber, AllCount);
                    break;
                default:
                    //SQL = MakePageSQLStringByMSSQL(strSQLInfo, strWhere, PageSize, PageNumber, AllCount);
                    //SQL = strSQLInfo;
                    //break;
                    throw new Exception("分页错误：不支持此种类型的数据库分页。");
            }
            return SQL;
        }

        /// <summary>
        /// 生成SQL分页语句，记录总数为0表示生成统计语句
        /// </summary>
        /// <param name="strSQLInfo">原始SQL语句</param>
        /// <param name="strWhere">在分页前要替换的字符串，用于分页前的筛选</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="PageNumber">页码</param>
        /// <param name="AllCount">记录总数，如果是0则生成统计记录数量的查询</param>
        /// <returns>生成SQL分页语句</returns>
        public static string MakeSQLStringByPage(string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount)
        {
            return MakeSQLStringByPage(DMEDb_SQLPage.DbmsType, strSQLInfo, strWhere, PageSize, PageNumber, AllCount);
        }

        /// <summary>
        /// MS SQLSERVER 分页SQL语句生成器，同样适用于ACCESS数据库
        /// </summary>
        /// <param name="strSQLInfo">原始SQL语句</param>
        /// <param name="strWhere">在分页前要替换的字符串，用于分页前的筛选</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="PageNumber">页码</param>
        /// <param name="AllCount">记录总数</param>
        /// <returns>生成SQL分页语句</returns>
        private static string MakePageSQLStringByMSSQL(string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount)
        {
            #region 分页位置分析
            string strSQLType = string.Empty;
            if (AllCount != 0)
            {
                if (PageNumber == 1) //首页
                {
                    strSQLType = "First";
                }
                else if (PageSize * PageNumber > AllCount) //最后的页 @@LeftSize
                {
                    PageSize = AllCount - PageSize * (PageNumber - 1);
                    strSQLType = "Last";
                }
                else //中间页
                {
                    strSQLType = "Mid";
                }
            }
            else if (AllCount < 0) //特殊处理 dth,2006.10.19
            {
                strSQLType = "First";
            }
            else
            {
                strSQLType = "Count";
            }

            #endregion

            #region SQL 复杂度分析
            //SQL 复杂度分析 开始
            bool SqlFlag = true;//简单SQL标记
            string TestSQL = strSQLInfo.ToUpper();
            int n = TestSQL.IndexOf("SELECT ", 0);
            n = TestSQL.IndexOf("SELECT ", n + 7);
            if (n == -1)
            {
                //可能是简单的查询，再次处理
                n = TestSQL.IndexOf(" JOIN ", n + 7);
                if (n != -1) SqlFlag = false;
                else
                {
                    //判断From 谓词情况
                    n = TestSQL.IndexOf("FROM ", 9);
                    if (n == -1) return "";
                    //计算 WHERE 谓词的位置
                    int m = TestSQL.IndexOf("WHERE ", n + 5);
                    // 如果没有WHERE 谓词
                    if (m == -1) m = TestSQL.IndexOf("ORDER BY ", n + 5);
                    //如果没有ORDER BY 谓词，那么无法排序，退出；
                    if (m == -1)
                        throw new Exception("查询语句分析：当前没有为分页查询指定排序字段！请适当修改SQL语句。\n" + strSQLInfo);
                    string strTableName = TestSQL.Substring(n, m - n);
                    //表名中有 , 号表示是多表查询
                    if (strTableName.IndexOf(",") != -1)
                        SqlFlag = false;
                }
            }
            else
            {
                //有子查询；
                SqlFlag = false;
            }
            //SQL 复杂度分析 结束
            #endregion

            #region 排序语法分析
            //排序语法分析 开始
            int iOrderAt = strSQLInfo.ToLower().LastIndexOf("order by ");
            //如果没有ORDER BY 谓词，那么无法排序分页，退出；
            if (iOrderAt == -1)
            {
                if (PageNumber == 1)
                {
                    string sqlTemp = "Select Top @@PageSize * FROM ( " + strSQLInfo +
                           " ) P_T0 @@Where ";
                    return sqlTemp.Replace("@@PageSize", PageSize.ToString()).Replace("@@Where", strWhere);
                }
                else
                {
                    throw new Exception("查询语句分析：当前没有为分页查询指定排序字段！请适当修改SQL语句。\n" + strSQLInfo);
                }
            }

            string strOrder = strSQLInfo.Substring(iOrderAt + 9);
            strSQLInfo = strSQLInfo.Substring(0, iOrderAt);
            string[] strArrOrder = strOrder.Split(new char[] { ',' });
            for (int i = 0; i < strArrOrder.Length; i++)
            {
                string[] strArrTemp = (strArrOrder[i].Trim() + " ").Split(new char[] { ' ' });
                //压缩多余空格
                for (int j = 1; j < strArrTemp.Length; j++)
                {
                    if (strArrTemp[j].Trim() == "")
                    {
                        continue;
                    }
                    else
                    {
                        strArrTemp[1] = strArrTemp[j];
                        if (j > 1) strArrTemp[j] = "";
                        break;
                    }
                }
                //判断字段的排序类型
                switch (strArrTemp[1].Trim().ToUpper())
                {
                    case "DESC":
                        strArrTemp[1] = "ASC";
                        break;
                    case "ASC":
                        strArrTemp[1] = "DESC";
                        break;
                    default:
                        //未指定排序类型，默认为降序
                        strArrTemp[1] = "DESC";
                        break;
                }
                //消除排序字段对象限定符
                if (strArrTemp[0].IndexOf(".") != -1)
                    strArrTemp[0] = strArrTemp[0].Substring(strArrTemp[0].IndexOf(".") + 1);
                strArrOrder[i] = string.Join(" ", strArrTemp);

            }
            //生成反向排序语句
            string strNewOrder = string.Join(",", strArrOrder).Trim();
            strOrder = strNewOrder.Replace("ASC", "ASC0").Replace("DESC", "ASC").Replace("ASC0", "DESC");
            //排序语法分析结束
            #endregion

            #region 构造分页查询
            string SQL = string.Empty;
            if (!SqlFlag)
            {
                //复杂查询处理
                switch (strSQLType.ToUpper())
                {
                    case "FIRST":
                        SQL = "Select Top @@PageSize * FROM ( " + strSQLInfo +
                            " ) P_T0 @@Where ORDER BY " + strOrder;
                        break;
                    case "MID":
                        SQL = @"SELECT Top @@PageSize * FROM
                         (SELECT Top @@PageSize * FROM
                           (
                             SELECT Top @@Page_Size_Number * FROM (";
                        SQL += " " + strSQLInfo + " ) P_T0 @@Where ORDER BY " + strOrder + " ";
                        SQL += @") P_T1
            ORDER BY " + strNewOrder + ") P_T2  " +
                            "ORDER BY " + strOrder;
                        break;
                    case "LAST":
                        SQL = @"SELECT * FROM (     
                          Select Top @@LeftSize * FROM (" + " " + strSQLInfo + " ";
                        SQL += " ) P_T0 @@Where ORDER BY " + strNewOrder + " " +
                            " ) P_T1 ORDER BY " + strOrder;
                        break;
                    case "COUNT":
                        SQL = "Select COUNT(*) FROM ( " + strSQLInfo + " ) P_Count @@Where";
                        break;
                    default:
                        SQL = strSQLInfo + strOrder;//还原
                        break;
                }

            }
            else
            {
                //简单查询处理
                switch (strSQLType.ToUpper())
                {
                    case "FIRST":
                        SQL = strSQLInfo.ToUpper().Replace("SELECT ", "SELECT TOP @@PageSize ");
                        SQL += "  @@Where ORDER BY " + strOrder;
                        break;
                    case "MID":
                        string strRep = @"SELECT Top @@PageSize * FROM
                         (SELECT Top @@PageSize * FROM
                           (
                             SELECT Top @@Page_Size_Number  ";
                        SQL = strSQLInfo.ToUpper().Replace("SELECT ", strRep);
                        SQL += "  @@Where ORDER BY " + strOrder;
                        SQL += "  ) P_T0 ORDER BY " + strNewOrder + " " +
                            " ) P_T1 ORDER BY " + strOrder;
                        break;
                    case "LAST":
                        string strRep2 = @"SELECT * FROM (     
                          Select Top @@LeftSize ";
                        SQL = strSQLInfo.ToUpper().Replace("SELECT ", strRep2);
                        SQL += " @@Where ORDER BY " + strNewOrder + " " +
                            " ) P_T1 ORDER BY " + strOrder;
                        break;
                    case "COUNT":
                        SQL = "Select COUNT(*) FROM ( " + strSQLInfo + " @@Where) P_Count ";//edit 2008.3.29
                        break;
                    default:
                        SQL = strSQLInfo + strOrder;//还原
                        break;
                }
            }

            //执行分页参数替换
            if (PageSize < 0) PageSize = 0;
            SQL = SQL.Replace("@@PageSize", PageSize.ToString())
                .Replace("@@Page_Size_Number", Convert.ToString(PageSize * PageNumber))
                .Replace("@@LeftSize", PageSize.ToString());//
            //.Replace ("@@Where",strWhere);
            //针对用户的额外条件处理：
            if (strWhere == null) strWhere = "";
            if (strWhere != "" && strWhere.ToUpper().Trim().StartsWith("WHERE "))
            {
                throw new Exception("分页额外查询条件不能带Where谓词！");
            }
            if (!SqlFlag)
            {
                if (strWhere != "") strWhere = " Where " + strWhere;
                SQL = SQL.Replace("@@Where", strWhere);
            }
            else
            {
                if (strWhere != "") strWhere = " And (" + strWhere + ")";
                SQL = SQL.Replace("@@Where", strWhere);
            }
            return SQL;
            #endregion

        }


        /// <summary>
        /// Oracle 分页SQL语句生成器
        /// </summary>
        /// <param name="strSQLInfo">原始SQL语句</param>
        /// <param name="strWhere">在分页前要替换的字符串，用于分页前的筛选</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="PageNumber">页码</param>
        /// <param name="AllCount">记录总数</param>
        /// <returns>生成SQL分页语句</returns>
        private static string MakePageSQLStringByOracle(string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount)
        {
            if (strWhere != null && strWhere != "")
            {
                strWhere = strWhere.Trim().ToUpper();
                if (strWhere.StartsWith("WHERE "))
                    throw new Exception("附加查询条件不能带 where 谓词");
                if (strWhere.IndexOf(" ORDER BY ") > 0)
                    throw new Exception("附加查询条件不能带 ORDER BY 谓词");
                strSQLInfo = "SELECT * FROM (" + strSQLInfo + ") temptable0 WHERE " + strWhere;
            }
            if (AllCount == 0)
            {
                //生成统计语句　
                return "select count(*) from (" + strSQLInfo + ") ";
            }
            //分页摸板语句

            string SqlTemplate = @"SELECT * FROM
 (SELECT rownum r_n,temptable.* FROM  
   ( @@SourceSQL ) temptable Where rowmun <= @@RecEnd
 ) temptable2 WHERE r_n > @@RecStart ";

            int iRecStart = (PageNumber - 1) * PageSize + 1;
            int iRecEnd = PageNumber * PageSize;

            //执行参数替换
            string SQL = SqlTemplate.Replace("@@SourceSQL", strSQLInfo)
                .Replace("@@RecStart", iRecStart.ToString())
                .Replace("@@RecEnd", iRecEnd.ToString());
            return SQL;
        }

        private static string MakePageSQLStringByMySQL_PgSQL(string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount, string offsetString)
        {
            strSQLInfo = strSQLInfo.Trim();
            //去除末尾的分号
            if (strSQLInfo.EndsWith(";"))
                strSQLInfo = strSQLInfo.TrimEnd(';');
            if (strWhere != null && strWhere != "")
            {
                strWhere = strWhere.Trim().ToUpper();
                if (strWhere.StartsWith("WHERE "))
                    throw new Exception("附加查询条件不能带 where 谓词");
                if (strWhere.IndexOf(" ORDER BY ") > 0)
                    throw new Exception("附加查询条件不能带 ORDER BY 谓词");
                strSQLInfo = "SELECT * FROM (" + strSQLInfo + ") temptable0 WHERE " + strWhere;
            }
            if (AllCount == 0)
            {
                //生成统计语句　
                return "select count(*) from (" + strSQLInfo + ") ";
            }

            if (PageNumber == 1)
                return strSQLInfo + " LIMIT " + PageSize;
            int offset = PageSize * PageNumber;
            return strSQLInfo + " LIMIT " + PageSize + offsetString + offset;
        }


        public static string MakePageSQLStringByMySQL(string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount)
        {
            return MakePageSQLStringByMySQL_PgSQL(strSQLInfo, strWhere, PageSize, PageNumber, AllCount,",");
        }

        public static string MakePageSQLStringByPostgreSQL(string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount)
        {
            return MakePageSQLStringByMySQL_PgSQL(strSQLInfo, strWhere, PageSize, PageNumber, AllCount, " offset ");
        }

        public static string MakePageSQLStringBySQLite(string strSQLInfo, string strWhere, int PageSize, int PageNumber, int AllCount)
        {
            return MakePageSQLStringByMySQL_PgSQL(strSQLInfo, strWhere, PageSize, PageNumber, AllCount, ",");
        }

        /// <summary>
        /// 根据主键的高效快速分页之倒序分页
        /// </summary>
        /// <param name="pageNum">页码，从1开始</param>
        /// <param name="pageSize">页大小，大于1</param>
        /// <param name="filedList">字段列表</param>
        /// <param name="tableName">表名称</param>
        /// <param name="PKName">主键名称</param>
        /// <param name="conditon">查询条件</param>
        /// <returns>返回指定页码的快速分页SQL语句</returns>
        public static string GetDescPageSQLbyPrimaryKey(int pageNum, int pageSize, string filedList, string tableName, string PKName, string conditon)
        {
            if (conditon == null || conditon == "")
                conditon = "1=1";
            if (pageNum == 1)
            {
                string sqlTemplage = "Select top @pageSize @filedList from @table1 where  @conditon order by @PKName desc ";
                return sqlTemplage
                    .Replace("@pageSize", pageSize.ToString())
                    .Replace("@filedList", filedList)
                    .Replace("@table1", tableName)
                    .Replace("@conditon", conditon)
                    .Replace("@PKName", PKName);
            }
            else
            {
                //@topNum= ((页码-1)*页大小)
                string sqlTemplage = @"
select top @pageSize @filedList
from @table1
where @conditon And @PKName<
      (select min (@PKName) from
      (select top @topNum @PKName from @table1 where @conditon order by @PKName desc) as T
       )    
  order by @PKName desc
";
                int topNum = (pageNum - 1) * pageSize;

                return sqlTemplage.Replace("@topNum", topNum.ToString())
                   .Replace("@pageSize", pageSize.ToString())
                   .Replace("@filedList", filedList)
                   .Replace("@table1", tableName)
                   .Replace("@conditon", conditon)
                   .Replace("@PKName", PKName);

            }
        }

        /// <summary>
        /// 根据主键的高效快速分页之 升序分页
        /// </summary>
        /// <param name="pageNum">页码，从1开始</param>
        /// <param name="pageSize">页大小，大于1</param>
        /// <param name="filedList">字段列表</param>
        /// <param name="tableName">表名称</param>
        /// <param name="PKName">主键名称</param>
        /// <param name="conditon">查询条件</param>
        /// <returns>返回指定页码的快速分页SQL语句</returns>
        public static string GetAscPageSQLbyPrimaryKey(int pageNum, int pageSize, string filedList, string tableName, string PKName, string conditon)
        {
            if (conditon == null || conditon == "")
                conditon = "1=1";
            if (pageNum == 1)
            {
                string sqlTemplage = "Select top @pageSize @filedList from @table1 where  @conditon order by @PKName desc ";
                return sqlTemplage
                    .Replace("@pageSize", pageSize.ToString())
                    .Replace("@filedList", filedList)
                    .Replace("@table1", tableName)
                    .Replace("@conditon", conditon)
                    .Replace("@PKName", PKName);
            }
            else
            {
                //@topNum= ((页码-1)*页大小)
                string sqlTemplage = @"
select top @pageSize @filedList
from @table1
where @conditon And @PKName>
      (select max (@PKName) from
      (select top @topNum @PKName from @table1 where @conditon order by @PKName asc) as T
       )    
  order by @PKName asc
";
                int topNum = (pageNum - 1) * pageSize;

                return sqlTemplage.Replace("@topNum", topNum.ToString())
                   .Replace("@pageSize", pageSize.ToString())
                   .Replace("@filedList", filedList)
                   .Replace("@table1", tableName)
                   .Replace("@conditon", conditon)
                   .Replace("@PKName", PKName);

            }
        }
    }







}
