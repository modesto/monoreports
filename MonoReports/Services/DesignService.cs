// 
// ReportView.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki (at) gmail.com>
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
using MonoReports.Model;
using System.Collections.Generic;
using Cairo;
using MonoReports.Core;
using MonoReports.Extensions.CairoExtensions;
using MonoReports.Model.Controls;
using System.Collections.ObjectModel;
using MonoReports.Tools;
using MonoReports.ControlView;

namespace MonoReports.Services
{
	public class DesignService
	{
		public double Zoom { get; set; }

		public double Width { get;  set; }

		public double Height { get; set; }

		public BaseTool SelectedTool {
			get { return ToolBoxService.SelectedTool; } 
			set {
				ToolBoxService.SelectedTool = value; 
			}
		}

		public Cairo.PointD StartPressPoint {
			get;
			private set;
		}

		public Cairo.PointD EndPressPoint { get; private set; }

		public Cairo.PointD MousePoint { get; private set; }

		public Cairo.PointD PreviousMousePoint { get; private set; }

		public Cairo.PointD DeltaPoint { get; private set; }

		public bool IsPressed { get; private set; }

		public bool IsMoving { get; private set; }

		public bool IsDesign { get; set; }

		public bool Render { get; private set; }

		public IWorkspaceService WorkspaceService { get; set; }

		public ToolBoxService ToolBoxService{get; set;}

		public event SelectedControlChanged OnSelectedControlChanged;
		public event ReportChanged OnReportChanged;

		ControlViewBase selectedControl;

		public ControlViewBase SelectedControl { 
			get { return selectedControl; } 
			set { 
				selectedControl = value; 
				if (selectedControl != null){
					ToolBoxService.SetToolByControlView (selectedControl);
				}else{
					ToolBoxService.UnselectTool ();
				}
				if (OnSelectedControlChanged != null)
					OnSelectedControlChanged (this, new EventArgs ());
			} 
		}

		internal Context CurrentContext;	
		
		
		ControlViewFactory controlViewFactory;

		Report report;
		
		public Report Report {
			get { return report;} 
			set {
				report = value;
				initReport();
				if(OnReportChanged != null)
					OnReportChanged(this,new EventArgs());
			}
		}

		private List<SectionView> sectionViews;

		public IList<SectionView> SectionViews {
			get { return sectionViews; }
			private set {
				;
			}
		}
		
		public PixbufRepository PixbufRepository{get;set;}
		
		

		public DesignService (IWorkspaceService workspaceService, Report report)
		{		
			this.WorkspaceService = workspaceService;
			controlViewFactory = new ControlViewFactory (this);
			PixbufRepository = new PixbufRepository(){ Report = report};
			IsDesign = true;
			Zoom = 1;
			Render = true;		
			Report = report;
		}
		
		void initReport(){
			sectionViews = new List<SectionView> ();
			addSectionView (report.ReportHeaderSection);
			addSectionView (report.PageHeaderSection);
			foreach (var groupHeader in report.GroupHeaderSections) {
				addSectionView (groupHeader);
			}
			addSectionView (report.DetailSection);
			foreach (var groupFooter in report.GroupFooterSections) {
				addSectionView (groupFooter);
			}
			
			addSectionView (report.PageFooterSection);
			addSectionView (report.ReportFooterSection);
		}

		public void RedrawReport (Context c)
		{
			
			CurrentContext = c;
			
			if (Zoom != 1) {
				CurrentContext.Scale (Zoom, Zoom);
				Width = (int)(Report.Width * Zoom);
				Height = (int)(Report.Height * Zoom);
			}
			
			
			if (SelectedTool != null) {
				SelectedTool.OnBeforeDraw (CurrentContext);
			}
			for (int i = 0; i < SectionViews.Count; i++) {
				var renderedSection = SectionViews [i];
				renderedSection.Render (CurrentContext);								
			}
			if (SelectedTool != null) {
				SelectedTool.OnAfterDraw (CurrentContext);
			}
			
		}

		public void DropedField (string fieldName, double x, double y)
		{
			var point = new Cairo.PointD (x / Zoom, y / Zoom);
			
			
			for (int i = 0; i < SectionViews.Count; i++) {
				var sectionView = SectionViews [i];
					
				if (sectionView.AbsoluteBound.ContainsPoint (point.X, point.Y)) {
						
					if (sectionView.HeaderAbsoluteBound.ContainsPoint (point.X, point.Y)) {
						SelectedControl = sectionView;
						continue;
					}
					point = sectionView.PointInSectionByAbsolutePoint (point);	
					ToolBoxService.SetToolByName ("TextBlockTool");							
					SelectedTool.CreateNewControl (sectionView);
					(SelectedControl.ControlModel as TextBlock).Text = fieldName;
					(SelectedControl.ControlModel as TextBlock).FieldName = fieldName;
					(SelectedControl.ControlModel as TextBlock).Location = new MonoReports.Model.Controls.Point (point.X,point.Y);
					SelectedTool.CreateMode = false;
					
				}
			}
		}
		
		public void KeyPress (Gdk.Key key){
			if(SelectedTool != null){
				SelectedTool.KeyPress(key);
			}
		}
		
		public void DeleteSelectedControl(){
			if(selectedControl != null){
				SelectedControl.ParentSection.RemoveControlView(selectedControl);				
				SelectedControl.ControlModel = null;
				SelectedControl = null;
				WorkspaceService.InvalidateDesignArea ();			
			}
		}

		public void ButtonPress (double x, double y)
		{
			StartPressPoint = new Cairo.PointD (x / Zoom, y / Zoom);
			
			
			IsPressed = true;
			IsMoving = false;
			
			if (!IsMoving) {
				PreviousMousePoint = StartPressPoint;
				DeltaPoint = new PointD (0, 0);
				for (int i = 0; i < SectionViews.Count; i++) {
					var sectionView = SectionViews [i];
	
					if (sectionView.AbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
						
						if (sectionView.HeaderAbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
							SelectedControl = sectionView;
							SelectedTool = null;
							continue;
						} else if (sectionView.GripperAbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
							SelectedControl = sectionView;
									
						} else {
							
							if (SelectedTool != null && SelectedTool.CreateMode) {
								SelectedTool.CreateNewControl (sectionView);
								SelectedTool.CreateMode = false;
							} else {
							
								SelectedControl = null;
							
								for (int j = 0; j < sectionView.Controls.Count; j++) {
									var controlView = sectionView.Controls [j];
									if (controlView.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
										SelectedControl = controlView;										 
										break;
									}
								}
							}
						}
					
							
					}
				}
					
			}

			
			if (SelectedTool != null)
				SelectedTool.OnMouseDown ();
			
			WorkspaceService.InvalidateDesignArea ();
			
		}

		public void MouseMove (double x, double y)
		{
			
			MousePoint = new Cairo.PointD (x / Zoom, y / Zoom);
			IsMoving = true;
			if (SelectedTool != null)
				SelectedTool.OnMouseMove ();
			DeltaPoint = new PointD (-PreviousMousePoint.X + MousePoint.X, -PreviousMousePoint.Y + MousePoint.Y);
			
			
			if (!IsPressed) {
				bool isOnGripper = false;
				foreach (SectionView sectionView in SectionViews) {
					if (sectionView.GripperAbsoluteBound.ContainsPoint (MousePoint)) {
						sectionView.SectionGripperHighlighted = true;
						isOnGripper = true;
					} else {
						sectionView.SectionGripperHighlighted = false;
					}
				}
				if (isOnGripper) {
					WorkspaceService.SetCursor (Gdk.CursorType.BottomSide);
				} else {
					WorkspaceService.SetCursor (Gdk.CursorType.LeftPtr);
				}
			}
			
			
			WorkspaceService.InvalidateDesignArea (); 
			PreviousMousePoint = MousePoint;
		}

		public void ZoomChanged (double zoom)
		{
			Zoom = zoom;
			WorkspaceService.InvalidateDesignArea (); 
		}

		public void ButtonRelease (double x, double y)
		{
			EndPressPoint = new Cairo.PointD (x / Zoom, y / Zoom);
			IsPressed = false;
			IsMoving = false;
			if (SelectedTool != null) {
				SelectedTool.OnMouseUp ();
				
			}
			
			if (SelectedControl != null) {
					WorkspaceService.ShowInPropertyGrid (SelectedControl.ControlModel);
			}
			
			WorkspaceService.InvalidateDesignArea (); 			
		}

		public void NextPage ()
		{
			CurrentContext.ShowPage ();
		}



		private void addSectionView (Section section)
		{
			Cairo.PointD sectionSpan;
			if (sectionViews.Count > 0) {
				var previousSection = sectionViews [sectionViews.Count - 1];
				sectionSpan = new Cairo.PointD (0, previousSection.AbsoluteBound.Y + previousSection.AbsoluteBound.Height);
			} else {
				sectionSpan = new Cairo.PointD (0, 0);
			}
			var sectionView = new SectionView (Report, controlViewFactory, section, sectionSpan);
			sectionViews.Add (sectionView);
			Height =  sectionView.AbsoluteBound.Y + sectionView.AbsoluteBound.Height;
		}
		
	}
	
	
}

