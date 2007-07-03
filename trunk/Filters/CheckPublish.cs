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
using NLog;
using System.Collections;

namespace CastlePortal
{
public class CheckPublishFilter : Castle.MonoRail.Framework.IFilter
{
    static NLog.Logger logger = LogManager.GetCurrentClassLogger();

    public bool Perform(Castle.MonoRail.Framework.ExecuteEnum when, 
            Castle.MonoRail.Framework.IRailsEngineContext context, 
            Castle.MonoRail.Framework.Controller controller)
    {
        string pub;
        try
        {
            pub = ConfigModel.GetValue(Constants.PUBLISHED);
        }      
        catch (Castle.ActiveRecord.Framework.ActiveRecordException ex)
        {
            logger.Error("WEBSITE not published: " + ex.Message + " , " + ex.StackTrace);
            if (ex.InnerException != null)
                logger.Error("WEBSITE not published: " + ex.InnerException.Message + " , " + ex.InnerException.StackTrace);

            throw new UnPublished("");
        }
        
        if (pub == "0")
        {
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] currentDomain = baseDir.Split('/');
            string dir = currentDomain[currentDomain.Length - 1];

            string loggedCookie = context.Request.ReadCookie(Constants.LOGGED_COOKIE + dir);
            if ((loggedCookie == "false") || (loggedCookie == null) || (loggedCookie.Length == 0))
            {
                context.Session["FromUrl"] = context.Url;
                context.Response.Redirect(Constants.LOGIN_CONTROLLER, "index");
                return false;
            }
            else if ((context.Session.Count == 0) || (context.Session[Constants.USER] == null))	// TimeOut
            {
                throw new TimeOut("");
            }
            else
                return true;

        }
        else
        {
            if ((controller.Name != Constants.PORTAL) && (controller.Name != Constants.LOGIN_CONTROLLER))
                if ((!(bool)context.Session[Constants.IS_ADMIN]) && 
                        (!(bool)context.Session[Constants.IS_ROOT]))
                {
                    throw new Unauthorized("");
                }
            return true;
        }
    }
}
}
