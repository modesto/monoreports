// 
// FieldBuilderTest
//  
// Author:
//       Tomasz Kubacki <tomasz (dot ) kubacki (at) gmail (dot) com>
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
using NUnit.Framework;
using MonoReports.Model.Engine;
using MonoReports.Model;
using System.Linq;
using MonoReports.Model.Controls;
using MonoReports.Model.Data;
using System.Collections.Generic;

namespace MonoReports.Tests
{
	[TestFixture()]
	public class FieldBuilderTest
	{
				
		[Test()]
		public void CreateFields_ForClass_ReturnsFieldsWithAllProperties ()
		{
			TestClass tc = new TestClass();
		 	var fields = new List<Field>( FieldBuilder.CreateFields(tc,"kl",FieldKind.Data));
			
			Assert.AreEqual(4,fields.Count);
		
		}
		
		[Test()]
		public void CreateFields_ForString_ShouldCreateFieldWithName ()
		{
			 
		 	var fields = FieldBuilder.CreateFields(typeof(string),"sdf",FieldKind.Parameter);
			
			Assert.IsNotNull(fields);
			Assert.AreEqual(1,fields.Length);
			Assert.IsNotNull( fields[0].Name);
			
		}
		
		 
				
				
		public class TestClass {
			public string Name {
				get;
				set;
			}		
				
				
			public int Count {
				get;
				set;
			}
									
				
			public double Price {
				get;
				set;
			}	
				
			public DateTime Date {
				get;
				set;
			}					
				
		}
		
		
		public class InnerClass {
			
			public object Obj {
				get;
				set;
			}
		}
	}
}

