// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Alberto Morales <amd77@gulic.org>
//    Héctor Rojas <hectorrojas@shidix.com>
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
using Castle.ActiveRecord.Framework;
using System.Collections;
using Iesi.Collections;
using NHibernate.Expression;
using System;

namespace CastlePortal
{
#if CACHE
[ActiveRecord("`group`",  Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord("`group`")] 
#endif
public class Group : ActiveRecordBase
{
    private int _Id;
    private string _Name;
    private ISet _Roles;

    private ISet _Users;


    public Group ()
    {
    }

    public Group (string __Name)
    {
        _Name = __Name;
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

#if CACHE
    [HasAndBelongsToMany(typeof(User), RelationType.Set, Table="usersgroups", ColumnRef="userid", ColumnKey="groupid", Cache=CacheEnum.ReadWrite, Cascade = ManyRelationCascadeEnum.SaveUpdate)]
#else
    [HasAndBelongsToMany(typeof(User), RelationType.Set, Table="usersgroups", ColumnRef="userid", ColumnKey="groupid", Cascade = ManyRelationCascadeEnum.SaveUpdate)]
#endif
    public ISet Users
    {
        get { return _Users; }
        set { _Users = value; }
    }

#if CACHE
    [HasAndBelongsToMany(typeof(Role), RelationType.Set, Table="group_role",  ColumnRef="role_id", ColumnKey="group_id", Cache=CacheEnum.ReadWrite)]
#else
    [HasAndBelongsToMany(typeof(Role), RelationType.Set, Table="group_role",  ColumnRef="role_id", ColumnKey="group_id")]
#endif
    public ISet Roles
    {
        get { return _Roles; }
        set { _Roles = value; }
    }

    public override string ToString()
    {
        return _Name;
    }

    public bool HasUser(User user)
    {
        foreach (User index in Users)
        if (index.Id == user.Id)
            return true;
        return false;
    }

    public new static Group[] FindAll()
    {
        return (Group[]) ActiveRecordBase.FindAll(
                   typeof(Group),
                   new Order[] { Order.Asc("Name") }
               );
    }

    public new static Group Find(int id)
    {
        try
        {
            return (Group) ActiveRecordBase.FindByPrimaryKey( typeof(Group), id );
        }
        catch (NotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public new static Group FindByExactName(string name)
    {
        return (Group) ActiveRecordBase.FindOne( typeof(Group), Expression.Eq("Name", name));
    }

    public new static Group[] FindByName(string name)
    {
        if (name.Length > 0)
            name = name.ToLower();
        name += '%';
        Group[] glower = (Group[])ActiveRecordBase.FindAll(
                             typeof(Group),
                             new Order[] { Order.Asc("Name") },
                             Expression.Like("Name", name));
        name = name.ToUpper();
        Group[] gupper = (Group[])ActiveRecordBase.FindAll(
                             typeof(Group),
                             new Order[] { Order.Asc("Name") },
                             Expression.Like("Name", name));
        Group[] r = new Group[gupper.Length + glower.Length];
        int i;
        for (i = 0; i < gupper.Length; i++)
            r[i] = gupper[i];
        for (int j = 0; j < glower.Length; j++)
        {
            r[i] = glower[j];
            i++;
        }
        return r;
    }
    
    public override bool Equals(object o)
    {
        Group group = o as Group;
        if (group != null)
        {
            if (Id == group.Id)
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


