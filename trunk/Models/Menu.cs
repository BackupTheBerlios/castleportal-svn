// Authors:
//    Alberto Morales <amd77@gulic.org>
//    Carlos Ble <carlosble@shidix.com>
//    Hector Rojas <hectorrojas@shidix.com>
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

using Castle.ActiveRecord.Queries;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using System.Collections;
using Iesi.Collections;
using NHibernate.Expression;
using NHibernate;

namespace CastlePortal
{
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class Menu : ActiveRecordBase
{
//    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    static ConfigManager config = ConfigManager.GetInstance();
    private int _Id;
    private string _Name;
    private string _Description;
    private string _Code;
    private int _Ordering;
    private string _Url; 
    private int _Show;
    private Language _Lang;
    private Menu _Parent;
    private int _CategoryId; 
    private IList _Children;

    public Menu() {}

    public Menu(string __Name, string __Description, string __Code, int __Ordering, 
                string __Url, Menu __Parent, Category __Category, int __Show)
    {
        _Name = __Name;
        _Description = __Description;
        _Code = __Code; 
        _Ordering = __Ordering;
        _Url = __Url;
        _Show= __Show;
        _Parent = __Parent;
        if (__Category != null)
            _CategoryId = __Category.Id;
        else
            _CategoryId = 0;
    }

    [PrimaryKey]
    public int Id
    {
    get { return _Id; }
        set { _Id = value; }
    }

    [BelongsTo]
    public Menu Parent
    {
        get { return _Parent; }
        set { _Parent = value; }
    }

    [BelongsTo]
    public Language Lang 
    {
        get { return _Lang; }
        set { _Lang = value; }
    }

#if CACHE
    [HasMany(typeof(Menu), OrderBy="ordering", Cache=CacheEnum.ReadWrite, Cascade=ManyRelationCascadeEnum.All)]
#else
    [HasMany(typeof(Menu), OrderBy="ordering", Cascade=ManyRelationCascadeEnum.All)]
#endif
    public IList Children
    {
        get { return _Children; }
        set { _Children = value; }
    }

    [Property]
    public int Ordering
    {
        get { return _Ordering; }
        set { _Ordering = value; }
    }

    [Property]
    public int Show
    {
        get { return _Show; }
        set { _Show = value; }
    }

    [Property]
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    [Property]
    public string Description
    {
        get { return _Description; }
        set { _Description = value; }
    }

    [Property]
    public string Code
    {
        get { return _Code; }
        set { _Code = value; }
    }

    [Property]
    public string Url
    {
        get { return _Url; }
        set { _Url = value; }
    }

    public Category GetCategory()
    {
        return (_CategoryId != 0) ? Category.Find(_CategoryId) : null;
    }

    [Property]
    public int CategoryId
    {
        get { return _CategoryId; }
        set { _CategoryId = value; }
    }

    public int ParentId()
    {
        if (_Parent == null)
            return 0;
        else
            return _Parent.Id;
    }

    public string Pathway(string baseurl)
    {
        if (_Parent == null)
            return System.String.Format("<a href=\"{0}0\">Top</a> / <a href=\"{0}{1}\">{2}</a> ",
                                        baseurl, _Id, _Name);
        else
            return System.String.Format("{0} / <a href=\"{1}{2}\">{3}</a> ",
                                        _Parent.Pathway(baseurl), baseurl, _Id, _Name);
    }

    public override string ToString()
    {
        return _Name;
    }

    public static Menu[] FindAll()
    {
        return (Menu[]) ActiveRecordBase.FindAll(
                   typeof(Menu),
                   new Order[] { Order.Asc("Ordering"), Order.Asc("Name") }
               );
    }

    public static Menu Find(int id)
    {
        return (Menu) ActiveRecordBase.FindByPrimaryKey( typeof(Menu), id );
    }

    public static Menu FindByName(string menuName)
    {
        Menu[] menus = (Menu[]) ActiveRecordBase.FindAll( typeof(Menu),
                               Expression.Eq("Name", menuName));
        if ((menus != null) && (menus.Length > 0))
            return menus[0];
        else
            return null;
    }

    public static Menu FindByShow()
    {
        return (Menu) FindOne( typeof(Menu), Expression.Eq("Show", 1));
    }

    public new static Menu[] FindByParent(int parentid)
    {
        if (parentid == -1)
            return (Menu[]) ActiveRecordBase.FindAll( typeof(Menu),
                    new Order[] { Order.Asc("Ordering"), Order.Asc("Name") },
                    Expression.IsNull("Parent"));
        else
            return (Menu[]) ActiveRecordBase.FindAll( typeof(Menu),
                    new Order[] { Order.Asc("Ordering"), Order.Asc("Name") },
                    Expression.Eq("Parent.Id", parentid));
    }

    public new static Menu FindByCategory(Category c)
    {
        Menu m = null;
        do 
        {
            if ( c == null ) break;
            m = FindByCategoryId(c.Id);
            c = c.Parent;
        } while (m == null);
        return m;
    }

    public new static Menu FindByCategoryId(int categoryid)
    {
        Menu[] menus = (Menu[]) ActiveRecordBase.FindAll( typeof(Menu),
                                                Expression.Eq("CategoryId", categoryid));
        if ((menus != null) && (menus.Length > 0))
        {
            return menus[0];
        }
        else
            return null;        
    }

    public new static Menu FindByUrl(string url)
    {
        Menu m = (Menu) ActiveRecordBase.FindOne( typeof(Menu),
                 Expression.Eq("Url", url));
        return m;
    }

    public ArrayList GetParents(int level)
    {
        ArrayList parents = new ArrayList();
        Menu menu = this;
        parents.Insert(0,menu);
        while (menu.Parent != null) 
        {
            menu = menu.Parent;
            parents.Insert(0,menu);
        }
        for (int i = 0; i< level; i++)
            parents.RemoveAt(0);
        return parents;
    }

    public ArrayList Tree(int level)
    {
        ArrayList parents = GetParents(level);
        if (parents.Count > 0) 
        {
            Menu m = (Menu)parents[0];
            return m.SubTree(ref parents);
        } 
        else 
        {
            return null;
        }
    }

    private ArrayList SubTree(ref ArrayList parents)
    {
        ArrayList tree = new ArrayList();
        foreach (Menu menu in Children) 
        {
            tree.Add(menu);
            bool contained = false;
            foreach (Menu m in parents)
            {
                if (m.Id == menu.Id)
                {
                    contained = true;
                    break;
                }
            }
            if (contained) 
            {
                tree.Add(menu.SubTree(ref parents));
            }
        }
        return tree;
    }

    public string ToUrl(string s)
    {
        if (_Url != "")
            return s+_Url;
        else
            return s + Constants.VIEW_PORTAL_CATEGORY + "." + config.GetValue(Constants.EXTENSION) + 
            "?id=" + _CategoryId;
            //return s+"/portal/viewcategory.html?id=" + _CategoryId;
    }

    public static Hashtable GetHashByCategoryHash(Hashtable categoryHash)
    {
        if (categoryHash == null)
        {
            return null;
        }
        Hashtable menuHash = new Hashtable();
        foreach (Menu m in Menu.FindAll())
        {
            if (m.CategoryId > 0)
            {
                menuHash[m.Id] = categoryHash[m.CategoryId];
            } 
            else
            {
                menuHash[m.Id] = Commons.GetPermissionsBaseHash(true);
            }
        }
        return menuHash;
    }

    public string FindTranslation(string lang)
    {
        if ((lang == null) || (lang.Length == 0))
            return Description;
        ISessionFactoryHolder sessionHolder = ActiveRecordMediator.GetSessionFactoryHolder();
        ISession session = sessionHolder.CreateSession(typeof (MenuTranslation));
        string query = "select menutranslation.translation from  menu, language, menutranslation ";
        query += "where menutranslation.menu = " + Id;
        query += " and menutranslation.lang = language.id and language.englishname = '" + lang +"'";
        IQuery sqlQuery = session.CreateSQLQuery(query, "menutranslation", typeof(MenuTranslation));
        sqlQuery.SetMaxResults(1);
        IList translations = sqlQuery.List();
        if ((translations != null) && (translations.Count > 0))
            return ((MenuTranslation)translations[0]).Translation;
        else
            return Description; // Constants.NO_TRANSLATION_FOUND;
    }

    public int GetLevel()
    {
        Menu menu = this;

        int level = 0;
        for (Menu m = menu; m.Parent != null; m = m.Parent)
            level++;

        return level;
    }

    public Menu GetMenuByLevel(int level)
    {
        Menu parent = this;
        int i = this.GetLevel();

        for (int j = i; j >= level; j--)
            parent = parent.Parent;

        return parent;
    }
}
}
