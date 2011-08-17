using System;
namespace DME.DataBase.Common
{
    /// <summary>
    /// 实体类接口
    /// </summary>
    public interface DMEDb_IEntity
    {
        /// <summary>
        /// 标识字段名称
        /// </summary>
        string IdentityName { get; }
        /// <summary>
        /// 获取主键
        /// </summary>
        System.Collections.Generic.List<string> PrimaryKeys { get; }
        /// <summary>
        /// 获取属性（和属性值）列表
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        object PropertyList(string propertyName);
        /// <summary>
        /// 数据表名称
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 获取属性字段名数组
        /// </summary>
        string[] PropertyNames { get; }
    }
}
