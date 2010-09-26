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
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace MonoReports.Model.Data
{
	public class ObjectDataSource<T> : IDataSource
	{
		IEnumerable<T> data;

		IEnumerator enumerator;
		int currentRowIndex = -1;

		public int CurrentRowIndex {
			get { return this.currentRowIndex; }
			set { currentRowIndex = value; }
		}

		List<DataField> fields;

		Dictionary<string, PropertyInfo> propertiesDict;


		public List<DataField> Fields {
			get { return this.fields; }
			set { fields = value; }
		}

		public ObjectDataSource (IEnumerable<T> data)
		{
			this.data = data as IEnumerable<T>;
			fields = new List<DataField> ();
			propertiesDict = typeof(T).GetProperties ().ToDictionary (pi => pi.Name);
		}



		public string GetValue (string fieldName, string format)
		{
			if (!string.IsNullOrEmpty (format))
				return string.Format (format, propertiesDict[fieldName].GetValue (current, null));
			else
				return propertiesDict[fieldName].GetValue (current, null).ToString ();
		}



		string[] sortingFields;


		object current = null;
		object next = null;
		bool curentRes = false;
		bool nextRes = false;

		public bool MoveNext ()
		{
			if (enumerator == null)
				enumerator = data.GetEnumerator ();
			
			if (CurrentRowIndex < 0) {
				curentRes = enumerator.MoveNext ();
				current = enumerator.Current;
			} else {
				current = next;
				curentRes = nextRes;
			}
			if (curentRes) {
				nextRes = enumerator.MoveNext ();
				if (nextRes) {
					next = enumerator.Current;
				}
			}
			CurrentRowIndex++;
			
			
			return curentRes;
		}

		bool isFirstOrdering;
		void prepareObjectDataSource ()
		{
			var queryableData = data.AsQueryable ();
			
			isFirstOrdering = true;
			if (sortingFields != null && sortingFields.Length > 0) {
				
				for (int i = 0; i < sortingFields.Length; i++) {
					
					if (isFirstOrdering) {
						queryableData = ApplyOrder<T> (queryableData, sortingFields[i], "OrderBy");
						isFirstOrdering = false;
					} else {
						queryableData = ApplyOrder<T> (queryableData, sortingFields[i], "ThenBy");
					}
				}
				
				
			}
			
			enumerator = queryableData.GetEnumerator ();
		}


		static IOrderedQueryable<W> ApplyOrder<W> (IQueryable<W> source, string property, string methodName)
		{
			string[] props = property.Split ('.');
			Type type = typeof(W);
			ParameterExpression arg = Expression.Parameter (type, "x");
			Expression expr = arg;
			foreach (string prop in props) {				
				PropertyInfo pi = type.GetProperty (prop);
				expr = Expression.Property (expr, pi);
				type = pi.PropertyType;
			}
			Type delegateType = typeof(Func<, >).MakeGenericType (typeof(W), type);
			LambdaExpression lambda = Expression.Lambda (delegateType, expr, arg);
			
			object result = typeof(Queryable).GetMethods ().Single (method => method.Name == methodName && method.IsGenericMethodDefinition && method.GetGenericArguments ().Length == 2 && method.GetParameters ().Length == 2).MakeGenericMethod (typeof(W), type).Invoke (null, new object[] { source, lambda });
			return (IOrderedQueryable<T>)result;
		}


		public void ApplySort (IEnumerable<string> sortingFields)
		{
			this.sortingFields = sortingFields.ToArray ();
			prepareObjectDataSource ();
		}

		public DataField[] DiscoverFields ()
		{
			List<DataField> datafields = new List<DataField> ();
			foreach (var kvp in propertiesDict) {
				datafields.Add (new PropertyDataField (kvp.Value));
			}
			return datafields.ToArray ();
		}


		public bool IsLast {
			get { return !nextRes; }
		}

		public void Reset ()
		{
			enumerator.Reset ();
			currentRowIndex = -1;
			
		}

		public object Current {
			get { return current; }
		}
		
		
	}
}

