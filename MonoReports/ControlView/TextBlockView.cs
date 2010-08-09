// 
// TextBlockView.cs
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
using MonoReports.Model.Controls;
using MonoReports.Core;
using MonoReports.Extensions.CairoExtensions;
using Cairo;
using MonoReports.Model;


namespace MonoReports.ControlView
{
	public class TextBlockView : ControlViewBase
	{
		
		public override Control ControlModel {
			get {
				return base.ControlModel;
			}
			set {
				base.ControlModel = value;
				textBlock = value as TextBlock;
			}
		}
		
		public TextBlock textBlock;
	 
		public TextBlock TextBlock {
			get {
				return textBlock;
			}			 
		}		

		public TextBlockView (TextBlock textBlock,SectionView parentSection):base(textBlock)
		{
			this.ParentSection = parentSection;
			AbsoluteBound = new Rectangle (parentSection.AbsoluteDrawingStartPoint.X + textBlock.Location.X,
			                                parentSection.AbsoluteDrawingStartPoint.Y + textBlock.Location.Y, textBlock.Width, textBlock.Height);
		}
		
		#region implemented abstract members of MonoReport.ControlView.ControlViewBase

		public override Size Render (Context c,RenderState renderState)
		{
			Rectangle borderRect;
			c.Save();
			borderRect = new Rectangle (textBlock.Location.X, textBlock.Location.Y, textBlock.Width, textBlock.Height);	 
			if(!textBlock.CanGrow || renderState.IsDesign)
				c.ClipRectangle(borderRect);
			
			var rect = c.DrawTextBlock (textBlock,renderState.Render);
				
			if(textBlock.CanGrow & !renderState.IsDesign){
				borderRect = new Rectangle (textBlock.Location.X, textBlock.Location.Y, textBlock.Width, Math.Max( rect.Height, textBlock.Height));				
			}else{
				borderRect = new Rectangle (textBlock.Location.X, textBlock.Location.Y, textBlock.Width, textBlock.Height);								
			}
			if(renderState.Render){
				c.FillRectangle(borderRect,textBlock.BackgroundColor.ToCairoColor());
				c.DrawTextBlock (textBlock,renderState.Render);
				c.DrawInsideBorder  (borderRect, textBlock.Border,renderState.Render);	
			}else{
				c.DrawTextBlock (textBlock,renderState.Render);
			}
			AbsoluteBound = new Rectangle (ParentSection.AbsoluteDrawingStartPoint.X + textBlock.Location.X , 
			                               ParentSection.AbsoluteDrawingStartPoint.Y + textBlock.Location.Y, borderRect.Width, borderRect.Height);
			
			c.Restore();

			return new Size(borderRect.Width,borderRect.Height);
		}

		 
		public override bool ContainsPoint (double x, double y)
		{
			return AbsoluteBound.ContainsPoint(x,y);
		}
		
		#endregion
		
	
		
		
	}
}

