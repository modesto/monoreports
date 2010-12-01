// 
// SubReport.cs
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
using MonoReports.Model.Engine;

namespace MonoReports.Model.Controls
{
	public class SubReport : Control, IResizable
	{
		public SubReport ()
		{
			
			Report = new Report();
            CanGrow = true;
			Report.Height = 35;
            Width = 100;
            Report.PageHeaderSection.IsVisible = false;
            Report.PageFooterSection.IsVisible = false;
			Report.PageHeaderSection.Height = 0;

			Report.PageFooterSection.Height = 0;
            Report.ReportHeaderSection.BackgroundColor = new Color(0.2, 0.8, 0.4);
            Report.ReportFooterSection.BackgroundColor = new Color(0.8, 0.2, 0.7);
            Report.PageFooterSection.BackgroundColor =   new Color(0.8, 0.2, 0.2);
            Report.PageHeaderSection.BackgroundColor = new Color(0.1, 0.3, 0.2);
			Report.ReportHeaderSection.Controls.Add (new Controls.TextBlock { FontSize = 12, FontName = "Helvetica", 
			Text = "11text random text random text 08Random text 33 Random text random WW text random _text Random text random text r 44 andom text Random text random text random text ZZZZ", FontColor = new Color(1,0,0), Location = new Point (0, 10), CanGrow = true, Size = new Model.Size (79, 30) });
			engine = new ReportEngine (this.Report,null) { 
			IsSubreport = true  
			};
		}
 
		public Report Report {
			get;
			set;
		}
		
		public bool CanGrow {get;set;}		
		
		public bool CanShrink {get;set;}		

        public bool KeepTogether {
            get;
            set;
        }

        public override double Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                Report.Width = value;
            }
        }
		
	
		public override Control CreateControl ()
		{
			var subreport = new SubReport();			
			CopyBasicProperties(subreport);
			subreport.CanGrow = CanGrow;
			subreport.CanShrink = CanShrink;           
			subreport.Report = Report;			 
			
			return subreport;
		}
		
		ReportEngine engine;
		
		public ReportEngine Engine {
			get {
				return this.engine;
			}
			set {
				engine = value;
			}
		}

        public bool Finished { get; set; }

		public void ProcessUpToPage(IReportRenderer renderer, double height){
			
			engine.ReportRenderer = renderer;
			engine.context.HeightLeftOnCurrentPage = height;
            Finished = engine.ProcessReportPage();					
			 
		}
 
		
    }
}

