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
using System.Collections; //Hashtable

namespace CastlePortal
{
public class WebHelper:Castle.MonoRail.Framework.Helpers.AbstractHelper
{
    public string Name(User user)
    {
        return "<input name=\"Form.Name\" type=\"text\" value=\"" + user.Name + "\" />";
    }
    public string UserPassword(User user)
    {
        return "<input name=\"Form.UserPassword\" type=\"text\" value=\"" + user.UserPassword + "\" />";
    }
    /*public string MachinePassword(User user)
    {
    	return "<input name=\"Form.MachinePassword\" type=\"text\" value=\"" + user.MachinePassword + "\" />";
    }*/
    public string DefaultRoleId(Role [] roles)
    {
        return DefaultRoleId(roles, (Role) null);
    }
    public string DefaultRoleId(Role [] roles, Role rol_selected)
    {
        string salida = "<select name=\"DefaultRoleId\">";
        salida += "<option value=\"0\">ningun rol</option>";
        foreach (Role rol in roles)
        {
            if (rol_selected != null  && rol.Id == rol_selected.Id)
                salida += System.String.Format("<option value=\"{0}\" selected>{1}</option>", rol.Id, rol.Name);
            else
                salida += System.String.Format("<option value=\"{0}\">{1}</option>", rol.Id, rol.Name);
        }
        salida += "</select>";
        return salida;
    }
    public string GroupId(Group [] groups)
    {
        return GroupId(groups, (Group []) null);
    }
    public string GroupId(Group [] groups, System.Collections.IList groups_selected)
    {
        string salida = "";
        foreach (Group group in groups)
        {
            if (groups_selected != null && groups_selected.Contains(group))
                salida += System.String.Format("{1}: <input type=\"checkbox\" NAME=\"Group{0}[{1}]\" VALUE=\"Yes\" checked>", group.Id, group.Name);
            else
                salida += System.String.Format("{1}: <input type=\"checkbox\" NAME=\"Group{0}[{1}]\" VALUE=\"Yes\">", group.Id, group.Name);
        }
        return salida;
    }
    public string Checkbox(string name)
    {
        return Checkbox(name, false);
    }
    public string Checkbox(string name, bool val)
    {
        if (val)
            return System.String.Format("<input type=\"checkbox\" name=\"{0}\" value=\"1\" checked />", name);
        else
            return System.String.Format("<input type=\"checkbox\" name=\"{0}\" value=\"1\" />", name);
    }

    public static string [] PermNames = new string []{"Create", "Modify", "Delete", "Publish", "Read"};

    public string Permissions()
    {
        return Permissions((Role)null);
    }
    /*public string Permissions(Role r)
    {
    	return Permissions((Role) r);
    }*/
    public string PermissionNames()
    {
        return "<td>Create</td><td>Modify</td><td>Delete</td><td>Publish</td><td>Read</td>";
    }
    public string Permissions(Role r)
    {
        string salida = "";
        bool is_table = true;
        string ts = is_table ? "<td>" : "";
        string te = is_table ? "</td>" : "";

        if (r == null)
        {
            salida += ts + Checkbox("Form.CanCreate", false) + te;
            salida += ts + Checkbox("Form.CanModify", false) + te;
            salida += ts + Checkbox("Form.CanDelete", false) + te;
            salida += ts + Checkbox("Form.CanPublish", false) + te;
            salida += ts + Checkbox("Form.CanRead", false) + te;
        }
        else
        {
            salida += ts + Checkbox("Form.CanCreate", r.CanCreate) + te;
            salida += ts + Checkbox("Form.CanModify", r.CanModify) + te;
            salida += ts + Checkbox("Form.CanDelete", r.CanDelete) + te;
            salida += ts + Checkbox("Form.CanPublish", r.CanPublish) + te;
            salida += ts + Checkbox("Form.CanRead", r.CanRead) + te;
        }
        return salida;
    }
    public string GroupNames(User u)
    {
        string salida = "<ul>";
        foreach (Group g in u.Groups)
        salida += "<li>"+g.Name+"</li>";
        salida += "</ul>";
        foreach (Group g in u.Groups)
        salida += " <input type=\"hidden\" name=\"GroupIds\" value=\""+g.Id+"\" />";
        return salida;
    }
    /*public string Groups(User u)
    {
    	return Groups(u, Group.FindAll());
    }*/
    public string Groups(User u, Group [] gg)
    {
        string salida = "";
        foreach (Group g in gg)
        {
            //string selected = u.Groups.Contains(g) ? "checked" : ""; // sería lo optimo pero no funciona :-(((  a lo cutre...
            string selected = "";
            if (u!=null)
                foreach (Group gx in u.Groups)
                if (gx.Id == g.Id)
                {
                    selected = "checked";
                    break;
                }
            //System.Console.WriteLine("mostrando group enabled {0}", g.Name);
            salida += System.String.Format("{0}:&nbsp;<input type=\"checkbox\" NAME=\"GroupIds\" VALUE=\"{1}\" {2}/> ", g.Name, g.Id, selected);
        }
        return salida;
    }

    /* este select no se puede sustituir por un createoptionswitharray porque
     * se usa un arraylist, no un array[] */
    public string SessionGroup(User u)
    {
        string salida = "<select name=\"SessionGroupId\">";
        salida += "<option value=\"0\">ningun grupo</option>";
        if (u != null && u.Groups != null)
            foreach (Group g in u.Groups)
        {
            string selected = (u.SessionGroup != null && u.SessionGroup.Id == g.Id) ? "selected" : "";
            salida += System.String.Format("<option value=\"{0}\" {2}>{1}</option>", g.Id, g.Name, selected);
        }
        salida += "</select>";
        return salida;
    }

}
}
