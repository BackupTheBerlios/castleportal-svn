// Authors:
//    Carlos Ble <carlosble@shidix.com>
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
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Security;
using System.Security.Permissions;

namespace CastlePortal
{
public class XmlXsltHelper
{
    static public string Transform (string xmldata, string xsluri)
    {
        // FIXME: PONER TRY/CATCH PARA EVITAR MOSTRAR EXPLOSIONES DEL TRANSFORM
        //
        // CARGA LA HOJA DE ESTILOS
        System.Xml.Xsl.XslTransform xslt = new System.Xml.Xsl.XslTransform ();
        try
        {
            xslt.Load (System.AppDomain.CurrentDomain.BaseDirectory + "/Public/xml/" + xsluri);
        }
        catch (System.IO.FileNotFoundException)
        {
            System.Console.WriteLine ("No se ha encontrado el fichero {0}", xsluri);
            return "Template no encontrado";
        }

        // LEER ENTRADA
        System.Xml.XmlReader xrd;
        try
        {
            xrd = new XmlTextReader (new System.IO.StringReader (xmldata));
        }
        catch (System.ArgumentNullException)
        {
            return "[Contenido vacio (1)]";
        }

        // EJECUTAMOS Y PASAMOS A STRING
        XPathDocument xpd;
        try
        {
            xpd = new XPathDocument (xrd);
        }
        catch (System.Xml.XmlException ex)
        {
            return "[xml fuente incorrecto, error: " + ex.Message + "]";
        }
        XmlResolver xrs = new XmlSecureResolver (new XmlUrlResolver (), new PermissionSet (PermissionState.Unrestricted));
        XmlReader xr = xslt.Transform (xpd, null, xrs);
        xrd.Close ();

        // Output results to string
        XmlDocument xd = new XmlDocument ();
        xd.Load (xr);
        System.IO.StringWriter sw = new System.IO.StringWriter ();
        try
        {
            xd.Save (sw);
        }
        catch (System.InvalidOperationException ex)
        {
            return "[resultado incorrecto: "+ ex.Message +"]";
        }
        return sw.ToString ();
    }

    static public string GetXmlQuoted(string xmldata)
    {
        if (xmldata==null)
            return "[Contenido vacio (2)]";
        xmldata = xmldata.Replace("&", "&amp;");
        xmldata = xmldata.Replace("<", "&lt;");
        xmldata = xmldata.Replace(">", "&gt;");
        return xmldata;
    }

    static public string GetXml(string xmluri)
    {
        string x = System.AppDomain.CurrentDomain.BaseDirectory + "/Public/xml/" + xmluri;
        System.IO.StreamReader sr = new System.IO.StreamReader(x);
        string s = sr.ReadToEnd();
        sr.Close();
        return s;
    }
}
}
