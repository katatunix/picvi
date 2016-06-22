using System;
using System.Windows.Forms;

namespace picvi
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(String[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (args.Length == 0)
			{
				MessageBox.Show(
					"Please right-click on an image file and select open with this app.",
					"picvi",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			Application.Run(new FormMain(args[0]));
		}
	}
}
