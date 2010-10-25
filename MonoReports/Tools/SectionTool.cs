// 
// SelectAndResizeTool.cs
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
using Cairo;
using MonoReports.Model;
using MonoReports.Core;
using MonoReports.Extensions.CairoExtensions;
using MonoReports.Model.Controls;
using MonoReports.Services;

namespace MonoReports.Tools
{
	public class SectionTool : BaseTool
	{
		SectionView currentSection = null;
			
		public SectionTool (DesignService designService) : base(designService)
		{
			
		}

		public override void OnBeforeDraw (Context c)
		{
			
			if (designService.IsPressed) {			
				 
				if (designService.IsMoving && designService.SelectedControl != null) {
					
					var section = designService.SelectedControl as SectionView;														
					section.ControlModel.Size = new Size(section.ControlModel.Width,section.ControlModel.Height + designService.DeltaPoint.Y);
										
				} 
				
			}
		}
		
		public override string Name {get {return "SectionTool"; }}
		
			public override bool IsToolbarTool {
			get {
				return false;
			}
		}
 
		public override void OnAfterDraw (Context c)
		{
			if(currentSection != null)
				c.FillRectangle (currentSection.GripperAbsoluteBound, currentSection.SectionGripperColor);
		}
		

		public override void OnMouseDown ()
		{
			currentSection = designService.SelectedControl as SectionView;
		}
	 
		public override void OnMouseUp ()
		{			 
			 
			double y = 0;
			SectionView previousSection = null;
			
			foreach (var sectionView in designService.SectionViews) {
				
				if(y > 0){
					sectionView.ControlModel.Location = new MonoReports.Model.Controls.Point(sectionView.ControlModel.Location.X,y);						
					sectionView.SectionSpan =  new Cairo.PointD (sectionView.ControlModel.Location.X, previousSection.AbsoluteBound.Y + previousSection.AbsoluteBound.Height);
					sectionView.InvalidateBound();
					y+=sectionView.ControlModel.Size.Height;
					
				}else{
					y  = sectionView.ControlModel.Location.Y + sectionView.ControlModel.Size.Height;										
				}
				previousSection = sectionView;
			}
			
		}
		
	}
}

