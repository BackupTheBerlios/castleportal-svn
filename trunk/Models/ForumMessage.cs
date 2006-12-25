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
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;

namespace CastlePortal
{

#if CACHE
    [ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
    [ActiveRecord]
#endif
    public class ForumMessage : ActiveRecordValidationBase
    {
        private int _id;
        private ForumFolder _forumFolder;
        private ForumMessage _parent;
        private User _owner;
        private string _title;
        private string _body;
        private DateTime _date;
        private int _level;
        private IList _messagesChildren;

        [PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [BelongsTo]
        public ForumFolder ForumFolder
        {
            get { return _forumFolder; }
            set { _forumFolder = value; }
        }

        [BelongsTo]
        public ForumMessage Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [BelongsTo]
        public User Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        [Property]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [Property]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        [Property]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        [Property]
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

#if CACHE
        [HasMany(typeof(ForumMessage), OrderBy="Date", Cache=CacheEnum.ReadWrite)]
#else
        [HasMany(typeof(ForumMessage), OrderBy="Date")]
#endif
        public IList MessagesChildren
        {
            get { return _messagesChildren; }
            set { _messagesChildren = value; }
        }

        public static ForumMessage[] FindAll()
        {
            return (ForumMessage[])ActiveRecordBase.FindAll(typeof(ForumMessage));
        }

        public static ForumMessage Find(int id)
        {
            return (ForumMessage)ActiveRecordBase.FindByPrimaryKey(typeof(ForumMessage), id);
        }
    }
}

