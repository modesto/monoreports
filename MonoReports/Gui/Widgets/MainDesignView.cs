// 
// MainDesignView.cs
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
using MonoReports.Services;
using MonoReports.Core;
using MonoReports.ControlView;
using MonoReports.Model.Engine;
using MonoReports.Model;
using Cairo;
namespace MonoReports.Gui.Widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class MainDesignView : Gtk.Bin
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

		ReportRenderer reportRenderer;
		ReportView reportView;
		int pageNumber = 0;
		IWorkspaceService workSpaceService;

		public IWorkspaceService WorkSpaceService {
			get {
				return this.workSpaceService;
			}
			set {
				workSpaceService = value;
			}
		}

		ToolBarSpinButton pageSpinButton = null;
		ReportEngine reportEngine;
		Report currentReport;
		
		public DrawingArea DesignDrawingArea { 
			get { return drawingarea;}
		}
		
		public DrawingArea PreviewDrawingArea { 
			get { return previewDrawingArea;}
		}
		
		
		public MainDesignView ()
		{
			this.Build ();			
			buildPreviewToolbar ();
			 
		}
		
 
		
		void buildPreviewToolbar ()
		{
			pageSpinButton = new ToolBarSpinButton (40, 1, 1, 1);
			pageSpinButton.SpinButton.ValueChanged += delegate(object sender, EventArgs e) {
				
				pageNumber = (int)(pageSpinButton.SpinButton.Value);
				pageNumber -= 1;
				previewDrawingArea.QueueDraw ();
			};
			
			ToolBarLabel pagelabel = new ToolBarLabel ("Page: ");
			previewToolbar.Insert (pagelabel, 0);
			previewToolbar.Insert (pageSpinButton, 1);
			
		}
		
		
		
		protected virtual void OnDrawingareaExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			if (designService != null) {
				DrawingArea area = (DrawingArea)o;
				Cairo.Context cr = Gdk.CairoHelper.Create (area.GdkWindow);
				cr.Antialias = Cairo.Antialias.Gray;
				designService.RedrawReport (cr);
				area.SetSizeRequest (designService.Width, designService.Height);
				(cr as IDisposable).Dispose ();
			}
		}

		protected virtual void OnPreviewDrawingareaExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			DrawingArea area = (DrawingArea)o;
			Cairo.Context cr = Gdk.CairoHelper.Create (area.GdkWindow);
			//Cairo.Context cr = new Cairo.Context (pdfSurface);
			cr.Antialias = Cairo.Antialias.Gray;
			designService.CurrentContext = cr;
			reportRenderer.RenderPage (designService.Report.Pages[pageNumber]);
			area.SetSizeRequest (designService.Width, designService.Height);
			
			(cr as IDisposable).Dispose ();
		}

		protected virtual void OnDrawingareaButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			
			if (designService.IsDesign) {
				workSpaceService.Status (String.Format ("press x:{0} y:{1} | xroot:{2} yroot:{3}", args.Event.X, args.Event.Y, args.Event.XRoot, args.Event.YRoot));
				designService.ButtonPress (args.Event.X, args.Event.Y);
			}
			
		}

		protected virtual void OnDrawingareaMotionNotifyEvent (object o, Gtk.MotionNotifyEventArgs args)
		{
			
			if (designService.IsDesign) {
				designService.MouseMove (args.Event.X, args.Event.Y);
				workSpaceService.Status (String.Format ("move x:{0} y:{1}", args.Event.X, args.Event.Y));
			}
			
		}

		protected virtual void OnDrawingareaButtonReleaseEvent (object o, Gtk.ButtonReleaseEventArgs args)
		{
			if (designService.IsDesign) {
				designService.ButtonRelease (args.Event.X, args.Event.Y);
			}
		}
 


		protected virtual void OnMainNotebookSwitchPage (object o, Gtk.SwitchPageArgs args)
		{
			
			if (args.PageNum == 1) {
				designService.IsDesign = false;
				reportRenderer = new ReportRenderer (designService);
				
				
				reportEngine = new ReportEngine (currentReport, reportRenderer);
				ImageSurface imagesSurface = new ImageSurface (Format.Argb32, (int)currentReport.Width, (int)currentReport.Height);
				Cairo.Context cr = new Cairo.Context (imagesSurface);
				designService.CurrentContext = cr;
				//reportEngine.DataSource = logicians;
				
				reportEngine.Process ();
				(cr as IDisposable).Dispose ();
				pageSpinButton.SpinButton.SetRange (1, designService.Report.Pages.Count);
				previewDrawingArea.QueueDraw ();
			} else {
				designService.IsDesign = true;
				drawingarea.QueueDraw ();
			}
		}
	}
}

