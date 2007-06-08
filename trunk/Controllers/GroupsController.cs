// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Hector Rojas <hectorrojas@shidix.com>
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

using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using Castle.ActiveRecord.Framework;
using Iesi.Collections;
using Castle.ActiveRecord;
using NHibernate.Expression;
using System;

namespace CastlePortal
{
[Layout ("general")]
[Helper (typeof (ExtraHelper))]
[Helper (typeof (MenuHelper))]
[Helper (typeof (StringHelper))]
[Rescue("generalerror")]
[Resource( "l10n", "l10n" )]
[LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
[Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
[Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
[DefaultAction("Redir")]
//[Filter (ExecuteEnum.BeforeAction, typeof (CheckGroupFilter))]
public class GroupsController:ARSmartDispatcherController      
{
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    
    public void Redir()
    {
        Response.Redirect(Constants.PORTAL_CONTROLLER , "index");
    }

    public void AddUser(int gid, int uid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        AdminAPI.AddUser(gid, uid, gindex, uindex);
        GroupsEdit(gid, gindex, uindex);
        LayoutName = null;
        RenderView("groupsedit");
    }

    public void DelUser(int gid, int uid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        AdminAPI.DelUser(gid, uid, gindex, uindex);
        GroupsEdit(gid, gindex, uindex);
        LayoutName = null;
        RenderView("groupsedit");
    }

    public void GroupsEdit()
    {
        Commons.CheckSuperUser(Session);
        string ln = LayoutName;
        this.GroupsEdit(0, "", "");
        LayoutName = ln;
    }

    //public void GroupsEdit(Group group, string gindex, string uindex)
    public void GroupsEdit(int gid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        GroupsEdit(0, gid, gindex, uindex);
    }

    private void GroupsEdit(int uid, int gid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        if ((gindex == "")|| (gindex == null))
            PropertyBag["groups"] = Group.FindAll ();
        else
            PropertyBag["groups"] = Group.FindByName(gindex);

        User[] allusers;
        if ((uindex == "") || (uindex == null))
        {
            allusers = User.FindAll();
            PropertyBag["allusers"] = allusers;
        }
        //PropertyBag["allusers"] = User.FindAllWithoutRoot();
        else
        {
            allusers = User.FindAll();
            PropertyBag["allusers"] = allusers;
        }
        if (gid != 0)
        {
            Group group = Group.Find(gid);
            PropertyBag["group"] = group;
            PropertyBag["users"] = group.Users;

            ArrayList allwithout = new ArrayList();
            bool exist = false;
            foreach (User u in allusers)
            {
                exist = false;
                foreach (User u2 in group.Users)
                {
                    if (u.Id == u2.Id)
                       exist = true;
                }

                if (!exist)
                    allwithout.Add(u);
            }

            PropertyBag["allusers"] = allwithout;
        }

        PropertyBag["gindex"] = gindex;
        PropertyBag["uindex"] = uindex;
        PropertyBag["uid"] = uid;
        PropertyBag["roles"] = Role.FindAll ();
        LayoutName = null;
    }

    // Cuando se carga la primera vez, o cuando no hay ningun grupo seleccionado
    public void GroupsEdit (string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        this.GroupsEdit(0, gindex, uindex);
    }

    public void GroupSave (int groupId, string groupName)
    {
        Commons.CheckSuperUser(Session);
        Group group = Group.Find(groupId);
        group.Name = groupName;
        string rolesString = (string) Request.Form["Roles[]"];
        group.Roles.Clear();
        if (rolesString != null)
        {
            string[] roles = rolesString.Split(',');
            if (roles != null)
            {
                foreach (string i in roles)
                {
                    Role role = Role.Find(int.Parse(i));
                    if (role != null)
                    {
                        group.Roles.Add(role);
                    }
                }
            }
        }
        group.Save();    
        RedirectToAction ("groupsedit");
    }

    public void GroupCreate (string groupName)
    {
        Commons.CheckSuperUser(Session);
        Group group = new Group(groupName);
        string rolesString = (string) Request.Form["Roles[]"];
        if (rolesString != null)
        {
            string[] roles = rolesString.Split(',');
            if (roles != null)
            {
                group.Roles = new HashedSet();
                foreach (string i in roles)
                {
                    Role role = Role.Find(int.Parse(i));
                    if (role != null)
                    {
                        group.Roles.Add(role);
                    }
                }
            }
        }
        group.Save();

        Role[] rols = Role.FindAll();
        foreach (Role r in rols)
        {
            bool isDefaultRole = false;
            if (group.Roles != null)
            {
              foreach(Role rol in group.Roles)
              {
                 if (rol.Id == r.Id)
                 {
                    isDefaultRole = true;
                 }
              }
            }
            if ((isDefaultRole) && (Request.Form["updateAcls"] != null))
            {
                Acl acl = new Acl();
                acl.Group = group;
                acl.Role = r;
                acl.Containers = new Iesi.Collections.HashedSet(); //ArrayList();
                acl.Save();
                Category[] categories = Category.FindAll();
                foreach(Category category in categories)
                {
                    category.AclSet.Add(acl);
                    category.Save();
#if CACHE
                    acl.Containers.Add(category);
#endif
                }
            }
            //acl.Save();
        }
        
        RedirectToAction ("groupsedit");
    }
    
    private void GroupAclsDelete(Group group)
    {
        Commons.CheckSuperUser(Session);
        if (group != null)
        {
            Acl[] acls = Acl.FindAll();
            for (int i = 0; i < acls.Length; i++)
            {
                Acl acl = acls[i];
                //Console.WriteLine("i vale=" + i +", " +  acl.Id +", "+ acl.Group.Id);
                if (acl.Group.Id == group.Id)
                {
              //Console.WriteLine("i es del grupo=" + i +", " + acl.Id + ", " + acl.Containers.Count);
                    foreach(Container container in acl.Containers)
                    {
                    	if (container.AclSet.Contains(acl)) // Contains checks Id ?
                    	{
                    		container.AclSet.Remove(acl);
                    	}
                    	container.Save();
                    	/*if (acl.Containers.Contains(acl)
                    	{
                    		container
                    	}
                        int aclIndex = -1;
                        for (int j = 0; j < container.AclSet.Count; j++)
                        {
                        	// acl.Id == con
                            if (acl.Id == ((Acl)container.AclSet. [j]).Id)
                            {
                                aclIndex = j;
                                break;
                            }
                        }
                        if (aclIndex >= 0)
                        {
                            container.AclSet.RemoveAt(aclIndex); 
                        }*/
                        
                    }
                    acl.Containers.Clear();
                    acl.Delete();
                }    
            }
        }
        else 
        {
            throw new NotFoundException("Group not found");
        }
    }
    
    public void GroupAclsDelete(int groupId)
    {
        Commons.CheckSuperUser(Session);
        Group group = Group.Find(groupId);
        GroupAclsDelete(group);
        RedirectToAction ("groupsedit");
    }

    public void GroupDelete (int groupId)
    {
        Commons.CheckSuperUser(Session);
        Group group = Group.Find(groupId);
        //Console.WriteLine("GROUP DELETE:" + group.Id);
        try
        {
            if (group != null)
            {
                GroupAclsDelete(group);
#if CACHE
                foreach (User user in group.Users)
                {
                    if (user.Groups.Contains(group))
                    {
                        user.Groups.Remove(group);
                    }
                    /*int groupIndex = -1;    
                    for (int i = 0; i < user.Groups.Count; i++)
                    {
                        if (((Group)user.Groups[i]).Id == group.Id)
                        {
                            groupIndex = i;
                            break;
                        }
                    }
                    if (groupIndex >= 0)
                    {
                        user.Groups.RemoveAt(groupIndex);
                    }*/
                    user.Save();
                }
                
                foreach (Role role in group.Roles)
                {
                    /*int roleIndex = -1;
                    for (int i = 0; i < role.Groups.Count; i++)
                    {
                        if (((Group)role.Groups[i]).Id == group.Id)
                        {
                            roleIndex = i;
                            break;
                        }
                    }
                    if (roleIndex >= 0)
                    {
                        role.Groups.RemoveAt(roleIndex);
                    }*/
                    if (role.Groups.Contains(group))
                    {
                        role.Groups.Remove(group);
                    }
                    role.Save();
                }
#endif             
                group.Delete();
            }
        }
        catch (ActiveRecordException ex)
        {
            logger.Error("GroupDelete exception:" + ex.Message  + ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.Error("GroupDelete inner exception:" + ex.InnerException.Message  + 
                            ex.InnerException.StackTrace);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            RedirectToAction ("groupsedit");
        }
    }

}
}
