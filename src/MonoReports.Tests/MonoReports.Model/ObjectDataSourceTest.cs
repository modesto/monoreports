// 
// ObjectDataSourceTest.cs
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
	public class ObjectDataSourceTest
	{
		
		[Test()]
		public void Reset_AfterFirstIteration_MakesSourceIterableAgain ()
		{
			ObjectDataSource<int> objds = new ObjectDataSource<int>(new int []{1,2,3,4,9});
			
			int count = 0;
			int count1 = 0;
			while( objds.MoveNext())
				count++;
			
			Assert.IsFalse(objds.MoveNext());
			
		    objds.Reset();
			
			while( objds.MoveNext())
				count1++;
			
			Assert.AreEqual(count,count1);
		}
	}
}

