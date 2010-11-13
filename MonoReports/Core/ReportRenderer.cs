// 
// ReportRenderer.cs
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
using MonoReports.Model.Controls;
using MonoReports.ControlView;
using System.Collections.Generic;
using MonoReports.Model;
using MonoReports.Services;
using Cairo;

namespace MonoReports.Core
{
	public class ReportRenderer : IReportRenderer
	{
		Dictionary<Type, object> renderersDictionary;
		DesignService designService;		
		
		public void RegisterRenderer(Type t,IControlRenderer renderer){
			renderersDictionary.Add(t,renderer);
		}
		
		public ReportRenderer (DesignService designService)
		{
			this.designService = designService;
			renderersDictionary = new Dictionary<Type, object>();
		} 

		public void RenderPage (Page p)
		{			 			
			for (int i = 0; i < p.Controls.Count; i++) {
				var control = p.Controls[i];
 					if(control.IsVisible)
						RenderControl (control);				 				
			}
			 
		}
 
		public Size MeasureControl (Control control)
		{
            Type controlType = control.GetType();
			if(renderersDictionary.ContainsKey(controlType)){
				var renderer = renderersDictionary[controlType] as IControlRenderer;
                
				return renderer.Measure(designService.CurrentContext,control);								
			}
			return default(Size);
 
		}

        public void RenderControl(Control control )
		{
            Type controlType = control.GetType();
			if(renderersDictionary.ContainsKey(controlType)){
				var renderer = renderersDictionary[controlType] as IControlRenderer;							
				renderer.Render(designService.CurrentContext,control);								
			}
 
		}

		
		
	}
}

