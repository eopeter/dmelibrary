namespace DME.Updater.Generator
{
	partial class Main
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            FSLib.Windows.Components.CueInfo cueInfo1 = new FSLib.Windows.Components.CueInfo(true);
            FSLib.Windows.Components.CueInfo cueInfo2 = new FSLib.Windows.Components.CueInfo(true);
            FSLib.Windows.Components.CueInfo cueInfo3 = new FSLib.Windows.Components.CueInfo(true);
            FSLib.Windows.Components.CueInfo cueInfo4 = new FSLib.Windows.Components.CueInfo(true);
            FSLib.Windows.Components.CueInfo cueInfo5 = new FSLib.Windows.Components.CueInfo(false);
            FSLib.Windows.Components.CueInfo cueInfo6 = new FSLib.Windows.Components.CueInfo(true);
            FSLib.Windows.Components.CueInfo cueInfo7 = new FSLib.Windows.Components.CueInfo(true);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAppName = new System.Windows.Forms.TextBox();
            this.txtPublishUrl = new System.Windows.Forms.TextBox();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnCreate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtAfterExecuteArgs = new System.Windows.Forms.TextBox();
            this.txtPreExecuteArgs = new System.Windows.Forms.TextBox();
            this.txtAppVersion = new System.Windows.Forms.TextBox();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.fileAfterExecute = new FSLib.Windows.Controls.FileComboBox();
            this.filePreExecute = new FSLib.Windows.Controls.FileComboBox();
            this.gradientBanner1 = new FSLib.Windows.Controls.GradientBanner();
            this.cueProvider1 = new FSLib.Windows.Components.CueProvider();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.txtNewSoftDir = new System.Windows.Forms.TextBox();
            this.txtPackagePath = new System.Windows.Forms.TextBox();
            this.epp = new System.Windows.Forms.ErrorProvider(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.browseFile = new FSLib.Windows.Controls.ButtonExtend();
            this.btnBrowseFolder = new FSLib.Windows.Controls.ButtonExtend();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.options = new DME.Updater.Generator.Controls.OptionTab();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.epp)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtAppName
            // 
            this.cueProvider1.SetCue(this.txtAppName, null);
            resources.ApplyResources(this.txtAppName, "txtAppName");
            this.txtAppName.Name = "txtAppName";
            // 
            // txtPublishUrl
            // 
            resources.ApplyResources(cueInfo1, "cueInfo1");
            this.cueProvider1.SetCue(this.txtPublishUrl, cueInfo1);
            resources.ApplyResources(this.txtPublishUrl, "txtPublishUrl");
            this.txtPublishUrl.Name = "txtPublishUrl";
            // 
            // pbProgress
            // 
            resources.ApplyResources(this.pbProgress, "pbProgress");
            this.pbProgress.Name = "pbProgress";
            // 
            // btnCreate
            // 
            resources.ApplyResources(this.btnCreate, "btnCreate");
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // txtAfterExecuteArgs
            // 
            resources.ApplyResources(cueInfo2, "cueInfo2");
            this.cueProvider1.SetCue(this.txtAfterExecuteArgs, cueInfo2);
            resources.ApplyResources(this.txtAfterExecuteArgs, "txtAfterExecuteArgs");
            this.txtAfterExecuteArgs.Name = "txtAfterExecuteArgs";
            // 
            // txtPreExecuteArgs
            // 
            resources.ApplyResources(cueInfo3, "cueInfo3");
            this.cueProvider1.SetCue(this.txtPreExecuteArgs, cueInfo3);
            resources.ApplyResources(this.txtPreExecuteArgs, "txtPreExecuteArgs");
            this.txtPreExecuteArgs.Name = "txtPreExecuteArgs";
            // 
            // txtAppVersion
            // 
            resources.ApplyResources(cueInfo4, "cueInfo4");
            this.cueProvider1.SetCue(this.txtAppVersion, cueInfo4);
            resources.ApplyResources(this.txtAppVersion, "txtAppVersion");
            this.txtAppVersion.Name = "txtAppVersion";
            // 
            // txtDesc
            // 
            cueInfo5.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            cueInfo5.Padding = new System.Windows.Forms.Padding(0);
            resources.ApplyResources(cueInfo5, "cueInfo5");
            cueInfo5.TextImageAlign = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            cueInfo5.WaterMarkColor = System.Drawing.SystemColors.GrayText;
            cueInfo5.WaterMarkFont = null;
            cueInfo5.WaterMarkImage = null;
            cueInfo5.WaterMarkImageSize = new System.Drawing.Size(0, 0);
            this.cueProvider1.SetCue(this.txtDesc, cueInfo5);
            resources.ApplyResources(this.txtDesc, "txtDesc");
            this.txtDesc.Name = "txtDesc";
            // 
            // fileAfterExecute
            // 
            this.fileAfterExecute.AllowDrop = true;
            this.cueProvider1.SetCue(this.fileAfterExecute, null);
            this.fileAfterExecute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileAfterExecute.EnableDropDown = false;
            this.fileAfterExecute.FileTypeFilter = "cmd,exe,bat,com";
            this.fileAfterExecute.FormattingEnabled = true;
            this.fileAfterExecute.Items.AddRange(new object[] {
            resources.GetString("fileAfterExecute.Items")});
            resources.ApplyResources(this.fileAfterExecute, "fileAfterExecute");
            this.fileAfterExecute.Name = "fileAfterExecute";
            this.fileAfterExecute.RootPath = null;
            this.fileAfterExecute.SelectedFileName = "";
            this.fileAfterExecute.ShowEmptyEntry = true;
            // 
            // filePreExecute
            // 
            this.filePreExecute.AllowDrop = true;
            this.cueProvider1.SetCue(this.filePreExecute, null);
            this.filePreExecute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filePreExecute.EnableDropDown = false;
            this.filePreExecute.FileTypeFilter = "cmd,exe,bat,com";
            this.filePreExecute.FormattingEnabled = true;
            this.filePreExecute.Items.AddRange(new object[] {
            resources.GetString("filePreExecute.Items")});
            resources.ApplyResources(this.filePreExecute, "filePreExecute");
            this.filePreExecute.Name = "filePreExecute";
            this.filePreExecute.RootPath = null;
            this.filePreExecute.SelectedFileName = "";
            this.filePreExecute.ShowEmptyEntry = true;
            // 
            // gradientBanner1
            // 
            this.gradientBanner1.BottomBorderColor = System.Drawing.SystemColors.ControlDark;
            resources.ApplyResources(this.gradientBanner1, "gradientBanner1");
            this.gradientBanner1.BriefTextColor = System.Drawing.Color.White;
            this.gradientBanner1.DisplayPadding = 5F;
            this.gradientBanner1.EndColor = System.Drawing.Color.White;
            this.gradientBanner1.HeadImage = null;
            this.gradientBanner1.Name = "gradientBanner1";
            this.gradientBanner1.StartColor = System.Drawing.Color.RoyalBlue;
            this.gradientBanner1.TextAlign = FSLib.Windows.Controls.GradientBanner.TextAlignType.MiddleLeft;
            this.gradientBanner1.TextColor = System.Drawing.Color.White;
            this.gradientBanner1.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // txtTimeout
            // 
            this.cueProvider1.SetCue(this.txtTimeout, null);
            resources.ApplyResources(this.txtTimeout, "txtTimeout");
            this.txtTimeout.Name = "txtTimeout";
            // 
            // txtNewSoftDir
            // 
            resources.ApplyResources(cueInfo6, "cueInfo6");
            this.cueProvider1.SetCue(this.txtNewSoftDir, cueInfo6);
            resources.ApplyResources(this.txtNewSoftDir, "txtNewSoftDir");
            this.txtNewSoftDir.Name = "txtNewSoftDir";
            this.txtNewSoftDir.TextChanged += new System.EventHandler(this.txtNewSoftDir_TextChanged);
            // 
            // txtPackagePath
            // 
            resources.ApplyResources(cueInfo7, "cueInfo7");
            this.cueProvider1.SetCue(this.txtPackagePath, cueInfo7);
            resources.ApplyResources(this.txtPackagePath, "txtPackagePath");
            this.txtPackagePath.Name = "txtPackagePath";
            this.txtPackagePath.TextChanged += new System.EventHandler(this.txtPackagePath_TextChanged);
            // 
            // epp
            // 
            this.epp.ContainerControl = this;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // btnOpen
            // 
            resources.ApplyResources(this.btnOpen, "btnOpen");
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.browseFile);
            this.tabPage1.Controls.Add(this.btnBrowseFolder);
            this.tabPage1.Controls.Add(this.txtPackagePath);
            this.tabPage1.Controls.Add(this.txtNewSoftDir);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtTimeout);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.txtAfterExecuteArgs);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.txtPreExecuteArgs);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.fileAfterExecute);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.filePreExecute);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.txtAppName);
            this.tabPage1.Controls.Add(this.txtPublishUrl);
            this.tabPage1.Controls.Add(this.txtDesc);
            this.tabPage1.Controls.Add(this.txtAppVersion);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // browseFile
            // 
            this.browseFile.ButtonImage = FSLib.Windows.ImageType.Save;
            resources.ApplyResources(this.browseFile, "browseFile");
            this.browseFile.Name = "browseFile";
            this.browseFile.ShowShield = false;
            this.browseFile.UseVisualStyleBackColor = true;
            this.browseFile.Click += new System.EventHandler(this.browseFile_Click);
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.ButtonImage = FSLib.Windows.ImageType.OpenedFolder;
            resources.ApplyResources(this.btnBrowseFolder, "btnBrowseFolder");
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.ShowShield = false;
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.options);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // options
            // 
            resources.ApplyResources(this.options, "options");
            this.options.Name = "options";
            // 
            // fbd
            // 
            resources.ApplyResources(this.fbd, "fbd");
            // 
            // sfd
            // 
            this.sfd.DefaultExt = "zip";
            this.sfd.FileName = "update.zip";
            resources.ApplyResources(this.sfd, "sfd");
            // 
            // Main
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.gradientBanner1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            ((System.ComponentModel.ISupportInitialize)(this.epp)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private FSLib.Windows.Controls.GradientBanner gradientBanner1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private FSLib.Windows.Components.CueProvider cueProvider1;
		private System.Windows.Forms.TextBox txtAppName;
		private System.Windows.Forms.TextBox txtAppVersion;
		private System.Windows.Forms.TextBox txtPublishUrl;
		private System.Windows.Forms.TextBox txtDesc;
		private System.Windows.Forms.ProgressBar pbProgress;
		private System.Windows.Forms.Button btnCreate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private FSLib.Windows.Controls.FileComboBox filePreExecute;
		private FSLib.Windows.Controls.FileComboBox fileAfterExecute;
		private System.Windows.Forms.TextBox txtPreExecuteArgs;
		private System.Windows.Forms.TextBox txtAfterExecuteArgs;
		private System.Windows.Forms.ErrorProvider epp;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtTimeout;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnOpen;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private Controls.OptionTab options;
		private FSLib.Windows.Controls.ButtonExtend browseFile;
		private FSLib.Windows.Controls.ButtonExtend btnBrowseFolder;
		private System.Windows.Forms.TextBox txtPackagePath;
		private System.Windows.Forms.TextBox txtNewSoftDir;
		private System.Windows.Forms.FolderBrowserDialog fbd;
		private System.Windows.Forms.SaveFileDialog sfd;
	}
}