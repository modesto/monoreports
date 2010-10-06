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

namespace MonoReports.Core
{
	public class ReportRenderer : IReportRenderer
	{
		Dictionary<Control, ControlViewBase> controlRenderersDictionary;
		DesignService designService;		
		RenderState renderState;
		
		public ReportRenderer (DesignService designService)
		{
			this.designService = designService;
			
			controlRenderersDictionary = new Dictionary<Control, ControlViewBase> ();
			
			foreach (var sectionView in designService.SectionViews) {
				controlRenderersDictionary.Add (sectionView.ControlModel, sectionView);
				foreach (var controlView in sectionView.Controls) {
					if(!controlRenderersDictionary.ContainsKey(controlView.ControlModel))
						controlRenderersDictionary.Add (controlView.ControlModel, controlView);
				}
			}
		}



		public void RenderPage ( Page p)
		{
			renderState = new RenderState(){ IsDesign = false, Render = true };
			for (int i = 0; i < p.Sections.Count; i++) {
				var section = p.Sections[i];
				var sectionView = controlRenderersDictionary[section.TemplateControl] as SectionView;			 				
			 	renderState.SectionView = sectionView;	
				renderState.Section  = section;
				designService.CurrentContext.Save();	
				RenderControl (section);
				designService.CurrentContext.Translate(0,section.Location.Y);
 
				for (int j = 0; j < section.Controls.Count; j++) {
					var ctrl = section.Controls[j];
					if(ctrl.IsVisible)
						RenderControl (ctrl);
				}
				designService.CurrentContext.Restore();
			}
		}

		#region IReportRenderer implementation
		public Size MeasureControl (Control control)
		{
			ControlViewBase controlView = null;
			try {
				controlView = controlRenderersDictionary[control.TemplateControl];			 				
			} catch (Exception exp) {
				throw new Exception ("No template control found", exp);
			}
			controlView.ControlModel = control;
			RenderState renderState = new RenderState(){ IsDesign = false, Render = false,
				SectionView = controlView.ParentSection };
			var result = controlView.Render (designService.CurrentContext, renderState);
			controlView.ControlModel = control.TemplateControl;
			return result;
		}

		public void RenderControl (Control control)
		{
			ControlViewBase controlView = null;
			
			try {
				controlView = controlRenderersDictionary[control.TemplateControl];
			} catch (Exception exp) {
				throw new Exception ("No template control found", exp);
			}
			controlView.ControlModel = control;
			
			controlView.Render (designService.CurrentContext, renderState);
			controlView.ControlModel = control.TemplateControl;
		}

		public void NextPage ()
		{
			designService.NextPage ();
		}

		
  
		#endregion
 
		
	}
}

