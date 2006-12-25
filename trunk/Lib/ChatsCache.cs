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
using System.Collections;

namespace CastlePortal
{
    public class ChatsCache
    {
        const int maxMsg = 100;        // Max number of messages in one chatcache
        public Hashtable chatsCache = new Hashtable();

        public class MessageItem
        {
            private int _id;
            private DateTime _date;
            private string _message;
            private int _ownerId;
            private string _owner;
            private int _chat;

            public int Id
            {
                get { return _id; }
                set { _id = value; }
            }

           public string Message
            {
                get { return _message; }
                set { _message = value; }
            }
            
            public DateTime Date
            {
                get { return _date; }
                set { _date = value; }
            }

            public int OwnerId
            {
                get { return _ownerId; }
                set { _ownerId = value; }
            }

            public string Owner
            {
                get { return _owner; }
                set { _owner = value; }
            }
        
            public int Chat
            {
                get { return _chat; }
                set { _chat = value; }
            }

        }

        public struct ChatCache
        {
            public int numMsg;
            public int last;
            public MessageItem[] messageCache;
        }

/*    public void ChatsCache()
    {
        Hashtable chatsCache = new Hashtable();
    }*/

        public ChatCache CreateChatCache()
        {
            ChatCache chatCache = new ChatCache();
            chatCache.messageCache = new MessageItem[maxMsg];
            chatCache.numMsg = 0;
            chatCache.last = 0;

            return chatCache;
        }

        public int AddMessage(int idChat, MessageItem mc) {
            ChatCache chat;
            chat = (ChatCache)chatsCache[idChat];
            if (chat.numMsg == 0)
                mc.Id = ChatMessage.FindLastIdMsg(idChat) + 1;
            else
                mc.Id = (chat.messageCache[chat.numMsg - 1]).Id + 1;
            chat.messageCache[chat.numMsg] = mc;

            chat.numMsg++;

            chatsCache[idChat] = chat;
            
            if (chat.numMsg == maxMsg)
                return 1;
            else
                return 0;
        }

        public int SearchMsgByIdInCache(int idChat, int idMsg)
        {
            int i;
            ChatCache chat;

            chat = (ChatCache)chatsCache[idChat];

            for (i = 0; i < chat.numMsg && chat.messageCache[i].Id != idMsg; i++)
                ;

            if (i != chat.numMsg) // idMsg exist in Cache
                return i;

            return -1;
        }

    }
}

