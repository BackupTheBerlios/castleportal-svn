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
using System;
using System.IO;
using NHibernate.Expression;
using NHibernate;

namespace CastlePortal
{

#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class Category : Container
{
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    static ConfigManager config = ConfigManager.GetInstance();

    private int _CategoryId;
    private System.DateTime _CreationDate;
    private System.DateTime _ModificationDate;
    private string _Description;
    private string _Code;
    private string _Information;

    private Category _Parent;
    private Template _Template;

    private IList _Children;
    private IList _contentList;
    private IList _contentListReverse;

    public Category () { }

    [JoinedKey]
    public int CategoryId
    {
        get { return _CategoryId; }
        set { _CategoryId = value; }
    }

    [BelongsTo]
    public Category Parent
    {
        get { return _Parent; }
        set { _Parent = value; }
    }

    [BelongsTo]
    public Template Template
    {
        get { return _Template; }
        set { _Template = value; }
    }

#if CACHE
    [HasMany(typeof(Category), Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasMany(typeof(Category), Lazy=true)]
#endif
    public IList Children
    {
        get { return _Children; }
        set { _Children = value; }
    }

#if CACHE
    [HasMany(typeof(Content), OrderBy="creationdate asc", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasMany(typeof(Content), OrderBy="creationdate asc", Lazy=true)]
#endif
    public IList ContentList
    {
        get { return _contentList; }
        set { _contentList = value; }
    }

    public IList ContentListSortedByField(string key)
    {
        IComparer c = new FieldComparer(key);
        ArrayList a = new ArrayList(ContentList);
        a.Sort(c);
        return a;
    }

    public IList ContentListSortedReverse(string key)
    {
        ArrayList a = (ArrayList) ContentListSortedByField(key);
        a.Reverse();
        return a;
    }

    /// <summary>
    /// Returns a Hashtable containing an ArrayList for every key.
    /// Every ArrayList contains Contents.
    /// So you have a Hashtable of Contents grouped by field contents
    /// </summary>
    public Hashtable ContentHashGroupedByFieldName(string fieldName, string pointedFieldName)
    {
        try
        {
            if ((pointedFieldName == null) || (pointedFieldName.Length == 0))
                pointedFieldName = Constants.TITLE;
            Hashtable hash = new Hashtable();
            foreach (Content content in ContentList)
            {
                DataModel datam = content.GetDataModelByFieldName(fieldName);
                if (datam != null)
                {
                    string fieldValue = datam.Value;
                    // field could be a reference to another Content in another category
                    if (datam.IsIntegerValue())
                    {
                        // fieldName must match Category name
                        Content pointedContent = datam.GetPointedContent(fieldName);
                        if (pointedContent != null)
                            fieldValue = pointedContent.GetValueByFieldName(pointedFieldName);
                    }
                    ArrayList ContentsByFieldValue;
                    if (!hash.ContainsKey(fieldValue))
                    {
                        ContentsByFieldValue = new ArrayList();
                        hash[fieldValue] = ContentsByFieldValue;
                    }
                    else
                    {
                        ContentsByFieldValue = (ArrayList) hash[fieldValue];
                    }
                    ContentsByFieldValue.Add(content);
                }
            }
            return hash;
        }
        catch (Exception ex)
        {
            logger.Error("Error ContentHashGroupedByFieldName:" + ex.Message +","+ ex.StackTrace);
            return null;
        }
    }

#if CACHE
    [HasMany(typeof(Content), OrderBy="creationdate desc", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasMany(typeof(Content), OrderBy="creationdate desc", Lazy=true)]
#endif
    public IList ContentListSortedByReverseDate
    {
        get { return _contentListReverse; }
        set { _contentListReverse = value; }
    }


    [Property]
    public System.DateTime CreationDate
    {
        get { return _CreationDate; }
        set { _CreationDate = value; }
    }

    [Property]
    public System.DateTime ModificationDate
    {
        get { return _ModificationDate; }
        set { _ModificationDate = value; }
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

    [Property(Length=10000)]
    public string Information
    {
        get { return _Information; }
        set { _Information = value; }
    }

    public string FullName()
    {
        if (_Parent == null)
            return Path.Combine(Constants.TOP, Name);
        else
            return Path.Combine(_Parent.FullName(), Name);
    }

    public override string ToString()
    {
        return Name;
    }

    public new static Category[] FindAll()
    {
        return (Category[]) ActiveRecordBase.FindAll(
                   typeof(Category),
                   new Order[] { Order.Asc("Name") }
               );
    }

    public new static Category Find(int id)
    {
        return (Category) ActiveRecordBase.FindByPrimaryKey( typeof(Category), id );
    }

    public new static Category FindById(int id)
    {
        return (Category) ActiveRecordBase.FindOne( typeof(Category),  Expression.Eq("Id", id));
    }

    public new static Category[] FindByParent(int parentid)
    {
        if (parentid > 0)
            return (Category[]) ActiveRecordBase.FindAll( typeof(Category),
                    new Order[] { Order.Asc("Name") },
                    Expression.Eq("Parent.Id", parentid));
        else
            return (Category[]) ActiveRecordBase.FindAll( typeof(Category),
                    new Order[] { Order.Asc("Name") },
                    Expression.IsNull("Parent"));
    }

    public static Category FindByName(string categoryName)
    {
        return (Category) FindOne( typeof(Category),
                                   Expression.Eq("Name", categoryName)
                                 );
    }

    public new Category[] GetChildren()
    {
        return (Category[]) ActiveRecordBase.FindAll(
                   typeof(Category),
                   new Order[] { Order.Asc("Id") },
                   Expression.Eq("Parent.Id", Id)
               );
    }

    public int NumberOfChildrens()
    {
        ScalarQuery q = new ScalarQuery(typeof(Category),
                                        "select count(*) from Category where Parent = ?", Id);
        return (int) ExecuteQuery(q);
    }


    public string ToUrl()
    {
        return ToUrl("");
    }

    public string ToUrl(string s)
    {
        try
        {
            return s + Constants.VIEW_PORTAL_CATEGORY + "." + config.GetValue(Constants.EXTENSION) + 
                "?id=" + Id;
        }
        catch (NullReferenceException)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public ArrayList Ancestors(int level)
    {
        ArrayList ancestors = new ArrayList();
        Category category = this;

        while (category != null) {
            ancestors.Insert(0, category);
            category = category.Parent;
        }
        for (int i = 0; i < level; i++)
            ancestors.RemoveAt(0);
        return ancestors;
    }

    public static Category[] SearchByWord(string s)
    {
        if (s == null)
            return new Category[0];

        return (Category[]) ActiveRecordBase.FindAll(
                   typeof(Category),
                   Expression.Or(
                       Expression.Or(
                           Expression.Like("Description", "%"+s+"%" ),
                           Expression.Like("Name", "%"+s+"%" )
                       ),
                       Expression.Like("Information", "%"+s+"%" )
                   )
               );
    }

    public static ArrayList SearchByWordAndUser(string s, User u)
    {
        Category[] results = SearchByWord(s);

        ArrayList visibleResults = new ArrayList();
        foreach (Category category in results)
        if (category.CanRead(u))
            visibleResults.Add(category);
        return visibleResults;
    }


    public string FindTranslation(string lang)
    {
        if ((lang == null) || (lang.Length == 0))
            return Description;
        ISessionFactoryHolder sessionHolder = ActiveRecordMediator.GetSessionFactoryHolder();
        ISession session = sessionHolder.CreateSession(typeof (CategoryTranslation));
        string query = "select categorytranslation.translation from  category, language, categorytranslation ";
        query += "where categorytranslation.category = " + Id;
        query += " and categorytranslation.lang = language.id and language.englishname = '" + lang +"'";
        IQuery sqlQuery = session.CreateSQLQuery(query, "categorytranslation", typeof(CategoryTranslation));
        sqlQuery.SetMaxResults(1);
        IList translations = sqlQuery.List();
        if ((translations != null) && (translations.Count > 0))
            return ((CategoryTranslation)translations[0]).Translation;
        else
            return Description; // Constants.NO_TRANSLATION_FOUND;
    }
    
    /*public static Category FindWithContentsByLang(int id, string lang)
    {
        if ((lang == null) || (lang.Length == 0))
            return null;
        ISessionFactoryHolder sessionHolder = ActiveRecordMediator.GetSessionFactoryHolder();
        ISession session = sessionHolder.CreateSession(typeof (Category));
        string query = "select category.categoryid from  category, content, language, template ";
        query += "where category.categoryid = " + id;
        query += " and category.template = template.id ";
        query += " and content.lang = language.id and language.englishname = '" + lang +"'";
        IQuery sqlQuery = session.CreateSQLQuery(query, "category", typeof(Category));
        sqlQuery.SetMaxResults(1);
        IList results = sqlQuery.List();
        if ((results != null) && (results.Count > 0))
            return (Category)results[0];
        else
            return null; // Constants.NO_TRANSLATION_FOUND;
    }*/
    
    public new static IList FindWithContentsByLang(int id, string lang, User user)
    {
        Category category = (Category) ActiveRecordBase.FindByPrimaryKey( typeof(Category), id );

        ArrayList contents = new ArrayList();
        foreach(Content content in category.ContentList)
        {
            if (content.Lang.Name == lang)
               if (user != null)
                contents.Add(content);
               else
                  if (content.Published)
                     contents.Add(content);
        }

        return contents;
    }

    public static Category FindByCode(string code)
    {
        return (Category)FindOne(typeof(Category), Expression.Eq("Code", code));
    }
}
}
