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
public class CategoryTranslationController : ARSmartDispatcherController
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
        PropertyBag["categories"] = Category.FindByParent(-1);    // listado completo para los combos
        PropertyBag["parent"] = -1;
    }

    public void Admin (int parent)
    {
        Commons.CheckSuperUser(Session);
        PropertyBag["categories"] = Category.FindByParent(parent);      // listado completo para los combos
        PropertyBag["parent"] = parent;
    }

    public void AdminBlock(int parent)
    {
        Commons.CheckSuperUser(Session);

        Language[] langs = Language.FindAll();
        Category[] categories = Category.FindByParent(parent);

        ArrayList categoriesTranslations = new ArrayList();
        foreach (Category c in categories)
        {
            foreach (Language l in langs)
                categoriesTranslations.Add(CategoryTranslation.FindByCategoryAndLang(c, l));
        }

        PropertyBag["categories"] = categories;
        PropertyBag["translations"] = categoriesTranslations.ToArray(typeof(CategoryTranslation));
        PropertyBag["parent"] = parent;
        LayoutName = null;
    }

    public void Edit (int id, int parent)
    {
        Commons.CheckSuperUser(Session);
        if (id > 0)
        {
            Category category = Category.Find(id);
            CategoryTranslation[] translations = CategoryTranslation.FindByCategory(category);

            PropertyBag["category"] = category;
            PropertyBag["translations"] = translations;
            PropertyBag["languages"] = Language.FindAll();
        }
    }

    public void Save (int id, int idCategory, string translation)
    {
        Commons.CheckSuperUser(Session);

        Language[] langs = Language.FindAll();
        foreach (Language l in langs)
        {
            CategoryTranslation categoryTranslation = CategoryTranslation.FindByCategoryAndLang(Category.Find(idCategory), l);
            if (categoryTranslation == null)
                categoryTranslation = new CategoryTranslation();

            categoryTranslation.Lang = l;
            categoryTranslation.Translation = Request.Form[l.Id.ToString()];
            categoryTranslation.Category = Category.Find(idCategory);
            categoryTranslation.Save();
        }

        RedirectToAction("admin");
    }
}
}
