// 
//Author:
//       Tomasz Kubacki <tomasz (dot ) kubacki (at) gmail (dot) com>
// copied from:
//   Lluis Sanchez Gual
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;
using MonoReports.Model.Controls;
using PropertyGrid;

namespace MonoReports.Extensions.PropertyGridEditors
{
	[PropertyEditorType (typeof (Color))]
	public class MonoreportsColorEditorCell: PropertyEditorCell 
	{
		const int ColorBoxSize = 16;
		const int ColorBoxSpacing = 3;
		
		public override void GetSize (int availableWidth, out int width, out int height)
		{
			base.GetSize (availableWidth - ColorBoxSize - ColorBoxSpacing, out width, out height);
			width += ColorBoxSize + ColorBoxSpacing;
			if (height < ColorBoxSize) height = ColorBoxSize;
		}
		
		protected override string GetValueText ()
		{
			Color color = (Color) Value;
			return String.Format("#{0};{1};{2}", color.R, color.G, color.B);
		}
		
		public override void Render (Gdk.Drawable window, Gdk.Rectangle bounds, Gtk.StateType state)
		{
			Gdk.GC gc = new Gdk.GC (window);
	   		gc.RgbFgColor = GetColor ();
	   		int yd = (bounds.Height - ColorBoxSize) / 2;
			window.DrawRectangle (gc, true, bounds.X, bounds.Y + yd, ColorBoxSize - 1, ColorBoxSize - 1);
			window.DrawRectangle (Container.Style.BlackGC, false, bounds.X, bounds.Y + yd, ColorBoxSize - 1, ColorBoxSize - 1);
			bounds.X += ColorBoxSize + ColorBoxSpacing;
			bounds.Width -= ColorBoxSize + ColorBoxSpacing;
			base.Render (window, bounds, state);
		}
		
		private Gdk.Color GetColor ()
		{
			Color color = (Color) Value;			
			return new Gdk.Color ( (byte)(color.R * 255), (byte) (color.G * 255), (byte) (color.B * 255));
		}

		protected override IPropertyEditor CreateEditor (Gdk.Rectangle cell_area, Gtk.StateType state)
		{
			return new MonoreportsColorEditor ();
		}
	}
	
	public class MonoreportsColorEditor : Gtk.ColorButton, IPropertyEditor
	{
		public void Initialize (EditSession session)
		{
			if (session.Property.PropertyType != typeof(Color))
				throw new ApplicationException ("Color editor does not support editing values of type " + session.Property.PropertyType);
		}
		
		 
		
		public object Value { 
			get {
				double r =  ( (double) Color.Red / ushort.MaxValue);
				double g =  ((double) Color.Green / ushort.MaxValue);
				double b =  ( (double) Color.Blue / ushort.MaxValue);
				return new Color(r,g,b);
			}
			set {
				Color color = (Color) value;	
				Color = new Gdk.Color ((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
			}
		}
		
		protected override void OnColorSet ()
		{
			base.OnColorSet ();
			if (ValueChanged != null)
				ValueChanged (this, EventArgs.Empty);
		}

		public event EventHandler ValueChanged;
	}
}

