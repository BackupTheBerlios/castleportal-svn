// Authors:
//    Héctor Rojas  <hectorrojas@shidix.com>
//    Carlos Ble  <carlosble@shidix.com>
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


using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using System.Collections;
using NHibernate.Expression;
using NHibernate;
using System;

namespace CastlePortal
{
#if CACHE
    [ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
    [ActiveRecord]
#endif
    public class ScheduledEvent : ActiveRecordBase
    {
        private int _Id;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private string _Name;
        private string _Description;
        private string _Link;
        
        // BelongsTo
        private Schedule _Schedule;

        public ScheduledEvent () { }

        [PrimaryKey]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        [Property]
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        [Property]
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        
        [Property]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        
        [Property]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        
        [Property]
        public string Link
        {
            get { return _Link; }
            set { _Link = value; }
        }
    
        [BelongsTo(NotNull=true)]
        public Schedule Schedule
        {
            get { return _Schedule; }
            set { _Schedule = value; }
        }

        public static ScheduledEvent[] FindByDateAndUser(string datetime, User user)
        {
            if ((datetime.Length > 0) && (user != null))      
            {
                /* This works on hibernate but not in nhibernate yet:
                return (ScheduledEvent[]) ActiveRecordBase.FindOne(typeof(ScheduledEvent), 
                    Expression.geProperty("StartDate", datetime), 
                    Expression.ltProperty("EndDate", datetime),
                    Expression.eq("Schedule", user.Schedule));
                */
                
                SimpleQuery q = new SimpleQuery(typeof(ScheduledEvent), @"
                    from ScheduledEvent S
                      where S.StartDate < ? and 
                    S.EndDate > ? and
                    S.Schedule.Owner = ?", datetime, datetime, user);
                return (ScheduledEvent[]) ExecuteQuery(q);
            }    
            else
                return null;
        }

        public static ScheduledEvent[] GetEventsByDateTime(Schedule sdle, DateTime dt, int idSdle)
        {
            SimpleQuery q = new SimpleQuery(typeof(ScheduledEvent), @"
                from ScheduledEvent S
                where S.Schedule = ?
                and S.StartDate <= ?
                and S.EndDate >= ?
                and S.Id != ?
                order by S.StartDate", sdle, dt, dt, idSdle);

            return (ScheduledEvent[]) ExecuteQuery(q);
        }
        
        public static ScheduledEvent[] GetOrderedEvents(Schedule sdle)
        {
            SimpleQuery q = new SimpleQuery(typeof(ScheduledEvent), @"
                from ScheduledEvent S
                where S.Schedule = ?
                order by S.StartDate", sdle.Id);

            return (ScheduledEvent[]) ExecuteQuery(q);
        }
        
        public static ScheduledEvent[] GetNextOrderedEvents(Schedule sdle, DateTime d)
        {
            //string dt = Shidix.PublicAPI.Date2Postgres(d);
            //Shidix.PublicAPI.Logger("Schedule id= " + sdle.Id);
            //Shidix.PublicAPI.Logger("datetime = " + dt);
            SimpleQuery q = new SimpleQuery(typeof(ScheduledEvent), @"
                from ScheduledEvent S
                where S.Schedule = ?
                and S.StartDate > ?
                order by S.StartDate", sdle, d);
            return (ScheduledEvent[]) ExecuteQuery(q);
        }

        /// <summary>
        /// Return all events in year and month
        /// </summary>
        /// <param name="sdel">Schedule where search</param>
        /// <param name="year">Year where search</param>
        /// <param name="month">Month to get the events</param>
        public static ScheduledEvent[] GetEventsInMonth(Schedule sdle, int year, int month)
        {
//            DateTime sd = new DateTime(year, month, System.DateTime.DaysInMonth(year, month), 23, 59, 59);
//            DateTime ed = new DateTime(year, month, 1, 0, 0, 0);

            DateTime beginMonth = new DateTime(year, month, 1, 0, 0, 0);
            DateTime endMonth = new DateTime(year, month, System.DateTime.DaysInMonth(year, month), 23, 59, 59);

            SimpleQuery q = new SimpleQuery(typeof(ScheduledEvent), @"
                FROM ScheduledEvent S
                WHERE
                    S.Schedule = ? AND
                    S.StartDate < ? AND
                      S.EndDate > ?
                ", sdle, endMonth, beginMonth);

            return (ScheduledEvent[]) ExecuteQuery(q);
        }
        
        /// <summary>
        /// Return all events in a day 
        /// </summary>
        /// <param name="sdel">Schedule where search</param>
        /// <param name="dt">DateTime with the day</param>
        public static ScheduledEvent[] GetEventsInDay(Schedule sdle, DateTime dt)
        {
            DateTime dtNextDay = dt.AddDays(1);
            DateTime dtBeforeDay = dt.AddSeconds(-1);
            
            SimpleQuery q = new SimpleQuery(typeof(ScheduledEvent), @"
                FROM ScheduledEvent S
                WHERE
                    S.Schedule = ? AND
                    ((S.StartDate >= ?) AND (S.EndDate < ?) OR
                    (S.StartDate > ?) AND (S.StartDate < ?) AND (S.EndDate > ?) OR
                    (S.StartDate < ?) AND (S.EndDate < ?) AND (S.EndDate > ?) OR
                    (S.StartDate < ?) AND (S.EndDate > ?))
                ", sdle, dt, dtNextDay, dtBeforeDay, dtNextDay, dt, dt, dtNextDay, dtBeforeDay, dt, dt);

            return (ScheduledEvent[]) ExecuteQuery(q);
        }
                
        /* This dont compile yet:    
        public static ScheduledEvent[] FindByDate(string datetime, User user)
        {
            return (ScheduledEvent[]) 
                      Execute( new NHibernateDelegate(FindByDateCallback));//, datetime); //, user);
        }

        private static object FindByDateCallback( ISession session, object date)
        {
            IQuery query = session.CreateQuery("from ScheduledEvent S where S.startdate < :date and S.enddate > :date"); // and S.schedule = :userschedule" );
            query.SetString("date", (string) date);
            //query.SetString("userschedule", (int) ((User)user).Schedule.Id);
            IList results = query.List();
            ScheduledEvent[] events = new ScheduledEvent[results.Count];
            results.CopyTo(events, 0);
            return events;
        }
        */


        public override string ToString()
        {
            return "Schedule"; 
        }

    }
}
