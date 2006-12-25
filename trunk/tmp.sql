--
-- PostgreSQL database dump
--

SET client_encoding = 'UTF8';
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS 'Standard public schema';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: acl; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE acl (
    acl_id integer NOT NULL,
    group_id integer,
    role_id integer
);


ALTER TABLE public.acl OWNER TO carlos;

--
-- Name: category; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE category (
    categoryid integer NOT NULL,
    creationdate timestamp without time zone,
    modificationdate timestamp without time zone,
    description character varying(255),
    information text,
    parent integer,
    "template" integer
);


ALTER TABLE public.category OWNER TO carlos;

--
-- Name: chat; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE chat (
    id integer NOT NULL,
    name character varying(255),
    ogroup integer
);


ALTER TABLE public.chat OWNER TO carlos;

--
-- Name: chatmessage; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE chatmessage (
    id integer NOT NULL,
    number integer,
    message character varying(255),
    date timestamp without time zone,
    chat integer,
    "owner" integer
);


ALTER TABLE public.chatmessage OWNER TO carlos;

--
-- Name: configcombo; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE configcombo (
    configcomboid integer NOT NULL,
    "key" character varying(255),
    name character varying(255),
    val character varying(255),
    description character varying(255)
);


ALTER TABLE public.configcombo OWNER TO carlos;

--
-- Name: configmodel; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE configmodel (
    id integer NOT NULL,
    "key" character varying(255),
    val character varying(255),
    description character varying(255)
);


ALTER TABLE public.configmodel OWNER TO carlos;

--
-- Name: container; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE container (
    id integer NOT NULL,
    name character varying(255),
    "owner" integer,
    anonrole integer,
    lang integer
);


ALTER TABLE public.container OWNER TO carlos;

--
-- Name: containeracl; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE containeracl (
    acl_id integer NOT NULL,
    container_id integer NOT NULL
);


ALTER TABLE public.containeracl OWNER TO carlos;

--
-- Name: content; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE content (
    id integer NOT NULL,
    creationdate timestamp without time zone,
    published boolean,
    category integer,
    lang integer
);


ALTER TABLE public.content OWNER TO carlos;

--
-- Name: datamodel; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE datamodel (
    id integer NOT NULL,
    fieldname character varying(255),
    value text,
    field integer,
    content integer
);


ALTER TABLE public.datamodel OWNER TO carlos;

--
-- Name: field; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE field (
    id integer NOT NULL,
    name character varying(255),
    description character varying(255),
    "type" integer
);


ALTER TABLE public.field OWNER TO carlos;

--
-- Name: fields_template; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE fields_template (
    field_id integer NOT NULL,
    template_id integer NOT NULL
);


ALTER TABLE public.fields_template OWNER TO carlos;

--
-- Name: fieldtemplate; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE fieldtemplate (
    id integer NOT NULL,
    orderlist integer,
    orderedit integer,
    field integer,
    "template" integer
);


ALTER TABLE public.fieldtemplate OWNER TO carlos;

--
-- Name: file; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE file (
    id integer NOT NULL,
    name character varying(255),
    filename character varying(255),
    contenttype character varying(255),
    createdate timestamp without time zone,
    size integer,
    directory character varying(255)
);


ALTER TABLE public.file OWNER TO carlos;

--
-- Name: forum; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE forum (
    id integer NOT NULL,
    title character varying(255),
    description character varying(255),
    date timestamp without time zone,
    forumgroup integer
);


ALTER TABLE public.forum OWNER TO carlos;

--
-- Name: forumfolder; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE forumfolder (
    id integer NOT NULL,
    title character varying(255),
    description character varying(255),
    date timestamp without time zone,
    forum integer,
    parent integer
);


ALTER TABLE public.forumfolder OWNER TO carlos;

--
-- Name: forummessage; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE forummessage (
    id integer NOT NULL,
    title character varying(255),
    body character varying(255),
    date timestamp without time zone,
    "level" integer,
    forumfolder integer,
    parent integer,
    "owner" integer
);


ALTER TABLE public.forummessage OWNER TO carlos;

--
-- Name: group; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE "group" (
    id integer NOT NULL,
    name character varying(255)
);


ALTER TABLE public."group" OWNER TO carlos;

--
-- Name: group_role; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE group_role (
    role_id integer NOT NULL,
    group_id integer NOT NULL
);


ALTER TABLE public.group_role OWNER TO carlos;

--
-- Name: hibernate_sequence; Type: SEQUENCE; Schema: public; Owner: carlos
--

CREATE SEQUENCE hibernate_sequence
    INCREMENT BY 1
    NO MAXVALUE
    NO MINVALUE
    CACHE 1;


ALTER TABLE public.hibernate_sequence OWNER TO carlos;

--
-- Name: hibernate_sequence; Type: SEQUENCE SET; Schema: public; Owner: carlos
--

SELECT pg_catalog.setval('hibernate_sequence', 173, true);


--
-- Name: language; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE "language" (
    id integer NOT NULL,
    name character varying(255),
    englishname character varying(255),
    description character varying(255)
);


ALTER TABLE public."language" OWNER TO carlos;

--
-- Name: menu; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE menu (
    id integer NOT NULL,
    ordering integer,
    "show" integer,
    name character varying(255),
    description character varying(255),
    url character varying(255),
    categoryid integer,
    parent integer,
    lang integer
);


ALTER TABLE public.menu OWNER TO carlos;

--
-- Name: menutranslation; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE menutranslation (
    id integer NOT NULL,
    translation character varying(255),
    menu integer,
    lang integer
);


ALTER TABLE public.menutranslation OWNER TO carlos;

--
-- Name: role; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE "role" (
    id integer NOT NULL,
    name character varying(255),
    cancreate boolean,
    canmodify boolean,
    candelete boolean,
    canpublish boolean,
    canread boolean
);


ALTER TABLE public."role" OWNER TO carlos;

--
-- Name: template; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE "template" (
    id integer NOT NULL,
    name character varying(255),
    public boolean,
    description character varying(255),
    tlist character varying(255),
    tview character varying(255),
    tedit character varying(255)
);


ALTER TABLE public."template" OWNER TO carlos;

--
-- Name: type; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE "type" (
    id integer NOT NULL,
    name character varying(255),
    description character varying(255)
);


ALTER TABLE public."type" OWNER TO carlos;

--
-- Name: typetranslation; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE typetranslation (
    id integer NOT NULL,
    translation character varying(255),
    "type" integer,
    lang integer
);


ALTER TABLE public.typetranslation OWNER TO carlos;

--
-- Name: user; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE "user" (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    userpassword character varying(255),
    sessiongroup integer
);


ALTER TABLE public."user" OWNER TO carlos;

--
-- Name: usersgroups; Type: TABLE; Schema: public; Owner: carlos; Tablespace: 
--

CREATE TABLE usersgroups (
    groupid integer NOT NULL,
    userid integer NOT NULL
);


ALTER TABLE public.usersgroups OWNER TO carlos;

--
-- Data for Name: acl; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY acl (acl_id, group_id, role_id) FROM stdin;
67	22	21
68	22	18
70	22	20
72	23	19
73	23	18
74	23	21
76	24	18
77	24	19
78	24	20
69	22	19
71	23	20
75	24	21
\.


--
-- Data for Name: category; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY category (categoryid, creationdate, modificationdate, description, information, parent, "template") FROM stdin;
79	-infinity	-infinity	main	\N	\N	43
80	-infinity	-infinity	left	\N	\N	43
81	-infinity	-infinity	right	\N	\N	43
82	-infinity	-infinity	Home	\N	79	41
83	-infinity	-infinity	News	\N	82	64
84	-infinity	-infinity	Web Links	\N	79	50
85	-infinity	-infinity	Support	\N	79	53
86	-infinity	-infinity	Downloads	\N	79	47
87	-infinity	-infinity	About	\N	79	53
88	-infinity	-infinity	Contribute	\N	79	53
89	-infinity	-infinity	Documentation	\N	80	41
90	-infinity	-infinity	Installation	\N	89	53
91	-infinity	-infinity	Users Guide		89	41
92	-infinity	-infinity	Developers Guide	\N	89	53
93	-infinity	-infinity	Roadmap		80	53
94	-infinity	-infinity	TO-DO List	\N	80	60
140	-infinity	-infinity	Categories and Contents		91	53
141	-infinity	-infinity	Users, Groups and Permissions		91	53
142	-infinity	-infinity	Look and Feel		91	53
144	-infinity	-infinity	WebSite Publishing		91	53
\.


--
-- Data for Name: chat; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY chat (id, name, ogroup) FROM stdin;
\.


--
-- Data for Name: chatmessage; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY chatmessage (id, number, message, date, chat, "owner") FROM stdin;
\.


--
-- Data for Name: configcombo; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY configcombo (configcomboid, "key", name, val, description) FROM stdin;
5	header	\N	gradientA	Sample 1
6	header	\N	gradientB	Sample 2
7	color	\N	1	Blue
8	color	\N	2	Yellow
\.


--
-- Data for Name: configmodel; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY configmodel (id, "key", val, description) FROM stdin;
4	published	0	Published
1	layout	layout_castleportal	Layout
2	header	gradientA	Header
3	color	1	Color
\.


--
-- Data for Name: container; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY container (id, name, "owner", anonrole, lang) FROM stdin;
79	main	\N	18	\N
80	left	\N	18	\N
81	right	\N	18	\N
82	Home	\N	18	\N
83	News	\N	18	\N
84	Web Links	\N	18	\N
85	Support	\N	18	\N
86	Downloads	\N	18	\N
87	About	\N	18	\N
88	Contribute	\N	18	\N
89	Documentation	\N	18	\N
90	Installation	\N	18	\N
91	Users Guide	116	18	\N
92	Developers Guide	\N	18	\N
93	Roadmap	116	18	\N
94	TO-DO List	\N	18	\N
140	Categories and Contents	116	18	\N
141	Users, Groups and Permissions	116	18	\N
142	Look and Feel	116	\N	\N
144	WebSite Publishing	116	\N	\N
\.


--
-- Data for Name: containeracl; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY containeracl (acl_id, container_id) FROM stdin;
69	79
71	79
75	79
69	80
71	80
75	80
69	81
71	81
75	81
69	82
71	82
75	82
69	83
71	83
75	83
69	84
71	84
75	84
69	85
71	85
75	85
69	86
71	86
75	86
69	87
71	87
75	87
69	88
71	88
75	88
69	89
71	89
75	89
69	90
71	90
75	90
69	92
71	92
75	92
69	94
71	94
75	94
69	140
71	140
75	140
69	91
71	91
75	91
69	141
71	141
75	141
69	142
71	142
75	142
69	144
71	144
75	144
69	93
71	93
75	93
\.


--
-- Data for Name: content; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY content (id, creationdate, published, category, lang) FROM stdin;
117	2006-12-18 19:20:21	f	87	112
119	2006-12-18 19:21:58	f	83	112
122	2006-12-18 19:43:30	f	88	112
124	2006-12-18 19:44:14	f	84	112
127	2006-12-18 19:44:28	f	84	112
130	2006-12-18 19:44:39	f	84	112
133	2006-12-18 19:44:50	f	84	112
136	2006-12-18 19:45:46	f	85	112
138	2006-12-18 19:47:29	f	90	112
154	2006-12-18 20:26:57	f	93	112
145	2006-12-18 20:15:30	f	94	112
156	2006-12-18 20:37:30	f	144	112
158	2006-12-18 20:37:58	f	142	112
160	2006-12-18 20:40:06	f	140	112
162	2006-12-18 20:43:30	f	141	112
164	2006-12-18 20:52:00	f	90	112
\.


--
-- Data for Name: datamodel; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY datamodel (id, fieldname, value, field, content) FROM stdin;
118	body	<p>CastlePortal is a <strong>Content Management System</strong> written over the <a href="http://castleproject.org">CastleProject</a> framework. <br />It has been developed by <a href="http://shidix.com">Shidix Technologies</a> using CastelProject and Mono on Linux. </p><p>CastlePortal is inspired on the idea of <a href="http://mamboxchange.com/projects/clasifier/">Mambo&#39;s Clasifier component</a> which lets the user create and manage virtual tables by selecting the columns and their types from scracth. Contents are grouped by categories and categories can be nested .</p><p>CastlePortal licence is Apache License  2.0</p><br /><br />	27	117
120	title	CastlePortal 0.1 Beta demo is up	26	119
121	body	CastlePortal 0.1 Beta is up!. There are dozens of changes to perform before 0.1 release. The most important are the performance issues. Please be patient. 	27	119
123	body	Join the project !!!<br />	27	122
125	title	Shidix Technologies	26	124
126	url	www.shidix.com	30	124
128	title	CastleProject	26	127
129	url	www.castleproject.org	30	127
131	title	Mono Project	26	130
132	url	www.mono-project.com	30	130
134	title	Carlos Ble's Blog	26	133
135	url	shidix.com/carlosble	30	133
137	body	Support coming soon. Let me release the project first :) 	27	136
139	body	Just folow the instructions to install any MonoRail application. <br />Edit <strong>web.config</strong> and put your database connection string. Database tables will be create automaticly the first time you access the web.  	27	138
155	body	<p>List of tasks to perform before 0.2 release</p><ul><li>Refactor Views and Helpers by replacing some of them with ViewComponents </li><li>Write a controller to manage headers, styles and layouts</li><li>Write NAnt build scripts</li><li>Get the project working on Windows + IIS</li><li>Get the project working on MySQL (not tested yet)</li><li>Add IBrowser component to TinyMCE in addition to our files manager.</li><li>Jump to .Net 2.0 (on Mono): Change current models for ActiveRecord Generics. Use partial classes for PortalController. A lot of other enhancements....</li><li>Work on security: Check parameters sent to controllers, avoid injection and so</li><li>Investigate the PrincipalPermission attribute instead of current Commons.CheckSuperUser()</li><li>Use &quot;capturefor&quot; to load tinymce on ajax instead of the current complex hack<br /></li></ul>	27	154
146	title	List of tasks to complete before 0.1 release	26	145
147	body	<ul><li>Menus and FieldTypes internationalization (performance issues)</li><li>Third level cache for configmodel table to avoid read database at Filters/ConfigLoad</li><li>Write more user documentation</li><li>Cool website desing (layout, images, icons, styles...)</li><li>Accesible layout (not the current joke)</li><li>Change Generator/*  and xml data files to english</li><li>Write acceptance tests using Selenium</li><li>Write more unit tests<br /></li><li>Test the application manually quite a lot more</li><li>Translate all fucking spanish source code</li><li>Get the internationalization support working within rescues  <br /></li></ul>	27	145
157	body	By default, a website is not public. You have to click the &quot;Publish&quot; button at the top to publish it. Notice that once published you can&#39;t change the look and feel by changing the stylesheet nor the header.  	27	156
159	body	Just change the style and the header using the topmost combos<br />	27	158
161	body	Every category can contain nested categories. Every category has a template which has a number of fields and a custom view. Eventually a category contains contents. A content can be published or not. Permissions work on categories not on contents<br />	27	160
163	body	Every user could be in one or more groups. Every group has a default role that can be used to update all group access control lists. An access control list has a group, a role and a list of categories to apply the group role.<br />Although there are roles, the main permissions are: read, modify, create, delete and publish. 	27	162
165	body	At the moment CastlePortal have been tested with PostgreSQL only, and with Mono on Linux only<br /> 	27	164
\.


--
-- Data for Name: field; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY field (id, name, description, "type") FROM stdin;
25	name	Name	16
26	title	Title	16
27	body	Body	12
28	postcod	Post Code	11
29	link	Link	13
30	url	Url	13
31	date	Date	9
32	file	File	10
33	image	Image	10
34	mail	Email address	14
35	flash	Flash Movie	17
\.


--
-- Data for Name: fields_template; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY fields_template (field_id, template_id) FROM stdin;
\.


--
-- Data for Name: fieldtemplate; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY fieldtemplate (id, orderlist, orderedit, field, "template") FROM stdin;
45	1	1	27	44
46	2	2	32	44
48	1	1	26	47
49	2	2	32	47
51	1	1	26	50
52	2	2	30	50
54	1	1	27	53
56	1	1	26	55
57	2	2	33	55
59	1	1	26	58
61	1	1	26	60
62	2	2	27	60
65	1	1	26	64
66	-1	2	27	64
\.


--
-- Data for Name: file; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY file (id, name, filename, contenttype, createdate, size, directory) FROM stdin;
\.


--
-- Data for Name: forum; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY forum (id, title, description, date, forumgroup) FROM stdin;
\.


--
-- Data for Name: forumfolder; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY forumfolder (id, title, description, date, forum, parent) FROM stdin;
\.


--
-- Data for Name: forummessage; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY forummessage (id, title, body, date, "level", forumfolder, parent, "owner") FROM stdin;
\.


--
-- Data for Name: group; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY "group" (id, name) FROM stdin;
22	Editors
24	Admins
23	Publishers
\.


--
-- Data for Name: group_role; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY group_role (role_id, group_id) FROM stdin;
18	22
19	22
18	23
20	23
18	24
19	24
20	24
21	24
\.


--
-- Data for Name: language; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY "language" (id, name, englishname, description) FROM stdin;
112	en	English	UK-US
113	es	Spanish	España
\.


--
-- Data for Name: menu; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY menu (id, ordering, "show", name, description, url, categoryid, parent, lang) FROM stdin;
95	1	0	main	main		79	\N	\N
96	1	0	Home	Home		82	95	\N
97	2	0	About	About		87	95	\N
98	3	0	News	News		83	95	\N
99	4	0	Downloads	Downloads		86	95	\N
100	5	0	Support	Support		85	95	\N
101	6	0	Contribute	Contribute		88	95	\N
102	7	0	Web Links	Web Links		84	95	\N
103	8	0	Site Map	Site Map	portal/showsitemap.aspx	0	95	\N
104	2	0	left	left		80	\N	\N
105	1	0	Documentation	Documentation		89	104	\N
106	1	0	Installation	Installation		90	105	\N
107	2	0	Users Guide	Users Guide		91	105	\N
108	3	0	Developers Guide	Developers Guide		92	105	\N
109	2	0	Roadmap	Roadmap		93	104	\N
110	3	0	TO-DO List	TO-DO List		94	104	\N
111	3	0	right	right		81	\N	\N
\.


--
-- Data for Name: menutranslation; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY menutranslation (id, translation, menu, lang) FROM stdin;
172	Documentation	105	112
173	Documentación	105	113
\.


--
-- Data for Name: role; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY "role" (id, name, cancreate, canmodify, candelete, canpublish, canread) FROM stdin;
19	Edition	t	t	t	f	t
20	Publish	f	f	f	t	t
21	All	t	t	t	t	t
18	Read Only	f	f	f	f	t
\.


--
-- Data for Name: template; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY "template" (id, name, public, description, tlist, tview, tedit) FROM stdin;
39	displayCategoryHelp	t	Display Help Category	displayCategoryHelp.vm	defaultView.vm	defaultEdit.vm
40	showAllCategoryChildrenContents	t	Show All Category Children Contents	showAllCategoryChildrenContents.vm	defaultView.vm	defaultEdit.vm
41	showAllCategoryChildrenList	t	Show All Category Children List	showAllCategoryChildrenList.vm	defaultView.vm	defaultEdit.vm
42	showFirstCategoryChild	t	Show First Category Child	showFirstCategoryChild.vm	defaultView.vm	defaultEdit.vm
43	unavailable	t	Unavailable	unavailable.vm	defaultView.vm	defaultEdit.vm
44	displayCategoryFilesBlog	t	Display Files Category Blog	displayCategoryFilesBlog.vm	defaultView.vm	defaultEdit.vm
47	displayCategoryFilesTable	t	Display Files Category Table	displayCategoryFilesTable.vm	defaultView.vm	defaultEdit.vm
50	displayCategoryLinksTable	t	Display Links Category Table	displayCategoryLinksTable.vm	defaultView.vm	defaultEdit.vm
53	displayCategoryTextsBlog	t	Display Texts Category Blog	displayCategoryTextsBlog.vm	defaultView.vm	defaultEdit.vm
55	displayCategoryTitlesImages	t	Display Titles and Images Category	displayCategoryTitlesImages.vm	defaultView.vm	defaultEdit.vm
58	displayCategoryTitlesTable	t	Display Titles Category Table	displayCategoryTitlesTable.vm	defaultView.vm	defaultEdit.vm
60	displayCategoryTitlesTexts	t	Display Title and Body Category	displayCategoryTitlesTexts.vm	defaultView.vm	defaultEdit.vm
63	displayCategoryWithReferenceField	t	Display Category With Reference Field	displayCategoryWithReferenceField.vm	defaultView.vm	defaultEdit.vm
64	news	t	News Board	news.vm	defaultView.vm	defaultEdit.vm
36	defaultEdit	f	Default Edition	defaultList.vm	defaultView.vm	defaultEdit.vm
37	defaultList	f	Default List	defaultList.vm	defaultView.vm	defaultEdit.vm
38	defaultView	f	Default View	defaultView.vm	defaultView.vm	defaultEdit.vm
\.


--
-- Data for Name: type; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY "type" (id, name, description) FROM stdin;
9	date	Date
10	file	Attachment
11	int	Number
12	largetext	Large Text
13	link	Url
14	mail	Email
15	reference	Pointer to another category
16	smalltext	Small text
17	flash	Flash movie
\.


--
-- Data for Name: typetranslation; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY typetranslation (id, translation, "type", lang) FROM stdin;
\.


--
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY "user" (id, name, userpassword, sessiongroup) FROM stdin;
115	test	CY9rzUYh03PK3k6DJie09g==	\N
116	root	Y6nw6nu5gFB5a2SehUgYRQ==	\N
114	test1	WhBei51A4TKXgNYuoiZdig==	\N
\.


--
-- Data for Name: usersgroups; Type: TABLE DATA; Schema: public; Owner: carlos
--

COPY usersgroups (groupid, userid) FROM stdin;
22	115
24	116
23	114
\.


--
-- Name: acl_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY acl
    ADD CONSTRAINT acl_pkey PRIMARY KEY (acl_id);


--
-- Name: category_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY category
    ADD CONSTRAINT category_pkey PRIMARY KEY (categoryid);


--
-- Name: chat_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY chat
    ADD CONSTRAINT chat_pkey PRIMARY KEY (id);


--
-- Name: chatmessage_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY chatmessage
    ADD CONSTRAINT chatmessage_pkey PRIMARY KEY (id);


--
-- Name: configcombo_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY configcombo
    ADD CONSTRAINT configcombo_pkey PRIMARY KEY (configcomboid);


--
-- Name: configmodel_key_key; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY configmodel
    ADD CONSTRAINT configmodel_key_key UNIQUE ("key");


--
-- Name: configmodel_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY configmodel
    ADD CONSTRAINT configmodel_pkey PRIMARY KEY (id);


--
-- Name: container_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY container
    ADD CONSTRAINT container_pkey PRIMARY KEY (id);


--
-- Name: containeracl_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY containeracl
    ADD CONSTRAINT containeracl_pkey PRIMARY KEY (container_id, acl_id);


--
-- Name: content_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY content
    ADD CONSTRAINT content_pkey PRIMARY KEY (id);


--
-- Name: datamodel_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY datamodel
    ADD CONSTRAINT datamodel_pkey PRIMARY KEY (id);


--
-- Name: field_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY field
    ADD CONSTRAINT field_pkey PRIMARY KEY (id);


--
-- Name: fieldtemplate_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY fieldtemplate
    ADD CONSTRAINT fieldtemplate_pkey PRIMARY KEY (id);


--
-- Name: file_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY file
    ADD CONSTRAINT file_pkey PRIMARY KEY (id);


--
-- Name: forum_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY forum
    ADD CONSTRAINT forum_pkey PRIMARY KEY (id);


--
-- Name: forumfolder_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY forumfolder
    ADD CONSTRAINT forumfolder_pkey PRIMARY KEY (id);


--
-- Name: forummessage_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY forummessage
    ADD CONSTRAINT forummessage_pkey PRIMARY KEY (id);


--
-- Name: group_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY "group"
    ADD CONSTRAINT group_pkey PRIMARY KEY (id);


--
-- Name: language_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY "language"
    ADD CONSTRAINT language_pkey PRIMARY KEY (id);


--
-- Name: menu_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY menu
    ADD CONSTRAINT menu_pkey PRIMARY KEY (id);


--
-- Name: menutranslation_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY menutranslation
    ADD CONSTRAINT menutranslation_pkey PRIMARY KEY (id);


--
-- Name: role_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY "role"
    ADD CONSTRAINT role_pkey PRIMARY KEY (id);


--
-- Name: template_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY "template"
    ADD CONSTRAINT template_pkey PRIMARY KEY (id);


--
-- Name: type_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY "type"
    ADD CONSTRAINT type_pkey PRIMARY KEY (id);


--
-- Name: typetranslation_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY typetranslation
    ADD CONSTRAINT typetranslation_pkey PRIMARY KEY (id);


--
-- Name: user_name_key; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY "user"
    ADD CONSTRAINT user_name_key UNIQUE (name);


--
-- Name: user_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY "user"
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);


--
-- Name: usersgroups_pkey; Type: CONSTRAINT; Schema: public; Owner: carlos; Tablespace: 
--

ALTER TABLE ONLY usersgroups
    ADD CONSTRAINT usersgroups_pkey PRIMARY KEY (groupid, userid);


--
-- Name: fk11cab96f200778; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY chatmessage
    ADD CONSTRAINT fk11cab96f200778 FOREIGN KEY (chat) REFERENCES chat(id);


--
-- Name: fk11cab96f4910293; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY chatmessage
    ADD CONSTRAINT fk11cab96f4910293 FOREIGN KEY ("owner") REFERENCES "user"(id);


--
-- Name: fk1788a1e2e76db; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY acl
    ADD CONSTRAINT fk1788a1e2e76db FOREIGN KEY (group_id) REFERENCES "group"(id);


--
-- Name: fk1788a52119584; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY acl
    ADD CONSTRAINT fk1788a52119584 FOREIGN KEY (role_id) REFERENCES "role"(id);


--
-- Name: fk19b07bb224060e; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY menutranslation
    ADD CONSTRAINT fk19b07bb224060e FOREIGN KEY (lang) REFERENCES "language"(id);


--
-- Name: fk19b07bb224897f; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY menutranslation
    ADD CONSTRAINT fk19b07bb224897f FOREIGN KEY (menu) REFERENCES menu(id);


--
-- Name: fk2007788aecdf70; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY chat
    ADD CONSTRAINT fk2007788aecdf70 FOREIGN KEY (ogroup) REFERENCES "group"(id);


--
-- Name: fk24897f24060e; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY menu
    ADD CONSTRAINT fk24897f24060e FOREIGN KEY (lang) REFERENCES "language"(id);


--
-- Name: fk24897f8e0ff4ca; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY menu
    ADD CONSTRAINT fk24897f8e0ff4ca FOREIGN KEY (parent) REFERENCES menu(id);


--
-- Name: fk268d20cf40e9d01; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY forumfolder
    ADD CONSTRAINT fk268d20cf40e9d01 FOREIGN KEY (forum) REFERENCES forum(id);


--
-- Name: fk268d20cf8e0ff4ca; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY forumfolder
    ADD CONSTRAINT fk268d20cf8e0ff4ca FOREIGN KEY (parent) REFERENCES forumfolder(id);


--
-- Name: fk36ebcbdf9105a9; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY "user"
    ADD CONSTRAINT fk36ebcbdf9105a9 FOREIGN KEY (sessiongroup) REFERENCES "group"(id);


--
-- Name: fk40bb0da28035a; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY field
    ADD CONSTRAINT fk40bb0da28035a FOREIGN KEY ("type") REFERENCES "type"(id);


--
-- Name: fk40e9d01435e61fe; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY forum
    ADD CONSTRAINT fk40e9d01435e61fe FOREIGN KEY (forumgroup) REFERENCES "group"(id);


--
-- Name: fk473c1dc117d5fda; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY usersgroups
    ADD CONSTRAINT fk473c1dc117d5fda FOREIGN KEY (groupid) REFERENCES "group"(id);


--
-- Name: fk473c1dcce2b3226; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY usersgroups
    ADD CONSTRAINT fk473c1dcce2b3226 FOREIGN KEY (userid) REFERENCES "user"(id);


--
-- Name: fk4c707a361e2e76db; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY group_role
    ADD CONSTRAINT fk4c707a361e2e76db FOREIGN KEY (group_id) REFERENCES "group"(id);


--
-- Name: fk4c707a3652119584; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY group_role
    ADD CONSTRAINT fk4c707a3652119584 FOREIGN KEY (role_id) REFERENCES "role"(id);


--
-- Name: fk5ef36c6124060e; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY container
    ADD CONSTRAINT fk5ef36c6124060e FOREIGN KEY (lang) REFERENCES "language"(id);


--
-- Name: fk5ef36c614910293; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY container
    ADD CONSTRAINT fk5ef36c614910293 FOREIGN KEY ("owner") REFERENCES "user"(id);


--
-- Name: fk5ef36c61deb8d582; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY container
    ADD CONSTRAINT fk5ef36c61deb8d582 FOREIGN KEY (anonrole) REFERENCES "role"(id);


--
-- Name: fk60c9757f40bb0da; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY datamodel
    ADD CONSTRAINT fk60c9757f40bb0da FOREIGN KEY (field) REFERENCES field(id);


--
-- Name: fk60c9757f9befcc59; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY datamodel
    ADD CONSTRAINT fk60c9757f9befcc59 FOREIGN KEY (content) REFERENCES content(id);


--
-- Name: fk66faff40697a9b00; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY fields_template
    ADD CONSTRAINT fk66faff40697a9b00 FOREIGN KEY (template_id) REFERENCES "template"(id);


--
-- Name: fk66faff40c8a07680; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY fields_template
    ADD CONSTRAINT fk66faff40c8a07680 FOREIGN KEY (field_id) REFERENCES field(id);


--
-- Name: fk6dd211e8e0ff4ca; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY category
    ADD CONSTRAINT fk6dd211e8e0ff4ca FOREIGN KEY (parent) REFERENCES category(categoryid);


--
-- Name: fk6dd211eb515309a; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY category
    ADD CONSTRAINT fk6dd211eb515309a FOREIGN KEY ("template") REFERENCES "template"(id);


--
-- Name: fk6dd211ec4195ad9; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY category
    ADD CONSTRAINT fk6dd211ec4195ad9 FOREIGN KEY (categoryid) REFERENCES container(id);


--
-- Name: fk7fa750697fa74999; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY containeracl
    ADD CONSTRAINT fk7fa750697fa74999 FOREIGN KEY (container_id) REFERENCES container(id);


--
-- Name: fk7fa75069ab2bb4f0; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY containeracl
    ADD CONSTRAINT fk7fa75069ab2bb4f0 FOREIGN KEY (acl_id) REFERENCES acl(acl_id);


--
-- Name: fk95efdaf724060e; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY typetranslation
    ADD CONSTRAINT fk95efdaf724060e FOREIGN KEY (lang) REFERENCES "language"(id);


--
-- Name: fk95efdaf728035a; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY typetranslation
    ADD CONSTRAINT fk95efdaf728035a FOREIGN KEY ("type") REFERENCES "type"(id);


--
-- Name: fk9befcc5924060e; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY content
    ADD CONSTRAINT fk9befcc5924060e FOREIGN KEY (lang) REFERENCES "language"(id);


--
-- Name: fk9befcc596dd211e; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY content
    ADD CONSTRAINT fk9befcc596dd211e FOREIGN KEY (category) REFERENCES category(categoryid);


--
-- Name: fkcbb87c6268d20cf; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY forummessage
    ADD CONSTRAINT fkcbb87c6268d20cf FOREIGN KEY (forumfolder) REFERENCES forumfolder(id);


--
-- Name: fkcbb87c64910293; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY forummessage
    ADD CONSTRAINT fkcbb87c64910293 FOREIGN KEY ("owner") REFERENCES "user"(id);


--
-- Name: fkcbb87c68e0ff4ca; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY forummessage
    ADD CONSTRAINT fkcbb87c68e0ff4ca FOREIGN KEY (parent) REFERENCES forummessage(id);


--
-- Name: fkccb7677440bb0da; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY fieldtemplate
    ADD CONSTRAINT fkccb7677440bb0da FOREIGN KEY (field) REFERENCES field(id);


--
-- Name: fkccb76774b515309a; Type: FK CONSTRAINT; Schema: public; Owner: carlos
--

ALTER TABLE ONLY fieldtemplate
    ADD CONSTRAINT fkccb76774b515309a FOREIGN KEY ("template") REFERENCES "template"(id);


--
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

