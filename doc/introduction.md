Introduction to Monoreports
===========================

QuickStart
----------
If you can't wait to see something, here is quickstart to make 'hello world' report

1. run Monoreports designer

2. press datasource tab.

4. press execute button (at bottom)

5. go back to design tab

6. drag *name* field and drop on details section

7. press preview tab

8. press export to pdf icon

Run report from code
--------------------
If you don't like/need designer, there is a project demonstrating how to create and run report from code in ****doc/example/MrptInvoiceExample****.

Basic Concepts
==============

What is Monoreports ?
---------------------
Monoreports is a reporting tool used to design and generate reports from object datasources. Datasource can anything from database data or file to dynamically generated data.

Monoreports consists of two main parts:

***report designer***  gtk-sharp application to design reports
***report engine***  engine is responsible for generating report from report designed in designer and data pushed to datasource

Report
------
Report is a template for result report. Every report has serveral sections. In sections you place controls.
Controls can be bound to datasource or parameters  to display datasource data.

Sections
--------

Section is empty space on the report. Sections differ by location and [...]

1. ***Report Header*** the first section printed at report top. It's printed once per report.
2. ***Page Header*** this section is printed at top of every page except first page (where report header is first)
3. ***Details*** this section is printed as many times as many rows there is in the data source
4. ***Page Footer*** printed at bottom of the page
5. ***Report Footer*** printed at the end of report

Controls
--------
There are three basic types of controls:

1. Textblock - represents text on report, can be bound to datafield, has background, border and font related properties
2. Line  - can be vertical horizontal (to make things easier in a designer)
3. Image - at the moment only static images are supported

and one more complex:

***Subreport control*** (currently not supported)





