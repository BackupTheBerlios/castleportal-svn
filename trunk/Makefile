CC=mcs
CFLAGS+=-debug
CFLAGS+=-define:CACHE 
CFLAGS+=-define:DEBUG
CFLAGS+=-define:NHLOGS
CFLAGS+=-define:CHAT
OUT=bin/castleportal.dll
include Makefile.config.orig
include Makefile.config
-include Makefile.project
CFLAGS+=$(patsubst %,-r:bin/%.dll,$(DLLS)) 
CFLAGS+=$(patsubst %,-r:%,$(MODULES)) 

RESOURCES= -resource:Resources/l10n/l10n.es.resources \
           -resource:Resources/l10n/l10n.en.resources \
           -resource:Resources/l10n/l10n.resources

all: $(OUT)
#	if pwd | grep ^/var/www ; then /etc/init.d/apache2 restart; else $(XSP) --port $(XSPPORT) $(XSPFLAGS); fi

$(OUT): $(MODELS) $(CONTROLLERS) $(HELPERS) $(WEBSERVICES) $(LIBRARIES) $(FILTERS) $(RAIZ) 
	$(CC) -target:library $(CFLAGS)  $(RESOURCES) -r:bin/NLog.dll -r:bin/log4net $^ -out:$@

#AR: 
#	sh activeRecord.sh $(wildcard $(MODELS))

dep: cleantodo
	ln -s Generator/dep_makefile.xml install.xml

fac: cleantodo
	ln -s Generator/fac_makefile.xml install.xml

gpc: cleantodo
	ln -s Generator/gpc_makefile.xml install.xml

cleantodo: 
	dropdb -U $(DBUSER) -W -h $(DBHOST) $(DBNAME) 
	su - postgres -c "createdb -E UNICODE -O $(DBUSER) $(DBNAME) "	
#	psql -U $(DBUSER) -h $(DBHOST) -p $(DBPORT) $(DBNAME) -c "\\d" | grep "public" | cut -b11- | sed 's/^/drop table "/; s/ *|.*$$/" cascade;/' > borratodo.sql
#	echo "DROP SEQUENCE hibernate_sequence;" >> borratodo.sql
#	-psql -U $(DBUSER) -h $(DBHOST) -p $(DBPORT) $(DBNAME) -f borratodo.sql
	rm -f $(OUT)* debug.log install.xml borratodo.sql
