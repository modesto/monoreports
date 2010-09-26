// 
// Field.cs
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
using System.Reflection;

namespace MonoReports.Model.Data
{
	public abstract class  DataField
	{
		public DataField ()
		{
		}
		
		public virtual string Name {
			get;
			set;
		}
		
		 
		
		public abstract string GetValue(object current);
	}
	
	
	public class PropertyDataField : DataField {
		
		public PropertyInfo propertyInfo;
		
		public PropertyDataField( PropertyInfo pinfo){
			propertyInfo = pinfo;
		}
		
		public override string Name {
			get {
				return propertyInfo.Name;
			}
			set {
				;
			}
		}
		
		public override  string GetValue (object current)
		{
			
			 var val = propertyInfo.GetValue(current,null);
			
			 return val != null ? val.ToString() : null;
		}
		
	}
}

