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
using MonoReports.Renderers;

namespace MonoReports.ControlView
{
	public class SubreportView : ControlViewBase
	{
		
		
		public override Control ControlModel {
			get { return base.ControlModel; }
			set {
				base.ControlModel = value;
				if (subreport != value) {
					subreport = value as SubReport;	
				}
			}
		}
		
		public SubreportRenderer SubreportRenderer {get;set;}
		
		SubReport subreport;
		
		public Model.Controls.SubReport SubReport {
			get { return subreport; }
		}

		public SubreportView (MonoReports.Model.Controls.SubReport subreport, SectionView parentSection): base(subreport)
		{			
			this.subreport = subreport;
			this.ParentSection = parentSection;
			AbsoluteBound = new Rectangle (parentSection.AbsoluteDrawingStartPoint.X + subreport.Location.X, parentSection.AbsoluteDrawingStartPoint.Y + subreport.Location.Y, subreport.Width, subreport.Height);			
			SubreportRenderer = new SubreportRenderer(){  DesignMode = true};
		}

		
		
		public override string DefaultToolName {
			get {
				return "SubreportTool";
			}
		}

		public override void Render (Context c)
		{
			SubreportRenderer.Render(c,subreport);
			AbsoluteBound = new Rectangle (ParentSection.AbsoluteDrawingStartPoint.X + subreport.Location.X ,
			                               ParentSection.AbsoluteDrawingStartPoint.Y + subreport.Location.Y, subreport.Width, subreport.Height);
			 
		}


		public override bool ContainsPoint (double x, double y)
		{
			return AbsoluteBound.ContainsPoint (x, y);
		}
		
		
	}
}

