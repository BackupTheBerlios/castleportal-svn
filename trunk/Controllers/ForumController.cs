// Authors:
//    Héctor Rojas  <hectorrojas@shidix.com>
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
using System.Net;
using System.IO;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using Castle.MonoRail.ActiveRecordSupport;

namespace CastlePortal
{
    [Layout ("general")]
    [Helper (typeof (MenuHelper))]
    [Helper (typeof (StringHelper))]
    [Helper (typeof (ExtraHelper))]
    [Helper (typeof (ForumHelper))]
    [Rescue("generalerror")]
    [Resource( "l10n", "l10n" )]
    [LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
    [Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
    [Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
    [DefaultAction("Redir")]
    public class ForumController : ARSmartDispatcherController
    {
/*        private void CheckSuperUser()
        {
            LayoutName = null;
            if ((!(bool)Session["isAdmin"]) && (!(bool)Session["isRoot"]))
                throw new Unauthorized("");
        }
*/

        private void CheckAdmins(Forum forum)
        {
            LayoutName = null;
            if (forum != null)
            {
                User user = (Session.Contains("User")) ? (User) Session["User"] : null;
                if (user != null)
                {
                    bool userbelongstogroup = false;
                    if (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"]))
                        userbelongstogroup = true;
                    foreach (Group g in user.Groups)
                    {
                        if (g.Id == forum.Admins.Id)
                            userbelongstogroup = true;
                    }

                    if (userbelongstogroup == false)
                        throw new Unauthorized();
                }
                else
                    throw new Unauthorized();
            }
            else
                throw new Unauthorized();
        }

        private void CheckModerators(ForumFolder folder)
        {
            LayoutName = null;
            if (folder != null)
            {
                User user = (Session.Contains("User")) ? (User) Session["User"] : null;
                if (user != null)
                {
                    bool userbelongstogroup = false;
                    if (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"]))
                        userbelongstogroup = true;

                    foreach (Group g in user.Groups)
                    {
                        if ((folder.Forum.Admins != null) && (g.Id == folder.Forum.Admins.Id))
                            userbelongstogroup = true;
                    }

                    foreach (Group g in user.Groups)
                    {
                        if ((folder.Moderators != null) && (g.Id == folder.Moderators.Id))
                            userbelongstogroup = true;
                    }

                    if (userbelongstogroup == false)
                        throw new Unauthorized();
                }
                else
                    throw new Unauthorized();
            }
            else
                throw new Unauthorized();
        }

        private void CheckGroup(Forum forum)
        {
            LayoutName = null;
            if (forum != null)
            {
                User user = (Session.Contains("User")) ? (User) Session["User"] : null;
                if (user != null)
                {
                    bool userbelongstogroup = false;
                    if (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"]))
                        userbelongstogroup = true;
                    foreach (Group g in user.Groups)
                    {
                        if (g.Id == forum.ForumGroup.Id)
                            userbelongstogroup = true;
                    }

                    if (userbelongstogroup == false)
                        throw new Unauthorized();
                }
                else
                    throw new Unauthorized();
            }
            else
                throw new Unauthorized();
        }

        // Funcion para redirigir al index del portal en caso de que se haya escrito algun metodo inexistente 
        // del controlador
        public void Redir()
        {
            RedirectToAction("index");
        }

        public void Index()
        {
            Forum[] forums = Forum.FindAllOrderByDateTime();

            PropertyBag["forums"] = forums;
        }

        public void ViewForum(string Id, bool layout)
        {
            Forum forum = Forum.Find(Int32.Parse(Id.Split('.')[0]));

            CheckGroup(forum);

            PropertyBag["forum"] = forum;

            if (layout == true)
                LayoutName = "layout_gpc_course";
            else
                LayoutName = null;
        }

        public void ViewFolder([ARFetch("Id", Create = false)] ForumFolder folder, bool layout)
        {
            CheckGroup(GetForumBelongsFolder(folder));

            PropertyBag["folder"] = folder;

            if (layout == true)
                LayoutName = "layout_gpc_course";
            else
                LayoutName = null;
        }

        public void ViewThread([ARFetch("Id", Create = false)] ForumMessage message, bool layout)
        {
            CheckGroup(GetForumBelongsMessage(message));

            PropertyBag["message"] = message;

            if (layout == true)
                LayoutName = "layout_gpc_course";
            else
                LayoutName = null;
        }

        public void CreateForum()
        {
            Commons.CheckSuperUser(Session);

            PropertyBag["groups"] = Group.FindAll();
            PropertyBag["forum"] = new Forum();
            LayoutName = null;
        }

        public void SaveForum([DataBind("forum")] Forum forum)
        {
            Commons.CheckSuperUser(Session);

            forum.Date = System.DateTime.Now;
            forum.Save();

            RedirectToAction("index");
        }

        public void CreateFolder(int idForum, int idFolderParent)
        {
            if (idForum != 0)
                CheckAdmins(Forum.Find(idForum));
            else
                CheckAdmins(GetForumBelongsFolder(ForumFolder.Find(idFolderParent)));

            PropertyBag["groups"] = Group.FindAll();
            PropertyBag["idForum"] = idForum;
            PropertyBag["idFolderParent"] = idFolderParent;
            PropertyBag["forum"] = new ForumFolder();

            LayoutName = null;
        }

        public void SaveFolder([DataBind("folder")] ForumFolder folder, int idFolderParent, int idForum)
        {
            if (idForum != 0)
                CheckAdmins(Forum.Find(idForum));
            else
                CheckAdmins(GetForumBelongsFolder(ForumFolder.Find(idFolderParent)));

            folder.Date = System.DateTime.Now;

            if (idForum != 0)
            {
                Forum forum = Forum.Find(idForum);
                folder.Forum =  forum;
                forum.ForumFolders.Add(folder);
                folder.Save();
                forum.Save();
            }
            else
            {
                ForumFolder folderParent = ForumFolder.Find(idFolderParent);
                folder.Parent = folderParent;
                folderParent.FoldersChildren.Add(folder);

                foreach (ForumMessage m in folderParent.ForumMessages)
                    DeleteMessagesChildren(m);

                folder.Save();
                folderParent.Save();
            }

            if (idForum != 0)
            {
                Hashtable parameters = new Hashtable();
                parameters["Id"] = folder.Forum.Id;
                parameters["layout"] = false;
                RedirectToAction("viewforum", parameters);
            }
            else
            {
                Hashtable parameters = new Hashtable();
                parameters["Id"] = folder.Parent.Forum.Id;
                parameters["layout"] = false;
                RedirectToAction("viewforum", parameters);
            }
        }

        public void CreateMessage(int idFolderParent, int idMessageParent)
        {
            if (idFolderParent != 0)
                CheckGroup(GetForumBelongsFolder(ForumFolder.Find(idFolderParent)));
            else
                CheckGroup(GetForumBelongsMessage(ForumMessage.Find(idMessageParent)));

            if (idFolderParent != 0) // New thread
            {
                ForumFolder folder = ForumFolder.Find(idFolderParent);

                if (folder.FoldersChildren.Count == 0)
                {
                    PropertyBag["idFolderParent"] = idFolderParent;
                    PropertyBag["idMessageParent"] = idMessageParent;
                }
                else // Can't be msg and subfolder in same folder
                {
                    Hashtable parameters = new Hashtable();
                    parameters["Id"] = folder.Forum.Id;
                    RedirectToAction("viewforum", parameters);
                }
            }
            else // Reply to message
            {
                    PropertyBag["idFolderParent"] = idFolderParent;
                    PropertyBag["idMessageParent"] = idMessageParent;
            }

            PropertyBag["message"] = new ForumMessage();
            LayoutName = null;
        }

        public void SaveMessage([DataBind("message")] ForumMessage message,
            int idFolderParent, int idMessageParent)
        {
            if (idFolderParent != 0)
                CheckGroup(GetForumBelongsFolder(ForumFolder.Find(idFolderParent)));
            else
                CheckGroup(GetForumBelongsMessage(ForumMessage.Find(idMessageParent)));

            User user = (User)Context.Session["User"];
            if (user != null)
            {
                message.Date = System.DateTime.Now;
                message.Owner = user;

                if (idFolderParent != 0)
                {
                    ForumFolder folder = ForumFolder.Find(idFolderParent);
                    message.ForumFolder = folder;
                    message.Level = 1;
                    folder.ForumMessages.Add(message);
                    message.Save();
                    folder.Save();
                }
                else
                {
                    ForumMessage messageParent = ForumMessage.Find(idMessageParent);

                    message.Parent = messageParent;
                     message.Level = message.Parent.Level + 1;
                    messageParent.MessagesChildren.Add(message);
                    message.Save();
                    messageParent.Save();
                }

                if (idFolderParent != 0)
                {
                    Hashtable parameters = new Hashtable();
                    parameters["Id"] = message.Id;
                    parameters["layout"] = false;
                    RedirectToAction("viewthread", parameters);
                }
                else
                {
                    while (message.Parent != null)
                        message = message.Parent;
    
                    Hashtable parameters = new Hashtable();
                    parameters["Id"] = message.Id;
                    parameters["layout"] = false;
                    RedirectToAction("viewthread", parameters);
                }
            }
        }

        public void EditForum([ARFetch("Id", Create = false)] Forum forum)
        {
            Commons.CheckSuperUser(Session);

            PropertyBag["groups"] = Group.FindAll();
            PropertyBag["forum"] = forum;
            LayoutName = null;
        }

        public void UpdateForum([ARDataBind("forum", AutoLoadBehavior.Always)] Forum forum)
        {
            Commons.CheckSuperUser(Session);

            forum.Save();
            RedirectToAction("index");
        }

        public void EditFolder([ARFetch("Id", Create = false)] ForumFolder folder)
        {
            CheckAdmins(GetForumBelongsFolder(folder));

            PropertyBag["groups"] = Group.FindAll();
            PropertyBag["folder"] = folder;
            LayoutName = null;
        }

        public void UpdateFolder([ARDataBind("folder", AutoLoadBehavior.Always)] ForumFolder folder)
        {
            CheckAdmins(folder.Forum);

            folder.Save();

            Hashtable parameters = new Hashtable();
            parameters["Id"] = GetForumBelongsFolder(folder).Id;
            parameters["layout"] = false;
            RedirectToAction("viewforum", parameters);
        }

        public void EditMessage([ARFetch("Id", Create = false)] ForumMessage message)
        {
            User user = (User)Context.Session["User"];
            if ((user.Id == message.Owner.Id) ||
                (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"])))
            { // Only user that create message or root can modify it
                if (message.MessagesChildren.Count > 0)
                { // Only update (add text)
                    PropertyBag["idMessage"] = message.Id;
                    PropertyBag["title"] = message.Title;
                    PropertyBag["body"] = message.Body;
                }
                else
                    PropertyBag["message"] = message;
            }
            else
            {
                throw new Unauthorized();
/*                while (message.Parent != null)
                    message = message.Parent;
                        
                Response.Redirect("viewthread.html?Id=" + message.Id);*/
            }

            LayoutName = null;
        }

        public void SaveEditedMessage([ARDataBind("message", AutoLoadBehavior.Always)] ForumMessage message)
        {
            User user = (User)Context.Session["User"];
            if ((user.Id == message.Owner.Id) ||
                (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"])))
            {
                message.Save();

                while (message.Parent != null)
                    message = message.Parent;
            }
            else
                throw new Unauthorized();
    
            Hashtable parameters = new Hashtable();
            parameters["Id"] = message.Id;
            parameters["layout"] = false;
            RedirectToAction("viewthread", parameters);
        }

        public void SaveUpdatedMessage([ARDataBind("message", AutoLoadBehavior.Always)] ForumMessage message)
        {
            User user = (User)Context.Session["User"];
            if ((user.Id == message.Owner.Id) ||
                (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"])))
            {
// FIXME Add text a body                message.Body = message.Body + text;
                message.Save();

                while (message.Parent != null)
                    message = message.Parent;
    
                Hashtable parameters = new Hashtable();
                parameters["Id"] = message.Id;
                parameters["layout"] = false;
                RedirectToAction("viewthread", parameters);
            }
            else
                throw new Unauthorized();
        }

        public void DeleteForum([ARFetch("Id", Create = false)] Forum forum)
        {
            Commons.CheckSuperUser(Session);
//FIXME IF CACHE
            foreach (ForumFolder folder in forum.ForumFolders)
                DeleteFolderAndMessages(folder);

            forum.Delete();
            RedirectToAction("index");
            LayoutName = null;
        }

        public void DeleteFolder([ARFetch("Id", Create = false)] ForumFolder folder)
        {
            CheckAdmins(folder.Forum);
            Forum forum = GetForumBelongsFolder(folder);
//FIXME IF CACHE
            DeleteFolderAndMessages(folder);

            Hashtable parameters = new Hashtable();
            parameters["Id"] = forum.Id;
            RedirectToAction("viewforum", parameters);
            LayoutName = null;
        }

        public void DeleteMessage([ARFetch("Id", Create = false)] ForumMessage message)
        {
            ForumFolder folder = GetFolderBelongsMessage(message);
            CheckModerators(folder);
//FIXME IF CACHE
//            DeleteMessagesChildren(message);
            foreach (ForumMessage m in message.MessagesChildren)
            {
                if (message.Parent != null)
                    m.Parent = message.Parent;
                else
                    m.ForumFolder = message.ForumFolder;
                m.Save();
            }
            message.MessagesChildren.Clear();
            message.Save();

            if (message.Parent != null)
            {
                ForumMessage parent = message.Parent;
                parent.MessagesChildren.Remove(message);
                message.Delete();
                parent.Save();

                Hashtable parameters = new Hashtable();
                parameters["Id"] = parent.Id;
                parameters["layout"] = null;
                RedirectToAction("viewthread", parameters);
            }
            else
            {
                folder.ForumMessages.Remove(message);
                message.Delete();
                folder.Save();

                Hashtable parameters = new Hashtable();
                parameters["Id"] = folder.Id;
                parameters["layout"] = null;
                RedirectToAction("viewfolder", parameters);
            }

            LayoutName = null;
        }

        private void DeleteFolderAndMessages(ForumFolder folder)
        {
            Forum forum = GetForumBelongsFolder(folder);
            if (folder.FoldersChildren.Count > 0)
            {
                foreach (ForumFolder f in folder.FoldersChildren)
                    f.Delete();
                folder.FoldersChildren.Clear();
                folder.Save();
            }
            else
            {
                foreach (ForumMessage message in folder.ForumMessages)
                    DeleteMessagesChildren(message);
                folder.ForumMessages.Clear();
                folder.Save();
            }

            if (folder.Parent == null)
            {
                forum.ForumFolders.Remove(folder);
                folder.Delete();
                forum.Save();
            }
            else
            {
                ForumFolder parent = folder.Parent;
                parent.FoldersChildren.Remove(folder);
                folder.Delete();
                parent.Save();
            }
        }

        private void DeleteMessagesChildren(ForumMessage message)
        {
            if (message.MessagesChildren.Count != 0)
            {
                foreach (ForumMessage m in message.MessagesChildren)
                    DeleteMessagesChildren(m);
                message.MessagesChildren.Clear();
                message.Save();
//FIXME IF CACHE
            }
            else
            {
                message.Delete();
            }
        }

        private Forum GetForumBelongsFolder(ForumFolder folder)
        {
            if (folder.Forum != null)
                return folder.Forum;
            else
                return folder.Parent.Forum;
        }

        private ForumFolder GetFolderBelongsMessage(ForumMessage message)
        {
            while (message.ForumFolder == null)
                message = message.Parent;

            return message.ForumFolder;
        }

        private Forum GetForumBelongsMessage(ForumMessage message)
        {
            while (message.ForumFolder == null)
                message = message.Parent;

            return GetForumBelongsFolder(message.ForumFolder);
        }
    }
}

