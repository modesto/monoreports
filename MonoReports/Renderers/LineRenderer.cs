// 
// LineRenderer.cs
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
	public class LineRenderer: ControlRendererBase, IControlRenderer
	{
		public LineRenderer ()
		{
		}

		public void Render (Cairo.Context c, Control control)
		{
			Line line = control as Line;
			Cairo.PointD p1 = new Cairo.PointD (line.Location.X ,line.Location.Y);
			Cairo.PointD p2 = new Cairo.PointD (line.End.X, line.End.Y);
			c.DrawLine (p1, p2, line.BackgroundColor.ToCairoColor (), line.LineWidth, line.LineType, true);
		}

		public Size Measure (Cairo.Context c, Control control)
		{
			Line line = control as Line;
			Cairo.PointD p1 = new Cairo.PointD (line.Location.X ,line.Location.Y);
			Cairo.PointD p2 = new Cairo.PointD (line.End.X, line.End.Y);
			var r = c.DrawLine (p1, p2, line.BackgroundColor.ToCairoColor (), line.LineWidth, line.LineType, false);
			return new Size (r.Width, r.Height);
		}

		public Control[] BreakOffControlAtMostAtHeight (Cairo.Context c, Control control, double height)
		{
			Control[] controls = new Control[2];
			var first = control.CreateControl () as Line;
			var second = control.CreateControl () as Line;
			double newX = 0;
						
			if ( first.Location.X != first.End.X ) {
			 
				
				if ( first.Location.Y > first.End.Y ) {
					newX = calculateXAtYZero(first.End.X,height,first.Location.X,-(first.Height - height));
					first.Location = new MonoReports.Model.Controls.Point(newX, first.End.Y + height);										
					double deltaW = second.End.X - newX;				
					second.Left -= deltaW;					
					second.Top = 0;
					second.Location = new MonoReports.Model.Controls.Point(second.Location.X + deltaW , second.Location.Y - height);	
				} else if (first.Location.Y < first.End.Y) {
					
					newX = calculateXAtYZero(first.Location.X,height,first.End.X,-(first.Height - height));
					first.End = new MonoReports.Model.Controls.Point(newX, first.Location.Y + height);										
					double deltaW = second.Location.X - newX;				
					second.Left -= deltaW;					
					second.Top = 0;
					second.End = new MonoReports.Model.Controls.Point(second.End.X + deltaW , second.End.Y - height);			
				}
			} else {
				if ( first.Location.Y > first.End.Y ) {
					first.Location = new MonoReports.Model.Controls.Point(first.Location.X, first.End.Y + height);		
					second.Top = 0;
					second.Location = new MonoReports.Model.Controls.Point(second.Location.X, second.Location.Y - height);
				} else if (first.Location.Y < first.End.Y) {
					first.End = new MonoReports.Model.Controls.Point(first.End.X, first.Location.Y + height);		
					second.Top = 0;
					second.End = new MonoReports.Model.Controls.Point(second.End.X , second.End.Y - height);
				}
				 		
			}

			controls [0] = first;			
			controls [1] = second;
			return controls;
		}
			
		/// <summary>
		/// Calculates the X at Y = 0.
		/// </summary>
		/// <returns>
		/// The X at Y = 0.
		/// </returns>
		/// <param name='x1'>
		/// x1
		/// </param>
		/// <param name='y1'>
		/// y1.
		/// </param>
		/// <param name='x2'>
		/// x2.
		/// </param>
		/// <param name='y2'>
		/// y2.
		/// </param>
		static double calculateXAtYZero (double x1,double y1, double x2, double y2)
		{	
			if (y1 == y2) {
				return x1;				
			} else {			
				return x1 - (((y1 * x1) - (y1 * x2)) / (y1 - y2));
			}
		}
	
	}
}

