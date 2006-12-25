// Authors:
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
public class MenuTranslation : ActiveRecordBase
{
//    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
//    static ConfigManager config = ConfigManager.GetInstance();
    
    private int _Id;
    private Language _lang;
    private Menu _menu;
    private string _translation;

    public MenuTranslation() {}

    public MenuTranslation(Language lang, Menu menu, string translation)
    {
        _lang = lang;
        _menu = menu;
        _translation = translation;
    }

    [PrimaryKey]
    public int Id
    {
    get { return _Id; }
        set { _Id = value; }
    }

    [BelongsTo]
    public Menu Menu
    {
        get { return _menu; }
        set { _menu = value; }
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

    public static MenuTranslation[] FindAll()
    {
        return (MenuTranslation[]) ActiveRecordBase.FindAll(typeof(MenuTranslation));
    }

    public static MenuTranslation Find(int id)
    {
        return (MenuTranslation) ActiveRecordBase.FindByPrimaryKey( typeof(MenuTranslation), id );
    }

    public static MenuTranslation FindByMenuAndLang(Menu menu, Language lang)
    {
        MenuTranslation[] translations = (MenuTranslation[]) ActiveRecordBase.FindAll( typeof(MenuTranslation),
                               Expression.Eq("Menu", menu), Expression.Eq("Lang", lang));
        if ((translations != null) && (translations.Length > 0))
            return translations[0];
        else
            return null;
    }

    public static MenuTranslation FindByMenuAndLang(Menu menu, string lang)
    {
            return null;
    }

    public static MenuTranslation[] FindByMenu(Menu menu)
    {
        SimpleQuery q = new SimpleQuery(typeof(MenuTranslation), @"
            from MenuTranslation T
            where T.Menu = ?", menu);

        return (MenuTranslation[]) ExecuteQuery(q);

    }
}
}
