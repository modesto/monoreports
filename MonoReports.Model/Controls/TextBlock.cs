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
using MonoReports.Model.Data;

	
namespace MonoReports.Model.Controls
{


	public class TextBlock : Control,IResizable
	{	

		public TextBlock ():base()
		{
			Border = new Border() {WidthAll = 1, Color = new Color(0,0,0)};
			FontName = "Helvetica";
			FontColor = new Color(0,0,0);
			FieldName = String.Empty;
			Padding = new Padding(1,1,1,1);
			CanGrow = true;
		}	
		
		
		public Border Border {get;set;}
		
		public string FontName {get;set;}
		
		public bool CanGrow {get;set;}		
		
		public bool CanShrink {get;set;}        
		
		public Padding Padding {get;set;}
		
		public double LineSpan {get;set;}
		
		public double FontSize {get;set;}
		
		public FontSlant FontSlant {get;set;}
		
		public FontWeight FontWeight {get;set;}
		
		public Color FontColor {get;set;}
		
		public string FieldName {get;set;}
		
		public string FieldTextFormat {get;set;}
		
		public HorizontalAlignment HorizontalAlignment {get;set;}
		public VerticalAlignment VerticalAlignment {get;set;}
		
		public string Text {
			get;
			set;
		}
		#region implemented abstract members of MonoReports.Model.Controls.Control
		public override Control CreateControl ()
		{
			TextBlock textBlock = new TextBlock();
			CopyBasicProperties(textBlock);		
			textBlock.CanGrow = CanGrow;
			textBlock.CanShrink = CanShrink;
			textBlock.Border = (Border) Border.Clone();
			textBlock.FontName = FontName;
			textBlock.LineSpan = LineSpan;
			textBlock.Padding = new Padding(Padding.Left,Padding.Top, Padding.Right, Padding.Bottom);
			textBlock.FontSize = FontSize;
			textBlock.FontSlant =   FontSlant;
			textBlock.FontWeight =   FontWeight;
			textBlock.FontColor =   new Color(FontColor.R,FontColor.G,FontColor.B,FontColor.A);
			textBlock.FieldName =   FieldName;
			textBlock.HorizontalAlignment =   HorizontalAlignment;
			textBlock.VerticalAlignment =   VerticalAlignment;
			textBlock.Text =   Text;
			return textBlock;
		}
		
		#endregion
		
		
		public override void AssignValue (IDataSource dataSource)
		{		
			if(!string.IsNullOrEmpty(this.FieldName) && dataSource != null){
				 			
				this.Text =  dataSource.GetValue(this.FieldName,FieldTextFormat);
			}
		}
	}
}
