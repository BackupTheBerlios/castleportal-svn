// Authors:
//    Carlos Ble <carlosble@shidix.com>
//    Alberto Morales <amd77@gulic.org>
//    Héctor Rojas González <hectorrojas@shidix.com>
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

	/**********************************************************************************/
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

/*
		public TreeAbstract (Menu givenMenu, Category currentCategory, Menu actualMenu) {
			_givenMenu = givenMenu;
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;

			System.Console.WriteLine ("escribo desde la clase" + this);
		}
*/

//		public abstract string BuildTree (string language, int depthLimit, string styleClass);
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

			if (!this.currentCategory.AnonRole.Can(Permission.Read))
				return false;

			if ((this.givenMenu.Children == null) || (this.givenMenu.Children.Count <= 0))
				return false;

			return true;
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

		protected bool CheckMenuAccess () {
			bool hasPermission = false;

			Hashtable menuPermissionsHash;
			if (this.givenMenu.CategoryId > 0)
			{
            Category category = Category.Find(this.givenMenu.CategoryId);
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
				tree += Divider();

				foreach (Menu menu in givenMenu.Children)
				{
					if (CheckMenuAccess())
					{
						if (Selected(menu))
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
						if (Selected(m))
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
	/**********************************************************************************/
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
	public class Tree: TreeAbstract
	{
		public Tree (Menu givenMenu, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = givenMenu;
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		public Tree (string menuName, Category currentCategory, Menu actualMenu, User user) {
			_givenMenu = Menu.FindByName (menuName);
			_currentCategory = currentCategory;
			_actualMenu = actualMenu;
			_user = user;
		}

		//public override string BuildTree(string language, int depthLimit, string styleClass) {return String.Empty;}
		public override string Header() {return "<ul class=\"" + this.styleClass + "_ul_" + "\">";}
		public override string Divider() {return "<li class=\"" + this.styleClass + "_li_" + "\">" + this.separator + "</li>";}
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

        html += "<li class=\"" + this.styleClass + "_li_current_" + this.actualDepth + "\">";
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

		//public override string BuildTree(string language, int depthLimit, string styleClass) {return String.Empty;}
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
	public string BuildTree (Menu givenMenu, Menu actualMenu, Category currentCategory, int depth_level, string currentLanguage, string styleClass)
	{
      User user = (User) Controller.Context.Session[Constants.USER];
		Tree tree = new Tree (givenMenu, currentCategory, actualMenu, user);

		return tree.BuildTree(currentLanguage, depth_level, styleClass);
	}
	public string BuildTree (string givenMenu, Menu actualMenu, Category currentCategory, int depth_level, string currentLanguage, string styleClass)
	{
      User user = (User) Controller.Context.Session[Constants.USER];
		Tree tree = new Tree (givenMenu, currentCategory, actualMenu, user);

		return tree.BuildTree(currentLanguage, depth_level, styleClass);
	}
	//			BuildTrees recauchutados

}
}
