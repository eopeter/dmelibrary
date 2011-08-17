using System;
using System.Collections.Generic;
using System.Text;
using DME.DataBase.DataProvider.Data;
using DME.DataBase.DataProvider.Adapter;

namespace DME.DataBase.DataMap.SqlMap
{
    /// <summary>
    /// SQLMAP数据处理层基类
    /// </summary>
    /// <remarks></remarks>
    public abstract class DMEDb_DBMapper
    {
        #region "公共的数据库接口"
        DMEDb_AdoHelper _DB;
        DMEDb_SqlMapper _Mapper;
        string _SqlMapFile;

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <remarks></remarks>
        public DMEDb_DBMapper()
        {
            _DB = DMEDb_MyDB.GetDBHelper();
            _Mapper = new DMEDb_SqlMapper();
            //_Mapper.CommandClassName = "EngineManager"
            _Mapper.DataBase = _DB;
        }

        /// <summary>
        /// 获取或设置当前使用的数据库操作对象
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DMEDb_AdoHelper CurrentDataBase
        {
            get { return _DB; }
            set
            {
                _DB = value;
                _Mapper.DataBase = _DB;
            }
        }

        /// <summary>
        /// 获取或设置SQL Map 配置文件地址(可以是一个外部配置文件或者嵌入程序集的配置文件)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SqlMapFile
        {
            //If _SqlMapFile = "" Then
            //    _SqlMapFile = System.Configuration.ConfigurationSettings.AppSettings("SqlMapFile")
            //    If _SqlMapFile = "" Then
            //        Throw New ArgumentOutOfRangeException("SqlMapFile", "该属性没有在应用程序中设置值，请在应用程序配置文件中配置SqlMapFile项和值。 ")
            //    End If
            //End If
            get { return _SqlMapFile; }
            set
            {
                _SqlMapFile = value;
                _Mapper.SqlMapFile = _SqlMapFile;
            }
        }

        /// <summary>
        /// 获取SQLMAP对象
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DMEDb_SqlMapper Mapper
        {
            get { return _Mapper; }
        }

        #endregion

    }

}
