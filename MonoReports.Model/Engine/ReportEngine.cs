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
		double spaceLeftOnCurrentPage = 0;
		double currentY = 0;
		List<int> groupColumnIndeces = null;
		List<string> groupCurrentKey = null;
		Section currentSection = null;
		Section lastFooterSection = null;
		List<SpanInfo> currentSectionSpans = null;
		List<Control> currentSectionOrderedControls = null;
        List<Control> currentSectionControlsBuffer = null;
		List<Line> currentSectionExtendedLines = null;
		int currentSectionControlIndex = 0;

		bool IsSubreport {get; set;}

		bool stop = false;

		public ReportEngine (Report report,IReportRenderer renderer)
		{			
			Report = report;
			source = Report._dataSource;
			ReportRenderer = renderer;
			currentSectionSpans = new List<SpanInfo> ();
			currentSectionOrderedControls = new List<Control> ();
			currentSectionExtendedLines = new List<Line> ();
            currentSectionControlsBuffer = new List<Control>();
            ReportContext = new ReportContext { CurrentPageIndex = 0, DataSource = null, Parameters = new Dictionary<string, string>(), ReportMode = ReportMode.Preview };
            Report.Pages = new List<Page>();

            if (Report.ReportHeaderSection.IsVisible)
            {
                selectCurrentSectionByTemplateSection( Report.ReportHeaderSection);
            }
            else if (!IsSubreport && Report.PageHeaderSection.IsVisible)
            {
                selectCurrentSectionByTemplateSection(  Report.PageHeaderSection);
            }
            else
            {
                 selectCurrentSectionByTemplateSection(  Report.DetailSection);
            }
		}

		public void Process ()
		{
            while (!ProcessReportUpToHeightTreshold(Report.Height))
                ;
			onAfterReportProcess ();
		}
          

        public bool ProcessReportUpToHeightTreshold(double treshold)
        {
            bool result = processSectionUpToHeightTreshold(treshold);
            if (result) {
                ;//TODO 3tk
            }
            else {

            }
            return result;
        }

		void init ()
		{
			
						
		}

		T selectCurrentSectionByTemplateSection<T> (T s) where T:Section
		{
			var newSection = s.CreateControl () as T;
			currentSectionSpans.Clear ();			
			newSection.Format ();
			newSection.Location = new Point (s.Location.X, currentY);				
			currentSection = newSection;
			currentSectionExtendedLines = new List<Line> ();
			currentSectionOrderedControls = currentSection.Controls.OrderBy (ctrl => ctrl.Location.Y).ToList ();
            currentSectionControlsBuffer = new List<Control>();
			currentSectionControlIndex = 0;
			return newSection;
		}

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

	/*	void addSection(Section s, double height){
			
				
				if (height > spaceLeftOnCurrentPage) {
					
						if (s is DetailSection){
							lastFooterSection.Location = new  Point(lastFooterSection.Location.X, lastFooterSection.Location.Y - spaceLeftOnCurrentPage);
							lastFooterSection.Size = new Size(lastFooterSection.Width,lastFooterSection.Height + spaceLeftOnCurrentPage);
						}
					if(s.KeepTogether){
						newPage ();
					}
				}
				s.Location = new Point (s.Location.X, currentY);
				s.Size = new Size (s.Size.Width, height);
				currentPage.Controls.Add (s);
				spaceLeftOnCurrentPage -= height;
				currentY += height;						
		}
	*/
		
		void addControl (Control c)
		{ 
			double height = c.Height;			 
			currentPage.Controls.Add (c);
			spaceLeftOnCurrentPage -= height;
			currentY += height;	
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
		bool processSectionUpToHeightTreshold (double heightTreshold)
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
				
                if(source != null)
				    control.AssignValue (source);
					
					
				y = control.Top + span;

                
				var controlSize = ReportRenderer.MeasureControl(control);
					
					
				foreach (SpanInfo item in currentSectionSpans) {
					if (y > item.Treshold) {
						tmpSpan = Math.Max (tmpSpan, item.Span);
					}
				}
					
				span = tmpSpan;
				ungrowedControlBottom = control.Bottom + span;
				marginBottom = Math.Min (marginBottom, currentSection.Height - control.Bottom);

                if (control.Bottom + span <= heightTreshold)
                {
                    currentSectionControlsBuffer.Add (control);
                }
                else
                {
                    
                    currentSectionControlIndex = i;
                    
                    if (control.Top > heightTreshold)
                        return false;

                    if (control is IResizable)
                    {
                        if ((control as IResizable).KeepTogether)
                        {
                            return false;
                        }
                        else
                        {
                          currentSectionControlsBuffer.AddRange ( control.SplitControlAt(heightTreshold));
                        }

                    }
                }

					
				control.MoveControlByY (span);
				control.Size = controlSize;								
				maxControlBottom = Math.Max (maxControlBottom, control.Bottom);

                if (maxHeight <= control.Bottom)
                {
                    maxHeight = control.Bottom;
				}						
				currentSectionSpans.Add (
                    new SpanInfo { 
                        Treshold = ungrowedControlBottom,
                        Span = span + control.Bottom - ungrowedControlBottom 
                    });
			}
				
				
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
			
			/*
			double currentSectionHight = 0;
			
			if (!currentSection.CanGrow)
				currentSectionHight = currentSection.Height;
			else {
				currentSectionHight =  (maxControlBottom + marginBottom);
			}
			*/
			
			return true;
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

