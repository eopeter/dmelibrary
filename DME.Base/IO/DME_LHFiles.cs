using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DME.Base.Common;

namespace DME.Base.IO
{
    /// <summary>
    /// 带Crc8认证的文件操作类
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
    public class DME_LHFiles
    {
        #region 私有变量
        /// <summary>
        /// 拷贝成果
        /// </summary>
        private const int OK = 0x00; //成功
        /// <summary>
        /// 未找到 sourceFileName
        /// </summary>
        private const int FileNotFound = 0x01; //源文件不存在

        /// <summary>
        /// sourceFileName 或 destFileName 是一个零长度字符串，仅包含空白或者包含一个或多个由 InvalidPathChars 定义的无效字符。
        /// - 或 - 
        /// sourceFileName 或 destFileName 指定目录。
        /// </summary>
        private const int ArgumentError = 0x02;

        /// <summary>
        /// sourceFileName 或 destFileName 为 空引用（在 Visual Basic 中为 Nothing）。
        /// </summary>
        private const int ArgumentNullError = 0x03;

        /// <summary>
        /// 指定的路径、文件名或者两者都超出了系统定义的最大长度。例如，在基于 Windows 的平台上，路径必须小于 248 个字符，文件名必须小于 260 个字符。
        /// </summary>
        private const int PathTooLong = 0x04;
        /// <summary>
        /// 调用方没有所要求的权限
        /// </summary>
        private const int UnauthorizedAccess = 0x05;
        /// <summary>
        /// 在 sourceFileName 或 destFileName 中指定的路径无效（例如，它位于未映射的驱动器上）。
        /// </summary>
        private const int DirectoryNotFound = 0x06;
        /// <summary>
        /// destFileName 是只读的，或者 destFileName 存在并且 overwrite 是 false。
        /// - 或 - 
        /// 出现 I/O 错误。
        /// </summary>
        private const int IOError = 0x07;
        /// <summary>
        /// sourceFileName 或 destFileName 的格式无效。
        /// </summary>
        private const int NotSupported = 0x08;
        /// <summary>
        /// 拷贝完成但文件大小不一致。
        /// </summary>
        private const int FileLengthDiffrent = 0x09;
        /// <summary>
        /// Crc码获取异常
        /// </summary>
        private const int CrcGetError = 0x0A;
        /// <summary>
        /// Crc校验结果错误
        /// </summary>
        private const int CrcCheckError = 0x0B;
        /// <summary>
        /// 未知错误
        /// </summary>
        private const int UnkonwnError = 0x0C;

        /// <summary>
        /// 错误描述
        /// </summary>
        private string ErrorString = "";
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
        #endregion

        #region 公开函数
        /// <summary>拷贝文件，0：成功，非0：对应的错误码 </summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="destFile">目标文件</param>
        /// <param name="overWrite">是否覆盖</param>
        /// <returns>执行结果编码</returns>
        public int LHCopyFile(string sourceFile, string destFile, bool overWrite)
        {
            int resultCode = OK;

            try
            {
                File.Copy(sourceFile, destFile, overWrite);
                resultCode = OK;
            }
            catch (UnauthorizedAccessException)
            {
                resultCode = UnauthorizedAccess;  //无权限
            }
            catch (DirectoryNotFoundException)
            {
                resultCode = DirectoryNotFound;  //未映射到驱动器上
            }
            catch (PathTooLongException)
            {
                resultCode = PathTooLong;
            }
            catch (IOException)
            {
                resultCode = IOError;  //I/O错误
            }
            catch (NotSupportedException)
            {
                resultCode = NotSupported;  //格式无效
            }
            catch (ArgumentNullException )
            {
                resultCode = ArgumentNullError;
            }
            catch (ArgumentException)
            {
                resultCode = ArgumentError;
            }
            catch (Exception)
            {
                resultCode = UnkonwnError;
            }
            finally
            {
                ErrorString = LHGetLastError(resultCode);
            }

            return resultCode;
        }

        /// <summary>拷贝文件，0：成功，非0：对应的错误码</summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="destFile">目标文件</param>
        /// <param name="overWrite">是否覆盖</param>
        /// <param name="crcCheck">是否进行Crc校验</param>
        /// <returns>执行结果编码</returns>
        public int LHCopyFile(string sourceFile, string destFile, bool overWrite, bool crcCheck)
        {
            #region
            int resultCode = OK;

            try
            {
                File.Copy(sourceFile, destFile, overWrite);

                resultCode = OK;
                if (crcCheck)//Crc校验
                {
                    uint CrcSourceFile;
                    uint CrcDestFile;
                    FileInfo sourceFileInfo = new FileInfo(sourceFile);
                    FileInfo destFileInfo = new FileInfo(destFile);

                    if (sourceFileInfo.Length != destFileInfo.Length)
                    {
                        throw new FileLengthException();
                    }
                    else
                    {
                        CrcSourceFile = DME_Crc8.CrcGet(sourceFile);
                        CrcDestFile = DME_Crc8.CrcGet(destFile);
                        if (CrcDestFile != CrcSourceFile)
                        {
                            throw new CrcCheckException();
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                resultCode = UnauthorizedAccess;  //无权限
            }
            catch (DirectoryNotFoundException ex)
            {
                resultCode = DirectoryNotFound;  //未映射到驱动器上
            }
            catch (PathTooLongException ex)
            {
                resultCode = PathTooLong;
            }
            catch (IOException ex)
            {
                resultCode = IOError;  //I/O错误
            }
            catch (NotSupportedException ex)
            {
                resultCode = NotSupported;  //格式无效
            }
            catch (ArgumentNullException ex)
            {
                resultCode = ArgumentNullError;
            }
            catch (ArgumentException ex)
            {
                resultCode = ArgumentError;
            }
            catch (FileLengthException ex)
            {
                resultCode = FileLengthDiffrent;
            }
            catch (CrcGetException ex)
            {
                resultCode = CrcGetError;
            }
            catch (CrcCheckException ex)
            {
                resultCode = CrcCheckError;
            }
            catch (Exception ex)
            {
                resultCode = UnkonwnError;
            }
            finally
            {
                ErrorString = LHGetLastError(resultCode);
            }

            return resultCode;
            #endregion
        }

        /// <summary>将目录中的文件拷贝到新的位置,如果目标文件存在，则将其覆盖。0：成功，非0：对应的错误码</summary>
        /// <param name="directoryToGetFrom">源目录路径</param>
        /// <param name="directoryToMoveTo">新目录路径</param>
        /// <param name="searchPattern">匹配方式，支持*和？号通配符</param>
        /// <param name="recursive">是否递归</param>
        /// <returns>执行结果编码</returns>
        public int LHCopyFiles(string directoryToGetFrom, string directoryToCopyTo, string searchPattern, bool recursive)
        {
            int resultCode = OK;
            int tempResult;
            string[] files = null;
            string[] folders = null;

            try
            {
                files = Directory.GetFiles(directoryToGetFrom, searchPattern);
            }
            catch (UnauthorizedAccessException)
            {
                resultCode = UnauthorizedAccess;  //无权限
            }
            catch (DirectoryNotFoundException)
            {
                resultCode = DirectoryNotFound;  //未映射到驱动器上
            }
            catch (PathTooLongException)
            {
                resultCode = PathTooLong;
            }
            catch (IOException)
            {
                resultCode = IOError;  //I/O错误
            }
            catch (ArgumentNullException)
            {
                resultCode = ArgumentNullError;
            }
            catch (ArgumentException)
            {
                resultCode = ArgumentError;
            }
            catch (Exception)
            {
                resultCode = UnkonwnError;
            }
            finally
            {
                ErrorString = LHGetLastError(resultCode);
            }

            foreach (string file in files)
            {
                string tempFile = file.Replace(directoryToGetFrom, directoryToCopyTo);
                tempResult = LHCopyFile(file, tempFile, true, true);
                if (tempResult != OK)
                    resultCode = tempResult;
            }

            if (recursive)
            {
                tempResult = LHGetSubFolders(directoryToGetFrom, out folders);
                if (tempResult != OK)
                    resultCode = tempResult;

                foreach (string folder in folders)
                {
                    string tempDirectoryCopyTo = null;

                    try
                    {
                        tempDirectoryCopyTo = folder.Replace(directoryToGetFrom, directoryToCopyTo);
                    }
                    catch (Exception)
                    {
                        resultCode = UnkonwnError;
                    }

                    if (!Directory.Exists(tempDirectoryCopyTo))
                    {
                        try
                        {
                            Directory.CreateDirectory(tempDirectoryCopyTo);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            resultCode = UnauthorizedAccess;  //无权限
                        }
                        catch (DirectoryNotFoundException)
                        {
                            resultCode = DirectoryNotFound;  //未映射到驱动器上
                        }
                        catch (PathTooLongException)
                        {
                            resultCode = PathTooLong;
                        }
                        catch (IOException)
                        {
                            resultCode = IOError;  //I/O错误
                        }
                        catch (ArgumentNullException)
                        {
                            resultCode = ArgumentNullError;
                        }
                        catch (ArgumentException)
                        {
                            resultCode = ArgumentError;
                        }
                        catch (NotSupportedException)
                        {
                            resultCode = NotSupported;
                        }
                        catch (Exception)
                        {
                            resultCode = UnkonwnError;
                        }
                        finally
                        {
                            ErrorString = LHGetLastError(resultCode);
                        }
                    }

                    tempResult = LHCopyFiles(folder, tempDirectoryCopyTo, searchPattern, true);
                    if (tempResult != OK)
                        resultCode = tempResult;
                }
            }

            return resultCode;

        }

        /// <summary>将目录中的文件移动到新的位置,如果目标文件存在，则将其覆盖。0：成功，非0：对应的错误码</summary>
        /// <param name="directoryToGetFrom">源目录路径</param>
        /// <param name="directoryToMoveTo">新目录路径</param>
        /// <param name="searchPattern">匹配方式，支持*和？号通配符</param>
        /// <param name="recursive">是否递归</param>
        /// <returns>执行结果编码</returns>
        public int LHMoveFiles(string directoryToGetFrom, string directoryToMoveTo, string searchPattern, bool recursive)
        {
            int resultCode = OK;

            resultCode = LHCopyFiles(directoryToGetFrom, directoryToMoveTo, searchPattern, recursive);
            if (resultCode == OK)
                resultCode = LHDeleteFiles(directoryToGetFrom, searchPattern, recursive);

            return resultCode;
        }

        /// <summary>删除文件，0：成功，非0：对应的错误码</summary>
        /// <param name="filePath">待删除文件的路径</param>
        /// <returns>执行结果编码</returns>
        public int LHDeleteFile(string filePath)
        {
            #region
            int resultCode = OK;

            try
            {
                File.Delete(filePath);
                resultCode = OK;
            }
            catch (UnauthorizedAccessException)
            {
                resultCode = UnauthorizedAccess;  //无权限
            }
            catch (DirectoryNotFoundException)
            {
                resultCode = DirectoryNotFound;  //未映射到驱动器上
            }
            catch (PathTooLongException)
            {
                resultCode = PathTooLong;
            }
            catch (IOException)
            {
                resultCode = IOError;  //I/O错误
            }
            catch (NotSupportedException)
            {
                resultCode = NotSupported;  //格式无效
            }
            catch (ArgumentNullException)
            {
                resultCode = ArgumentNullError;
            }
            catch (ArgumentException)
            {
                resultCode = ArgumentError;
            }
            catch (Exception)
            {
                resultCode = UnkonwnError;
            }
            finally
            {
                ErrorString = LHGetLastError(resultCode);
            }

            return resultCode;
            #endregion
        }

        /// <summary>删除目录中所有子目录，0：成功，非0：对应的错误码</summary>
        /// <param name="directoryPath">目录路径</param>
        /// <returns>执行结果编码</returns>
        public int LHDeleteSubFolders(string directoryPath)
        {

            #region
            int resultCode = OK;
            int tempResult;
            string[] folders = null;

            tempResult = LHGetSubFolders(directoryPath, out folders);
            if (tempResult != OK)
                resultCode = tempResult;

            foreach (string folder in folders)
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (UnauthorizedAccessException)
                {
                    resultCode = UnauthorizedAccess;  //无权限
                }
                catch (DirectoryNotFoundException)
                {
                    resultCode = DirectoryNotFound;  //未映射到驱动器上
                }
                catch (PathTooLongException)
                {
                    resultCode = PathTooLong;
                }
                catch (IOException)
                {
                    resultCode = IOError;  //I/O错误
                }
                catch (ArgumentNullException)
                {
                    resultCode = ArgumentNullError;
                }
                catch (ArgumentException)
                {
                    resultCode = ArgumentError;
                }
                catch (Exception)
                {
                    resultCode = UnkonwnError;
                }
                finally
                {
                    ErrorString = LHGetLastError(resultCode);
                }
            }

            return resultCode;
            #endregion
        }

        /// <summary>删除目录中所有文件，0：成功，非0：对应的错误码</summary>
        /// <param name="directoryPath">目录路径</param>
        /// <returns>执行结果编码</returns>
        public int LHDeleteFiles(string directoryPath)
        {
            #region
            int resultCode = OK;
            int tempResult;
            string[] files = null;

            tempResult = LHGetSubFiles(directoryPath, out files);
            if (tempResult != OK)
                resultCode = tempResult;

            foreach (string file in files)
            {
                tempResult = LHDeleteFile(file);
                if (tempResult != OK)
                    resultCode = tempResult;
            }

            return resultCode;
            #endregion
        }

        /// <summary>删除指定目录中与指定搜索模式匹配的文件，0：成功，非0：对应的错误码</summary>
        /// <param name="directoryPath">目录路径</param>
        /// <param name="searchPattern">匹配方式，支持*和？号通配符</param>
        /// <returns>执行结果编码</returns>
        public int LHDeleteFiles(string directoryPath, string searchPattern)
        {
            #region
            int resultCode = OK;
            int tempResult;
            string[] files = null;

            try
            {
                files = Directory.GetFiles(directoryPath, searchPattern);
            }
            catch (UnauthorizedAccessException)
            {
                resultCode = UnauthorizedAccess;  //无权限
            }
            catch (DirectoryNotFoundException)
            {
                resultCode = DirectoryNotFound;  //未映射到驱动器上
            }
            catch (PathTooLongException)
            {
                resultCode = PathTooLong;
            }
            catch (IOException)
            {
                resultCode = IOError;  //I/O错误
            }
            catch (ArgumentNullException)
            {
                resultCode = ArgumentNullError;
            }
            catch (ArgumentException)
            {
                resultCode = ArgumentError;
            }
            catch (Exception)
            {
                resultCode = UnkonwnError;;
            }

            foreach (string file in files)
            {
                tempResult = LHDeleteFile(file);
                if (tempResult != OK)
                    resultCode = tempResult;
            }

            return resultCode;
            #endregion
        }

        /// <summary>递归删除指定目录中与指定搜索模式匹配的文件，0：成功，非0：对应的错误码</summary>
        /// <param name="directoryPath">目录路径</param>
        /// <param name="searchPattern">匹配方式，支持*和？号通配符</param>
        /// <param name="recursive">是否递归</param>
        /// <returns>执行结果编码</returns>
        public int LHDeleteFiles(string directoryPath, string searchPattern, bool recursive)
        {
            #region
            int resultCode = OK;
            int tempResult;
            string[] folders = null;

            resultCode = LHDeleteFiles(directoryPath, searchPattern);

            if (recursive == true)
            {
                tempResult = LHGetSubFolders(directoryPath, out folders);
                if (tempResult != OK)
                    resultCode = tempResult;

                foreach (string folder in folders)
                {
                    tempResult = LHDeleteFiles(folder, searchPattern, true);
                    if (tempResult != OK)
                        resultCode = tempResult;
                }
            }

            return resultCode;
            #endregion
        }

        /// <summary>得到最后的错误信息</summary>
        public string LHGetLastError()
        {
            return ErrorString;
        }

        /// <summary>得到最后的错误信息</summary>
        public string LHGetLastError(int ErrorCode)
        {
            string sErrorString = "";
            switch (ErrorCode)
            {
                #region
                case OK:
                    sErrorString = "";
                    break;
                case FileNotFound:
                    sErrorString = "未找到 sourceFileName";
                    break;
                case ArgumentError:
                    sErrorString = "sourceFileName 或 destFileName 是一个零长度字符串";
                    break;
                case ArgumentNullError:
                    sErrorString = "sourceFileName 或 destFileName 为 空引用";
                    break;
                case PathTooLong:
                    sErrorString = "指定的路径、文件名或者两者都超出了系统定义的最大长度";
                    break;
                case UnauthorizedAccess:
                    sErrorString = "调用方没有所要求的权限";
                    break;
                case DirectoryNotFound:
                    sErrorString = "在 sourceFileName 或 destFileName 中指定的路径无效";
                    break;
                case IOError:
                    sErrorString = "destFileName 是只读的，或者 destFileName 存在并且 overwrite 是 false";
                    break;
                case NotSupported:
                    sErrorString = "sourceFileName 或 destFileName 的格式无效";
                    break;
                case FileLengthDiffrent:
                    sErrorString = "源文件与目标文件大小不一致";
                    break;
                case CrcCheckError:
                    sErrorString = "Crc校验码不相同";
                    break;
                case CrcGetError:
                    sErrorString = "Crc码获取异常";
                    break;
                default:
                    sErrorString = "未知错误！";
                    break;
                #endregion
            }
            return sErrorString;
        }

        /// <summary>获取子目录，0：成功，非0：对应的错误码</summary>
        /// <param name="directoryPath">源目录路径</param>
        /// <param name="folders">输出的子目录集合</param>
        /// <returns>执行结果编码</returns>
        public int LHGetSubFolders(string directoryPath, out string[] folders)
        {
            int resultCode = OK;
            folders = null;

            try
            {
                folders = Directory.GetDirectories(directoryPath);
                resultCode = OK;
            }
            catch (UnauthorizedAccessException)
            {
                resultCode = UnauthorizedAccess;  //无权限
            }
            catch (DirectoryNotFoundException)
            {
                resultCode = DirectoryNotFound;  //未映射到驱动器上
            }
            catch (PathTooLongException)
            {
                resultCode = PathTooLong;
            }
            catch (IOException)
            {
                resultCode = IOError;  //I/O错误
            }
            catch (ArgumentNullException)
            {
                resultCode = ArgumentNullError;
            }
            catch (ArgumentException)
            {
                resultCode = ArgumentError;
            }
            catch (Exception)
            {
                resultCode = UnkonwnError;
            }
            finally
            {
                ErrorString = LHGetLastError(resultCode);
            }

            return resultCode;
        }

        /// <summary>获取目录下所有文件，0：成功，非0：对应的错误码</summary>
        /// <param name="directoryPath">目录路径</param>
        /// <param name="files">输出的文件集合</param>
        /// <returns>执行结果编码</returns>
        public int LHGetSubFiles(string directoryPath, out string[] files)
        {
            int resultCode = OK;
            files = null;

            try
            {
                files = Directory.GetFiles(directoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                resultCode = UnauthorizedAccess;  //无权限
            }
            catch (DirectoryNotFoundException)
            {
                resultCode = DirectoryNotFound;  //未映射到驱动器上
            }
            catch (PathTooLongException)
            {
                resultCode = PathTooLong;
            }
            catch (IOException)
            {
                resultCode = IOError;  //I/O错误
            }
            catch (ArgumentNullException)
            {
                resultCode = ArgumentNullError;
            }
            catch (ArgumentException)
            {
                resultCode = ArgumentError;
            }
            catch (Exception)
            {
                resultCode = UnkonwnError;
            }
            finally
            {
                ErrorString = LHGetLastError(resultCode);
            }

            return resultCode;
        }
        #endregion
    }

    /// <summary>文件Crc校验结果异常类</summary>
    public class CrcCheckException : Exception
    { }

    /// <summary>源文件与目标文件大小匹配异常类</summary>
    public class FileLengthException : Exception
    { }
}
