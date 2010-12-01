// 
// ReportContext.cs
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
using System.Collections.Generic;
using MonoReports.Model.Data;
using MonoReports.Model.Controls;

namespace MonoReports.Model
{
	public class ReportContext
	{
		public ReportContext ()
		{
		}
		
		public Dictionary<string,string> Parameters {get;set;}
		
		public IDataSource DataSource {get;set;}
				
		
		public int CurrentPageIndex {
			get;
			set;
		}
		
		public int RowIndex {
			get;
			set;
		}
 
		public ReportMode ReportMode {get;set;}
		
		double heightLeftOnCurrentPage;
		public double HeightLeftOnCurrentPage {
			get { 
				return heightLeftOnCurrentPage;
			}
			internal set { heightLeftOnCurrentPage = value; }}



     

        double heightUsedOnCurrentPage;
        public double HeightUsedOnCurrentPage
        {
            get
            {
                return heightUsedOnCurrentPage;
            }
            internal set { heightUsedOnCurrentPage = value; }
        }

	}
}

