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
			
			
			Evaluator.MessageOutput = Console.Out;
			
			Evaluator.Init (new string[0]);
			AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoaded;
			
			// Add all currently loaded assemblies
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies ()) {
				try {
					Evaluator.ReferenceAssembly (a);
				} catch (Exception exp) {
					Console.WriteLine (exp.ToString ());
				}
			}
			
			Evaluate ("using System; using System.Linq; using System.Collections.Generic; using System.Collections;");
			
			
		}




		StringWriter outputWriter = new StringWriter ();
		DesignService designService;

		public DesignService DesignService {

			get { return designService; }

				
			set { designService = value; }
		}

		static void AssemblyLoaded (object sender, AssemblyLoadEventArgs e)
		{
			
			Evaluator.ReferenceAssembly (e.LoadedAssembly);
			
		}



		void updateTree ()
		{
			
			
			var model = new Gtk.TreeStore (typeof(string));
			
			exporerTreeview.Model = model;
			
			var staticDataFieldsNode = model.AppendValues ("Static Fields");
			model.AppendValues (staticDataFieldsNode, "PageNumber");
			model.AppendValues (staticDataFieldsNode, "NumberOfPages");
			
			var dataFieldsNode = model.AppendValues ("Data Fields");
			foreach (var field in designService.Report.Fields) {
				model.AppendValues (dataFieldsNode, field.Name);
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
			
		}

		protected virtual string Evaluate (string input)
		{
			bool result_set;
			object result;
			
			try {
				input = Evaluator.Evaluate (input, out result, out result_set);
				
				if (result_set) {
					
					outputTextview.Buffer.Text = result.ToString ();
					DesignService.Report.DataSource = result;
					updateTree ();
				}
			} catch (Exception e) {
				Console.WriteLine (e);
				return null;
			}
			
			return input;
		}


		protected virtual void OnExecScriptButtonClicked (object sender, System.EventArgs e)
		{
			
			
			object returnVal = null;
			string script = scriptTextView.Buffer.Text;
			Evaluate (script);
 
		}
		
		
		
		
		
		
	}
}

