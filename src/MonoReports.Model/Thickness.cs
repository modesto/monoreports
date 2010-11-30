// 
// Padding.cs
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

namespace MonoReports.Model
{
	public struct Thickness
	{
		
		public Thickness (double all) {
			l = all;
			t = all;
			r = all;
			b = all;
		}
		
		public Thickness (double left, double top, double right, double bottom) {
			l = left;
			t = top;
			r = right;
			b = bottom;
		}

		double l;
		public double Left {
			get { return l; }
			set { l = value; }
		}
		
		double t;
		public double Top
		{
			get { return t; }
			set { t = value; }
		}
		
		
		double r;
		public double Right {
			get { return r; }
			set { r = value; }
		}
		
		double b;
		public double Bottom
		{
			get { return b; }
			set { b = value; }
		}
		
		public override string ToString ()
		{
			return string.Format ("[Padding: Left={0}, Top={1},Right={2}, Bottom={3}]", l, t, r, b); 
		}
	}
}

