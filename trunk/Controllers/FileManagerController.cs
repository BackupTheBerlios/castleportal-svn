// Authors:
//    Bea <bgarci@shidix.com>
//    Hector Rojas <hectorrojas@shidix.com>
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

using System.Collections;
using System.IO;
using System.Web;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using NHibernate.Expression;

namespace CastlePortal
{
    [Layout ("general")]
    [Helper (typeof (MenuHelper))]
    [Helper (typeof (StringHelper))]
    [Helper (typeof (ForumHelper))]
    [Helper (typeof (FileManagerHelper))]
    [Rescue("generalerror")]
    [Resource( "l10n", "l10n" )]
    //[LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
    [Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
    [Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
    [DefaultAction("Redir")]
    //[Filter (ExecuteEnum.BeforeAction, typeof (CheckGroupFilter))]
    public class FileManagerController: ARSmartDispatcherController
    {
        static ConfigManager config = ConfigManager.GetInstance();

        private void CheckGroup(string directory)
        {
            LayoutName = null;
            if (directory.Length > 0)
            {
                string subdir = GetSubdirectory(directory);

                if ((subdir != null) && (subdir.Length > 0))
                {
                    User user = (Session.Contains("User")) ? (User) Session["User"] : null;
                    if (user != null)
                    {
                        bool userbelongstogroup = false;
                        if (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"]))
                            userbelongstogroup = true;
                        foreach (Group g in user.Groups)
                        {
                            if (g.Name.CompareTo(subdir) == 0)
                                userbelongstogroup = true;
                        }

                        if (userbelongstogroup == false)
                            throw new Unauthorized();
                    }
                    else
                        throw new Unauthorized();
                }
              //  else
              //      throw new Unauthorized();
             }
             else
                 throw new Unauthorized();

        }

        // Funcion para redirigir al index del portal en caso de que se haya escrito algun metodo inexistente 
        // del controlador
        public void Redir()
        {
            RedirectToAction("list");
        }
    
        // Funcion para listar el contenido de un directorio
        public void List()
        {
            string dirRoot = System.AppDomain.CurrentDomain.BaseDirectory;
            dirRoot = System.IO.Path.Combine(dirRoot, config.GetValue(Constants.PRIVATE_FOLDER));
            dirRoot = System.IO.Path.Combine(dirRoot, config.GetValue(Constants.FILE_FOLDER));

            CheckGroup(dirRoot);

//            if (System.IO.Directory.Exists(dirRoot) == false)
//                System.IO.Directory.CreateDirectory(dirRoot);

            Hashtable parameters = new Hashtable();
            parameters["parent"] = dirRoot;
            parameters["layout"] = true; // la primera vez que se llama queremos que muestre el layout
            RedirectToAction("showdirectorytree", parameters);
        }

        public void ShowDirectoryTree (string parent, bool layout)
        {
            CheckGroup(parent);
            string[] directories = System.IO.Directory.GetDirectories(parent);

            ArrayList dirs = new ArrayList();
            ArrayList subdirs = new ArrayList();
            string dirRoot = System.AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < directories.Length; i++)
            {
                dirs.Add(directories[i]);
                subdirs.Add(directories[i].Substring(dirRoot.Length, directories[i].Length - dirRoot.Length));
            }

            string[] filesaux = System.IO.Directory.GetFiles(parent);
 
            ArrayList filesfullpath = new ArrayList();
            ArrayList files = new ArrayList();
            for (int i = 0; i < filesaux.Length; i++)
            {
                filesfullpath.Add(filesaux[i]);
                files.Add(filesaux[i].Substring(dirRoot.Length, filesaux[i].Length - dirRoot.Length));
            }
           
            PropertyBag["parent"] = parent;
            PropertyBag["directories"] = dirs;
            PropertyBag["subdirs"] = subdirs;
            PropertyBag["files"] = filesfullpath;
            PropertyBag["filesrelativepath"] = files;

            if (!layout)
                LayoutName = null;
            else
                LayoutName = "layout_castleportal";
        }

        public void DeleteDirectory (string name)
        {
            CheckGroup(name);
            System.IO.Directory.Delete(name);
            RedirectToAction("list");
        }

        public void DeleteFile(string name)
        {
            CheckGroup(name);
            System.IO.File.Delete (name);
            RedirectToAction("list");
        }

        public void AddItem(string parent, string type)
        {
            CheckGroup(parent);
            PropertyBag["parent"] = parent;
            PropertyBag["type"] = type;
            LayoutName = "layout_castleportal";
        }

        public void Save(string parent, string kind, string directory)
        {
            CheckGroup(parent);
            string path;
            if (kind == "directory")
            {
                path = System.IO.Path.Combine (parent, directory);
                System.IO.Directory.CreateDirectory (path);
            }
            if (kind == "file")
            {
                foreach (string input in Request.Files.Keys)
                if (((System.Web.HttpPostedFile)Request.Files[input]).ContentLength != 0)
                {
                    string filename = ((System.Web.HttpPostedFile)Request.Files[input]).FileName;
                    path = System.IO.Path.Combine (parent, filename);
                    ((System.Web.HttpPostedFile)Request.Files[input]).SaveAs(path);
                }
            }

            RedirectToAction("list");
        }

        private string GetSubdirectory(string directory)
        {
            string dirRoot = System.AppDomain.CurrentDomain.BaseDirectory;
            dirRoot = System.IO.Path.Combine(dirRoot, config.GetValue(Constants.PRIVATE_FOLDER));
            dirRoot = System.IO.Path.Combine(dirRoot, config.GetValue(Constants.FILE_FOLDER));

            string subdir = directory.Substring(dirRoot.Length, directory.Length - dirRoot.Length);
            if (subdir.Length > 0)
                subdir = subdir.Split('/')[1];

            return subdir;
        }
    }
}

