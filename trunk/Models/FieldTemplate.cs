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
using Castle.ActiveRecord.Queries;

namespace CastlePortal
{
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class FieldTemplate : ActiveRecordBase
{
    private int _id;
    private Field _field;
    private Template _template;
    private int _orderlist;
    private int _orderedit;

    public FieldTemplate() {}

    public FieldTemplate(Field field, Template template, int orderlist, int orderedit)
    {
        _field = field;
        _template = template;
        _orderlist = orderlist;
        _orderedit = orderedit;
    }

    [PrimaryKey]
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [BelongsTo]
    public Field Field
    {
        get { return _field; }
        set { _field = value; }
    }

    [BelongsTo]
    public Template Template
    {
        get { return _template; }
        set { _template = value; }
    }

    [Property]
    public int OrderList
    {
        get { return _orderlist; }
        set { _orderlist = value;}
    }

    [Property]
    public int OrderEdit
    {
        get { return _orderedit; }
        set { _orderedit = value;}
    }

    public new static FieldTemplate[] FindAll()
    {
        return (FieldTemplate[]) ActiveRecordBase.FindAll( typeof(FieldTemplate), new Order[] { Order.Asc("Name") });
    }

    public new static FieldTemplate Find(int id)
    {
        return (FieldTemplate) ActiveRecordBase.FindByPrimaryKey( typeof(FieldTemplate), id );
    }

    public static FieldTemplate[] FindByTemplate(int template)
    {
        return (FieldTemplate[]) ActiveRecordBase.FindAll(
            typeof(FieldTemplate),
            Expression.Eq("Template.Id", template )
        );

    }

    public static FieldTemplate FindByFieldAndTemplate(int field, int template)
    {
        return (FieldTemplate) ActiveRecordBase.FindOne(
            typeof(FieldTemplate),
            Expression.Eq("Field.Id", field ),
            Expression.Eq("Template.Id", template )
        );
    }
}
}
