Monoreports is a cross-platform  report designer and reporting engine for .net/mono  
It's similar to the CrystalReport (TM) and other tools like that.

Monoreports features
--------------------
- simple layouting - if control in section is growable and will grow due to assigned data, engine will do layouting to make report look properly
- page breaking - Monoreports engine will break or keep together report sections whatever is needed
- generating and running reports from designer and code. 

Simple code generated report example example:
		
		Report report = new Report;
		report.Details.Controls.Add(new Textblock(){ FieldName = "InvoiceNumber" });
		report.Datasource = myInvoicesCollection;
		report.ExportToPdf();
		
- pdf export
- reporting engine is not tightly coupled with gtk/cairo stuff, therefore it's reasonably easy to write new export backends (e.g. html, xls etc)

Status
------
Monoreports is in pre 0.1 version state. 


Binary version
--------------

You can grab current binary verision in monoreports downloads on github site

TODO
----
At the moment Monoreports lack of two features commonly used in this kind of tools.

- group section - group header section appears before details every time value of group expression is changed.
- subreports - report in report
 
Other this in roadmap are:

- improve designer to support multiselections, undo/redo
- imporove report engine quality by writing more unit tests


Final Remarks
-------------
I'm not a Novell employee and Monoreports is not a mono team (www.go-mono.org) project (however monoreports is using mono to run it on linux).
I've named this project Monoreports simply because i didn't have better idea for name.


