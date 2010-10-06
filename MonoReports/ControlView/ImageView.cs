// 
// ImageView.cs
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
using MonoReports.ControlView;
using Cairo;
using MonoReports.Core;
using MonoReports.Extensions.CairoExtensions;
using MonoReports.Model.Controls;
using MonoReports.Services;

namespace MonoReports.ControlView
{
	public class ImageView : ControlViewBase
	{
		Image image;
		DesignService designService = null;
		
		public override Control ControlModel {
			get { return base.ControlModel; }
			set {
				base.ControlModel = value;
				if (image != value) {
					image = value as Image;	
					if(designService != null){
						pixbuf = new Gdk.Pixbuf (designService.Report.ResourceRepository[image.ImageIndex]);
					}
				}
			}
		}

		public Model.Controls.Image Image {
			get { return image; }
		}


		private Gdk.Pixbuf pixbuf;


		public ImageView (MonoReports.Model.Controls.Image image, SectionView parentSection, DesignService designService): base(image)
		{			
			this.ParentSection = parentSection;
			this.designService = designService;
			pixbuf = new Gdk.Pixbuf (designService.Report.ResourceRepository[image.ImageIndex]);
			AbsoluteBound = new Rectangle (parentSection.AbsoluteDrawingStartPoint.X + image.Location.X, parentSection.AbsoluteDrawingStartPoint.Y + image.Location.Y, image.Width, image.Height);
			
		}

		#region implemented abstract members of MonoReport.ControlView.ControlViewBase


		public override Size Render (Context c, RenderState renderState)
		{
			Rectangle borderRect;
			c.Save ();
			borderRect = new Rectangle (image.Location.X, image.Location.Y, image.Width, image.Height);
			c.ClipRectangle (borderRect);
			
			//c.DrawPixbuf(pixbuf,image.Location.ToCairoPointD());
			Rectangle rect = new Rectangle (image.Location.X, image.Location.Y, image.Width, image.Height);
			
			borderRect = new Rectangle (image.Location.X, image.Location.Y, image.Width, image.Height);
			
			if (renderState.Render) {
				c.FillRectangle (borderRect, image.BackgroundColor.ToCairoColor ());
				c.DrawPixbuf (pixbuf, image.Location.ToCairoPointD ());
				c.DrawInsideBorder (borderRect, image.Border, renderState.Render);
			} else {
				// c.DrawPixbuf(pixbuf,image.Location.ToCairoPointD());
			}
			AbsoluteBound = new Rectangle (ParentSection.AbsoluteDrawingStartPoint.X + image.Location.X, ParentSection.AbsoluteDrawingStartPoint.Y + image.Location.Y, borderRect.Width, borderRect.Height);
			
			c.Restore ();
			return new MonoReports.Model.Controls.Size (rect.Width, rect.Height);
		}


		public override bool ContainsPoint (double x, double y)
		{
			return AbsoluteBound.ContainsPoint (x, y);
		}
		
		#endregion
	}
}

