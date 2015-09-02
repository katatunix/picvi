using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace picvi
{
	class MyPictureBox : PictureBox
	{
		private List<String> m_files;
		private int m_index;
		private Observer m_observer;
		private float m_ratio;
		private Size m_maxInitSize;
		private bool m_isMangaMode;

		public MyPictureBox(String path, Size maxInitSize, Observer observer, EventHandler resizeHandler)
		{
			this.SizeMode = PictureBoxSizeMode.StretchImage;
			this.Image = null;
			m_ratio = 1.0f;
			m_maxInitSize = maxInitSize;
			m_isMangaMode = true;
			m_observer = observer;
			this.Resize += new System.EventHandler(resizeHandler);

			loadFiles(path);
			findIndex(path);
		}

		private void loadFiles(String path)
		{
			m_files = new List<string>();
			String allowesExts = ".png|.jpg|.jpeg|.gif|.tga";
			foreach (String file in Directory.GetFiles(Path.GetDirectoryName(path)))
			{
				String ext = Path.GetExtension(file).ToLower();
				if (ext.Length > 0 && allowesExts.Contains(ext))
				{
					m_files.Add(file);
				}
			}
		}

		private void findIndex(String path)
		{
			int i = 0;
			while (i < m_files.Count && m_files[i] != path) i++;
			if (i >= m_files.Count)
			{
				throw new Exception("Not support the file " + path);
			}
			m_index = i;
		}

		public void loadImage()
		{
			if (this.Image != null)
			{
				this.Image.Dispose();
				this.Image = null;
			}
			String path = m_files[m_index];
			try
			{
				Image img = Image.FromFile(path);
				if (Array.IndexOf(img.PropertyIdList, 274) > -1)
				{
					var orientation = (int) img.GetPropertyItem(274).Value[0];
					switch (orientation)
					{
						case 1:
							// No rotation required.
							break;
						case 2:
							img.RotateFlip(RotateFlipType.RotateNoneFlipX);
							break;
						case 3:
							img.RotateFlip(RotateFlipType.Rotate180FlipNone);
							break;
						case 4:
							img.RotateFlip(RotateFlipType.Rotate180FlipX);
							break;
						case 5:
							img.RotateFlip(RotateFlipType.Rotate90FlipX);
							break;
						case 6:
							img.RotateFlip(RotateFlipType.Rotate90FlipNone);
							break;
						case 7:
							img.RotateFlip(RotateFlipType.Rotate270FlipX);
							break;
						case 8:
							img.RotateFlip(RotateFlipType.Rotate270FlipNone);
							break;
					}
					// This EXIF data is now invalid and should be removed.
					img.RemovePropertyItem(274);
				}
				this.Image = img;
			}
			catch (Exception)
			{
				throw new Exception("Could not load the file " + path);
			}

			this.Left = this.Top = 0;
			if (!m_isMangaMode)
			{
				m_ratio = 1.0f;
			}

			bool overWidth = (int)(this.Image.Width * m_ratio) > m_maxInitSize.Width;
			bool overSize = m_isMangaMode ?
				overWidth :
				overWidth || (int)(this.Image.Height * m_ratio) > m_maxInitSize.Height;
			if (overSize)
			{
				m_ratio = 1.0f * m_maxInitSize.Width / this.Image.Width;
				if (!m_isMangaMode)
				{
					m_ratio = Math.Min(m_ratio, 1.0f * m_maxInitSize.Height / this.Image.Height);
				}
			}

			m_observer.onNew(Path.GetFileName(path) + '/' + m_files.Count);
			zoom();
		}

		public void toogleMode()
		{
			m_isMangaMode = !m_isMangaMode;
		}

		private const float ZOOM_STEP = 0.05f;
		public void zoomIn()
		{
			m_ratio += ZOOM_STEP;
			zoom();
		}
		public void zoomOut()
		{
			m_ratio -= ZOOM_STEP;
			zoom();
		}
		public void zoomReset()
		{
			m_ratio = 1.0f;
			zoom();
		}
		private void zoom()
		{
			this.Size = new Size((int)(this.Image.Width * m_ratio), (int)(this.Image.Height * m_ratio));
		}

		public float getRatio()
		{
			return m_ratio;
		}

		private const int MOVE_STEP = 40;
		public void moveLeft()
		{
			this.Left += MOVE_STEP;
		}

		public void moveRight()
		{
			this.Left -= MOVE_STEP;
		}

		public void moveUp()
		{
			this.Top += MOVE_STEP;
		}

		public void moveDown()
		{
			this.Top -= MOVE_STEP;
		}

		public void next()
		{
			m_index++;
			if (m_index >= m_files.Count) m_index = 0;
			loadImage();
		}

		public void prev()
		{
			m_index--;
			if (m_index < 0) m_index = m_files.Count - 1;
			loadImage();
		}
	}
}
