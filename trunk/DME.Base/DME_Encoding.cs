using System;
using System.Collections.Generic;
using System.Text;
using DME.Base.Helper;
using DME.Base.Collections;

namespace DME.Base
{
    [DME_EnumDescription("常用编码")]
    public enum DME_Encoding
    {
        [DME_EnumDescription("utf-7", "Unicode (UTF-7)")]
        UTF_7 = 65000,
        [DME_EnumDescription("utf-8", "Unicode (UTF-8)")]
        UTF_8 = 65001,
        [DME_EnumDescription("utf-32", "Unicode (UTF-32)")]
        UTF_32 = 65005,
        [DME_EnumDescription("utf-32BE", "Unicode (UTF-32 Big-Endian)")]
        UTF_32BE = 65006,
        [DME_EnumDescription("us-ascii", "US-ASCII")]
        ASCII = 20127,
        [DME_EnumDescription("utf-16", "Unicode")]
        Unicode = 1200,
        [DME_EnumDescription("big5 ", "繁体中文 (Big5)")]
        BIG5 = 950,
        [DME_EnumDescription("gb2312", "简体中文 (GB2312)")]
        GB2312 = 936,
        [DME_EnumDescription("GB18030", "简体中文 (GB18030)")]
        GB18030 = 54936,
        [DME_EnumDescription("hz-gb-2312", "简体中文 (HZ)")]
        HZ_GB_2312 =  52936,
    }
}
