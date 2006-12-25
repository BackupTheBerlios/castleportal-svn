// Authors:
//    Alberto Morales <amd77@gulic.org>
//
// Copyright 2006 Shidix Technologies - http://www.shidix.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
/* GUIA RAPIDA DE TAGS
 * 
 * Acl: nombre group role
 * Config: key val
 * ConfigCombo: key val nombre
 * Category: name visiblename category template acl role ayuda
 * Field: nombrecorto nombrelargo type
 * Group: nombre role
 * Include: nombrefichero cadena1 sustitucion1 cadena2 sustitucion2 
 * Menu: nombre description parent order category url
 * Role: nombrecorto nombrelargo create modify delete publish read
 * Content: category keys vals 
 * Template: nombrecorto nombrelargo fichero listaedit listalist
 * Type: nombrecorto nombrelargo
 * User: username password group
 *
 */
namespace CastlePortal
{
using System.IO;
using System.Configuration;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Iesi.Collections;
using System.Collections;
using System.Xml;

public class SchemaGenerator
{
    static bool debug = false;
    //static bool debug = true;

    //TODO: poner esto como una hashtable de hashtables
    //y pasarlo como un único parametro... mas que nada para
    //que no se queden en memoria
    private static Hashtable acls = new Hashtable();
    private static Hashtable categories = new Hashtable();
    private static Hashtable fields = new Hashtable();
    private static Hashtable groups = new Hashtable();
    private static Hashtable menus = new Hashtable();
    private static Hashtable roles = new Hashtable();
    private static Hashtable templates = new Hashtable();
    private static Hashtable types = new Hashtable();
    private static Hashtable users = new Hashtable();
    private static Hashtable languages = new Hashtable();

    private static string NodeGetString(XmlNode node, string name)
    {
        XmlAttribute a = node.Attributes[name];
        if (a != null)
            return a.Value;
        else
            return "";
    }

    private static bool NodeGetBool(XmlNode node, string name)
    {
        string s = NodeGetString(node, name);
        return (s == "true") || (s == "1") | (s == "yes");
    }

    private static object NodeGetObject(XmlNode node, string key, Hashtable input)
    {
        string fieldname = NodeGetString(node, key);
        if (fieldname == "")
        {
            if (debug)
                System.Console.WriteLine("{0}: <{1} {2}=\"\"> nulo [1].", node.BaseURI, node.Name, key);
            return null;
        }
        else if (!input.ContainsKey(fieldname))
        {
            System.Console.WriteLine("{0}: <{1} {2}=\"{3}\"> no existe [1].", node.BaseURI, node.Name, key, fieldname);
            return null;
        }
        else
        {
            //System.Console.WriteLine(input[fieldname].GetType());
            //System.Console.WriteLine(input[fieldname]);
            return input[fieldname];
        }
    }

    private static object GetObject(string fieldname, Hashtable input, XmlNode node, string debugmsg)
    {
        if (fieldname == "")
        {
            if (debug)
                System.Console.WriteLine("{0}: <{1} {2}=\"\"> nulo [2].", node.BaseURI, node.Name, debugmsg);
            return null;
        }
        else if (!input.ContainsKey(fieldname))
        {
            System.Console.WriteLine("{0}: <{1} {2}=\"{3}\"> no existe [2].", node.BaseURI, node.Name, debugmsg, fieldname);
            return null;
        }
        else
        {
            return input[fieldname];
        }
    }

    private static Set NodeGetObjectSet(XmlNode node, string element, Hashtable input)
    {
        HashedSet tmp = new HashedSet();
        foreach (XmlNode n in node.ChildNodes)
        if (n.Name == element)
            tmp.Add(NodeGetObject(n, "nombre", input));
        return tmp;
    }

    private static IList NodeGetObjectList(XmlNode node, string element, Hashtable input)
    {
        IList tmp = new ArrayList();
        foreach (XmlNode n in node.ChildNodes)
        if (n.Name == element)
            tmp.Add(NodeGetObject(n, "nombre", input));
        return tmp;
    }


    private static string ReadType(XmlNode node)
    {
        if (node.Name == "Type")
        {
            string name = NodeGetString(node, "nombrecorto");
            string description = NodeGetString(node, "nombrelargo");

            System.Console.WriteLine ("Tipo Base: "+ name);
            Type t = new Type(name, description);
            t.Save();
            types[name] = t;
        }
        return null;
    }

    private static string ReadLanguage(XmlNode node)
    {
        if (node.Name == "Language")
        {
            string name = NodeGetString(node, Constants.LANGUAGE_NAME);
            string englishName = NodeGetString(node, Constants.LANGUAGE_ENGLISHNAME);
            string description = NodeGetString(node, Constants.DESCRIPTION);

            System.Console.WriteLine ("Language: "+ name);
            Language lang = new Language(name, englishName, description);
            lang.Save();
            languages[name] = lang;
        }
        return null;
    }

    private static string ReadHelp(XmlNode node)
    {
        if (node.Name == "CategoryHelp")
        {
            string catname = NodeGetString(node, "category");
            System.Console.WriteLine ("Ayuda: "+ catname);
            Category c = Category.FindByName(catname);
            c.Information = node.InnerXml;
            c.Update();
        }
        return null;
    }

    private static string ReadConfig(XmlNode node)
    {
        if (node.Name == "Config")
        {
            string key = NodeGetString(node, "key");
            string val = NodeGetString(node, "val");
            string desc = NodeGetString(node, "description");

            System.Console.WriteLine ("Configuracion: "+ key);
            ConfigModel c = ConfigModel.FindByKey(key);
            if (c == null)
            {
                ConfigModel cm = new ConfigModel(key, val, desc);
                cm.Save();
            }
        }
        return null;
    }

    private static string ReadConfigCombo(XmlNode node)
    {
        if (node.Name == "ConfigCombo")
        {
            string key = NodeGetString(node, "key");
            string val = NodeGetString(node, "val");
            string nombre = NodeGetString(node, "nombre");
            System.Console.WriteLine ("Configuracion Combo: "+ key);

            ConfigCombo c = new ConfigCombo (key, val, nombre);
            c.Save();
        }
        return null;
    }

    private static string ReadRole(XmlNode node)
    {
        if (node.Name == "Role")
        {
            string nombre = NodeGetString(node, "nombrecorto");
            string name = NodeGetString(node, "nombrelargo");
            bool create = NodeGetBool(node, "create");
            bool modify = NodeGetBool(node, "modify");
            bool delete = NodeGetBool(node, "delete");
            bool publish = NodeGetBool(node, "publish");
            bool read = NodeGetBool(node, "read");

            System.Console.WriteLine ("Roles: "+ nombre);
            Role r = new Role (name, create, modify, delete, publish, read);
            r.Save();

            roles[nombre] = r;
        }
        return null;
    }

    private static string ReadGroup(XmlNode node)
    {
        if (node.Name == "Group")
        {
            string nombre = NodeGetString(node, "nombre");

            Group g = new Group (nombre);
            g.Save();

            g.Roles = NodeGetObjectSet(node, "Role", roles);
            g.Save();

            /*				Group x = Group.Find(g.Id);
            				foreach (Role r in g.Roles)
            			System.Console.WriteLine ("Grupo ROL: "+ r.Name);

            			System.Console.WriteLine ("Grupo: "+ nombre);
            				g.Save();*/
            groups[nombre] = g;
        }
        return null;
    }

    private static string ReadInclude(XmlNode node, string rootpath)
    {
        if (node.Name == "Include")
        {
            string nombre = NodeGetString(node, "fichero");
            StreamReader sr = System.IO.File.OpenText (rootpath+"Generator/"+nombre) ;
            string fileContent = sr.ReadToEnd();

            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == "Replace")
                {
                    string from = NodeGetString(n, "from");
                    string to = NodeGetString(n, "to");
                    fileContent = fileContent.Replace(from, to);
                }
            }
            CreateAllString(fileContent, rootpath);
        }
        return null;
    }

    private static string ReadUser(XmlNode node)
    {
        if (node.Name == "User")
        {
            string username = NodeGetString(node, "username");
            string passwd = NodeGetString(node, "password");

            // Creamos usuario
            User u = new User (username, passwd);
            u.Save();

            // u.Save no tiene un u valido para hacer un Groups.Add
            u = User.Find(u.Id);

            u.Groups = NodeGetObjectList(node, "Group", groups);

            System.Console.WriteLine ("Usuario: "+username);
            u.Save();
            users[username] = u;
        }
        return null;
    }

    private static string ReadField(XmlNode node)
    {
        if (node.Name == "Field")
        {
            string nombre = NodeGetString(node, "nombrecorto");
            string description = NodeGetString(node, "nombrelargo");
            Type type = (Type) NodeGetObject(node, "type", types);

            System.Console.WriteLine ("Field: "+ nombre);
            Field f = new Field(nombre, description, type);
            f.Save();
            fields[nombre] = f;
        }
        return null;
    }

    private static string ReadTemplate(XmlNode node, string rootpath)
    {
        if (node.Name == "Template")
        {
            string name = NodeGetString(node, "nombre");
            string description = NodeGetString(node, "nombrelargo");
            string path = NodeGetString(node, "fichero");
            string isPublic = NodeGetString(node, "public");

            string filelist = Constants.DEFAULT_LIST_TEMPLATE;
            string fileview = Constants.DEFAULT_VIEW_TEMPLATE;
            string fileedit = Constants.DEFAULT_EDIT_TEMPLATE;
            string prepath = rootpath + Constants.TEMPLATES_FOLDER + path;

            if (System.IO.File.Exists(prepath + ".vm"))
                filelist = path + ".vm";
            if (System.IO.File.Exists(prepath + "_list.vm"))
                filelist = path + "_list.vm";
            if (System.IO.File.Exists(prepath + "_edit.vm"))
                fileedit = path + "_edit.vm";
            if (System.IO.File.Exists(prepath + "_view.vm"))
                fileview = path + "_view.vm";

            Template t = new Template(name, description, filelist, fileview, fileedit);
            if (isPublic == "true")
            {
                t.Public = true;
            }
            else
            {
                t.Public = false;
            }
            /* FIXME: Estropeado porque antes era field y ahora es fieldtemplate
            t.FieldEdit = listaedit;
            */
            t.Save();

            int i = 1; // orden para la edicion
            int j = 1; // orden para el listado
            foreach (XmlNode field in node.ChildNodes)
            if (field.Name == "Field")
            {
                Field f = (Field) NodeGetObject(field, "nombre", fields);
                string show = NodeGetString(field, "show");

                FieldTemplate ft;
                if (show == "0")
                    ft = new FieldTemplate(f, t, -1, i++);
                else
                    ft = new FieldTemplate(f, t, j++, i++);
                ft.Save();
            }

            System.Console.WriteLine ("Template: "+ name);
            templates[name] = t;
        }
        return null;
    }

    private static string ReadAcl(XmlNode node)
    {
        if (node.Name == "Acl")
        {
            string nombre = NodeGetString(node, "nombre");
            Group g = (Group) NodeGetObject(node, "group", groups);
            Role r = (Role) NodeGetObject(node, "role", roles);

            Acl a = new Acl(g, r);
            System.Console.WriteLine ("Acl: "+ nombre);
            a.Save();
            acls[nombre] = a;
        }
        return null;
    }

    private static string ReadContent(XmlNode node, Category c)
    {
        string  keys = NodeGetString(node, "keys");
        string  vals = NodeGetString(node, "vals");

        string[] colname = keys.Split(',');
        string[] namedescr = vals.Split(',');

        int l = colname.Length;

        Field [] f = new Field[l];
        for (int i=0; i<l; i++)
            f[i] = Field.GetByName(colname[i]);

        Content r = new Content(c);
        r.Save();

        for (int i=0; i<l; i++)
        {
            DataModel d = new DataModel(r, f[i], namedescr[i]);
            d.Save();
        }

        return null;

    }

    private static string ReadCategory(XmlNode node)
    {
        if (node.Name == "Category")
        {
            string name = NodeGetString(node, "name");
            string visiblename = NodeGetString(node, "visiblename");
            Category parent = (Category) NodeGetObject(node, "category", categories);
            Template t = (Template) NodeGetObject(node, "template", templates);
            Role r = (Role) NodeGetObject(node, "role", roles);
            //	string ayuda = NodeGetString(node, "ayuda");

            Category c = new Category ();
            if (name != "" && visiblename == "")
                visiblename = name;
            if (name == "" && visiblename != "")
                name = visiblename;
            c.Name = name;
            c.Description = visiblename;
            c.Parent = parent;
            c.Template = t;
            c.AnonRole = r;
            //c.Information = ayuda;
            c.Save();

            System.Console.WriteLine ("Categoria: "+ name);
            c.AclSet = NodeGetObjectSet(node, "Acl", acls);

            c.Save();
            categories[name] = c;

            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == "Category")
                    ReadCategory(n);
                else
                    if (n.Name == "Content")
                        ReadContent(n, c);
            }
        }
        return null;
    }

    private static string ReadMenu(XmlNode node)
    {
        try
        {
            if (node.Name == "Menu")
            {
                string name = NodeGetString(node, "nombre");
                string description = NodeGetString(node, "description");
                if (name != "" && description == "")
                    description = name;
                if (name == "" && description != "")
                    name = description;

                Menu parent = (Menu)NodeGetObject(node, "parent", menus);
                string orderString = NodeGetString(node, "order");
                int ordering = int.Parse(orderString);

                string url = NodeGetString(node, "url");

                string categorystring = NodeGetString(node, "category");
                if (categorystring == "")
                    categorystring = name;

                Category category = null;
                if (url == "")
                    category = (Category) GetObject(categorystring, categories, node, "category");

                System.Console.WriteLine ("Menu: "+ name);
                int show = 0;
                Menu m = new Menu(name, description, ordering, url, parent, category, show);
                m.Save();
                menus[name] = m;

                foreach (XmlNode n in node.ChildNodes)
                if (n.Name == "Menu")
                    ReadMenu(n);
            }
            return null;
        }
        catch(System.FormatException)
        {
            return "No se pudo leeer columna ordering";
        }
    }


    /************************ CREATES *************************/

    private static void CreateAllString(string xmldata, string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(new StringReader(xmldata));
        CreateAllBase(doc, path);
    }

    private static void CreateAll(string fileName, string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path+"/"+fileName);
        CreateAllBase(doc, path);
    }

    private static void CreateAllBase(XmlDocument doc, string path)
    {
        string[] elements = { "Configs", "ConfigCombos", "Roles", "Groups",
                              "Includes", "Users", "Fields", "Templates", "Acls",
                              "Categories", "Menus", "Help", "BaseTypes", "Languages"
                            };

        foreach (string element in elements)
        {
            XmlNodeList xmlNodeList = doc.GetElementsByTagName(element);
            foreach (XmlNode nodebase in xmlNodeList)
            foreach (XmlNode node in nodebase.ChildNodes)
            switch (element)
            {
            case "Configs":
                ReadConfig(node);
                break;
            case "ConfigCombos":
                ReadConfigCombo(node);
                break;
            case "Roles":
                ReadRole(node);
                break;
            case "Groups":
                ReadGroup(node);
                break;
            case "Includes":
                ReadInclude(node, path);
                break;
            case "Users":
                ReadUser(node);
                break;
            case "Fields":
                ReadField(node);
                break;
            case "Templates":
                ReadTemplate(node, path);
                break;
            case "Acls":
                ReadAcl(node);
                break;
            case "Categories":
                ReadCategory(node);
                break;
            case "Menus":
                ReadMenu(node);
                break;
            case "Help":
                ReadHelp(node);
                break;
            case "BaseTypes":
                ReadType(node);
                break;
            case "Languages":
                ReadLanguage(node);
                break;
            }
        }
    }

    public static bool Crea(string path)
    {
        string file = "install.xml";
        if (!System.IO.File.Exists(path+file))
        {
            System.Console.WriteLine("Error!: install.xml not found");
            return false;
        }

        Castle.ActiveRecord.ActiveRecordStarter.CreateSchema();

        CreateAll(file, path);

        //System.Console.Write(", borrando");
        //System.IO.File.Delete(path+file);

        return true;
    }
}
}
