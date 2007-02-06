// Authors:
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

using System;
using System.IO;
using System.Xml;
using System.Collections;
using Castle.ActiveRecord;
using NHibernate;

namespace CastlePortal
{

public static class Constants
{
    public const string ACCESIBLE = "accesible";
    public const string ACLS = "acls";
    public const string ADMIN_GROUP = "ADMIN_GROUP";
    public const string ADMIN_GROUP_NAME = "Admins";
    public const string ALL_LINGUAS = "all_linguas";
    public const string ALL_PRIVILEDGES = "ALL_PRIVILEDGES";
    
    public const string CATEGORIES = "categories";
    public const string CATEGORIES_ACLS = "aclcat";
    public const string CATEGORY_NOT_FOUND = "givenCategoryNotFound";
    public const string CATEGORY = "category";
    public const string COLOR = "color";
    public const string COLORS_AVAILABLE= "colors";
    public const string CONTENT = "content";
    public const string CONTENT_NOT_FOUND = "givenContentNotFound";

    public const string DEFAULT_LIST_TEMPLATE = "defaultList.vm";
    public const string DEFAULT_VIEW_TEMPLATE = "defaultView.vm";
    public const string DEFAULT_EDIT_TEMPLATE = "defaultEdit.vm";
    public const string DESCRIPTION = "description";
    
    public const string ELEARNING_LAYOUT = "ELEARNING_LAYOUT";
    public const string EXTENSION = "EXTENSION"; 
    
    public const string FIELD = "field";
    public const string FIELDS = "fields";
    public const string FRONTPAGE = "FRONTPAGE";

    public const string HEADER = "header";
    public const string HEADERS_AVAILABLE = "headers";

    public const string INDEX = "index";
    public const string IS_ADMIN = "isAdmin";
    public const string IS_ROOT = "isRoot";
    
    public const string LANG = "lang";
    public const string LANGUAGES = "languages";
    public const string LANGUAGE_NAME = "name";
    public const string LANGUAGE_ENGLISHNAME  = "englishname";

    public const string LAST_URL = "lasturl";
    public const string LAYOUT = "LAYOUT";
    public const string LAYOUT_LOWER = "layout";
    public const string LAYOUTS_AVAILABLE= "layouts";
    public const string LOCALE_COOKIE = "locale";
    public const string LOGIN_LAYOUT = "LOGIN_LAYOUT";
    public const string LOGIN_CONTROLLER = "login";
    public const string LOGIN_SUB = "_login";
    public const string LOGGED_COOKIE = "logged";
    
    public const string MEDIA_FOLDER = "MEDIA_FOLDER" ;
    public const string MENU = "menu";
    public const string MENUS_ACLS = "aclmenu";

    public const string MENU_TRANSLATION_LANG = "lang";
    public const string MENU_TRANSLATION_CODE = "code";
    public const string MENU_TRANSLATION = "translation";
 
    public const string NAMESPACE = "CastlePortal";
    public const string NO_TRANSLATION_FOUND = "No translation found";

    public const string PARENT = "parent";
    public const string PERMISSION_READ = "PermissionRead";
    public const string PERMISSION_CREATE = "PermissionCreate";
    public const string PERMISSION_MODIFY = "PermissionModify";
    public const string PERMISSION_DELETE = "PermissionDelete";
    public const string PERMISSION_PUBLISH = "PermissionPublish";
    public const string PORTAL = "portal";
    public const string PORTAL_CONTROLLER = "portal";
    public const string PRIVATE_FOLDER = "PRIVATE_FOLDER";
    public const string PUBLISHED = "published";
    
    public const string ROLES = "roles";
   
    public const char   SEPARATOR = ':' ; 
    public const string SITE_NAME = "SITE_NAME";
    public const string SITE_ROOT = "SITE_ROOT";
    public const string SUPER_USER = "root";

    public const string TEMPLATES = "templates";
    public const string TEMPLATES_FOLDER = "Views/general_templates/";
    public const string TITLE = "title";
    public const string TOP = "Top";
    public const string TYPES = "types";
    
    public const string USER = "User";
    public const string USERS = "users";

    public const string VIEW_PORTAL_CATEGORY = "/portal/viewcategory";

    public const string WEBSITE_UNAVAILABLE= "<div style='text-align: center;'> Web site unavailable, sorry<br> </div>";
}

}
