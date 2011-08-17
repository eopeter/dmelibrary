using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DME.Web.Net
{
    public class DMEWeb_HttpUploadingFile
    {
        private string fileName;
        private string fieldName;
        private byte[] data;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public DMEWeb_HttpUploadingFile(string fileName, string fieldName)
        {
            this.fileName = fileName;
            this.fieldName = fieldName;
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                byte[] inBytes = new byte[stream.Length];
                stream.Read(inBytes, 0, inBytes.Length);
                data = inBytes;
            }
        }

        public DMEWeb_HttpUploadingFile(byte[] data, string fileName, string fieldName)
        {
            this.data = data;
            this.fileName = fileName;
            this.fieldName = fieldName;
        }
    }
}
