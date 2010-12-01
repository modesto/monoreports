// 
// TextBlockRenderer.cs
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
using MonoReports.Renderers;

namespace MonoReports.Tests
{
	[TestFixture()]
	public class TextBlockRendererTest
	{
		[Test()]
		public void BreakOffControlAtMostAtHeight_WithHeightInText_FirstControlAlwaysLowerOrEqualThanHeight ()
		{
			TextBlockRenderer tbr = new TextBlockRenderer();
			using(Cairo.PdfSurface pdf = new Cairo.PdfSurface( System.IO.Path.Combine( System.Environment.CurrentDirectory, "monoreports_tmp_test.pdf"),600,800)) {
				
				var cr = new  Cairo.Context(pdf);
				
				TextBlock tb = new TextBlock();
				tb.Text = loremIpsum;
				tb.Location = new Point(10,50);
				tb.Width = 89;
				tb.Height = 40;
				 
				Control[] tab = null;
				
			 	
				
				
				tab = tbr.BreakOffControlAtMostAtHeight(cr,tb,120);
				
				Assert.LessOrEqual(tab[0].Height,120);

						
				tab = tbr.BreakOffControlAtMostAtHeight(cr,tb,330);
				
				Assert.LessOrEqual(tab[0].Height,330);
				
				//chhamge padding
				
				tb.Padding = new Thickness(4,22,8,8);
				
				tab = tbr.BreakOffControlAtMostAtHeight(cr,tb,20);
				
				
			
				Assert.LessOrEqual(tab[0].Height,20);
				
				tab = tbr.BreakOffControlAtMostAtHeight(cr,tb,47);
				
				Assert.LessOrEqual(tab[0].Height,47);
				
				pdf.Finish();
			} 
					
		}
					
					
		static string loremIpsum = @" żółć
Lorem ipsum dolor sit amet, consectetur adipiscing elit. 


Integer semper mi  zółta łódź vitae justo placerat vitae bibendum elit facilisis. Duis tempus malesuada arcu vitae auctor. 

Etiam nec egestas erat. Mauris interdum pulvinar lectus, ac tempus sapien accumsan semper. Vestibulum posuere
laoreet sapien ut vestibulum. Quisque ac magna iaculis urna egestas hendrerit. 
Quisque vehicula bibendum imperdiet. Aliquam mattis mollis leo, eu vestibulum sapien ornare non. 
Mauris tempus ornare nisi, in blandit nisi suscipit sit amet. Vestibulum enim odio, suscipit c
Integer semper mi  zółta łódź vitae justo placerat vitae bibendum elit facilisis. Duis tempus malesuada arcu vitae auctor. 

Etiam nec egestas erat. Mauris interdum pulvinar lectus, ac tempus sapien accumsan semper. Vestibulum posuere
laoreet sapien ut vestibulum. Quisque ac magna iaculis urna egestas hendrerit. 
Quisque vehicula bibendum imperdiet. Aliquam mattis mollis leo, eu vestibulum sapien ornare non. 
Mauris tempus ornare nisi, in blandit nisi suscipit sit amet. Vestibulum enim odio, suscipit c
Integer semper mi  zółta łódź vitae justo placerat vitae bibendum elit facilisis. Duis tempus malesuada arcu vitae auctor. 


Quisque vehicula bibendum imperdiet. Aliquam mattis mollis leo, eu vestibulum sapien ornare non. 
Mauris tempus ornare nisi, in blandit nisi suscipit sit amet. Vestibulum enim odio, suscipit c
Integer semper mi  zółta łódź vitae justo placerat vitae bibendum elit facilisis. Duis tempus malesuada arcu vitae auctor. 

Etiam nec egestas erat. Mauris interdum pulvinar lectus, ac tempus sapien accumsan semper. Vestibulum posuere
laoreet sapien ut vestibulum. Quisque ac magna iaculis urna egestas hendrerit. 
Quisque vehicula bibendum imperdiet. Aliquam mattis mollis leo, eu vestibulum sapien ornare non. 
Mauris tempus ornare nisi, in blandit nisi suscipit sit amet. Vestibulum enim odio, suscipit c

Duis mollis mattis tortor, at semper sem sodales non. 

Integer semper mi  zółta łódź vitae justo placerat vitae bibendum elit facilisis. Duis tempus malesuada arcu vitae auctor. 

Etiam nec egestas erat. Mauris interdum pulvinar lectus, ac tempus sapien accumsan semper. Vestibulum posuere
laoreet sapien ut vestibulum. Quisque ac magna iaculis urna egestas hendrerit. 
Quisque vehicula bibendum imperdiet. Aliquam mattis mollis leo, eu vestibulum sapien ornare non. 
Mauris tempus ornare nisi, in blandit nisi suscipit sit amet. Vestibulum enim odio, suscipit convallis dictum sit amet,
lobortis id orci. Nulla facilisi. Donec tellus mi, fringilla sed dictum id, eleifend in libero. Nulla lobortis feugiat 
libero in viverra. Suspendisse nec diam ac dui tempor  ółłłłńń ććć porta ac sed augue. Phasellus iaculis, orci nec sollicitudin scelerisque,
purus massa accumsan risus, vitae bibendum arcu urna ut dui. Aenean non est at turpis porta porta. Vivamus pretium lobortis varius. 
";
		
	}
}

