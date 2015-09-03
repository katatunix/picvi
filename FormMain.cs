using System;
using System.Drawing;
using System.Windows.Forms;

namespace picvi
{
	public partial class FormMain : Form, Observer
	{
		private String m_path;
		private MyPictureBox m_pic;
		private String m_curPicTitle = "";

		public FormMain(String path)
		{
			m_path = path;
			InitializeComponent();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			Size border = this.SizeFromClientSize(new Size(0, 0));
			Rectangle wa = Screen.PrimaryScreen.WorkingArea;
			
			Size maxInitPicSize = new Size(wa.Width - border.Width, wa.Height - border.Height);
			String configFielPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/picvi.config";
			m_pic = new MyPictureBox(m_path, new Config(configFielPath), maxInitPicSize, this);

			panel.Controls.Add(m_pic);
			m_pic.loadImage();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			bool handled = false;

			switch (keyData)
			{
				case Keys.Escape:
					this.Close();
					handled = true;
					break;

				case Keys.PageDown:
					m_pic.zoomIn();
					handled = true;
					break;
				case Keys.PageUp:
					m_pic.zoomOut();
					handled = true;
					break;
				case Keys.Home:
					m_pic.zoomReset();
					handled = true;
					break;

				case Keys.A:
					m_pic.moveLeft();
					handled = true;
					break;
				case Keys.D:
					m_pic.moveRight();
					handled = true;
					break;
				case Keys.Up: case Keys.W:
					m_pic.moveUp();
					handled = true;
					break;
				case Keys.Down: case Keys.S:
					m_pic.moveDown();
					handled = true;
					break;

				case Keys.Right:
					m_pic.next();
					handled = true;
					break;
				case Keys.Left:
					m_pic.prev();
					handled = true;
					break;

				case Keys.M:
					m_pic.toogleMode();
					handled = true;
					break;
				
				case Keys.H:
					showHelp();
					handled = true;
					break;
			}

			if (handled) showTitle();

			return handled;
		}

		private void panel_SizeChanged(object sender, EventArgs e)
		{
			centralizePictureBox();
		}

		public void onPicSizeChanged()
		{
			SetClientSizeCore(m_pic.Width, m_pic.Height); // this will raise panel_SizeChanged() also???? NO
			
			centralizePictureBox();
			centralizeWindow();
		}

		private void centralizePictureBox()
		{
			int x = (panel.Width - m_pic.Width) / 2;
			if (x < 0) x = 0;
			int y = (panel.Height - m_pic.Height) / 2;
			if (y < 0) y = 0;

			m_pic.Location = new Point(x, y);
		}

		private void centralizeWindow()
		{
			Rectangle wa = Screen.PrimaryScreen.WorkingArea;
			if (this.Width > wa.Width) this.Width = wa.Width;
			if (this.Height > wa.Height) this.Height = wa.Height;

			int y = (wa.Height - this.Size.Height) / 2;
			if (y < 0) y = 0;
			int x = (wa.Width - this.Size.Width) / 2;
			if (x < 0) x = 0;
			this.Location = new Point(wa.X + x, wa.Y + y);
		}

		private void showHelp()
		{
			String content =
				"Zoom\t\tPageUp PageDown Home\n"
				+ "Position\t\tW A S D Up Down\n"
				+ "Next/Prev\t\tLeft Right\n"
				+ "Mode\t\tM";
			MessageBox.Show(this, content, "picvi (c) 2015 katatunix@gmail.com", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void onNewImage(String title)
		{
			m_curPicTitle = title;
			showTitle();
		}

		private void showTitle()
		{
			this.Text = String.Format("picvi | {0} | {1} % | Press H for help | {2}",
				m_pic.modeName(),
				(m_pic.getRatio() * 100).ToString("0.00"),
				m_curPicTitle);
		}

	}
}
