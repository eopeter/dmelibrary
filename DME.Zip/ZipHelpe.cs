using System;
using System.IO;
using System.Collections;
using DME.Zip.Checksums;
using DME.Zip.Zip;
using DME.Zip.GZip;

namespace DME.Zip
{
    public class ZipHelpe
    {
        #region 成员变量    
        /// <summary> 压缩率：0-9</summary>
        private int compressionLevel = 9;
        /// <summary>缓冲区大小</summary>
        private byte[] buffer = new byte[2048];
        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ZipHelpe()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bufferSize">缓冲区大小</param>
        /// <param name="compressionLevel">压缩率：0-9</param>
        public ZipHelpe(int bufferSize, int compressionLevel)
        {
            buffer = new byte[bufferSize];
            this.compressionLevel = compressionLevel;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件路径</param>
        /// <param name="zipedFile">压缩后的文件路径</param>
        public void ZipFile(string fileToZip, string zipedFile)
        {
            if (!File.Exists(fileToZip))
            {
                throw new FileNotFoundException("The specified file " + fileToZip + " could not be found.");
            }

            if (!string.IsNullOrEmpty(zipedFile))
            {
                zipedFile = Path.GetFileNameWithoutExtension(zipedFile) + ".zip";
            }

            if (Path.GetExtension(zipedFile) != ".zip")
            {
                zipedFile = zipedFile + ".zip";
            }

            string zipedDir = zipedFile.Substring(0, zipedFile.LastIndexOf("\\"));
            if (!Directory.Exists(zipedDir))
            {
                Directory.CreateDirectory(zipedDir);
            }

            using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile)))
            {
                string fileName = Path.GetFileName(fileToZip);
                ZipEntry zipEntry = new ZipEntry(fileName);
                zipStream.PutNextEntry(zipEntry);
                zipStream.SetLevel(compressionLevel);

                using (FileStream streamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read))
                {
                    int size = streamToZip.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, size);

                    while (size < streamToZip.Length)
                    {
                        int sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                        zipStream.Write(buffer, 0, sizeRead);
                        size += sizeRead;
                    }
                }
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="directoryToZip">要压缩的文件夹路径</param>
        /// <param name="zipedFile">压缩后的文件路径</param>
        public void ZipDerctory(string directoryToZip, string zipedFile)
        {

            if (string.IsNullOrEmpty(zipedFile))
            {
                zipedFile = directoryToZip.Substring(directoryToZip.LastIndexOf("\\") + 1);
                zipedFile = directoryToZip.Substring(0, directoryToZip.LastIndexOf("\\")) + "\\" + zipedFile + ".zip";
            }

            if (Path.GetExtension(zipedFile) != ".zip")
            {
                zipedFile = zipedFile + ".zip";
            }

            using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile)))
            {
                ArrayList fileList = GetFileList(directoryToZip);
                int directoryNameLength = (Directory.GetParent(directoryToZip)).ToString().Length;

                zipStream.SetLevel(compressionLevel);
                ZipEntry zipEntry = null;
                FileStream fileStream = null;

                foreach (string fileName in fileList)
                {
                    zipEntry = new ZipEntry(fileName.Remove(0, directoryNameLength));
                    zipStream.PutNextEntry(zipEntry);

                    if (!fileName.EndsWith(@"/"))
                    {
                        fileStream = File.OpenRead(fileName);
                        fileStream.Read(buffer, 0, buffer.Length);
                        zipStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        public void UnZipFile(string zipFilePath, string unZipDir)
        {
            if (zipFilePath == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("The specified file " + zipFilePath + " could not be found.");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
            if ((string.IsNullOrEmpty(unZipDir)))
            {
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            }
            if (!unZipDir.EndsWith("\\"))
            {
                unZipDir += "\\";
            }
            if (!Directory.Exists(unZipDir))
            {
                Directory.CreateDirectory(unZipDir);
            }

            using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry zipEntry = null;
                while ((zipEntry = zipStream.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(zipEntry.Name);
                    string fileName = Path.GetFileName(zipEntry.Name);

                    if (!(string.IsNullOrEmpty(directoryName)))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (!(string.IsNullOrEmpty(fileName)))
                    {
                        if (zipEntry.CompressedSize == 0)
                        {
                            break;
                        }
                        using (FileStream stream = File.Create(unZipDir + fileName))
                        {
                            while (true)
                            {
                                int size = zipStream.Read(buffer, 0, buffer.Length);
                                if (size > 0)
                                {
                                    stream.Write(buffer, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 得到文件夹下的所有文件
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <returns>文件列表</returns>
        private ArrayList GetFileList(string directory)
        {
            ArrayList fileList = new ArrayList();
            bool isEmpty = true;
            foreach (string file in Directory.GetFiles(directory))
            {
                fileList.Add(file);
                isEmpty = false;
            }
            if (isEmpty)
            {
                if (Directory.GetDirectories(directory).Length == 0)
                {
                    fileList.Add(directory + @"/");
                }
            }
            foreach (string dirs in Directory.GetDirectories(directory))
            {
                foreach (object obj in GetFileList(dirs))
                {
                    fileList.Add(obj);
                }
            }
            return fileList;
        }
    }
}
