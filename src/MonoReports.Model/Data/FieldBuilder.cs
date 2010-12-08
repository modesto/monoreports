// 
// ExpressionBuilder.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot) kubacki (at) gmail (dot ) com>
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
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace MonoReports.Model.Data
{
	public class FieldBuilder
	{
		public FieldBuilder ()
		{
		}
 
		public static IEnumerable<Field> CreateFields (object obj,string name, FieldKind fieldKind ) {
			Type rootObjectType = obj.GetType();
			foreach(var field in CreateFields (rootObjectType, name,  fieldKind )){				
			   	field.DefaultValue = obj;
				yield return field;
			}
			
		}
		
		public static Field[] CreateFields (Type rootObjectType,string name, FieldKind fieldKind ) {
			List<Field> fields = new List<Field>();
			 
		
			ParameterExpression arg = Expression.Parameter(rootObjectType, name);
			fillFields(rootObjectType,fields,arg,arg,name,rootObjectType,fieldKind);
			return fields.ToArray ();
		}
		
		static void fillFields(Type rootObjectType,List<Field> fields, ParameterExpression par,Expression parent,string namePrefix, Type t,FieldKind fieldKind) {
			
			if(t.IsPrimitive || t == typeof(string) || t == typeof(DateTime)){
				var field = CreateSimpleProperty(t,t,par,parent,namePrefix,fieldKind);
				field.Name = namePrefix;
				fields.Add(field);
			}else {
				PropertyInfo[] properties = t.GetProperties();
				foreach (var property in properties) {
					if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string) || property.PropertyType == typeof(DateTime)) {
	 					Field field = CreateSimpleProperty(rootObjectType,property.PropertyType,par,parent,property.Name,fieldKind);
						field.Name = string.IsNullOrEmpty(namePrefix) ?  property.Name : namePrefix + "." + property.Name;
						fields.Add(field);
					} else {
						
						if(property.PropertyType.IsArray)
							continue;						
						if(property.PropertyType.IsGenericType){
							var genType = property.PropertyType.GetGenericTypeDefinition();
							if((typeof(List<>)).IsAssignableFrom(genType))
								continue;
						}
						
						var expr = Expression.Property(parent,property.Name);
						fillFields(rootObjectType,fields, par,expr,property.Name,property.PropertyType,fieldKind);
					}
				}	
			}
		}
		
		
		public static Field  CreateSimpleProperty (Type rootObjectType,
			Type propertyType,
			ParameterExpression rootParameterExpression,
			Expression parentExpression,
			string name,
			FieldKind fieldKind) {
			var genericType = typeof(PropertyDataField<,>).MakeGenericType(rootObjectType, propertyType);
			var p = Activator.CreateInstance(genericType,rootParameterExpression,parentExpression,name) as Field;			 
			Field f = (p as Field);
			f.FieldKind = fieldKind;
			return f;			
		}
	}
}

