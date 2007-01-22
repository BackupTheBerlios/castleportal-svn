// Authors:
//    Carlos Ble <carlosble@shidix.com>
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
#if CACHE
[ActiveRecord("`user`", Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord("`user`")]
#endif
public class User : ActiveRecordValidationBase, System.Security.Principal.IPrincipal, System.Security.Principal.IIdentity
{
    private int _Id;
    private string _Name;
    private string _UserPassword;
    private Group _SessionGroup;
    private IList _Groups;

    public User ()
    {
    }

    public User (string __Name, string __UserPassword)
    {
        _Name = __Name;
        _UserPassword = __UserPassword;
    }

    [PrimaryKey]
    public int Id
    {
        get { return _Id; }
        set { _Id = value; }
    }

    [Property(NotNull=true, Unique=true)]
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    [Property]
    public string UserPassword
    {
        get { return _UserPassword; }  
        set { _UserPassword = Hash(value);}
    }

    [BelongsTo]
    public Group SessionGroup
    {
        get { return _SessionGroup; }
        set { _SessionGroup = value; }
    }

#if CACHE
    [HasAndBelongsToMany(typeof(Group), Table="usersgroups", ColumnRef="groupid", ColumnKey="userid", Cache=CacheEnum.ReadWrite, Cascade = ManyRelationCascadeEnum.SaveUpdate)]
#else
    [HasAndBelongsToMany(typeof(Group), Table="usersgroups", ColumnRef="groupid", ColumnKey="userid")]
#endif
    public IList Groups
    {
        get { return _Groups; }
        set { _Groups = value; }
    }

    static System.Security.Cryptography.MD5 hasher = System.Security.Cryptography.MD5.Create();

    private static string Hash(string password)
    {
        if (password == null)
           return String.Empty;
        if (password.Length < 10)
        {
            byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = hasher.ComputeHash(valueBytes);
            return System.Convert.ToBase64String(hashedBytes);
        }
        else
            return password;
    }

#region IPrincipal Members
    public System.Security.Principal.IIdentity Identity
    {
        get { return this; }
    }
    
    public bool IsAuthenticated
    {
        get { return true; }
    }

    public string AuthenticationType
    {
        get { return "custom"; }
    }
    
    public bool IsInRole(string role)
    {
        return false;
    }
#endregion 

    public override string ToString()
    {
        return "User " + _Name;
    }

    public static User[] FindAll()
    {
        return (User[]) ActiveRecordBase.FindAll(
                   typeof(User),
                   new Order[] { Order.Asc("Name") }
               );
    }

    public static User[] FindAllWithoutRoot()
    {
        SimpleQuery q = new SimpleQuery(typeof(User), @"from User where Name != 'root'");
        User[] allwithout = (User[]) ExecuteQuery(q);
        return (User[]) allwithout;
    }

    public static User Find(int id)
    {
        return (User) ActiveRecordBase.FindByPrimaryKey( typeof(User), id );
    }

    public new static User FindByExactName(string name)
    {
        return (User) ActiveRecordBase.FindOne( typeof(User), Expression.Eq("Name", name));
    }

    public static User FindByUsernameAndPasswd(string username, string passwd)
    {
        string hashedPassword = Hash(passwd);

        return (User) ActiveRecordBase.FindOne(typeof(User), 
               Expression.Eq("Name", username), Expression.Eq("UserPassword", hashedPassword));
    }

    public Group[] NotGroups()
    {
        SimpleQuery q = new SimpleQuery(typeof(Group), @"from Group where Id not in (4)");
        Group[] allwithout = (Group[]) ExecuteQuery(q);
        return allwithout;
    }

    public static string FindPasswordByUsername(string username)
    {
        SimpleQuery q = new SimpleQuery(typeof(User), @"
                                        from User U
                                        where U.Name = ?", username);
        User u = (User) ExecuteQuery(q);
        return u.UserPassword;
    }

    public bool IsInGroup(string groupname)
    {
        foreach (Group g in Groups)
        if (g.Name == groupname)
            return true;
        return false;
    }

    public new static User[] FindByName(string name)
    {
        if (name.Length > 0)
            name = name.ToLower();
        name += '%';
        User[] ulower = (User[])ActiveRecordBase.FindAll(
                            typeof(User),
                            new Order[] { Order.Asc("Name") },
                            Expression.Like("Name", name));
        name = name.ToUpper();
        User[] uupper = (User[])ActiveRecordBase.FindAll(
                            typeof(User),
                            new Order[] { Order.Asc("Name") },
                            Expression.Like("Name", name));
        User[] r = new User[uupper.Length + ulower.Length];
        int i;
        for (i = 0; i < uupper.Length; i++)
            r[i] = uupper[i];
        for (int j = 0; j < ulower.Length; j++)
        {
            r[i] = ulower[j];
            i++;
        }
        return r;
    }
    
    public override bool Equals(object o)
    {
        User user = o as User;
        if (user != null)
        {
            if (Id == user.Id)
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
