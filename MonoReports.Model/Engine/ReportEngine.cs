// 
// ReportEngine.cs
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
using System.Linq;
using MonoReports.Model;
using System.Collections.Generic;
using MonoReports.Model.Controls;
using System.Collections;
using MonoReports.Model.Data;

namespace MonoReports.Model.Engine
{
	public class ReportEngine
	{

		IReportRenderer ReportRenderer;
		Report Report;
		IDataSource source;
		ReportContext ReportContext;

		public ReportEngine (Report report, IReportRenderer renderer)
		{
			source = new DummyDataSource ();
			Report = report;
			ReportRenderer = renderer;
		}

		private object datasource;

		public object DataSource {
			get { return datasource; }
			set {
				
				if (value != datasource) {
					datasource = value;
					
					if (datasource != null) {
						Type type = datasource.GetType ();
						//FIXME handle anonymous types
						if (type.IsGenericType && datasource is IEnumerable) {
							
							var args = type.GetGenericArguments ();
							var genArgType = args[0];
							
							var pocoSourceType = typeof(GenericEnumerableDataSource<>);
							
							var genType = pocoSourceType.MakeGenericType (genArgType);
							try {
								source = Activator.CreateInstance (genType, datasource) as IDataSource;
							} catch (Exception exp) {
								Console.WriteLine (exp.ToString ());
							}
						}
						
						
					}
					
				}
			}
		}

		private void onBeforeReportProcess ()
		{
			//todo exec Report event			
		}

		Page currentPage = null;
		double spaceLeftOnCurrentPage = 0;
		PageHeaderSection rheaderSection = null;
		List<DetailSection> detailSections = null;
		PageFooterSection rfooterSection = null;
		double currentY = 0;

		public void Process ()
		{
			
			ReportContext = new ReportContext { CurrentPageIndex = 0, DataSource = null, Parameters = new Dictionary<string, string> (), ReportMode = ReportMode.Preview };
			//ReportRenderer.ReportContext = ReportContext;
			rheaderSection = null;
			detailSections = new List<DetailSection> ();
			rfooterSection = null;
			Report.Pages.Clear ();
			newPage ();
			
			for (int i = 0; i < Report.Sections.Count; i++) {
				var section = Report.Sections[i];
				
				switch (section.GetType ().Name) {
				
				case "PageHeaderSection":
					rheaderSection = section as PageHeaderSection;
					break;
				case "DetailSection":
					detailSections.Add (section as DetailSection);
					break;
				case "PageFooterSection":
					rfooterSection = section as PageFooterSection;
					break;
				default:
					throw new Exception ("Unknown section type");
				}
				
			}
			
			processHeader ();
			processFooter ();
			processDetails ();
			
			onAfterReportProcess ();
		}

		void processHeader ()
		{
			
			var headerSection = rheaderSection.Clone() as PageHeaderSection;
			headerSection.Format ();
			headerSection.Location = new Point (headerSection.Location.X, currentY);
			double height = processSection (headerSection, null);
			headerSection.Size = new Size (headerSection.Size.Width, height);
			currentY += height;
			currentPage.Sections.Add (headerSection);
			
			
			spaceLeftOnCurrentPage -= currentY;
		}

		void processDetails ()
		{
			
			var enumerator = source.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				
				for (int i = 0; i < detailSections.Count; i++) {
					var detailSection = detailSections[i].Clone () as DetailSection;
					detailSection.Format ();
					
					double height = processSection (detailSection, enumerator.Current);
					
					if (height > spaceLeftOnCurrentPage) {
						
						newPage ();
						processHeader ();
					}
					detailSection.Location = new Point (detailSection.Location.X, currentY);
					detailSection.Size = new Size (detailSection.Size.Width, height);
					currentPage.Sections.Add (detailSection);
					spaceLeftOnCurrentPage -= height;
					currentY += height;
				}
				
			}
		}

		public Section footerSectionOnCurrentPage = null;

		void processFooter ()
		{
			double footerSectionsHeight = 0;
			footerSectionOnCurrentPage = null;
			
			var footerSection = rfooterSection.Clone() as PageFooterSection;
			footerSection.Format ();
			double height = processSection (footerSection, null);
			footerSection.Size = new Size (footerSection.Size.Width, height);
			footerSectionOnCurrentPage = footerSection;
			footerSectionsHeight += height;
			
			
			
			
			double currentFooterY = 0;
			
			footerSectionOnCurrentPage.Location = new Point (footerSectionOnCurrentPage.Location.X, (Report.Height - footerSectionsHeight) + currentFooterY);
			currentFooterY += footerSection.Height;
			
			
			spaceLeftOnCurrentPage -= footerSectionsHeight;
		}



		double processSection (Section section, object current)
		{
			
			var orderedControls = section.Controls.OrderBy (ctrl => ctrl.Location.Y).ToList ();
			double span = 0;
			double y = 0;
			double maxHeight = 0;
			double marginBottom = 0;
			double maxControlBottom = 0;
			double controlBottomY = 0;
			double tmpSpan = 0;
			double ungrowedControlBottom = 0;
			
			
			List<SpanInfo> spans = new List<SpanInfo> ();
			
			
			if (orderedControls.Count > 0) {
				marginBottom = double.MaxValue;
				
				foreach (var control in orderedControls) {
					controlBottomY = 0;
					tmpSpan = 0;
					ungrowedControlBottom = 0;
					
					
					if (current != null && control is TextBlock) {
						TextBlock tb = control as TextBlock;
						if (!string.IsNullOrEmpty (tb.FieldName)) {
							tb.Text = source.GetValue (tb.FieldName, current);
						}
					}
					
					y = control.Location.Y + span;
					var controlSize = ReportRenderer.MeasureControl (control);
					
					
					foreach (SpanInfo item in spans) {
						if (y > item.Treshold) {
							tmpSpan = Math.Max (tmpSpan, item.Span);
						}
					}
					
					span = tmpSpan;
					ungrowedControlBottom = control.Location.Y + span + control.Height;
					marginBottom = Math.Min (marginBottom, section.Height - (control.Location.Y + control.Height));
					
					
					control.Location = new Point (control.Location.X, span + control.Location.Y);
					
					if (control.CanGrow) {
						control.Size = controlSize;
					}
					
					controlBottomY = control.Location.Y + controlSize.Height;
					maxControlBottom = Math.Max (maxControlBottom, controlBottomY);
					
					if (maxHeight <= controlBottomY) {
						maxHeight = controlBottomY;
					}
					
					
					spans.Add (new SpanInfo { Treshold = ungrowedControlBottom, Span = span + controlBottomY - ungrowedControlBottom });
					
				}
				
			}
			
			if (!section.CanGrow)
				return section.Height;
			else {
				return (maxControlBottom + marginBottom);
			}
			
			
			
		}

		void newPage ()
		{
			
			if (ReportContext.CurrentPageIndex > 0) {
				ReportRenderer.NextPage ();
				currentPage.Sections.Add(footerSectionOnCurrentPage);
			}
			currentY = 0;
			ReportContext.CurrentPageIndex++;
			currentPage = new Page { PageNumber = ReportContext.CurrentPageIndex };
			spaceLeftOnCurrentPage = Report.Height;
			Report.Pages.Add (currentPage);
		}


		private void onAfterReportProcess ()
		{
			//todo exec Report event
			
		}
	}

	internal struct SpanInfo
	{
		internal double Treshold;
		internal double Span;
	}
}

