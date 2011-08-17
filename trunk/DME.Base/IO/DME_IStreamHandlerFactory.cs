using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DME.Base.IO
{
    /// <summary>
    /// 数据流处理器工厂接口
    /// </summary>
    public interface DME_IStreamHandlerFactory
    {
        /// <summary>
        /// 获取处理器
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        DME_IStreamHandler GetHandler(Stream stream);
    }
}
