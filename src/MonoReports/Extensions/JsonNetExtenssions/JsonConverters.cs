// 
// JsonConverters.cs
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
using MonoReports.Model.Controls;
using Newtonsoft.Json.Converters;
using MonoReports.Model;
namespace MonoReports.Extensions
{
	public class PointConverter : CustomCreationConverter<Point>
	{
		public override Point Create (Type objectType)
		{
			return new Point (0, 0);
		}
		
		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
		{
			
			reader.Read();
			reader.Read();
			reader.Read();
			reader.Read();
			
			double x = double.Parse( reader.Value.ToString());
			
			reader.Read();
			
			reader.Read();
			double y = double.Parse( reader.Value.ToString());
			 
			reader.Read();
			return new Point(x,y);
		}
		
		
		
	
	}
	
	
	public class SizeConverter : CustomCreationConverter<Size>
	{
		public override Size Create (Type objectType)
		{
			return new Size (0, 0);
		}
		
		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
		{
			
			reader.Read();
			reader.Read();
			reader.Read();
			reader.Read();
			
			double w = double.Parse( reader.Value.ToString());
			
			reader.Read();
			
			reader.Read();
			double h = double.Parse( reader.Value.ToString());
			 
			reader.Read();
			return new Size(w,h);
		}
	
	}
	
	
	public class ColorConverter : CustomCreationConverter<MonoReports.Model.Color>
	{
		public override MonoReports.Model.Color Create (Type objectType)
		{
			return new MonoReports.Model.Color (0, 0,0);
		}
		
		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
		{			
			reader.Read();
			reader.Read();
			reader.Read();
			reader.Read();
			
			double r = double.Parse( reader.Value.ToString());
			reader.Read();	
			reader.Read();
			double g = double.Parse( reader.Value.ToString());
			reader.Read();	
			reader.Read();
			double b = double.Parse( reader.Value.ToString());
			
			reader.Read();
			reader.Read();
			double a = double.Parse( reader.Value.ToString());
			reader.Read();
			return new MonoReports.Model.Color(r,g,b,a);
		}
	
	}
}

