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
	public class GenericEnumerableDataSource<T> : IDataSource
	{
		IEnumerable<T> data;
		Dictionary<string,int> columnIndeces;


		public GenericEnumerableDataSource (IEnumerable<T> data)
		{
			this.data = data;
			
			columnIndeces = new Dictionary<string, int>();
			int i = 0;
			
			
		}


		#region IDataSource implementation
		
		

		public IList<DataRow> GetRows ()
		{
			List<DataRow> rows = new List<DataRow> (100);
			//3tk change IDataSource !!!!
//			var enumerator = data.GetEnumerator ();
//			while (enumerator.MoveNext ()) {
//				DataRow row = new DataRow ();
//				row.Values = new string[Columns.Count];
//				for (int i = 0; i < Columns.Count; i++) {
//					row.Values[i] = Columns[i].GetValue (enumerator.Current);
//				}
//				rows.Add (row);
//			}
			
			return rows;
		}


		public int ColumnIndex (string columnName)
		{
			return columnIndeces[columnName];
		}
		#endregion
		
	}
}

