// 
// SectionRenderer.cs
//  
// Author:
//       Tomasz Kubacki <tomasz.kubacki (at) gmail.com>
// 
// Copyright (c) 2010 Tomasz Kubacki
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MonoReports.Core;
using MonoReports.Model.Controls;
using MonoReports.Extensions.CairoExtensions;
using Cairo;
using MonoReports.Model;

namespace MonoReports.Renderers
{
	public class SectionRenderer: ControlRendererBase, IControlRenderer
	{
		public SectionRenderer ()
		{
		}
 

		public void Render (Cairo.Context c, MonoReports.Model.Controls.Control control)
		{
			Section section = control as Section;
			Rectangle borderRect;
			c.Save ();
			borderRect = new Rectangle (section.Location.X, section.Location.Y, section.Width, section.Height);
			c.ClipRectangle (borderRect);
			borderRect = new Rectangle (section.Location.X, section.Location.Y, section.Width, section.Height);
			c.FillRectangle (borderRect, section.BackgroundColor.ToCairoColor ());
			 		
			c.Restore (); 
		}

		public Size Measure (Cairo.Context c, MonoReports.Model.Controls.Control control)
		{
			Section section = control as Section;
			Rectangle borderRect = new Rectangle (section.Location.X, section.Location.Y, section.Width, section.Height);
			return new MonoReports.Model.Size(borderRect.Width,borderRect.Height);
		}
		
		public Control[] BreakOffControlAtMostAtHeight(Cairo.Context c, Control control, double height) {
			Control[] controls = new Control[2];
            var newControl = control.CreateControl();
			var newControl1 = control.CreateControl();
			newControl.Height = height;
            newControl1.Height = control.Height - height;			
            newControl1.Top = 0;
            controls[1] = newControl1;
			controls[0] = newControl;
			return controls;
		}
		 
	}
}

