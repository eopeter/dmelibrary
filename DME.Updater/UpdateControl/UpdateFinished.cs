using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DME.Updater.UpdateControl
{
	public partial class UpdateFinished : DME.Updater.UpdateControl.ControlBase
	{
		public UpdateFinished()
		{
			InitializeComponent();

			if (Program.IsRunning)
			{
				Updater.Instance.UpdateFinsihed += Instance_UpdateFinsihed;
			}
		}

		void Instance_UpdateFinsihed(object sender, EventArgs e)
		{
			this.HideControls();
			this.Show();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.FindForm().Close();
		}
	}
}
