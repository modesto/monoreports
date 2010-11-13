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
		Page currentPage = null;
		double heightLeftOnCurrentPage = 0;
		double heightUsedOnCurrentPage = 0;
		double currentY = 0;
	    int currentGroupIndex = -1;
		Section currentSection = null;		
		List<SpanInfo> currentSectionSpans = null;
		List<Control> currentSectionOrderedControls = null;
		List<Control> currentSectionControlsBuffer = null;
		List<Control> currentPageFooterSectionControlsBuffer = null;
		List<Line> currentSectionExtendedLines = null;
		int currentSectionControlIndex = 0;
		double spanCorrection = 0;
		bool IsSubreport {get; set;}
		bool dataSourceHasNextRow = true;
		bool stop = false;

		public ReportEngine (Report report, IReportRenderer renderer)
		{			
			Report = report;
			source = Report._dataSource;	
			if(source == null)
				source = new DummyDataSource();
			ReportRenderer = renderer;
			currentSectionSpans = new List<SpanInfo> ();
			currentSectionOrderedControls = new List<Control> ();
			currentSectionExtendedLines = new List<Line> ();
			currentSectionControlsBuffer = new List<Control> ();
			currentPageFooterSectionControlsBuffer = new List<Control>();
			ReportContext = new ReportContext { CurrentPageIndex = 0, DataSource = null, Parameters = new Dictionary<string, string>(), ReportMode = ReportMode.Preview };
			Report.Pages = new List<Page> ();			
			nextPage();			
			selectCurrentSectionByTemplateSection(Report.ReportHeaderSection);
		}

		public void Process ()
		{				
			while (!ProcessReportPage ()){
				nextPage();
			}				
			onAfterReportProcess ();
		}

		


		T selectCurrentSectionByTemplateSection<T> (T s) where T:Section
		{
			var newSection = s.CreateControl () as T;
			currentSectionSpans.Clear ();			
			newSection.Format ();
			newSection.Location = new Point (s.Location.X, currentY);				
			currentSection = newSection;
			currentSectionExtendedLines.Clear();
			currentSectionOrderedControls = currentSection.Controls.OrderBy (ctrl => ctrl.Top).ToList ();
			currentSectionControlsBuffer.Clear();
			
			currentSectionControlIndex = 0;
			return newSection;
		}
		
		#region old processing details code
		 /*
		void processDetails ()
		{
			
			groupColumnIndeces = new List<int> ();
			for (int i = 0; i < Report.Groups.Count; i++) {
				var col = Report.Fields.FirstOrDefault (cl => cl.Name == Report.Groups [i].GroupingFieldName);
				
				if (col != null)
					groupColumnIndeces.Add (Report.Fields.IndexOf (col)); else {
					groupColumnIndeces.Add (-1);
				}
			}
			
			IDataSource dataSource = (source as IDataSource);
			List<string> sorting = new List<string> ();
			
			if (Report.Groups.Count > 0) {
					
				groupCurrentKey = new List<string> ();
				for (int i = 0; i < Report.Groups.Count; i++) {
					sorting.Add (Report.Groups [i].GroupingFieldName);
					groupCurrentKey.Add (String.Empty);
				}
	
			}
			
			if (dataSource == null)
				dataSource = new DummyDataSource ();
			
			dataSource.ApplySort (sorting);
			
				
			while (dataSource.MoveNext ()) {
	
				for (int g = 0; g < Report.Groups.Count; g++) {
					var currentGroup = Report.Groups [g];
					if (!string.IsNullOrEmpty (currentGroup.GroupingFieldName)) {
						string newKey = dataSource.GetValue (currentGroup.GroupingFieldName, String.Empty);
						if (groupCurrentKey [g] != newKey) {	
									
							if (dataSource.CurrentRowIndex > 0) {
								//processGroupFooter(g);
							}
							groupCurrentKey [g] = newKey;
							//processGroupHeader(g);
							
						}
					} else {
						if (dataSource.IsLast) {
							//processGroupFooter(g);
						}
							
						if (dataSource.CurrentRowIndex == 0) {
							//processGroupHeader(g);
						}
					}
				}
				
				//var detailSection = Report.DetailSection. 
				
				//detailSection.Format ();
				
				//processCrossSectionControls(detailSection);
				//add controls
				
			}
				
			for (int i = Report.Groups.Count - 1; i >= 0; i--) {
					//processGroupFooter(i);	 
			}
			
		}
		*/
		#endregion
		
		
		
		public bool ProcessReportPage ()
		{
			bool result = false;		
			stop = false;
			
			do {
				
				result = processSectionUpToHeightTreshold (heightLeftOnCurrentPage);						
				addControlsToCurrentPage (heightUsedOnCurrentPage);
				
				heightLeftOnCurrentPage -= currentSection.Height;
				heightUsedOnCurrentPage += currentSection.Height;
				
				if (result) {
					nextSection();
				} else {
					return false;
				}
			} while (!stop);
				
			return result;
		}
	
		/// <summary>
		/// Processes the section up to heightTreshold.
		/// </summary>
		/// <returns>
		///  returns <c>true</c> if finished processig section and <c>false</c> while not
		/// </returns>
		/// <param name='pageBreakTreshold'>
		/// maxiumum height (starting from current section Location.Y) after which page will break
		/// </param>
		bool processSectionUpToHeightTreshold ( double heightTreshold)
		{
			double span = 0;
			double y = 0;
			double maxHeight = 0;
			double marginBottom = 0;
			double maxControlBottom = 0;			
			double tmpSpan = 0;
			double ungrowedControlBottom = 0;
			marginBottom = double.MaxValue;
		
			for (int i = currentSectionControlIndex; i <  currentSectionOrderedControls.Count; i++) {
					
				var control = currentSectionOrderedControls [i];				        
				tmpSpan = 0;
				ungrowedControlBottom = 0;
				if (!control.IsVisible)
					continue;
				if (control is Line && (control as Line).ExtendToBottom) {
					currentSectionExtendedLines.Add (control as Line);	
				}
				
				if (source != null)
					control.AssignValue (source);
					
					
				y = control.Top + span;

				
				var controlSize = ReportRenderer.MeasureControl (control);
					
					
				foreach (SpanInfo item in currentSectionSpans) {
					if (y > item.Treshold) {
						tmpSpan = Math.Max (tmpSpan, item.Span);
					}
				}
					
				span = tmpSpan;
				ungrowedControlBottom = control.Bottom + span;
				marginBottom = Math.Min (marginBottom, currentSection.Height - control.Bottom);

				if (control.Bottom + span <= heightTreshold) {
					currentSectionControlsBuffer.Add (control);
					
				} else {
				
					currentSectionControlIndex = i;
				
					if (control.Top > heightTreshold)
						return false;

					if (control is IResizable) {
						if ((control as IResizable).KeepTogether) {
							return false;
						} else {
							currentSectionControlsBuffer.AddRange (control.SplitControlAt (heightTreshold));
						}

					}else{
						return false;
					}
				}

				control.MoveControlByY (span);
				control.Size = controlSize;								
				maxControlBottom = Math.Max (maxControlBottom, control.Bottom);
   			
				if (maxHeight <= control.Bottom) {
					maxHeight = control.Bottom;
				}						
				currentSectionSpans.Add (
				new SpanInfo { 
					Treshold = ungrowedControlBottom,
					Span = span + control.Bottom - ungrowedControlBottom 
				});
			}
				
			
			var heighWithMargin =  maxControlBottom + marginBottom;
			
			if (!currentSection.CanGrow && !currentSection.CanShrink || !currentSection.CanShrink && heighWithMargin < currentSection.Height) {
				;
			}
			else if(heighWithMargin <= heightTreshold){
				currentSection.Height = heighWithMargin;
			}
 
			/* 3tk TODO extending line should be done in smarter way
			 * e.g. handling page break
			 * */
			foreach (Line lineItem in currentSectionExtendedLines) {
				if (lineItem.Location.Y == lineItem.End.Y) {
					lineItem.Location = new Point (lineItem.Location.X,maxControlBottom + marginBottom - lineItem.LineWidth / 2);
					lineItem.End = new Point (lineItem.End.X,maxControlBottom + marginBottom - lineItem.LineWidth / 2);
				} else if (lineItem.Location.Y > lineItem.End.Y) {
					lineItem.Location = new Point (lineItem.Location.X,maxControlBottom + marginBottom );
				} else {
					lineItem.End = new Point (lineItem.End.X,maxControlBottom + marginBottom);
				}
			}
			
			
			return true;
		}
	
		 
		
		void nextRecord(){			
	 		dataSourceHasNextRow = source.MoveNext();					
		}
		
		void nextSection(){
		
				switch (currentSection.SectionType) {
				
					case SectionType.ReportHeader:	
						if (Report.ReportHeaderSection.BreakPageAfter)
							nextPage();				
						nextRecord();
						selectCurrentSectionByTemplateSection(Report.PageHeaderSection);
						break;
					case SectionType.PageHeader:
					
						selectCurrentSectionByTemplateSection(Report.PageFooterSection);					
						break;
					case SectionType.PageFooter:
				
						if (Report.Groups.Count > 0) {
							currentGroupIndex = 0;						
							selectCurrentSectionByTemplateSection(Report.GroupHeaderSections[currentGroupIndex]);					
						} else {
							selectCurrentSectionByTemplateSection(Report.DetailSection);		
						}
						break;
					case SectionType.GroupHeader:				
					
						if(currentGroupIndex < Report.Groups.Count -1) {
							currentGroupIndex++;
							selectCurrentSectionByTemplateSection(Report.GroupHeaderSections[currentGroupIndex]);	
						} else {
							selectCurrentSectionByTemplateSection(Report.DetailSection);
						}								
						break;
				
					case SectionType.Details:
						if (dataSourceHasNextRow) {
							nextRecord();
							selectCurrentSectionByTemplateSection(Report.DetailSection);
						} else {
							selectCurrentSectionByTemplateSection(Report.ReportFooterSection);
						}
						break;
				
					case SectionType.GroupFooter:
				
				
						break;
				
					case SectionType.ReportFooter:
						addControlsToCurrentPage(Report.Height - Report.PageFooterSection.Height,currentPageFooterSectionControlsBuffer);					
						stop = true;
						break;
					default:
						break;
				}
		
			if(!currentSection.IsVisible)
					nextSection();
		}

		void addControlsToCurrentPage (double span)
		{		
			if(currentSection.SectionType != SectionType.PageFooter){ 
				addControlsToCurrentPage(span + spanCorrection,currentSectionControlsBuffer);
			}else{
				currentPageFooterSectionControlsBuffer.AddRange(currentSectionControlsBuffer);
				spanCorrection -= currentSection.Height;
			}
			currentSectionControlsBuffer.Clear ();					
		}

	 
		void addControlsToCurrentPage(double span, List<Control> controls) {
			foreach (var control in controls ) {
					control.MoveControlByY(span);
					currentPage.Controls.Add(control);
			}
		}

		void nextPage ()
		{
			addControlsToCurrentPage(Report.Height - Report.PageFooterSection.Height,currentPageFooterSectionControlsBuffer);	
			spanCorrection = 0;
			currentY = 0;
			ReportContext.CurrentPageIndex++;
			currentPage = new Page { PageNumber = ReportContext.CurrentPageIndex };
			heightLeftOnCurrentPage = Report.Height;
			heightUsedOnCurrentPage = 0;			
			currentPageFooterSectionControlsBuffer.Clear();			
			Report.Pages.Add (currentPage);		
			selectCurrentSectionByTemplateSection(Report.PageHeaderSection);
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

