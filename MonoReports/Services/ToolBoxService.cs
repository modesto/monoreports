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

namespace MonoReports.Services
{
	public class ToolBoxService : IToolBoxService
	{
		DesignService designService;
		
		public DesignService DesignService {
			get {
				return this.designService;
			}
			set {
				designService = value;
			}
		}

	BaseTool selectedTool;
		public BaseTool SelectedTool{get { return selectedTool; } 
			private set { selectedTool = value;
				if(designService != null)
					designService.SelectedTool = selectedTool;
			}}
		
		public Dictionary<string,Func<BaseTool>> ToolByNameFactory;
		
		
		
		public ToolBoxService(DesignService designService){
			
			this.designService = designService;
			this.designService.OnSelectedControlChanged += handleSelectedControlChange;
			
		}
		
		
		
		void handleSelectedControlChange(object sender, EventArgs args){
			SetToolByControlView(designService.SelectedControl);
		}
		
		public ToolBoxService ()
		{
			ToolByNameFactory = new Dictionary<string, Func<BaseTool>>();
			
			ToolByNameFactory.Add("LineTool", () => { return new LineTool(designService);  });
			ToolByNameFactory.Add("CrossSectionLineTool", () => { return new CrossSectionLineTool(designService);  });
			 
		}
 
		
		public void UnselectTool(){
			SelectedTool = null;
		}
		
		#region IToolBoxService implementation
 
	
		public void SetToolByControlView(ControlViewBase control){
			if(control is TextBlockView){
				SelectedTool = new RectTool(designService);				
			}else if(control is CrossSectionLineView){
				SelectedTool = new CrossSectionLineTool(designService);
			}else if(control is LineView){
				SelectedTool = new LineTool(designService);
			}else if(control is ImageView){
				SelectedTool = new RectTool(designService);
			}else if(control is SectionView){
				SelectedTool = new SectionTool(designService);
			}else if(control is CrossSectionLineView){
				SelectedTool = new CrossSectionLineTool(designService);
			}
			
		}
		
		public void SetTool (BaseTool tool)
		{
			SelectedTool = tool;
		}

		public void SetToolByName (string toolName)
		{
			DesignService.SelectedControl = null;
			SelectedTool = ToolByNameFactory[toolName]();
			
			SelectedTool.CreateMode = true;
		}
		#endregion
	}
}

