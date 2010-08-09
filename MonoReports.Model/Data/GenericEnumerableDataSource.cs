// 
// SimpleDataRow.cs
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
using System.Collections.Generic;
using System.Collections;

namespace MonoReports.Model.Data
{
	public class GenericEnumerableDataSource<T>  : IDataSource
	{
		IEnumerable<T> list;
		
		
		
		public GenericEnumerableDataSource(IEnumerable<T> list){	
			this.list = list;
			Columns = new Dictionary<string, DataColumn>();
			 
			foreach (var p in  typeof(T).GetProperties()){				
				DataColumn dc = new PropertyDataColumn(this,p);
			 
				Columns.Add(dc.Name,dc);
			}
			 
		}
		
		
		#region IDataSource implementation
		 public Dictionary<string,DataColumn> Columns {get;set;}
		 
		public IEnumerator GetEnumerator ()
		{
			return list.GetEnumerator();
		}
		

		 
		public string GetValue (string columnName, object current)
		{	 if(Columns.ContainsKey(columnName))
			 	return Columns[columnName].GetValue(current);
			  else 
			      return string.Empty;
		}
		 

 
#endregion
		 
 
	}
}

