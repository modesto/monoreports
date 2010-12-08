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
			pixbufDictionary = new Dictionary<string, Pixbuf>();
		}
		
		public PixbufRepository (Dictionary<string,byte[]> imagesRepository)
		{
			pixbufDictionary = new Dictionary<string, Pixbuf>();
			foreach (KeyValuePair<string, byte[]> kvp in imagesRepository) {
				pixbufDictionary.Add(kvp.Key,new Gdk.Pixbuf (kvp.Value));
			}
		}
		
		
		public Report Report {get;set;}
		
		
		public Dictionary<string,Pixbuf> pixbufDictionary {get;set;}
		
		
		public Pixbuf this [string key] {
			get {
				
				if(!pixbufDictionary.ContainsKey (key)){
					pixbufDictionary.Add(key,	 new Gdk.Pixbuf (Report.ResourceRepository[key]));
				}

				return pixbufDictionary[key];
			}
		}
		
		
		public bool ContainsKey (string key) {
			return pixbufDictionary.ContainsKey(key);
		}
		
		public void AddOrUpdatePixbufByName(string key) {
			if (Report.ResourceRepository.ContainsKey(key)) {
				var pixbuf = new Gdk.Pixbuf (Report.ResourceRepository[key]);
				if (pixbufDictionary.ContainsKey(key)) {
					Gdk.Pixbuf pb =  pixbufDictionary[key];
					pb.Dispose();
					pixbufDictionary[key] = pixbuf;
					
				} else {
					pixbufDictionary.Add(key,pixbuf);
				}
			}
		}
		
		public void DeletePixbufAtIndex(string key){
			var pixbuf = pixbufDictionary[key];			
			pixbufDictionary.Remove(key);
			pixbuf.Dispose();			
		}
		
		public int Count {
			get { return pixbufDictionary.Count; }		
		}
		
		
	}
}

