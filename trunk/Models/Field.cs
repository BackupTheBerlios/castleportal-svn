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
public class Field : ActiveRecordBase
{
    private int _id;
    private string _name;
    private string _description;
    private IList _templateList;
    private Type _type;

    public Field()
    {}

    public Field(string name, string description, Type type)
    {
        _name = name;
        _description = description;
        _type = type;
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
        set { _name = value; }
    }

    [Property]
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

#if CACHE
    [HasAndBelongsToMany(typeof(Template), Table="fields_template", ColumnRef="template_id", ColumnKey="field_id", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasAndBelongsToMany(typeof(Template), Table="fields_template", ColumnRef="template_id", ColumnKey="field_id", Lazy=true)]
#endif
    public IList TemplateList
    {
        get { return _templateList; }
        set { _templateList = value; }
    }

    [BelongsTo]
    public Type Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public override string ToString()
    {
        return _description;
    }

    public new static Field[] FindAll()
    {
        return (Field[]) ActiveRecordBase.FindAll( typeof(Field), new Order[] { Order.Asc("Description") });
    }

    public new static Field Find(int id)
    {
        return (Field) ActiveRecordBase.FindByPrimaryKey( typeof(Field), id );
    }

    public static Field GetByName(string name)
    {
        return (Field) ActiveRecordBase.FindOne(
                   typeof(Field),
                   Expression.Eq("Name", name )
               );

    }

    public IList ReferenceRowList()
    {
        // El nombre de la category está en Field.Type.Name
        Category c = Category.FindByName(_name);
        if (c == null)
        {
            System.Console.WriteLine("ReferenceRowList() error: no existe la categoria "+_name);
            return new ArrayList();
        }
        else
        {
            return c.ContentList;
        }
        // El id de la row elegida esta en this.Values
        /*System.Console.WriteLine("Encontrando una row llamada {0} ({1})", _values, int.Parse(_values));
        Row r = Row.Find(int.Parse(_values));
        System.Console.WriteLine("...Encontré " + r);
        return r;*/
    }
}
}
