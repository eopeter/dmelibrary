using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Net
{
    public class DMEWeb_StatusUpdateEventArgs : EventArgs
    {
        private readonly int bytesGot;
        private readonly int bytesTotal;

        public DMEWeb_StatusUpdateEventArgs(int got, int total)
        {
            bytesGot = got;
            bytesTotal = total;
        }

        /// <summary>
        /// 已经下载的字节数
        /// </summary>
        public int BytesGot
        {
            get { return bytesGot; }
        }

        /// <summary>
        /// 资源的总字节数
        /// </summary>
        public int BytesTotal
        {
            get { return bytesTotal; }
        }

    }
}
