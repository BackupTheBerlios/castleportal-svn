// Authors:
//    Zebenzui Perez <zebenperez@shidix.com>
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
using System.IO;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using NHibernate.Expression;

namespace CastlePortal
{
	[Layout ("general")]
	[Helper (typeof (MenuHelper))]
	//[Filter(ExecuteEnum.BeforeAction, typeof(CheckAuthenticationFilter))]
	//[Filter(ExecuteEnum.BeforeAction, typeof(CheckGroupFilter))]
	[Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
	[Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
	[DefaultAction("Redir")]
	public class BackupController:Castle.MonoRail.ActiveRecordSupport.ARSmartDispatcherController
	{
		// Funcion para redirigir al index del portal en caso de que se haya escrito algun metodo inexistente 
		// del controlador
		public void Redir()
		{
			Response.Redirect("portal" , "index"); 
		}

		public void Admin ()
		{
		}
		
		/*_____________________ CREATES _______________________*/
		public string CreateTypes ()
		{
			string tmp = "\n<!--________________ Tipos Base _______________ -->\n";
			tmp += "<BaseTypes>\n";
			Type[] types = Type.FindAll();
			foreach (Type t in types)
				tmp += "\t<Type nombrecorto=\""+t.Name+"\" nombrelargo=\""+t.Description+"\" />\n";

			tmp += "</BaseTypes>";

			return tmp;
		}
			
		public string CreateConfigCombos ()
		{
			string tmp = "\n<!--________________ Configuraciones Combos _______________ -->\n";
			tmp += "<ConfigCombos>\n";
			ConfigCombo[] configcombos = ConfigCombo.FindAll();
			foreach (ConfigCombo c in configcombos)
				tmp += "\t<ConfigCombo key=\""+c.Key+"\" val=\""+c.Val+"\" nombre=\""+c.Name+"\" />\n";

			tmp += "</ConfigCombos>\n";

			return tmp;
		}

		public string CreateConfigs ()
		{
			string tmp = "\n<!--________________ Configuraciones _______________ -->\n";
			tmp += "<Configs>\n";
			ConfigModel[] configs = ConfigModel.FindAll();
			foreach (ConfigModel c in configs)
				tmp += "\t<Config key=\""+c.Key+"\" val=\""+c.Val+"\" />\n";

			tmp += "</Configs>\n";

			return tmp;
		}

		public string CreateRoles()
		{
			string tmp = "\n<!--________________ Roles _______________ -->\n";
			tmp += "<Roles>\n";
			Role[] roles = Role.FindAll();
			foreach (Role c in roles)
				tmp += "\t<Role  nombrecorto=\""+c.Name+"\" nombrelargo=\""+c.Name+"\" create=\""+c.CanCreate+"\" modify=\""+c.CanModify+"\" delete=\""+c.CanDelete+"\" publish=\""+c.CanPublish+"\" read=\""+c.CanRead+"\" />\n";

			tmp += "</Roles>\n";

			return tmp;
		}

		public string CreateAcls()
		{
			string tmp = "\n<!--________________ Acls _______________ -->\n";
			tmp += "<Acls>\n";
			Acl[] acls = Acl.FindAll();
			foreach (Acl c in acls)
				tmp += "\t<Acl  nombre=\""+c.Id+"\" group=\""+c.Group.Name+"\" role=\""+c.Role.Name+"\" />\n";

			tmp += "</Acls>\n";

			return tmp;
		}

		public string CreateGroups()
		{
			string tmp = "\n<!--________________ Grupos _______________ -->\n";
			tmp += "<Groups>\n";
			Group[] groups = Group.FindAll();
			foreach (Group c in groups)
			{
				tmp += "\t<Group nombre=\""+c.Name+"\">\n";
				foreach (Role r in c.Roles)
					tmp += "\t\t<Role nombre=\""+r.Name+"\" />\n";
				tmp += "\t</Group>\n";
			}
			tmp += "</Groups>\n";

			return tmp;
		}

		public string CreateUsers()
		{
			string tmp = "\n<!--________________ Users _______________ -->\n";
			tmp += "<Users>\n";
			User[] users = User.FindAll();
			foreach (User c in users)
			{
				tmp += "\t<User username=\""+c.Name+"\" password=\""+c.UserPassword+"\" >\n";
				foreach (Group g in c.Groups)
					tmp += "\t\t<Group nombre=\""+g.Name+"\" />\n";
				tmp += "\t</User>\n";
			}
			tmp += "</Users>\n";

			return tmp;
		}

		public string CreateFields()
		{
			string tmp = "\n<!--________________ Fields _______________ -->\n";
			tmp += "<Fields>\n";
			Field[] fields = Field.FindAll();
			foreach (Field c in fields)
				tmp += "\t<Field  nombrecorto=\""+c.Name+"\" nombrelargo=\""+c.Description+"\" type=\""+c.Type+"\" />\n";

			tmp += "</Fields>\n";

			return tmp;
		}

		public string CreateTemplates()
		{
			string tmp = "\n<!--________________ Templates _______________ -->\n";
			tmp += "<Templates>\n";
			Template[] templates = Template.FindAll();
			foreach (Template c in templates)
			{
				// Buscamos el nombre del fichero
				string file = "";
				if (c.TEdit != "")
					file = c.TEdit.TrimEnd('_');
				else
					if (c.TList != "")
						file = c.TList.TrimEnd('_');
					else
						if (c.TView != "")
							file = c.TView.TrimEnd('_');
						else
							file = "general";
				
				tmp += "\t<Template nombre=\""+c.Name+"\" nombrelargo=\""+c.Description+"\" fichero=\""+file+"\" >\n";
				
				// Buscamos los campos asociados a las plantillas
				FieldTemplate[] ft = FieldTemplate.FindByTemplate(c.Id);
				foreach (FieldTemplate f in ft)
				{
					if (f.OrderList == -1)
						tmp += "\t\t<Field nombre=\""+f.Field.Name+"\" show=\"0\" />\n";
					else
						tmp += "\t\t<Field nombre=\""+f.Field.Name+"\" show=\"1\" />\n";
				}
				tmp += "\t</Template>\n";
			}
			tmp += "</Templates>\n";

			return tmp;
		}

		public void CreateCategoriesRec(int idParent, ref string tmp, int tab)
		{
			Category[] categories = Category.FindByParent(idParent);
			foreach (Category c in categories)
			{
				string parent = "";
				string template = "";
				string role = "";
				if (c.Parent != null)
					parent = c.Parent.Name;
				if (c.Template != null)
					template = c.Template.Name;
				if (c.AnonRole != null)
					role = c.AnonRole.Name;

				// Tabuladores para la presentacion
				for (int i = 0; i < tab; i++)
					tmp += "\t";

				tmp += "<Category name=\""+c.Name+"\" visiblename=\""+c.Description+"\" category=\""+parent+"\" template=\""+template+"\" role=\""+role+"\">\n";
				
				foreach (Acl a in c.AclSet)
				{
					// Tabuladores para la presentacion
					for (int i = 0; i < tab; i++)
						tmp += "\t";
					tmp += "\t<Acl nombre=\""+a.Id+"\" />\n";
				}

				CreateCategoriesRec(c.Id, ref tmp, tab + 1);
				
				// Tabuladores para la presentacion
				for (int i = 0; i < tab; i++)
					tmp += "\t";
				tmp += "</Category>\n";
			}

		}

		public string CreateCategories()
		{
			string tmp = "\n<!--________________ Categories _______________ -->\n";
			tmp += "<Categories>\n";
			CreateCategoriesRec(-1, ref tmp, 1);
			tmp += "</Categories>\n";

			return tmp;
		}

		public void CreateMenusRec(int idParent, ref string tmp, int tab)
		{
			Menu[] menus = Menu.FindByParent(idParent);
			foreach (Menu c in menus)
			{
				string parent = "";
				if (c.Parent != null)
					parent = c.Parent.Name;

				// Tabuladores para la presentacion
				for (int i = 0; i < tab; i++)
					tmp += "\t";

				tmp += "<Menu nombre=\""+c.Name+"\" descripcion=\""+c.Description+"\" parent=\""+parent+"\" order=\""+c.Ordering+"\" >\n";
				
				CreateMenusRec(c.Id, ref tmp, tab + 1);
				
				// Tabuladores para la presentacion
				for (int i = 0; i < tab; i++)
					tmp += "\t";
				tmp += "</Menu>\n";
			}

		}

		public string CreateMenus()
		{
			string tmp = "\n<!--________________ Menus _______________ -->\n";
			tmp += "<Menus>\n";
			CreateMenusRec(-1, ref tmp, 1);
			tmp += "</Menus>\n";

			return tmp;
		}

		public string CreateHelp()
		{
			string tmp = "\n<!--________________ Ayudas _______________ -->\n";
			tmp += "<Help>\n";
			Category[] categories = Category.FindAll();
			foreach(Category c in categories)
			{
				tmp += "\t<CategoryHelp category=\""+c.Name+"\" >\n";
				tmp += "\t\t"+c.Information+"\n";
				tmp += "\t</CategoryHelp>\n";

			}

			tmp += "</Help>\n";

			return tmp;
		}

		public void Create ()
		{
			string tmp = "<xml>";
				
			// Tipos base
			tmp += CreateTypes();
		
			// Configuraciones
			tmp += CreateConfigCombos();
				
			// Combos
			tmp += CreateConfigs();
				
			// Roles
			tmp += CreateRoles();

			// Acl
			tmp += CreateAcls();
			
			// Grupos
			tmp += CreateGroups();

			// Usuarios
			tmp += CreateUsers();
				
			// Fields
			tmp += CreateFields();
				
			// Plantillas
			tmp += CreateTemplates();
				
			// Categorias
			tmp += CreateCategories();
				 
			// Menus 
			tmp += CreateMenus();
				
			// Ayudas
			tmp += CreateHelp();
			
			tmp += "</xml>";

			/*string fileWriter = "backup.xml";
			StreamWriter sw = new StreamWriter(fileWriter, false) ;	
			sw.Write(tmp);
			sw.Close();*/

			Response.Charset = "ASCII";
			Response.StatusCode = 200;
			Response.ContentType = "application/xml";
		 	Response.AppendHeader("Content-Disposition", "attachment; filename=\"backup.xml\"");
			Response.Output.Write (tmp);
			CancelView();
		}

     }
}
