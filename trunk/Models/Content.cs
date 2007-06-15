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
using System;

namespace CastlePortal
{
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class Content : ActiveRecordBase
{
#if DEBUG
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
#endif
    private int _id;
    private bool _published;
    private bool _frontpage;
    private bool _sectionFrontpage;
    private Category _category;
    private Language _lang;
    private IDictionary _dataHash = new Hashtable();
    private System.DateTime _creationdate;

    public Content () {}

    public Content (Category category)
    {
        _category = category;
        _creationdate = System.DateTime.Now;
    }

    public Content(Category __category, Language __lang, bool __published, bool __f, bool __sf, DateTime __dt)
    {
        _category = __category;
        _lang = __lang;
        _published = __published;
        _frontpage = __f;
        _sectionFrontpage = __sf;
        _creationdate = __dt;
    }

    [PrimaryKey]
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [Property]
    public System.DateTime CreationDate
    {
        get { return _creationdate; }
        set { _creationdate = value; }
    }

    [Property]
    public bool Published
    {
        get { return _published; }
        set { _published = value; }
    }
 
    [Property]
    public bool Frontpage
    {
        get { return _frontpage; }
        set { _frontpage = value; }
    }

    [Property]
    public bool SectionFrontpage
    {
        get { return _sectionFrontpage; }
        set { _sectionFrontpage = value; }
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

#if CACHE
    [HasMany(typeof(DataModel), Index="FieldName" ,IndexType="string", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasMany(typeof(DataModel), Index="FieldName" ,IndexType="string", Lazy=true)]
#endif
    public IDictionary DataHash
    {
        get { return _dataHash; }
        set { _dataHash = value; }
    }

    public static Content[]FindAll()
    {
        return (Content[])ActiveRecordBase.FindAll(typeof(Content));
    }

    public string GetValueByFieldName(string fieldName)
    {
        if (DataHash.Contains(fieldName)) 
        {
            DataModel d = (DataModel) DataHash[fieldName];
            return d.Value;
        }
        else 
        {
            return "";
//            throw new NotFoundException("GetValueByFieldName:" + fieldName);
        }
    }

    public bool ExistsField(string fieldName)
    {
        if (DataHash.Contains(fieldName))
            return true;
        else
            return false;
    }

    public bool ExistsFieldAndHaveContent(string fieldName)
    {
        if (DataHash.Contains(fieldName))
        {
            if (((DataModel)DataHash[fieldName]).Value.Length > 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public DataModel GetDataModelByFieldName(string fieldName)
    {
        if (DataHash.Contains(fieldName)) 
        {
            return (DataModel) DataHash[fieldName];
        } 
        else 
        {
            return null;
        }
    }

    public static Content[] GetCategoryContents (Category c)
    {
        return (Content[]) ActiveRecordBase.FindAll(
                   typeof(Content),
                   Expression.Eq("Category", c )
               );
    }

    public static Content Find(int id)
    {
        try
        {		
           return (Content) ActiveRecordBase.FindByPrimaryKey( typeof(Content), id );
        }
        catch (NotFoundException nfe)
        {	
           logger.Error("Content not found: " + id);		
           throw nfe;
           //return null;
        }
        catch (Exception ex)
        {
           throw ex;
        }		
    }
}
}

