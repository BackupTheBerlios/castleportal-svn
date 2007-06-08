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

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Configuration;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;

namespace CastlePortal
{

public class HttpApp : System.Web.HttpApplication
{
#if DEBUG
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
#endif


    static ConfigManager config = ConfigManager.GetInstance();

    public HttpApp()
    {
#if NHLOGS
        log4net.Config.XmlConfigurator.Configure();
#endif
    }


    private static bool dbExist = false;        // FIXME!!!: Synchronize parallel access to dbExist

    protected void Application_Start(Object sender, EventArgs e)
    {
#if dotNet2
        IConfigurationSource source = 
		ConfigurationManager.GetSection("activerecord") as IConfigurationSource;
#else
        IConfigurationSource source = 
		ConfigurationSettings.GetConfig("activerecord") as IConfigurationSource;
#endif

        ActiveRecordStarter.Initialize( source , 
            typeof(Acl) , 
            typeof(Category) , 
            typeof(Chat) , 
            typeof(ChatMessage) , 
            typeof(ConfigCombo) , 
            typeof(ConfigModel) , 
            typeof(Container) , 
            typeof(Content) , 
            typeof(DataModel) , 
            //typeof(FieldComparer) , 
            typeof(Field) , 
            typeof(FieldTemplate) , 
            typeof(File) ,
            typeof(Forum) ,
            typeof(ForumFolder) ,
            typeof(ForumMessage) ,
            typeof(Group) , 
            typeof(Language),
            typeof(Menu) , 
            typeof(MenuTranslation),
            typeof(Role) , 
            typeof(Schedule) , 
            typeof(ScheduledEvent) , 
            typeof(Template) , 
            typeof(Type) , 
            typeof(TypeTranslation),
            typeof(User)
        );

        try
        {
            lock(this)
            {
                ConfigModel.GetValue("published");
                dbExist = true;
                config.LoadFromFile(
                Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "site.config"));
                logger.Debug("config loaded");
            }
        }
        catch (Castle.ActiveRecord.Framework.ActiveRecordException)
        {
            logger.Debug("Database connection problems, published key not found");
            dbExist = false;
        }
    }

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
#if !PREVIOUS
        HttpContext.Current.Items.Add( "nh.sessionscope", new SessionScope() );
#endif
        try
        {
            if (!dbExist)
            {
                logger.Error("dbExist = false !!!!!!!!!!!!!!!!!!!!!!!");
                string path = Server.MapPath("/");
                if (CastlePortal.SchemaGenerator.Crea(path))
                    dbExist = true;
                else
                    logger.Error("Fatal: SchemaGenerator could not create database");
                config.LoadFromFile(
                    Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "site.config"));
            }
        }
        catch (Exception ex)
        {
            logger.Error("Error on BeginRequest:" + ex.Message + "," + ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.Error("InnerEx:" + ex.InnerException.Message + ex.InnerException.StackTrace);
            }
        }
    }

    protected void Application_EndRequest(Object sender, EventArgs e)
    {
#if !PREVIOUS
        try
        {
            SessionScope scope = HttpContext.Current.Items["nh.sessionscope"] as SessionScope;
            if (scope != null)
            {
                scope.Dispose();
            }
        }
        catch(Exception ex)
        {
            HttpContext.Current.Trace.Warn( "Error", "EndRequest: " + ex.Message, ex );
        }
#endif
    }

    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
    }

    protected void Application_Error(Object sender, EventArgs e)
    {
    }

    protected void Session_End(Object sender, EventArgs e)
    {
    }

    protected void Application_End(Object sender, EventArgs e)
    {
    }
}
}

