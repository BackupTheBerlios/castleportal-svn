// Authors:
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

using System;
using System.IO;
using System.Xml;
using System.Collections;
using Castle.ActiveRecord;
using NHibernate;

namespace CastlePortal
{


public class ConfigManager
{
    private static ConfigManager instance = null;
    private static Hashtable ConfigHash = Hashtable.Synchronized(new Hashtable());
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    private ConfigManager(){}

    public static ConfigManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ConfigManager();
        }
        return instance;
    }

    public ICollection Keys 
    {
        get { return ConfigHash.Keys; }
    }
    
    public void LoadFromFile(string fileName)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNodeList xmlNodeList = doc.GetElementsByTagName("Config");
            foreach (XmlNode node in xmlNodeList)
            {
                if ((node.Attributes["key"] != null) && (node.Attributes["value"] != null))
                    ConfigHash[node.Attributes["key"].Value] = node.Attributes["value"].Value;
                logger.Debug("reading node=" + node.Attributes["key"].Value + ":"+ node.Attributes["value"].Value);
            }
        }
        catch (Exception e)
        {
            logger.Error("Error while loading from file: " + e.Message);
            throw e;
        }
    }

    public string GetValue(string key)
    {
        if (ConfigHash.ContainsKey(key))
        {
            return (string)ConfigHash[key];
        }
        else
        {
            logger.Error("Requested key not found: " + key);
            throw new NullReferenceException();
        }
    }

    public string GetExistentValue(string key)
    {
        return (string)ConfigHash[key];
    }

/*    public string GetValue(ConfigEnum key)
    {
        return GetValue(key.ToString());
    }
*/


}
}
