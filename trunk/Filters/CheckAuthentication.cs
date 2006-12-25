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
namespace CastlePortal
{
public class CheckAuthenticationFilter : Castle.MonoRail.Framework.IFilter
{
    public bool Perform(Castle.MonoRail.Framework.ExecuteEnum when, 
            Castle.MonoRail.Framework.IRailsEngineContext context, 
            Castle.MonoRail.Framework.Controller controller)
    {
        context.CurrentUser = context.Session[Constants.USER] as System.Security.Principal.IPrincipal;

        if (context.CurrentUser == null || !context.CurrentUser.Identity.IsAuthenticated)
        {
            context.Session["FromUrl"] = context.Url;
            context.Response.Redirect(Constants.LOGIN_CONTROLLER, "index");
            return false;
        }
        else
        {
            return true;
        }
    }
}
}

