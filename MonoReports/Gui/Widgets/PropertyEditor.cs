// 
// PropertyEditor.cs
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
using System.Collections.Generic;
using System.Reflection;

namespace MonoReports.Gui.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class PropertyEditor : Gtk.Bin
	{
		public PropertyEditor ()
		{
			this.Build ();
		}
		
		List<IPropertyEditorRow> PropertyRows {get; set;}
 	 
		 
		object currentObject;
		public object CurrentObject {
			get { return currentObject; }
			set { 
				
				if(currentObject != value){
					currentObject = value;
				}							
			}
		}
		
		void addRow(IPropertyEditorRow row){
			PropertyRows.Add(row);
		}
	}
	
	
	 
	
	public interface IPropertyEditorRow 
	{		
		string DisplayName {get;}
		object Value {get;set;}		
		 	
	}
			
	public delegate void EditorValueChanged(object sender,EventArgs args);
		
	
}

