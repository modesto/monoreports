// 
// Rectangle.cs
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

namespace MonoReports.Model.Controls
{
	public class Line : Control
	{
		public Line ():base()
		{			
			BackgroundColor =  new Color(0,0,0);			
			LineWidth = 2;
		}				
		
		public Point End { get; set; }
		public double LineWidth {get;set;}
		public bool ExtendToBottom {get;set;}
		public LineType LineType {get;set;}
		
		
		public override void MoveControlByY (double y)
		{
			Location = new Point(this.Location.X,this.Location.Y + y);
			End = new Point(this.End.X,this.End.Y + y);
		}
		
		#region implemented abstract members of MonoReports.Model.Controls.Control
		public override Control CreateControl ()
		{
			Line line = new Line();
			CopyBasicProperties(line);			 
			line.End = new Point(End.X,End.Y);
			line.LineWidth = LineWidth;
			line.LineType = LineType;
			line.ExtendToBottom = ExtendToBottom;
			return line;
		}
		
		#endregion
	}
}

