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

using System;
using System.Collections;
using Castle.MonoRail.Framework;

namespace CastlePortal
{
public class MenuHelper:Castle.MonoRail.Framework.Helpers.AbstractHelper
{
    static ConfigManager config = ConfigManager.GetInstance();

    private bool CanRead(int menuid, ref IDictionary acl)
    {
        IDictionary a = (IDictionary) acl[menuid];

        // no es una category => se ve.
        if (a == null)
            return true;

        // miramos si se ve o no
        if ((bool) a[Permission.Read])
            return true;
        else
            return false;
    }

    public Menu GetMenu(string s)
    {
        return Menu.FindByName(s);
    }

    public Category GetCategory (string s)
    {
        return Category.FindByName(s);
    }

    /*
     *--------------------------------------------------------------------------------------------
     * 							FUNCIONES SIN AJAX
     *--------------------------------------------------------------------------------------------	
     */

    //-----------------------------------------------------------------------------------------------
    // 							ARBOLES DE MENU
    //-----------------------------------------------------------------------------------------------

    /// <summary>
    /// Replaces the old BuildTree velocity macro
    /// </summary>
    public string BuildTree (string menuName, Category currentCategory,
                             string currentLanguage, string divName)
    {
        string div = String.Empty;
        //Category currentCategory = Category.Find(int.Parse(currentCategoryId));
        //Console.WriteLine("------- :" + menuName +","+ currentCategory.Name);
        Menu givenMenu = Menu.FindByName(menuName);
        if (givenMenu == null)
        {
            return String.Empty;
        }
        //Console.WriteLine("--- 2");
        Hashtable menusAcl = Controller.Context.Session[Constants.MENUS_ACLS] as Hashtable;
        if ((menusAcl == null) && (currentCategory != null) && (!currentCategory.AnonRole.Can(Permission.Read)))
        {
            return String.Empty;
        }
        //Console.WriteLine("--- 3");
        if (givenMenu.Children == null)
        {
            return String.Empty;
        }
        //Console.WriteLine("--- 4");
        foreach (Menu menu in givenMenu.Children)
        {
            bool hasPermission = false;
            if (menusAcl == null)   // perhaps anonymous user
            {
                //if (givenMenu.GetCategory().AnonRole.Can(Permission.Read)) // FIXME: extremely performance penalty
                hasPermission = true;
            }
            else
            {
                Hashtable menuPermissionsHash = menusAcl[menu.Id] as Hashtable;
                if (menuPermissionsHash != null)
                {
                    // Permissions check: (is working fine)
                    Object permission = menuPermissionsHash[Permission.Read];
                    if (permission == null)
                    {
                        return String.Empty;
                    }
                    bool readPermission = (bool) permission;
                    if (!readPermission)
                    {
                        return String.Empty;
                    }
                    hasPermission = true;
                }
            }
            if (hasPermission)
            {
            //Console.WriteLine("--- 5");
                div += "<DIV class=" + divName +">";
                if ((currentCategory != null) && (menu.CategoryId == currentCategory.Id))
                {
                    //div += "<SPAN>" + menu.FindTranslation(currentLanguage) + "</SPAN>"; FIXME: Big performance penalty
                    div += "<SPAN>" + menu.Description + "</SPAN>";
                }
                else
                {
                    div += "<A href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
                    div += " title=\"" + menu.Name +"\">"+ menu.Description; // menu.FindTranslation(currentLanguage); FIXME Big performance penalty
                    div += "</A>";
                }
                div += "</DIV>";
            }
            //Console.WriteLine("--- 6");
        }
        return div;
    }


    /*
     *  Construye el arbol completo
     */
    public string BuildTree (string s, string div, IDictionary acl)
    {
        Menu m = Menu.FindByName(s);
        if (m == null)
            return "Lo siento, no existe el menu "+s;

        Menu [] mm = Menu.FindAll();
        return BuildTree(m, ref mm, div, ref acl);
    }

    /*
     *  Construye el primer nivel de un arbol 
     */
    public string BuildTree (string s, string div, string separator, IDictionary acl)
    {
        Menu m = Menu.FindByName(s);
        if (m == null)
            return "Lo siento, no existe el menu "+s;

        Menu [] mm = Menu.FindByParent(m.Id);
        return BuildTree(m, ref mm, div, separator, ref acl);
    }

    /*
     *  Construye el arbol completo
     */
    public string BuildTree (Menu x, ref Menu [] mm, string div, ref IDictionary acl)
    {
        string html = "";
        if (x == null)
            return "Lo siento, no hay menus aún.";

        //if (!CanRead(x.Id, ref acl)) return "No tienes permiso";

        foreach (Menu m in mm)
        {
            if (!CanRead(m.Id, ref acl))
                continue;

            if (m.Parent != null && m.Parent.Id == x.Id)
            {
                html += "<div class='"+div+"'>";
                html += "<a href='/menu/red.html?id="+m.Id+"' title='" + m.Name +"'>" + m.Name + "</a>";
                html += BuildSubTree(m, ref mm, div, ref acl);
                html += "</div>";
            }
        }
        return html;
    }

    /*
     *  Construye el primer nivel de un arbol 
     */
    public string BuildTree (Menu x, ref Menu [] mm, string div, string separator, ref IDictionary acl)
    {
        string html = "";
        if (x == null)
            return "Lo siento, no hay menus aún.";

        //if (!CanRead(x.Id, ref acl)) return "No tienes permiso";

        //System.Console.Write("BuildTree(): entrando, ");
        //PublicAPI.Show((Hashtable) acl);

        foreach (Menu m in mm)
        {
            //System.Console.Write(m.Name+ " ("+m.Id+") ");
            if (!CanRead(m.Id, ref acl))
            {
                //System.Console.Write("NO, ");
                continue;
            }
            else
            {
                //System.Console.Write("SI, ");
            }


            if (m.Parent != null && m.Parent.Id == x.Id)
            {
                html += "<div class='"+div+"'>";
                html += "<a href='/menu/red.html?id="+m.Id+"' title='" + m.Name +"'>" + m.Name + "</a>";
                html += separator;
                html += "</div>";
            }
        }
        //System.Console.WriteLine("saliendo");
        return html;
    }

    public string BuildSubTree (string s, string div, IDictionary acl)
    {
        Menu m = Menu.FindByName(s);
        if (m == null)
            return "Lo siento, no existe el menu "+s;

        //if (!CanRead(m.Id, ref acl)) return "No tienes permiso";

        Menu [] menus = Menu.FindByParent(m.Id);
        Menu auxMenu = null;
        foreach (Menu menu in menus)
        if (menu.Show == 1)
        {
            auxMenu = menu;
            break;
        }
        Menu [] mm = Menu.FindAll();
        return BuildSubTree(auxMenu, ref mm, div, ref acl);
    }

    private string BuildSubTree (Menu x, ref Menu [] mm, string div, ref IDictionary acl)
    {
        string html = "";
        if (x == null)
            return html;

        //if (!CanRead(x.Id, ref acl)) return "No tienes permiso";

        foreach (Menu m in mm)
        {
            if (!CanRead(m.Id, ref acl))
                continue;

            if (m.Parent != null && m.Parent.Id == x.Id && x.Show == 1)
            {
                html += "<div class='"+div+"' style='padding-left:20px'>";
                html += "<a href='/menu/red.html?id="+m.Id+"' title='" + m.Name +"'>" + m.Name + "</a>";
                html += BuildSubTree (m, ref mm, div, ref acl);
                html += "</div>";
            }
        }
        return html;
    }

    /*
     *-------------------------------------------------------------------------------------------------------
     * 								FUNCIONES CON AJAX
     *-------------------------------------------------------------------------------------------------------	
     */
    public string BuildMenu (string currentName)
    {
        Menu current = Menu.FindByName(currentName);
        if (current == null)
            return "Lo siento, no existe el menu "+currentName;

        string html = "";
        Menu[] menus = Menu.FindByParent(current.Id);
        foreach (Menu m in menus)
        if (m.Parent != null)
            if (m.Parent.Id == current.Id)
            {
                html += "<div class=\"list\">";
                html += "<img src='/Public/images/plus.gif' id=\"img"+m.Id+"\" onclick=\"javascript:reloadSubmenu("+m.Id+", 'submenu"+m.Id+"', '/menu/submenu.html', '')\"/>";
                html += "<a href='javascript:void(0)' onclick='javascript:reloadContent(\"content\",\""+m.Url+"\")'>"+m.Name+"</a>";
                html += "<div id=\"submenu"+m.Id+"\" style=\"padding-left: 50px;\"></div></div>";
            }
        return html;
    }

    public string BuildMenu (string currentName, string div, bool vertical, string separator)
    {
        Menu current = Menu.FindByName(currentName);
        if (current == null)
            return "Lo siento, no existe el menu "+currentName;

        string html = "";
        Menu[] menus = Menu.FindByParent(current.Id);
        foreach (Menu m in menus)
        if (m.Parent != null)
            if (m.Parent.Id == current.Id)
            {
                //html += "<div id=\"MyMenu"+m.Id+"\">";
                html += "<a href='javascript:void(0)' onclick='javascript:reloadMenu("+m.Id+", \""+div+"\", \"http://localhost:8080/menu/submenu.html\")'>" + m.Name + "</a>";
                if (vertical)
                    html += "<br/>";
                else
                    html += separator;
                //html += "</div>";
            }
        return html;
    }

    public string BuildCategoryMenu (string currentName)
    {
        Category current = Category.FindByName(currentName);
        if (current == null)
            return "Lo siento, no existe el menu "+currentName;

        string html = "";
        Category[] categories = Category.FindByParent(current.Id);
        //System.Console.WriteLine ("LLEGO"+current.Id);
        foreach (Category c in categories)
        if (c.Parent != null)
            if (c.Parent.Id == current.Id)
            {
                html += "<div class=\"list\">";
                html += "<img src='/Public/images/plus.gif' id=\"img"+c.Id+"\" onclick=\"javascript:reloadSubmenu("+c.Id+", 'subcategory"+c.Id+"', '/portal/subcategory.html', '')\"/>";
                html += "<a href='javascript:void(0)' onclick='javascript:reloadContent(\"content\",\""+c.ToUrl()+"\")'>"+c.Name+"</a>";
                html += "<div id=\"subcategory"+c.Id+"\" style=\"padding-left: 50px;\"></div></div>";
            }
        return html;
    }

    public string BuildCategoryMenu (string currentName, string div, bool vertical, string separator)
    {
        Category current = Category.FindByName(currentName);
        if (current == null)
            return "Lo siento, no existe el menu "+currentName;

        string html = "";
        Category[] categories = Category.FindByParent(current.Id);
        foreach (Category c in categories)
        if (c.Parent != null)
            if (c.Parent.Id == current.Id)
            {
                //html += "<div id=\"MyCategory"+m.Id+"\">";
                html += "<a href='javascript:void(0)' onclick='javascript:reloadMenu("+c.Id+", \""+div+"\", \"http://localhost:8080/portal/subcategory.html\");reloadContent(\"content\",\""+c.ToUrl()+"\");'>" + c.Name + "</a>";
                if (vertical)
                    html += "<br/>";
                else
                    html += separator;
                //html += "</div>";
            }
        return html;
    }
}
}
