// 
// Report.cs
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
using System;
using System.Collections.Generic;
using MonoReports.Model.Controls;

using System.Collections;
using MonoReports.Model.Data;

namespace MonoReports.Model
{
	public class Report
	{

		public Report ()
		{
			Width = 600;
			Height = 800;
			Groups = new List<Group> ();
			Fields = new List<DataField> ();
			Parameters = new List<DataField> ();
			GroupHeaderSections = new List<GroupHeaderSection> ();
			GroupFooterSections = new List<GroupFooterSection> ();
			Pages = new List<Page> ();
			ResourceRepository = new List<byte[]> ();
			ReportHeaderSection = new Controls.ReportHeaderSection { Location = new Point (0, 0), Size = new Model.Size (Width, 150) };
			PageHeaderSection = new Controls.PageHeaderSection { Location = new Point (0, 0), Size = new Model.Size (Width, 30) };
			DetailSection = new Controls.DetailSection { Location = new Point (0, 150), Size = new Model.Size (Width, 150) };
			PageFooterSection = new Controls.PageFooterSection { Location = new Point (0, 300), Size = new Model.Size (Width, 30) };
			ReportFooterSection = new Controls.ReportFooterSection { Location = new Point (0, 300), Size = new Model.Size (Width, 30) };
		}

		public string Title { get; set; }
		
		public string DataScript {get;set;}

		public PageHeaderSection PageHeaderSection { get; set; }

		public PageFooterSection PageFooterSection { get; set; }
		
		public ReportHeaderSection ReportHeaderSection { get; set; }

		public ReportFooterSection ReportFooterSection { get; set; }

		public DetailSection DetailSection { get; internal set; }

		public List<GroupHeaderSection> GroupHeaderSections { get; set; }

		public List<GroupFooterSection> GroupFooterSections { get; set; }

		public List<Page> Pages { get; internal set; }

		public List<Group> Groups { get; internal set; }

		public List<byte[]> ResourceRepository { get; set; }

		public double Height { get; set; }

		public double Width { get; set; }

		public UnitType Unit { get; set; }

		public void AddGroup (string fieldName)
		{
			Group gr = new Group { GroupingFieldName = fieldName };
			Groups.Add (gr);
			GroupHeaderSection gh = new GroupHeaderSection { Name = "Group header " + gr.GroupingFieldName, Size = new Model.Size (Width, 20), Location = new Point (0, 150) };
			GroupHeaderSections.Add (gh);
			GroupFooterSection gf = new GroupFooterSection { Name = "Group footer " + gr.GroupingFieldName, Size = new Model.Size (Width, 20), Location = new Point (0, 250) };
			GroupFooterSections.Add (gf);
		}

		public void RemoveGroup (Group gr)
		{
			int index = Groups.IndexOf (gr);
			if (index != -1) {
				Groups.RemoveAt (index);
				GroupHeaderSections.RemoveAt (index);
				GroupFooterSections.RemoveAt (index);
			}
		}

		object dataSource;

		public object DataSource { 
			get { return dataSource; } 
			set {
				dataSource = value;
				if (dataSource != null) {
					Type r = dataSource.GetType ();
					Type r2 = r.GetElementType ();
					if (r2 == null) {
						Type t = r.GetGenericArguments () [0];
						Type genericType = typeof(ObjectDataSource<>); 
						var ttt = genericType.MakeGenericType (new Type[]{t});
						_dataSource = (Activator.CreateInstance (ttt, dataSource))  as IDataSource;
					} else {
					
						Type genericType = typeof(ObjectDataSource<>); 
						var ttt = genericType.MakeGenericType (new Type[]{r2});
						_dataSource = (Activator.CreateInstance (ttt, dataSource))  as IDataSource;
					}
				}
				FillFieldsFromDataSource ();
			} 
		
		}

		internal IDataSource _dataSource {get; set;}

		public List<DataField> Fields { get; private set; }

		public List<DataField> Parameters { get; private set; }

		public void FillFieldsFromDataSource ()
		{
			Fields = new List<DataField> ();				
			if (DataSource != null) {
				Fields.AddRange( _dataSource.DiscoverFields ());				 
			}
		}

 
	}
}
