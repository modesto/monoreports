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
using MonoReports.Services;
using Gdk;
using Gtk;
using MonoReports.Model.Data;
using System.Linq;

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
			updateTreeNode(dataFieldsNode,designService.Report.DataFields); 
			updateTreeNode(parametersNode,designService.Report.Parameters); 
		}
 
		void HandleDesignServiceOnReportChanged (object sender, EventArgs e)
		{			
			updateTreeNode(dataFieldsNode,designService.Report.DataFields); 
			updateTreeNode(parametersNode,designService.Report.Parameters); 
			updateTreeNode(groupsNode,designService.Report.Groups); 
			updateTreeNode(imagesNode,designService.Report.ResourceRepository); 
		}

		public IWorkspaceService Workspace {get; set;}

		public ReportExplorer ()
		{
			this.Build ();
		   	var reportCellRenderer = new Gtk.CellRendererText ();
			var reportColumn = exporerTreeview.AppendColumn ("Report", reportCellRenderer);
			reportColumn.SetCellDataFunc(reportCellRenderer,new Gtk.TreeCellDataFunc (renderReportCell));
			theModel = new Gtk.TreeStore (typeof(TreeItemWrapper));	
			exporerTreeview.Model = theModel;
			
			
			
			reportNode =  theModel.AppendValues(new TreeItemWrapper("Report"));
			parametersNode = theModel.AppendValues (reportNode,new TreeItemWrapper("Parameters"));
			dataFieldsNode = theModel.AppendValues (reportNode,new TreeItemWrapper("Data"));
			staticDataFieldsNode = theModel.AppendValues (reportNode,new TreeItemWrapper("Expressions"));
			
			var pageNumberField = MonoReports.Model.Data.FieldBuilder.CreateFields(0,"#PageNumber",FieldKind.Expression).Single();
			pageNumberField.Name = "#PageNumber";
			var numberOfPagesField = MonoReports.Model.Data.FieldBuilder.CreateFields(0,"#NumberOfPages",FieldKind.Expression).Single();
			numberOfPagesField.Name = "#NumberOfPages";
			var rowNumberField = MonoReports.Model.Data.FieldBuilder.CreateFields(0,"#RowNumber",FieldKind.Expression).Single();
			rowNumberField.Name = "#RowNumber";
			
			theModel.AppendValues (staticDataFieldsNode, new TreeItemWrapper(pageNumberField));
			theModel.AppendValues (staticDataFieldsNode, new TreeItemWrapper(numberOfPagesField));
			theModel.AppendValues (staticDataFieldsNode, new TreeItemWrapper(rowNumberField));		
			groupsNode = theModel.AppendValues (reportNode,new TreeItemWrapper("Groups"));
			imagesNode = theModel.AppendValues (reportNode,new TreeItemWrapper("Images"));
			exporerTreeview.Selection.Changed += HandleExporerTreeviewSelectionChanged;

			Gtk.Drag.SourceSet (exporerTreeview, 
				ModifierType.Button1Mask, 
				new TargetEntry[]{new TargetEntry ("Field", TargetFlags.OtherWidget,2)}, 
			DragAction.Copy);
			
			exporerTreeview.RowActivated += HandleExporerTreeviewRowActivated;
			exporerTreeview.ExpandAll();
		}
		
		//3tk needs to be cleaned
		void HandleExporerTreeviewRowActivated (object o, RowActivatedArgs args)
		{
			
			//Indices [0] = Data Fields
			//if (args.Path.Indices [0] == 1 && args.Path.Depth == 2) {
			//	var field = DesignService.Report.Fields [args.Path.Indices [1]];										
			//}
		}

		void HandleExporerTreeviewSelectionChanged (object sender, EventArgs e)
		{
//			TreeIter item;
//			exporerTreeview.Selection.GetSelected (out item);
//			var path = theModel.GetPath(item);
//		    if(path.Depth == 3) {
//				if (path.Indices[1] == 1) {
//					Gtk.Drag.SourceSet (
//								exporerTreeview, ModifierType.None, new TargetEntry[]{new TargetEntry ("Field", TargetFlags.OtherWidget,2)}, 
//							DragAction.Copy);
//				}
//			}
				
		}

		void updateTreeNode<T> (TreeIter theNode , IEnumerable<T> objects)
		{
			TreeIter item;
			if (theModel.IterChildren (out item, theNode)) {
				int depth = theModel.IterDepth (theNode);
	
				while (theModel.Remove (ref item) && 
					theModel.IterDepth (item) > depth)
					;
			}
			int i=0;
			foreach (T o in objects) {
				 
				theModel.AppendValues (theNode, new TreeItemWrapper(o));
				i++;
			}
			exporerTreeview.ExpandRow(theModel.GetPath(theNode),true);				
		}
		
		private void renderReportCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
		TreeItemWrapper w = (TreeItemWrapper) model.GetValue (iter, 0);
 		
//		if (song.Artist.StartsWith ("X") == true) {
//			(cell as Gtk.CellRendererText).Foreground = "red";
//		} else {
//			(cell as Gtk.CellRendererText).Foreground = "darkgreen";
//		}
 
		(cell as Gtk.CellRendererText).Text = w.ToString();
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
					    /*TODO 3tk - at the moment user added datafields are disabled
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
										//DesignService.Report.Fields.Add (new PropertyDataField (){ Name = pfe.PropertyName});
										updateTreeNode(dataFieldsNode,designService.Report.DataFields); 
			
									}else {
										//DesignService.Report.Parameters.Add (new PropertyDataField< (){ Name = pfe.PropertyName, DefaultValue = pfe.DefaultValue });
										updateTreeNode(parametersNode,designService.Report.Parameters); 
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
						*/
					}else if (index == 4 && path.Depth == 2) {
						 Gtk.Menu jBox = new Gtk.Menu ();
						 
							addNewMenuItem = new MenuItem ("add image");
							jBox.Add (addNewMenuItem);
							addNewMenuItem.Activated += delegate(object sender, EventArgs e) {
								
								
								Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose the Monoreports file to open",null, FileChooserAction.Open , "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
								var fileFilter = new FileFilter { Name = "Images" };
								fileFilter.AddPattern ("*.jpg");
								fileFilter.AddPattern ("*.png");
								fileFilter.AddPattern ("*.gif");
								fileFilter.AddPattern ("*.JPG");
								fileFilter.AddPattern ("*.PNG");
								fileFilter.AddPattern ("*.GIF");				
								fc.AddFilter (fileFilter);
		
								if (fc.Run () == (int)ResponseType.Accept) {
									System.IO.FileStream file = System.IO.File.OpenRead (fc.Filename);
								 
									byte[] bytes = new byte[file.Length];
									file.Read (bytes, 0, (int)file.Length);
									string fileName = System.IO.Path.GetFileName(fc.Filename);
									designService.Report.ResourceRepository.Add(fileName, bytes);
									file.Close ();
								}
		
								fc.Destroy ();																								
								updateTreeNode(imagesNode,designService.Report.ResourceRepository); 
							};
						jBox.ShowAll ();
						jBox.Popup ();	
					}
				} 
	
				} else if ( args.Event.Button == 1 ) {
					
					if (path.Depth == 3) {
						int index = path.Indices [1];
						
						if (index == 1) {
							Workspace.ShowInPropertyGrid( designService.Report.DataFields[path.Indices[2]]);
						} else if (index == 0) {
							Workspace.ShowInPropertyGrid( designService.Report.Parameters[path.Indices[2]]);
						}
					}
				}
	
				
			}
		

		}
	}
	
	public class TreeItemWrapper {
		
		public TreeItemWrapper () {
		}
		
		public TreeItemWrapper (object obj) {
			this.obj = obj;
		}
		
		object obj;
		public object Object {
			get { return obj; }
			set { obj = value; }
		}
		
		public override string ToString ()
		{
			 return obj.ToString();
		}
	}

}