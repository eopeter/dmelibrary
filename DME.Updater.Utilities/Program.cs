using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DME.Updater.Utilities
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {

            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "deletetmp": DelayDelteFile(int.Parse(args[1]), args[2]); break;
                    case "deletetarget": DeleteTarget(args[1], args[2]); break;
                    default:
                        break;
                }
            }
        }

        #region 根据正则表达式脚本删除文件或目录
        /// <summary>
        /// 删除目标
        /// </summary>
        /// <param name="scriptFile">脚本文件</param>
        /// <param name="targetDirectory">目标目录</param>
        static void DeleteTarget(string scriptFile, string targetDirectory)
        {
            if (!System.IO.File.Exists(scriptFile) || !System.IO.Directory.Exists(targetDirectory)) return;
            var lines = System.IO.File.ReadAllLines(scriptFile);
            var regs = new Regex[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != null) regs[i] = new Regex(lines[i], RegexOptions.IgnoreCase);
            }

            //search all files
            SearchDirectory(targetDirectory, regs);
        }

        /// <summary>
        /// 搜索目录并删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regs"></param>
        static void SearchDirectory(string path, Regex[] regs)
        {
            if (ValidateFileName(path, regs))
            {
                try
                {
                    System.IO.Directory.Delete(path, true);
                }
                catch (Exception) { return; }

                return;
            }

            string[] directories, files;
            try
            {
                directories = System.IO.Directory.GetDirectories(path);
                files = System.IO.Directory.GetFiles(path);
            }
            catch (Exception)
            {
                return;
            }

            foreach (var d in directories)
            {
                SearchDirectory(d, regs);
            }

            foreach (var f in files)
            {
                if (!ValidateFileName(f, regs)) continue;
                try
                {
                    System.IO.File.Delete(f);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 确定一个路径是否符合规则
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regs"></param>
        /// <returns></returns>
        static bool ValidateFileName(string path, Regex[] regs)
        {
            foreach (var r in regs)
            {
                if (r.IsMatch(path)) return true;
            }
            return false;
        }
        #endregion


        /// <summary>
        /// 延迟删除程序
        /// </summary>
        static void DelayDelteFile(int pid, string path)
        {
            try
            {
                var process = System.Diagnostics.Process.GetProcessById(pid);
                if (process != null && !process.HasExited) process.WaitForExit();
            }
            catch (Exception)
            {
            }
            if (System.IO.Directory.Exists(path))
                System.IO.Directory.Delete(path, true);
        }
    }
}
