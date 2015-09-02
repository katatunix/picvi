using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace picvi
{
	interface Observer
	{
		void onNew(Image image, String title);
	}
}
