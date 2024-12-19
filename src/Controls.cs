using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BeHappy
{
	/// <summary>
	/// GroupBox with LinkLabel on top
	/// </summary>
	public class GroupBoxLinkLabel : GroupBox
	{
		private LinkLabel linkLabel;

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LinkBehavior LinkBehavior {
			get { return linkLabel.LinkBehavior;}
			set { linkLabel.LinkBehavior = value;}
		}
		private ContextMenuStrip contextMenu;

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public GroupBoxLinkLabel()
		{
			linkLabel = new LinkLabel();
			linkLabel.LinkClicked += LinkClicked;
			linkLabel.Left = 8;
			linkLabel.AutoSize = true;
			linkLabel.MouseEnter += LinkLabelMouseEnter;
			linkLabel.MouseLeave += LinkLabelMouseLeave;
			this.Controls.Add(linkLabel);
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public override string Text {
			get { return linkLabel.Text; }
			set { linkLabel.Text = value; }
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShowContext();
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void ShowContext()
		{
			if (contextMenu != null)
				contextMenu.Show(linkLabel, 0, 16);
		}
		
		public void AddContextMenuStrip(ContextMenuStrip strip)
		{
			contextMenu = strip;
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        void LinkLabelMouseEnter(object sender, EventArgs e)
		{
			LinkLabel lb = (LinkLabel)sender;
			lb.LinkColor = lb.ActiveLinkColor;
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        void LinkLabelMouseLeave(object sender, EventArgs e)
		{
			LinkLabel lb = (LinkLabel)sender;
			lb.LinkColor = Color.Blue;
		}
	}
	
	
	/// <summary>
	/// Button with integrated CheckBox
	/// </summary>
	public class CheckBoxButton :Button
	{
		private CheckBox checkBox;

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool Checked {
			get {return checkBox.Checked;}
			set {checkBox.Checked = value;}
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public  string CheckBoxText {
			get {return checkBox.Text;}
			set {checkBox.Text = value;}
		}

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public CheckBoxButton()
		{
			this.MinimumSize = new Size(80, 30);
			this.TextAlign = ContentAlignment.BottomCenter;
			checkBox = new CheckBox();
			checkBox.AutoSize = false;
			checkBox.Text = "checkBox";
			checkBox.Font = new Font(this.Font.Name, 8, FontStyle.Regular);
			checkBox.UseVisualStyleBackColor = true;
			checkBox.BackColor = Color.Transparent;
			checkBox.Location = new Point(3, 3);
			checkBox.Height = 17;
			checkBox.Width = this.Width - 6;
			checkBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.Controls.Add(checkBox);
		}
	}
}

