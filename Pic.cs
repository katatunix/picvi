using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace picvi
{
	class Pic
	{
		private String m_path;
		private Image m_image;
		private List<String> m_files;
		private Observer m_observer;

		public Pic(String path, Observer observer)
		{
			m_path = path;
			m_observer = observer;
			m_image = null;

			loadFiles();
			loadImage();
		}

		private void loadFiles()
		{
			m_files = new List<string>();
			String allowesExts = ".png|.jpg|.jpeg|.gif|.tga";
			foreach (String path in Directory.GetFiles(Path.GetDirectoryName(m_path)))
			{
				String ext = Path.GetExtension(path);
				if (ext.Length > 0 && allowesExts.Contains(ext))
				{
					m_files.Add(path);
				}
			}
		}

		public void next()
		{
			navi(true);
		}

		public void prev()
		{
			navi(false);
		}

		private void navi(bool isNext)
		{
			int i = 0;
			while (i < m_files.Count && m_files[i] != m_path) i++;
			if (i < m_files.Count)
			{
				if (isNext) naviNext(i); else naviPrev(i);
				loadImage();
			}
		}

		private void naviPrev(int i)
		{
			m_path = i == 0 ? m_files[m_files.Count - 1] : m_files[i - 1];
		}

		private void naviNext(int i)
		{
			m_path = i == m_files.Count - 1 ? m_files[0] : m_files[i + 1];
		}

		private void loadImage()
		{
			if (m_image != null)
			{
				m_image.Dispose();
				m_image = null;
			}
			m_image = Image.FromFile(m_path);
			m_observer.onNew(m_image, Path.GetFileName(m_path) + '/' + m_files.Count);
		}
	}
}
