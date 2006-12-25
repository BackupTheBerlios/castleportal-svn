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
public class Role : ActiveRecordBase
{
    private int _Id;
    private string _Name;
    private bool _CanCreate;
    private bool _CanModify;
    private bool _CanDelete;
    private bool _CanPublish;
    private bool _CanRead;
    private ISet _Groups;

    public Role () { }

    public Role (string __Name, bool c, bool m, bool d, bool p, bool r)
    {
        _Name = __Name;
        _CanCreate = c;
        _CanModify = m;
        _CanDelete= d;
        _CanPublish = p;
        _CanRead = r;
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
        get { return _Name; }
        set { _Name = value; }
    }

    [Property]
    public bool CanCreate
    {
        get { return _CanCreate; }
        set { _CanCreate = value; }
    }

    [Property]
    public bool CanModify
    {
        get { return _CanModify; }
        set { _CanModify = value; }
    }

    [Property]
    public bool CanDelete
    {
        get { return _CanDelete; }
        set { _CanDelete = value; }
    }

    [Property]
    public bool CanPublish
    {
        get { return _CanPublish; }
        set { _CanPublish = value; }
    }

    [Property]
    public bool CanRead
    {
        get { return _CanRead; }
        set { _CanRead = value; }
    }

    public bool Can (Permission perm)
    {
        switch (perm) {
        case Permission.Create: return _CanCreate;
        case Permission.Modify: return _CanModify;
        case Permission.Delete: return _CanDelete;
        case Permission.Publish: return _CanPublish;
        case Permission.Read: return _CanRead;
        default: return false;
        }
    }

#if CACHE
    [HasAndBelongsToMany(typeof(Group), RelationType.Set, Table="group_role", ColumnRef="group_id", ColumnKey="role_id", Cache=CacheEnum.ReadWrite)]
#else
    [HasAndBelongsToMany(typeof(Group), RelationType.Set, Table="group_role", ColumnRef="group_id", ColumnKey="role_id")]
#endif
    public ISet Groups
    {
    get { return _Groups; }
        set { _Groups = value; }
    }

    public override string ToString()
    {
        string s = "Rol ";
        s += _Name;
        s += " (" + _Id + ", ";
        s += (_CanCreate) ? "C" : "c";
        s += (_CanModify) ? "M" : "m";
        s += (_CanDelete) ? "D" : "d";
        s += (_CanPublish) ? "P" : "p";
        s += (_CanRead) ? "R" : "r";
        s += ")";
        return s;
    }

    public Hashtable ToHashtable()
    {
        Hashtable t = new Hashtable();
        t[Permission.Create] = _CanCreate;
        t[Permission.Modify] = _CanModify;
        t[Permission.Delete] = _CanDelete;
        t[Permission.Publish] = _CanPublish;
        t[Permission.Read] = _CanRead;
        return t;
    }

    public void UpdateHash(ref Hashtable t)
    {
        t[Permission.Create] = (bool)t[Permission.Create] || _CanCreate;
        t[Permission.Modify] = (bool)t[Permission.Modify] || _CanModify;
        t[Permission.Delete] = (bool)t[Permission.Delete] || _CanDelete;
        t[Permission.Publish] = (bool)t[Permission.Publish] || _CanPublish;
        t[Permission.Read] = (bool)t[Permission.Read] || _CanRead;
    }

    public static Role[] FindAll()
    {
        return (Role[]) ActiveRecordBase.FindAll(
                   typeof(Role),
                   new Order[] { Order.Asc("Name") }
               );
    }

    public static Role Find(int id)
    {
        return (Role) ActiveRecordBase.FindByPrimaryKey( typeof(Role), id );
    }

    public static Role FindByName(string roleName)
    {
        return (Role) FindOne( typeof(Role),
                               Expression.Eq("Name", roleName)
                             );
    }
    
    public override bool Equals(object o)
    {
        Role role = o as Role;
        if (role != null)
        {
            if (Id == role.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    
    public override int GetHashCode()
    {
        return Id;
    }
}
}
