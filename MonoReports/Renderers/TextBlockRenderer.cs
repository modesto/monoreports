// 
// TextBlockRenderer.cs
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
using MonoReports.Core;
using MonoReports.Model.Controls;
using MonoReports.Extensions.CairoExtensions;
using Cairo;
using MonoReports.Model;

namespace MonoReports.Renderers
{
	public class TextBlockRenderer: IControlRenderer
	{
		public TextBlockRenderer ()
		{
			
		}

		public void Render (Cairo.Context c,Control control)
		{
            TextBlock textBlock = control as TextBlock;
			Rectangle borderRect;			
			borderRect = new Rectangle (textBlock.Location.X, textBlock.Location.Y, textBlock.Width, textBlock.Height);	 						
			var rect = c.DrawTextBlock (textBlock,true);
				
			if(textBlock.CanGrow && rect.Height > textBlock.Height || textBlock.CanShrink && rect.Height < textBlock.Height){
				borderRect = new Rectangle (textBlock.Location.X, textBlock.Location.Y, textBlock.Width, rect.Height);				
			} else {
				borderRect = new Rectangle (textBlock.Location.X, textBlock.Location.Y, textBlock.Width, textBlock.Height);								
			}
		
			c.FillRectangle(borderRect,textBlock.BackgroundColor.ToCairoColor());
			c.DrawTextBlock (textBlock,true);
			c.DrawInsideBorder  (borderRect, textBlock.Border,true);	
		}

        public Size Measure(Cairo.Context c, Control control)
		{
            TextBlock textBlock = control as TextBlock;
			var r = c.DrawTextBlock (textBlock,false);
			return new Size(r.Width, r.Height);;
		}						

	}
}

