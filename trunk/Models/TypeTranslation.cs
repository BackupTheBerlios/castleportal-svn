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
public class TypeTranslation : ActiveRecordBase
{
//    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
//    static ConfigManager config = ConfigManager.GetInstance();
    
    private int _Id;
    private Language _lang;
    private Type _type;
    private string _translation;

    public TypeTranslation() {}

    public TypeTranslation(Language lang, Type type, string translation)
    {
        _lang = lang;
        _type = type;
        _translation = translation;
    }

    [PrimaryKey]
    public int Id
    {
    get { return _Id; }
        set { _Id = value; }
    }

    [BelongsTo]
    public Type Type
    {
        get { return _type; }
        set { _type = value; }
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

    public static TypeTranslation[] FindAll()
    {
        return (TypeTranslation[]) ActiveRecordBase.FindAll(typeof(TypeTranslation));
    }

    public static TypeTranslation Find(int id)
    {
        return (TypeTranslation) ActiveRecordBase.FindByPrimaryKey( typeof(TypeTranslation), id );
    }

    public static TypeTranslation FindByTypeAndLang(Type type, Language lang)
    {
        TypeTranslation[] translations = (TypeTranslation[]) ActiveRecordBase.FindAll( typeof(TypeTranslation),
                               Expression.Eq("Type", type), Expression.Eq("Lang", lang));
        if ((translations != null) && (translations.Length > 0))
            return translations[0];
        else
            return null;
    }

    public static TypeTranslation FindByTypeAndLang(Type type, string lang)
    {
            return null;
    }

}
}
