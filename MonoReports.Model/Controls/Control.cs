// 
// Control.cs
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
using MonoReports.Model.Data;
using System.Collections.Generic;

namespace MonoReports.Model.Controls
{
	public abstract class Control
	{

		public Control ()
		{
			BackgroundColor = new Color (1,1,1,1);
			Location = new Point (0,0);
			Size = new Size (0,0);
			IsVisible = true;
		}

		public Control TemplateControl {get; set;}

		Point location;

		public Point Location {
			get { return location; }
			set { location = value; }
		}

		Size size;

		public Size Size {
			get { return size;}
			set { size = value;}
		}

		public Color BackgroundColor {get; set;}

		public double Height {

			get { return size.Height; }
			set {
				size = new Size (size.Width, value);
			}
		}

		public double Width {

			get { return size.Width; }
			set {
				size = new Size (value, size.Height);
			}
		}

		public virtual double Top {
			
			get { return location.Y; }
			
			set { 
				location = new Point (Location.X, value);
			}
		}

		public double Left {

			get { return location.X; }
			
			set { 
				location = new Point ( value, Location.Y);
			}
		}

		public virtual double Bottom {
			get { return Location.Y + Size.Height; }
		}

		public bool IsVisible { get; set; }

		internal double measureBottomMarginFromSection (Section s)
		{
			
			return  s.Height - (Location.Y + Size.Height);
			
		}

		public virtual void MoveControlByY (double y)
		{
			Location = new Point (this.Location.X,this.Location.Y + y);
		}

		public abstract Control CreateControl ();

		internal void CopyBasicProperties (Control c)
		{
			c.Location = new Point (Location.X,Location.Y);
			c.Size = new Size (Size.Width,Size.Height);			
			c.IsVisible = IsVisible;
			c.BackgroundColor = new Color (BackgroundColor.R,BackgroundColor.G,BackgroundColor.B,BackgroundColor.A);
			c.TemplateControl = this;
			
		}

		public virtual IEnumerable<Control> SplitControlAt (double y)
		{ 
			return new Control[0];
		}

		public virtual void AssignValue (IDataSource dataSource)
		{
			
		}
		
	}
}
