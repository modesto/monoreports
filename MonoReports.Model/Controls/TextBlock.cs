// 
// TextBlock.cs
//  
// Author:
//       Tomasz Kubacki <Tomasz.Kubacki(at)gmail.com>
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
using System.Drawing;
using MonoReports.Model.Data;

namespace MonoReports.Model.Controls
{


	public class TextBlock : Control
	{	

		public TextBlock ():base()
		{
			Border = new Border() {WidthAll = 1, Color = System.Drawing.Color.Black};
			FontName = "Helvetica";
			FontColor = Color.Black;
			FieldName = String.Empty;
		}	
		
		
		public Border Border {get;set;}
		
		public string FontName {get;set;}
		
		
		public double Span {get;set;}
		
		public double LineSpan {get;set;}
		
		public double FontSize {get;set;}
		
		public FontSlant FontSlant {get;set;}
		
		public FontWeight FontWeight {get;set;}
		
		public Color FontColor {get;set;}
		
		public string FieldName {get;set;}
		
		public HorizontalAlignment HorizontalAlignment {get;set;}
		public VerticalAlignment VerticalAlignment {get;set;}
		
		public string Text {
			get;
			set;
		}
		#region implemented abstract members of MonoReports.Model.Controls.Control
		public override object Clone ()
		{
			TextBlock textBlock = new TextBlock();
			CopyBasicProperties(textBlock);			 
			textBlock.Border = (Border) Border.Clone();
			textBlock.FontName = FontName;
			textBlock.LineSpan = LineSpan;
			textBlock.Span = Span;
			textBlock.FontSize = FontSize;
			textBlock.FontSlant =   FontSlant;
			textBlock.FontWeight =   FontWeight;
			textBlock.FontColor =   Color.FromArgb(FontColor.ToArgb());
			textBlock.FieldName =   FieldName;
			textBlock.HorizontalAlignment =   HorizontalAlignment;
			textBlock.VerticalAlignment =   VerticalAlignment;
			textBlock.Text =   Text;
			return textBlock;
		}
		
		#endregion
		
		
		public override void AssignValue (Data.IDataSource source, DataRow row)
		{		
			if(!string.IsNullOrEmpty(this.FieldName) && source.ColumnIndex(this.FieldName) != -1){
				int index =  source.ColumnIndex(this.FieldName);				
				this.Text =  row[index];
			}
		}
	}
}
