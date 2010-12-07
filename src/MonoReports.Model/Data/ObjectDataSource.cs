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
		string[] sortingFields;
    	bool nextRes = false;
		IEnumerator enumerator;
		int currentRowIndex = -1;
		List<Field> fields;
		Dictionary<string, Field> propertiesDictionary;
		Type rootObjectType;
		
		public int CurrentRowIndex {
			get { return this.currentRowIndex; }
			set { currentRowIndex = value; }
		}

	

		public ObjectDataSource (IEnumerable<T> data)
		{			 
			this.data = data as IEnumerable<T>;
			fields = new List<Field> ();
			rootObjectType = typeof(T);	
			DiscoverFields();
            nextRes = true;
		}
		

		public string GetValue (string fieldName, string format)
		{		
	        
			if (enumerator != null && propertiesDictionary.ContainsKey (fieldName)) {
                if (nextRes)
                {
                    var property = propertiesDictionary[fieldName] as Field;
					
                    return property.GetValue(enumerator.Current, format != null ? format : "{0}");                    
                }
			}

			return string.Empty;
		}


		public bool MoveNext ()
		{
            if (enumerator == null)
            {
                enumerator = data.GetEnumerator();
            }

            if (nextRes)
            {
                nextRes = enumerator.MoveNext();
                CurrentRowIndex++;
            }
            return nextRes;
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
			enumerator = data.GetEnumerator();
		}
		
		
		 

 
		static IOrderedQueryable<K> ApplyOrder<K>(IQueryable<K> source, string property, string methodName) {
        string[] props = property.Split('.');
        Type type = typeof(K);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;
        foreach(string prop in props) {           
            PropertyInfo pi = type.GetProperty(prop);
            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(K), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
		 
		 
        object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(K), type)
                .Invoke(null, new object[] {source, lambda});
        return (IOrderedQueryable<K>)result;
   }


		public void ApplySort (IEnumerable<string> sortingFields)
		{
			this.sortingFields = sortingFields.ToArray ();
			prepareObjectDataSource ();
		}

		public Field[] DiscoverFields ()
		{        	
			fields.Clear();
		    fields.AddRange( FieldBuilder.CreateFields(rootObjectType,"p",FieldKind.Data));
 			propertiesDictionary = fields.ToDictionary(pr=>pr.Name);
			return fields.ToArray ();
		}
 
		public void Reset ()
		{
			enumerator.Reset ();
			currentRowIndex = -1;
			
		}

		public object Current {
            get { return enumerator.Current; }
		}
		
		public bool ContainsField (string fieldName)
		{
			return propertiesDictionary.ContainsKey(fieldName);
		}
	}
}

