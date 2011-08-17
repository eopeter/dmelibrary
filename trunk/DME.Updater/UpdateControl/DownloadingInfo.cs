using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DME.Updater.UpdateControl
{
	public partial class DownloadingInfo : DME.Updater.UpdateControl.ControlBase
	{
		public DownloadingInfo()
		{
			InitializeComponent();

			if (Program.IsRunning)
			{
				Updater.Instance.DownloadUpdateInfo += Instance_DownloadUpdateInfo;
			}
		}

		void Instance_DownloadUpdateInfo(object sender, EventArgs e)
		{
			this.Show();
		}
	}
}
