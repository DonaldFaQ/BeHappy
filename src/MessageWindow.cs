
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BeHappy
{
	/// <summary>
	/// Description of MessageWindow.
	/// </summary>
	public partial class MessageWindow : Form
	{
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public MessageWindow()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			contextMenuStrip1.Items.Add("Copy", null, (sender, e) => richTextBox1.Copy());
			richTextBox1.ContextMenuStrip = contextMenuStrip1;
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void AddText(string text)
		{
			richTextBox1.Text += text;
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
			richTextBox1.ScrollToCaret();
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public void ClearText()
		{
			richTextBox1.Clear();
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        void BtnCloseClick(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
