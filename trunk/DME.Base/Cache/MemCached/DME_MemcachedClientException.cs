using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemCached
{
    public class DME_MemcachedClientException : ApplicationException
    {
        public DME_MemcachedClientException(string message) : base(message) { }
        public DME_MemcachedClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
