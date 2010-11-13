// 
// CairoExtensions.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki(at)gmail.com>
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

// This file is more or less copy of Jonathan Pobsts Pinta project file:
// http://github.com/jpobst/Pinta/raw/master/Pinta.Core/Extensions/CairoExtensions.cs
// CairoExtensions.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2010 Jonathan Pobst 

// Some functions are from Paint.NET:

/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using MonoReports.Model;
using MonoReports.Model.Controls;
using Cairo;

namespace MonoReports.Extensions.CairoExtensions
{
	
	
	public static class CairoExtensions
	{
		
		static Cairo.Color gripperColor = new Cairo.Color(1,0.2,0.2);
		static int gripperSize = 6;
		
		public static PointD ToCairoPointD(this MonoReports.Model.Controls.Point p){
			return new PointD(p.X,p.Y);
		}
		
		
		
		#region context
		public static Rectangle DrawRectangle (this Context g, Rectangle r, Cairo.Color color, double lineWidth)
		{			 	
			g.Save ();
			
			g.MoveTo (r.X, r.Y);
			g.LineTo (r.X + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y + r.Height);
			g.LineTo (r.X, r.Y + r.Height);
			g.LineTo (r.X, r.Y);
			
			g.Color = color;
			g.LineWidth = lineWidth;
			g.LineCap = LineCap.Square;
			
			Rectangle dirty = g.StrokeExtents ();
			g.Stroke ();
			
			g.Restore ();
			
			return dirty;
		}
		
		public static void DrawGripper(this Context g,PointD r){
			g.Save ();			 
			g.Color = gripperColor;
			 
			int halfPSize  = gripperSize / 2;			
			g.MoveTo (r.X - halfPSize , r.Y - halfPSize);
			g.LineTo (r.X + halfPSize , r.Y - halfPSize);
			g.LineTo (r.X + halfPSize , r.Y + halfPSize);
			g.LineTo (r.X - halfPSize , r.Y + halfPSize);
			g.LineTo (r.X - halfPSize , r.Y - halfPSize);		
			g.Fill ();
			g.Restore();
		}
		
		
		public static Rectangle DrawSelectBox (this Context g, Rectangle r)
		{
 
			g.Save ();
			
			g.Color = gripperColor;
			
			//left upper
			g.MoveTo (r.X, r.Y);
			g.LineTo (r.X + gripperSize, r.Y);
			g.LineTo (r.X + gripperSize, r.Y + gripperSize);
			g.LineTo (r.X, r.Y + gripperSize);
			g.LineTo (r.X, r.Y);		
			g.Fill ();
			
			//right upper
			g.MoveTo (r.X - gripperSize + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y + gripperSize);
			g.LineTo (r.X - gripperSize + r.Width, r.Y + gripperSize);
			g.LineTo (r.X - gripperSize + r.Width, r.Y);
			g.Fill ();
			g.Color = gripperColor;
			//right bottom
			g.MoveTo (r.X + r.Width - gripperSize , r.Y + r.Height - gripperSize);
			g.LineTo (r.X + r.Width, r.Y + r.Height- gripperSize);
			g.LineTo (r.X + r.Width, r.Y + r.Height);
			g.LineTo (r.X - gripperSize + r.Width, r.Y + r.Height);
			g.LineTo (r.X - gripperSize + r.Width, r.Y + r.Height - gripperSize);
			g.Fill ();
			
			g.Color = gripperColor;
			//left bottom
			g.MoveTo (r.X , r.Y + r.Height - gripperSize);
			g.LineTo (r.X + gripperSize, r.Y + r.Height- gripperSize);
			g.LineTo (r.X + gripperSize, r.Y + r.Height);
			g.LineTo (r.X, r.Y + r.Height);
			g.LineTo (r.X , r.Y + r.Height - gripperSize);
			g.Fill ();
			
			Rectangle dirty = g.StrokeExtents ();
 
			g.Restore ();
			
			return dirty;
		}
		
		public static void ClipRectangle (this Context g, Rectangle r)
		{
			g.MoveTo (r.X, r.Y);
			g.LineTo (r.X + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y + r.Height);
			g.LineTo (r.X, r.Y + r.Height);
			g.LineTo (r.X, r.Y);
			g.Clip ();
			
		}
		
		public static void DrawInsideBorder(this Context g,Rectangle r,Border border, bool render){
			
			g.Save ();
		
			g.Color = border.Color.ToCairoColor();
			g.LineWidth = border.TopWidth;
			g.LineCap = LineCap.Butt;
			double newUpperY = (r.Y + border.TopWidth / 2);			
			g.MoveTo (r.X, newUpperY);
			g.LineTo (r.X + r.Width, newUpperY);
			g.Stroke ();
			g.LineWidth = border.RightWidth;
			
			double rightX = r.X + r.Width-border.RightWidth/2;
			
			g.MoveTo (rightX, r.Y);
			g.LineTo (rightX, r.Y + r.Height);
			
			g.Stroke ();
			
			double bottomY = r.Y + r.Height - border.BottomWidth /2;
			g.MoveTo(r.X + r.Width, bottomY);
			g.LineWidth = border.BottomWidth;			
			g.LineTo (r.X,bottomY);
			g.Stroke ();
			
			g.LineWidth = border.LeftWidth;	
			double leftX = r.X + border.LeftWidth /2;
			g.MoveTo(leftX, r.Y + r.Height);
			g.LineTo (leftX, r.Y);
			if(render)
				g.Stroke ();			
			
			g.Restore ();
			
		}
		
	
		
		public static Path CreateRectanglePath (this Context g, Rectangle r)
		{
			g.Save ();
			
			g.MoveTo (r.X, r.Y);
			g.LineTo (r.X + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y + r.Height);
			g.LineTo (r.X, r.Y + r.Height);
			g.LineTo (r.X, r.Y);
			
			Path path = g.CopyPath ();
			g.Restore ();
			
			return path;
		}

		public static Rectangle FillRectangle (this Context g, Rectangle r, Cairo.Color color)
		{
			g.Save ();
			
			g.MoveTo (r.X, r.Y);
			g.LineTo (r.X + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y + r.Height);
			g.LineTo (r.X, r.Y + r.Height);
			g.LineTo (r.X, r.Y);
			
			g.Color = color;
			
			Rectangle dirty = g.StrokeExtents ();

			g.Fill ();
			g.Restore ();

			return dirty;
		}

		public static Rectangle FillRectangle (this Context g, Rectangle r, Pattern pattern)
		{
			g.Save ();			
			g.MoveTo (r.X, r.Y);
			g.LineTo (r.X + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y + r.Height);
			g.LineTo (r.X, r.Y + r.Height);
			g.LineTo (r.X, r.Y);
			
			g.Pattern = pattern;

			Rectangle dirty = g.StrokeExtents ();
			g.Fill ();

			g.Restore ();

			return dirty;
		}

		public static Rectangle DrawPolygonal (this Context g, PointD[] points, Cairo.Color color)
		{
			Random rand=new Random();
			
			g.Save ();
			g.MoveTo (points [0]);
			foreach (var point in points) {
				g.LineTo (point.X - rand.NextDouble()*0, point.Y);
				//g.Stroke();
			}
			
			g.Color = color;
			
			Rectangle dirty = g.StrokeExtents ();
			g.Stroke ();

			g.Restore ();

			return dirty;
		}

		public static Rectangle FillPolygonal (this Context g, PointD[] points, Cairo.Color color)
		{
			g.Save ();
			
			g.MoveTo (points [0]);
			foreach (var point in points)
				g.LineTo (point);
			
			g.Color = color;
			
			Rectangle dirty = g.StrokeExtents ();
			g.Fill ();

			g.Restore ();

			return dirty;
		}

		public static Rectangle FillStrokedRectangle (this Context g, Rectangle r, Cairo.Color fill, Cairo.Color stroke, int lineWidth)
		{
			double x = r.X;
			double y = r.Y;
			
			g.Save ();

			// Put it on a pixel line
			if (lineWidth == 1) {
				x += 0.5;
				y += 0.5;
			}
			
			g.MoveTo (x, y);
			g.LineTo (x + r.Width, y);
			g.LineTo (x + r.Width, y + r.Height);
			g.LineTo (x, y + r.Height);
			g.LineTo (x, y);
			
			g.Color = fill;
			g.FillPreserve ();
			
			g.Color = stroke;
			g.LineWidth = lineWidth;
			g.LineCap = LineCap.Square;
			
			Rectangle dirty = g.StrokeExtents ();
			
			g.Stroke ();
			g.Restore ();
			
			return dirty;
		}

		public static Rectangle DrawEllipse (this Context g, Rectangle r, Cairo.Color color, int lineWidth)
		{
			double rx = r.Width / 2;
			double ry = r.Height / 2;
			double cx = r.X + rx;
			double cy = r.Y + ry;
			double c1 = 0.552285;
			
			g.Save ();
			
			g.MoveTo (cx + rx, cy);
			
			g.CurveTo (cx + rx, cy - c1 * ry, cx + c1 * rx, cy - ry, cx, cy - ry);
			g.CurveTo (cx - c1 * rx, cy - ry, cx - rx, cy - c1 * ry, cx - rx, cy);
			g.CurveTo (cx - rx, cy + c1 * ry, cx - c1 * rx, cy + ry, cx, cy + ry);
			g.CurveTo (cx + c1 * rx, cy + ry, cx + rx, cy + c1 * ry, cx + rx, cy);
			
			g.ClosePath ();
			
			g.Color = color;
			g.LineWidth = lineWidth;
			
			Rectangle dirty = g.StrokeExtents ();

			g.Stroke ();
			g.Restore ();

			return dirty;
		}

		public static Rectangle FillEllipse (this Context g, Rectangle r, Cairo.Color color)
		{
			double rx = r.Width / 2;
			double ry = r.Height / 2;
			double cx = r.X + rx;
			double cy = r.Y + ry;
			double c1 = 0.552285;
			
			g.Save ();
			
			g.MoveTo (cx + rx, cy);
			
			g.CurveTo (cx + rx, cy - c1 * ry, cx + c1 * rx, cy - ry, cx, cy - ry);
			g.CurveTo (cx - c1 * rx, cy - ry, cx - rx, cy - c1 * ry, cx - rx, cy);
			g.CurveTo (cx - rx, cy + c1 * ry, cx - c1 * rx, cy + ry, cx, cy + ry);
			g.CurveTo (cx + c1 * rx, cy + ry, cx + rx, cy + c1 * ry, cx + rx, cy);
			
			g.ClosePath ();
			
			g.Color = color;
			
			Rectangle dirty = g.StrokeExtents ();
			
			g.Fill ();
			g.Restore ();
			
			return dirty;
		}

		public static Path CreateEllipsePath (this Context g, Rectangle r)
		{
			double rx = r.Width / 2;
			double ry = r.Height / 2;
			double cx = r.X + rx;
			double cy = r.Y + ry;
			double c1 = 0.552285;
			
			g.Save ();
			
			g.MoveTo (cx + rx, cy);
			
			g.CurveTo (cx + rx, cy - c1 * ry, cx + c1 * rx, cy - ry, cx, cy - ry);
			g.CurveTo (cx - c1 * rx, cy - ry, cx - rx, cy - c1 * ry, cx - rx, cy);
			g.CurveTo (cx - rx, cy + c1 * ry, cx - c1 * rx, cy + ry, cx, cy + ry);
			g.CurveTo (cx + c1 * rx, cy + ry, cx + rx, cy + c1 * ry, cx + rx, cy);
			
			g.ClosePath ();

			Path path = g.CopyPath ();
			
			g.Restore ();
			
			return path;
		}

		public static Rectangle FillStrokedEllipse (this Context g, Rectangle r, Cairo.Color fill, Cairo.Color stroke, int lineWidth)
		{
			double rx = r.Width / 2;
			double ry = r.Height / 2;
			double cx = r.X + rx;
			double cy = r.Y + ry;
			double c1 = 0.552285;
			
			g.Save ();
			
			g.MoveTo (cx + rx, cy);
			
			g.CurveTo (cx + rx, cy - c1 * ry, cx + c1 * rx, cy - ry, cx, cy - ry);
			g.CurveTo (cx - c1 * rx, cy - ry, cx - rx, cy - c1 * ry, cx - rx, cy);
			g.CurveTo (cx - rx, cy + c1 * ry, cx - c1 * rx, cy + ry, cx, cy + ry);
			g.CurveTo (cx + c1 * rx, cy + ry, cx + rx, cy + c1 * ry, cx + rx, cy);
			
			g.ClosePath ();
			
			g.Color = fill;
			g.FillPreserve ();
			
			g.Color = stroke;
			g.LineWidth = lineWidth;
			
			Rectangle dirty = g.StrokeExtents ();
			
			g.Stroke ();
			g.Restore ();
			
			return dirty;
		}

		public static Rectangle FillStrokedRoundedRectangle (this Context g, Rectangle r, double radius, Cairo.Color fill, Cairo.Color stroke, int lineWidth)
		{
			g.Save ();

			if ((radius > r.Height / 2) || (radius > r.Width / 2))
				radius = Math.Min (r.Height / 2, r.Width / 2);

			g.MoveTo (r.X, r.Y + radius);
			g.Arc (r.X + radius, r.Y + radius, radius, Math.PI, -Math.PI / 2);
			g.LineTo (r.X + r.Width - radius, r.Y);
			g.Arc (r.X + r.Width - radius, r.Y + radius, radius, -Math.PI / 2, 0);
			g.LineTo (r.X + r.Width, r.Y + r.Height - radius);
			g.Arc (r.X + r.Width - radius, r.Y + r.Height - radius, radius, 0, Math.PI / 2);
			g.LineTo (r.X + radius, r.Y + r.Height);
			g.Arc (r.X + radius, r.Y + r.Height - radius, radius, Math.PI / 2, Math.PI);
			g.ClosePath ();
			
			g.Color = fill;
			g.FillPreserve ();
			
			g.Color = stroke;
			g.LineWidth = lineWidth;
			
			Rectangle dirty = g.StrokeExtents ();
			
			g.Stroke ();
			g.Restore ();
			
			return dirty;
		}

		public static Rectangle FillRoundedRectangle (this Context g, Rectangle r, double radius, Cairo.Color fill)
		{
			g.Save ();

			if ((radius > r.Height / 2) || (radius > r.Width / 2))
				radius = Math.Min (r.Height / 2, r.Width / 2);

			g.MoveTo (r.X, r.Y + radius);
			g.Arc (r.X + radius, r.Y + radius, radius, Math.PI, -Math.PI / 2);
			g.LineTo (r.X + r.Width - radius, r.Y);
			g.Arc (r.X + r.Width - radius, r.Y + radius, radius, -Math.PI / 2, 0);
			g.LineTo (r.X + r.Width, r.Y + r.Height - radius);
			g.Arc (r.X + r.Width - radius, r.Y + r.Height - radius, radius, 0, Math.PI / 2);
			g.LineTo (r.X + radius, r.Y + r.Height);
			g.Arc (r.X + radius, r.Y + r.Height - radius, radius, Math.PI / 2, Math.PI);
			g.ClosePath ();

			g.Color = fill;
			
			Rectangle dirty = g.StrokeExtents ();

			g.Fill ();
			g.Restore ();

			return dirty;
		}

		public static void FillRegion (this Context g, Gdk.Region region, Cairo.Color color)
		{
			g.Save ();
			
			g.Color = color;
			
			foreach (Gdk.Rectangle r in region.GetRectangles())
			{
				g.MoveTo (r.X, r.Y);
				g.LineTo (r.X + r.Width, r.Y);
				g.LineTo (r.X + r.Width, r.Y + r.Height);
				g.LineTo (r.X, r.Y + r.Height);
				g.LineTo (r.X, r.Y);
				
				g.Color = color;

				g.StrokeExtents ();
				g.Fill ();
			}
			
			g.Restore ();
		}

		public static Rectangle DrawRoundedRectangle (this Context g, Rectangle r, double radius, Cairo.Color stroke, int lineWidth)
		{
			g.Save ();
			
			Path p = g.CreateRoundedRectanglePath (r, radius);
			
			g.AppendPath (p);
			
			g.Color = stroke;
			g.LineWidth = lineWidth;
			
			Rectangle dirty = g.StrokeExtents ();

			g.Stroke ();
			g.Restore ();

			(p as IDisposable).Dispose ();
			
			return dirty;
		}

		public static Path CreateRoundedRectanglePath (this Context g, Rectangle r, double radius)
		{
			g.Save ();

			if ((radius > r.Height / 2) || (radius > r.Width / 2))
				radius = Math.Min (r.Height / 2, r.Width / 2);

			g.MoveTo (r.X, r.Y + radius);
			g.Arc (r.X + radius, r.Y + radius, radius, Math.PI, -Math.PI / 2);
			g.LineTo (r.X + r.Width - radius, r.Y);
			g.Arc (r.X + r.Width - radius, r.Y + radius, radius, -Math.PI / 2, 0);
			g.LineTo (r.X + r.Width, r.Y + r.Height - radius);
			g.Arc (r.X + r.Width - radius, r.Y + r.Height - radius, radius, 0, Math.PI / 2);
			g.LineTo (r.X + radius, r.Y + r.Height);
			g.Arc (r.X + radius, r.Y + r.Height - radius, radius, Math.PI / 2, Math.PI);
			g.ClosePath ();
		
			Path p = g.CopyPath ();
			g.Restore ();
			
			return p;
		}
		
		
		static double[] dotsStyle = new double[]{1};
		static double[] dashesStyle = new double[]{3};
		static double[] dashDotStyle = new double[]{3,1};
		static double[] dashDotDotStyle = new double[]{3,1,1};
		
		static double[] getDashByLineStyle(LineType type ){
			
		switch ( type) {
				
				case LineType.Solid:				
					return null;
				case LineType.Dots:
					return dotsStyle;
				case LineType.Dash:
					return dashesStyle;
				case LineType.DashDot:
					return dashDotStyle;
				case LineType.DashDotDot:
					return dashDotDotStyle;			 
				 
			default:
			break;
			}
			return null;
		}
		
		

		public static Rectangle DrawLine (this Context g, PointD p1, PointD p2, Cairo.Color color, double lineWidth, LineType lineType, bool render)
		{
			g.Save ();
			var dashesStyle = getDashByLineStyle(lineType);
			if(dashesStyle != null){
				g.SetDash(dashesStyle,lineWidth);
			}
			g.MoveTo (p1.X, p1.Y);
			g.LineTo (p2.X, p2.Y);

			g.Color = color;
			g.LineWidth = lineWidth;
			g.LineCap = LineCap.Butt;

			Rectangle dirty = g.StrokeExtents ();
			if(render)
				g.Stroke ();
			
			g.Restore ();

			return dirty;
		}

		private static Pango.Style CairoToPangoSlant (Cairo.FontSlant slant)
		{
			switch (slant) {
			case Cairo.FontSlant.Italic:
				return Pango.Style.Italic;
			case Cairo.FontSlant.Oblique:
				return Pango.Style.Oblique;
			default:
				return Pango.Style.Normal;
			}
		}
		
		private static Pango.Style ReportToPangoSlant (MonoReports.Model.FontSlant slant)
		{
			switch (slant) {
			case MonoReports.Model.FontSlant.Italic:
				return Pango.Style.Italic;
			case MonoReports.Model.FontSlant.Oblique:
				return Pango.Style.Oblique;
			default:
				return Pango.Style.Normal;
			}
		}
		
		

		private static Pango.Weight CairoToPangoWeight (Cairo.FontWeight weight)
		{
			return (weight == Cairo.FontWeight.Bold) ? Pango.Weight.Bold : Pango.Weight.Normal;
		}
		
		private static Pango.Weight ReportToPangoWeight (MonoReports.Model.FontWeight weight)
		{
			return (weight == MonoReports.Model.FontWeight.Bold) ? Pango.Weight.Bold : Pango.Weight.Normal;
		}
		
	 
		
		private static Pango.Alignment ReportToPangoAlignment (HorizontalAlignment align)
		{
			switch (align) {
			case MonoReports.Model.Controls.HorizontalAlignment.Left:
				return Pango.Alignment.Left;
			case MonoReports.Model.Controls.HorizontalAlignment.Center:
				return Pango.Alignment.Center;
			case MonoReports.Model.Controls.HorizontalAlignment.Right:
				return Pango.Alignment.Right;
			default:
				return Pango.Alignment.Left;
			}
		}

		public static Rectangle DrawText (this Context g, PointD p, string family, Cairo.FontSlant slant, Cairo.FontWeight weight, double size, Cairo.Color color, double width, string text)
		{
			g.Save ();
			g.MoveTo (p.X, p.Y);
			g.Color = color;
			Pango.Layout layout = Pango.CairoHelper.CreateLayout (g);
			layout.Wrap = Pango.WrapMode.Char;
			layout.Width = (int)(width * Pango.Scale.PangoScale);
			Pango.FontDescription fd = new Pango.FontDescription ();			
			fd.Family = family;
		
			
			fd.Style = CairoToPangoSlant (slant);
			fd.Weight = CairoToPangoWeight (weight);
			
			fd.AbsoluteSize = size * Pango.Scale.PangoScale;
			layout.FontDescription = fd;
		 
			layout.Spacing = 0;
			layout.Alignment = Pango.Alignment.Left;
			layout.SetText (text);
		
			Pango.Rectangle unused = Pango.Rectangle.Zero;
			Pango.Rectangle te = Pango.Rectangle.Zero;
			layout.GetExtents (out unused, out te);						
			Pango.CairoHelper.ShowLayout(g, layout);			
			
			layout.GetExtents (out unused, out te);
			
			(layout as IDisposable).Dispose();
			
			g.Restore ();
			
			return new Rectangle( p.X + unused.X / Pango.Scale.PangoScale, p.Y + unused.Y / Pango.Scale.PangoScale, (unused.Width / Pango.Scale.PangoScale) , (unused.Height/ Pango.Scale.PangoScale) );
						
		}
		
		
		public static Rectangle DrawTextBlock (this Context g, TextBlock tb, bool render) {
			
		 	double topBottomSpan = tb.Padding.Top + tb.Padding.Bottom;
			double leftRightSpan = tb.Padding.Left + tb.Padding.Right;
			g.Save ();
			g.MoveTo (tb.Left + tb.Padding.Left, tb.Top + tb.Padding.Top);
			g.Color = tb.FontColor.ToCairoColor();
			Pango.Layout layout = Pango.CairoHelper.CreateLayout (g);
			
			layout.Wrap = Pango.WrapMode.Word;
			layout.Width = (int)((tb.Width - leftRightSpan) * Pango.Scale.PangoScale);
			Pango.FontDescription fd = new Pango.FontDescription ();			
			fd.Family = tb.FontName;
		
			
			fd.Style = ReportToPangoSlant (tb.FontSlant);
			fd.Weight = ReportToPangoWeight (tb.FontWeight);
			
			fd.AbsoluteSize = tb.FontSize * Pango.Scale.PangoScale;
			layout.FontDescription = fd;
		 
			layout.Spacing = (int)(tb.LineSpan * Pango.Scale.PangoScale);
			layout.Indent = (int)(0 * Pango.Scale.PangoScale);			
			layout.Alignment = ReportToPangoAlignment (tb.HorizontalAlignment);  
			layout.SetText (tb.Text);
		
			Pango.Rectangle unused = Pango.Rectangle.Zero;
			Pango.Rectangle te = Pango.Rectangle.Zero;
			layout.GetExtents (out unused, out te);		
			
			if (tb.VerticalAlignment == VerticalAlignment.Center) {
					double height = te.Height / Pango.Scale.PangoScale;
					if(height < tb.Height){
						double sp = ((tb.Height  - (height + (tb.FontSize/2))) / 2) ;
						g.MoveTo (tb.Left + tb.Padding.Left, tb.Top + tb.Padding.Top + sp);
					}
			}
			
			if (render) {
				Pango.CairoHelper.ShowLayout(g, layout);						
			}
			
			layout.GetExtents (out unused, out te);
			
			(layout as IDisposable).Dispose();
			
			g.Restore ();
		
			return new Rectangle( tb.Left - tb.Padding.Left + te.X / Pango.Scale.PangoScale, tb.Top - tb.Padding.Top + te.Y / Pango.Scale.PangoScale, (te.Width / Pango.Scale.PangoScale) + leftRightSpan , (te.Height/ Pango.Scale.PangoScale) + topBottomSpan);
						
		}

		public static void DrawPixbuf (this Context g, Gdk.Pixbuf pixbuf, PointD dest)
		{
			g.Save ();

			Gdk.CairoHelper.SetSourcePixbuf (g, pixbuf, dest.X, dest.Y);
			g.Paint ();
			g.Restore ();
		}

		public static void DrawLinearGradient (this Context g, Surface oldsurface, GradientColorMode mode, Cairo.Color c1, Cairo.Color c2, PointD p1, PointD p2)
		{
			g.Save ();
			
			Gradient gradient = new Cairo.LinearGradient (p1.X, p1.Y, p2.X, p2.Y);
			
			if (mode == GradientColorMode.Color) {
				gradient.AddColorStop (0, c1);
				gradient.AddColorStop (1, c2);
				g.Source = gradient;
				g.Paint ();
			}
			else if (mode == GradientColorMode.Transparency) {
				gradient.AddColorStop (0, new Cairo.Color (0, 0, 0, 1));
				gradient.AddColorStop (1, new Cairo.Color (0, 0, 0, 0));
				g.Source = new SurfacePattern (oldsurface);
				g.Mask (gradient);
			}
			
			g.Restore ();
		}

		public static void DrawLinearReflectedGradient (this Context g, Surface oldsurface, GradientColorMode mode, Cairo.Color c1, Cairo.Color c2, PointD p1, PointD p2)
		{
			g.Save ();
			
			Gradient gradient = new Cairo.LinearGradient (p1.X, p1.Y, p2.X, p2.Y);
			
			if (mode == GradientColorMode.Color) {
				gradient.AddColorStop (0, c1);
				gradient.AddColorStop (0.5, c2);
				gradient.AddColorStop (1, c1);
				g.Source = gradient;
				g.Paint ();
			}
			else if (mode == GradientColorMode.Transparency) {
				gradient.AddColorStop (0, new Cairo.Color (0, 0, 0, 1));
				gradient.AddColorStop (0.5, new Cairo.Color (0, 0, 0, 0));
				gradient.AddColorStop (1, new Cairo.Color (0, 0, 0, 1));
				g.Source = new SurfacePattern (oldsurface);
				g.Mask (gradient);
			}
			
			g.Restore ();
		}

		public static void DrawRadialGradient (this Context g, Surface oldsurface, GradientColorMode mode, Cairo.Color c1, Cairo.Color c2, PointD p1, PointD p2, double r1, double r2)
		{
			g.Save ();
			
			Gradient gradient = new Cairo.RadialGradient (p1.X, p1.Y, r1, p2.X, p2.Y, r2);
			
			if (mode == GradientColorMode.Color) {
				gradient.AddColorStop (0, c1);
				gradient.AddColorStop (1, c2);
				g.Source = gradient;
				g.Paint ();
			}
			else if (mode == GradientColorMode.Transparency) {
				gradient.AddColorStop (0, new Cairo.Color (0, 0, 0, 1));
				gradient.AddColorStop (1, new Cairo.Color (0, 0, 0, 0));
				g.Source = new SurfacePattern (oldsurface);
				g.Mask (gradient);
			}
			
			g.Restore ();
		}
		#endregion
		
		public static double Distance (this PointD s, PointD e)
		{
			return Magnitude (new PointD (s.X - e.X, s.Y - e.Y));
		}
		
		public static double Magnitude(this PointD p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

		public static Cairo.Rectangle ToCairoRectangle (this Gdk.Rectangle r)
		{
			return new Cairo.Rectangle (r.X, r.Y, r.Width, r.Height);
		}

		public static Cairo.Point Location (this Cairo.Rectangle r)
		{
			return new Cairo.Point ((int)r.X, (int)r.Y);
		}

		public static Cairo.Rectangle Clamp (this Cairo.Rectangle r)
		{
			double x = r.X;
			double y = r.Y;
			double w = r.Width;
			double h = r.Height;
			
			if (x < 0) {
				w -= x;
				x = 0;
			}
			
			if (y < 0) {
				h -= y;
				y = 0;
			}
			
			return new Cairo.Rectangle (x, y, w, h);
		}

		public static Gdk.Rectangle ToGdkRectangle (this Cairo.Rectangle r)
		{
			return new Gdk.Rectangle ((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
		}

		public static bool ContainsPoint (this Cairo.Rectangle r, double x, double y)
		{
			if (x < r.X || x >= r.X + r.Width)
				return false;

			if (y < r.Y || y >= r.Y + r.Height)
				return false;

			return true;
		}
				

		public static bool ContainsPoint (this Cairo.Rectangle r, PointD point)
		{
			return ContainsPoint (r, point.X, point.Y);
		}
		
 
		
	 
		
		

		public static string ToString2 (this Cairo.Color c)
		{
			return string.Format ("R: {0} G: {1} B: {2} A: {3}", c.R, c.G, c.B, c.A);
		}

		public static string ToString2 (this PointD c)
		{
			return string.Format ("{0}, {1}", c.X, c.Y);
		}

		
		
		public static Gdk.Size ToSize (this Cairo.Point point)
		{
			return new Gdk.Size (point.X, point.Y);
		}
		
		public static ImageSurface Clone (this ImageSurface surf)
		{
			ImageSurface newsurf = new ImageSurface (surf.Format, surf.Width, surf.Height);

			using (Context g = new Context (newsurf)) {
				g.SetSource (surf);
				g.Paint ();
			}

			return newsurf;
		}

		//public static Path Clone (this Path path)
		//{
	//		Path newpath;
			
			//using (Context g = new Context (ReportContext.CurrentReportContext.Surface)) {
			//	g.AppendPath (path);
			//	newpath = g.CopyPath ();
			//}
			
		 	//return newpath;
		//}
		
		//public static Gdk.Rectangle GetBounds (this Path path)
		//{
		//	Rectangle rect;
			 
			 
		//	using (Context g = new Context (ReportContext.CurrentReportContext.Surface)) {
		//		g.AppendPath (ReportContext.CurrentReportContext.SelectionPath);

				// We don't want the bounding box to include a stroke width 
				// of 1, but setting it to 0 returns an empty rectangle.  Set
				// it to a sufficiently small width and rounding takes care of it
		//		g.LineWidth = .01;
		//		rect = g.StrokeExtents ();
//}

	//		return new Gdk.Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width - (int)rect.X, (int)rect.Height - (int)rect.Y);
	//	}
		
		public static Gdk.Color ToGdkColor (this Cairo.Color color)
		{
			Gdk.Color c = new Gdk.Color ();
			c.Blue = (ushort)(color.B * ushort.MaxValue);
			c.Red = (ushort)(color.R * ushort.MaxValue);
			c.Green = (ushort)(color.G * ushort.MaxValue);
			
			return c;
		}
		
		public static Cairo.Color ToCairoColor (this MonoReports.Model.Controls.Color color)
		{
			Cairo.Color c = new Cairo.Color ();
			c.A =  (((double)color.A) );
			c.R = (((double)color.R) );
			c.G = (((double)color.G) );
			c.B = (((double)color.B) );
			
			return c;
		}
		
		public static ushort GdkColorAlpha (this Cairo.Color color)
		{
			return (ushort)(color.A * ushort.MaxValue);
		}

		public static double GetBottom (this Rectangle rect)
		{
			return rect.Y + rect.Height;
		}

		public static double GetRight (this Rectangle rect)
		{
			return rect.X + rect.Width;
		}

        /// <summary>
        /// Determines if the requested pixel coordinate is within bounds.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>true if (x,y) is in bounds, false if it's not.</returns>
        public static bool IsVisible(this ImageSurface surf, int x, int y)
        {
            return x >= 0 && x < surf.Width && y >= 0 && y < surf.Height;
        }

		
		 

		 

		public static Gdk.Rectangle GetBounds (this ImageSurface surf)
		{
			return new Gdk.Rectangle (0, 0, surf.Width, surf.Height);
		}

		public static Gdk.Size GetSize (this ImageSurface surf)
		{
			return new Gdk.Size (surf.Width, surf.Height);
		}


		/// <summary>
		/// There was a bug in gdk-sharp where this returns incorrect values.
		/// We will probably have to use this for a long time until every distro
		/// has an updated gdk.
		/// </summary>
		public static bool ContainsCorrect (this Gdk.Rectangle r, int x, int y)
		{
			return ((((x >= r.Left) && (x < r.Right)) && (y >= r.Top)) && (y < r.Bottom));
		}

		/// <summary>
		/// There was a bug in gdk-sharp where this returns incorrect values.
		/// We will probably have to use this for a long time until every distro
		/// has an updated gdk.
		/// </summary>
		public static bool ContainsCorrect (this Gdk.Rectangle r, Gdk.Point pt)
		{
			return r.ContainsCorrect (pt.X, pt.Y);
		}

  
		
		private struct Edge
        {
            public int miny;   // int
            public int maxy;   // int
            public int x;      // fixed point: 24.8
            public int dxdy;   // fixed point: 24.8

            public Edge(int miny, int maxy, int x, int dxdy)
            {
                this.miny = miny;
                this.maxy = maxy;
                this.x = x;
                this.dxdy = dxdy;
            }
        }
		
		public static void TranslatePointsInPlace (this Cairo.Point[] Points, int dx, int dy)
		{
			for (int i = 0; i < Points.Length; ++i)
            {
                Points[i].X += dx;
                Points[i].Y += dy;
            }
		}
		
		
		public static Path CreatePolygonPath (this Context g, Cairo.Point[][] polygonSet)
		{
			g.Save ();
			Cairo.Point p;
			for (int i =0; i < polygonSet.Length; i++)
			{
				if (polygonSet[i].Length == 0)
					continue;
				
				p = polygonSet[i][0];
				g.MoveTo (p.X, p.Y);
				
				for (int j =1; j < polygonSet[i].Length; j++)
				{
					p = polygonSet[i][j];
					g.LineTo (p.X, p.Y);	
				}
				g.ClosePath ();
			}
			
			Path path = g.CopyPath ();
			
			g.Restore ();
			
			return path;
		}
		
		public static Gdk.Point ToGdkPoint (this PointD point)
		{
			return new Gdk.Point ((int)point.X, (int)point.Y);
		}
		
		
		 public enum GradientColorMode
{
Color,
Transparency
}
	}
}
