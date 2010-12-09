$(if $(wildcard config.make),,$(error You need to run './configure' before running 'make'.))
include config.make
conf=Debug
SLN=src/MonoReports.sln
EXAMPLE=doc/example/MrptInvoiceExample/MrptInvoiceExample.sln
VERBOSITY=normal
version=0.0.1
XBUILD_ARGS=/verbosity:$(VERBOSITY) /nologo
srcdir_abs=$(shell pwd)
XBUILD=xbuild $(XBUILD_ARGS)
NUNIT_CONSOLE = nunit-console2


all: 
	@test -f config.make || (echo "You need to run ./configure." && exit 1)
	$(XBUILD) $(SLN) /property:Configuration=$(conf)

run-tests: all
	$(NUNIT_CONSOLE) build/MonoReports.Tests.dll

#update-docs: all
#	mdoc update -o ./docs/api/en ./build/MonoReports.Model.dll

clean:
	$(XBUILD) $(SLN) /property:Configuration=$(conf) /t:Clean
	rm -rf build/*
example: all
	$(XBUILD) $(EXAMPLE) /property:Configuration=$(conf)


#install: install-bin 


#install-docs:
#	test -d $(install_docs_dir) || install -d $(install_docs_dir)
#	cp -rf ./docs/* $(install_docs_dir)

#install-bin: all
#	test -d $(install_bin_dir) || install -d $(install_bin_dir)
#	cp -rf ./build/* $(install_bin_dir)

