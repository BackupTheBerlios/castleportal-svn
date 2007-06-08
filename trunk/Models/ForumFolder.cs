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
    public class ForumFolder : ActiveRecordValidationBase
    {
        private int _id;
        private Forum _forum;
        private ForumFolder _parent;
        private string _title;
        private string _description;
        private DateTime _date;
        private Group _moderators;
        private IList _foldersChildren;
        private IList _forumMessages;

        [PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [BelongsTo]
        public Forum Forum
        {
            get { return _forum; }
            set { _forum = value; }
        }

        [BelongsTo]
        public ForumFolder Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [Property]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [Property]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [Property]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

       [BelongsTo]
        public Group Moderators
        {
            get { return _moderators; }
            set { _moderators = value; }
        }

#if CACHE
        [HasMany(typeof(ForumFolder), OrderBy="Date", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
        [HasMany(typeof(ForumFolder), OrderBy="Date", Lazy=true)]
#endif
        public IList FoldersChildren
        {
            get { return _foldersChildren; }
            set { _foldersChildren = value; }
        }

#if CACHE
        [HasMany(typeof(ForumMessage), OrderBy="Date", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
        [HasMany(typeof(ForumMessage), OrderBy="Date", Lazy=true)]
#endif
        public IList ForumMessages
        {
            get { return _forumMessages; }
            set { _forumMessages = value; }
        }

        public static ForumFolder[] FindAll()
        {
            return (ForumFolder[])ActiveRecordBase.FindAll(typeof(ForumFolder));
        }

        public static ForumFolder Find(int id)
        {
            return (ForumFolder)ActiveRecordBase.FindByPrimaryKey(typeof(ForumFolder), id);
        }
    }
}

