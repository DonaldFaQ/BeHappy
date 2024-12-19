using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BeHappy.DSP.ConfigurationForms
{
	public partial class ConfigureFormForNormalizeDSP : BeHappy.ConfigurationFormBase
	{
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public ConfigureFormForNormalizeDSP()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
            Utils.ChangeFontRecursive(new Control[] { this },
				Utils.HasMono ? new Font(SystemFonts.MessageBoxFont.Name, 8) : SystemFonts.MessageBoxFont);
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public int value {
			get {return trackBar1.Value;}
			set {trackBar1.Value = value;}
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void trackBar1_ValueChanged(object sender, System.EventArgs e)
		{
			label1.Text = string.Format("Normalize to {0}%", trackBar1.Value);
		}
	}
}
