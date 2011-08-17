using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using DME.Base.IO;

namespace DME.Web.Common
{
    /// <summary>
    /// 数据流Http处理器。可以在web.config中配置一个处理器指向该类。
    /// </summary>
    public class DMEWeb_StreamHttpHandler : IHttpHandler
    {
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            // 以文件名（没有后缀）作为数据流工厂总线名称
            String name = Path.GetFileNameWithoutExtension(context.Request.FilePath);
            DME_StreamHandlerFactory.Process(name, new DMEWeb_HttpStream(context));
        }

        /// <summary>
        /// 是否可以重用
        /// </summary>
        public Boolean IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
