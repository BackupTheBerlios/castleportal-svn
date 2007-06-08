// Authors:
//    Alberto Morales <amd77@gulic.org>
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

using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using System.Collections;
using NHibernate.Expression;

namespace CastlePortal
{
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class Template: ActiveRecordBase
{
    private int _Id;
    private string _name;
    private bool _public;
    private string _description;
    private string _tList;
    private string _tView;
    private string _tEdit;
    private IList _listingVisibleFields;
    private IList _fields;

    public Template () { }

    public Template (string name, string description, string tList, string tView, string tEdit)
    {
        _name = name;
        _description = description;
        _tList = tList;
        _tView = tView;
        _tEdit = tEdit;
    }

    [PrimaryKey]
    public int Id
    {
        get { return _Id; }
        set { _Id = value; }
    }

    [Property]
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    [Property]
    public bool Public
    {
        get { return _public; }
        set { _public = value; }
    }

    [Property]
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    [Property]
    public string TList
    {
        get { return _tList; }
        set { _tList = value; }
    }

    [Property]
    public string TView
    {
        get { return _tView; }
        set { _tView = value; }
    }

    [Property]
    public string TEdit
    {
        get { return _tEdit; }
        set { _tEdit = value; }
    }

#if CACHE
    [HasMany(typeof(FieldTemplate), Where="orderlist > 0", OrderBy="orderlist", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasMany(typeof(FieldTemplate), Where="orderlist > 0", OrderBy="orderlist", Lazy=true)]
#endif
    public IList ListingVisibleFields
    {
        get { return _listingVisibleFields; }
        set { _listingVisibleFields = value; }
    }

#if CACHE
    [HasMany(typeof(FieldTemplate), Where="orderedit > 0", OrderBy="orderedit", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasMany(typeof(FieldTemplate), Where="orderedit > 0", OrderBy="orderedit", Lazy=true)]
#endif
    public IList Fields
    {
        get { return _fields; }
        set { _fields = value; }
    }

    public override string ToString()
    {
        return "Template " + _name;
    }

    public static Template[] FindAll()
    {
        return (Template[]) ActiveRecordBase.FindAll(
                   typeof(Template),
                   new Order[] { Order.Asc("Name") }
               );
    }
    
    public static Template[] FindAllPublic()
    {
        SimpleQuery q = new SimpleQuery(typeof(Template), @"from Template where Public = true");
        Template[] allpublic = (Template[]) ExecuteQuery(q);
        return allpublic;
    }

    public static Template Find(int id)
    {
        return (Template) ActiveRecordBase.FindByPrimaryKey( typeof(Template), id );
    }

    public static Template GetDefault()
    {
        return (Template) ActiveRecordBase.FindOne( typeof(Template), Expression.Eq("Name", "default"));
    }

    public static Template FindByName(string name)
    {
        return (Template) ActiveRecordBase.FindOne( typeof(Template), Expression.Eq("Name", name));
    }
}
}
