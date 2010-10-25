// 
// Section.cs
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
using System.Collections.Generic;


namespace MonoReports.Model.Controls
{


	public abstract class Section : Control
	{

		public Section ():base()
		{
			Controls = new List<Control>();
			Size = new Size(double.MaxValue,100);
			Location = new Point(0,0);
			CanGrow = true;
			BackgroundColor =  new Color(1,1,1);
			
		}		
				
		
		public List<Control> Controls {get;set;}
		
		public string Name {get;set;}
			
		public virtual void Format(){}
		
		public virtual void BeforePrint(){}
		
		public virtual void AfterPrint(){}
		
		public void CopyTo(Section s){
		
			CopyBasicProperties(s);
			s.Name = Name;			 		 			
 
			foreach (Control ctrl in Controls) {
				s.Controls.Add( ctrl.CreateControl() as Control);
			}
			
		}
		
		 
	}
}

