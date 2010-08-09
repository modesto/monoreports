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

namespace MonoReports.Tools
{
	internal enum GripperType
	{
		NE,
		NW,
		SW,
		SE
	}

	public class RectTool : BaseTool
	{

		const int gripSize = 10;
		bool isResizing;
		Border selectBorder;
		GripperType gripperType;

		public RectTool (DesignView designView) : base(designView)
		{
			selectBorder = new Border ();
			selectBorder.Color = System.Drawing.Color.Black;
			selectBorder.WidthAll = 1;
		}

		public override void OnBeforeDraw (Context c)
		{
			
			if (designView.IsPressed && designView.IsMoving && designView.SelectedControl != null) {
				var control = designView.SelectedControl;
				var location = control.ControlModel.Location;
				
				if (designView.IsMoving) {
					if (!isResizing) {
						var point = new MonoReports.Model.Controls.Point (Math.Max(0, location.X + designView.DeltaPoint.X), Math.Max(0, location.Y + designView.DeltaPoint.Y));
						control.ControlModel.Location = point;
					} else {
						
						switch (gripperType) {
						case GripperType.NE:
							control.ControlModel.Size = new Size (Math.Abs (control.ControlModel.Size.Width + designView.DeltaPoint.X), Math.Abs (control.ControlModel.Size.Height - designView.DeltaPoint.Y));
							control.ControlModel.Location = new MonoReports.Model.Controls.Point (location.X, location.Y + designView.DeltaPoint.Y);
							break;
						case GripperType.SE:
							control.ControlModel.Size = new Size (Math.Abs (control.ControlModel.Size.Width + designView.DeltaPoint.X), Math.Abs (control.ControlModel.Size.Height + designView.DeltaPoint.Y));
							break;
						case GripperType.SW:
							control.ControlModel.Size = new Size (Math.Abs (control.ControlModel.Size.Width - designView.DeltaPoint.X), Math.Abs (control.ControlModel.Size.Height + designView.DeltaPoint.Y));
							control.ControlModel.Location = new MonoReports.Model.Controls.Point (location.X + designView.DeltaPoint.X, location.Y);
							break;
						case GripperType.NW:
							control.ControlModel.Size = new Size (Math.Abs (control.ControlModel.Size.Width - designView.DeltaPoint.X), Math.Abs (control.ControlModel.Size.Height - designView.DeltaPoint.Y));
							control.ControlModel.Location = new MonoReports.Model.Controls.Point (location.X + designView.DeltaPoint.X, location.Y + designView.DeltaPoint.Y);
							break;
						default:
							break;
						}
						
						
					}
				}
				
			}
			
			
		}
		
		public override string Name {get {return "RectTool"; }}

		public override void OnAfterDraw (Context c)
		{
			if (designView.SelectedControl != null && designView.IsDesign) {
				c.Save ();
				c.SetDash (new double[] { 1.0 }, 1);
				c.DrawInsideBorder (designView.SelectedControl.AbsoluteBound, selectBorder, true);
				c.DrawSelectBox (designView.SelectedControl.AbsoluteBound);
				c.Restore ();
			}
		}

		public override void OnMouseDown ()
		{
			if (designView.SelectedControl != null) {
				
				var control = designView.SelectedControl;
				var pointInSection = control.ParentSection.PointInSectionByAbsolutePoint (designView.MousePoint);
				var location = control.ControlModel.Location;
				isResizing = false;
				if (pointInSection.Y > location.Y && pointInSection.Y < location.Y + gripSize) {
					if (pointInSection.X > location.X && location.X + gripSize > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.NW;
					} else if (pointInSection.X > location.X + control.ControlModel.Size.Width - gripSize && location.X + control.ControlModel.Size.Width > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.NE;
					}
					
				} else if (pointInSection.Y > location.Y + control.ControlModel.Size.Height - gripSize && pointInSection.Y < location.Y + control.ControlModel.Size.Height) {
					if (pointInSection.X > location.X && location.X + gripSize > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.SW;
					} else if (pointInSection.X > location.X + control.ControlModel.Size.Width - gripSize && location.X + control.ControlModel.Size.Width > pointInSection.X) {
						isResizing = true;
						gripperType = GripperType.SE;
					}
				}
			}
		}

		public override void OnMouseUp ()
		{
			isResizing = false;
		}
		
		
		
	}
}

