// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Alberto Morales <amd77@shidix.com>
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
using System;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using NHibernate.Expression;

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
public class UsersController:ARSmartDispatcherController      
{
    public void Redir()
    {
        Response.Redirect("portal" , "index");
    }

    public void index ()
    {
        Commons.CheckSuperUser(Session);
    }

    // ROLES

    public void RolesEdit ()
    {
        Commons.CheckSuperUser(Session);
        PropertyBag["roles"] = Role.FindAll ();
    }

    public void RoleCreate ([DataBind ("Form")] Role role)
    {
        Commons.CheckSuperUser(Session);
        role.Create ();
        Flash["aviso"] = "Creado role";
        RedirectToAction ("rolesedit");
    }

    public void RoleSave ([DataBind ("Form")] Role role)
    {
        Commons.CheckSuperUser(Session);
        role.Save();
        Flash["aviso"] = "Guardado role";
        RedirectToAction ("rolesedit");
    }

    public void RoleDelete ([ARFetch ("Id", Create = false)] Role role)
    {
        Commons.CheckSuperUser(Session);
        role.Delete ();
        Flash["aviso"] = "Borrado rol";
        RedirectToAction ("rolesedit");
    }


    // USERS
    public void AddUser(int gid, int uid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        AdminAPI.AddUser(gid, uid, gindex, uindex);
        UsersEdit(uid, gindex, uindex);
        LayoutName = null;
        RenderView("usersedit");
    }

    public void DelUser(int gid, int uid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        AdminAPI.DelUser(gid, uid, gindex, uindex);
        UsersEdit(uid, gindex, uindex);
        LayoutName = null;
        RenderView("usersedit");
    }

    public void UsersEdit()
    {
        Commons.CheckSuperUser(Session);
        string ln = LayoutName;
        this.UsersEdit(0, "", "");
        LayoutName = ln;
    }

    public void UsersEdit(int uid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        UsersEdit(0, uid, gindex, uindex);
    }

    private void UsersEdit(int gid, int uid, string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        if ((uindex == "")|| (uindex == null))
            PropertyBag["users"] = User.FindAll ();
        //PropertyBag["users"] = User.FindAllWithoutRoot ();
        else
            PropertyBag["users"] = User.FindByName(uindex);

        if ((gindex == "") || (gindex == null))
            PropertyBag["allgroups"] = Group.FindAll();
        else
            PropertyBag["allgroups"] = Group.FindByName(gindex);
        if (uid != 0)
        {
            User user = User.Find(uid);
            user.NotGroups();
            PropertyBag["user"] = user;
            PropertyBag["groups"] = user.Groups;
            ArrayList allwithout = new ArrayList((Group[])PropertyBag["allgroups"]);
            foreach (Group g in user.Groups)
            //if (allwithout.Contains(u))  NO RULA porque las instancias no coinciden
            //	allwithout.Remove(u);
            //	Por eso hace falta este otro bucle, aunque la mejor solucion seria usar :
            //	BinarySearch (object value, IComparer comparer) del ArrayList
            foreach (Group i in allwithout)
            if (g.Name == i.Name)
                i.Name = "";
            PropertyBag["allgroups"] = allwithout;
        }

        PropertyBag["gindex"] = gindex;
        PropertyBag["uindex"] = uindex;
        PropertyBag["gid"] = gid;
        PropertyBag["roles"] = Role.FindAll ();
        LayoutName = null;
    }

    // Cuando se carga la primera vez, o cuando no hay ningun grupo seleccionado
    public void UsersEdit (string gindex, string uindex)
    {
        Commons.CheckSuperUser(Session);
        this.UsersEdit(0, gindex, uindex);
    }

    public void UserSave (int id, string name, string password) //[ARDataBind ("user")] User user)
    {
        Commons.CheckSuperUser(Session);
        User user = User.Find(id);
        if  (user != null)
        {
            if (password != String.Empty)
                user.UserPassword = password;
            user.Name = name;
            user.Save ();
            Flash["aviso"] = "Guardado usuario";
            RedirectToAction ("usersedit");
        }
    }

    public void UserCreate ([DataBind ("user")] User user)
    {
        Commons.CheckSuperUser(Session);
        User u = new User(user.Name, user.UserPassword);
        u.Save();
        Flash["aviso"] = "Creado usuario";
        RedirectToAction ("usersedit");
    }

    public void UserDelete ([ARFetch ("Id", Create = false)] User user)
    {
        Commons.CheckSuperUser(Session);
        user.Delete ();
        Flash["aviso"] = "Borrado usuario";
        RedirectToAction ("usersedit");
    }

    // ACLS

    public void AclCreate (int GroupId, int RoleId)
    {
        Commons.CheckSuperUser(Session);
        Acl Form = new Acl(GroupId, RoleId);
        Form.Save ();
        Response.Redirect (Context.UrlReferrer);       // vuelve al sitio de partida
    }

    public void AclDelete (int GroupId, int RoleId)
    {
        Commons.CheckSuperUser(Session);
        Acl Form = Acl.FindByGroupRole(GroupId, RoleId);
        Form.Delete ();
        Flash["aviso"] = "Borrado grouppermission";
        Response.Redirect (Context.UrlReferrer);       // vuelve al sitio de partida
    }

/*#if OBSOLETE
    public void GroupPermissionSave ([DataBind ("Form")] GroupPermission Form, int GroupId, int ContainerId, int RoleId)
    {
        Form.Group = (GroupId != 0) ? Group.Find (GroupId) : null;
        Form.Container = (ContainerId != 0) ? Container.Find (ContainerId) : null;
        Form.Role = (RoleId != 0) ? Role.Find (RoleId) : null;
        Flash["aviso"] = "Guardado grouppermission";
        Form.Save ();
        Response.Redirect (Context.UrlReferrer);       // vuelve al sitio de partida
    }

    public void GroupPermissionCreate ([DataBind ("Form")] GroupPermission Form, int GroupId, int ContainerId, int RoleId)
    {
        Form.Group = (GroupId != 0) ? Group.Find (GroupId) : null;
        Form.Container = (ContainerId != 0) ? Container.Find (ContainerId) : null;
        Form.Role = (RoleId != 0) ? Role.Find (RoleId) : null;
        Form.Id = 0; // seguridad: obligamos creacion

        Flash["aviso"] = "Creado grouppermission";
        Form.Create ();
        Response.Redirect (Context.UrlReferrer);       // vuelve al sitio de partida
    }

    public void GroupPermissionDelete ([ARFetch ("Id", Create = false)] GroupPermission Form)
    {
        Form.Delete ();
        Flash["aviso"] = "Borrado grouppermission";
        Response.Redirect (Context.UrlReferrer);       // vuelve al sitio de partida
    }
#endif*/

}
}
