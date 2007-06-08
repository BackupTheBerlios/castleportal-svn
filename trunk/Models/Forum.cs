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
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using System.Collections;
using NHibernate.Expression;
using NHibernate;

namespace CastlePortal
{

#if CACHE
    [ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
    [ActiveRecord]
#endif
    public class Forum : ActiveRecordValidationBase 
    {
        private int _id;
        private string _title;
        private string _description;
        private DateTime _date;
        private Group _admins;
        private Group _forumGroup;
        private IList _forumFolders;

        [PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
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
        public Group ForumGroup
        {
            get { return _forumGroup; }
            set { _forumGroup = value; }
        }

       [BelongsTo]
        public Group Admins
        {
            get { return _admins; }
            set { _admins = value; }
        }

#if CACHE
        [HasMany(typeof(ForumFolder), OrderBy="Date", Cache=CacheEnum.ReadWrite, Lazy=true)]
#else
        [HasMany(typeof(ForumFolder), OrderBy="Date", Lazy=true)]
#endif
        public IList ForumFolders
        {
            get { return _forumFolders; }
            set { _forumFolders = value; }
        }

      public static Forum[] FindAll()
      {
         return (Forum[])ActiveRecordBase.FindAll(typeof(Forum));
      }

        public static Forum Find(int id)
        {
            return (Forum)ActiveRecordBase.FindByPrimaryKey(typeof(Forum), id);
        }

        public static Forum[] FindAllOrderByDateTime()
        {
            SimpleQuery q = new SimpleQuery(typeof(Forum), @"
                from Forum F
                order by F.Date");

            return (Forum[]) ExecuteQuery(q);
        }
    }
}

