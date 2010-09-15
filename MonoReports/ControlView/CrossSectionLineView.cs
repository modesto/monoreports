// 
// CrossSectionLineView.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki (at) gmail.com>
// 
// Copyright (c) 2010 Tomasz Kubacki 2010
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
using MonoReports.Model.Controls;
using MonoReports.Extensions.CairoExtensions;
using MonoReports.Core;

namespace MonoReports.ControlView
{
	public class CrossSectionLineView : LineView
	{
		
		SectionView startSection;
		
		public SectionView StartSection {
			get {
				return this.startSection;
			}
			set {
				startSection = value;
			}
		}

		SectionView endSection;

		public SectionView EndSection {
			get { return this.endSection; }
			set { endSection = value; }
		}

		public CrossSectionLineView (Line line, SectionView startSection, SectionView endSection) : base(line, startSection)
		{
			this.startSection = startSection;
			this.endSection = endSection;
		}

		bool isActive;

		public override Size Render (Cairo.Context c, RenderState renderState)
		{
			c.Save ();
			//renderState.IsDesign &&
			if ( renderState.Render) {
				if(renderState.SectionView == StartSection){
					Cairo.PointD p1 = new Cairo.PointD (line.Location.X, line.Location.Y);
					Cairo.PointD p2 = new Cairo.PointD (line.Location.X, renderState.Section.Height);
					c.DrawLine (p1, p2, line.BackgroundColor.ToCairoColor (), line.LineWidth,line.LineType);
					isActive = true;
				
				}else if(renderState.SectionView == EndSection){
					Cairo.PointD p1 = new Cairo.PointD (line.Location.X, line.End.Y);
					Cairo.PointD p2 = new Cairo.PointD (line.Location.X, 0);
					c.DrawLine (p1, p2, line.BackgroundColor.ToCairoColor (), line.LineWidth,line.LineType);
					isActive = false;
				}else{
					//if(isActive){
					Cairo.PointD p1 = new Cairo.PointD (line.Location.X, 0);
					Cairo.PointD p2 = new Cairo.PointD (line.Location.X, renderState.Section.Height);
					c.DrawLine (p1, p2, line.BackgroundColor.ToCairoColor (), line.LineWidth,line.LineType);
					//}
				}
			}
			c.Restore ();
			return new MonoReports.Model.Controls.Size (0, 0);
		}


		public override bool ContainsPoint (double x, double y)
		{
			double span = 2;
			Cairo.PointD p1 = ParentSection.AbsolutePointByLocalPoint (line.Location.X, line.Location.Y);
			Cairo.PointD p2 = ParentSection.AbsolutePointByLocalPoint (line.Location.X, double.MaxValue);
			Cairo.PointD hitPoint = new Cairo.PointD (x, y);
			if (hitPoint.X >= (Math.Max (p1.X, p2.X) + span) || hitPoint.X <= (Math.Min (p1.X, p2.X) - span) || hitPoint.Y >= (Math.Max (p1.Y, p2.Y) + span) || hitPoint.Y <= (Math.Min (p1.Y, p2.Y) - span))
				return false;
			
			if (p1.X == p2.X || p1.Y == p2.Y)
				return true;
			
			double y1, y2, x1, x2;
			double m, b;
			double ny;
			
			if (Math.Abs (p1.Y - p2.Y) <= Math.Abs (p1.X - p2.X)) {
				y1 = p1.Y;
				y2 = p2.Y;
				x1 = p1.X;
				x2 = p2.X;
			} else {
				y1 = p1.X;
				y2 = p2.X;
				x1 = p1.Y;
				x2 = p2.Y;
				
				double tmp = hitPoint.Y;
				hitPoint.Y = hitPoint.X;
				hitPoint.X = tmp;
			}
			
			m = (y2 - y1) / (x2 - x1);
			b = y1 - m * x1;
			
			ny = (m * ((double)hitPoint.X) + b) + 0.5;
			
			if (Math.Abs (hitPoint.Y - ny) > span)
				return false;
			
			return true;
		}
	}
}

