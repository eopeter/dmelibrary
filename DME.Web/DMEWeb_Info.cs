using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DME.Web
{
    /// <summary>
    /// 提供程序集基本信息
    /// </summary>
    public static class DMEWeb_Info
    {
        private static string _helpLink = "http://www.miaodo.com";

        private static string _adminEmail = "gzlance@163.com";
        /// <summary>
        /// 获取或设置管理员的电子邮件地址。
        /// </summary>
        /// <remarks>此地址将显示到生成的错误日志中。</remarks>
        public static string AdminEmail { get { return _adminEmail; }}

        private static Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

        /// <summary>
        /// 获取已加载DME.Base.dll的内部版本号。
        /// </summary>
        public static Version Version { get { return asm.GetName().Version; } }
        /// <summary>
        /// 获取已加载DME.Base.dll的路径。
        /// </summary>
        public static string Path { get { return asm.Location; } }
        /// <summary>
        /// 获取已加载DME.Base.dll的程序集名称。
        /// </summary>
        public static string Name { get { return asm.GetName().Name; } }
        /// <summary>
        /// 获取已加载DME.Base.dll的URL位置。
        /// </summary>
        public static string CodeBase { get { return asm.GetName().CodeBase; } }

        /// <summary>
        /// 获取DME.Base的官方帮助链接地址。
        /// </summary>
        public static string HelpLink { get { return _helpLink; } }
    }
}
