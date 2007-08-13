// Authors:
//    Héctor Rojas  <hectorrojas@shidix.com>
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
using NHibernate.Expression;
using System;

namespace CastlePortal
{
[Layout ("general")]
[Helper (typeof (MenuHelper))]
[Helper (typeof (StringHelper))]
[Rescue("generalerror")]
[Resource( "l10n", "l10n" )]
[LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
[Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
[Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
[DefaultAction("Redir")]
//[Filter (ExecuteEnum.BeforeAction, typeof (CheckGroupFilter))]
public class AclsController:ARSmartDispatcherController      
{
    //static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    
    public void Redir()
    {
        Response.Redirect(Constants.PORTAL_CONTROLLER , "index");
    }

    public void Index()
    {
        RedirectToAction("list");
    }

    public void AclsEdit()
    {
        PropertyBag["categories"] = Category.FindAll();
        PropertyBag["groups"] = Group.FindAll();
        PropertyBag["roles"] = Role.FindAll();
    }

    public void List()
    {
        PropertyBag["acls"] = Acl.FindAll();
    }

    public void Edit(int id)
    {
         PropertyBag["acl"] = Acl.Find(id);
         PropertyBag["groups"] = Group.FindAll();
         PropertyBag["roles"] = Role.FindAll();
    }

    public void Update([ARDataBind("acl", AutoLoadBehavior.Always)]Acl acl)
    {
        if (acl.IsValid())
        {
            acl.Update();
            Flash["edited"] = acl.Id;
            RedirectToAction("list");
        }
        else
        {
            PropertyBag["acl"] = acl;
            RenderView("edit");
        }
    }

    public void New()
    {
        PropertyBag["acl"] = new Acl();
        PropertyBag["groups"] = Group.FindAll();
        PropertyBag["roles"] = Role.FindAll();
    }

    public void Create([DataBind("acl")]Acl acl)
    {
        if (acl.IsValid())
        {
            acl.Create();
            Flash["edited"] = acl.Id;
            RedirectToAction("list");
        }
        else
        {
            PropertyBag["acl"] = acl;
            RenderView("new");
        }
    }

    public void Delete(int id)
    {
//        Usuario.Find(id).Delete();
        CancelView();
    }

    public void AclSave(int idCategory, int idGroup, string[] roles)
    {
        Container category = Container.Find(idCategory);
        Group group = Group.Find(idGroup);
        Acl[] acls = Acl.FindAll();

        bool rm = false;
        do
        {
            rm = false;
            foreach(Acl acl in category.AclSet)
            {
                foreach(Container c in acl.Containers)
                {
                    if ((c.Id == category.Id) && (acl.Group.Id == group.Id))
                    {
                        rm = true;
                        acl.Containers.Remove(c);
                        category.AclSet.Remove(acl);
                        break;
                    }
                }
                acl.Save();
                category.Save();
                break;
            }
        } while (rm);

        // Add acls to category
        foreach (string r in roles)
        {
            Role role = Role.Find(Int32.Parse(r));

            foreach (Acl acl in acls)
            {
                if ((acl.Group.Id == group.Id) && (acl.Role.Id == role.Id))
                {
                    acl.Containers.Add(category);
                    category.AclSet.Add(acl);
                    acl.Save();
                    category.Save();
                }
            }
        }

        RedirectToAction("aclsedit");
    }

    public void GetRolesByCategoryAndGroup(int idGroup, int idCategory)
    {
        if ((idGroup != 0) && (idCategory != 0))
        {
            Group group = Group.Find(idGroup);
            Category category = Category.Find(idCategory);

            Acl[] acls = Acl.FindAll();
            IList rolesByCategoryAndGroup = new ArrayList();
            foreach (Acl a in acls)
            {
                foreach (Container c in a.Containers)
                {
                    if ((a.Group.Id == group.Id) && (c.Id == category.Id))
                        rolesByCategoryAndGroup.Add(a.Role);
                }
            }

            PropertyBag["rolesbycategoryandgroup"] = rolesByCategoryAndGroup; 
        }

        PropertyBag["roles"] = Role.FindAll();
        LayoutName = null;
    }

    public void RolesEdit()
    {
        PropertyBag["roles"] = Role.FindAll();
    }

    public void RolesSave()
    {
        Role[] roles = Role.FindAll();
        string[] permissions = null;
        foreach (Role r in roles)
        {
            if (Request.Form[r.Id.ToString()] != null)
            {
                permissions = (Request.Form[r.Id.ToString()]).Split(',');
                r.CanCreate = false;
                r.CanModify = false;
                r.CanDelete = false;
                r.CanPublish = false;
                r.CanRead = false;
                foreach (string s in permissions)
                {
                    if (s != null)
                    {
                        if (s.CompareTo(Constants.PERMISSION_CREATE) == 0) r.CanCreate = true;
                        if (s.CompareTo(Constants.PERMISSION_MODIFY) == 0) r.CanModify = true;
                        if (s.CompareTo(Constants.PERMISSION_DELETE) == 0) r.CanDelete = true;
                        if (s.CompareTo(Constants.PERMISSION_PUBLISH) == 0) r.CanPublish = true;
                        if (s.CompareTo(Constants.PERMISSION_READ) == 0) r.CanRead = true;
                    }
                }
            r.Save();
            }
        }

        RedirectToAction("rolesedit");
    }

    public void RoleNew(string name)
    {
        Role role = new Role();
        role.Name = name;
        role.Save();

        Group[] groups = Group.FindAll();
        foreach (Group g in groups)
        {
            Acl acl = new Acl();
            acl.Group = g;
            acl.Role = role;
            acl.Containers = new Iesi.Collections.HashedSet(); // ArrayList();
            acl.Save();
        }

        RedirectToAction("rolesedit");
    }

    public void RoleDelete([ARFetch("id", Create = false)]Role role)
    {
        Acl[] acls = Acl.FindAll();
        foreach (Acl acl in acls)
        {
            if (acl.Role.Id == role.Id)
                acl.Delete();
        }

        Group[] groups = Group.FindAll();
        foreach (Group g in groups)
        {
            g.Roles.Remove(role);
            g.Save();
        }

        role.Delete();

        RedirectToAction("rolesedit");
    }
}
}

