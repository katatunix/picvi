using System;
using System.Drawing;
using System.Windows.Forms;

namespace picvi
{
	public partial class FormMain : Form
	{
		private String m_path;
		private Pic m_pic;
		private float m_zoom;
		private int m_left, m_top;

		public FormMain(String path)
		{
			m_path = path;
			InitializeComponent();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			m_zoom = 1.0f;
			m_pic = new Pic(m_path);
			applyNew();
		}

		private void applyNew()
		{
			applyZoom();
			Image image = m_pic.image();
			pictureBox.Image = image;
			this.Text = "[picvi © 2015 katatunix@gmail.com] Press H for help [" + m_pic.title() + "]";
		}

		private void applyZoom()
		{
			Image image = m_pic.image();
			int width = (int)(image.Width * m_zoom);
			int height = (int)(image.Height * m_zoom);

			m_left = (panel.Width - width) / 2;
			if (m_left < 0) m_left = 0;
			m_top = (panel.Height - height) / 2;
			if (m_top < 0) m_top = 0;

			pictureBox.Bounds = new Rectangle(m_left, m_top, width, height);
		}

		private void applyPosition()
		{
			pictureBox.Location = new Point(m_left, m_top);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			const float ZOOM = 0.02f;
			const int MOVE = 40;

			switch (keyData)
			{
				case Keys.Escape:
					this.Close();
					return true;

				case Keys.PageDown:
					m_zoom += ZOOM;
					applyZoom();
					return true;
				case Keys.PageUp:
					m_zoom -= ZOOM;
					applyZoom();
					return true;
				case Keys.Home:
					m_zoom = 1.0f;
					applyZoom();
					return true;

				case Keys.A:
					m_left += MOVE;
					applyPosition();
					return true;
				case Keys.D:
					m_left -= MOVE;
					applyPosition();
					return true;
				case Keys.Up: case Keys.W:
					m_top += MOVE;
					applyPosition();
					return true;
				case Keys.Down: case Keys.S:
					m_top -= MOVE;
					applyPosition();
					return true;

				case Keys.Right:
					m_pic.next();
					applyNew();
					return true;
				case Keys.Left:
					m_pic.prev();
					applyNew();
					return true;
				
				case Keys.H:
					showHelp();
					return true;
			}

			return false;
		}

		private void FormMain_Resize(object sender, EventArgs e)
		{
			applyZoom();
		}

		private void showHelp()
		{
			String content = "Zoom: PageUp, PageDown\n"
				+ "Position: W A S D Up Down\n"
				+ "Next/Prev: Left Right";
			MessageBox.Show(this, content, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
