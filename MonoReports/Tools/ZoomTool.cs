// 
// ZoomTool.cs
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
using MonoReports.Gui.Widgets;
namespace MonoReports.Tools
{
	public class ZoomTool : BaseTool
	{
		public ZoomTool(DesignService designService) : base(designService)
		{
			
		}
		
		public override string Name {get {return "ZoomTool"; }}
		
		public override bool IsToolbarTool {
			get {
				return true;
			}
		}

		
		public override void BuildToolbar (Gtk.Toolbar toolBar)
		{
			
			ToolBarComboBox zoomCombobox = new ToolBarComboBox (100, 4, true, new string[] { "400%", "300%", "200%", "150%", "100%", "66%", "50%" });
			zoomCombobox.ComboBox.Changed += delegate(object sender, EventArgs e) {			
				string text = zoomCombobox.ComboBox.ActiveText;			
				text = text.Trim ('%');
			
				double percent;
			
				if (!double.TryParse (text, out percent))
					return;
				percent = Math.Min (percent, 400);
				designService.ZoomChanged (percent / 100.0);											
			};
			toolBar.Insert(zoomCombobox,0);		
			
			
		}

		
	}
}

