using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace picvi
{
	class Config
	{
		private String m_configFilePath;
		private bool m_isMangaMode;

		public Config(String configFilePath)
		{
			m_configFilePath = configFilePath;

			try
			{
				using (StreamReader reader = new StreamReader(m_configFilePath))
				{
					m_isMangaMode = reader.ReadLine() == "1";
				}
			}
			catch (Exception)
			{
				m_isMangaMode = true;
			}
		}

		public bool isMangaMode()
		{
			return m_isMangaMode;
		}

		public void toggleMode()
		{
			m_isMangaMode = !m_isMangaMode;
			using (StreamWriter writer = new StreamWriter(m_configFilePath))
			{
				writer.WriteLine(m_isMangaMode ? "1" : "0");
			}
		}
	}
}
