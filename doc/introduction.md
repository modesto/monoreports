Introduction to Monoreports
===========================


QuickStart
----------
If you can't wait/don't have time here is quickstart to make report and see "something working"

1. run Monoreports designer

2. press datasource tab.

3. copy below and paste in textview:

	new [] { 
		new { name = "Alfred", surname="Tarski"},
		new { name = "Stefan", surname="Banach"},
		new { name = "Stanisław", surname="Leśniewski"}
	};

4. press execute button (at bottom)

5. press design tab

6. on right report explorer treeview expand data fields node

7. drag name field and drop on details section

8. press preview tab

9. press export to pdf icon


Basic Concepts
==============

Monoreports is a reporting tool to design and generate reports from datasources. Datasource can anything from database data or file to dynamically generated data.

Report Designer
---------------
Report designer is a gui application where you can design report layout. Report consists of sections which consist of controls.

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

Sections are filled with controls of different type. Most popular are:

1. Textblock 
2. Line 
3. Image




