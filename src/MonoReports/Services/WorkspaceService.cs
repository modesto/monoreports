// 
// WorkspaceService.cs
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
using Gtk;
using PropertyGrid;
 

namespace MonoReports.Services
	
{
	public class WorkspaceService : IWorkspaceService
	{
		
		DrawingArea designArea;
		DrawingArea previewArea;
		PropertyGrid.PropertyGrid propertyGrid;
		Gtk.Bin mainControl;
		Gtk.Label StatusLabel {get;set;}
		
		public WorkspaceService (Gtk.Bin mainWindow, DrawingArea designArea,DrawingArea previewArea, PropertyGrid.PropertyGrid propertyGrid,Gtk.Label statusLabel)
		{
			this.designArea = designArea;
			this.previewArea = previewArea;
			this.propertyGrid = propertyGrid;
			this.mainControl = mainWindow;			
			this.StatusLabel = statusLabel;
		}
		
	
		#region IWorkspaceService implementation
		public void Status (string message)
		{
			 StatusLabel.Text = message;
		}

		public void SetCursor (Gdk.CursorType cursorType)
		{
		 	mainControl.GdkWindow.Cursor = new Gdk.Cursor (cursorType);
		}

		public void InvalidateDesignArea ()
		{
			designArea.QueueDraw();
		}

		public void InvalidatePreviewArea ()
		{
		  	previewArea.QueueDraw();
		}

		public void ShowInPropertyGrid (object o)
		{
			propertyGrid.CurrentObject = o;
		}
		#endregion 
}
}

