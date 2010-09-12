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
		List<List<Control>> crossSectionControlsRepository;
		int crossSectionRepositoryIndex = 0;
		Page currentPage = null;
		double spaceLeftOnCurrentPage = 0;
		double currentY = 0;

		List<int> groupColumnIndeces = null;
		List<string> groupCurrentKey = null;

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


		void init ()
		{
			ReportContext = new ReportContext { CurrentPageIndex = 0, DataSource = null, Parameters = new Dictionary<string, string> (), ReportMode = ReportMode.Preview };
			Report.Pages = new List<Page> ();
			crossSectionControlsRepository = new List<List<Control>> ();
			var headerCrossControls = Report.PageHeaderSection.GetCrossSectionControls (Report.PageFooterSection);
			crossSectionControlsRepository.Add (new List<Control> (headerCrossControls));
			
			for (int i = 0; i < Report.GroupHeaderSections.Count; i++) {
				GroupHeaderSection hs = Report.GroupHeaderSections[i];
				var groupHeaderCrossControls = hs.GetCrossSectionControls (Report.GroupFooterSections[i]);
				crossSectionControlsRepository.Add (new List<Control> (groupHeaderCrossControls));
			}
			
		}

		public void Process ()
		{
			
			init ();
			newPage ();
			
			processDetails ();
			
			onAfterReportProcess ();
		}

		void processPageHeader ()
		{
			var headerSection = Report.PageHeaderSection.Clone () as PageHeaderSection;
			headerSection.Format ();
			headerSection.Location = new Point (headerSection.Location.X, currentY);
			double height = processSection (headerSection, new DataRow ());
			headerSection.Size = new Size (headerSection.Size.Width, height);
			currentY += height;
			
			
			spaceLeftOnCurrentPage -= currentY;
			
			
			currentPage.Sections.Add (headerSection);
		}



		void processGroupHeader (int groupIndex)
		{
			var groupHeaderSection = Report.GroupHeaderSections[groupIndex].Clone () as Section;
			double gh = processSection (groupHeaderSection, new DataRow ());
			processCrossSectionControls(groupHeaderSection);
			addSection(groupHeaderSection,gh);
			crossSectionRepositoryIndex++;
		}
		
		
		void processGroupFooter (int groupIndex)
		{
			var groupFooterSection = Report.GroupFooterSections[groupIndex].Clone () as Section;
			double gh = processSection (groupFooterSection, new DataRow ());		
			processCrossSectionControls(groupFooterSection);
			addSection(groupFooterSection,gh);
			crossSectionRepositoryIndex--;
		}
		
		
		void processCrossSectionControls(Section section){
			for (int w = 0; w <= crossSectionRepositoryIndex; w++) {
					foreach (var crossSectionControl in crossSectionControlsRepository[w]) {
						section.Controls.Add (crossSectionControl.Clone () as Control);
					}
			}
		}


		void processDetails ()
		{
			
			
			IDataSource dataSource = (source as IDataSource);
			groupColumnIndeces = new List<int> ();
			for (int i = 0; i < Report.Groups.Count; i++) {
				var col = dataSource.Columns.FirstOrDefault (cl => cl.Name == Report.Groups[i].GroupingFieldName);
				if (col != null)
					groupColumnIndeces.Add (dataSource.Columns.IndexOf (col));
				else {
					groupColumnIndeces.Add (-1);
				}
			}
			
			
			var rowsAll = dataSource.GetRows ();
			
			if (Report.Groups.Count > 0) {
				groupCurrentKey = new List<string> ();
				
				bool isFirstOrdering = true;
				IOrderedEnumerable<DataRow> orderedRows = null;
				for (int i = 0; i < Report.Groups.Count; i++) {
					
					groupCurrentKey.Add (String.Empty);
					if (groupColumnIndeces[i] != -1) {
						
						if (isFirstOrdering) {
							orderedRows = rowsAll.OrderBy (r => r.Values[groupColumnIndeces[i]]);
							isFirstOrdering = false;
						} else {
							orderedRows = orderedRows.ThenBy (r => r.Values[groupColumnIndeces[i]]);
						}
					}
					
				}
				
				if (orderedRows != null) {
					rowsAll = orderedRows.ToList ();
				}
				
				
			}
			
			var rows = rowsAll;
			
			for (int j = 0; j < rows.Count; j++) {
				
				var row = rows[j];
				
				if (j == 0) {
					processGroupHeader(0);
				}
				
				for (int g = 0; g < groupColumnIndeces.Count; g++) {
					if (groupColumnIndeces[g] != -1 && groupCurrentKey[g] != row[groupColumnIndeces[g]]) {
						processGroupHeader(g);
					}
				}
				
				var detailSection = Report.DetailSection.Clone () as DetailSection;
				
				detailSection.Format ();
				double height = processSection (detailSection, row);
				processCrossSectionControls(detailSection);
				addSection(detailSection,height);
 				 if(j == rows.Count - 1){
					processGroupFooter(0);
				}
			 
			}
			
			
			
		}
		
	
		//Section previousSection = null;
		
		void addSection(Section s, double height){
			
				if (height > spaceLeftOnCurrentPage) {
					//if (previousSection != null)
					//	previousSection.Size = new Size (s.Size.Width, previousSection.Height + spaceLeftOnCurrentPage);
					newPage ();
				}
				s.Location = new Point (s.Location.X, currentY);
				s.Size = new Size (s.Size.Width, height);
				currentPage.Sections.Add (s);
				//previousSection = s;
				spaceLeftOnCurrentPage -= height;
				currentY += height;						
		}
		 

		void processPageFooter ()
		{
			double footerSectionsHeight = 0;
			var footerSection = Report.PageFooterSection.Clone () as PageFooterSection;
			footerSection.Format ();
			
			double height = processSection (footerSection, new DataRow ());
			
				foreach (var crossSectionControl in crossSectionControlsRepository[0]) {
					footerSection.Controls.Add (crossSectionControl.Clone () as Control);
				}
		
			
			footerSection.Size = new Size (footerSection.Size.Width, height);
			footerSectionsHeight += height;
			double currentFooterY = 0;
			footerSection.Location = new Point (footerSection.Location.X, (Report.Height - footerSectionsHeight) + currentFooterY);
			currentFooterY += footerSection.Height;
			
			spaceLeftOnCurrentPage -= footerSectionsHeight;
						
			currentPage.Sections.Add (footerSection);
		}



		double processSection (Section section, DataRow currentRow)
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
					
					
					control.AssignValue (source, currentRow);
					
					
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
			}
			currentY = 0;
			ReportContext.CurrentPageIndex++;
			currentPage = new Page { PageNumber = ReportContext.CurrentPageIndex };
			spaceLeftOnCurrentPage = Report.Height;
			Report.Pages.Add (currentPage);			
			processPageHeader ();
			processPageFooter ();
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



	static internal class SectionExtensions
	{

		public static IEnumerable<Control> GetCrossSectionControls (this Section section, Section endSection)
		{
			
			foreach (var c in section.Controls.Where (ctrl => ctrl is ICrossSectionControl)) {
				
				ICrossSectionControl csc = c as ICrossSectionControl;
				csc.StartSection = section;
				csc.EndSection = endSection;
				yield return c;
			}
		}
		
	}
}

