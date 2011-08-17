/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  TagFactory
 *
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template
{
    /// <summary>
    /// 标签工厂
    /// </summary>
    internal class DMEWeb_TagFactory
    {
        /// <summary>
        /// 根据标签名建立标签实例
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        internal static DMEWeb_Tag FromTagName(DMEWeb_Template ownerTemplate, string tagName)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                switch (tagName.ToLower())
                {
                    case "for":
                        return new DMEWeb_ForTag(ownerTemplate);
                    case "foreach":
                        return new DMEWeb_ForEachTag(ownerTemplate);
                    case "foreachelse":
                        return new DMEWeb_ForEachElseTag(ownerTemplate);
                    case "if":
                        return new DMEWeb_IfTag(ownerTemplate);
                    case "elseif":
                        return new DMEWeb_IfConditionTag(ownerTemplate);
                    case "else":
                        return new DMEWeb_ElseTag(ownerTemplate);
                    case "template":
                        return new DMEWeb_Template(ownerTemplate);
                    case "include":
                        return new DMEWeb_IncludeTag(ownerTemplate);
                    case "expression":
                        return new DMEWeb_ExpressionTag(ownerTemplate);
                    case "function":
                        return new FunctionTag(ownerTemplate);
                    case "property":
                        return new DMEWeb_PropertyTag(ownerTemplate);
                    case "serverdata":
                        return new DMEWeb_ServerDataTag(ownerTemplate);
                    case "set":
                        return new DMEWeb_SetTag(ownerTemplate);
                    case "import":
                        return new DMEWeb_ImportTag(ownerTemplate);
                    case "output":
                        return new DMEWeb_OutputTag(ownerTemplate);
                    case "datareader":
                        if (ownerTemplate.OwnerDocument.DocumentConfig != null
                            && ownerTemplate.OwnerDocument.DocumentConfig.TagOpenMode == DMEWeb_TagOpenMode.Full)
                        {
                            return new DMEWeb_DataReaderTag(ownerTemplate);
                        }
                        else
                        {
                            return null;
                        }
                }
            }
            return null;
        }
    }
}
