// 
// ReportExtensions.cs
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
using MonoReports.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using MonoReports.Extensions.JsonNetExtenssions;
using Cairo;
using MonoReports.Core;
using MonoReports.Renderers;
using MonoReports.Model.Controls;
using MonoReports.Model.Data;

 

namespace MonoReports.Model
{
	public static class ReportExtensions
	{
		public static void Load (this Report r,string path) {
			using(System.IO.FileStream file = System.IO.File.OpenRead (path)) {				 
				byte[] bytes = new byte[file.Length];
				file.Read (bytes, 0, (int)file.Length);
				
				var report = JsonConvert.DeserializeObject<Report> (System.Text.Encoding.UTF8.GetString (bytes), 
					new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects,
					Converters = new List<JsonConverter> (
						new JsonConverter[] { 
						new MonoReports.Extensions.PointConverter (), 
						new MonoReports.Extensions.SizeConverter (),
						new MonoReports.Extensions.ColorConverter (),
					})  
				});
				report.CopyToReport(r);
				file.Close ();				 
			}
		}
		
		
		public static void Save(this Report report , string path) {		
			
			//TODO 3tk workaround hence can't serialize anonymous classes hidden in PropertyDataField<T,K> - 
			//should find way to serialize anonymous types			
			report.Parameters.Clear();
			report.DataFields.Clear();
			
			using (System.IO.FileStream file = System.IO.File.OpenWrite (path)) {
					var serializedProject = JsonConvert.SerializeObject (report,
					Formatting.None, 
					new JsonSerializerSettings { ContractResolver = new MonoReportsContractResolver(), TypeNameHandling = TypeNameHandling.Objects }
				);
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes (serializedProject);
				file.SetLength (bytes.Length);
				file.Write (bytes, 0, bytes.Length);
			
				file.Close ();
			}
		}
		
		public static void ExportToPdf(this Report report ,string path, IDictionary<string,object> parameters) {
			report.Parameters.Clear();
			
			foreach (KeyValuePair<string, object> kvp in parameters) {
					 report.Parameters.AddRange(FieldBuilder.CreateFields(kvp.Value, kvp.Key,FieldKind.Parameter));
			}
			
			
			report.ExportToPdf(path);			
		}
		
		public static void ExportToPdf(this Report report ,string path) {
			using (PdfSurface pdfSurface = new PdfSurface (
				path,report.WidthWithMargins,report.HeightWithMargins)) {
				Cairo.Context cr = new Cairo.Context (pdfSurface);
				cr.Translate(report.Margin.Left,report.Margin.Top);
				ReportRenderer renderer = new ReportRenderer (){ Context = cr};
				renderer.RegisterRenderer (typeof(TextBlock), new TextBlockRenderer ());
				renderer.RegisterRenderer (typeof(Line), new LineRenderer ());
				renderer.RegisterRenderer (typeof(Image), new ImageRenderer (){ PixbufRepository = new PixbufRepository(report.ResourceRepository)});
				SectionRenderer sr = new SectionRenderer();
				renderer.RegisterRenderer(typeof(ReportHeaderSection), sr);
				renderer.RegisterRenderer(typeof(ReportFooterSection), sr);
				renderer.RegisterRenderer(typeof(DetailSection), sr);
				renderer.RegisterRenderer(typeof(PageHeaderSection), sr);
				renderer.RegisterRenderer(typeof(PageFooterSection), sr);
				MonoReports.Model.Engine.ReportEngine engine = new MonoReports.Model.Engine.ReportEngine (report,renderer);
				engine.Process ();
				for (int i = 0; i < report.Pages.Count; ++i) {
					renderer.RenderPage (report.Pages [i]);
					cr.ShowPage ();
				}			
				pdfSurface.Finish ();		
				(cr as IDisposable).Dispose ();
			}
		}
		
		
		
	}
}

