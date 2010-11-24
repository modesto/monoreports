using System;

namespace PropertyGrid
{
	public class CompactScrolledWindow : Gtk.ScrolledWindow
	{
		 
		
		static CompactScrolledWindow ()
		{
			 
		}
		
		public CompactScrolledWindow () : base ()
		{
			//HACK to hide the useless padding that many themes have inside the ScrolledWindow - GTK default is 3
			//Gtk.Rc.ParseString (string.Format ("widget \"*.{0}\" style \"{1}\" ", Name, styleName));
		}
	}
}

