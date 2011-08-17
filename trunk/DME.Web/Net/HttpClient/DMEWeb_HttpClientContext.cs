using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DME.Web.Net
{
    public class DMEWeb_HttpClientContext
    {
        private CookieCollection cookies;

        private string referer;

        public CookieCollection Cookies
        {
            get { return cookies; }
            set { cookies = value; }
        }

        public string Referer
        {
            get { return referer; }
            set { referer = value; }
        }
    }
}
