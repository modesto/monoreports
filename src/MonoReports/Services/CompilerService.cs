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
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text;
using System.CodeDom;
using System.Collections.Generic;

namespace MonoReports.Services
{
	public class CompilerService
	{
		public CompilerService ()
		{
		}

		public void Init ()
		{
			//Evaluator.MessageOutput = Console.Out;
			
			//Evaluator.Init (new string[0]);
			//AppDomain.CurrentDomain.AssemblyLoad += delegate (object sender, AssemblyLoadEventArgs e)
			//{
			
			//	Evaluator.ReferenceAssembly (e.LoadedAssembly);			
			//};
			
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
			Evaluate ("using System; using System.Linq; using System.Collections.Generic; using System.Collections;", out obj, out msg);
			
		}

		public bool Evaluate (string input, out object result,out string message)
		{
			bool result_set = false;
			message = String.Empty;
			result = new  object ();
			
			LanguageCompiler cmp = new LanguageCompiler (LangType.CSharp,false,true,true);
			cmp.Code = @"
using System;
public class Te {
    public object Test()
    { 
        var cos = " + input.Replace("\n","") + @";

        return cos;
    }
}

";
			result_set = cmp.RunCode ("Te", "Test", new object[]{});
			if (result_set) {
				result = cmp.ResultObject;
			} else {
				message = cmp.CompileErrors;
			}

			return result_set;
			
		}

		[Serializable]
		public enum LangType
		{
			CSharp,
			VBNet
		}

		public class LanguageCompiler
		{
		// Fields
			private string code;
			private StringBuilder compileErrors = new StringBuilder ();
			private CompilerParameters parms;

		// private CodeDomProvider provider = null;
			private CompilerResults results;
			public object ResultObject;
			private StringBuilder runErrors = new StringBuilder ();
			private LangType language;
		// Methods
			public LanguageCompiler (LangType lang, bool generateExecutable, bool generateInMemory, bool includeDebugInformation)			{
				language = lang;
				this.parms = new CompilerParameters ();
				this.parms.GenerateExecutable = generateExecutable;
				this.parms.GenerateInMemory = generateInMemory;
				this.parms.IncludeDebugInformation = includeDebugInformation;
			}

			public void addReferencedAssembly (string assembly)
			{
				this.parms.ReferencedAssemblies.Add (assembly);
			}

			public bool CompileCode ()
			{
				this.compileErrors = new StringBuilder ();
				this.results = new NetLanguageProviderFactory ().createLanguageCompiler (language).CompileAssemblyFromSource (this.parms, new string[] { this.code });
				if (this.results.Errors.Count > 0) {
					foreach (CompilerError error in this.results.Errors) {

						this.compileErrors.Append (error.ToString () + System.Environment.NewLine + error.ErrorText);
					}
				} else {
					this.compileErrors.Append ("Compilation success!" + System.Environment.NewLine);
					return true;
				}
				return false;
			}

			public void replaceTokens (string key, string value)
			{
				this.code = this.code.Replace (key, value);
			}

			public bool RunCode (string className, string methodName, object[] arguments)
			{
				if (this.CompileCode ()) {
					Type type = this.results.CompiledAssembly.GetType (className);
					object obj2 = this.results.CompiledAssembly.CreateInstance (className);
					try {
						if ((type != null) && (obj2 != null)) {
							ResultObject = type.GetMethod (methodName).Invoke (obj2, arguments);
							return true;
						} 
					} catch (Exception exception) {
						this.runErrors.Append ("error while running code " + className + System.Environment.NewLine + exception.ToString ());
					}
					
				}else {					
					this.runErrors.Append (this.CompileErrors);
				}
				return false;
			}

		// Properties
			public string Code {
				get {
					return this.code;
				}
				set {
					this.code = value;
				}
			}

			public string CompileErrors {
				get {
					return this.compileErrors.ToString ();
				}
			}

			public CompilerParameters Parms {
				get {
					return this.parms;
				}
				set {
					this.parms = value;
				}
			}

			public CompilerResults Results {
				get {
					return this.results;
				}
				set {
					this.results = value;
				}
			}

			public string RunErrors {
				get {
					return this.runErrors.ToString ();
				}
			}
		}

		public class NetLanguageProviderFactory
		{
			public CodeDomProvider createLanguageCompiler (LangType lang)
			{
				switch (lang) {
				case LangType.CSharp:
					return  new CSharpCodeProvider (new Dictionary<string, string>() { { "CompilerVersion", "v3.5" }});
                    

				}
				return null;
			}
		}

	}
}

