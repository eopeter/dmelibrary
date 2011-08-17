using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DME.Base.IO
{
    /// <summary>
    /// 数据流处理器接口
    /// </summary>
    public interface DME_IStreamHandler
    {
        /// <summary>
        /// 处理数据流
        /// </summary>
        /// <param name="stream"></param>
        void Process(Stream stream);

        /// <summary>
        /// 是否可以重用
        /// </summary>
        Boolean IsReusable { get; }
    }
}
