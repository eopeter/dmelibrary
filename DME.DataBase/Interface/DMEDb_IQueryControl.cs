using System;


namespace DME.DataBase.Common
{
    /// <summary>
    /// 数据查询控件接口
    /// </summary>
    public interface DMEDb_IQueryControl
    {
        /// <summary>
        /// 查询的比较符号,例如 =,>=,
        /// </summary>
        string CompareSymbol
        {
            get;
            set;
        }

        /// <summary>
        /// 发送到数据库查询前的字段值格式字符串
        /// </summary>
        string QueryFormatString
        {
            get;
            set;
        }
    }
}
