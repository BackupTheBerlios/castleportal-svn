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

namespace CastlePortal
{
/// <summary>
/// Table to store general site variables like "published"
/// </summary>
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class ConfigModel: ActiveRecordBase
{
    private int _id;
    private string _key;
    private string _val;
    private string _desc;

    public ConfigModel () {}

    public ConfigModel (string key, string val, string desc)
    {
        _key = key;
        _val = val;
        _desc = desc;
    }

    [PrimaryKey]
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [Property(Unique=true)]
    public string Key
    {
        get { return _key; }
        set { _key = value; }
    }

    [Property]
    public string Val
    {
        get { return _val; }
        set { _val = value; }
    }

    [Property]
    public string Description
    {
        get { return _desc; }
        set { _desc = value; }
    }

    public override string ToString()
    {
        return _val;
    }

    public static ConfigModel Find(int id)
    {
        return (ConfigModel) ActiveRecordBase.FindByPrimaryKey( typeof(ConfigModel), id );
    }

    public new static ConfigModel[] FindAll()
    {
        return (ConfigModel[]) ActiveRecordBase.FindAll(typeof(ConfigModel));
    }

    public static string GetValue(string keyname)
    {
        ConfigModel config = (ConfigModel) FindOne( typeof(ConfigModel), Expression.Eq("Key", keyname));
        if (config == null)
            throw new Castle.ActiveRecord.Framework.ActiveRecordException("ConfigModel.GetValue");
        else
            return config.Val;
    }

    public static ConfigModel FindByKey(string keyname)
    {
        ConfigModel config = (ConfigModel) FindOne( typeof(ConfigModel), Expression.Eq("Key", keyname));
        if (config == null)
            return null;
        else
            return config;
    }
}
}
