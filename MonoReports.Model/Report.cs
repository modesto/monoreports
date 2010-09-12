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

namespace MonoReports.Model
{


	public class Report
	{

		public Report ()
		{		
			Width = 600;
			Height = 800;
			Groups = new List<Group>();
			GroupHeaderSections = new List<GroupHeaderSection>();
			GroupFooterSections = new List<GroupFooterSection>();
			Pages = new List<Page>();
			ResourceRepository = new List<byte[]>();
			PageHeaderSection  = new Controls.PageHeaderSection { Location = new Controls.Point (0, 0), Size = new Controls.Size (600, 150) };
			DetailSection = new Controls.DetailSection { Location = new Controls.Point (0, 150), Size = new Controls.Size (600, 150) };
			PageFooterSection = new Controls.PageFooterSection { Location = new Controls.Point (0, 300), Size = new Controls.Size (600, 100) };
		}
		
		public string Title {get;set;}
		
		public PageHeaderSection PageHeaderSection {get;set;}
		public PageFooterSection PageFooterSection {get;set;}
		public DetailSection DetailSection {get; internal set;}
		public List<GroupHeaderSection> GroupHeaderSections {get;set;}
		public List<GroupFooterSection> GroupFooterSections {get;set;}		
		public List<Page> Pages {get; internal set;}				
		public List<Group> Groups {get; internal set;}		
		public List<byte[]> ResourceRepository {get; set;}
		public double Height {get;set;}
		public double Width {get;set;}
		public UnitType Unit {get;set;}
 
		public void AddGroup(string fieldName){
			Group group = new Group(){ GroupingFieldName = fieldName};
			Groups.Add(group);			
			GroupHeaderSection gh = new GroupHeaderSection(){ Name = "Group header " + group.GroupingFieldName, Size = new Controls.Size (600, 20), Location = new Controls.Point (0, 150)};
			GroupHeaderSections.Add(gh);							
			GroupFooterSection gf = new GroupFooterSection(){ Name = "Group footer " + group.GroupingFieldName, Size = new Controls.Size (600, 20), Location = new Controls.Point (0, 250)};
			GroupFooterSections.Add(gf);
		}
		
		public void RemoveGroup(Group group){
			int index = Groups.IndexOf(group);
			if(index != -1){
				Groups.RemoveAt(index);
				GroupHeaderSections.RemoveAt(index);
				GroupFooterSections.RemoveAt(index);
			}
		}
	}
}
