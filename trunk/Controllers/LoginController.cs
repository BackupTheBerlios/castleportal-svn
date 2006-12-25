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

using System;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;

namespace CastlePortal
{
[Layout ("login")]
[Resource( "l10n", "l10n" )]
[LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
[Helper (typeof (MenuHelper))]
[Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
[DefaultAction("Redir")]
public class LoginController: SmartDispatcherController
{
    public void Redir()
    {
        Response.Redirect(Constants.PORTAL_CONTROLLER , "index");
    }

    public void Index ()
    {
        Session[Constants.LAST_URL] = Context.UrlReferrer;
        // FIXME Hay que descomentar esto...
        //LayoutName = "login";
    }

    public void Portal ()
    {
        Session[Constants.LAST_URL] = Context.UrlReferrer;
        RenderView("index");
    }

    public void Login (string username, string password)
    {
        DateTime dt = DateTime.Now;
        User user = User.FindByUsernameAndPasswd (username, password);
        DateTime af = DateTime.Now;
        Console.WriteLine(dt + "," + af);
        if (user == null)
        {
            Flash["error"] = "No se ha podido autentificar, inténtelo de nuevo";
            RedirectToAction("index");
        }
        else
        {
            Flash["aviso"] = "Bienvenido al sistema";
            Response.CreateCookie("logged", "true");
            Session[Constants.USER] = user;
            Session[Constants.CATEGORIES_ACLS] = null;
            Session[Constants.MENUS_ACLS] = null;
            Session[Constants.ACCESIBLE] = null;
            /*if (Session[Constants.LAST_URL] != null)
            {
                string lastUrl = (string)Session[Constants.LAST_URL];
                if (lastUrl.IndexOf("login/index", 0, lastUrl.Length -1) == -1)
                    Response.Redirect((string)Session[Constants.LAST_URL]);
                else
                    Response.Redirect(Constants.PORTAL_CONTROLLER , "index");
                Session[Constants.LAST_URL] = null;
            }
            else
            {
            */    Response.Redirect(Constants.PORTAL_CONTROLLER , "index");
            //}
        }
    }

    public void Logout ()
    {
        if (Session[Constants.USER] != null)
        {
            Session[Constants.USER] = null;
            Flash["aviso"] = "Gracias por su visita";
        }
        else
        {
            Flash["aviso"] = "Ya está usted fuera del sistema.";
        }
        System.Console.WriteLine("Gracias por su visita");
        Response.RemoveCookie("logged");
        Response.CreateCookie("logged", "");
        Session[Constants.CATEGORIES_ACLS] = null;
        Session[Constants.MENUS_ACLS] = null;
//#if GPC

        Response.Redirect(Constants.LOGIN_CONTROLLER, "index");
//#else

//        Response.Redirect(Context.UrlReferrer);
//#endif

    }
}
}
