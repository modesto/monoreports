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
using System.Collections.Generic;

namespace MonoReports.Tools
{
	public class ToolBoxService : IToolBoxService
	{
		DesignView designView;
		
		public DesignView DesignView {
			get {
				return this.designView;
			}
			set {
				designView = value;
			}
		}

		public BaseTool SelectedTool{get; private set;}
		
		public Dictionary<string,Func<BaseTool>> ToolByNameFactory;
		
		public ToolBoxService ()
		{
			ToolByNameFactory = new Dictionary<string, Func<BaseTool>>();
			
			ToolByNameFactory.Add("LineTool", () => { return new LineTool(designView);  });
			ToolByNameFactory.Add("CrossSectionLineTool", () => { return new CrossSectionLineTool(designView);  });
			ToolByNameFactory.Add("CrossSectionLineTool1", () => { return new CrossSectionLineTool(designView);  });
		}
 
		
		public void UnselectTool(){
			SelectedTool = null;
		}
		
		#region IToolBoxService implementation
 
	
		public void SetToolByControlView(ControlViewBase control){
			if(control is TextBlockView){
				SelectedTool = new RectTool(designView);				
			}else if(control is CrossSectionLineView){
				SelectedTool = new CrossSectionLineTool(designView);
			}else if(control is LineView){
				SelectedTool = new LineTool(designView);
			}else if(control is ImageView){
				SelectedTool = new RectTool(designView);
			}else if(control is SectionView){
				SelectedTool = new SectionTool(designView);
			}else if(control is CrossSectionLineView){
				SelectedTool = new CrossSectionLineTool(designView);
			}
			
		}
		
		public void SetTool (BaseTool tool)
		{
			SelectedTool = tool;
		}

		public void SetToolByName (string toolName)
		{
			DesignView.SelectedControl = null;
			SelectedTool = ToolByNameFactory[toolName]();
			
			SelectedTool.CreateMode = true;
		}
		#endregion
	}
}

