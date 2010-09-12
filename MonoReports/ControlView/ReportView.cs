// 
// ReportView.cs
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
using MonoReports.Core;
using MonoReports.Model;
using System.Collections.Generic;
using MonoReports.Model.Controls;
namespace MonoReports.ControlView
{
	public class ReportView
	{
		
		ControlViewFactory controlViewFactory;
		public Report Report {get;set;}
		
		public ReportView (Report report)
		{
			Report = report;
			controlViewFactory = new ControlViewFactory (this);
			sectionViews = new List<SectionView> ();
			addSectionView (report.PageHeaderSection);
			foreach (var groupHeader in report.GroupHeaderSections) {
				addSectionView (groupHeader);
			}
			addSectionView (report.DetailSection);
			foreach (var groupFooter in report.GroupFooterSections) {
				addSectionView (groupFooter);
			}
			addSectionView (report.PageFooterSection);
		}
		
		private List<SectionView> sectionViews;

		public IList<SectionView> SectionViews {
			get { return sectionViews; }
			private set {
				;
			}
		}
		
		
		 
		
		
		private void addSectionView (Section section)
		{
			Cairo.PointD sectionSpan;
			if (sectionViews.Count > 0) {
				var previousSection = sectionViews[sectionViews.Count - 1];
				sectionSpan = new Cairo.PointD (0, previousSection.AbsoluteBound.Y + previousSection.AbsoluteBound.Height);
			} else {
				sectionSpan = new Cairo.PointD (0, 0);
			}
			var sectionView = new SectionView (Report, controlViewFactory, section, sectionSpan);
			sectionViews.Add (sectionView);
		}
		
		
		 
	}
}

