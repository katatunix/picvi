﻿using System;
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
		private Size m_maxInitSize;
		private Config m_config;
		private double m_ratio;
		private double m_actualRatio;

		private const double ZOOM_DEFAULT = 1.0;

		public MyPictureBox(String path, Config config, Size maxInitSize, Observer observer)
		{
			this.SizeMode = PictureBoxSizeMode.StretchImage;
			this.Image = null;
			m_ratio = ZOOM_DEFAULT;
			m_actualRatio = m_ratio;
			m_maxInitSize = maxInitSize;
			m_config = config;
			m_observer = observer;

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
				this.Image = loadImageWithCorrectOrientation(path);
			}
			catch (Exception)
			{
				throw new Exception("Could not load the file " + path);
			}

			bool isMangeMode = m_config.isMangaMode();
			if (!isMangeMode)
			{
				m_ratio = ZOOM_DEFAULT;
			}
			m_actualRatio = m_ratio;

			bool overWidth = (int)(this.Image.Width * m_actualRatio) > m_maxInitSize.Width;
			bool overSize = isMangeMode ?
				overWidth :
				overWidth || (int)(this.Image.Height * m_actualRatio) > m_maxInitSize.Height;
			if (overSize)
			{
				m_actualRatio = (double)m_maxInitSize.Width / (double)this.Image.Width;
				if (!isMangeMode)
				{
					m_actualRatio = Math.Min(m_actualRatio, (double)m_maxInitSize.Height / (double)this.Image.Height);
				}
			}

			m_observer.onNewImage(Path.GetFileName(path) + '/' + m_files.Count);
			zoom();
		}

		private Image loadImageWithCorrectOrientation(string path)
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
			return img;
		}

		public void toogleMode()
		{
			m_config.toggleMode();
		}

		private const double ZOOM_STEP = 0.05;
		public void zoomIn()
		{
			m_actualRatio += ZOOM_STEP;
			m_ratio = m_actualRatio;
			zoom();
		}
		public void zoomOut()
		{
			m_actualRatio -= ZOOM_STEP;
			m_ratio = m_actualRatio;
			zoom();
		}
		public void zoomReset()
		{
			m_actualRatio = ZOOM_DEFAULT;
			m_ratio = m_actualRatio;
			zoom();
		}
		private void zoom()
		{
			this.Size = new Size((int)(this.Image.Width * m_actualRatio), (int)(this.Image.Height * m_actualRatio));
			m_observer.onPicSizeChanged();
		}

		public double getActualRatio()
		{
			return m_actualRatio;
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

		public string modeName()
		{
			return m_config.isMangaMode() ? "manga" : "pic";
		}
	}
}
