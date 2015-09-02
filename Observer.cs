using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace picvi
{
	interface Observer
	{
		void onNewImage(String title);
		void onPicSizeChanged();
	}
}
