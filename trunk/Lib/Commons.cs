// Authors:
//    Alberto Morales <amd77@gulic.org>
//    Carlos Ble <carlosble@shidix.com>
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

using Castle.Windsor;
using Castle.MonoRail.Framework;
using System;
using System.IO;
using System.Collections;
using System.Diagnostics; // Process
using NLog;

namespace CastlePortal
{

public class Commons
{
    public static void CheckSuperUser(IDictionary Session)
    {
        if ((!(bool)Session[Constants.IS_ADMIN]) && (!(bool)Session[Constants.IS_ROOT]))
            throw new Unauthorized("");
    }
    
    public static string Date2Postgres(DateTime d)
    {
        return (d.Month + "/" + d.Day + "/" + d.Year);
    }

    public static bool IsNumeric(object Expression)
    {
        bool isNum;
        double retNum;

        isNum = Double.TryParse(Convert.ToString(Expression),
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.NumberFormatInfo.InvariantInfo,
                                out retNum );
        return isNum;
    }

    public static string GetCurrentLang(Controller controller)
    {
        string lang = controller.Request.ReadCookie(Constants.LOCALE_COOKIE);
        if ((lang == null) || (lang.Length == 0))
        {
            lang = controller.Request.UserLanguages[0].Substring(0,2);
        }
        return lang;
    }
    
    public static Hashtable GetPermissionsBaseHash(bool x)
    {
        Hashtable h = new Hashtable();
        h[Permission.Create] = x;
        h[Permission.Read] = x;
        h[Permission.Modify] = x;
        h[Permission.Publish] = x;
        h[Permission.Delete] = x;
        return h;
    }

    public static Hashtable HashReadOnly()
    {
        Hashtable h = GetPermissionsBaseHash(false);
        h[Permission.Read] = true;
        return h;
    }

    public static void Show(Hashtable h)
    {
        System.Console.WriteLine("------------------------------------");
        System.Console.WriteLine("key\tcrea\tedit\tdele\tread\tpubl ");
        foreach (int id in h.Keys)
        {
            Hashtable a = (Hashtable) h[id];
            if (a == null)
                System.Console.WriteLine(id + "\ttiene un error grave");
            else
                System.Console.WriteLine(id + "\t" +
                                         a[Permission.Create] + "\t" +
                                         a[Permission.Modify] + "\t" +
                                         a[Permission.Delete] + "\t" +
                                         a[Permission.Read] + "\t" +
                                         a[Permission.Publish]
                                        );
        }
    }

    public static Hashtable File2Hashtable(string fileName)
    {
        Hashtable t = new Hashtable();
        StreamReader sr = new StreamReader(fileName);

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            char[] x = {'='};
            string[] ss = line.Split(x, 2);
            if (ss.Length == 2)
            {
                t[ss[0]] = ss[1];
            }
            else
            {
                //System.Console.WriteLine("File2Hashtable(). Linea desconocida: {0}", ss);
            }
        }
        sr.Close();
        return t;
    }

    static System.Security.Cryptography.MD5 hasher = System.Security.Cryptography.MD5.Create();

    public static string Hash(string value)
    {
        if (value.Length < 10)
        {
            byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] hashedBytes = hasher.ComputeHash(valueBytes);
            return System.Convert.ToBase64String(hashedBytes);
        }
        else
            return value;
    }

    public static string Ejecuta(string cmd, string args)
    {
        string s = "";
        Process p = new Process();
        ProcessStartInfo pStartInfo = new ProcessStartInfo(cmd);
        pStartInfo.Arguments = args;
        pStartInfo.UseShellExecute = false;
        pStartInfo.RedirectStandardOutput = true;
        pStartInfo.RedirectStandardError = true;
        p.StartInfo = pStartInfo;

        s += "Comenzando proceso<br>";
        s += "<pre>"+cmd+" "+args+"</pre>";
        p.Start();

        p.WaitForExit();
        s += "Standard output:<br>";
        s += "<pre>" + p.StandardOutput.ReadToEnd() + "</pre>";

        s += "Standard error:<br>";
        s += "<pre>"+ p.StandardError.ReadToEnd() + "</pre>";

        p.Close();
        s += "Done<br>";
        return s;
    }
}
}
