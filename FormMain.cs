using System;
using System.Drawing;
using System.Windows.Forms;

namespace picvi
{
	public partial class FormMain : Form, Observer
	{
		private String m_path;
		private MyPictureBox m_pic;
		private String[] m_modes = { "manga", "pic" };
		private int m_curMode = 0;
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
			m_pic = new MyPictureBox(m_path, maxInitPicSize, this, this.pic_Resize);
			panel.Controls.Add(m_pic);

			m_pic.loadImage();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Escape:
					this.Close();
					return true;

				case Keys.PageDown:
					m_pic.zoomIn();
					return true;
				case Keys.PageUp:
					m_pic.zoomOut();
					return true;
				case Keys.Home:
					m_pic.zoomReset();
					return true;

				case Keys.A:
					m_pic.moveLeft();
					return true;
				case Keys.D:
					m_pic.moveRight();
					return true;
				case Keys.Up: case Keys.W:
					m_pic.moveUp();
					return true;
				case Keys.Down: case Keys.S:
					m_pic.moveDown();
					return true;

				case Keys.Right:
					m_pic.next();
					return true;
				case Keys.Left:
					m_pic.prev();
					return true;

				case Keys.M:
					m_pic.toogleMode();
					m_curMode = (m_curMode + 1) % m_modes.Length;
					showTitle();
					return true;
				
				case Keys.H:
					showHelp();
					return true;
			}

			return false;
		}

		private void panel_Resize(object sender, EventArgs e)
		{
			centralizePictureBox();
		}

		private void pic_Resize(object sender, EventArgs e)
		{
			SetClientSizeCore(m_pic.Width, m_pic.Height);
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
			String content = "Zoom: PageUp, PageDown, Home\n"
				+ "Position: W A S D Up Down\n"
				+ "Next/Prev: Left Right";
			MessageBox.Show(this, content, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void onNew(String title)
		{
			m_curPicTitle = title;
			showTitle();
		}

		private void showTitle()
		{
			this.Text = String.Format("picvi | {0} | {1} % | Press H for help | {2}",
				m_modes[m_curMode],
				(m_pic.getRatio() * 100).ToString("0.00"),
				m_curPicTitle);
		}

	}
}
