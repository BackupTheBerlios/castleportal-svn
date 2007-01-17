// Authors:
//    Carlos Ble  <carlosble@shidix.com>
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
    public class Schedule : ActiveRecordBase
    {
        private int _Id;

        // BelongsTo
        private User _Owner;

        // HasMany 
        private IList _SharedSchedules;
        private IList _SharingMeSchedules;
        private IList _Events;                                // All events
        
        public Schedule () { }

        [PrimaryKey]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        [BelongsTo]
        public User Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

#if CACHE
        [HasAndBelongsToMany(typeof(Schedule), Table="schedule_schedule", 
                             ColumnRef="sharing", ColumnKey="shared", Cache=CacheEnum.ReadWrite)]
#else
        [HasAndBelongsToMany(typeof(Schedule), Table="schedule_schedule", 
                             ColumnRef="sharing", ColumnKey="shared")]
#endif
        public IList SharedSchedules
        {
            get { return _SharedSchedules; }
            set { _SharedSchedules = value; }
        }
        
#if CACHE
        [HasAndBelongsToMany(typeof(Schedule), Table="schedule_schedule", 
                             ColumnRef="shared", ColumnKey="sharing", Cache=CacheEnum.ReadWrite)]
#else
        [HasAndBelongsToMany(typeof(Schedule), Table="schedule_schedule", 
                             ColumnRef="shared", ColumnKey="sharing")]
#endif
        public IList SharingMeSchedules
        {
            get { return _SharingMeSchedules; }
            set { _SharingMeSchedules = value; }
        }
    
        // Example of key: "2006-13-05". Value = IList of ScheduledEvent     
        /*[HasMany(typeof(EventsByDate), Index="datetime" ,IndexType="string")]
        public IDictionary EventsHash
        {
            get { return _EventsHash; }
            set { _EventsHash = value; }
        }*/

#if CACHE
        [HasMany(typeof(ScheduledEvent), Cache=CacheEnum.ReadWrite)]
#else
        [HasMany(typeof(ScheduledEvent))]
#endif
        public IList Events
        {
            get { return _Events; }
            set { _Events = value; }
        }

        public override string ToString()
        {
            return "Schedule"; 
        }

        public static Schedule FindByUser(User user)
        {
            SimpleQuery q = new SimpleQuery(typeof(Schedule), @"
                from Schedule S
                where S.Owner = ?", user);

            Schedule[] schedules = (Schedule[])ExecuteQuery(q);
            if (schedules.Length == 0)
                return null;
            else
                return schedules[0];
        }
    }
}

