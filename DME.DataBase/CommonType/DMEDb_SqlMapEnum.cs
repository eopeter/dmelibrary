using System;
using System.Collections.Generic;
using System.Text;

namespace DME.DataBase.Common
{
    /// <summary>
    /// 参数结构
    /// </summary>
    /// <remarks></remarks>
    public struct DMEDb_ParamMapType
    {
        public string ParamName;
        public System.TypeCode TypeCode;
    }


    /// <summary>
    /// 参数类型
    /// </summary>
    /// <remarks></remarks>
    public enum DMEDb_enumParamType
    {
        /// <summary>
        /// 参数
        /// </summary>
        /// <remarks></remarks>
        DataParameter,
        /// <summary>
        /// 替换文本
        /// </summary>
        /// <remarks></remarks>
        ReplacedText
    }

    /// <summary>
    /// 查询类型
    /// </summary>
    /// <remarks></remarks>
    public enum DMEDb_enumQueryType
    {
        Select,
        Update,
        Delete,
        Create
    }


    /// <summary>
    /// 结果类型
    /// </summary>
    /// <remarks></remarks>
    public enum DMEDb_enumResultClass
    {
        /// <summary>
        /// 值类型，比如Integer，String 等
        /// </summary>
        /// <remarks></remarks>
        ValueType,
        /// <summary>
        /// 数据集
        /// </summary>
        /// <remarks></remarks>
        DataSet,
        /// <summary>
        /// 实体对象，指定该类型后，需要指定 ResuleMap 属性
        /// </summary>
        /// <remarks></remarks>
        EntityObject,
        /// <summary>
        /// 实体对象集合，指定该类型后，需要指定 ResuleMap 属性
        /// </summary>
        /// <remarks></remarks>
        EntityList,
        /// <summary>
        /// 默认类型，根据配置决定默认生成哪一种结果类型
        /// </summary>
        /// <remarks></remarks>
        Default
    }

    /// <summary>
    /// 实体对象字段比较枚举
    /// </summary>
    /// <remarks></remarks>
    public enum DMEDb_enumCompare
    {
        /// <summary>
        /// 大于
        /// </summary>
        /// <remarks></remarks>
        Greater,
        /// <summary>
        /// 不大于
        /// </summary>
        /// <remarks></remarks>
        NoGreater,
        /// <summary>
        /// 小于
        /// </summary>
        /// <remarks></remarks>
        Smaller,
        /// <summary>
        /// 不小于
        /// </summary>
        /// <remarks></remarks>
        NoSmaller,
        /// <summary>
        /// 等于
        /// </summary>
        /// <remarks></remarks>
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        /// <remarks></remarks>
        NotEqual,
        /// <summary>
        /// 类似于
        /// </summary>
        /// <remarks></remarks>
        Like,
        /// <summary>
        /// 空
        /// </summary>
        IsNull,
        /// <summary>
        /// 非空
        /// </summary>
        IsNotNull
    }

    /// <summary>
    /// 实体类的映射类型
    /// </summary>
    public enum DMEDb_EntityMapType
    {
        /// <summary>
        /// 表实体类，该实体具有对数据库CRUD功能。
        /// </summary>
        Table,
        /// <summary>
        /// 视图实体类，通常是数据库视图的映射，属性数据不能持久化。
        /// </summary>
        View,
        /// <summary>
        /// SQL映射实体类，将从SQL-MAP配置文件中使用用户定义的查询。
        /// </summary>
        SqlMap
    }

}
