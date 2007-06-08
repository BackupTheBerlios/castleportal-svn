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
using Castle.ActiveRecord.Queries;
using Castle.ActiveRecord;
using System.Collections;
using Iesi.Collections;
using NHibernate.Expression;
using System;

namespace CastlePortal
{
/// <summary>
/// Base class for categories and others.
/// See doc about Active Record hierarchy at:
/// http://castleproject.org/index.php/ActiveRecord:Type_hierarchy#Type_hierarchy_using_joined_subclasses
/// </summary>

#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
[JoinedBase]
public class Container : ActiveRecordBase
{
#if DEBUG
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
#endif
    private int _Id;
    private string _Name;
    private Language _Lang;
    private User _Owner;
    private Role _AnonRole;
    private ISet _AclSet;

    public Container ()
    { }

    public Container ( string __Name, User __Owner)
    {
        _Name = __Name;
        _Owner = __Owner;
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

    [BelongsTo]
    public User Owner
    {
        get { return _Owner; }
        set { _Owner = value; }
    }

    [BelongsTo]
    public Role AnonRole
    {
        get { return _AnonRole; }
        set { _AnonRole = value; }
    }

    [BelongsTo]
    public Language Lang
    {
        get { return _Lang; }
        set { _Lang = value; }
    }

    
#if CACHE
    [HasAndBelongsToMany(typeof(Acl), RelationType.Set, Table="containeracl", ColumnRef="acl_id", ColumnKey="container_id", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
    [HasAndBelongsToMany(typeof(Acl), RelationType.Set, Table="containeracl", ColumnRef="acl_id", ColumnKey="container_id", Lazy=true)]
#endif
    public ISet AclSet
    {
        get { return _AclSet;}
        set { _AclSet = value; }
    }

    public override string ToString()
    {
        return _Name + " (" + _Owner + ")";
    }

    public static Container[] FindAll()
    {
      try
      {  	      
        return (Container[]) ActiveRecordBase.FindAll(
                   typeof(Container),
                   new Order[] { Order.Asc("Name") }
               );
      }
      catch (Exception ex)
      {
         logger.Error("Container FindAll Exception: " + ex.Message + ex.StackTrace);
         return null;
      }	      
    }

    public static Container Find(int id)
    {
        return (Container) ActiveRecordBase.FindByPrimaryKey( typeof(Container), id );
    }

    /// <summary>
    ///	Return or throw an exception if user has not permissions
    /// </summary>
    public void CanOrThrow(User user, Permission permission)
    {
        if (user == null)
        {
            if ((_AnonRole != null) && (_AnonRole.Can(permission)))
                return;
            else
                throw new LoginRequired("");
        }
        else
        {
            if ((_Owner != null) && (user.Id == _Owner.Id))
                return;

            foreach (Acl acl in AclSet)
            if (acl.Role.Can(permission) && acl.Group.HasUser(user))
                return;
            throw new Unauthorized("");
        }
    }

    public bool Can(User user, Permission permission)
    {
        if (user == null)
        {
            if (_AnonRole != null && AnonRole.Can(permission))
                return true;
            else
                return false;
        }
        else
        {
            if (_Owner != null && user.Id == _Owner.Id)
                return true;

            foreach (Acl acl in AclSet)
            {
                if (acl.Role.Can(permission) && acl.Group.HasUser(user))
                    return true;
            }
            return false;
        }
    }

    public Hashtable GetPermissionsHash(User user)
    {
        if (user == null)
        {
            if (AnonRole == null)
                return Commons.GetPermissionsBaseHash(false);
            else
                return AnonRole.ToHashtable();
        }
        else
        {
            if (_Owner != null && user.Id == _Owner.Id)
                return Commons.GetPermissionsBaseHash(true);
            else
            {
                Hashtable permissions = Commons.GetPermissionsBaseHash(false);
                if (AclSet != null)
                    foreach (Acl acl in AclSet)
                    if (acl.Group.HasUser(user))
                        acl.Role.UpdateHash(ref permissions);
                return permissions;
            }
        }
    }

    public static Hashtable GetHashesByUser(User user)
    {
        if (user == null)
        {
            return null;
        }
        Hashtable hash = new Hashtable();
        Container[] conts = FindAll();
        if (conts == null)
        {
            return null;
        }
        foreach (Container container in conts)
        {
            hash[container.Id] = container.GetPermissionsHash(user);
        }
        return hash;
    }

    public bool CanCreate(User u)
    {
        return Can(u, Permission.Create);
    }
    public bool CanRead(User u)
    {
        return Can(u, Permission.Read);
    }
    public bool CanModify(User u)
    {
        return Can(u, Permission.Modify);
    }
    public bool CanDelete(User u)
    {
        return Can(u, Permission.Delete);
    }
    public bool CanPublish(User u)
    {
        return Can(u, Permission.Publish);
    }
    
    public override bool Equals(object o)
    {
        Container container = o as Container;
        if (container != null)
        {
            if (Id == container.Id)
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
