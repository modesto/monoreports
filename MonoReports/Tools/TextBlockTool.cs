// 
// TextBlockTool.cs
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
using MonoReports.Services;
using MonoReports.Gui.Widgets;
using MonoReports.Model.Controls;
using MonoReports.ControlView;

namespace MonoReports.Tools
{
	public class TextBlockTool : RectTool
	{
		
		public override string Name {
			get { return "TextBlockTool"; }
		}
				
		
		public override string ToolbarImageName {
			get {
				return "ToolText.png";
			}
		}
		
		public override bool IsToolbarTool {
			get {
				return true;
			}
		}
		

		public TextBlockTool (DesignService designService) :base(designService)
		{
		}

		public override void CreateNewControl (SectionView sectionView)
		{				
			var startPoint = sectionView.PointInSectionByAbsolutePoint (designService.StartPressPoint.X, designService.StartPressPoint.Y);
			
			var tb = new TextBlock { Location = new MonoReports.Model.Controls.Point (startPoint.X, startPoint.Y), Text="text", FontName="Helvetica", FontSize=12, Size = new Size(50,20) };				
			TextBlockView textBlockView = sectionView.CreateControlView (tb) as TextBlockView;			
			sectionView.Section.Controls.Add (tb);				
			textBlockView.ParentSection = sectionView;
			designService.SelectedControl = textBlockView;				
				
		}

	}
}

