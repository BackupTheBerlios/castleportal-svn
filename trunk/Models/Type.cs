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

using Castle.ActiveRecord;
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
public class Type : ActiveRecordBase
{
    private int _id;
    private string _name;
    private string _description;

    public Type () {}

    public Type (string name, string description)
    {
        _name = name;
        _description = description;
    }

    [PrimaryKey]
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }


    [Property]
    public string Name
    {
        get { return _name; }
        set { _name = value;}
    }

    [Property]
    public string Description
    {
        get { return _description; }
        set { _description= value;}
    }

    public override string ToString()
    {
        return _name + " (" + _description + ")";
    }

    public new static Type[] FindAll()
    {
        return (Type[]) ActiveRecordBase.FindAll( typeof(Type), new Order[] { Order.Asc("Name") });
    }

    public new static Type Find(int id)
    {
        return (Type) ActiveRecordBase.FindByPrimaryKey( typeof(Type), id );
    }

    public static Type FindByName(string typeName)
    {
        return (Type) FindOne( typeof(Type),
                               Expression.Eq("Name", typeName)
                             );
    }

    public string FindTranslation(string lang)
    {
        if ((lang == null) || (lang.Length == 0))
            return Description;
        ISession session = holder.CreateSession(typeof (TypeTranslation));
        string query = "select typetranslation.translation from  type, language, typetranslation ";
        query += "where typetranslation.menu = " + Id;
        query += " and typetranslation.lang = language.id and language.englishname = '" + lang +"'";
        IQuery sqlQuery = session.CreateSQLQuery(query, "typetranslation", typeof(TypeTranslation));
        sqlQuery.SetMaxResults(1);
        IList translations = sqlQuery.List();
        if ((translations != null) && (translations.Count > 0))
            return ((TypeTranslation)translations[0]).Translation;
        else
            return Description; //Constants.NO_TRANSLATION_FOUND;
    }
}
}
