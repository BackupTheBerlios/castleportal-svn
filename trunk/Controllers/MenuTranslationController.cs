// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Alberto Morales <amd77@gulic.org>
//    Hector Rojas Gonzalez <hectorrojas@shidix.com>
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
public class MenuTranslationController : ARSmartDispatcherController
{
//    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
//    static ConfigManager config = ConfigManager.GetInstance();

/*    private void RefreshMenusAcl()
    {
        Hashtable categoriesHash = Session[Constants.CATEGORIES_ACLS] as Hashtable;
        if (categoriesHash == null)
        {
            logger.Error("Danger, Session[Constants.CATEGORIES_ACLS] is null or not hashtable");
        }
        Session[Constants.MENUS_ACLS] = Menu.GetHashByCategoryHash(categoriesHash);
    }
*/ 
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

        Language[] langs = Language.FindAll();
        Menu[] menus = Menu.FindByParent(parent);

        ArrayList menusTranslations = new ArrayList();
        foreach (Menu m in menus)
        {
            foreach (Language l in langs)
                menusTranslations.Add(MenuTranslation.FindByMenuAndLang(m, l));
        }

        PropertyBag["menus"] = menus;
        PropertyBag["translations"] = menusTranslations.ToArray(typeof(MenuTranslation));
        PropertyBag["parent"] = parent;
        LayoutName = null;
    }

    public void Edit (int id, int parent)
    {
        Commons.CheckSuperUser(Session);
        if (id > 0)
        {
            Menu menu = Menu.Find(id);
            MenuTranslation[] translations = MenuTranslation.FindByMenu(menu);

            PropertyBag["menu"] = menu;
            PropertyBag["translations"] = translations;
            PropertyBag["languages"] = Language.FindAll();
        }
    }

    public void Save (int id, int idMenu, string translation)
    {
        Commons.CheckSuperUser(Session);

        Language[] langs = Language.FindAll();
        foreach (Language l in langs)
        {
            MenuTranslation menuTranslation = MenuTranslation.FindByMenuAndLang(Menu.Find(idMenu), l);
            if (menuTranslation == null)
                menuTranslation = new MenuTranslation();

            menuTranslation.Lang = l;
            menuTranslation.Translation = Request.Form[l.Id.ToString()];
            menuTranslation.Menu = Menu.Find(idMenu);
            menuTranslation.Save();
        }

        RedirectToAction("admin");
    }
}
}
