// 
// CompilerService.cs
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
using Mono.CSharp;

namespace MonoReports.Services
{
	public class CompilerService
	{
		public CompilerService ()
		{
		}
		
		
		public void Init(){
			Evaluator.MessageOutput = Console.Out;
			
			Evaluator.Init (new string[0]);
			AppDomain.CurrentDomain.AssemblyLoad += delegate (object sender, AssemblyLoadEventArgs e)
			{
			
				Evaluator.ReferenceAssembly (e.LoadedAssembly);			
			};
			
			// Add all currently loaded assemblies
			//			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies ()) {
			//				try {
			//					Evaluator.ReferenceAssembly (a);
			//				} catch (Exception exp) {
			//					Console.WriteLine (exp.ToString ());
			//				}
			//			}
			object obj;
			string msg;
			Evaluate ("using System; using System.Linq; using System.Collections.Generic; using System.Collections;",out obj,out msg);
			
		}
 
		
		
		public bool Evaluate (string input, out object result, out string message)
		{
			
			bool result_set = false;
			object res = new object();
			string m = String.Empty;
			
			try {
				input = Evaluator.Evaluate (input, out res, out result_set);

				if (result_set) {				
					m = result.ToString ();					 
				}
			} catch (Exception e) {
				m = e.ToString();		
				
			}
			result = res;	
			message = m;
			return result_set;
			
		}
	}
}

