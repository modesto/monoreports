// 
// ToolBarComboBox.cs
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
//
// Based on ToolBarComboBox.cs 
//  
// by:
//       Jonathan Pobst <monkey@jpobst.com>
// in Pinta Project

using System;
using Gtk;
namespace MonoReports.Extensions.GtkExtensions
{
	public class ToolBarSpinButton : ToolItem
	{
		public SpinButton SpinButton { get; private set; }
		 
		 

		public ToolBarSpinButton (int width,double min, double max, double step )
		{
			
				SpinButton = new SpinButton (min,max,1);
			 

			SpinButton.AddEvents ((int)Gdk.EventMask.ButtonPressMask);
			SpinButton.WidthRequest = width;
			
			 
			
			SpinButton.Show ();
			
			Add (SpinButton);
			Show ();
		}
	}
}

