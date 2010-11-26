// 
// ToolBoxService.cs
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
using MonoReports.Services;
using System.Collections.Generic;
using MonoReports.Tools;
using MonoReports.Gui.Widgets;

namespace MonoReports.Services
{
	public class ToolBoxService : IToolBoxService
	{
		 

		BaseTool selectedTool;
		public BaseTool SelectedTool {
			
			get { return selectedTool; } 
			set { selectedTool = value; }
		}
		
		public Dictionary<string,BaseTool> ToolDictionary;
		
		public ToolBoxService(){

			ToolDictionary = new Dictionary<string, BaseTool>();
			
		}

		public void BuildToolBar(Gtk.Toolbar mainToolbar){
			
			foreach (var tool in ToolDictionary.Values) {				
				 tool.BuildToolbar(mainToolbar);				
			}			
		}
		
		public void AddTool(BaseTool tool){
			ToolDictionary.Add(tool.Name,tool);
		}
 
		
		public void UnselectTool(){
			SelectedTool = null;
		}
		
		#region IToolBoxService implementation
 
	
		public void SetToolByControlView(ControlViewBase control){
		 	if(ToolDictionary.ContainsKey(control.DefaultToolName))
				SelectedTool = ToolDictionary[control.DefaultToolName];
		}
		
		public void SetTool (BaseTool tool)
		{
			SelectedTool = tool;
		}

		public void SetToolByName (string toolName)
		{			 
			SelectedTool = ToolDictionary[toolName]; 
			SelectedTool.CreateMode = true;
		}
		#endregion
	}
}

