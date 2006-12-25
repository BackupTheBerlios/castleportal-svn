CC=mcs
OUT=bin/unit-tests.dll
CFLAGS=
DLLS=Castle.ActiveRecord \
   NHibernate Iesi.Collections log4net Castle.Windsor nunit.framework \
   Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine  NLog \
   Castle.Core castleportal NVelocity Castle.Windsor

CFLAGS+=$(patsubst %,-r:bin/%.dll,$(DLLS)) 

all: $(OUT)
$(OUT): *.cs  
	$(CC) -target:library $(CFLAGS)  $^ -out:$@

