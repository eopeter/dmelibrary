using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.IO
{
    /// <summary>
    /// 二进制数据访问器接口，用于把对象数据写入到写入器，或者从读取器中读取数据到对象
    /// </summary>
    public interface DME_IBinaryAccessor
    {
        /// <summary>
        /// 从读取器中读取数据到对象
        /// </summary>
        /// <param name="reader"></param>
        void Read(DME_BinaryReader reader);

        /// <summary>
        /// 把对象数据写入到写入器
        /// </summary>
        /// <param name="writer"></param>
        void Write(DME_BinaryWriter writer);
    }
}
