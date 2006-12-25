// Authors:
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
using System.Collections;

namespace CastlePortal
{
/* Conversiones variadas a string normalmente */
public class StringHelper
{
    // SAMPLE CODE SOBRE REGEX
    /*private static System.Object[] busquedas = new System.Object[] {
    	// Comment all <script></script> occurrences
    	new System.Text.RegularExpressions.Regex(@"[*(?:(?!</)<){0,1})*</script[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
    	// Comment everything before <body>
    	new System.Text.RegularExpressions.Regex(@"(.*<body[^>]*>)", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
    };*/

    /* Escapar HTML */
    static public string Filtra (string entrada)
    {
        /*foreach ( System.Text.RegularExpressions.Regex item in busquedas ) {
        	entrada = item.Replace(entrada, "$1");
        }*/
        entrada = entrada.Replace("&", "&amp;");
        entrada = entrada.Replace(">", "&gt;");
        entrada = entrada.Replace("<", "&lt;");
        entrada = entrada.Replace("\\", "\\\\");
        return entrada;
    }

    /* Convertir una string del RFC822 a algo human-readable */
    // de http://www.informit.com/guides/content.asp?g=dotnet&seqNum=172&rl=1
    static private System.DateTime Rfc822Parse (string s)
    {
        const string Rfc822DateFormat = @"^((Mon|Tue|Wed|Thu|Fri|Sat|Sun), *)?(?<day>\d\d?) +" + @"(?<month>Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) +" + @"(?<year>\d\d(\d\d)?) +" + @"(?<hour>\d\d):(?<min>\d\d)(:(?<sec>\d\d))? +" + @"(?<ofs>([+\-]?\d\d\d\d)|UT|GMT|EST|EDT|CST|CDT|MST|MDT|PST|PDT).*$";

        // try to parse it
        System.Text.RegularExpressions.Regex reDate = new System.Text.RegularExpressions.Regex (Rfc822DateFormat);
        System.Text.RegularExpressions.Match m = reDate.Match (s);
        if (!m.Success)
        {
            // Didn't match either expression. Throw an exception.
            throw new System.FormatException ("String is not a valid date time stamp.");
        }
        try
        {
            int year = int.Parse (m.Groups["year"].Value);
            // handle 2-digit and 3-digit years
            if (year < 1000)
            {
                if (year < 50)
                    year = year + 2000;
                else
                    year = year + 1999;
            }

            int month = ParseRfc822Month (m.Groups["month"].Value);
            int day = m.Groups["day"].Success ? int.Parse (m.Groups["day"].Value) : 1;
            int hour = m.Groups["hour"].Success ? int.Parse (m.Groups["hour"].Value) : 0;
            int min = m.Groups["min"].Success ? int.Parse (m.Groups["min"].Value) : 0;
            int sec = m.Groups["sec"].Success ? int.Parse (m.Groups["sec"].Value) : 0;
            int ms = m.Groups["ms"].Success ? (int) System.Math.Round ((1000 * double.Parse (m.Groups["ms"].Value))) : 0;

            System.TimeSpan ofs = System.TimeSpan.Zero;
            if (m.Groups["ofs"].Success)
            {
                ofs = ParseRfc822Offset (m.Groups["ofs"].Value);
            }
            // datetime is stored in UTC
            return new System.DateTime (year, month, day, hour, min, sec, ms) - ofs;
        }
        catch (System.Exception ex)
        {
            throw new System.FormatException ("String is not a valid date time stamp.", ex);
        }
    }

    /* Auxiliar de la de arriba */
    // de http://www.informit.com/guides/content.asp?g=dotnet&seqNum=172&rl=1
    private static readonly string[] MonthNames = new string[]{ "Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
                                                              };

    /* Auxiliar de la de arriba */
    // de http://www.informit.com/guides/content.asp?g=dotnet&seqNum=172&rl=1
    private static int ParseRfc822Month (string monthName)
    {
        for (int i = 0; i < 12; i++)
        {
            if (monthName == MonthNames[i])
            {
                return i + 1;
            }
        }
        throw new System.ApplicationException ("Invalid month name");
    }

    /* Auxiliar de la de arriba */
    // de http://www.informit.com/guides/content.asp?g=dotnet&seqNum=172&rl=1
    private static System.TimeSpan ParseRfc822Offset (string s)
    {
        if (s == string.Empty)
            return System.TimeSpan.Zero;
        int hours = 0;
        switch (s)
        {
        case "UT":
        case "GMT":
            break;
        case "EDT":
            hours = -4;
            break;
        case "EST":
        case "CDT":
            hours = -5;
            break;
        case "CST":
        case "MDT":
            hours = -6;
            break;
        case "MST":
        case "PDT":
            hours = -7;
            break;
        case "PST":
            hours = -8;
            break;
        default:
            if (s[0] == '+')
            {
                string sfmt = s.Substring (1, 2) + ":" + s.Substring (3, 2);
                return System.TimeSpan.Parse (sfmt);
            }
            else
                return System.TimeSpan.Parse (s.Insert (s.Length - 2, ":"));
        }
        return System.TimeSpan.FromHours (hours);
    }

    /* Covertir un array de bytes a string */
    public static string ByteArray (System.Byte[]x)
    {
        return System.Text.Encoding.ASCII.GetString (x);
    }

    /* Mostrar un link a un attachment, según el tipo de content-type que sea */
    // TODO: Poner iconos para PDF, ZIP, RAR, etc.
    /*		public static string Attachment (System.Net.Imap.Attachment x)
    		{
    			if (x.ContentType == "image/png")
    			{
    				return "<img src=\"attachment.html?folder=$folder&mid=$mid&aid=$i>";
    			}
    			else
    			{
    				return "<p>filename: " + x.FileName + ", Content-Type: " + x.ContentType + "</p>";
    			}
    		}*/

    /* Convertir un numero entero a número con unidades */
    public static string IntFriendly (int size)
    {
        if (size < 1024)
            return size + "&nbsp;B";
        else if (size < (1024 * 1024))
            return (size / 1024) + "&nbsp;KB";
        else
            return (size / (1024 * 1024)) + "&nbsp;MB";
    }
    public static string IntFriendly (string i)
    {
        if (i == null)
            return "--nulo--";
        int size = System.Int32.Parse (i);
        return IntFriendly (size);
    }

    /* Convertir una fecha a formato human-readable */
    public static string DateFriendly (string d)
    {
        System.DateTime fecha;
        try
        {
            fecha = Rfc822Parse (d);
        }
        catch (System.Exception)
        {
            // estamos amistoseando una fecha... si no sirve, la dejamos poco amistosa.
            System.Console.WriteLine ("Error al parsear: \""+d+"\"");
            return d;
        }
        return DateFriendly(fecha);
    }
    public static string DateFriendly (System.DateTime fecha)
    {
        System.DateTime today = System.DateTime.Now;
        System.DateTime midnight= new System.DateTime(today.Year, today.Month, today.Day);

        if (fecha > midnight)
        {
            return "Hoy " + fecha.ToShortTimeString();
        }
        else if (fecha.AddDays(1) > midnight)
        {
            return "Ayer "+fecha.ToShortTimeString();
        }
        else if (fecha.AddDays(7) > midnight)
        {
            return fecha.ToString("dddd") + " " + fecha.ToShortTimeString();
        }
        else
        {
            return fecha.ToString();
        }
    }

    /* Convertir un nombre de carpeta IMAP a amistoso */
    public static string FolderNameFriendly (string name)
    {
        name = name.Replace("INBOX.", "");
        name = name.Replace("INBOX", "Entrada");
        return name;
    }

    /* Formatear el body de un reply de un email dado */
    /*		public static string ReplyFriendly (System.Net.Imap.Mail m)
    		{
    			string html;
    			html = "\"" + m.To + "\" escribio lo siguiente el ";
    			html += DateFriendly (m.Date);
    			html += ":<br />";
    			html += "<div style=\"margin-left: 5px; padding-left: 5px; border-left: 2px solid blue; \">";
    			// FIXME: esto hay que chequearlo un poco antes de meterlo
    			html += m.Body;
    			html += "<br/></div>";
    			html += "[Escriba aqui su respuesta]";
    			return html;
    		}*/

    public static CastlePortalHashtable Hash(string s)
    {
        return new CastlePortalHashtable(s);
    }
}
}
