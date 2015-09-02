using System;
using System.Drawing;
using System.Windows.Forms;

namespace picvi
{
	public partial class FormMain : Form, Observer
	{
		private String m_path;
		private Pic m_pic;
		private float m_zoom;
		private int m_picLeft, m_picTop;

		public FormMain(String path)
		{
			m_path = path;
			InitializeComponent();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			m_zoom = 1.0f;
			m_pic = new Pic(m_path, this);
		}

		private void adjustPictureBoxPosition()
		{
			pictureBox.Location = new Point(m_picLeft, m_picTop);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			const float ZOOM = 0.03f;
			const int MOVE = 40;

			switch (keyData)
			{
				case Keys.Escape:
					this.Close();
					return true;

				case Keys.PageDown:
					m_zoom += ZOOM;
					centralizeAll();
					return true;
				case Keys.PageUp:
					m_zoom -= ZOOM;
					centralizeAll();
					return true;
				case Keys.Home:
					m_zoom = 1.0f;
					centralizeAll();
					return true;

				case Keys.A:
					m_picLeft += MOVE;
					adjustPictureBoxPosition();
					return true;
				case Keys.D:
					m_picLeft -= MOVE;
					adjustPictureBoxPosition();
					return true;
				case Keys.Up: case Keys.W:
					m_picTop += MOVE;
					adjustPictureBoxPosition();
					return true;
				case Keys.Down: case Keys.S:
					m_picTop -= MOVE;
					adjustPictureBoxPosition();
					return true;

				case Keys.Right:
					m_pic.next();
					return true;
				case Keys.Left:
					m_pic.prev();
					return true;
				
				case Keys.H:
					showHelp();
					return true;
			}

			return false;
		}

		private void FormMain_Resize(object sender, EventArgs e)
		{
			// When window is resized, panel will be resized too
			centralizePictureBox();
		}

		public void onNew(Image image, String title)
		{
			pictureBox.Image = image;
			this.Text = "[picvi © 2015 katatunix@gmail.com] Press H for help [" + title + "]";

			centralizeAll();
		}

		private void centralizeAll()
		{
			zoomPictureBox();
			centralizePictureBox();
			SetClientSizeCore(pictureBox.Width, pictureBox.Height);
			centralizeWindow();
		}

		private void centralizePictureBox()
		{
			m_picLeft = (panel.Width - pictureBox.Width) / 2;
			if (m_picLeft < 0) m_picLeft = 0;
			m_picTop = (panel.Height - pictureBox.Height) / 2;
			if (m_picTop < 0) m_picTop = 0;

			pictureBox.Location = new Point(m_picLeft, m_picTop);
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

		private void zoomPictureBox()
		{
			pictureBox.Width = (int)(pictureBox.Image.Width * m_zoom);
			pictureBox.Height = (int)(pictureBox.Image.Height * m_zoom);
		}

		private void showHelp()
		{
			String content = "Zoom: PageUp, PageDown, Home\n"
				+ "Position: W A S D Up Down\n"
				+ "Next/Prev: Left Right";
			MessageBox.Show(this, content, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

	}
}
