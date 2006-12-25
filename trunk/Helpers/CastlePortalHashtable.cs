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
using System.Runtime;
using System.IO;
using System;

//[Serializable()]
public class CastlePortalHashtable : Hashtable
{
    const string quote =  "'";
    const string quotequoted =  "\"\"";

    public CastlePortalHashtable (string s)
    {
        Clear();

        const string Formatkeyval= @"^(?<key>\w+)="+quote+@"(?<val>[^"+quote+@"]+)"+quote+" *(?<resto>.*)$";

        // Esto se debería poder hacer:
        // const string Formatkeyval= @"^(?<key>\w+)='(?<val>[^']+[^\\])' *(?<resto>.*)$";

        System.Console.WriteLine("regexp={0}", Formatkeyval);

        System.Text.RegularExpressions.Regex reformat= new System.Text.RegularExpressions.Regex (Formatkeyval);
        while (s!= "")
        {
            System.Text.RegularExpressions.Match m = reformat.Match (s);
            if (!m.Success)
            {
                System.Console.WriteLine("CastlePortalHashtable error, se ignora <<{0}>>", s);
                break;
            }
            string key = m.Groups["key"].Success ? (m.Groups["key"].Value) : null;
            string val = m.Groups["val"].Success ? (m.Groups["val"].Value) : null;
            val = val.Replace(quotequoted, quote);
            s = m.Groups["resto"].Success ? (m.Groups["resto"].Value) : null;
            Add(key, val);
        }
    }

    /*public string this[string key] {
    	get {
    		string s = (string)(base[key]);
    		//return s.Replace("\"\"", "'");
    		return s;
    	} 
    	set {
    		//base[key] = value.Replace("'", "\"\"");
    		base[key] = value;
    	}
          	}*/

    public string Serialize()
    {
        string x = "";
        foreach(string key in Keys)
        {
            string val = (string)(this[key]);
            val = val.Replace(quote, quotequoted);
            x += key+"="+quote+val+quote+" ";
        }
        return x;
    }

    /*public static void Main()
    {
    	// Esto se deberia poder hacer
    	// string s = "hola='que' adios='dani\\'s place'";
    	
    	string s = "hola='que' adios='dani"+quotequoted+"s place'";
    	System.Console.WriteLine("entrada del sistema: "+ s);
    	CastlePortalHashtable x = new CastlePortalHashtable(s);
    	x["quetal"] = "capullito";
    	x["hola"] = "mujer'iego";
    	System.Console.WriteLine("salida del sistema: "+ x.Serialize() );

    	foreach (string k in x.Keys) 
    		System.Console.WriteLine("x[{0}] = {1}", k, x[k]);

    }*/
}
