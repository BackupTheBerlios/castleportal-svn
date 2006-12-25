// Authors:
//    Alberto Morales <amd77@gulic.org>
//    Carlos Ble <carlosble@shidix.com>
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
using NLog;

namespace CastlePortal
{
public class ConfigLoadFilter : Castle.MonoRail.Framework.IFilter
{
    static NLog.Logger logger = LogManager.GetCurrentClassLogger();
    static ConfigManager config = ConfigManager.GetInstance();

    public bool Perform(Castle.MonoRail.Framework.ExecuteEnum when,
                        Castle.MonoRail.Framework.IRailsEngineContext context,
                        Castle.MonoRail.Framework.Controller controller)
    {
        try
        {
            User user = null;
            if (context.Session.Contains(Constants.USER))
            {
                user = (User) context.Session[Constants.USER];
            }
            if (context.Session[Constants.CATEGORIES_ACLS] == null)
            {
                Hashtable aclcat = Category.GetHashesByUser(user);
                Hashtable aclmenu = Menu.GetHashByCategoryHash(aclcat);
                context.Session[Constants.CATEGORIES_ACLS] = aclcat;
                context.Session[Constants.MENUS_ACLS] = aclmenu;
            }

            // Get config parameters from database
            foreach (ConfigModel configModel in ConfigModel.FindAll())
            {
                if (configModel.Key == Constants.LAYOUT_LOWER)
                {
                    if (controller.Name == Constants.LOGIN_CONTROLLER)
                        controller.LayoutName = configModel.Val/*.Substring(0,6)*/ + Constants.LOGIN_SUB;
                    else
                        controller.LayoutName = configModel.Val;
                }
                else
                    controller.PropertyBag[configModel.Key] = configModel.Val;
            }

            // Get config parameters from site.config
            foreach (string key in config.Keys)
            {
                controller.PropertyBag[key] = config.GetExistentValue(key);
            }
            
            // Get config parameters from cookies:
            string lang = Commons.GetCurrentLang(controller);
            controller.PropertyBag[Constants.LANG] = lang ;

            // Get all supported languages:
            Language[] all_linguas = Language.FindAll();
            controller.PropertyBag[Constants.ALL_LINGUAS] = all_linguas;
            
            
            controller.PropertyBag[Constants.LAYOUTS_AVAILABLE] =
                ConfigCombo.FindAllByKey(Constants.LAYOUT_LOWER);
            controller.PropertyBag[Constants.COLORS_AVAILABLE] =
                ConfigCombo.FindAllByKey(Constants.COLOR);
            controller.PropertyBag[Constants.HEADERS_AVAILABLE] =
                ConfigCombo.FindAllByKey(Constants.HEADER);

            controller.PropertyBag[Constants.PERMISSION_READ] = Permission.Read;
            controller.PropertyBag[Constants.PERMISSION_CREATE] = Permission.Create;
            controller.PropertyBag[Constants.PERMISSION_MODIFY] = Permission.Modify;
            controller.PropertyBag[Constants.PERMISSION_DELETE] = Permission.Delete;
            controller.PropertyBag[Constants.PERMISSION_PUBLISH] = Permission.Publish;

            context.Session[Constants.IS_ROOT] =
                ((user != null) && (user.Name == Constants.SUPER_USER));
            context.Session[Constants.IS_ADMIN] =
                ((user  != null) && user.IsInGroup(Constants.ADMIN_GROUP_NAME));

            return true;
        }
        catch (Castle.ActiveRecord.Framework.ActiveRecordException ex)
        {
            logger.Error(ex.Message + "," + ex.StackTrace);
            controller.RenderText(Constants.WEBSITE_UNAVAILABLE);
            return false;
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }
}
}
