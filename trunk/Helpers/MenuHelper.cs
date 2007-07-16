// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Alberto Morales <amd77@gulic.org>
//    Héctor Rojas González <hectorrojas@shidix.com>
//    Inocencio del Castillo Suárez <chencho@shidix.com>
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
/*
    public Menu GetMenu(string s)
    {
        return Menu.FindByName(s);
    }

    public Category GetCategory (string s)
    {
        return Category.FindByName(s);
    }
*/

	  /*********************************************************************/
	 /******************			Classes Abstractas			*****************/
	/*********************************************************************/
	public abstract class TreeAbstract
	{
		protected Menu			_givenMenu;
		protected Menu			_actualMenu;
		protected Category	_currentCategory;
		protected User			_user;
		protected int			_depthLimit;
		protected int			_numChilds;
		protected int			_actualDepth = 1;
		protected string		_separator = String.Empty;
		protected string		_styleClass = String.Empty;
		protected string		_language = String.Empty;

		public abstract string Header ();
		public abstract string Divider ();
		public abstract string Footer ();
		public abstract string PrintItem (Menu menu, string item);
		public abstract string PrintSelectedItem (Menu menu, string item);

		public Menu givenMenu {
			get {return _givenMenu;}
			set {_givenMenu = value;}
		}
		public Menu actualMenu {
			get {return _actualMenu;}
			set {_actualMenu = value;}
		}
		public Category currentCategory {
			get {return _currentCategory;}
			set {_currentCategory = value;}
		}
		public User user {
			get {return _user;}
			set {_user = value;}
		}
		public int depthLimit {
			get {return _depthLimit;}
			set {_depthLimit = value;}
		}
		public int actualDepth {
			get {return _actualDepth;}
			set {_actualDepth = value;}
		}
		public string styleClass {
			get {return _styleClass;}
			set {_styleClass = value;}
		}
		public string separator {
			get {return _separator;}
			set {_separator = value;}
		}
		public string language {
			get {return _language;}
			set {_language = value;}
		}
		public int numChilds {
			get {return _numChilds;}
			set {_numChilds = value;}
		}

		protected bool CanBuildTree () {
			if (this.givenMenu == null)
				return false;

			if (this.currentCategory == null)
				return false;

			if ((this.givenMenu.Children == null) || (this.givenMenu.Children.Count <= 0))
				return false;

			return true;
		}
	
		protected string GetDescription (Menu menu)
		{
			MenuTranslation mt = MenuTranslation.FindByMenuAndLang(menu, Language.FindByName(this.language));
			string description;

			if ((mt != null) && (mt.Translation.Length > 0))
				description = mt.Translation;
			else
				description = menu.Description;

			return description;
		}

		private bool CheckMenuPermission(Menu menu)
		{
			bool hasPermission = false;

			Hashtable menuPermissionsHash;
			if (menu.CategoryId > 0)
			{
				Category category = Category.Find(menu.CategoryId);
				menuPermissionsHash = category.GetPermissionsHash(this.user);
			}
			else
				menuPermissionsHash = Commons.GetPermissionsBaseHash(true);

			if (menuPermissionsHash != null)
			{
				// Permissions check: (is working fine)
				Object permission = menuPermissionsHash[Permission.Read];
				if ((permission != null) && ((bool)permission))
					hasPermission = true;
			}

			return hasPermission;
		}

		protected bool Selected (Menu menu)
		{
			bool isAntecessor = false;
	
			if ((menu != null) && (this.actualMenu != null)) 
			{
				for (Menu m = this.actualMenu; m.Parent != null; m = m.Parent)
				{
					if (m.Id == menu.Id) 
					{
						isAntecessor = true;
						break;
					}
				}
			}

			if (((this.currentCategory != null) && (menu.CategoryId == this.currentCategory.Id)) || (isAntecessor == true))
				return true;
			return false;
		}

		public string BuildTree(string language, int depthLimit, string styleClass)
		{
			if (depthLimit > 0)
			{
				if (!CanBuildTree())
					return String.Empty;

				this.depthLimit = depthLimit;
				this.language = language;
				this.styleClass = styleClass;
				this.numChilds = this.givenMenu.Children.Count;

				string tree = Header();

				foreach (Menu menu in givenMenu.Children)
				{
					if (CheckMenuPermission(menu))
					{
						if ((Selected(menu)) || (this.actualMenu.Code == "showAllChilds"))
						{
							tree += PrintSelectedItem(menu, GetDescription(menu));
							tree += BuildSubTree (menu);
						}
						else
							tree += PrintItem(menu, GetDescription(menu));
					}
					
					tree += Divider();
				}
				tree += Footer();
				return tree;
			} else
				return String.Empty;
		}


		public string BuildSubTree (Menu menu)
		{
			this.actualDepth++;
			if (this.actualDepth <= this.depthLimit) {
				if (menu.Children.Count > 0)
				{
					String subtree = String.Empty;
					foreach (Menu m in menu.Children) 
					{
						if ((Selected(m)) || (this.actualMenu.Code == "showAllChilds"))
						{
							subtree += PrintSelectedItem (m, GetDescription(m));
							subtree += BuildSubTree(m);
						}
						else
							subtree += PrintItem (m, GetDescription(m));
					}
					this.actualDepth--;
					return subtree;
				}
			}
			
			this.actualDepth--;
			return String.Empty;
		}
	}

	/**********************************************************************************/
	public abstract class SimpleMenuAbstract
	{
		protected string	_separator = String.Empty;
		protected string	_styleClass = String.Empty;
		protected string	_language = String.Empty;
		protected bool		_drawIcons = true;

		public abstract string Header ();
		public abstract string Divider ();
		public abstract string Footer ();
		public abstract string PrintItem (string code);

		public string styleClass {
			get {return _styleClass;}
			set {_styleClass = value;}
		}
		public string separator {
			get {return _separator;}
			set {_separator = value;}
		}
		public string language {
			get {return _language;}
			set {_language = value;}
		}
		public bool drawIcons {
			get {return _drawIcons;}
			set {_drawIcons = value;}
		}

		protected string GetDescription (Menu menu)
		{
			MenuTranslation mt = MenuTranslation.FindByMenuAndLang(menu, Language.FindByName(this.language));
			string description;

			if (mt != null)
				description = mt.Translation;
			else
				description = menu.Description;

			return description;
		}

		protected string GetDescription (Category category)
		{
			CategoryTranslation ct = CategoryTranslation.FindByCategoryAndLang(category, Language.FindByName(this.language));
			string description;

			if (ct != null)
				description = ct.Translation;
			else
				description = category.Description;

			return description;
		}


		public string BuildMenu (params string []entries)
		{
			if (entries.Length > 0)
			{
				string html = Header();
				foreach (string menuItem in entries)
				{
					html += PrintItem(menuItem);
					html += Divider();
				}
					
				html += Footer();
				return html;
			} else
				return String.Empty;
		}
	}

	  /*********************************************************************/
	 /******************			Classes Herederas			********************/
	/*********************************************************************/
	public class TreeLeft: TreeAbstract
	{

		public TreeLeft (Menu givenMenu, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = givenMenu;
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public TreeLeft (string menuName, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = Menu.FindByName (menuName);
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public override string Header() {return "<ul class=\"" + this.styleClass + "_ul_" + this.actualDepth + "\">";}
		public override string Footer() {return "</ul>";}
		public override string Divider()
		{
			if (this.numChilds > 0)
			{
				this.numChilds--;
				return "<li class=\"" + this.styleClass + "_li_" + this.actualDepth + "\">" + this.separator + "</li>";
			}
			return String.Empty;
		}

		public override string PrintItem (Menu menu, string item)
		{
			string html = String.Empty;

			html += "<li class=\"" + this.styleClass + "_li_" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">" + item;
			html += "</a>";
			html += "</li>";
			return html;
		}

		public override string PrintSelectedItem (Menu menu, string item)
		{
			string html = String.Empty;

			html += "<li class=\"" + this.styleClass + "_li_current_" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">" + item;
			html += "</a>";
			html += "</li>";
			return html;
		}

	}

	/**********************************************************************************/
	/**********************************************************************************/
	public class TreeLeftWithVinyetas: TreeAbstract
	{

		public TreeLeftWithVinyetas (Menu givenMenu, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = givenMenu;
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public TreeLeftWithVinyetas (string menuName, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = Menu.FindByName (menuName);
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public override string Header() {return "<ul class=\"" + this.styleClass + "_ul_" + this.actualDepth + "\">";}
		public override string Footer() {return "</ul>";}
		public override string Divider()
		{
			if (this.numChilds > 0)
			{
				this.numChilds--;
				return "<li class=\"" + this.styleClass + "_li_" + this.actualDepth + "\">" + this.separator + "</li>";
			}
			return String.Empty;
		}

		public override string PrintItem (Menu menu, string item)
		{
			string html = String.Empty;

			html += "<li class=\"" + this.styleClass + "_li_" + this.actualDepth + "\">";
			html += "<table class=\"" + this.styleClass +"\">";
			html += "<td class=\"" + this.styleClass + "vinyeta" + this.actualDepth + "\">";
			if (this.actualDepth != 1)
				html += ">";						// Viñeta
			html += "</td>";
			html += "<td class=\"" + this.styleClass + "texto" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">" + item;
			html += "</a>";
			html += "</td>";
			html += "</table>";
			html += "</li>";
         html += "<img src=\"" + config.GetValue(Constants.SITE_ROOT) + "/Public/style/img/separamenu.gif\" class=\"" + this.styleClass + ">";
			return html;
		}

		public override string PrintSelectedItem (Menu menu, string item)
		{
			string html = String.Empty;

			html += "<li class=\"" + this.styleClass + "_li_current_" + this.actualDepth + "\">";
			html += "<table class=\"" + this.styleClass + "\">";
			html += "<td class=\"" + this.styleClass + "vinyeta" + this.actualDepth + "\">";
			if (this.actualDepth != 1)
				html += ">";						// Viñeta
			html += "</td>";
			html += "<td class=\"" + this.styleClass + "texto" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">" + item;
			html += "</a>";
			html += "</td>";
			html += "</table>";
			html += "</li>";
         html += "<img src=\"" + config.GetValue(Constants.SITE_ROOT) + "/Public/style/img/separamenu.gif\" class=\"" + this.styleClass + ">";
			return html;
		}

	}

	/**********************************************************************************/
	/**********************************************************************************/
	public class Tree: TreeAbstract
	{
		public Tree (string menuName, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = Menu.FindByName (menuName);
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public override string Header() {return "<ul class=\"" + this.styleClass + "_ul_" + "\">";}
		public override string Divider() {return String.Empty;}
		public override string Footer() {return "</ul>";}

		public override string PrintItem (Menu menu, string item)
		{
			string html = String.Empty;

			html += "<li class=\"" + this.styleClass + "_li_" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">"+ item;
			html += "</a>";
			html += "</li>";
			return html;
		}

		public override string PrintSelectedItem (Menu menu, string item)
		{
			string html = String.Empty;

			html += "<li class=\"" + this.styleClass + "_li_" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">"+ item;
			html += "</a>";
			html += "</li>";
			return html;
		}

	}

	/**********************************************************************************/
	/**********************************************************************************/
	public class TreeTable: TreeAbstract
	{
		public TreeTable (Menu givenMenu, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = givenMenu;
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public TreeTable (string menuName, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = Menu.FindByName (menuName);
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public override string Header() {return "<table class=\"" + this.styleClass + "_table_" + "\"> <tr valign=\"center\">";}
		public override string Divider() {return "<td class=\"" + this.styleClass + "_td_" + "\">" + this.separator + "</td>";}
		public override string Footer() {return "</tr> </table>";}

		public override string PrintItem (Menu menu, string item)
		{
			string html = String.Empty;

			html += "<td class=\"" + this.styleClass + "_td_" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">"+ item;
			html += "</a>";
			html += "</td>";
			return html;
		}

		public override string PrintSelectedItem (Menu menu, string item)
		{
			string html = String.Empty;
			html += "<td class=\"" + this.styleClass + "_td_current_" + this.actualDepth + "\">";
			html += "<a href=" + menu.ToUrl(config.GetValue(Constants.SITE_ROOT));
			html += " title=\"" + menu.Name +"\">"+ item;
			html += "</a>";
			html += "</td>";
			return html;
		}

	}

	/**********************************************************************************/
	/**********************************************************************************/
	public class SimpleMenuByCode: SimpleMenuAbstract
	{
		public SimpleMenuByCode (string style, string language) {
			_styleClass = style;
			_language = language;
			this.separator = "&nbsp;&nbsp;&nbsp;";
		}

		public override string Header() {return "";}
		public override string Divider() {return this.separator;}
		public override string Footer() {return "";}

		public override string PrintItem (string code)
		{
			Category cat = Category.FindByCode(code); 
			if (cat == null) {
				System.Console.WriteLine("SimpleMenu no puede crear el enlace porque no existe la categoría con código '" + code + "'");
				return String.Empty;
			}

			string url = cat.ToUrl(config.GetValue(Constants.SITE_ROOT));
			string html = String.Empty;

			if (drawIcons)
				html += "<img src=\"" + config.GetValue(Constants.SITE_ROOT) + "/Public/style/img/" + code + ".gif\" class=\"" + this.styleClass + "\" >";
			html += "<a class=\"" + this.styleClass + "\" href=\"" + url + "\">";
			html += GetDescription(cat);
			html += "</a>";
			return html;
		}
	}

	//===============================================================================

	static ConfigManager config = ConfigManager.GetInstance();

	//			BuildTreesLefts recauchutados
	public string BuildTreeLeft (Menu givenMenu, Menu actualMenu,  Category currentCategory, int depth_level, string currentLanguage, string styleClass)
	{
      User user = (User) Controller.Context.Session[Constants.USER];		
		TreeLeft tl = new TreeLeft (givenMenu, currentCategory, actualMenu, user);

		return tl.BuildTree(currentLanguage, depth_level, styleClass);
	}

	public string BuildTreeLeft (string givenMenu, Menu actualMenu, Category currentCategory, int depth_level, string currentLanguage, string styleClass)
	{
      User user = (User) Controller.Context.Session[Constants.USER];
		TreeLeft tl = new TreeLeft (givenMenu, currentCategory, actualMenu, user);

		return tl.BuildTree(currentLanguage, depth_level, styleClass);
	}

	public string BuildTreeMapaWeb (string givenMenu, Category currentCategory, string currentLanguage, string styleClass)
	{
		Menu menu = new Menu("", "", "showAllChilds", 0, "", null, null, 0);

      User user = (User) Controller.Context.Session[Constants.USER];
		Tree mw = new Tree (givenMenu, currentCategory, menu, user);

		return mw.BuildTree(currentLanguage, 999, styleClass);
	}

	public string MakeMenu(string givenMenu, Category currentCategory, string currentLanguage, string styleClass)
	{
		Menu menu = new Menu(); //("", "", "", 0, "", null, null, 0);

      User user = (User) Controller.Context.Session[Constants.USER];
		Tree mw = new Tree (givenMenu, currentCategory, menu, user);

		return mw.BuildTree(currentLanguage, 1, styleClass);
	}

	public string BuildTreeLeftWithVinyetas (string givenMenu, Menu actualMenu, Category currentCategory, int depth_level, string currentLanguage, string styleClass)
	{
      User user = (User) Controller.Context.Session[Constants.USER];
		TreeLeftWithVinyetas wv = new TreeLeftWithVinyetas (givenMenu, currentCategory, actualMenu, user);

		return wv.BuildTree(currentLanguage, depth_level, styleClass);
	}

	public string MenuSimple (string style, string language, bool icons, params string [] items)
	{
		SimpleMenuByCode sm = new SimpleMenuByCode (style, language);

		sm.drawIcons = icons;

		return sm.BuildMenu (items);
	}

	//			BuildTrees recauchutados

}
}
