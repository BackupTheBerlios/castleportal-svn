// Authors:
//    Carlos Ble <carlosble@shidix.com>
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
using System.Collections;
using Iesi.Collections;
using NHibernate.Expression;

namespace CastlePortal
{
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class CategoryTranslation : ActiveRecordBase
{
//    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
//    static ConfigManager config = ConfigManager.GetInstance();
    
    private int _Id;
    private Language _lang;
    private Category _category;
    private string _translation;

    public CategoryTranslation() {}

    public CategoryTranslation(Language lang, Category category, string translation)
    {
        _lang = lang;
        _category = category;
        _translation = translation;
    }

    [PrimaryKey]
    public int Id
    {
    get { return _Id; }
        set { _Id = value; }
    }

    [BelongsTo]
    public Category Category
    {
        get { return _category; }
        set { _category = value; }
    }

    [BelongsTo]
    public Language Lang 
    {
        get { return _lang; }
        set { _lang = value; }
    }

    [Property]
    public string Translation
    {
        get { return _translation; }
        set { _translation = value; }
    }

    public override string ToString()
    {
        return _translation;
    }

    public static CategoryTranslation[] FindAll()
    {
        return (CategoryTranslation[]) ActiveRecordBase.FindAll(typeof(CategoryTranslation));
    }

    public static CategoryTranslation Find(int id)
    {
        return (CategoryTranslation) ActiveRecordBase.FindByPrimaryKey( typeof(CategoryTranslation), id );
    }

    public static CategoryTranslation FindByCategoryAndLang(Category category, Language lang)
    {
        CategoryTranslation[] translations = (CategoryTranslation[]) ActiveRecordBase.FindAll( typeof(CategoryTranslation),
                               Expression.Eq("Category", category), Expression.Eq("Lang", lang));
        if ((translations != null) && (translations.Length > 0))
            return translations[0];
        else
            return null;
    }

    public static CategoryTranslation FindByCategoryAndLang(Category category, string lang)
    {
        return null;
    }

    public static CategoryTranslation[] FindByCategory(Category category)
    {
        CategoryTranslation[] translations = (CategoryTranslation[]) ActiveRecordBase.FindAll( typeof(CategoryTranslation),
                               Expression.Eq("Category", category));
        if ((translations != null) && (translations.Length > 0))
            return translations;
        else
            return null;
        
				/*SimpleQuery q = new SimpleQuery(typeof(CategoryTranslation), @"
            from CategoryTranslation T
            where T.Category = ?", category);

        return (CategoryTranslation[]) ExecuteQuery(q);
				*/
    }
}
}
