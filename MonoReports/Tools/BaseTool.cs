// 
// BaseTool.cs
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
using MonoReports.Model.Controls;
using Gtk;
using MonoReports.Services;
using MonoReports.Gui.Widgets;
namespace MonoReports.Tools
{
	public abstract class BaseTool
	{
		protected DesignService designService;

		public BaseTool (DesignService designService)
		{
			this.designService = designService;
		}
		
 
	    public abstract string Name {get;}
		
		public virtual string ToolbarImageName {get { return  String.Empty; }}
		
		public abstract bool IsToolbarTool {get;}
		
		public bool CreateMode;
		
		public virtual void CreateNewControl (SectionView sectionView){}
		
		public virtual void BuildToolbar(Gtk.Toolbar toolBar){
			if(IsToolbarTool){
			ToolBarButton toolButton = new ToolBarButton (ToolbarImageName,Name,Name);
			toolButton.Clicked += delegate(object sender, EventArgs e) {
				designService.SelectedControl = null;
				designService.SelectedTool = this;
				this.CreateMode = true;
			};
			toolBar.Insert (toolButton, 1);		
			}
		}
				

		public virtual void OnBeforeDraw (Context c)
		{
			
		}

		public virtual void OnAfterDraw (Context c)
		{
			
		}

		public virtual void OnMouseDown ()
		{
			
		}
		
		public virtual void OnDoubleClick ()
		{
		 
		}


		public virtual void OnMouseUp ()
		{
			
		}

		public virtual void OnMouseMove ()
		{
			
		}
		
		public virtual void KeyPress(Gdk.Key key){
			switch (key) {
				case Gdk.Key.Delete:				
					designService.DeleteSelectedControl();
				break;
			default:
			break;
			}
			
			
		}
		
		
		
	}
}

