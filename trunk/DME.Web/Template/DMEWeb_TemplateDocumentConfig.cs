using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    #region 标签的开放模式
    /// <summary>
    /// 标签的开放模式
    /// </summary>
    public enum DMEWeb_TagOpenMode
    {
        /// <summary>
        /// 简单的.不支持&lt;vt:datareader&gt;等标签
        /// </summary>
        Simple,
        /// <summary>
        /// 完全的.将支持所有标签
        /// </summary>
        Full
    }
    #endregion

    /// <summary>
    /// 模板文档的配置参数
    /// </summary>
    public class DMEWeb_TemplateDocumentConfig
    {
        /// <summary>
        /// 标签的开放模式为简单,不压缩文本
        /// </summary>
        public static readonly DMEWeb_TemplateDocumentConfig Default;
        /// <summary>
        /// 标签的开放模式为完全,不压缩文本
        /// </summary>
        public static readonly DMEWeb_TemplateDocumentConfig Full;
        /// <summary>
        /// 标签的开放模式为简单,压缩文本
        /// </summary>
        public static readonly DMEWeb_TemplateDocumentConfig Compress;
        /// <summary>
        /// 标签的开放模式为简单,不压缩文本，且采用兼容模式
        /// </summary>
        public static readonly DMEWeb_TemplateDocumentConfig Compatible;
        /// <summary>
        /// 
        /// </summary>
        static DMEWeb_TemplateDocumentConfig()
        {
            DMEWeb_TemplateDocumentConfig.Default = new DMEWeb_TemplateDocumentConfig();
            DMEWeb_TemplateDocumentConfig.Full = new DMEWeb_TemplateDocumentConfig(DMEWeb_TagOpenMode.Full, false);
            DMEWeb_TemplateDocumentConfig.Compress = new DMEWeb_TemplateDocumentConfig(DMEWeb_TagOpenMode.Simple, true);
            DMEWeb_TemplateDocumentConfig.Compatible = new DMEWeb_TemplateDocumentConfig(DMEWeb_TagOpenMode.Simple, false, true);
        }
        /// <summary>
        /// 实例化默认的配置.标签的开放模式为简单、不压缩文本
        /// </summary>
        public DMEWeb_TemplateDocumentConfig()
        {
            this.TagOpenMode = DMEWeb_TagOpenMode.Simple;
            this.CompressText = false;
            this.CompatibleMode = false;
        }
        /// <summary>
        /// 根据参数实例化
        /// </summary>
        /// <param name="tagOpenMode">标签的开放模式</param>
        public DMEWeb_TemplateDocumentConfig(DMEWeb_TagOpenMode tagOpenMode)
        {
            this.TagOpenMode = tagOpenMode;
            this.CompressText = false;
            this.CompatibleMode = false;
        }
        /// <summary>
        /// 根据参数实例化
        /// </summary>
        /// <param name="tagOpenMode">标签的开放模式</param>
        /// <param name="compressText">是否压缩文本</param>
        public DMEWeb_TemplateDocumentConfig(DMEWeb_TagOpenMode tagOpenMode, bool compressText)
        {
            this.TagOpenMode = tagOpenMode;
            this.CompressText = compressText;
            this.CompatibleMode = false;
        }

        /// <summary>
        /// 根据参数实例化
        /// </summary>
        /// <param name="tagOpenMode">标签的开放模式</param>
        /// <param name="compressText">是否压缩文本</param>
        /// <param name="compatibleMode">是否采用兼容模式</param>
        public DMEWeb_TemplateDocumentConfig(DMEWeb_TagOpenMode tagOpenMode, bool compressText, bool compatibleMode)
        {
            this.TagOpenMode = tagOpenMode;
            this.CompressText = compressText;
            this.CompatibleMode = compatibleMode;
        }

        /// <summary>
        /// 标签的开放模式
        /// </summary>
        public DMEWeb_TagOpenMode TagOpenMode { get; private set; }
        
        /// <summary>
        /// 是否压缩文本
        /// </summary>
        /// <remarks>
        /// 压缩文本.即是删除换行符和无用的空格(换行符前后的空格)
        /// </remarks>
        public bool CompressText { get; private set; }

        /// <summary>
        /// 兼容模式
        /// </summary>
        /// <remarks>如果采用兼容模式.则&lt;vt:foreach&gt;标签的from属性与&lt;vt:expression&gt;标签的args属性等可以不以$字符开头定义变量表达式</remarks>
        public bool CompatibleMode { get; private set; }
    }
}
