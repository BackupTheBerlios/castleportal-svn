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
[ActiveRecord("acl", Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord("acl")]
#endif
public class Acl : ActiveRecordBase
{
    private int _Id;
    private string _Code;
    private Group _Group;
    private Role _Role;
    public ISet _Containers;

    public Acl () { }

    public Acl(string code, Group g, Role r)
    {
        _Code = code;
        _Group = g;
        _Role = r;
    }

    public Acl(int groupid, int roleid)
    {
        _Group = Group.Find(groupid);
        _Role = Role.Find(roleid);
    }

    [PrimaryKey(PrimaryKeyType.Native, "acl_id")]
    public int Id
    {
        get { return _Id; }
        set { _Id = value; }
    }

    [Property]
    public string Code
    {
        get { return _Code; }
        set { _Code = value; }
    }

    [BelongsTo("group_id")]
    public Group Group
    {
        get { return _Group; }
        set { _Group = value; }
    }

    [BelongsTo("role_id")]
    public Role Role
    {
        get { return _Role; }
        set { _Role = value; }
    }

#if CACHE
    [HasAndBelongsToMany(typeof(Container), RelationType.Set, Table="containeracl", ColumnRef="container_id", ColumnKey="acl_id", Inverse=true, Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasAndBelongsToMany(typeof(Container), RelationType.Set, Table="containeracl", ColumnRef="container_id", ColumnKey="acl_id", Inverse=true, Lazy=true)]
#endif
    public ISet Containers
    {
        get { return _Containers; }
        set { _Containers = value; }
    }

    public override string ToString()
    {
        return _Role + " -> " + _Group;
    }

    public new static Acl[] FindAll()
    {
        return (Acl[]) ActiveRecordBase.FindAll(
                   typeof(Acl)
               );
    }

    public new static Acl Find(int id)
    {
        return (Acl) ActiveRecordBase.FindByPrimaryKey( typeof(Acl), id );
    }

    public new static Acl[] FindByGroup(Group g)
    {
        return FindByGroup(g.Id);
    }

    public new static Acl[] FindByGroup(int groupid)
    {
        return (Acl[]) ActiveRecordBase.FindAll(
                   typeof(Acl),
                   Expression.Eq("Group.Id", groupid)
               );
    }

    public new static Acl FindByGroupRole(int groupid, int roleid)
    {
        return (Acl) ActiveRecordBase.FindOne(
                   typeof(Acl),
                   Expression.Eq("Group.Id", groupid),
                   Expression.Eq("Role.Id", roleid)
               );
    }
    
    public override bool Equals(object o)
    {
        Acl acl = o as Acl;
        if (acl != null)
        {
            if (Id == acl.Id)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    
    public override int GetHashCode()
    {
        return Id;
    }

}
}
