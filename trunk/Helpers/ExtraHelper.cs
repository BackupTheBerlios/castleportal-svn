// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Hector Rojas <hectorrojas@shidix.com>
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
using System.Text.RegularExpressions;

namespace CastlePortal
{
public class ExtraHelper:Castle.MonoRail.Framework.Helpers.AbstractHelper
{
    public char[] CharRange(int length)
    {
        char[] range = new char[length];
        range[0] = 'A';
        for (int i = 1; i < length; i++)
        {
            range[i] = (char)(range[i-1] +1);
        }
        return range;
    }

    public string CategoryDescription(Category category, string lang)
    {
        if (category != null)
        {
            if ((lang != null) && (lang.Length > 0))
            {
                Language language = Language.FindByName(lang);
                CategoryTranslation ct = CategoryTranslation.FindByCategoryAndLang(category, language);

                if (ct == null)
                    return category.Description;
                else
                    return ct.Translation;
            }
            else
                return category.Description;
        }
        else
            return String.Empty;
    }

/*
    public string GetMonth(int month)
    {
        switch (month)
        {
        case 1:
            return "Enero";
        case 2:
            return "Febrero";
        case 3:
            return "Marzo";
        case 4:
            return "Abril";
        case 5:
            return "Mayo";
        case 6:
            return "Junio";
        case 7:
            return "Julio";
        case 8:
            return "Agosto";
        case 9:
            return "Septiembre";
        case 10:
            return "Octubre";
        case 11:
            return "Noviembre";
        case 12:
            return "Diciembre";
        default:
            return "";
        }
    }

    public int GetNextMonth(int month)
    {
        if (month == 12)
            return 1;
        else
            return month +1;
    }

    public int GetNextYear(int year, int month)
    {
        if (month == 12)
            return year +1;
        else
            return year;
    }

    public int GetPrevMonth(int month)
    {
        if (month == 1)
            return 12;
        else
            return month -1;
    }

    public int GetPrevYear(int year, int month)
    {
        if (month == 1)
            return year -1;
        else
            return year;
    }

    public string GetDaysOfWeek()
    {
        return "";
    }

    public string DrawDay(int day, int year, int month, int[] eventsForDay, string calToday, string calEvent, string calEvents, string action)
    {
        //if ((day) && (year) && (month))
        //{
        DateTime dt = DateTime.Now;
        string div = "";
        if ((day == dt.Day) && (year == dt.Year) && (month == dt.Month))
            div += "<td class='" + calToday + "'>";
        else if (eventsForDay[day] == 1)
            div += "<td class='" + calEvent + "'>";
        else if (eventsForDay[day] > 1)
            div += "<td class='" + calEvents + "'>";
        else
            div += "<td>";
        div += "<a href=\"javascript:;\"  onclick=\"javascript:frames['dia'].location.href = '"+ action+"dateTime="+day+"/"+month+"/"+year+"';\">";
        div += day;
        div += "</a>";
        div += "</td>";
        return div;
        //}
    }

#if GPC
    public string ShowDay(ArrayList quarters, ArrayList times)
    {
        string ret= "";
        string color = "yellow";

        if (quarters != null)
            for (int i = 0; i < quarters.Count; i++)
            {
                if (quarters[i] != null)
                {
                    ScheduledEvent[] events = (ScheduledEvent[])quarters[i];
                    if ((events != null) && (events.Length > 0))
                    {
                        if (events.Length == 1)
                        {
                            ret += "<tr class='event'><td class='eventTime'>"+ times[i]+"</td><td class='eventName'>";
                            ret += events[0].Name;
                            ret += "</td><td class='eventDescription'>";
                            ret += events[0].Description;

                            ret += "</td><td class='eventLink'>";
                            ret += "<a href=\"showdetail.html?Id=" + events[0].Id + "\">";
                            ret += "Ver mas</a>";
                            ret += "</td></tr>";
                        }
                        else if (events.Length > 1)
                        {
                            ret += "<tr class='eventOverLap'><td class='eventTime'>"+ times[i]+"</td><td class='eventName'>";
                            ret += "<a href='#' class='overLap'>SOLAPAMIENTO</a>";
                            for (int j = 0; j < events.Length; j++)
                            {
                                ret += "<br>" + events[j].Name;
                            }

                            ret += "</td><td class='eventDescription'>";
                            for (int j = 0; j < events.Length; j++)
                            {
                                ret += "<br>" + events[j].Description;
                            }

                            ret += "</td><td class='eventLink'>";
                            for (int j = 0; j < events.Length; j++)
                            {
                                ret += "<br><a href=\"showdetail.html?Id=" + events[j].Id + "\">";
                                ret += "Ver mas</a>";
                            }
                            ret += "</td>";
                            ret += "</tr>";

                        }

                    }
                    if (color.CompareTo("yellow") == 0) // alternate color row
                        color = "white";
                    else
                        color = "yellow";

                }
            }
        return ret;
    }
    public string WriteOptionsMinute()
    {
        string ret = "";

        for (int i = 0; i < 60; i = i + 5)
            if (i < 10)
                ret += "<option value='0" + i.ToString() + "'>0" + i.ToString();
            else
                ret += "<option value='" + i.ToString() + "'>" + i.ToString();

        return ret;
    }

    public string WriteOptionsHour()
    {
        string ret = "";

        for (int i = 0; i < 24; i++)
            if (i < 10)
                ret += "<option value='0" + i.ToString() + "'>0" + i.ToString();
            else
                ret += "<option value='" + i.ToString() + "'>" + i.ToString();

        return ret;
    }
#endif
*/
    public string getTypeOfDay (int day, int year, int month)
    {
        //if ((day) && (year) && (month))
        //{
        DateTime dt = DateTime.Now;
        string c = "";
        if ((day == (int)dt.Day) && (year == (int)dt.Year) && (month == (int)dt.Month))
            c += "calendarToday";
        else
            c += "ordinaryDay";
        return c;
        //}
    }

    public string RemoveHtmlTagsContextualSearch(string htmlString, int length, string key)
    {
        Regex regex = new Regex("</?[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        htmlString = regex.Replace(htmlString, string.Empty);

        int i = htmlString.IndexOf(key);
        int startIndex = i - length / 2;
        if (startIndex < 0)
            startIndex = 0;
        if (startIndex + length > htmlString.Length)
            length = htmlString.Length - startIndex;

        char[] htmlCharArray = htmlString.ToCharArray();

        while ((startIndex > 0) && (htmlCharArray[startIndex] != ' '))
        {
            startIndex--;
            length++;
        }
        while ((startIndex + length < htmlString.Length) && (htmlCharArray[startIndex + length] != ' '))
            length++;

        string htmlSubstring = htmlString.Substring(startIndex, length);

        return htmlSubstring;
    }

}
}
