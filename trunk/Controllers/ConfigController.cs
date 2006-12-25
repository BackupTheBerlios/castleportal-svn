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
using System.Collections;
using System;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using NHibernate.Expression;

namespace CastlePortal
{
[Helper (typeof (MenuHelper))]
[Layout ("general")]
[Rescue("generalerror")]
[Resource( "l10n", "l10n" )]
[LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
[Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
[Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
[DefaultAction("Redir")]
public class ConfigController:ARSmartDispatcherController       // .SmartDispatcherController
{
    public void Redir()
    {
        Response.Redirect("portal" , "index");
    }

    public void index ()
    {
        PropertyBag["configs"] = ConfigModel.FindAll();
        PropertyBag["layouts"] = ConfigCombo.FindAllByKey("layout");
        PropertyBag["styles"] = ConfigCombo.FindAllByKey("style");
        PropertyBag["headers"] = ConfigCombo.FindAllByKey("header");
    }

    public void theme (string layout, string color, string header)
    {
        /*if (layout == null || color == null || header == null ||
                ConfigModel.FindByKey("published").Val == "1")
        {
            RedirectToAction("index");
        }
        else
        {*/
            ConfigModel c;
            c = ConfigModel.FindByKey(Constants.LAYOUT_LOWER);
            c.Val = layout;
            c.Save();

            c = ConfigModel.FindByKey(Constants.HEADER);
            c.Val = header;
            c.Save();

            c = ConfigModel.FindByKey(Constants.COLOR);
            c.Val = color;
            c.Save();

            Response.Redirect(Context.UrlReferrer);
        //}
        LayoutName = null;
    }

    public void activate(string confirm)
    {
        Commons.CheckSuperUser(Session);
        if (confirm == "yes")
        {
            ConfigModel c;
            c = ConfigModel.FindByKey("published");
            c.Val = "1";
            c.Save();
            Response.Redirect((string)Session["lasturl"]);
            Session["lasturl"] = null;
        }
        else
        {
            Session["lasturl"] = Context.UrlReferrer;
        }
    }

    public void unactivate()
    {
        Commons.CheckSuperUser(Session);
        //if (confirm == "yes")
        //{
            ConfigModel c;
            c = ConfigModel.FindByKey("published");
            c.Val = "0";
            c.Save();
            Response.Redirect((string)Session["lasturl"]);
            Session["lasturl"] = null;
        /*}
        else
        {
            Session["lasturl"] = Context.UrlReferrer;
        }*/
        Response.Redirect(Constants.PORTAL_CONTROLLER, Constants.INDEX);
    }

    public void Save()
    {
        Commons.CheckSuperUser(Session);
        if (Request.Form.Count > 0)
        {
            bool published = (ConfigModel.FindByKey("published").Val == "1");
            foreach (string key in Request.Form)
            {
                string val = Request.Form[key];
                if (published && (key=="layout" ||
                                  key=="color" || key=="cabecera" ||
                                  key=="published" ))
                    continue;

                ConfigModel c = ConfigModel.FindByKey(key);
                if (c != null)
                {
                    c.Val = val;
                    c.Save();
                }
            }
        }
        RedirectToAction("index");
    }
}
}
