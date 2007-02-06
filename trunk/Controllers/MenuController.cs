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

using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using NHibernate.Expression;
using System;

namespace CastlePortal
{
[Layout ("general")]
[Rescue("generalerror")]
[Helper (typeof (MenuHelper))]
[Resource( "l10n", "l10n" )]
[LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
[Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
[Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
[DefaultAction("Redir")]
public class MenuController:Castle.MonoRail.ActiveRecordSupport.ARSmartDispatcherController
{
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    static ConfigManager config = ConfigManager.GetInstance();

    private void RefreshMenusAcl()
    {
        Hashtable categoriesHash = Session[Constants.CATEGORIES_ACLS] as Hashtable;
        if (categoriesHash == null)
        {
            logger.Error("Danger, Session[Constants.CATEGORIES_ACLS] is null or not hashtable");
        }
        Session[Constants.MENUS_ACLS] = Menu.GetHashByCategoryHash(categoriesHash);
    }
    
    public void Redir()
    {
        RedirectToAction("index");
    }

    public void Admin ()
    {
        Commons.CheckSuperUser(Session);
        Admin(0);
        PropertyBag["menus"] = Menu.FindByParent(-1);    // listado completo para los combos
        PropertyBag["parent"] = -1;
    }

    public void Admin (int parent)
    {
        Commons.CheckSuperUser(Session);
        PropertyBag["menus"] = Menu.FindByParent(parent);      // listado completo para los combos
        PropertyBag["parent"] = parent;
    }

    public void AdminBlock(int parent)
    {
        Commons.CheckSuperUser(Session);
        PropertyBag["menus"] = Menu.FindByParent(parent);
        PropertyBag["parent"] = parent;
        LayoutName = null;
    }

    public void Edit (int id, int parent)
    {
        Commons.CheckSuperUser(Session);
        if (id > 0)
        {
            Menu menu = Menu.Find(id);
            if (parent == 0)
            {
                if (menu == null)
                    menu = new Menu();
                PropertyBag["menuitem"] = menu;
                PropertyBag["parent"] = menu.Parent;
                PropertyBag["categories"] = Category.FindAll ();
            }
            else
            {
                PropertyBag["parent"] = menu;
                PropertyBag["categories"] = Category.FindAll ();
            }
        }
    }

    public void Save (int id, string name, string desc, 
            int ordering, int parentId, int categoryId, string newUrl)
    {
        Commons.CheckSuperUser(Session);
        Menu menu;
        if (id != 0)
            menu = Menu.Find(id);
        else
            menu = new Menu();
        menu.Name = name;
        menu.Url = newUrl;
        menu.Description = desc;
        menu.Ordering = ordering;
        menu.CategoryId = categoryId; 
        menu.Parent = null;
        if (parentId != 0)
        {
            menu.Parent = Menu.Find(parentId);
        }
#if CACHE
        if (menu.Parent != null)
        {
            bool contained = false;
            foreach (Menu men in menu.Parent.Children)
            {
                if (men.Id == menu.Id)
                {
                    contained = true;
                    break;
                }
            }
            if (!contained)
            {
                menu.Parent.Children.Add(menu);
                menu.Parent.Save();
            }
        }
#endif
        menu.Save();
        RefreshMenusAcl();
        RedirectToAction("admin");
    }

    public void Delete ([Castle.MonoRail.ActiveRecordSupport.ARFetch ("Id", Create = false)] Menu menu)
    {
        Commons.CheckSuperUser(Session);
        if (menu != null)
        {
#if CACHE
            if (menu.Parent != null)
            {
                bool contained = false;
                foreach (Menu mnu in menu.Parent.Children)
                {
                    if (mnu.Id == menu.Id)
                    {
                        contained = true;
                        menu.Parent.Children.Remove(mnu);
                        break;
                    }
                }
                if (contained)
                {
                    menu.Parent.Save();
                }
            }
#endif
            menu.Delete ();
        }
        RefreshMenusAcl();
        RedirectToAction("admin");
    }

    public void DisableMenu (int id)
    {
        Commons.CheckSuperUser(Session);
        Menu[] childs = Menu.FindByParent(id);
        foreach (Menu child in childs)
        if (child.Show == 1)
        {
            child.Show = 0;
            child.Save();
            DisableMenu(child.Id);
        }
    }

    public void DisableAllMenu (ref Menu[] menus)
    {
        Commons.CheckSuperUser(Session);
        foreach (Menu m in menus)
        if (m.Show == 1)
        {
            m.Show = 0;
            m.Save();
        }
    }


    public void Red (int id)
    {
        Commons.CheckSuperUser(Session);
        Menu[] menus = Menu.FindAll();
        Menu menu = Menu.Find(id);
        //DisableMenu (menu.Parent.Id);
        DisableAllMenu(ref menus);
        menu.Show = 1;
        menu.Save();

        if (menu.CategoryId != 0)
        {
            //Response.Redirect ("../portal/viewcategory.html?Id="+menu.CategoryId);
            Response.Redirect ("../"+ Constants.VIEW_PORTAL_CATEGORY + "." + 
                    config.GetValue(Constants.EXTENSION) + "?Id=" + menu.CategoryId);
        }
        else
            if (menu.Url != "")
                Response.Redirect (menu.Url);
            else
                Response.Redirect ("users", "index");
    }

    public void Submenu (int parent)
    {
        Commons.CheckSuperUser(Session);
        Menu [] menus = Menu.FindByParent(parent);
        PropertyBag["submenus"] = menus;
        LayoutName = null;
    }
}
}
