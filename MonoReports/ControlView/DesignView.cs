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


namespace MonoReports.ControlView
{
	public class DesignView
	{

		public double Zoom { get; set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public ControlViewBase SelectedControl { get; set; }
		public BaseTool SelectedTool {
			get { return ToolBoxService.SelectedTool; }
		}
		public Cairo.PointD StartPressPoint { get; private set; }
		public Cairo.PointD EndPressPoint { get; private set; }
		public Cairo.PointD MousePoint { get; private set; }
		public Cairo.PointD PreviousMousePoint { get; private set; }
		public Cairo.PointD DeltaPoint { get; private set; }
		public bool IsPressed { get; private set; }
		public bool IsMoving { get; private set; }
		public bool IsDesign { get; set; }
		public bool Render { get; private set; }
		public IWorkspaceService WorkspaceService { get; set; }
		public IToolBoxService ToolBoxService { get; set; }
		public ReportView ReportView { get; set; }
		internal Context CurrentContext;




		public DesignView (ReportView reportView, IWorkspaceService workspaceService, IToolBoxService toolBoxService)
		{
			ReportView = reportView;
			WorkspaceService = workspaceService;
			ToolBoxService = toolBoxService;
			toolBoxService.DesignView = this;
			IsDesign = true;
			Zoom = 1;
			Render = true;
		}

		public void RedrawReport (Context c)
		{
			
			CurrentContext = c;
			if (Zoom != 1) {
				CurrentContext.Scale (Zoom, Zoom);
				Width = (int)(ReportView.Report.Width * Zoom);
				Height = (int)(ReportView.Report.Height * Zoom);
			}
			
			
			if (SelectedTool != null) {
				SelectedTool.OnBeforeDraw (CurrentContext);
			}
			for (int i = 0; i < ReportView.SectionViews.Count; i++) {
				var b = ReportView.SectionViews[i];
				b.Render (CurrentContext, Render, IsDesign);
			}
			if (SelectedTool != null) {
				SelectedTool.OnAfterDraw (CurrentContext);
			}
			PreviousMousePoint = MousePoint;
		}



		public void ButtonPress (double x, double y)
		{
			StartPressPoint = new Cairo.PointD (x / Zoom, y / Zoom);
			
			
			IsPressed = true;
			IsMoving = false;
			
			if (!IsMoving) {
				PreviousMousePoint = StartPressPoint;
				DeltaPoint = new PointD (0, 0);
				for (int i = 0; i < ReportView.SectionViews.Count; i++) {
					var sectionView = ReportView.SectionViews[i];
					
					
					
					if (sectionView.AbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
						
						if (SelectedTool != null && SelectedTool.CreateMode) {
							SelectedTool.CreateNewControl (sectionView);
							SelectedTool.CreateMode = false;
						} else {
							
							if (sectionView.GripperAbsoluteBound.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
								SelectedControl = sectionView;
								ToolBoxService.SetToolByControlView (sectionView);
							} else {
								ToolBoxService.UnselectTool ();
								for (int j = 0; j < sectionView.Controls.Count; j++) {
									var controlView = sectionView.Controls[j];
									if (controlView.ContainsPoint (StartPressPoint.X, StartPressPoint.Y)) {
										SelectedControl = controlView;
										ToolBoxService.SetToolByControlView (controlView);
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
			
			WorkspaceService.InvalidateDrawingArea ();
			
		}

		public void MouseMove (double x, double y)
		{
			
			MousePoint = new Cairo.PointD (x / Zoom, y / Zoom);
			IsMoving = true;
			DeltaPoint = new PointD (-PreviousMousePoint.X + MousePoint.X, -PreviousMousePoint.Y + MousePoint.Y);
			
			
			if (!IsPressed) {
				bool isOnGripper = false;
				foreach (SectionView sectionView in ReportView.SectionViews) {
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
			if (SelectedTool != null)
				SelectedTool.OnMouseMove ();
			
			WorkspaceService.InvalidateDrawingArea ();
			
		}

		public void ZoomChanged (double zoom)
		{
			Zoom = zoom;
			WorkspaceService.InvalidateDrawingArea ();
		}

		public void ButtonRelease (double x, double y)
		{
			EndPressPoint = new Cairo.PointD (x / Zoom, y / Zoom);
			IsPressed = false;
			IsMoving = false;
			if (SelectedTool != null) {
				SelectedTool.OnMouseUp ();
				if (SelectedControl != null) {
					WorkspaceService.ShowInPropertyGrid (SelectedControl.ControlModel);
				}
			}
			WorkspaceService.InvalidateDrawingArea ();
			
		}




		public void NextPage ()
		{
			CurrentContext.ShowPage ();
		}
		
		
		
	}
}

