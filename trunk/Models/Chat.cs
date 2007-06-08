// Authors:
//    Carlos Ble <carlosble@shidix.com>
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
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using Iesi.Collections;

namespace CastlePortal
{
#if CACHE
    [ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
    [ActiveRecord]
#endif
    public class Chat : ActiveRecordBase
    {
        private IList _Messages;
        private string _name;
        private int _id;
        private Group _OGroup;

        [PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
#if CACHE
        [HasMany(typeof(ChatMessage), Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
        [HasMany(typeof(ChatMessage), Lazy=true)]
#endif
        public IList Messages
        {
            get { return _Messages; }
            set { _Messages = value; }
        }

        [BelongsTo]
//        [BelongsTo(Cascade=CascadeEnum.All)]
        public Group OGroup
        {
            get { return _OGroup; }
            set { _OGroup = value; }
        }

        [Property]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public static Chat[] FindAll()
        {
            return (Chat[]) ActiveRecordBase.FindAll( typeof(Chat) );
        }

        public static Chat Find(int id)
        {
            return (Chat) FindByPrimaryKey( typeof(Chat), id );
        }

        public static Chat[] FindByUser(IList groups)
        {
            IList tmp = new ArrayList();
            foreach (Group g in groups)
               tmp.Add(g.Id);

            SimpleQuery q = new SimpleQuery(typeof(Chat), @"
            from Chat C
            where
               C.OGroup.Id in (select Id from Acl)
            ");

         return (Chat[])ExecuteQuery(q);
      }

    }
}

