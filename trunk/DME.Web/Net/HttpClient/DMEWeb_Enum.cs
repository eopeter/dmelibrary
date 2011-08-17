using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Net
{
    public enum DMEWeb_HttpVerb
    {
        GET,
        POST,
        HEAD,
        POSTXML,
    }

    public enum DMEWeb_FileExistsAction
    {
        Overwrite,
        Append,
        Cancel,
    }
}
