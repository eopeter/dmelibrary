using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using FSLib.IO.SerializeHelper;

namespace DME.Updater.Generator
{
    public partial class Main : FSLib.Windows.Forms.FunctionalForm
	{
		public Main()
		{
			InitializeComponent();
			InitWorker();
			InitDropSupport();

			this.btnOpen.Click += btnOpen_Click;
		}

		#region 拖放支持

		void InitDropSupport()
		{
			this.AllowDrop = true;
			this.txtNewSoftDir.AllowDrop = true;

			//自身
			this.DragEnter += (s, e) =>
			{
				System.Collections.Specialized.StringCollection files;
				DataObject doe = e.Data as DataObject;
				if (
					!doe.ContainsFileDropList()
					||
					(files = doe.GetFileDropList()).Count == 0
					||
					!System.IO.File.Exists(files[0])
					||
					!files[0].EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
					) return;

				e.Effect = DragDropEffects.Link;
			};
			this.DragDrop += (s, e) =>
			{
				var file = (e.Data as DataObject).GetFileDropList()[0];
				OpenXML(file);
			};
			//升级包
			this.txtNewSoftDir.DragEnter += (s, e) =>
			{
				System.Collections.Specialized.StringCollection files;
				DataObject doe = e.Data as DataObject;
				if (
					!doe.ContainsFileDropList()
					||
					(files = doe.GetFileDropList()).Count == 0
					||
					!System.IO.Directory.Exists(files[0])
					) return;

				e.Effect = DragDropEffects.Link;
			};
			this.txtNewSoftDir.DragDrop += (s, e) =>
			{
				this.SelectedNewSoftDirPath = (e.Data as DataObject).GetFileDropList()[0];
			};
		}

		#endregion

		#region 界面响应函数

		/// <summary>
		/// 打开
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void btnOpen_Click(object sender, EventArgs e)
		{
			OpenFileDialog open = new OpenFileDialog()
			{
				Title = "打开升级信息文件",
				Filter = "XML信息文件(*.xml)|*.xml"
			};
			if (open.ShowDialog() != DialogResult.OK) return;
			OpenXML(open.FileName);
		}

		/// <summary>
		/// 打开配置文件
		/// </summary>
		/// <param name="path"></param>
		void OpenXML(string path)
		{

			var ui = typeof(UpdateInfo).XmlDeserializeFile(path) as UpdateInfo;
			if (ui == null) Information("无法加载信息文件，请确认选择正确的文件");
			else
			{
				this.txtAfterExecuteArgs.Text = ui.ExecuteArgumentAfter;
				this.txtAppName.Text = ui.AppName;
				this.txtAppVersion.Text = ui.AppVersion;
				this.txtDesc.Text = ui.Desc;
				this.txtPreExecuteArgs.Text = ui.ExecuteArgumentBefore;
				this.txtPublishUrl.Text = ui.PublishUrl;
				this.txtTimeout.Text = ui.ExecuteTimeout.ToString();
				this.SelectedPackagePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), ui.Package);

				this.options.UpdateInterface(ui);
			}
		}


		private void btnCreate_Click(object sender, EventArgs e)
		{
			epp.Clear();

			if (string.IsNullOrEmpty(this.txtAppName.Text)) { epp.SetError(this.txtAppName, "请输入应用程序名"); return; }
			try
			{
				new Version(this.txtAppVersion.Text);
			}
			catch (Exception)
			{
				epp.SetError(this.txtAppVersion, "请输入版本号");
				return;
			}
			if (!System.IO.Directory.Exists(this.SelectedNewSoftDirPath)) { epp.SetError(this.txtNewSoftDir, "请选择新程序的目录"); return; }
			if (string.IsNullOrEmpty(this.SelectedPackagePath)) { epp.SetError(this.txtPackagePath, "请选择打包后的组件和升级信息文件所在路径"); return; }
			if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(this.SelectedPackagePath))) { epp.SetError(this.txtPackagePath, "文件包所在目录不存在"); return; }
			if (System.IO.File.Exists(this.SelectedPackagePath))
			{
				System.IO.File.Delete(this.SelectedPackagePath);
			}
			if (System.IO.File.Exists(GetXmlPath(this.SelectedPackagePath)))
			{
				System.IO.File.Delete(GetXmlPath(this.SelectedPackagePath));
			}

			Create();
		}


		#endregion

		/// <summary>
		/// 获得对应升级包的升级信息文件路径
		/// </summary>
		/// <returns></returns>
		string GetXmlPath(string pkgPath)
		{
			return System.IO.Path.ChangeExtension(pkgPath, ".xml");
		}

		#region 主要创建流程

		/// <summary>
		/// 初始化线程类
		/// </summary>
		void InitWorker()
		{
			bgw.WorkerProgressChanged += (s, e) =>
			{
				lblStatus.Text = e.Progress.StateMessage;
				this.pbProgress.Value = e.Progress.TaskPercentage;
			};
			bgw.WorkCompleted += (s, e) =>
			{
				btnCreate.Enabled = true;
				this.pbProgress.Visible = false;
				Information("已经成功创建");
			};
			bgw.WorkFailed += (s, e) =>
			{
				btnCreate.Enabled = true;
				this.pbProgress.Visible = false;
				Information("出现错误：" + e.Exception.Message);
			};
			bgw.DoWork += CreatePackage;
			this.FormClosing += (s, e) =>
			{
				e.Cancel = !this.btnCreate.Enabled;
			};
		}

		//创建信息的具体操作函数
		void CreatePackage(object sender, FSLib.Threading.RunworkEventArgs e)
		{
			UpdateInfo info = new UpdateInfo()
			{
				AppName = this.txtAppName.Text,
				AppVersion = this.txtAppVersion.Text,
				Desc = this.txtDesc.Text,
				ExecuteArgumentAfter = this.txtAfterExecuteArgs.Text,
				ExecuteArgumentBefore = this.txtPreExecuteArgs.Text,
				PublishUrl = this.txtPublishUrl.Text,
				FileExecuteAfter = this.fileAfterExecute.SelectedFileName,
				FileExecuteBefore = this.filePreExecute.SelectedFileName,
				MD5 = "",
				Package = System.IO.Path.GetFileName(this.txtPackagePath.Text),
				ExecuteTimeout = txtTimeout.Text.ToInt32(),
				PackageSize = 0,
				RequiredMinVersion = ""
			};
			this.options.SaveSetting(info);

            DME.Zip.Zip.FastZipEvents evt = new DME.Zip.Zip.FastZipEvents();
			evt.ProcessFile += (s, f) =>
			{
				e.ReportProgress(0, 0, "正在压缩文件 " + System.IO.Path.GetFileName(f.Name));
			};
            DME.Zip.Zip.FastZip zip = new DME.Zip.Zip.FastZip(evt);
			if (!info.PackagePassword.IsNullOrEmpty()) zip.Password = info.PackagePassword;
			zip.CreateZip(this.txtPackagePath.Text, this.txtNewSoftDir.Text, true, "");

			//校验MD5
			byte[] hash = null;
			var size = 0;
			using (var fs = new FSLib.IO.ExtendFileStream(SelectedPackagePath, System.IO.FileMode.Open))
			{
				e.ReportProgress((int)fs.Length, 0, "");
				fs.ProgressChanged += (s, f) =>
				{
					e.ReportProgress((int)fs.Position);
				};
				MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

				hash = md5.ComputeHash(fs);
				size = (int)fs.Length;
			}
			info.MD5 = BitConverter.ToString(hash).Replace("-", "").ToUpper();
			info.PackageSize = size;
			info.XmlSerilizeToFile(GetXmlPath(SelectedPackagePath));

			e.ReportProgress(0, 0, "生成成功，MD5校验：" + info.MD5);
		}

		void Create()
		{
			this.btnCreate.Enabled = false;
			this.pbProgress.Visible = true;
			bgw.RunWorkASync();
		}

		FSLib.Threading.BackgroundWorker bgw = new FSLib.Threading.BackgroundWorker()
		{
			WorkerSupportReportProgress = true
		};

		#endregion

		#region 界面属性

		/// <summary>
		/// 获得或设置文件包路径
		/// </summary>
		public string SelectedPackagePath
		{
			get { return this.txtPackagePath.Text; }
			set { this.txtPackagePath.Text = value; }
		}

		/// <summary>
		/// 获得或设置选定的新软件目录
		/// </summary>
		public string SelectedNewSoftDirPath
		{
			get { return this.txtNewSoftDir.Text; }
			set
			{
				this.txtNewSoftDir.Text = value;
				Environment.CurrentDirectory = value;
				filePreExecute.RootPath = fileAfterExecute.RootPath = value;
			}
		}

		#endregion

		#region 界面响应函数
		private void btnBrowseFolder_Click(object sender, EventArgs e)
		{
			if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

			SelectedNewSoftDirPath = fbd.SelectedPath;
		}

		private void browseFile_Click(object sender, EventArgs e)
		{
			if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

			SelectedPackagePath = sfd.FileName;
		}

		private void txtNewSoftDir_TextChanged(object sender, EventArgs e)
		{
			if (System.IO.Directory.Exists(txtNewSoftDir.Text))
			{
				filePreExecute.RootPath = fileAfterExecute.RootPath = this.SelectedNewSoftDirPath;
			}
		}

		private void txtPackagePath_TextChanged(object sender, EventArgs e)
		{

		}
		#endregion
	}
}
