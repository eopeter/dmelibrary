﻿using System;
using System.Collections.Generic;
using System.Text;
using DME.DataBase;

namespace DME.DataBase.DataMap.Entity
{

    /// <summary>
    /// 实体对象查询表达式
    /// </summary>
    public class DMEDb_OQL
    {
        private DMEDb_OQL1 oql1;
        private DME.DataBase.Common.DMEDb_EntityMapType _mapType;

        private DMEDb_EntityBase currEntity;
        private List<DMEDb_EntityBase> joinedEntitys=new List<DMEDb_EntityBase> ();
        protected internal bool isJoinOpt;//是否是连接操作
        protected internal bool HaveJoinOpt;//是否已经发生了连接操作
        private string joinedString=string.Empty ;

        private List<string> selectedFields = new List<string>();

        private bool hasSelected = false;
        private string optFlag = "SELECT ";
        private string sqlUpdate = "";
        private Dictionary<string, object> updateParameters = null;

        protected internal string sql_fields = string.Empty;
        protected internal string sql_table = string.Empty;
        protected internal string sql_condition = string.Empty;

        /// <summary>
        /// 实体类映射的类型
        /// </summary>
        public DME.DataBase.Common.DMEDb_EntityMapType EntityMap
        {
            get { return _mapType; }
            protected set { _mapType = value; }
        }

        /// <summary>
        /// 条件表达式
        /// </summary>
        public DMEDb_OQL2 Condition;

        /// <summary>
        /// 查询前N条记录，目前仅支持Access/SqlServer，其它数据库可以使用Limit(N) 方法替代。
        /// </summary>
        public int TopCount=0;
        /// <summary>
        /// 是否开启分页功能，如果启用，OQL不能设定“排序”信息，分页标识字段将作为排序字段
        /// </summary>
        public bool PageEnable = false;
        /// <summary>
        /// 分页时候每页的记录大小，默认为10
        /// </summary>
        public int PageSize = 10;
        /// <summary>
        /// 分页时候的当前页码，默认为1
        /// </summary>
        public int PageNumber = 1;
        /// <summary>
        /// 分页时候的记录标识字段，默认为主键字段。不支持多主键。
        /// </summary>
        public string PageField = "";
        /// <summary>
        /// 分页的时候记录按照倒序排序（对Oracle数据库不起效）
        /// </summary>
        public bool PageOrderDesc = true;
        /// <summary>
        /// 分页的时候，记录的总数量，如未设置虚拟为999条。如需准确分页，应设置该值。
        /// </summary>
        public int PageWithAllRecordCount = 999;

        /// <summary>
        /// 是否排除重复记录
        /// </summary>
        public bool Distinct;

        /// <summary>
        /// 限制查询的记录数量，对于SQLSERVER/ACCESS，将采用主键作为标识的高速分页方式。
        /// 注：调用该方法不会影响生OQL.ToString()结果，仅在最终执行查询的时候才会去构造当前特点数据库的SQL语句。
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public DMEDb_OQL Limit(int pageSize)
        {
            this.PageEnable = true;
            this.PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// 限制查询的记录数量，对于SQLSERVER/ACCESS，将采用主键作为标识的高速分页方式。
        /// 注：调用该方法不会影响生OQL.ToString()结果，仅在最终执行查询的时候才会去构造当前特点数据库的SQL语句。
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageNumber">页号码</param>
        /// <returns></returns>
        public DMEDb_OQL Limit(int pageSize,int pageNumber)
        {
            this.PageEnable = true;
            this.PageSize = pageSize;
            this.PageNumber = pageNumber;
            return this;
        }

        /// <summary>
        /// 限制查询的记录数量，对于SQLSERVER/ACCESS，将采用指定字段作为标识的高速分页方式。
        /// 注：调用该方法不会影响生OQL.ToString()结果，仅在最终执行查询的时候才会去构造当前特点数据库的SQL语句。
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageNumber">页号码</param>
        /// <param name="pageField">要排序的字段</param>
        /// <returns></returns>
        public DMEDb_OQL Limit(int pageSize, int pageNumber,string pageField)
        {
            this.PageEnable = true;
            this.PageSize = pageSize;
            this.PageNumber = pageNumber;
            this.PageField = pageField;
            return this;
        }

        /// <summary>
        /// 重新初始化实体对象查询表达式的内部状态
        /// </summary>
        public void ReSet()
        {
            selectedFields.Clear();
            hasSelected = false;
            isJoinOpt = false;
            HaveJoinOpt = false;
            optFlag = "SELECT ";
            sqlUpdate = "";
            updateParameters = null;

            TopCount = 0;
            PageEnable = false;
        }

               
        /// <summary>
        /// 获取条件参数
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get {
                if (optFlag == "UPDATE ")
                {
                    Dictionary<string, object> paras = new Dictionary<string, object>();
                    foreach (string key in updateParameters.Keys )
                        paras .Add (key ,updateParameters [key ]);
                    foreach (string key in oql1.Parameters.Keys)
                        paras.Add(key, oql1.Parameters[key]);

                    return paras;
                }
                return oql1.Parameters ;
            }
        }

        /// <summary>
        /// 使用一个实体对象初始化实体对象查询表达式
        /// </summary>
        /// <param name="e">实体对象</param>
        public DMEDb_OQL(DMEDb_EntityBase e)
        {
            this.currEntity = e;
            this.Condition = new DMEDb_OQL2(e);
            this.EntityMap = e.EntityMap;
            this.sql_table = this.currEntity.TableName;
            this.currEntity.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(currEntity_PropertyGetting);
        }

        private void currEntity_PropertyGetting(object sender, PropertyGettingEventArgs e)
        {
            doPropertyGetting(((DMEDb_EntityBase)sender).TableName, e.PropertyName);
        }

        /// <summary>
        /// 静态实体对象表达式
        /// </summary>
        /// <param name="e">实体对象实例</param>
        /// <returns>实体对象查询表达式</returns>
        public static DMEDb_OQL From(DMEDb_EntityBase e)
        {
           return  new DMEDb_OQL(e);
        }
        #region 处理实体连接

        /// <summary>
        /// 内连接查询
        /// </summary>
        /// <param name="e">要连接的实体对象</param>
        /// <returns>连接对象</returns>
        public DMEDb_JoinEntity  Join(DMEDb_EntityBase e)
        {
            return Join(e, "Inner Join");
        }

        /// <summary>
        /// 内连接查询
        /// </summary>
        /// <param name="e">要连接的实体对象</param>
        /// <returns>连接对象</returns>
        public DMEDb_JoinEntity InnerJoin(DMEDb_EntityBase e)
        {
            return Join(e, "Inner Join");
        }
        /// <summary>
        /// 左连接查询
        /// </summary>
        /// <param name="e">要连接的实体对象</param>
        /// <returns>连接对象</returns>
        public DMEDb_JoinEntity LeftJoin(DMEDb_EntityBase e)
        {
            return Join(e, "Left Join");
        }
        /// <summary>
        /// 右连接查询
        /// </summary>
        /// <param name="e">要连接的实体对象</param>
        /// <returns>连接对象</returns>
        public DMEDb_JoinEntity RightJoin(DMEDb_EntityBase e)
        {
            return Join(e, "Left Join");
        }


        private DMEDb_JoinEntity Join(DMEDb_EntityBase e,string joinTypeString)
        {
            this.joinedEntitys.Add(e);
            this.isJoinOpt = true;
            this.joinedString = string.Format (" {0} [" + e.TableName + "] On ",joinTypeString );

            e.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(e_PropertyGetting);
            DMEDb_JoinEntity je = new DMEDb_JoinEntity(this);
            return je;
        }

        void e_PropertyGetting(object sender, PropertyGettingEventArgs e)
        {
            doPropertyGetting(((DMEDb_EntityBase)sender).TableName, e.PropertyName);
        }

        private void doPropertyGetting(string tableName, string propertyName)
        {
            if (isJoinOpt)
            {
                string propName = "[" + tableName + "].[" + propertyName + "]";
                if (this.joinedString.EndsWith(" On "))
                    this.joinedString += " " + propName;
                else
                    this.joinedString += " = " + propName;
            }
            else
            {
                string field = this.joinedString.Length > 0 ? tableName + "].[" + propertyName : propertyName;
                if (!hasSelected && !selectedFields.Contains(field))
                    selectedFields.Add(field);
            }
        }

        #endregion

        /// <summary>
        /// 选取实体对象的属性
        /// </summary>
        /// <param name="fields">属性字段列表</param>
        /// <returns>实体对象查询基本表达式</returns>
        public DMEDb_OQL1 Select(params object[] fields)
        {
            hasSelected = true;
            if (fields.Length == 0)
                selectedFields.Clear();
            //注意 有可能OQL对象会重复使用，所以下面的事件不能被注销
            //this.currEntity.PropertyGetting -= new EventHandler<PropertyGettingEventArgs>(currEntity_PropertyGetting);
            
            oql1 = new DMEDb_OQL1(this.currEntity,this);
            oql1.Selected = true;
            return oql1;
        }

       
        /// <summary>
        /// 使用是否排除重复记录的方式，来选取实体对象的属性
        /// </summary>
        /// <param name="distinct"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public DMEDb_OQL1 Select(bool distinct, params object[] fields)
        {
            this.Distinct = distinct;
            return Select(fields);
        }

        /// <summary>
        /// 更新实体类的某些属性值，如果未指定条件，则使用主键值为条件。
        /// </summary>
        /// <param name="fields">实体熟悉列表</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL1 Update(params object[] fields)
        {
            optFlag = "UPDATE ";
            sqlUpdate = "UPDATE [" + this.currEntity.TableName +"] SET ";
            updateParameters = new Dictionary<string, object>();
            foreach (string field in selectedFields)
            {
                sqlUpdate  +="["+  field + "]=@" + field+",";
                updateParameters.Add(field, this.currEntity.PropertyList(field));//某些数据库不支持"@" 格式的参数名称
            }
            sqlUpdate = sqlUpdate.TrimEnd(',');

            oql1 = new DMEDb_OQL1(this.currEntity,this);
            oql1.Selected = true;
            return oql1;
        }

        /// <summary>
        /// 执行自操作的字段更新，比如为某一个数值性字段执行累加
        /// </summary>
        /// <param name="selfOptChar">自操作类型，有+，-，*，/ 四种类型</param>
        /// <param name="fields">字段列表</param>
        /// <returns></returns>
        public DMEDb_OQL1 UpdateSelf(char selfOptChar,params object[] fields)
        {
            if (selfOptChar == '+' || selfOptChar == '-' || selfOptChar == '*' || selfOptChar == '/')
            {
                optFlag = "UPDATE ";
                sqlUpdate = "UPDATE [" + this.currEntity.TableName + "] SET ";
                updateParameters = new Dictionary<string, object>();
                foreach (string field in selectedFields)
                {
                    sqlUpdate += "[" + field + "]=[" + field + "] "+selfOptChar +" @" + field + ",";
                    updateParameters.Add(field, this.currEntity.PropertyList(field));//某些数据库不支持"@" 格式的参数名称
                }
                sqlUpdate = sqlUpdate.TrimEnd(',');

                oql1 = new DMEDb_OQL1(this.currEntity, this);
                oql1.Selected = true;
                return oql1;
            }
            throw new Exception("OQL的字段自操作只能是+，-，*，/ 四种类型");
        }

        /// <summary>
        /// 删除实体类，如果未指定条件，则使用主键值为条件。
        /// </summary>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL1 Delete()
        {
            optFlag = "DELETE ";
            sqlUpdate = "DELETE FROM [" + this.currEntity.TableName + "] ";
            
            oql1 = new DMEDb_OQL1(this.currEntity,this);
            oql1.Selected = true;
            return oql1;
        }

        /// <summary>
        /// 根据用户自定义的查询（临时视图），从该查询进一步获取指定的记录的查询语句
        /// </summary>
        /// <param name="tempViewSql">作为子表的用户查询（临时视图）</param>
        /// <returns>符合当前限定条件的查询语句</returns>
        public string GetMapSQL(string tempViewSql)
        {
            if (string .IsNullOrEmpty (tempViewSql ))
                throw new Exception ("用户的子查询不能为空。");
            tempViewSql = " (" + tempViewSql + " ) tempView ";

            string sql = "SELECT ";
            if (TopCount > 0)
                sql += " Top " + TopCount + " ";//仅限于SQLSERVER/ACCESS
            if (selectedFields.Count == 0)
                sql += "*";
            else
            {
                foreach (string field in selectedFields)
                {
                    sql += "["+field + "],";
                }
                sql = sql.TrimEnd(',');
            }
            if (this.PageEnable)
                this.sql_fields = sql.Substring("SELECT ".Length);

            string groupString = oql1.GroupByFields;
            if (groupString.Length > 0)
                sql = "SELECT " + oql1.GroupByFields + oql1.sqlFunctionString + " \r\n FROM " + tempViewSql + "\r\n  " + oql1.ToString();
            else if (oql1.sqlFunctionString.Length > 0)
                sql = "SELECT " + oql1.sqlFunctionString.Substring(1) + " \r\n FROM " + tempViewSql + "\r\n  " + oql1.ToString();
            else
            {
                string condition = oql1.ToString();
                sql += " \r\n FROM " + tempViewSql + "\r\n  " + condition;
                if (this.PageEnable)
                {
                    this.sql_table = tempViewSql;
                    int len = " Where ".Length;
                    this.sql_condition = condition.Length > len ? condition.Substring(len) : "";
                    if (this.PageField == "")
                    {
                        if (this.currEntity.PrimaryKeys == null || this.currEntity.PrimaryKeys.Count == 0)
                            throw new Exception("OQL 分页错误，没有指明分页标识字段，也未给当前实体类设置主键。");
                        this.PageField = this.currEntity.PrimaryKeys[0];
                    }
                }
            }
            return sql;
        }

        
        /// <summary>
        /// 获取SQL表达式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string sql = optFlag;// "SELECT ";
            if (optFlag == "SELECT ")
            {
                if (this.Distinct)
                    sql += " DISTINCT ";
                if (TopCount > 0)
                    sql += " Top " + TopCount + " ";//仅限于SQLSERVER/ACCESS
                if (selectedFields.Count == 0)
                {
                    sql += string.Join(",", DMEDb_CommonUtil.PrepareSqlFields(this.currEntity.PropertyNames));  //edit at 11.10 取消原来的
                }
                else
                {
                    //foreach (string field in selectedFields)
                    //{
                    //    sql += field + ",";
                    //}
                    //sql = sql.TrimEnd(',');
                    sql += string.Join(",", DMEDb_CommonUtil.PrepareSqlFields(selectedFields.ToArray()));
                }
                //if (this.PageEnable)
                this.sql_fields = sql.Substring("SELECT ".Length);

                string groupString = oql1.GroupByFields;
                if (groupString.Length > 0)
                    sql = "SELECT " + oql1.GroupByFields + oql1.sqlFunctionString + " \r\n FROM [" + this.currEntity.TableName + "]\r\n  " + oql1.ToString();
                else if (oql1.sqlFunctionString.Length > 0)
                    sql = "SELECT " + oql1.sqlFunctionString.Substring(1) + " \r\n FROM [" + this.currEntity.TableName + "]\r\n  " + oql1.ToString();
                else
                {
                    string condition = oql1.ToString();
                    sql += " \r\n FROM [" + this.currEntity.TableName + "]\r\n  " +this.joinedString +" "+  condition;
                    if (this.PageEnable)
                    {

                        int len = " Where ".Length;
                        this.sql_condition = condition.Length > len ? condition.Substring(len) : "";
                        if (this.PageField == "")
                        {
                            if (this.currEntity.PrimaryKeys == null || this.currEntity.PrimaryKeys.Count == 0)
                                throw new Exception("OQL 分页错误，没有指明分页标识字段，也未给当前实体类设置主键。");
                            this.PageField = this.currEntity.PrimaryKeys[0];
                        }
                    }
                }

            }
            else
            {
                //如果未限定条件，则使用主键的值
                string whereString = oql1.ToString();
                if (whereString.Length < 8)
                {
                    whereString = " Where 1=1 ";
                    //updateParameters
                    if (this.currEntity.PrimaryKeys.Count == 0)
                        throw new Exception("未指定更新实体的范围，也未指定实体的主键。");
                    foreach (string pk in this.currEntity.PrimaryKeys)
                    {
                        whereString += " And [" + pk + "] =@" + pk + ",";
                        updateParameters.Add( pk, this.currEntity.PropertyList(pk));
                    }
                    whereString = whereString.TrimEnd(',');
                }
                sql = sqlUpdate + "\r\n  " + whereString;
            }
            
            return sql;
        }

    }

    /// <summary>
    /// 基本表达式
    /// </summary>
    public class DMEDb_OQL1
    {
        /// <summary>
        /// 基本表达式中的当前实体对象
        /// </summary>
        protected DMEDb_EntityBase CurrEntity;

        private DMEDb_OQL CurrOQL;

        /// <summary>
        /// 是否已经执行完OQL之Select方法
        /// </summary>
        public bool Selected = false;

        string whereString = "";
        string orderString = "";
        string groupString = "";
        private string currFieldName = "";
        private bool useWhereMethod=false ;
        /// <summary>
        /// Select 中的函数名字符串。例如Count，Max，Min 等
        /// </summary>
        protected internal  string sqlFunctionString = "";

        //private List<string> WhereFields = new List<string>();
        private Dictionary<string, object> paras;
        /// <summary>
        /// 获取条件参数
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get { return paras; }
        }

        /// <summary>
        /// 获取分组自动列表
        /// </summary>
        protected internal  string GroupByFields
        {
            get {
                if (groupString.Length > 0)
                {
                    return  groupString.Substring(1);
                }
                return "";
            }
        }

        /// <summary>
        /// 使用实体对象初始化构造函数
        /// </summary>
        /// <param name="e"></param>
        public DMEDb_OQL1(DMEDb_EntityBase e,DMEDb_OQL q)
        {
            this.CurrEntity = e;
            this.CurrOQL = q;
            this.CurrEntity.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
        }

       private  void CurrEntity_PropertyGetting(object sender, PropertyGettingEventArgs e)
        {
            string propName = "";
            if (this.CurrOQL.HaveJoinOpt )
            {
                //如果是连接操作，条件字段需要增加上查询的表名称
                string tableName = ((DMEDb_EntityBase)sender).TableName;
                propName = "[" + tableName + "].[" + e.PropertyName + "]";
            }
            else
            {
                propName = "[" + e.PropertyName + "]";
            }

            if (Selected & !useWhereMethod)
            {
                if (paras == null)
                    paras = new Dictionary<string, object>();
                if (!paras.ContainsKey(propName))
                {
                    //在选取Where字段，Where方法尚未执行完成
                    paras.Add(propName, this.CurrEntity.PropertyList(e.PropertyName));
                }
            }
            currFieldName = propName;// "[" + e.PropertyName + "]";

        }

      

        /// <summary>
        /// 获取复杂查询条件
        /// </summary>
        /// <param name="c">多条件表达式</param>
        /// <returns>基本表达式</returns>
        public DMEDb_OQL1 Where(DMEDb_OQL2 c)
        {
            whereString = " Where "+c.ConditionString;
            paras = c.Parameters;

            useWhereMethod = true;
            //this.CurrEntity.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
            return this;
        }

        /// <summary>
        /// 获取复杂查询条件(具有逻辑优先级的复杂比较条件）
        /// </summary>
        /// <param name="compare">实体对象比较类</param>
        /// <returns>基本表达式</returns>
        public DMEDb_OQL1 Where(DMEDb_OQLCompare compare)
        {
            whereString = " Where " + compare.CompareString;
            paras = compare.ComparedParameters;

            useWhereMethod = true;
            //this.CurrEntity.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
            return this;
        }

        /// <summary>
        /// 获取并列查询条件，如 Where(u.Uid,u.Name);
        /// </summary>
        /// <param name="expression">实体属性列表</param>
        /// <returns>基本表达式</returns>
        public DMEDb_OQL1 Where(params object[] expression)
        {
            if (expression.Length > 0)
            {

                string str = "";
                int count = 0;
                Dictionary<string, object> tempParas = new Dictionary<string, object>();
                foreach (string field in paras.Keys )
                {
                    if (count >= expression.Length)//避免调试陷阱
                        break;
                    string paraName = "P" + count;
                    tempParas.Add(paraName, paras[field]);

                    str += " AND " + field + "=@" + paraName;
                    count++;
                }
                if (str != "")
                {
                    str = str.Substring(" AND ".Length);
                    paras = tempParas;                
                }
                    
                whereString = " Where " + str;
            }
            useWhereMethod = true;
            //Order by 可能还需要
            //this.CurrEntity.PropertyGetting -= new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
            return this;
        }

        /// <summary>
        /// 根据传入的查询参数数组，对字段名执行不区分大小写的比较，生成查询条件。
        /// </summary>
        /// <param name="queryParas">查询参数数组</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL1 Where(DMEDb_QueryParameter[] queryParas)
        {
            if (paras == null)
                paras = new Dictionary<string, object>();
           
            string[] fields = this.CurrEntity.PropertyNames;
            string str = "";
            int count=0;
            foreach (DMEDb_QueryParameter para in queryParas)
            {
                foreach (string temp in fields)//比较字段是否在实体类中
                {
                    if (string.Compare(temp, para.FieldName, true) == 0)
                    {
                        string paraName = temp + (count++);
                        if (!paras.ContainsKey(paraName))
                        {
                            paras.Add(paraName, para.FieldValue);
                            string cmpType = "";
                            switch (para.CompareType)
                            {
                                case DME.DataBase.Common.DMEDb_enumCompare.Equal:
                                    cmpType = "=";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.Greater:
                                    cmpType = ">";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.Like:
                                    cmpType = " LIKE ";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.NoGreater:
                                    cmpType = "<=";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.NoSmaller:
                                    cmpType = ">=";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.NotEqual:
                                    cmpType = "<>";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.Smaller:
                                    cmpType = "<";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.IsNull:
                                    cmpType = " IS NULL ";
                                    break;
                                case DME.DataBase.Common.DMEDb_enumCompare.IsNotNull:
                                    cmpType = " IS NOT NULL ";
                                    break;
                                default :
                                    cmpType = "=";
                                    break;
                            }
                            if (para.CompareType != DME.DataBase.Common.DMEDb_enumCompare.IsNull && para.CompareType != DME.DataBase.Common.DMEDb_enumCompare.IsNotNull)
                                str += " AND [" + temp +"]"+ cmpType + "@" + paraName;
                            else
                                str += " AND [" + temp + "]" + cmpType;
                        }
                        break;
                    }
                }
            }
           
            if (str != "")
                str = str.Substring(" AND ".Length);

            whereString = " Where " + str;
            useWhereMethod = true;
            return this;
        }

        /// <summary>
        /// 设定分组字段。分组字段将自动作为选择的候选字段，而且覆盖原有选择的字段。
        /// </summary>
        /// <param name="field">分组字段，如果有多个，请继续调用本方法</param>
        /// <returns>OQL1</returns>
        public DMEDb_OQL1 GroupBy(object field)
        {
            if (!useWhereMethod)
            {
                //可能没有调用Where方法
                paras = null;
            }
            groupString += "," + currFieldName;
            return this;
        }
       
        /// <summary>
        /// 设定排序条件
        /// </summary>
        /// <param name="field">实体对象属性</param>
        /// <param name="orderType">排序类型 ASC，DESC</param>
        /// <returns></returns>
        public DMEDb_OQL1 OrderBy(object field, string orderType)
        {
            if (!useWhereMethod)
            {
                //可能没有调用Where方法
                paras = null;
            }
            if (string.IsNullOrEmpty(orderType))
                orderType = "ASC";

            orderString += ","+currFieldName+" "+orderType;
            return this;
        }

        /// <summary>
        /// OQL1表达式之统计数量，请在结果实体类中使用PropertyList["字段别名"] 的方式获取查询值
        /// </summary>
        /// <param name="field">属性字段</param>
        /// <param name="asFieldName">别名，如果不指定，则使用字段名称</param>
        /// <returns>OQL1</returns>
        public DMEDb_OQL1 Count(object field, string asFieldName)
        {
            return sqlFunction("COUNT", currFieldName, asFieldName);
        }

        /// <summary>
        /// OQL1表达式之求最大值，请在结果实体类中使用PropertyList["字段别名"] 的方式获取查询值
        /// </summary>
        /// <param name="field">属性字段</param>
        /// <param name="asFieldName">别名，如果不指定，则使用字段名称</param>
        /// <returns>OQL1</returns>
        public DMEDb_OQL1 Max(object field, string asFieldName)
        {
            return sqlFunction("MAX", currFieldName, asFieldName);
        }

        /// <summary>
        /// OQL1表达式之求最小值，请在结果实体类中使用PropertyList["字段别名"] 的方式获取查询值
        /// </summary>
        /// <param name="field">属性字段</param>
        /// <param name="asFieldName">别名，如果不指定，则使用字段名称</param>
        /// <returns>OQL1</returns>
        public DMEDb_OQL1 Min(object field, string asFieldName)
        {
            return sqlFunction("MIN", currFieldName, asFieldName);
        }

        /// <summary>
        /// OQL1表达式之求合计，请在结果实体类中使用PropertyList["字段别名"] 的方式获取查询值
        /// </summary>
        /// <param name="field">属性字段</param>
        /// <param name="asFieldName">别名，如果不指定，则使用字段名称</param>
        /// <returns>OQL1</returns>
        public DMEDb_OQL1 Sum(object field, string asFieldName)
        {
            return sqlFunction("SUM", currFieldName, asFieldName);
        }

        /// <summary>
        /// OQL1表达式之求平均，请在结果实体类中使用PropertyList["字段别名"] 的方式获取查询值
        /// </summary>
        /// <param name="field">属性字段</param>
        /// <param name="asFieldName">字段别名，如果不指定，则使用字段名称</param>
        /// <returns>OQL1</returns>
        public DMEDb_OQL1 Avg(object field, string asFieldName)
        {
            return sqlFunction("AVG", currFieldName, asFieldName);
        }

        private  DMEDb_OQL1 sqlFunction(string sqlFunctionName, string  fieldName, string asFieldName)
        {
            //paras.Remove(currFieldName.Replace("[", "").Replace("]", "")); //必须清除Where方法之前的键
            paras.Remove(currFieldName); //必须清除Where方法之前的键

            if (string.IsNullOrEmpty(asFieldName))
                asFieldName = fieldName;
            sqlFunctionString += "," + sqlFunctionName + "(" + fieldName + ") AS " + asFieldName;
            this.CurrEntity.setProperty(asFieldName, 0);
            return this;
        }



        /// <summary>
        /// 获取基本的条件语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string temp = "";
            if (orderString != "")
                temp = "    Order by " + orderString.TrimStart (',');
            
            if(this.GroupByFields.Length >0)
                return whereString +"    Group By "+ this.GroupByFields + "\r\n    " + temp;
            else
                return whereString + "\r\n    " + temp;
        }

        /// <summary>
        /// OQL表达式构造结束
        /// </summary>
        public DMEDb_OQL END
        {
            get { return this.CurrOQL; }
        }

    }

    /// <summary>
    /// 条件表达式
    /// </summary>
    public class DMEDb_OQL2
    {
        private DMEDb_EntityBase CurrEntity;
        private Dictionary<string, object> paras = new Dictionary<string, object>();
        private string currFieldName = "";
        private string currParaName = "";
        private string _conditionString = "";
        private int paraIndex = 0;

        /// <summary>
        /// 获取条件参数
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get { return paras; }
        }

        /// <summary>
        /// 使用一个实体对象，初始化条件表达式
        /// </summary>
        /// <param name="e">实体对象实例</param>
        public DMEDb_OQL2(DMEDb_EntityBase e)
        {
            this.CurrEntity = e;
            this.CurrEntity.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
        }

        private void CurrEntity_PropertyGetting(object sender, PropertyGettingEventArgs e)
        {
            //int n = 0;
            //while (paras.ContainsKey(e.PropertyName  + n))
            //    n++;
            //currParaName = e.PropertyName.Replace(" ", "") + n;//消除实际字段名中的空格
            //PostgreSQL的dotConnect驱动不支持参数名中的中文，故注释上面的代码，采用数字编号
            currParaName = "P"+this.paraIndex;
            currFieldName = "["+e.PropertyName+"]";
            this.paraIndex++;
          
        }

        private  DMEDb_OQL2 subCondition(string logicType, object field, string compareType, object Value)
        {
            if (compareType == null || compareType == "")
                compareType = "=";
            else
                compareType = compareType.Trim().ToLower();

            if (compareType == "=" || compareType == ">=" || compareType == ">" || compareType == "<=" || compareType == "<" || compareType == "<>" || compareType.StartsWith("like"))
            {
                _conditionString += logicType + currFieldName + " " + compareType + " @" + currParaName;
                paras.Add(currParaName, Value);
            }
            else if (compareType.StartsWith("is"))
            {
                _conditionString += logicType + currFieldName + " IS " + Value.ToString ();
            }
            else
            {
                throw new Exception("比较符号必须是 =,>,>=,<,<=,<>,like,is 中的一种。");
            }
            return this;
        }

       /// <summary>
        /// 选取 与 条件
       /// </summary>
       /// <param name="field">实体对象的属性</param>
       /// <param name="compareType">SQL 比较条件，如"=","LIKE","IS" 等</param>
       /// <param name="Value">要比较的值</param>
       /// <returns>多条件表达式</returns>
        public DMEDb_OQL2 AND(object field, string compareType,object Value)
        {
            return subCondition(" AND ", field, compareType, Value);
        }

        /// <summary>
        /// 获取并列条件集合（待实现）
        /// </summary>
        /// <param name="cmps">条件集合</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL2 AND(params  DMEDb_OQLCompare[] cmps)
        {

            return this;
        }

       /// <summary>
        /// 选取 或 条件
       /// </summary>
        /// <param name="field">实体对象的属性</param>
        /// <param name="compareType">SQL 比较条件，如"=","LIKE","IS" 等</param>
        /// <param name="Value">要比较的值</param>
        /// <returns>多条件表达式</returns>
        public DMEDb_OQL2 OR(object field, string compareType, object Value)
        {
            return subCondition(" OR  ", field, compareType, Value);
           
        }

        /// <summary>
        /// 获取 或者 条件集合（待实现）
        /// </summary>
        /// <param name="cmps">条件集合</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL2 OR(params  DMEDb_OQLCompare[] cmps)
        {

            return this;
        }

       /// <summary>
        /// 选取 非 条件
       /// </summary>
        /// <param name="field">实体对象的属性</param>
        /// <param name="compareType">SQL 比较条件，如"=","LIKE","IS" 等</param>
        /// <param name="Value">要比较的值</param>
        /// <returns>多条件表达式</returns>
        public DMEDb_OQL2 NOT(object field, string compareType, object Value)
        {
            return subCondition(" NOT ", field, compareType, Value);
           
        }

        /// <summary>
        /// 获取 非 条件集合（待实现）
        /// </summary>
        /// <param name="cmps">条件集合</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL2 NOT(params  DMEDb_OQLCompare[] cmps)
        {

            return this;
        }

        /// <summary>
        /// 选取 字段 列表条件
        /// </summary>
        /// <param name="field">实体对象的属性</param>
        /// <param name="Values">值列表</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL2 IN(object field, object[] Values)
        {
            //string temp = "";
            //foreach (object obj in Values)
            //{
            //    if (obj != null)
            //    {
            //        if (obj is string)
            //            temp += ",'" + obj.ToString() + "'";
            //        else
            //            temp += "," + obj.ToString();
            //    }
            //    else
            //        temp += ",NULL";
            
            //}
            //if (temp != "")
            //{
            //    _conditionString += " AND " + currFieldName + " IN("+temp.TrimStart (',')+")";
            //}
            _conditionString += getInCondition(Values, " IN ");
            return this;
        }

        /// <summary>
        /// 构造Not In查询条件
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="Values">值数组</param>
        /// <returns></returns>
        public DMEDb_OQL2 NotIn(object field, object[] Values)
        {
            _conditionString += getInCondition(Values, " NOT IN ");
            return this;
        }


        private string getInCondition(object[] Values,string inWords)
        {
            string temp = "";
            foreach (object obj in Values)
            {
                if (obj != null)
                {
                    if (obj is string)
                        temp += ",'" + obj.ToString() + "'";
                    else
                        temp += "," + obj.ToString();
                }
                else
                    temp += ",NULL";

            }
            if (temp != "")
            {
                return " AND " + currFieldName +inWords+ " (" + temp.TrimStart(',') + ")";
            }
            return "";
        }

        /// <summary>
        /// 选取 字段 列表条件
        /// </summary>
        /// <param name="field">实体对象的属性</param>
        /// <param name="Values">值列表</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL2 IN(object field, string[] Values)
        {
            //string temp = "";
            //foreach (string obj in Values)
            //    temp += ",'" + obj+"'";
            //if (temp != "")
            //{
            //    _conditionString += " AND " + currFieldName + " IN(" + temp.TrimStart(',') + ")";
            //}
            _conditionString += getStringInCondition(Values, " IN ");
            return this;
        }

        /// <summary>
        /// Not In 条件
        /// </summary>
        /// <param name="field">实体对象的属性</param>
        /// <param name="Values">值列表</param>
        /// <returns>条件表达式</returns>
        public DMEDb_OQL2 NotIn(object field, string[] Values)
        {
            _conditionString += getStringInCondition(Values, " NOT IN ");
            return this;
        }

        private string getStringInCondition(object[] Values, string inWords)
        {
            string temp = "";
            foreach (string obj in Values)
                temp += ",'" + obj + "'";
            if (temp != "")
            {
                return " AND " + currFieldName +inWords+ " (" + temp.TrimStart(',') + ")";
            }
            return "";
        }

        /// <summary>
        /// 以另外一个OQL条件作为In的子查询
        /// </summary>
        /// <seealso cref="http://www.cnblogs.com/bluedoctor/archive/2011/02/24/1963606.html"/>
        /// <param name="field">属性字段</param>
        /// <param name="q">OQL表达式</param>
        /// <returns></returns>
        public DMEDb_OQL2 IN(object field, DMEDb_OQL q)
        {
            //if (q.sql_fields.IndexOf(',') > 0)
            //    throw new Exception("OQL 语法错误，包含在In查询中的子查询只能使用1个实体属性，请修改子查询的Select参数。");
            //_conditionString += " AND " + currFieldName + " IN (\r\n" + q.ToString () + ")";

            //foreach (string key in q.Parameters.Keys)
            //    this.paras.Add(key, q.Parameters[key]);
                
            //return this;
            return IN(field, q, true);
        }

        /// <summary>
        /// 以另外一个OQL条件作为Not In的子查询
        /// </summary>
        /// <seealso cref="http://www.cnblogs.com/bluedoctor/archive/2011/02/24/1963606.html"/>
        /// <param name="field">属性字段</param>
        /// <param name="q">OQL表达式</param>
        /// <returns></returns>
        public DMEDb_OQL2 NotIn(object field, DMEDb_OQL q)
        {
            return IN(field, q, false );
        }

        private DMEDb_OQL2 IN(object field, DMEDb_OQL q,bool isIn)
        {
            string inString = isIn ? " IN " : " NOT IN ";
            if (q.sql_fields.IndexOf(',') > 0)
                throw new Exception("OQL 语法错误，包含在In查询中的子查询只能使用1个实体属性，请修改子查询的Select参数。");
            string childSql = q.ToString().Replace("@P","@INP");
            _conditionString += " AND " + currFieldName + inString + "  (\r\n" + childSql + ")";

            foreach (string key in q.Parameters.Keys)
                this.paras.Add("IN"+key , q.Parameters[key]);

            return this;
        }

        /// <summary>
        /// 获取条件字符串
        /// </summary>
        public string ConditionString
        {
            get
            {
                return _conditionString == "" ? "" : _conditionString.Substring(" And".Length);
            }
        }

    }


    /// <summary>
    /// 实体对象条件比较类，用于复杂条件比较表达式
    /// </summary>
    public class DMEDb_OQLCompare
    {
        private DMEDb_EntityBase CurrEntity;
        private List<DMEDb_EntityBase> joinedEntityList;
        private string _compareString = "";
        private string currPropName = "";
        //private List<string> propertyList = new List<string>();
        //private List<object> compareValueList = new List<object>();
        //private List<CompareType> compareTypeList = new List<CompareType>();
        //private List<string> compareLogicStrList = new List<string>();
        private int compareIndex = 0;
        private Dictionary<string, object> compareValueList = new Dictionary<string, object>();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DMEDb_OQLCompare()
        {

        }

        /// <summary>
        /// 比较类别
        /// </summary>
        public enum CompareType
        {
            /// <summary>
            /// 大于
            /// </summary>
            Greater,
            /// <summary>
            /// 不大于（小于或等于）
            /// </summary>
            NoGreater,
            /// <summary>
            /// 小于
            /// </summary>
            Smaller,
            /// <summary>
            /// 不小于（大于或等于）
            /// </summary>
            NoSmaller,
            /// <summary>
            /// 相等
            /// </summary>
            Equal,
            /// <summary>
            /// 不等于
            /// </summary>
            NotEqual,
            /// <summary>
            /// 类似于
            /// </summary>
            Like,
            /// <summary>
            /// IS NULL / IS NOT NULL
            /// </summary>
            IS,
            /// <summary>
            /// IN 查询
            /// </summary>
            IN,
            /// <summary>
            /// Not In 查询
            /// </summary>
            NotIn

        }

        /// <summary>
        /// 条件表达式逻辑符号
        /// </summary>
        public enum CompareLogic
        {
            /// <summary>
            /// 逻辑 与
            /// </summary>
            AND,
            /// <summary>
            /// 逻辑 或
            /// </summary>
            OR,
            /// <summary>
            /// 逻辑 非
            /// </summary>
            NOT
        }

        //public string _DbCompareTypeStr;
        ///// <summary>
        ///// 数据库中的比较符号，如大于，小于符号
        ///// </summary>
        //public string DbCompareTypeStr
        //{
        //    get { return _DbCompareTypeStr; }
        //}
        ///// <summary>
        ///// 要比较的字段名称
        ///// </summary>
        //public string CompareFieldName
        //{
        //    get { return currPropName; }
        //}
        //public object _CompareValue;
        ///// <summary>
        ///// 要比较的值
        ///// </summary>
        //public object CompareValue
        //{
        //    get { return _CompareValue; }
        //}

        /// <summary>
        /// （条件表达式）比较的参数信息表
        /// </summary>
        public Dictionary<string, object> ComparedParameters
        {
            get { return compareValueList; }
        }

        /// <summary>
        /// 获取比较表达式的字符串形式
        /// </summary>
        public string CompareString
        {
            get
            {
                //if (_compareString != "")
                //{
                return _compareString;
                //}
                //string str = "";
                //int j = 0;
                //List<string> list = new List<string>();

                //for (int i = 0; i < propertyList.Count; i++)
                //{
                //    list.Add ( propertyList[i] + this.GetDbCompareTypeStr(compareTypeList[i]) + "@P" + i);
                //}

                //for (int i = 0; i < compareLogicStrList.Count; i++)
                //{
                //    str +=" ("+ list[j] + compareLogicStrList[i] + list[++j]+" ) ";
                //    j++;
                //    if (j== list.Count-1)
                //        break;
                //}
                //if (j == list.Count - 1)
                //    str += compareLogicStrList[compareLogicStrList.Count-1] + list[list.Count - 1];
                //return str;
                ////return str.Length >0? str.Substring (5):"";//.TrimEnd(" AND "); 
            }
            protected internal set { _compareString = value; }
        }

        /// <summary>
        /// 使用一个实体对象初始化本类
        /// </summary>
        /// <param name="e"></param>
        public DMEDb_OQLCompare(DMEDb_EntityBase e)
        {
            this.CurrEntity = e;
            //this.CurrEntity.ToCompareFields = true;
            this.CurrEntity.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
        }

        /// <summary>
        /// 使用多个实体类进行连接查询的条件
        /// </summary>
        /// <param name="e"></param>
        /// <param name="joinedEntitys"></param>
        public DMEDb_OQLCompare(DMEDb_EntityBase e,params DMEDb_EntityBase [] joinedEntitys)
        {
            this.CurrEntity = e;
            this.CurrEntity.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
            //处理多个实体类
            if (joinedEntitys != null && joinedEntitys.Length > 0)
            {
                this.joinedEntityList = new List<DMEDb_EntityBase>();
                foreach (DMEDb_EntityBase item in joinedEntitys)
                {
                    this.joinedEntityList.Add(item);
                    item.PropertyGetting += new EventHandler<PropertyGettingEventArgs>(CurrEntity_PropertyGetting);
                }
            }

        }

        private void dictionaryAddRange(Dictionary<string, object> dictMain, Dictionary<string, object> dictSalary)
        {
            foreach (string key in dictSalary.Keys)
            {
                dictMain[key] = dictSalary[key];
            }
        }

        /// <summary>
        /// 对一组OQLCompare 对象，执行CompareLogic 类型的比较，通常用于构造复杂的带括号的条件查询
        /// </summary>
        /// <seealso cref="http://www.cnblogs.com/bluedoctor/archive/2011/02/24/1963606.html"/>
        /// <param name="compares">OQL比较对象列表</param>
        /// <param name="logic">各组比较条件的组合方式，And，Or，Not</param>
        /// <returns>新的条件比较对象</returns>
        public DMEDb_OQLCompare Comparer(List<DMEDb_OQLCompare> compares, CompareLogic logic)
        {
            if (compares == null || compares.Count ==0)
                throw new Exception("OQL 条件比较对象集合不能为空或者空引用！");
            if (compares.Count == 1)
                return compares[0];
            DMEDb_OQLCompare cmp = new DMEDb_OQLCompare();
            string typeString = logic == CompareLogic.AND ? " And " : logic == CompareLogic.OR ? " Or " : " Not ";
            foreach (DMEDb_OQLCompare item in compares)
            {
                cmp.CompareString += item.CompareString + typeString;
                if (item.ComparedParameters != null)
                    foreach (string key in item.ComparedParameters.Keys)
                    {
                        cmp.ComparedParameters.Add(key, item.ComparedParameters[key]);
                    }
                
            }
            cmp.CompareString = cmp.CompareString.Substring(0, cmp.CompareString.Length - typeString.Length);
            cmp.CompareString = " ( "+cmp.CompareString+" ) ";
            return cmp;
        }

        /// <summary>
        /// 采用两个实体比较对象按照某种比较逻辑进行处理，构造一个新的实体比较对象
        /// </summary>
        /// <seealso cref="http://www.cnblogs.com/bluedoctor/archive/2010/11/28/1870095.html"/>
        /// <param name="compare1">比较逻辑符号左边的实体比较对象</param>
        /// <param name="logic">比较逻辑</param>
        /// <param name="compare2">比较逻辑符号左边的实体比较对象</param>
        public DMEDb_OQLCompare(DMEDb_OQLCompare compare1, CompareLogic logic, DMEDb_OQLCompare compare2)
        {
            //propertyList.AddRange(compare1.propertyList);
            //compareValueList.AddRange(compare1.compareValueList);
            //compareTypeList.AddRange(compare1.compareTypeList);

            //propertyList.AddRange(compare2.propertyList);
            //compareValueList.AddRange(compare2.compareValueList);
            //compareTypeList.AddRange(compare2.compareTypeList);
            //compareLogicStrList.AddRange(compare1.compareLogicStrList);

            dictionaryAddRange(compareValueList, compare1.compareValueList);
            dictionaryAddRange(compareValueList, compare2.compareValueList);

            switch (logic)
            {
                case CompareLogic.AND:
                    //this.compareLogicStrList.Add(" AND ");
                    this.CompareString = " (" + compare1.CompareString + " AND " + compare2.CompareString + ") ";
                    break;
                case CompareLogic.OR:
                    //this.compareLogicStrList.Add(" OR ");
                    this.CompareString = " (" + compare1.CompareString + " OR " + compare2.CompareString + ") ";
                    break;
                //case CompareLogic.NOT :
                //    this.compareLogicStrList.Add(" NOT ");
                //    this.CompareString = " NOT (" + compare1.CompareString + " AND " + compare2.CompareString + ") "; 
                //    break;

            }
        }

        void CurrEntity_PropertyGetting(object sender, PropertyGettingEventArgs e)
        {
            if (this.joinedEntityList != null)
            {
                this.currPropName ="["+((DMEDb_EntityBase )sender).TableName+"].["+  e.PropertyName+"]";
            }
            else
            {
                this.currPropName ="["+ e.PropertyName+"]";
                //propertyList.Add(e.PropertyName);
            }
            
        }

        /// <summary>
        /// 将当前实体属性的值和要比较的值进行比较，得到一个新的实体比较对象
        /// </summary>
        /// <param name="field">实体对象属性</param>
        /// <param name="type">比较类型枚举</param>
        /// <param name="Value">要比较的值</param>
        /// <returns>比较表达式</returns>
        public DMEDb_OQLCompare Comparer(object field, CompareType type, object Value)
        {

            //string typeStr = "";
            //switch (type)
            //{
            //    case CompareType.Equal: typeStr = "="; break;
            //    case CompareType.Greater: typeStr = ">"; break;
            //    case CompareType.Like: typeStr = "LIKE"; break;
            //    case CompareType.NoGreater: typeStr = "<="; break;
            //    case CompareType.NoSmaller: typeStr = ">="; break;
            //    case CompareType.NotEqual: typeStr = "<>"; break;
            //    case CompareType.Smaller: typeStr = "<"; break;
            //    default: typeStr = "="; break;
            //}
            //this._DbCompareTypeStr = typeStr;
            //this._CompareValue = Value;
            //

            //
            //this.CompareString = this.CompareFieldName + typeStr+" @" + this.CompareFieldName;   
            this.compareIndex++;
            DMEDb_OQLCompare cmp = new DMEDb_OQLCompare();
            if (type == CompareType.IS)
            {
                cmp.CompareString = this.currPropName + " IS " + Value.ToString();//此处可能不安全
            }
            else if (type == CompareType.IN )
            {
                cmp.CompareString = this.currPropName + " IN ( " + Value.ToString() + " )";//此处可能不安全
            }
            else if (type == CompareType.NotIn )
            {
                cmp.CompareString = this.currPropName + " NOT IN ( " + Value.ToString() + " )";//此处可能不安全
            }
            else
            {
                string paraName = "@CP" + this.compareIndex;
                cmp.compareValueList.Add(paraName.Substring(1), Value);
                //cmp.compareTypeList.Add(type);
                //cmp.propertyList.Add(this.currPropName);
                cmp.CompareString = this.currPropName + GetDbCompareTypeStr(type) + paraName;
            }
            return cmp;
        }

        /// <summary>
        /// 将当前实体属性的值和要比较的值进行比较，得到一个新的实体比较对象
        /// </summary>
        /// <param name="field">实体对象属性</param>
        /// <param name="compareTypeString">数据库比较类型字符串</param>
        /// <param name="Value">要比较的值</param>
        /// <returns>比较表达式</returns>
        public DMEDb_OQLCompare Comparer(object field, string compareTypeString, object Value)
        {
            string[] cmpStrs = { "=", "<>", ">=", "<=", "like", "is","in","not in" };
            if (String.IsNullOrEmpty(compareTypeString))
                compareTypeString = "=";
            else
                compareTypeString = compareTypeString.Trim().ToLower();
            bool flag = false;
            foreach (string str in cmpStrs)
            {
                if (compareTypeString == str)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
                throw new Exception("比较符号必须是 =,>=,<=,<>,like,is,in,not in 中的一种。");


            this.compareIndex++;
            DMEDb_OQLCompare cmp = new DMEDb_OQLCompare();
            if (compareTypeString == "is")
            {
                cmp.CompareString = this.currPropName + " IS " + Value.ToString();//此处可能不安全，IS NULL，IS NOT NULL
            }
            else if (compareTypeString == "in")
            {
                cmp.CompareString = this.currPropName + " IN ( " + Value.ToString() + " )";//此处可能不安全

            }
            else if (compareTypeString == "not in")
            {
                cmp.CompareString = this.currPropName + " NOT IN ( " + Value.ToString() + " )";//此处可能不安全

            }
            else
            {
                string paraName = "@CP" + this.compareIndex;
                cmp.compareValueList.Add(paraName.Substring(1), Value);
                cmp.CompareString = this.currPropName +" "+ compareTypeString+" " + paraName;
            }
            return cmp;
        }

        /// <summary>
        /// 将当前实体属性的值作为比较的值，得到一个新的实体比较对象
        /// </summary>
        /// <param name="field">实体对象的属性字段</param>
        /// <returns>比较表达式</returns>
        public DMEDb_OQLCompare Comparer(object field)
        {
            //this.compareIndex++;
            //OQLCompare cmp = new OQLCompare();
            //string paraName = "@CP" + this.compareIndex;
            //cmp.compareValueList.Add(paraName, field);
            //cmp.CompareString = this.currPropName + "=" + paraName;
            //return cmp;
            return this.Equals(field, field);
        }

        /// <summary>
        /// 开始字段名称的相等比较（待实现）
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DMEDb_OQLCompare BeginEquals(object field)
        {
            return this;
        }

        /// <summary>
        /// 结束字段名称的相等比较（待实现）
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DMEDb_OQLCompare EndEquals(object field)
        {
            return this;
        }

        /// <summary>
        /// 将当前实体属性的值和要比较的值进行比较，得到一个新的实体比较对象
        /// </summary>
        /// <param name="field">实体对象属性</param>
        /// <param name="Value">要比较的值</param>
        /// <returns>比较表达式</returns>
        public new DMEDb_OQLCompare  Equals(object field,object Value)
        {
            this.compareIndex++;
            DMEDb_OQLCompare cmp = new DMEDb_OQLCompare();
            string paraName = "@CP" + this.compareIndex;
            cmp.compareValueList.Add(paraName.Substring (1), Value);
            cmp.CompareString = this.currPropName + "=" + paraName;
            return cmp;
        }

        private string GetDbCompareTypeStr(CompareType type)
        {
            string typeStr = "";
            switch (type)
            {
                case CompareType.Equal: typeStr = "="; break;
                case CompareType.Greater: typeStr = ">"; break;
                case CompareType.Like: typeStr = " LIKE "; break;
                case CompareType.NoGreater: typeStr = "<="; break;
                case CompareType.NoSmaller: typeStr = ">="; break;
                case CompareType.NotEqual: typeStr = "<>"; break;
                case CompareType.Smaller: typeStr = "<"; break;
                case CompareType.IN: typeStr = " IN "; break;
                case CompareType.IS: typeStr = " IS "; break;
                case CompareType.NotIn:typeStr =" NOT IN ";break ;
                default: typeStr = "="; break;
            }
            return typeStr;
        }

        /// <summary>
        /// 将两个实体比较对象进行逻辑 与 比较，得到一个新的实体比较表达式
        /// </summary>
        /// <param name="compare1">左表达式</param>
        /// <param name="compare2">右表达式</param>
        /// <returns>实体比较表达式</returns>
        public static DMEDb_OQLCompare operator &(DMEDb_OQLCompare compare1, DMEDb_OQLCompare compare2)
        {
            return new DMEDb_OQLCompare(compare1, CompareLogic.AND, compare2);
        }

        /// <summary>
        /// 将两个实体比较对象进行逻辑 与 比较，得到一个新的实体比较表达式
        /// </summary>
        /// <param name="compare1">左表达式</param>
        /// <param name="compare2">右表达式</param>
        /// <returns>实体比较表达式</returns>
        public static DMEDb_OQLCompare operator |(DMEDb_OQLCompare compare1, DMEDb_OQLCompare compare2)
        {
            return new DMEDb_OQLCompare(compare1, CompareLogic.OR, compare2);
        }



    }

    public class DMEDb_JoinEntity
    {
        private DMEDb_OQL _mainOql;
        public DMEDb_JoinEntity(DMEDb_OQL mainOql)
        {
            this._mainOql = mainOql;
        }

        public DMEDb_OQL On(object field1,object field2)
        {
            //条件查询等地方可能还用的上??
            this._mainOql.isJoinOpt = false;
            this._mainOql.HaveJoinOpt = true;
            return this._mainOql;
        }
    }


}
