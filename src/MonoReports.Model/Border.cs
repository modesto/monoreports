// 
// Border.cs
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
using MonoReports.Model.Controls;


namespace MonoReports.Model
{

	public class Border : ICloneable
	{

		public Border ()
		{
			
		}
		
		public Border (double all): this(all,all,all,all)
		{
			
		}
		
		public Border (double left, double top, double right, double bottom)
		{
			_leftWidth = left;
			_topWidth = top;
			_rightWidth = right;
			_bottomWidth = bottom;
			Color = new Color(0,0,0);
		}
		
		double _leftWidth;
		public double LeftWidth {get { return _leftWidth; }set { _leftWidth = value; }}
		double _topWidth;
		public double TopWidth {get { return _topWidth; }set { _topWidth = value; }}
		double _rightWidth;
		public double RightWidth {get { return _rightWidth; }set { _rightWidth = value; }}
		double _bottomWidth;
		public double BottomWidth {get { return _bottomWidth; }set { _bottomWidth = value; }}
		
		public double WidthAll{
			get {return LeftWidth;}
			set {
				_leftWidth = value;
				_topWidth = value;
				_rightWidth = value;
				_bottomWidth = value;				
				}
		}
		
		public Color Color {get;set;}
		
	 
		public object Clone ()
		{
			Border b = new Border();
			
			b.LeftWidth = LeftWidth;
			b.RightWidth = RightWidth;
			b.TopWidth = TopWidth;
			b.BottomWidth = BottomWidth;
			b.Color =  new Color(Color.R,Color.G,Color.B,Color.A);
			return b;
		}
		
		public override string ToString ()
		{
			return string.Format ("[Border: Left={0}, Top={1}, Right={2}, Bottom={3}, Color={4}]", LeftWidth, TopWidth, RightWidth, BottomWidth, Color);
		}
		 
	}
}
