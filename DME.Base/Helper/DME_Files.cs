using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
namespace DME.Base.Helper
{
    /// <summary>
    /// 文件/文件夹操作类
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public static class DME_Files
    {
        #region 私有变量
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        #endregion

        #region 私有函数
        /// <summary>根据操作系统的类型的作判断</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string CheckLastIndex(string path)
        {
            if (path.LastIndexOf("\\") == path.Length - 1)
            {
                path = path.Substring(0, path.LastIndexOf("\\"));
            }
            if (path.LastIndexOf("/") == path.Length - 1)
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }
            return path;
        }

        /// <summary></summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static long FolderFileSize(string path)
        {
            long size = 0;
            path = CheckLastIndex(path);
            try
            {
                FileInfo[] files = (new DirectoryInfo(path)).GetFiles();
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return size;
        }

        /// <summary>计算哈希值</summary>
        /// <param name="stream">要计算哈希值的 Stream</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] HashData(System.IO.Stream stream, string algName)
        {
            System.Security.Cryptography.HashAlgorithm algorithm;
            if (algName == null)
            {
                throw new ArgumentNullException("algName 不能为 null");
            }
            if (string.Compare(algName, "sha1", true) == 0)
            {
                algorithm = System.Security.Cryptography.SHA1.Create();
            }
            else
            {
                if (!DME_String.StringCompare(algName, "md5",false))
                {
                    throw new Exception("algName 只能使用 sha1 或 md5");
                }
                algorithm = System.Security.Cryptography.MD5.Create();
            }
            return algorithm.ComputeHash(stream);
        }

        /// <summary>计算文件的哈希值</summary>
        /// <param name="fileName">要计算哈希值的文件名和路径</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(string fileName, string algName)
        {
            if (!System.IO.File.Exists(fileName))
                return string.Empty;

            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] hashBytes = HashData(fs, algName);
            fs.Close();
            return DME_TypeParse.BytesToHexString(hashBytes);
        }


        #endregion

        #region 公开函数

        /// <summary> 根据文件名命名规则验证字符串是否符合文件名格式 </summary>
        /// <param name="fileName">待验证的字符串</param>
        /// <returns>true or false</returns>
        public static bool IsFileName(string fileName)
        {
            if (DME_Validation.IsNull(fileName)) 
            {
                return false; 
            }
            else
            {
                // 不能以 “.” 开头
                fileName = fileName.Trim().ToLower();
                if (fileName.StartsWith(".")) return false;

                // “nul”、“aux”、“con”、“com1”、“lpt1”不能为文件夹/文件的名称
                if (DME_String.StringCompare(fileName, "con", false)) return false;
                if (DME_String.StringCompare(fileName, "nul", false)) return false;
                if (DME_String.StringCompare(fileName, "aux", false)) return false;
                if (DME_String.StringCompare(fileName, "com1", false)) return false;
                if (DME_String.StringCompare(fileName, "lpt1", false)) return false;
                Regex re = new Regex(@"^[^\\\./:\*\?\""<>\|]{1}[^\\/:\*\?\""<>\|]{0,254}$", RegexOptions.IgnoreCase);
                return re.IsMatch(fileName);
            }
        }

        /// <summary> 根据文件夹命名规则验证字符串是否符合文件夹格式</summary>
        /// <param name="folderName">待验证的字符串</param>
        /// <returns>true or false</returns>
        public static bool IsFolderName(string folderName)
        {
            if (DME_Validation.IsNull(folderName)) 
            {
                return false; 
            }
            else
            {
                // 不能以 “.” 开头
                folderName = folderName.Trim().ToLower();
                if (folderName.StartsWith(".")) return false;

                // “nul”、“aux”、“con”、“com1”、“lpt1”不能为文件夹/文件的名称

                if (DME_String.StringCompare(folderName, "con", false)) return false;
                if (DME_String.StringCompare(folderName, "nul", false)) return false;
                if (DME_String.StringCompare(folderName, "aux", false)) return false;
                if (DME_String.StringCompare(folderName, "com1", false)) return false;
                if (DME_String.StringCompare(folderName, "lpt1", false)) return false;

                Regex re = new Regex(@"^[^\\\/\?\*\""\>\<\:\|]*$", RegexOptions.IgnoreCase);
                return re.IsMatch(folderName);
            }
        }

        /// <summary>获取可用于文件名的安全字符串，确保该字符串中不含文件名不允许的字符。 注意：“con”也不能用于文件名，但此处未验证 </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetValidFileName(string fileName)
        {
            if (DME_Validation.IsNull(fileName)) { return null; }
            else
            {
                Regex re = new Regex(@"^[^\\\./:\*\?\""<>\|]{1}[^\\/:\*\?\""<>\|]{0,254}$", RegexOptions.IgnoreCase);
                fileName = re.Replace(fileName, string.Empty);
                return fileName;
            }
        }

        /// <summary>获取可用于文件夹名称的安全字符串，确保该字符串中不含文件夹名称不允许的字符。注意：“con”也不能用于文件夹名，但此处未验证</summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetValidFolderName(string folderName)
        {
            if (DME_Validation.IsNull(folderName)) { return null; }
            else
            {
                Regex re = new Regex(@"^[^\\\/\?\*\""\>\<\:\|]*$", RegexOptions.IgnoreCase);
                folderName = re.Replace(folderName, string.Empty);
                return folderName;
            }
        }

        /// <summary>将以字节表示的文件大小格式化为对用户友好的字符串表示。</summary>
        /// <param name="sizeAsBytes">要格式化的数字（字节）</param>
        /// <param name="decimals">小数点后保留的位数</param>
        /// <returns>已格式化的字符串</returns>
        public static string FormatFileSize(long sizeAsBytes, int decimals)
        {
            StringBuilder sb = new StringBuilder();
            if (sizeAsBytes >= 1099511627776)
            {
                sb.Append(Math.Round((decimal)sizeAsBytes / 1099511627776, decimals).ToString());
                sb.Append("TB");
            }
            else if (sizeAsBytes >= 1073741824)
            {
                sb.Append(Math.Round((decimal)sizeAsBytes / 1073741824, decimals).ToString());
                sb.Append("GB");
            }
            else if (sizeAsBytes >= 1048576)
            {
                sb.Append(Math.Round((decimal)sizeAsBytes / 1048576, decimals).ToString());
                sb.Append("MB");
            }
            else if (sizeAsBytes >= 1024)
            {
                sb.Append(Math.Round((decimal)sizeAsBytes / 1024, decimals).ToString());
                sb.Append("KB");
            }
            else
            {
                sb.Append(sizeAsBytes.ToString());
                sb.Append("Bytes");
            }
            return sb.ToString();
        }

        /// <summary>将以字节表示的文件大小格式化为对用户友好的字符串表示。此方法可以讲以字节数表示的文件大小格式化为对用户友好的字符串表示，
        /// 比如对于大小为 3,032,576 字节的文件，使用此方法可以将之格式化表示为 2.89MB。
        /// </summary>
        /// <param name="sizeAsBytes">要格式化的数字（字节）</param>
        /// <returns>已格式化的字符串</returns>
        public static string FormatFileSize(long sizeAsBytes)
        {
            return FormatFileSize(sizeAsBytes, 2);
        }

        /// <summary>计算文件的大小</summary>
        /// <param name="TargetFile">目标文件</param>
        /// <returns></returns>
        public static string FileSize(string TargetFile)
        {
            FileInfo fi = new FileInfo(TargetFile);
            long filesize = fi.Length;            
            return FormatFileSize(filesize,2);
        }

        /// <summary>计算文件夹的大小</summary>
        /// <param name="TargetFolder">目标文件夹</param>
        /// <returns></returns>
        public static string FolderSize(string TargetFolder)
        {
            long Fsize = 0;
            try
            {
                Fsize = FolderFileSize(TargetFolder);
                DirectoryInfo[] folders = (new DirectoryInfo(TargetFolder)).GetDirectories();
                foreach (DirectoryInfo folder in folders)
                    Fsize += FolderFileSize(folder.FullName);
            }
            catch (Exception)
            {
                Fsize = 0;
            }
            return FormatFileSize(Fsize, 2);
        }

        /// <summary>磁盘剩余空间计算</summary>
        /// <param name="TargetDisk">目标驱动器</param>
        /// <returns></returns>
        public static string DiskFreeSpace(string TargetDisk)
        {
            long x = new DriveInfo(TargetDisk).AvailableFreeSpace;
            return FormatFileSize(x, 2); ;
        }

        /// <summary>获取图片扩展名</summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetPicExt(Stream stream)
        {
            string strExt = "";
            if (stream == null)//判断文件是否存在
            {
                return strExt;
            }
            byte[] imagebytes = new byte[stream.Length];
            BinaryReader br = new BinaryReader(stream);//二进制文件读取器
            imagebytes = br.ReadBytes(2);//从当前流中将2个字节读入字节数组中
            string s = "";
            for (int i = 0; i < imagebytes.Length; i++)
            {
                s += imagebytes[i];
            }
            switch (s)
            {
                case "6677":
                    strExt = ".bmp";
                    break;
                case "7173":
                    strExt = ".gif";
                    break;
                case "7373":
                    strExt = ".tiff";
                    break;
                case "7777":
                    strExt = ".tiff";
                    break;
                case "13780":
                    strExt = ".png";
                    break;
                case "255216":
                    strExt = ".jpg";
                    break;
                case "208207":
                    strExt = ".doc";
                    break;
                case "6787":
                    strExt = ".swf";
                    break;
                default:
                    break;
            }
            br.Close();
            return strExt;
        }

        /// <summary>获取图片扩展名(图片格式，其他返回空值)</summary>
        /// <param name="filepath">文件路径</param>
        /// <returns>文件后缀，如bmp，jpg等</returns>
        public static string GetPicExt(string filepath)
        {
            filepath = CheckLastIndex(filepath);
            string strExt = "";
            if (!FileExists(filepath))//判断文件是否存在
            {
                return strExt;
            }
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            strExt = GetPicExt(fs);
            fs.Close();
            fs.Dispose();
            return strExt;
        }

        /// <summary> 提取扩展名</summary>
        /// <param name="FilePath">文件路径</param>
        public static string GetExt(string FilePath)
        {
            return Path.GetExtension(FilePath);
        }

        /// <summary>提取文件名</summary>
        /// <param name="FilePath">文件路径</param>
        public static string GetFileName(string FilePath)
        {
            return Path.GetFileName(FilePath);
        }

        /// <summary>提取目录路径</summary>
        /// <param name="FilePath">文件路径</param>
        public static string GetDirName(string FilePath)
        {
            return Path.GetDirectoryName(FilePath);
        }

        /// <summary>判断文件是否存在 </summary>
        /// <param name="strSourceFilePath">文件路径</param>
        /// <returns>判断结果</returns>
        public static bool FileExists(string strSourceFilePath)
        {
            try
            {
                strSourceFilePath = CheckLastIndex(strSourceFilePath);
                if (!File.Exists(strSourceFilePath))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>修改文件只读属性 </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns>bool值</returns>
        public static bool UpdateFileReadOnlyAttributes(string filepath)
        {
            filepath = CheckLastIndex(filepath);
            if (!FileExists(filepath))
            {
                return false;
            }
            try
            {
                FileInfo fi = new FileInfo(filepath);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                {
                    fi.Attributes = FileAttributes.Normal;
                }                
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>修改文件夹只读属性</summary>
        /// <param name="dirpath">文件路径</param>
        /// <returns>bool值</returns>
        public static bool UpdateFolderReadOnlyAttributes(string dirpath)
        {
            dirpath = CheckLastIndex(dirpath);
            if (!FolderExists(dirpath))
            {
                return false;
            }
            try
            {
                DirectoryInfo diFolder = new DirectoryInfo(dirpath);
                if (diFolder.Attributes.ToString().IndexOf("ReadOnly") != -1)
                {
                    diFolder.Attributes = FileAttributes.Normal;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>复制或者剪切文件</summary>
        /// <param name="strSourceFilePath">源文件路径</param>
        /// <param name="strTargetFilePath">目标文件路径</param>
        /// <param name="isOverWrite">是否允许覆盖</param>
        /// <param name="isCut">是否剪切</param>
        /// <returns>复制或剪切结果</returns>
        public static bool CopyFiles(string strSourceFilePath, string strTargetFilePath, bool isOverWrite, bool isCut)
        {
            try
            {
                strSourceFilePath = CheckLastIndex(strSourceFilePath);
                strTargetFilePath = CheckLastIndex(strTargetFilePath);
                //判断原文件是否存在
                if (!FileExists(strSourceFilePath))
                {
                    return false;
                }
                //判断目标文件夹是否存在
                int position = strTargetFilePath.LastIndexOf("\\") > strTargetFilePath.LastIndexOf("/") ? strTargetFilePath.LastIndexOf("\\") : strTargetFilePath.LastIndexOf("/");
                string strTargetFolderPath = strTargetFilePath.Substring(0, position + 1);
                if (!InitFolder(strTargetFolderPath))
                {
                    return false;
                }
                //判断目标文件是否存在
                if (FileExists(strTargetFilePath))
                {
                    if (!isOverWrite)
                    {
                        return false;
                    }
                }
                FileInfo file = new FileInfo(strSourceFilePath);
                file.CopyTo(strTargetFilePath, isOverWrite);
                if (FileExists(strTargetFilePath))
                {
                    return true;
                }

                if (isCut)
                {
                    if (!DeleteFile(strSourceFilePath))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>复制文件(覆盖目标文件)</summary>
        /// <param name="strSourceFilePath">源路径</param>
        /// <param name="strTargetFilePath">目标文件路径</param>
        /// <param name="isOverWrite">是否允许覆盖</param>
        /// <returns>复制结果</returns>
        public static bool CopyFiles(string strSourceFilePath, string strTargetFilePath)
        {
            return CopyFiles(strSourceFilePath, strTargetFilePath, true, false);
        }

        /// <summary>剪切文件（覆盖目标文件）</summary>
        /// <param name="strSourceFilePath">源路径</param>
        /// <param name="strTargetFilePath">目标文件路径</param>
        /// <returns>结果</returns>
        public static bool CutFiles(string strSourceFilePath, string strTargetFilePath)
        {
            return CopyFiles(strSourceFilePath, strTargetFilePath, true, true);
        }

        /// <summary>删除文件</summary>
        /// <param name="strSourceFilePath">要删除的文件路径</param>
        /// <returns>删除结果</returns>
        public static bool DeleteFile(string strSourceFilePath)
        {
            try
            {
                strSourceFilePath = CheckLastIndex(strSourceFilePath);
                if (!FileExists(strSourceFilePath))
                {
                    return true;
                }
                File.Delete(strSourceFilePath);
                if (FileExists(strSourceFilePath))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>删除文件夹</summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="isMe">如果为true,包含自己</param>
        /// <returns></returns>
        public static bool DeleteFolder(string path, bool isMe)
        {
            try
            {
                if (Directory.Exists(path)) //如果存在这个文件夹删除之 
                {
                    foreach (string d in Directory.GetFileSystemEntries(path))
                    {
                        if (File.Exists(d))
                        {
                            File.SetAttributes(d, FileAttributes.Normal);
                            File.Delete(d); //直接删除其中的文件 
                        }
                        else
                        {
                            DeleteFolder(d, isMe); //递归删除子文件夹 
                        }
                    }
                    if (isMe)
                    {
                        Directory.Delete(path); //删除已空文件夹 
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        ///<summary>删除指定目录的所有子文件和子目录</summary>
        ///<param name="strSourcePath">要删除的文件夹路径</param>
        ///<param name="isDdelSubDir">如果为true,删除子目录</param>
        public static bool DeleteAllFiles(string strSourcePath, bool isDdelSubDir)
        {
            try
            {
                strSourcePath = CheckLastIndex(strSourcePath);
                if (!FolderExists(strSourcePath))
                {
                    return true;
                }
                if (isDdelSubDir)
                {
                    if (!IsFoldersAndFilesAcsPro(strSourcePath))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!IsFilesAcsPro(strSourcePath))
                    {
                        return false;
                    }
                }
                foreach (string fileName in Directory.GetFiles(strSourcePath))
                {
                    File.SetAttributes(fileName, FileAttributes.Normal);
                    if (!DeleteFile(fileName))
                    {
                        return false;
                    }
                }
                if (isDdelSubDir)
                {
                    DirectoryInfo dir = new DirectoryInfo(strSourcePath);
                    foreach (DirectoryInfo subDi in dir.GetDirectories())
                    {
                        DeleteAllFiles(subDi.FullName, true);
                        subDi.Delete();
                        if (FolderExists(subDi.ToString()))
                        {
                            return false;
                        }
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        ///<summary>删除指定目录的所有文件和子目录</summary>
        ///<param name="strTargetDir">要删除的文件夹路径</param>
        public static bool DeleteAllFiles(string strTargetDir)
        {
            return DeleteAllFiles(strTargetDir, true);
        }

        /// <summary>创建一个文件</summary>
        /// <param name="strFileName">文件名和路径</param>
        /// <param name="isOverWrite">是否允许覆盖</param>
        /// <param name="chartSet">字符集</param>
        /// <param name="strText">要写入的文本</param>
        /// <param name="isWrite">不否写入</param>
        /// <returns></returns>
        public static bool CreateFile(string strFileName, string strText, Encoding chartSet, bool isOverWrite, bool isWrite)
        {
            FileStream fs = null;
            try
            {
                strFileName = CheckLastIndex(strFileName);
                int iPathLan = strFileName.LastIndexOf("\\") + 1;
                if (strFileName.LastIndexOf("\\") < strFileName.LastIndexOf("/"))
                {
                    iPathLan = strFileName.LastIndexOf("/") + 1;
                }
                string strFolderPath = strFileName.Substring(0, iPathLan);
                if (!InitFolder(strFolderPath))
                {
                    return false;
                }
                if (FileExists(strFileName) && !isOverWrite)
                {
                    return false;
                }
                if (isOverWrite)
                {
                    fs = new FileStream(strFileName, FileMode.Create, FileAccess.ReadWrite);
                }
                else
                {

                    fs = new FileStream(strFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                }

                if (isWrite)
                {
                    using (StreamWriter sw = new StreamWriter(fs, chartSet))
                    {
                        sw.Write(strText);
                    }
                }
                UpdateFileReadOnlyAttributes(strFileName);
                return FileExists(strFileName);
            }
            catch
            {
                return false;
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>创建一个文件</summary>
        /// <param name="strFileName">文件名和路径</param>
        /// <param name="isOverWrite">是否允许覆盖</param>
        /// <param name="chartSet">字符集</param>
        /// <param name="strText">要写入的文本</param>
        /// <returns></returns>
        public static bool CreateFile(string strFileName, string strText, Encoding chartSet, bool isOverWrite)
        {
            return CreateFile(strFileName, strText, chartSet, isOverWrite, true);
        }

        /// <summary> 创建一个文件</summary>
        /// <param name="strFileName">文件名和路径</param>
        /// <param name="chartSet">字符集</param>
        /// <param name="strText">要写入的文本</param>
        /// <returns></returns>
        public static bool CreateFile(string strFileName, string strText, Encoding chartSet)
        {
            return CreateFile(strFileName, strText, chartSet, false, true);
        }

        /// <summary> 创建一个文件</summary>
        /// <param name="strFileName">文件名和路径</param>
        /// <param name="strText">要写入的文本</param>
        /// <returns></returns>
        public static bool CreateFile(string strFileName, string strText)
        {
            return CreateFile(strFileName, strText, Encoding.Unicode, false, true);
        }

        /// <summary>创建一个文件</summary>
        /// <param name="strFileName">文件名和路径</param>
        /// <param name="isOverWrite">是否允许覆盖</param>
        /// <returns></returns>
        public static bool CreateFile(string strFileName, bool isOverWrite)
        {
            return CreateFile(strFileName, null, null, isOverWrite, false);
        }

        ///<summary>创建一个文件</summary>
        /// <param name="strFileName">文件名和路径</param>
        /// <returns></returns>
        public static bool CreateFile(string strFileName)
        {
            return CreateFile(strFileName, false);
        }

        /// <summary> 文件重命名</summary>
        /// <param name="strOriginalName">源路径和名称</param>
        /// <param name="fileName">文件名称（不用写后缀，默认为原文件的后缀）</param>
        /// <returns>重命名结果</returns>
        public static bool RenameFile(string strOriginalName, string fileName)
        {
            try
            {
                strOriginalName = CheckLastIndex(strOriginalName);
                if (!FileExists(strOriginalName))
                {
                    return false;
                }
                string newfilepath = strOriginalName.Substring(0, strOriginalName.LastIndexOf("\\") + 1) + fileName;
                if (strOriginalName.Contains("/"))
                {
                    newfilepath = strOriginalName.Substring(0, strOriginalName.LastIndexOf("/") + 1) + fileName;
                }
                int iposition = strOriginalName.LastIndexOf(".");
                newfilepath += strOriginalName.Substring(iposition, strOriginalName.Length - iposition);
                if (FileExists(newfilepath))
                {
                    return false;
                }
                File.Move(strOriginalName, newfilepath);
                if (!FileExists(newfilepath))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>初始化文件，如果文件不存在，则创建文件</summary>
        /// <param name="strFileName">文件名和路径</param>
        /// <returns></returns>
        public static bool InitFile(string strFileName)
        {
            strFileName = CheckLastIndex(strFileName);
            string folderPath = strFileName.Substring(0, strFileName.LastIndexOf("\\"));
            InitFolder(folderPath);
            try
            {
                if (!FileExists(strFileName))
                {
                    FileStream fs = new FileStream(strFileName, FileMode.CreateNew);
                    fs.Close();
                    fs.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> 判断是否是隐藏文件</summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsHidden(string path)
        {
            FileAttributes MyAttributes = File.GetAttributes(path);
            string MyFileType = MyAttributes.ToString();
            if (MyFileType.LastIndexOf("Hidden") != -1) //是否隐藏文件
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>获取文件的属性</summary>
        /// <param name="strFileName">文件路径和名称</param>
        /// <returns> 返回文件的属性</returns>
        public static Hashtable GetFileAttributes(string strFileName)
        {
            try
            {
                strFileName = CheckLastIndex(strFileName);
                FileInfo fiFile = new FileInfo(strFileName);

                if (!FileExists(strFileName))
                {
                    return null;
                }

                Hashtable htFile = new Hashtable();

                htFile.Add("文件名", fiFile.Name);
                htFile.Add("创建时间", fiFile.CreationTime.ToShortDateString());
                htFile.Add("上次访问时间", fiFile.LastAccessTime.ToShortDateString());
                htFile.Add("上次修改时间", fiFile.LastWriteTime.ToShortDateString());
                htFile.Add("路径", fiFile.DirectoryName);
                htFile.Add("是否只读", fiFile.IsReadOnly == true ? "只读" : "可写");
                htFile.Add("大小", (fiFile.Length / 1024).ToString() + "KB");
                htFile.Add("类型", String.IsNullOrEmpty(fiFile.Extension) == true ? "文件夹" : fiFile.Extension);
                htFile.Add("是否隐藏文件", IsHidden(strFileName) == true ? "是" : "不是");
                return htFile;
            }
            catch
            {

                return null;
            }
        }

        /// <summary>复制文件夹</summary>
        /// <param name="strSourceFolderPath">源路径</param>
        /// <param name="strTargetFolderPath">目标文件路径</param>
        /// <param name="isOverWrite">是否允许覆盖</param>
        /// <param name="isCut">是否剪切</param>
        /// <returns>复制的结果</returns>
        public static bool CopyFolder(string strSourceFolderPath, string strTargetFolderPath, bool isOverWrite, bool isCut)
        {
            try
            {
                if (!FolderExists(strSourceFolderPath))
                {
                    return false;
                }
                //判断原路径和目标路径是否重叠
                if (strSourceFolderPath == strTargetFolderPath.Substring(0, strSourceFolderPath.Length))
                {
                    return false;
                }
                //如果目标文件夹不存在，则创建目标文件夹
                if (!InitFolder(strTargetFolderPath))
                {
                    return false;
                }
                //取得要拷贝的文件夹名
                string strFolderName = strSourceFolderPath.Substring(strSourceFolderPath.LastIndexOf("\\") + 1, strSourceFolderPath.Length - strSourceFolderPath.LastIndexOf("\\") - 1);
                if (strSourceFolderPath.Contains("/"))
                {
                    strFolderName = strSourceFolderPath.Substring(strSourceFolderPath.LastIndexOf("/") + 1, strSourceFolderPath.Length - strSourceFolderPath.LastIndexOf("/") - 1);
                    //如果目标文件夹中没有源文件夹则在目标文件夹中创建源文件夹
                    if (!InitFolder(strTargetFolderPath + "/" + strFolderName))
                    {
                        return false;
                    }
                    string[] strFiles = Directory.GetFiles(strSourceFolderPath);//创建数组保存源文件夹下的文件名

                    for (int i = 0; i < strFiles.Length; i++)//循环拷贝文件
                    {
                        //取得拷贝的文件名，只取文件名，地址截掉。
                        string strFileName = strFiles[i].Substring(strFiles[i].LastIndexOf("/") + 1, strFiles[i].Length - strFiles[i].LastIndexOf("/") - 1);
                        //开始拷贝文件,true表示覆盖同名文件
                        if (!CopyFiles(strFiles[i], strTargetFolderPath + "/" + strFolderName, isOverWrite, isCut))
                        {
                            return false;
                        }
                    }

                    //创建DirectoryInfo实例
                    DirectoryInfo dirInfo = new DirectoryInfo(strSourceFolderPath);
                    //取得源文件夹下的所有子文件夹名称
                    DirectoryInfo[] ZiPath = dirInfo.GetDirectories();
                    for (int j = 0; j < ZiPath.Length; j++)
                    {
                        //获取所有子文件夹名
                        string strZiPath = strSourceFolderPath + "/" + ZiPath[j].ToString();
                        //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
                        CopyFolder(strZiPath, strTargetFolderPath + "/" + strFolderName);
                    }
                    return true;
                }

                //如果目标文件夹中没有源文件夹则在目标文件夹中创建源文件夹
                if (!InitFolder(strTargetFolderPath + "\\" + strFolderName))
                {
                    return false;
                }
                string[] strWFiles = Directory.GetFiles(strSourceFolderPath);//创建数组保存源文件夹下的文件名

                for (int i = 0; i < strWFiles.Length; i++)//循环拷贝文件
                {
                    //取得拷贝的文件名，只取文件名，地址截掉。
                    string strFileName = strWFiles[i].Substring(strWFiles[i].LastIndexOf("\\") + 1, strWFiles[i].Length - strWFiles[i].LastIndexOf("\\") - 1);
                    //开始拷贝文件,true表示覆盖同名文件
                    if (!CopyFiles(strWFiles[i], strTargetFolderPath + "\\" + strFolderName, isOverWrite, isCut))
                    {
                        return false;
                    }
                }

                //创建DirectoryInfo实例
                DirectoryInfo dirWInfo = new DirectoryInfo(strSourceFolderPath);
                //取得源文件夹下的所有子文件夹名称
                DirectoryInfo[] ZiWPath = dirWInfo.GetDirectories();
                for (int j = 0; j < ZiWPath.Length; j++)
                {
                    //获取所有子文件夹名
                    string strZiPath = strSourceFolderPath + "\\" + ZiWPath[j].ToString();
                    //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
                    CopyFolder(strZiPath, strTargetFolderPath + "\\" + strFolderName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>复制文件夹（不剪切）</summary>
        /// <param name="strSourceFolderPath">源路径</param>
        /// <param name="strTargetFolderPath">目标路径</param>
        /// <param name="isOverWrite">是否允许覆盖</param>
        /// <returns>复制的结果</returns>
        public static bool CopyFolder(string strSourceFolderPath, string strTargetFolderPath, bool isOverWrite)
        {
            return CopyFolder(strSourceFolderPath, strTargetFolderPath, isOverWrite, false);
        }

        /// <summary>复制或剪切文件夹（不覆盖）</summary>
        /// <param name="strSourceFolderPath">源路径</param>
        /// <param name="strTargetFolderPath">目标路径</param>
        /// <param name="isCut">是否剪切</param>
        /// <returns>复制或剪切的结果</returns>
        public static bool CopyFolder(bool isCut, string strSourceFolderPath, string strTargetFolderPath)
        {
            return CopyFolder(strSourceFolderPath, strTargetFolderPath, false, isCut);
        }

        /// <summary>复制文件夹(不覆盖，不剪切)</summary>
        /// <param name="strSourceFolderPath">源路径</param>
        /// <param name="strTargetFolderPath">目标路径</param>
        /// <returns>复制的结果</returns>
        public static bool CopyFolder(string strSourceFolderPath, string strTargetFolderPath)
        {
            return CopyFolder(strSourceFolderPath, strTargetFolderPath, false, false);
        }

        ///<summary>删除指定目录的所有子目录,不包括对当前目录文件的删除</summary>
        ///<param name="strTargetDir">目录路径</param>
        public static bool DeleteSubFolder(string strTargetDir)
        {
            try
            {
                strTargetDir = CheckLastIndex(strTargetDir);
                if (!FolderExists(strTargetDir))
                {
                    return true;
                }
                foreach (string subDir in Directory.GetDirectories(strTargetDir))
                {
                    if (!DeleteAllFolder(subDir))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        ///<summary>删除指定目录,包括当前目录和所有子目录和文件</summary>
        ///<param name="strTargetDir">目录路径</param>
        public static bool DeleteAllFolder(string strTargetDir)
        {
            try
            {
                strTargetDir = CheckLastIndex(strTargetDir);
                if (!FolderExists(strTargetDir))
                {
                    return true;
                }
                DirectoryInfo dirInfo = new DirectoryInfo(strTargetDir);
                if (!DeleteAllFiles(strTargetDir, true))
                {
                    return false;
                }
                dirInfo.Delete(true);
                if (FolderExists(strTargetDir))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>文件夹是否存在</summary>
        /// <param name="strFolderPath">文件夹路径</param>
        /// <returns>判断文件夹是否存在</returns>
        public static bool FolderExists(string strFolderPath)
        {
            try
            {
                strFolderPath = CheckLastIndex(strFolderPath);
                if (!Directory.Exists(strFolderPath))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>创建文件夹</summary>
        /// <param name="strFolderPath">文件路径</param>
        /// <returns></returns>
        public static bool CreateFolder(string strFolderPath)
        {
            try
            {
                strFolderPath = CheckLastIndex(strFolderPath);
                if (FolderExists(strFolderPath))
                {
                    return false;
                }
                Directory.CreateDirectory(strFolderPath);
                UpdateFolderReadOnlyAttributes(strFolderPath);
                if (!FolderExists(strFolderPath))
                {
                    return false;
                }
            }
            catch (AccessViolationException)
            {
                return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary> 初始化文件夹（不存在才创建）</summary>
        /// <param name="strFolderPath">文件路径</param>
        /// <returns>0为初始成功，其它为初始失败</returns>
        public static bool InitFolder(string strFolderPath)
        {
            try
            {
                strFolderPath = CheckLastIndex(strFolderPath);
                if (FolderExists(strFolderPath))
                {
                    return true;
                }

                Directory.CreateDirectory(strFolderPath);
                if (!FolderExists(strFolderPath))
                {
                    return false;
                }
            }
            catch (AccessViolationException)
            {
                return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>重命名文件夹</summary>
        /// <param name="strOriginalName">原文件夹名和路径</param>
        /// <param name="strTargetName">新文件夹名和路径</param>
        /// <returns>重命名结果</returns>
        public static bool RenameFolder(string strOriginalName, string strTargetName)
        {
            try
            {
                strOriginalName = CheckLastIndex(strOriginalName);
                strTargetName = CheckLastIndex(strTargetName);
                DirectoryInfo di = new DirectoryInfo(strOriginalName);
                if (!FolderExists(strOriginalName))
                {
                    return false;
                }
                if (!FolderExists(strTargetName))
                {
                    return false;
                }
                di.MoveTo(strTargetName);
                if (!FolderExists(strTargetName))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>获取文件夹属性</summary>
        /// <param name="strFolderName">文件夹路径</param>
        /// <returns>文件夹属性</returns>
        public static Hashtable GetFolderAttributes(string strFolderName)
        {
            try
            {
                strFolderName = CheckLastIndex(strFolderName);
                DirectoryInfo diFolder = new DirectoryInfo(strFolderName);
                if (!FolderExists(strFolderName))
                {
                    return null;
                }
                Hashtable htFolder = new Hashtable();

                htFolder.Add("名称", diFolder.Name.ToString());
                htFolder.Add("创建时间", diFolder.CreationTime.ToShortDateString());
                htFolder.Add("上次访问时间", diFolder.LastAccessTime.ToShortDateString());
                htFolder.Add("上次修改时间", diFolder.LastWriteTime.ToShortDateString());
                htFolder.Add("文件路径", diFolder.FullName);

                return htFolder;

            }
            catch
            {
                return null;
            }
        }

        /// <summary>以一个文件夹的框架在另一个目录创建文件夹和空文件</summary>
        /// <param name="OrignFolder">源路径</param>
        /// <param name="NewFolder">目标路径</param>
        public static void FolderBuild(string orignFolder, string NewFolder)
        {
            string path = (NewFolder.LastIndexOf("\\") == NewFolder.Length - 1) ? NewFolder : NewFolder + "\\";
            string parent = Path.GetDirectoryName(orignFolder);
            Directory.CreateDirectory(path + Path.GetFileName(orignFolder));
            DirectoryInfo dir = new DirectoryInfo((orignFolder.LastIndexOf("\\") == orignFolder.Length - 1) ? orignFolder : orignFolder + "\\");
            FileSystemInfo[] fileArr = dir.GetFileSystemInfos();
            Queue<FileSystemInfo> Folders = new Queue<FileSystemInfo>(dir.GetFileSystemInfos());
            while (Folders.Count > 0)
            {
                FileSystemInfo tmp = Folders.Dequeue();
                FileInfo f = tmp as FileInfo;
                if (f == null)
                {
                    DirectoryInfo d = tmp as DirectoryInfo;
                    Directory.CreateDirectory(d.FullName.Replace((parent.LastIndexOf("\\") == parent.Length - 1) ? parent : parent + "\\", path));
                    foreach (FileSystemInfo fi in d.GetFileSystemInfos())
                    {
                        Folders.Enqueue(fi);
                    }
                }
                else
                {
                    File.Create(f.FullName.Replace(parent, path));
                }
            }
        }

        /// <summary>判断驱动器是否存在</summary>
        /// <param name="strDriverName">驱动器名称</param>
        /// <returns>判断驱动器是否存在</returns>
        public static bool DriverExists(string strDriverName)
        {
            try
            {
                DriveInfo di = new DriveInfo(strDriverName);
                return true;

            }
            catch (DriveNotFoundException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>获取驱动器属性</summary>
        /// <param name="strDriverName">驱动器名称</param>
        /// <returns></returns>
        public static Hashtable GetDriverAttributes(string strDriverName)
        {
            try
            {
                Hashtable htDriver = new Hashtable();
                DriveInfo diDriver = new DriveInfo(strDriverName);
                htDriver.Add("名称", diDriver.Name);
                htDriver.Add("文件系统", diDriver.DriveFormat);
                htDriver.Add("卷标", diDriver.VolumeLabel);
                htDriver.Add("总大小", (diDriver.TotalSize / 1024).ToString() + "KB");
                htDriver.Add("可用空间", (diDriver.TotalFreeSpace / 1024).ToString() + "KB");
                htDriver.Add("路径", diDriver.RootDirectory.ToString());
                return htDriver;
            }
            catch (DriveNotFoundException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>写文件</summary>
        /// <param name="strFileName">文件路径</param>
        /// <param name="strText">要写入的文本</param>
        /// <param name="charset">要采用的字符集</param>
        /// <param name="isAppend">是否允许追加</param>
        /// <returns>返回写入结果</returns>
        public static bool STMSetText(string strFileName, string strText, Encoding charset, bool isAppend)
        {
            try
            {
                strFileName = CheckLastIndex(strFileName);
                if (!FileExists(strFileName))
                {
                    return false;
                }
                using (StreamWriter sw = new StreamWriter(strFileName, isAppend, charset))
                {
                    sw.WriteLine(strText);
                    sw.Flush();
                    sw.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>写文件(Unicode编码)</summary>
        /// <param name="strFileName">文件路径</param>
        /// <param name="strText">要写入的文本</param>
        /// <param name="isAppend">是否允许追加</param>
        /// <returns>返回写入结果</returns>
        public static bool STMSetText(string strFileName, string strText, bool isAppend)
        {
            return STMSetText(strFileName, strText, Encoding.Unicode, isAppend);
        }

        /// <summary>写文件（追加）</summary>
        /// <param name="strFileName">文件路径</param>
        /// <param name="strText">要写入的文本</param>
        /// <param name="charset">要采用的字符集</param>
        /// <returns>返回写入结果</returns>
        public static bool STMSetText(string strFileName, string strText, Encoding charset)
        {
            return STMSetText(strFileName, strText, charset, true);
        }

        /// <summary>写文件(ASCII编码)</summary>
        /// <param name="strFileName">文件路径</param>
        /// <param name="strText">要写入的文本</param>
        /// <returns>返回写入结果</returns>
        public static bool STMSetText(string strFileName, string strText)
        {
            return STMSetText(strFileName, strText, Encoding.ASCII, true);
        }

        /// <summary> 读文本文件内容</summary>
        /// <param name="strFileName">文件路径</param>
        /// <param name="charset">要采用的字符集</param>
        /// <returns>返回从流中读出的数据</returns>
        public static string STMGetText(string strFileName, Encoding charset)
        {
            try
            {
                strFileName = CheckLastIndex(strFileName);
                using (StreamReader sr = new StreamReader(strFileName, charset))
                {
                    return sr.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>读文本文件内容</summary>
        /// <param name="strFileName">文件路径</param>
        /// <returns>返回从流中读出的数据</returns>
        public static string STMGetText(string strFileName)
        {
            try
            {
                strFileName = CheckLastIndex(strFileName);
                using (StreamReader sr = new StreamReader(strFileName))
                {
                    return sr.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>将二进制数据写入文件中</summary>
        /// <param name="buff">二进制数据</param>
        /// <param name="offset">开始位置</param>
        /// <param name="len">结束位置</param>
        /// <param name="filePath">文件路径</param>   
        public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(buff, offset, len);
            bw.Flush();

            bw.Close();
            fs.Close();
        }

        /// <summary>将二进制数据写入文件中</summary>
        /// <param name="buff">二进制数据</param>
        /// <param name="filePath">文件路径</param>
        public static void WriteBuffToFile(byte[] buff, string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(buff);
            bw.Flush();

            bw.Close();
            fs.Close();
        }

        /// <summary>从文件中读取二进制数据</summary>
        /// <param name="filePath">文件路径</param>
        /// <returns> byte[]二进制数据</returns>
        public static byte[] ReadFileReturnBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            BinaryReader br = new BinaryReader(fs);

            byte[] buff = br.ReadBytes((int)fs.Length);

            br.Close();
            fs.Close();

            return buff;
        }

        /// <summary>判断文件夹下的子文件夹或文件是否有访问保护</summary>
        /// <param name="strSourcePath">文件夹路径</param>
        /// <returns>0为没有访问保护,其它为有访问保护</returns>
        public static bool IsFoldersAndFilesAcsPro(string strSourcePath)
        {
            try
            {
                if (strSourcePath.LastIndexOf("/") > 0 && strSourcePath.IndexOf(":") <= 0)
                {
                    return true;
                }
                strSourcePath = CheckLastIndex(strSourcePath);
                DirectoryInfo dir = new DirectoryInfo(strSourcePath);
                foreach (FileInfo fi in dir.GetFiles())
                {
                    if (!IsFileAcsPro(fi.FullName))
                    {
                        return false;
                    }
                }
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    if (!IsFolderAcsPro(subDir.FullName))
                    {
                        return false;
                    }
                    if (!IsFoldersAndFilesAcsPro(subDir.FullName))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> 判断文件夹是否有访问保护</summary>
        /// <param name="strSourcePath">文件夹路径</param>
        /// <returns>0为没有访问保护,其它为有访问保护</returns>
        public static bool IsFolderAcsPro(string strSourcePath)
        {
            try
            {
                if (strSourcePath.LastIndexOf("/") > 0 && strSourcePath.IndexOf(":") <= 0)
                {
                    return true;
                }
                strSourcePath = CheckLastIndex(strSourcePath);
                DirectoryInfo diFolder = new DirectoryInfo(strSourcePath);
                if (diFolder.GetAccessControl().AreAccessRulesProtected)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>判断文件是否有访问保护</summary>
        /// <param name="strSourcePath">文件路径</param>
        /// <returns>0为没有访问保护,其它为有访问保护</returns>
        public static bool IsFileAcsPro(string strSourcePath)
        {
            try
            {
                if (strSourcePath.LastIndexOf("/") > 0 && strSourcePath.IndexOf(":") <= 0)
                {
                    return true;
                }
                strSourcePath = CheckLastIndex(strSourcePath);
                FileInfo fiFile = new FileInfo(strSourcePath);
                if (fiFile.GetAccessControl().AreAccessRulesProtected)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>判断文件夹下的所有文件是否有访问保护</summary>
        /// <param name="strSourcePath">文件夹路径</param>
        /// <returns>0为没有访问保护,其它为有访问保护</returns>
        public static bool IsFilesAcsPro(string strSourcePath)
        {
            try
            {
                if (strSourcePath.LastIndexOf("/") > 0 && strSourcePath.IndexOf(":") <= 0)
                {
                    return true;
                }
                strSourcePath = CheckLastIndex(strSourcePath);
                DirectoryInfo dir = new DirectoryInfo(strSourcePath);
                foreach (FileInfo fi in dir.GetFiles())
                {
                    if (!IsFileAcsPro(fi.FullName))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>计算文件的 sha1 值</summary>
        /// <param name="fileName">要计算 sha1 值的文件名和路径</param>
        /// <returns>sha1 值16进制字符串</returns>
        public static string SHA1File(string fileName)
        {
            return HashFile(fileName, "sha1");
        }

        /// <summary>计算文件的 MD5 值</summary>
        /// <param name="fileName">要计算 MD5 值的文件名和路径</param>
        /// <returns>MD5 值16进制字符串</returns>
        public static string MD5File(string fileName)
        {
            return HashFile(fileName, "md5");
        }

        public static bool SaveBinaryFile(Stream inStream, string savePath)
        {
            bool value = false;
            byte[] buffer = new byte[1024];
            Stream outStream = null;
            try
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                outStream = System.IO.File.Create(savePath);
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                } while (l > 0);
                value = true;
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
            return value;
        }
        #endregion
    }
}
