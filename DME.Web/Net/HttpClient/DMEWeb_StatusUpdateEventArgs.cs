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
        /// �Ѿ����ص��ֽ���
        /// </summary>
        public int BytesGot
        {
            get { return bytesGot; }
        }

        /// <summary>
        /// ��Դ�����ֽ���
        /// </summary>
        public int BytesTotal
        {
            get { return bytesTotal; }
        }

    }
}
