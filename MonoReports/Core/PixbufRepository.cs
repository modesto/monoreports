// 
// PixbufRepository.cs
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
using MonoReports.Model;
using Gdk;
using System.Collections.Generic;

namespace MonoReports.Core
{
	public class PixbufRepository
	{
		public PixbufRepository ()
		{
			pixbufDictionary = new Dictionary<int, Pixbuf>();
		}
		
		
		public Report Report {get;set;}
		
		
		public Dictionary<int,Pixbuf> pixbufDictionary {get;set;}
		
		
		public Pixbuf this [int index] {
			get {
				
				if(!pixbufDictionary.ContainsKey(index)){
					pixbufDictionary.Add(index,	 new Gdk.Pixbuf (Report.ResourceRepository[index]));
				}

				return pixbufDictionary[index];
			}
		}
		
		public void AddOrUpdatePixbufAtIndex(int index) {
			var pixbuf = new Gdk.Pixbuf (Report.ResourceRepository[index]);
			if (pixbufDictionary.ContainsKey(index)) {
				pixbufDictionary[index] = pixbuf;
			} else {
				pixbufDictionary.Add(index,pixbuf);
			}
		}
		
		public void DeletePixbufAtIndex(int index){
			var pixbuf = pixbufDictionary[index];			
			pixbufDictionary.Remove(index);
			pixbuf.Dispose();			
		}
		
		
	}
}

