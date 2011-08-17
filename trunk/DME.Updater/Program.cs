using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DME.Updater
{
	static class Program
	{
		public static bool IsRunning = false;

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			IsRunning = true;

			if (args.Length == 0)
			{
				DME.Updater.Wrapper.FunctionalForm.Information(DME.Updater.SR.DonotRunMeDirectly);
				return;
			}
			else if (args[0] == "selfupdate")
			{
				new Dialogs.SelfUpdate().ShowDialog();
				return;
			}

			//升级需要的参数
			// 主程序版本 主程序目录 升级信息路径 [*主程序ID|主程序进程名]
			Application.Run(new MainWindow());
		}
	}
}
