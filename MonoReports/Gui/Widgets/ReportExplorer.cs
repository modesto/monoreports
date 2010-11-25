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
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Mono.CSharp;
using MonoReports.Services;
using Gdk;
using Gtk;
using MonoReports.Model.Data;

namespace MonoReports.Gui.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ReportExplorer : Gtk.Bin
	{
		Gtk.TreeStore theModel;
		
		Gtk.TreeIter reportNode;
		Gtk.TreeIter staticDataFieldsNode;
		Gtk.TreeIter dataFieldsNode;
		Gtk.TreeIter groupsNode;
		Gtk.TreeIter parametersNode;
		Gtk.TreeIter imagesNode;
		DesignService designService;

		public DesignService DesignService {

			get { return designService; }

				
			set { 	
				
				if (designService != null) {
					designService.OnReportChanged -= HandleDesignServiceOnReportChanged;	
					designService.OnReportDataFieldsRefreshed -= HandleDesignServiceOnReportDataFieldsRefreshed;
				}
				
				designService = value; 
				
				if (designService != null) {
					designService.OnReportChanged += HandleDesignServiceOnReportChanged;				
					designService.OnReportDataFieldsRefreshed += HandleDesignServiceOnReportDataFieldsRefreshed;
				}
			}
		}

		void HandleDesignServiceOnReportDataFieldsRefreshed (object sender, EventArgs e)
		{
			updateFieldTree ();
		}
 
		void HandleDesignServiceOnReportChanged (object sender, EventArgs e)
		{
			updateFieldTree ();
			updateParameterTree ();
			updateGroupTree ();
		}

		public IWorkspaceService Workspace {get; set;}

		public ReportExplorer ()
		{
			this.Build ();
			
			Gtk.TreeViewColumn objectColumn = new Gtk.TreeViewColumn ();
			objectColumn.Title = "Report";
			Gtk.CellRendererText cell = new Gtk.CellRendererText ();
			objectColumn.PackStart (cell, true);
			
			exporerTreeview.AppendColumn (objectColumn);
			objectColumn.AddAttribute (cell, "text", 0);
			theModel = new Gtk.TreeStore (typeof(string));	
			exporerTreeview.Model = theModel;
			
			reportNode =  theModel.AppendValues("Report");
			staticDataFieldsNode = theModel.AppendValues (reportNode,"Static Fields");
			theModel.AppendValues (staticDataFieldsNode, "#PageNumber");
			theModel.AppendValues (staticDataFieldsNode, "#NumberOfPages");
			theModel.AppendValues (staticDataFieldsNode, "#RowNumber");
			
			
			dataFieldsNode = theModel.AppendValues (reportNode,"Data Fields");
			
			parametersNode = theModel.AppendValues (reportNode,"Parameters");
			
			groupsNode = theModel.AppendValues (reportNode,"Groups");
			
			imagesNode = theModel.AppendValues (reportNode,"Images");
			
			exporerTreeview.Selection.Changed += HandleExporerTreeviewSelectionChanged;

			Gtk.Drag.SourceSet (exporerTreeview, 
				ModifierType.Button1Mask, 
				new TargetEntry[]{new TargetEntry ("Field", TargetFlags.OtherWidget,2)}, 
			DragAction.Copy);
			
			exporerTreeview.RowActivated += HandleExporerTreeviewRowActivated;
			exporerTreeview.ExpandAll();
		}

		void HandleExporerTreeviewRowActivated (object o, RowActivatedArgs args)
		{
			
			//Indices [0] = Data Fields
			//if (args.Path.Indices [0] == 1 && args.Path.Depth == 2) {
			//	var field = DesignService.Report.Fields [args.Path.Indices [1]];										
			//}
		}

		void HandleExporerTreeviewSelectionChanged (object sender, EventArgs e)
		{
			TreeIter item;
			exporerTreeview.Selection.GetSelected (out item);
					
			if (item.UserData == staticDataFieldsNode.UserData || item.UserData == dataFieldsNode.UserData || item.UserData == parametersNode.UserData) {
				Gtk.Drag.SourceSet (
								exporerTreeview, ModifierType.None, new TargetEntry[]{new TargetEntry ("Field", TargetFlags.OtherWidget,2)}, 
							DragAction.Copy);
			} else {
				Gtk.Drag.SourceSet (
								exporerTreeview, ModifierType.Button1Mask, new TargetEntry[]{new TargetEntry ("Field", TargetFlags.OtherWidget,2)}, 
							DragAction.Copy);
			}
				
		}

		void updateFieldTree ()
		{
			TreeIter item;
			if (theModel.IterChildren (out item, dataFieldsNode)) {
				int depth = theModel.IterDepth (dataFieldsNode);
	
				while (theModel.Remove (ref item) && 
					theModel.IterDepth (item) > depth)
					;
			}
				
			foreach (var field in designService.Report.Fields) {
				theModel.AppendValues (dataFieldsNode, field.Name);
			}
			
			exporerTreeview.ExpandRow(theModel.GetPath(dataFieldsNode),true);
		}

		void updateParameterTree ()
		{
			TreeIter item;
			if (theModel.IterChildren (out item, parametersNode)) {
				int depth = theModel.IterDepth (parametersNode);
	
				while (theModel.Remove (ref item) && 
					theModel.IterDepth (item) > depth)
					;
			}
				
			foreach (var parameter in designService.Report.Parameters) {
				theModel.AppendValues (parametersNode, parameter.Name);
			}
			exporerTreeview.ExpandRow(theModel.GetPath(parametersNode),true);				
		}

		void updateGroupTree ()
		{
			TreeIter item;
			if (theModel.IterChildren (out item, groupsNode)) {
				int depth = theModel.IterDepth (groupsNode);
	
				while (theModel.Remove (ref item) && 
					theModel.IterDepth (item) > depth)
					;
			}
				
			foreach (var gr in designService.Report.Groups) {
				theModel.AppendValues (groupsNode, gr.GroupingFieldName);
			}
				
		}
		
		void updateImageTree ()
		{
			TreeIter item;
			if (theModel.IterChildren (out item, imagesNode)) {
				int depth = theModel.IterDepth (imagesNode);
	
				while (theModel.Remove (ref item) && 
					theModel.IterDepth (item) > depth)
					;
			}
			int i=0;
			foreach (var image in designService.Report.ResourceRepository) {
				 
				theModel.AppendValues (imagesNode, i);
				i++;
			}
			exporerTreeview.ExpandRow(theModel.GetPath(imagesNode),true);				
		}

		protected virtual void OnUpdateFieldsFromDataSourceButtonButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			
		}

		protected virtual void OnUpdateFieldsFromDataSourceButtonActivated (object sender, System.EventArgs e)
		{
			
		}

		protected virtual void OnUpdateFieldsFromDataSourceButtonClicked (object sender, System.EventArgs e)
		{
			
		}

		[GLib.ConnectBefore]
		protected virtual void OnExporerTreeviewButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			TreePath path;
			exporerTreeview.GetPathAtPos ((int)args.Event.X, (int)args.Event.Y, out path);
			if (path != null) {
				if (args.Event.Button == 3) { 					
					Gtk.MenuItem addNewMenuItem = null;
					if(path.Depth > 1 ) {
					int index = path.Indices [1];
					if ((index == 2 || index == 1) && path.Depth == 2) {
						Gtk.Menu jBox = new Gtk.Menu ();
						if (index == 1) {
							addNewMenuItem = new MenuItem ("add field");
								
						} else {
							addNewMenuItem = new MenuItem ("add parameter");								
						}
						jBox.Add (addNewMenuItem);		
								
						addNewMenuItem.Activated += delegate(object sender, EventArgs e) {					
							PropertyFieldEditor pfe = new PropertyFieldEditor ();
							pfe.Response += delegate(object oo, ResponseArgs argss) {						
								if (argss.ResponseId == ResponseType.Ok) {
									if (index == 1){
										DesignService.Report.Fields.Add (new PropertyDataField (){ Name = pfe.PropertyName});
										updateFieldTree ();
									}else {
										DesignService.Report.Parameters.Add (new PropertyDataField (){ Name = pfe.PropertyName, DefaultValue = pfe.DefaultValue });
										updateParameterTree ();
									}
									
									pfe.Destroy ();
									
								}else {
									pfe.Destroy ();
								}
							};
							pfe.Show ();
						}; 
						
						jBox.ShowAll ();
						jBox.Popup ();	
					}else if (index == 4 && path.Depth == 2) {
						 Gtk.Menu jBox = new Gtk.Menu ();
						 
							addNewMenuItem = new MenuItem ("add images");
					}
				} 
	
				} else if ( args.Event.Button == 1 ) {
					
					if (path.Depth == 3) {
						int index = path.Indices [1];
						
						if (index == 1) {
							Workspace.ShowInPropertyGrid( designService.Report.Fields[path.Indices[2]]);
						} else if (index == 2) {
							Workspace.ShowInPropertyGrid( designService.Report.Parameters[path.Indices[2]]);
						}
					}
				}
	
				
			}
		

		}
	}

}