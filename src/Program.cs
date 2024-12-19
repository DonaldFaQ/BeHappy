using System;
using System.Windows.Forms;

namespace BeHappy
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
			Application.Run(new MainForm());
		}
	}
}


