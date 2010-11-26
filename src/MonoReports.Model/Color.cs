// 
// Color.cs
//  
// Author:
//       Tomasz Kubacki <tomasz.kubacki(at)gmail.com>
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
	public  struct Color : ICloneable
	{
 
	public Color (double r, double g, double b):this(r, g, b,1){
			 
	}

	public Color (double r, double g, double b, double a){
			this.r=r;
			this.b=b;
			this.g=g;
			this.a=a;			 
	}

 
	double r;
	public double R { get { return r; } set { r = value; } }

	double g;
	public double G { get { return g; } set { g = value; } }

	double b;
	public double B { get { return b; } set { b = value; } }

	double a;
	public double A { get { return a; } set { a = value; } }
		
	public object Clone ()
	{
		return new Color(r,g,b,a);		
	}

	}
}

