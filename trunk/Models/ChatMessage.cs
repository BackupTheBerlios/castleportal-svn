// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Héctor Rojas <hectorrojas@shidix.com>
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
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using System.Collections;
using Iesi.Collections;

namespace CastlePortal
{
#if CACHE
    [ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
    [ActiveRecord]
#endif
    public class ChatMessage : ActiveRecordBase
    {
        private int _id;
        private int _number;
        private DateTime _date;
        private string _message;
        private User _Owner;
        private Chat _Chat;

        [PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [BelongsTo]
        public Chat Chat
        {
            get { return _Chat; }
            set { _Chat = value; }
        }

        [BelongsTo]
        public User Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        [Property]
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        [Property]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        [Property]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public static ChatMessage[] FindAll()
        {
            return (ChatMessage[]) ActiveRecordBase.FindAll( typeof(ChatMessage) );
        }

        public static ChatMessage Find(int id)
        {
            return (ChatMessage) FindByPrimaryKey( typeof(ChatMessage), id );
        }

        public static ChatMessage[] FindLastById(int chat, int id)
        {
            return FindLast(chat, id, 5);
        }

        public static ChatMessage[] FindLastByMinutes(int chat, int minutes)
        {
            return FindLast(chat, -1, minutes);
        }

        public static ChatMessage[] FindLast(int chat, int id, int minutes)
        {
            DateTime date = System.DateTime.Now.AddMinutes(- minutes);

            SimpleQuery q = new SimpleQuery(typeof(ChatMessage), @"
                FROM ChatMessage C
                WHERE
                    C.Chat = ? AND
                    C.Id > ? AND
                    C.Date > ?
                    ORDER BY date;", chat, id, date);

            return (ChatMessage[]) ExecuteQuery(q);
        }

        public static int FindLastIdMsg(int chat)
        {
            ScalarQuery q = new ScalarQuery(typeof(ChatMessage), @"
            SELECT MAX(C.Id)
            FROM ChatMessage C
            WHERE C.Chat = ?", chat);

// return (int) ExecuteQuery(q);
            object val = ExecuteQuery(q);
            if (val != null)
                return (int)val;
            else
                return 0;
        }
    }
}

