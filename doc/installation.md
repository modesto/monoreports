Installing Monoreports
================

Monoreports can be installed on Linux and Windows. 


Dependencies
------------

To build MonoReports depends on three managed libraries

Mono.CSharp - comes with mono installation
Newtonsoft.Json - can be found in src/lib directory


Installing from source
----------------------

1. install git

2. clone repository

git clone git://github.com/tomaszkubacki/monoreports.git

3. enter direcor and prepare 

	./configure --prefix=/opt/monoreports
	make
	make install

 

