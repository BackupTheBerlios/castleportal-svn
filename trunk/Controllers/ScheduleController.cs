// Authors:
//    Héctor Rojas  <hectorrojas@shidix.com>
//    Carlos Ble  <carlosble@shidix.com>
//    Alberto Morales <amd77@gulic.org>
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
using System.IO;
using System.Globalization;
using log4net;
using log4net.Config;

namespace CastlePortal
{
    [Layout ("general")]
    [Helper (typeof (MenuHelper))]
    [Helper (typeof (StringHelper))]
    [Helper (typeof (ScheduleHelper))]
    [Helper (typeof (ForumHelper))]
    [Rescue("generalerror")]
    [Resource( "l10n", "l10n" )]
    [LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
    [Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
    [Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
    [DefaultAction("Redir")]
    public class ScheduleController : ARSmartDispatcherController
    {
/*        private void CheckSuperUser()
        {
            LayoutName = null;
            if ((!(bool)Session["isAdmin"]) && (!(bool)Session["isRoot"]))
                throw new Unauthorized("");
        }
*/
        private void CheckPermissions(Schedule schedule)
        {
/*            LayoutName = null;
            if (schedule != null)
            {          
                User user = (Session.Contains("User")) ? (User) Session["User"] : null;
                if (user != null)
                {
                    if ((user.Id != schedule.Owner.Id) ||
                        (((bool)Session["isAdmin"]) || ((bool)Session["isRoot"])))
                    {
                        bool shared = false;
/*                        foreach (Schedule s in user.Schedule.SharedSchedules)
                            if (s.Id == schedule.Id)
                            {
                                shared = true;
                                 break;
                            }*
                        if ((!shared) &&
                            ((!(bool)Session["isAdmin"]) && (!(bool)Session["isRoot"])))
                            throw new Unauthorized();

                    }
                }
                else
                    throw new Unauthorized();
            }
            else
                throw new Unauthorized();
*/
        }

        public void Redir()
        {
            RedirectToAction("index");
        }

        public void index ()
        {
            RedirectToAction("showcalendar");
        }

        //static int MAX_WEEKS_IN_MONTH = 6;
        static int DAYS_IN_WEEK = 7;
        static int QUARTER = 15;
        static int QUARTERS_IN_DAY = 24 * 4;
        static int SUNDAY = 6;
        static int MAX_DAYS_IN_MONTH = 31;
        //private ILog log = ((HttpApp) HttpContext.Current.ApplicationInstance).log;
        /*private static readonly Castle.Services.Logging.ILogger log = 
                  Shidix.PublicAPI.LogFactory.Create(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
*/
        public ScheduleController()
        {
                  
        }
        
        public void ShowDay(string dateTime)
        {
            User user = (User)Context.Session["User"];
            if (user != null)
            {
                CheckPermissions(Schedule.FindByUser(user));    
                Console.WriteLine("usuario = " + user.Id);
                user = User.Find(user.Id);
                Console.WriteLine("usuario = " + user.Id);
                string [] d = dateTime.Split('/');
                DateTime dt = new DateTime(int.Parse(d[2]), int.Parse(d[1]), int.Parse(d[0]), 0, 0, 0);
                ArrayList quarters = new ArrayList(QUARTERS_IN_DAY);
                ArrayList times = new ArrayList(QUARTERS_IN_DAY);

                ScheduledEvent[] events = ScheduledEvent.GetEventsInDay(Schedule.FindByUser(user), dt);
//                foreach (Schedule s in user.Schedule.SharedSchedules)
//                    events = AddScheduledEvent(events, ScheduledEvent.GetEventsInDay(s, dt));

                ScheduledEvent[] eventsTemp;
                for (int i = 0; i < (QUARTERS_IN_DAY); i++)
                {
                    times.Add(dt.ToShortTimeString());

                    eventsTemp = GetEventsByDateTimeInArray(events, dt);

                    quarters.Add(eventsTemp);
                    dt = dt.AddMinutes(QUARTER);
                }
                 PropertyBag["quarters"] = quarters;        
                PropertyBag["times"] = times;
                PropertyBag["day"] = dateTime;

                if (events.Length > 0)
                    PropertyBag["eventsInDay"] = events.Length;
            }
//            LayoutName = null;
        }

        public void ShowCalendar(int year, int month)
        {
            User user = (User)Context.Session["User"];
            if (user != null)
                CheckPermissions(Schedule.FindByUser(user));
            //int daysInMonth = System.DateTime.DaysInMonth(year, month);

          //Shidix.PublicAPI.Logger("Mostrando calendar");

//            ShareSchedules(user, User.Find(75));  // Usado solo una vez para compartir la agenda

            ScheduledEvent[] sdles = ScheduledEvent.GetEventsInMonth(Schedule.FindByUser(user), year, month);
            foreach (ScheduledEvent s in sdles)
                System.Console.WriteLine("Evento  : {0} {1} {2}", s.Name, s.StartDate, s.EndDate);
            int[] dayEvents = CountDayEvents(sdles, year, month);

/*            foreach (Schedule s in user.Schedule.SharedSchedules)
            {
                ScheduledEvent[] sdlesTemp = ScheduledEvent.GetEventsInMonth(s, year, month);
                int[] dayEventsTemp = CountDayEvents(sdlesTemp, year, month);

                for (int i = 1; i <= daysInMonth; i++)
                    dayEvents[i] += dayEventsTemp[i];

                sdles = AddScheduledEvent(sdles, sdlesTemp);
            }
*/
            IList monthMatrix = BuildMonth(year,month);
            PropertyBag["selectedMonth"] = month;
            PropertyBag["selectedYear"] = year;
            PropertyBag["monthMatrix"] = monthMatrix;
            PropertyBag["dayEvents"] = dayEvents;
            if (user != null)
            {
                //Shidix.PublicAPI.Logger(DateTime.Now.ToShortDateString());
                ScheduledEvent[] events = ScheduledEvent.GetNextOrderedEvents(Schedule.FindByUser(user), DateTime.Now);
//                foreach (Schedule s in user.Schedule.SharedSchedules)
//                    events = AddScheduledEvent(events, ScheduledEvent.GetNextOrderedEvents(s, DateTime.Now));

                if (events.Length > 5)
                {
                    ScheduledEvent[] sdlesTemp = new ScheduledEvent[5];
                    Array.Copy(events, sdlesTemp, 5);
                    events = sdlesTemp;
                }

                PropertyBag["events"] = events;
                PropertyBag["today"] = DateTime.Now.ToShortDateString();
            }
        }

        public void ShowCalendar()
        {
            DateTime dt = DateTime.Now;
            this.ShowCalendar(dt.Year, dt.Month);
            /*IList monthMatrix = BuildMonth(dt.Year, dt.Month);
            PropertyBag["selectedMonth"] = dt.Month;
            PropertyBag["selectedYear"] = dt.Year;
            PropertyBag["monthMatrix"] = monthMatrix;*/
        }

        // Build month matrix. Rows are weeks, columns are days
        private IList BuildMonth(int year, int month)
        {
            DateTime dt = new System.DateTime(year, month, 1, 0,0,0,0);
            DateTime dayIterator = dt;
            Calendar cal = CultureInfo.InvariantCulture.Calendar;

            IList monthMatrix = new ArrayList(MAX_DAYS_IN_MONTH);
            int week = 0;
            monthMatrix.Add(new ArrayList(DAYS_IN_WEEK));
            for (int i = 0 ; i <= MAX_DAYS_IN_MONTH -1; i++)
            {
                IList w = (ArrayList) monthMatrix[week];
                for (int k = 0; k < DAYS_IN_WEEK; k++)
                {
                    if (w.Count < DAYS_IN_WEEK)
                        w.Add("");
                    //else
                    //    w.
                }
                int day = dayIterator.Day;
                int dayOfWeek = (int)cal.GetDayOfWeek(dayIterator) -1;  // We want first day on Monday, not Sunday
                if (dayOfWeek == -1)                        
                    dayOfWeek = SUNDAY;
                w[dayOfWeek] = day;
                if (dayOfWeek == SUNDAY)
                { 
                    week++;
                    monthMatrix.Add(new ArrayList(DAYS_IN_WEEK));
                }
                dayIterator = dayIterator.AddDays(1);
                if (cal.GetMonth(dayIterator) != cal.GetMonth(dt))       // discard if day is in next month
                    break;
            }
            return monthMatrix;
        }

        public void CreateSchedule([ARFetch ("Id", Create = false)] User user)
        {
            if (user != null)
            {
                if (Schedule.FindByUser(user) == null)
                {
                    Schedule schedule = new Schedule();
                    schedule.Owner = user;
                    schedule.Save();
                }
            }

            PropertyBag["user"] = user;
            Hashtable parameters = new Hashtable();
            parameters["id"] = user.Id;
            RedirectToAction("showschedule", parameters);
        }

        public void ShowSchedule()
        {
            User user = (User)Context.Session["User"];
            if (user != null)
                user = User.Find(user.Id);
            PropertyBag["user"] = user;
            LayoutName = null;
        }

        public void ShowDetail([ARFetch ("Id", Create = false)]ScheduledEvent sdle)
        {
            PropertyBag["event"] = sdle;

            LayoutName = null;
        }

        // CreateEvent by clicking on date 
        public void CreateEvent([ARFetch ("Id", Create = false)] User user, string datetime)
        {
            // TODO: Search for other event in same datetime and alert user
            PropertyBag["user"]  = user;
            PropertyBag["start"] = datetime;

            LayoutName = null;
        }

        // Create an empty event
        public void CreateEvent([ARFetch ("Id", Create = false)] User user)
        {
            PropertyBag["user"]  = user;
        }

        public void GetEvents([ARFetch("Id", Create = false)] User user, string datetime)
        {
            if (user != null)
            {
                PropertyBag["events"] = ScheduledEvent.FindByDateAndUser(datetime, user);
            }
        }

//        public void SaveEvent([ARFetch ("Id", Create = false)] User user, 
//                                     [DataBind ("Event")] ScheduledEvent ev)
        public void SaveEvent(ScheduledEvent ev)
        {
            User user = (User)Context.Session["User"];
            ev.Schedule = Schedule.FindByUser(user);
            DateTime dt = ev.StartDate;
            if (dt.Day < 13)
                ev.StartDate = new DateTime(dt.Year, dt.Day, dt.Month, dt.Hour, dt.Minute, dt.Second);
            dt = ev.EndDate;
            if (dt.Day < 13)
                ev.EndDate = new DateTime(dt.Year, dt.Day, dt.Month, dt.Hour, dt.Minute, dt.Second);
            if (ev.StartDate > ev.EndDate) // EndDate can't be before StartDate
                ev.EndDate = ev.StartDate;
            Schedule.FindByUser(user).Events.Add(ev);
            if (ev.Id != 0)
                ev.Save();
            else
                ev.Create();
            user.Update();
            PropertyBag["user"] = user;
            LayoutName = null;
            RedirectToAction("showcalendar");
        }

        public void UpdateEvent([ARDataBind ("Event")] ScheduledEvent ev)
        {
            ev.Update();

            RedirectToAction("showcalendar");
        }

        public void DeleteEvent([ARFetch ("Id", Create = false)]ScheduledEvent sdle)
        {
            Schedule s = sdle.Schedule;
            s.Events.Remove(sdle);
            sdle.Delete();
            s.Update();
            RedirectToAction("showcalendar");
        }

        public void ModifyEvent([ARFetch ("Id", Create = false)]ScheduledEvent sdle)
        {
            PropertyBag["idevent"] = sdle.Id;
            PropertyBag["Event"] = sdle;

            LayoutName = null;
        }

        /// <summary>
        /// if there are overlap between different events request confirmation or
        /// modifify data to save event, else continue and save the event.
        /// </summary>
        public void CheckOverlap([ARFetch ("Id", Create = false)] User user, 
                                    [DataBind ("Event")] ScheduledEvent sdle,
                                    int modifycheck)
        {
            ScheduledEvent[] events = ScheduledEvent.GetEventsByDateTime(Schedule.FindByUser(user), sdle.StartDate, sdle.Id);
//                foreach (Schedule s in user.Schedule.SharedSchedules)
//                    events = AddScheduledEvent(events, ScheduledEvent.GetEventsByDateTime(s, sdle.StartDate, sdle.Id));
System.Console.WriteLine("C0 {0}", events.Length);
System.Console.WriteLine("C01 {0}", sdle.StartDate);

            if (events.Length > 0) // Overlap
            {
System.Console.WriteLine("C1");
                if (modifycheck == 1) // First change. Request confirmation. Show form again 
                {
System.Console.WriteLine("C2");
                    PropertyBag["overlaps"] = events.Length;
                    PropertyBag["event"] = sdle;
                    PropertyBag["events"] = events;
                    PropertyBag["idevent"] = sdle.Id;
                    PropertyBag["target"] = "_parent";
                    LayoutName = null;
                }
                else
                {
System.Console.WriteLine("C3");
                    PropertyBag["target"] = "_parent";
                    SaveEvent(sdle); // Form confirmed. Save data
                }
            }
            else
            {
System.Console.WriteLine("C4");
                PropertyBag["target"] = "_parent";
                SaveEvent(sdle); // There are not overlap. Save data
            }
System.Console.WriteLine("C5");
        }

        public void ShareSchedules(Schedule sche1, Schedule sche2)
        {
            bool exist = false;
    
            foreach (Schedule s in sche1.SharedSchedules)
                if (s.Id == sche2.Id)
                    exist = true;

            if (exist == false)
            {
                sche1.SharedSchedules.Add(sche2);
                sche1.Save();
            }
            else
                System.Console.WriteLine("Ya estaba compartida");
        }

        /// <summary>
        /// Return array with the number events in each day
        /// </summary>
        /// <param name=sdles>Array with events</param>
        private int[] CountDayEvents(ScheduledEvent[] sdles, int year, int month)
        {
            int daysInMonth = System.DateTime.DaysInMonth(year, month);
            Int32[] events = new Int32[daysInMonth + 1];
            DateTime beginDay, endDay;

            for (int i = 1; i <= daysInMonth; i++)
            {
                events[i] = 0;
                beginDay = new DateTime(year, month, i, 0, 0, 0);
                endDay = new DateTime(year, month, i, 23, 59, 59);

                foreach (ScheduledEvent sdle in sdles)
                {
                    if ((endDay >= sdle.StartDate) && (beginDay <= sdle.EndDate))
                        events[i]++;
                }
            }

            return events;
        }

        /// <summary>
        /// Chain two arrays of ScheduledEvent
        /// </summary>
        /// <param name=s1>First array</param>
        /// <param name=s2>Second array</param>
        /// <returns>Array with s1 + s2</returns>
/*        private ScheduledEvent[] AddScheduledEvent(ScheduledEvent[] s1, ScheduledEvent[] s2)
        {
            int i;
            ScheduledEvent[] sdle = new ScheduledEvent[s1.Length + s2.Length];

            for (i = 0; i < s1.Length; i++)
                sdle[i] = s1[i];

            for (int j = i; j < s1.Length + s2.Length; j++)
                sdle[j] = s2[j - s1.Length];
            
            return sdle;
        }
*/
        /// <summary>
        /// Returns events in interval between dt and dt + 15 minutes
        /// </summary>
        /// <param name=sdles>Array of events where we search</param>
        /// <param name=dt>Start of the interval</param>
        /// <returns>Events in interval</returns>
        private ScheduledEvent[] GetEventsByDateTimeInArray(ScheduledEvent[] sdles, DateTime dt)
        {
            int j = 0;
            ScheduledEvent[] sdlesTemp = new ScheduledEvent[sdles.Length];

            for (int i = 0; i < sdles.Length; i++)
            {
                if (((sdles[i].StartDate <= dt) && (sdles[i].EndDate >= dt)) ||
                 ((sdles[i].StartDate >= dt) && (sdles[i].EndDate < dt.AddMinutes(15)))) // Short events
                {
                    sdlesTemp[j] = sdles[i];
                    j++;
                }
            }

            ScheduledEvent[] sdlesReturn = new ScheduledEvent[j];
            for (int i = 0; i < j; i++)
                sdlesReturn[i] = sdlesTemp[i];

            return sdlesReturn;

        }
    }
}

