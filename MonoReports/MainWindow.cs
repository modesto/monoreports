// 
// MainWindow.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki(at)gmail.com>
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
using MonoReports.Model;
using Controls = MonoReports.Model.Controls;
using MonoReports.Core;
using Gtk;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Cairo;
using Model = MonoReports.Model.Engine;
using MonoReports.Extensions.GtkExtensions;
using MonoReports.ControlView;
using Newtonsoft.Json;
using MonoReports.Tools;

public partial class MainWindow : Gtk.Window, IWorkspaceService
{

	DesignView designView;
	ReportView reportView;
	ReportRenderer reportRenderer;
	MonoReports.Model.Engine.ReportEngine reportEngine;
	Report currentReport;
	ToolBoxService toolBoxService;
	
	public Report CurrentReport {
		get { return this.currentReport; }
		set { currentReport = value; }
	}


	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		buildMainToolbar ();
		exampleReport ();
		buildPreviewToolbar ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	void exampleReport ()
	{
		
		currentReport = new Report ();
		
		
		
		
		currentReport.PageHeaderSection.Controls.Add (new Controls.TextBlock { FontSize = 16, FontName = "Helvetica", Text = "First textblock Żection Żecsdfsdfsdfsfsdfsdion Żecsdfsdfsdfs fsdfsdion Żecsdfsdfsdfsfsdfsdion Żection Żecsdfs dfsdfsfsdfs dfs dfsdfsdfsdfsd ftion Żection ŻSection Ż", FontColor = System.Drawing.Color.Red, CanGrow = true, Location = new Controls.Point (3, 3), Size = new Controls.Size (200, 80) });
		
		try {
			//var bytes = System.IO.File.ReadAllBytes ("../../resources/Alfred_Tarski.jpeg");
			var bytes = System.IO.File.ReadAllBytes ("Alfred_Tarski.jpeg");
			currentReport.ResourceRepository.Add (bytes);
			var img = new MonoReports.Model.Controls.Image { ImageIndex = 0, Location = new Controls.Point (3, 3), Size = new Controls.Size (300, 280) };
			currentReport.DetailSection.Controls.Add (img);
		} catch (Exception exp) {
			//FIXME image loading os independent
		}
		
		currentReport.PageHeaderSection.Controls.Add (new Controls.TextBlock { FontSize = 24, FontName = "Helvetica", Text = "Second ection Żection Żection Żection Żection ŻSection Ż", FontColor = System.Drawing.Color.LightSeaGreen, Location = new Controls.Point (123, 87), CanGrow = false, Size = new Controls.Size (160, 60) });
		
		currentReport.PageHeaderSection.Controls.Add (new Controls.TextBlock { FontSize = 12, FontName = "Helvetica", Text = "Third ection Żection Żection Żection Żection ŻSection Ż", FontColor = System.Drawing.Color.Green, Location = new Controls.Point (300, 17), CanGrow = true, Size = new Controls.Size (100, 20) });
		
		currentReport.DetailSection.Size = new Controls.Size (600, 300);
		
		currentReport.DetailSection.Controls.Add (new Controls.TextBlock { FontSize = 12, FontName = "Helvetica", Text = "Chars", FontColor = System.Drawing.Color.Blue, Location = new Controls.Point (223, 42), Size = new Controls.Size (200, 30), FieldName = "Name", BackgroundColor = System.Drawing.Color.Transparent, HorizontalAlignment = Controls.HorizontalAlignment.Left, Border = new Border { WidthAll = 0 },
		CanGrow = true });
		
		currentReport.DetailSection.Controls.Add (new Controls.TextBlock { FontSize = 12, FontName = "Helvetica", Text = "Surname", FontColor = System.Drawing.Color.Red, Location = new Controls.Point (223, 12), Size = new Controls.Size (200, 30), FieldName = "Surname", BackgroundColor = System.Drawing.Color.Orange, HorizontalAlignment = Controls.HorizontalAlignment.Left, Border = new Border { WidthAll = 0 },
		CanGrow = true });
		
		
		
		
		currentReport.PageFooterSection.Controls.Add (new Controls.TextBlock { FontSize = 12, FontName = "Times", Text = "Deatail section detail section lorem ipsum dolores tratatratrat ", FontColor = System.Drawing.Color.White, Location = new Controls.Point (23, 12), Size = new Controls.Size (400, 70), BackgroundColor = System.Drawing.Color.FromArgb (255, 95, 84, 120), HorizontalAlignment = Controls.HorizontalAlignment.Left, Border = new Border { WidthAll = 0 }, CanGrow = false });
		
		
		currentReport.PageFooterSection.Controls.Add (new Controls.Line { Location = new Controls.Point (20, 20), End = new Controls.Point (420, 10) });
		currentReport.AddGroup("Age");
        reportView = new ReportView(currentReport);
		designView = new MonoReports.ControlView.DesignView (reportView, this,toolBoxService);
	}

	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		Application.Quit ();
	}


	protected virtual void OnDrawingareaExposeEvent (object o, Gtk.ExposeEventArgs args)
	{
		if (designView != null) {
			DrawingArea area = (DrawingArea)o;
			Cairo.Context cr = Gdk.CairoHelper.Create (area.GdkWindow);
			cr.Antialias = Cairo.Antialias.Gray;
			designView.RedrawReport (cr);
			area.SetSizeRequest (designView.Width, 800);
			(cr as IDisposable).Dispose ();
		}
	}

	protected virtual void OnPreviewDrawingareaExposeEvent (object o, Gtk.ExposeEventArgs args)
	{
		DrawingArea area = (DrawingArea)o;
		Cairo.Context cr = Gdk.CairoHelper.Create (area.GdkWindow);
		//Cairo.Context cr = new Cairo.Context (pdfSurface);
		cr.Antialias = Cairo.Antialias.Gray;
		designView.CurrentContext = cr;
		reportRenderer.RenderPage (reportView.Report.Pages[pageNumber]);
		area.SetSizeRequest (designView.Width, 800);
		
		(cr as IDisposable).Dispose ();
	}





	protected virtual void OnDrawingareaButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
	{
		
		if (designView.IsDesign) {
			Status (String.Format ("press x:{0} y:{1} | xroot:{2} yroot:{3}", args.Event.X, args.Event.Y, args.Event.XRoot, args.Event.YRoot));
			designView.ButtonPress (args.Event.X, args.Event.Y);
		}
		
	}

	protected virtual void OnDrawingareaMotionNotifyEvent (object o, Gtk.MotionNotifyEventArgs args)
	{
		
		if (designView.IsDesign) {
			designView.MouseMove (args.Event.X, args.Event.Y);
			Status (String.Format ("move x:{0} y:{1}", args.Event.X, args.Event.Y));
		}
		
	}

	protected virtual void OnDrawingareaButtonReleaseEvent (object o, Gtk.ButtonReleaseEventArgs args)
	{
		if (designView.IsDesign) {
			designView.ButtonRelease (args.Event.X, args.Event.Y);
		}
	}


	public void Status (string message)
	{
		bottomStatusbar.Push (1, message);
	}


	public void ShowInPropertyGrid (object o)
	{
		propertygrid2.CurrentObject = o;
	}

	public void SetCursor (Gdk.CursorType cursorType)
	{
		this.GdkWindow.Cursor = new Gdk.Cursor (cursorType);
	}

	public void InvalidateDrawingArea ()
	{
		drawingarea.QueueDraw ();
	}

	void buildMainToolbar ()
	{
		
		toolBoxService = new ToolBoxService();
		
		
		
		ToolBarComboBox zoomCombobox = new ToolBarComboBox (100, 4, true, new string[] { "400%", "300%", "200%", "150%", "100%", "66%", "50%" });
		zoomCombobox.ComboBox.Changed += delegate(object sender, EventArgs e) {
			
			string text = zoomCombobox.ComboBox.ActiveText;
			
			text = text.Trim ('%');
			
			double percent;
			
			if (!double.TryParse (text, out percent))
				return;
			percent = Math.Min (percent, 300);
			designView.ZoomChanged (percent / 100.0);
		};
		
		mainToolbar.Insert (zoomCombobox, 0);

		
		 
	}

	int pageNumber = 0;
	ToolBarSpinButton pageSpinButton = null;
	
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

	protected virtual void OnEditActionActivated (object sender, System.EventArgs e)
	{
		
		toolBoxService.SetToolByName("LineTool");
		
	}
	

	protected virtual void OnSaveActionActivated (object sender, System.EventArgs e)
	{
		
		using (System.IO.FileStream file = System.IO.File.OpenWrite ("test.mrp")) {
			
		 
			var serializedProject = JsonConvert.SerializeObject (reportView.Report, 
			                                                     Formatting.None, 
			                                                     new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes (serializedProject);
			file.Write (bytes, 0, bytes.Length);
			
			file.Close ();
		}
		
	}

	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose the file to open", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
		var fileFilter = new FileFilter { Name = "Monoreports project" };
		fileFilter.AddPattern ("*.mrp");
		fc.AddFilter (fileFilter);
		
		if (fc.Run () == (int)ResponseType.Accept) {
			System.IO.FileStream file = System.IO.File.OpenRead (fc.Filename);
			byte[] bytes = new byte[file.Length];
			file.Read (bytes, 0, (int)file.Length);
			propertygrid2.CurrentObject = null;
				
				
			var report = JsonConvert.DeserializeObject<Report> (System.Text.Encoding.UTF8.GetString (bytes), 
			                                                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects,
				Converters = new List<JsonConverter> (
				    new JsonConverter[] { new MonoReports.Extensions.PointConverter (), 
					new MonoReports.Extensions.SizeConverter () 
				}) });
			CurrentReport = report;
			reportView = new ReportView(currentReport);
			designView = new DesignView (reportView, this,toolBoxService);
			file.Close ();
		}
		
		fc.Destroy ();
		drawingarea.QueueDraw ();
	}
	
//TODO remove example report and data source to some external class	
	
	protected virtual void OnMainNotebookSwitchPage (object o, Gtk.SwitchPageArgs args)
	{
		
		if (args.PageNum == 1) {
			designView.IsDesign = false;
			reportRenderer = new ReportRenderer (designView);
			reportEngine = new Model.ReportEngine (currentReport, reportRenderer);
			var logicians = new List<Logician> ();
			logicians.Add (new Logician { Name = "Alfred", Surname = "Tarski", Age =33 });
			logicians.Add (new Logician { Name = "Gotlob", Surname = "Frege", Age =42 });
			logicians.Add (new Logician { Name = "Kurt", Surname = "Gödel", Age =22 });
			logicians.Add (new Logician { Name = "Unknown ", Surname = "Logican", Age =33 });
			logicians.Add (new Logician { Name = "Józef", Surname = "Bocheński", Age =62 });
			logicians.Add (new Logician { Name = "Stanisław", Surname = "Leśniewski", Age =79 });
			logicians.Add (new Logician { Name = "Saul", Surname = "Kripke", Age =40 });
			logicians.Add (new Logician { Name = "George", Surname = "Boolos", Age =79 });
			ImageSurface imagesSurface = new ImageSurface (Format.Argb32, (int)currentReport.Width, (int)currentReport.Height);
			Cairo.Context cr = new Cairo.Context (imagesSurface);
			designView.CurrentContext = cr;
			reportEngine.DataSource = logicians;
			reportEngine.Process ();
			(cr as IDisposable).Dispose ();
			pageSpinButton.SpinButton.SetRange (1, reportView.Report.Pages.Count);
			previewDrawingArea.QueueDraw ();
		} else {
			designView.IsDesign = true;
			drawingarea.QueueDraw ();
		}
	}
	
	protected virtual void OnSortAscendingActionActivated (object sender, System.EventArgs e)
	{
		toolBoxService.SetToolByName("CrossSectionLineTool");
		
	}
	
	
	
	
	
	
	public class Logician
	{

		public string Surname { get; set; }

		public string Name { get; set; }
		
		public int Age { get; set; }
	}
	
	
}

