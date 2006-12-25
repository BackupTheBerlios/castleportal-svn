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
public class Language : ActiveRecordBase
{
    private int _id;
    private string _name;
    private string _englishName;
    private string _description;

    public Language () {}

    public Language (string name, string englishName, string description)
    {
        _name = name;
	_englishName = englishName;
        _description = description;
    }

    public Language(string name, string englishName)
    {
        _name = name;
        _englishName = englishName;	
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
    public string EnglishName
    {
        get { return _englishName; }
        set { _englishName = value; }	
    }
    
    [Property]
    public string Description
    {
        get { return _description; }
        set { _description= value;}
    }

    public override string ToString()
    {
        return _name + " (" + _englishName + ")";
    }

    public new static Language[] FindAll()
    {
        return (Language[]) ActiveRecordBase.FindAll( typeof(Language), new Order[] { Order.Asc("EnglishName") });
    }

    public new static Language Find(int id)
    {
       try
       {
          return (Language) ActiveRecordBase.FindByPrimaryKey( typeof(Language), id );
       }
       catch(NotFoundException)
       {
          return null;
       }
       catch(System.Exception ex)
       {
          throw ex;
       }
    }

    public static Language FindByEnglishName(string langName)
    {
        return (Language) FindOne( typeof(Language),
                               Expression.Eq("EnglishName", langName)
                             );
    }

    public static Language FindByName(string langName)
    {
        return (Language) FindOne( typeof(Language),
                               Expression.Eq("Name", langName)
                             );
    }
}
}
