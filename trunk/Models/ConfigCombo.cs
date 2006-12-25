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
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class ConfigCombo: ActiveRecordBase
{
    private int _configComboId;
    private string _key;
    private string _name;
    private string _val;
    private string _descr;

    public ConfigCombo () {}

    public ConfigCombo (string key, string val, string descr)
    {
        _key = key;
        _val = val;
        _descr = descr;
    }

    [PrimaryKey]
    public int ConfigComboId
    {
        get { return _configComboId; }
        set { _configComboId = value; }
    }

    [Property]
    public string Key
    {
        get { return _key; }
        set { _key = value; }
    }

    [Property]
    public string Name
    {
        get { return _name; }
        set { _name = value; }
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
        get { return _descr; }
        set { _descr = value; }
    }

    public override string ToString()
    {
        return _val;
    }

    public new static ConfigCombo Find(int id)
    {
        return (ConfigCombo) ActiveRecordBase.FindByPrimaryKey( typeof(ConfigCombo), id );
    }

    public new static ConfigCombo[] FindAll()
    {
        return (ConfigCombo[]) ActiveRecordBase.FindAll(typeof(ConfigCombo));
    }

    public static string Value(string keyname)
    {
        ConfigCombo c = (ConfigCombo) FindOne( typeof(ConfigCombo), Expression.Eq("Key", keyname));
        if (c==null)
            return null;
        else
            return c.Val;
    }

    public static ConfigCombo[] FindAllByKey(string keyname)
    {
        return (ConfigCombo[]) FindAll( typeof(ConfigCombo), Expression.Eq("Key", keyname));
    }
}
}
