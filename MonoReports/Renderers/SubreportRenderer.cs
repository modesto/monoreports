// 
// ImageRenderer.cs
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
	public class SubreportRenderer: ControlRendererBase, IControlRenderer
	{
		public SubreportRenderer ()
		{
		}
		
		
		 

		public void Render (Cairo.Context c, MonoReports.Model.Controls.Control control)
		{
			SubReport subreport = control as SubReport;
			Rectangle borderRect;
			c.Save ();
			borderRect = new Rectangle (subreport.Location.X, subreport.Location.Y, subreport.Width, subreport.Height);
			c.ClipRectangle (borderRect);
			borderRect = new Rectangle (subreport.Location.X, subreport.Location.Y, subreport.Width, subreport.Height);
			c.FillRectangle (borderRect, subreport.BackgroundColor.ToCairoColor ());			
			c.Restore (); 
		}

		public MonoReports.Model.Controls.Size Measure (Cairo.Context c, MonoReports.Model.Controls.Control control)
		{
			SubReport subreport = control as SubReport;
			Rectangle borderRect = new Rectangle (subreport.Location.X, subreport.Location.Y, subreport.Width, subreport.Height);
			return new MonoReports.Model.Controls.Size(borderRect.Width,borderRect.Height);
		}
		
		public Control[] BreakOffControlAtMostAtHeight(Cairo.Context c, Control control, double height) {
			Control[] controls = new Control[2];
			controls[1] = control;
			return controls;
		}
		 
	}
}

