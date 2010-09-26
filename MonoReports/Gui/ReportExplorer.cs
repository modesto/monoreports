// 
// ReportExplorer.cs
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
namespace MonoReports.Gui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ReportExplorer : Gtk.Bin
	{
		public ReportExplorer ()
		{
			this.Build ();
			
		Gtk.TreeViewColumn objectColumn = new Gtk.TreeViewColumn ();
			objectColumn.Title = "Report";
			Gtk.CellRendererText cell = new Gtk.CellRendererText ();
		    objectColumn.PackStart (cell, true);
			
			exporerTreeview.AppendColumn (objectColumn);
			objectColumn.AddAttribute (cell, "text", 0);
			
		}
		
		DesignView designView;
		
		public DesignView DesignView {
			
			get{
				return designView;
			}
			
			set{			
				designView = value;				
				refreshDataFieldsFromDatasource ();
			}
		}
		
		
		void refreshDataFieldsFromDatasource () {
			 
				if (designView != null)
					updateTree ();
		}
		
		void updateTree (){
 
			
			var model = new Gtk.TreeStore(typeof(string));
			
			exporerTreeview.Model = model;
			
			var staticDataFieldsNode = model.AppendValues("Static Fields");
			model.AppendValues(staticDataFieldsNode,"PageNumber");
			model.AppendValues(staticDataFieldsNode,"NumberOfPages");
			
			var dataFieldsNode = model.AppendValues("Data Fields");
			foreach (var field in designView.ReportView.Report.Fields) {
					model.AppendValues(dataFieldsNode,field.Name);
			}
		
		 
		}
		
		 
		protected virtual void OnUpdateFieldsFromDataSourceButtonButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
		  	
		}
		
		protected virtual void OnUpdateFieldsFromDataSourceButtonActivated (object sender, System.EventArgs e)
		{
		
		}
		
		protected virtual void OnUpdateFieldsFromDataSourceButtonClicked (object sender, System.EventArgs e)
		{
				refreshDataFieldsFromDatasource ();
		}
		
		
		
		
		
		
	}
}

