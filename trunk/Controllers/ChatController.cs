// Authors:
//    Héctor Rojas  <hectorrojas@shidix.com>
//    Carlos Ble  <carlosble@shidix.com>
//    Alberto Morales <amd77@shidix.com>
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
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using Castle.MonoRail.ActiveRecordSupport;
using System.Globalization;
using System.Timers;

namespace CastlePortal
{
    [Layout ("general")]
    [Helper (typeof (MenuHelper))]
    [Helper (typeof (StringHelper))]
    [Helper (typeof (ExtraHelper))]
    [Helper (typeof (ChatHelper))]
    [Rescue("generalerror")]
    [Resource( "l10n", "l10n" )]
    [LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
    [Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
    [Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
    [DefaultAction("Redir")]
    public class ChatController : ARSmartDispatcherController
    {
        // Funcion para redirigir al index del portal en caso de que se haya escrito algun metodo inexistente 
        // del controlador
        public void Redir()
        {
            RedirectToAction("index");
        }

        static Hashtable OnlinePeople = new Hashtable();    // Usuarios conectados, indexados por id
        static ChatsCache chatsCache = new ChatsCache();
        static Hashtable newMessage = new Hashtable();
        static Timer timerDrop = new Timer();
        static Timer timerClose = new Timer();

        private void CheckSuperUser()
        {
            LayoutName = null;
            if ((!(bool)Session["isAdmin"]) && (!(bool)Session["isRoot"]))
                throw new Unauthorized("");
        }

        private void CheckGroup(Chat chat)
        {
            LayoutName = null;
            if (chat != null)
            {
                User user = (Session.Contains("User")) ? (User) Session["User"] : null;
                if (user != null)
                {
                    bool userbelongstogroup = false;
                    if (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"]))
                        userbelongstogroup = true;
                    foreach (Group g in user.Groups)
                    {
                        if (g.Id == chat.OGroup.Id)
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

        private User GetUser()
        {
            // funcion privada copiada de UsersController
            // (algun dia habra tiempo de poner esto en un "algo" comun.
            if (Session.Contains("user") && Session["user"] != null)
                return (User) Session["user"];
            else
                return null;
        }

        public void Index()
        {
            User user = GetUser();
            if (user != null)
            {
                Chat[] chats = Chat.FindAll();
                if (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"]))
                    PropertyBag["chats"] = chats;
                else
                {
                    ArrayList userChats = new ArrayList();
                    foreach (Chat c in chats)
                    {
                        foreach (Group g in user.Groups)
                        {
                            if (c.OGroup.Id == g.Id)
                                userChats.Add(c);
                        }
                    }
                    PropertyBag["chats"] = userChats.ToArray(typeof(Chat));
                }

                PropertyBag["user"] = user;
                PropertyBag["people"] = OnlinePeople;
            }
        }
 
        public void Index(bool layout)
        {
            User user = GetUser();
            if (user != null)
            {
                Chat[] chats = Chat.FindAll();
                if (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"]))
                    PropertyBag["chats"] = chats;
                else
                {
                    ArrayList userChats = new ArrayList();
                    foreach (Chat c in chats)
                    {
                        foreach (Group g in user.Groups)
                        {
                            if (c.OGroup.Id == g.Id)
                                userChats.Add(c);
                        }
                    }
                    PropertyBag["chats"] = userChats.ToArray(typeof(Chat));
                }

                PropertyBag["user"] = user;
                PropertyBag["people"] = OnlinePeople;
            }

            if (layout == false)
                LayoutName = null;
        }

        public void New()
        {
            CheckSuperUser();
            PropertyBag["groups"] = Group.FindAll();
            PropertyBag["chat"] = new Chat();
        }

        public void Create([DataBind("chat")] Chat chat)
        {
            CheckSuperUser();
            chat.Save();

            Hashtable parameters = new Hashtable();
            parameters["layout"] = false;
				RedirectToAction("index", parameters);
        }

        public void Delete([ARFetch("Id", Create = false)] Chat chat)
        {
            CheckSuperUser();
            chat.Delete();

            Hashtable parameters = new Hashtable();
            parameters["layout"] = false;
				RedirectToAction("index", parameters);
        }
       
        public void Join([ARFetch("Cid",Create=false)] Chat chat)
        {
            CheckGroup(chat);
            User user = GetUser();
            if ((user != null) && (chat != null))
            {
                if (!OnlinePeople.ContainsKey(chat.Id))
                    OnlinePeople[chat.Id] = new ArrayList();
                if (!((ArrayList)(OnlinePeople[chat.Id])).Contains(user))
                    ((ArrayList)(OnlinePeople[chat.Id])).Add(user);
                Hashtable parameters = new Hashtable();
                parameters["cid"] = chat.Id;
                RedirectToAction("room", parameters);
            }
        }
        
        public void Room([ARFetch("Cid",Create=false)] Chat chat)
        {
            CheckGroup(chat);
            ArrayList messages = new ArrayList();
            User user = GetUser();

            newMessage[user.Id] = 0;
            if (chatsCache.chatsCache[chat.Id] == null) // ChatCache not exist. Create it
            {
                chatsCache.chatsCache[chat.Id] = chatsCache.CreateChatCache();
                CreateTimers();
            }

            if ((user != null) && (chat != null))
            {
                PropertyBag["user"] = user;
                PropertyBag["chat"] = chat;
                PropertyBag["group"] = chat.OGroup;
                PropertyBag["people"] = OnlinePeople[chat.Id];

                messages.AddRange(ChatMessage.FindLastByMinutes(chat.Id, 60));

                if (((ChatsCache.ChatCache)chatsCache.chatsCache[chat.Id]).numMsg != 0) // There are messages in cache
                    messages.AddRange(CopyMessagesCache(chat.Id, 0));

                PropertyBag["messages"] = messages;

                if (messages.Count != 0)
                {
                    object x = messages[messages.Count - 1];

                    if (x.GetType() == System.Type.GetType(Constants.NAMESPACE + ".ChatMessage"))
                        PropertyBag["idLastMsg"] = ((ChatMessage)messages[messages.Count - 1]).Id;
                    else
                    {
                        PropertyBag["idLastMsg"] = ((ChatsCache.MessageItem)messages[messages.Count - 1]).Id;
                    }
                }
                else
                    PropertyBag["idLastMsg"] = -1;
            }

            LayoutName = null;
        }

        public void GetMessages(int idchat, int idmessage)
        {
            CheckGroup(Chat.Find(idchat));
            int indexMsgCache; // Index in array MessagesItem[] with the idmessage that we want
            ArrayList messages = new ArrayList();
            User user = GetUser();

            PropertyBag["idLastMsg"] = idmessage;
            if ((int)newMessage[user.Id] == 1) { // Is there a new message?
                if (((ChatsCache.ChatCache)chatsCache.chatsCache[idchat]).numMsg == 0) // Read Messages in DB. There aren't messages in cache
                {
                    messages.AddRange(ChatMessage.FindLastById(idchat, idmessage + 1));
                    if (messages.Count == 0)
                        PropertyBag["idLastMsg"] = idmessage;
                    else
                        PropertyBag["idLastMsg"] = ((ChatMessage)messages[messages.Count - 1]).Id;
                    PropertyBag["messages"] = messages;
                }
                else
                {
                    indexMsgCache = chatsCache.SearchMsgByIdInCache(idchat, idmessage + 1);

                    if (indexMsgCache == -1) // messages in cache?
                    {
                        messages.AddRange(ChatMessage.FindLastById(idchat, idmessage));
                        messages.AddRange(CopyMessagesCache(idchat, 0));
                    }
                    else
                        messages.AddRange(CopyMessagesCache(idchat, indexMsgCache));

                    ChatsCache.ChatCache chat = ((ChatsCache.ChatCache)chatsCache.chatsCache[idchat]);

                    PropertyBag["messages"] = messages;
                    PropertyBag["idLastMsg"] = (chat.messageCache[chat.numMsg - 1]).Id;
                }
            }

            newMessage[user.Id] = 0;
            LayoutName = null;        // We dont want the layout for this method
        }

        public void Enter([ARFetch("Cid",Create=false)] Chat chat, [DataBind ("ChatMessage")] ChatMessage message)
        {
            CheckGroup(chat);
            User user = GetUser();
            if ((user != null) && (chat != null))
            {
                ChatsCache.MessageItem mc = new ChatsCache.MessageItem();
                mc.Date = System.DateTime.Now;
                mc.Message = message.Message;
                mc.OwnerId = (int)user.Id;
                mc.Owner = (string)user.Name;
                mc.Chat = (int)chat.Id;

                lock(newMessage) {
//FIXME
                    for (int i = 0; i < 10000; i++) // Deberia usarse lo de abajo
                        newMessage[i] = 1;
/*                    foreach (int c in newMessage.Keys)
                    {
                        System.Console.WriteLine("C: {0}", c);
                        newMessage[c] = 1;
                    }*/
                }

                if (chatsCache.AddMessage(chat.Id, mc) == 1) // Cache full
                    lock(chatsCache) {
                        WriteCacheInDB(chat.Id);
                    }
                RenderText("");
            }
        }

        public void Quit([ARFetch("Cid",Create=false)] Chat chat)
        {
            User user = GetUser();
            if ((user != null) && (chat != null))
            {
                if (((ArrayList)(OnlinePeople[chat.Id])).Contains(user))
                    ((ArrayList)(OnlinePeople[chat.Id])).Remove(user);
                Hashtable parameters = new Hashtable();
                parameters["layout"] = false;
                RedirectToAction("index", parameters);
            }

            // The last user save date in DB
            if (((ArrayList)OnlinePeople[chat.Id]).Count == 0)
                lock(chatsCache) {
                    WriteCacheInDB(chat.Id);
                }
        }

        private static void WriteCacheInDB(int idChat)
        {
            ChatsCache.ChatCache chat;
            ChatsCache.MessageItem mi;
            ChatMessage message;

            chat = (ChatsCache.ChatCache)chatsCache.chatsCache[idChat];
            for (int i = 0; i < chat.numMsg; i++)
            {
                mi = chat.messageCache[i];
                message = ParseMsgItemToChatMessage(mi);
                message.Save();
            }

            chat.numMsg = 0;
            chatsCache.chatsCache[idChat] = chat;
        }

        /// <summary>
        /// Copy messages since indexMsgCache to numMsg
        /// </summary>
        /// <param name=idChat>Chat that we want get messages</param>
        /// <param name=indexMsgCache>Index in List of messages</param>
        private ArrayList CopyMessagesCache(int idChat, int indexMsgCache)
        {
            ChatsCache.ChatCache chat;
            ArrayList messages = new ArrayList();

            chat = (ChatsCache.ChatCache)chatsCache.chatsCache[idChat];
            for (int i = indexMsgCache; i < chat.numMsg; i++)
                messages.Add(chat.messageCache[i]);

            return messages;
        }

        /// <summary>
        /// Parse Message Items of Cache to object ChatMessage
        /// </summary>
        /// <param name=mi>Item that we want parse</param>
        private static ChatMessage ParseMsgItemToChatMessage(ChatsCache.MessageItem mi)
        {
            ChatMessage message = new ChatMessage();

            message.Message = mi.Message;
//            message.Number = mi.Number;
            message.Owner  = User.Find(mi.OwnerId); // Expensive?
            message.Chat   = Chat.Find(mi.Chat);    // Expensive?
            message.Date   = mi.Date;

            return message;
        }

/*        private void OnlineUser(int idChat)
        {
            User user = GetUser();

            if (!OnlinePeople.ContainsKey(idChat))
                OnlinePeople[idChat] = new ArrayList();
            if (!((ArrayList)(OnlinePeople[idChat])).Contains(user))
                ((ArrayList)(OnlinePeople[idChat])).Add(user);
        }
*/
        private static void DropUsers(object source, ElapsedEventArgs e)
        {
            foreach (int i in OnlinePeople.Keys)
                ((ArrayList)OnlinePeople[i]).Clear();
        }

        private static void CloseChatsEmpty(object source, ElapsedEventArgs e)
        {
             foreach (int i in OnlinePeople.Keys)
                 if (((ArrayList)OnlinePeople[i]).Count == 0) {
                    WriteCacheInDB(i);
                    OnlinePeople.Remove(i);
                }
        }

        private static void CreateTimers()
        {
            timerDrop.Elapsed += new ElapsedEventHandler(DropUsers);
            timerDrop.Interval = 600 * 1000;
            timerDrop.Start();

            timerClose.Elapsed += new ElapsedEventHandler(CloseChatsEmpty);
            timerClose.Interval = 3600 * 1000;
            timerClose.Start();
        }
    }
}

