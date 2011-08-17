using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.RedisCached
{
    [Serializable]
    public class DME_ResponseException : Exception
    {
        public DME_ResponseException(string code)
            : base("Response error")
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
