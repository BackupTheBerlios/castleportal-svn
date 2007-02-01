// Authors:
//    Alberto Morales <amd77@gulic.org>
//    Carlos Ble <carlosble@shidix.com>
//    Beatriz Garcia <beagarci@shidix.com>
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
using System.Web.Mail;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Filters;
using Iesi.Collections;
using Castle.ActiveRecord;
using NHibernate.Expression;
using System;

namespace CastlePortal
{
	
[Helper (typeof (MenuHelper))]
[Helper (typeof (StringHelper))]
[Helper (typeof (TemplateVarsTester))]
[Layout ("general")]
[Rescue("generalerror")]
[Resource( "l10n", "l10n" )]
[LocalizationFilter( RequestStore.Cookie, Constants.LOCALE_COOKIE )]
[Filter (ExecuteEnum.BeforeAction, typeof (CheckPublishFilter))]
[Filter (ExecuteEnum.BeforeAction, typeof (ConfigLoadFilter))]
[DefaultAction("Redir")]
public class PortalController:ARSmartDispatcherController
{
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    static ConfigManager config = ConfigManager.GetInstance();

    // **************** Private Methods ***************************

    private void CheckPermissions(Category category, Permission perm)
    {
        if ((bool)Session[Constants.IS_ROOT])
            return;
        if (category == null)
        {
            throw new Unauthorized("Null category!");
        }
        
        IDictionary categoriesAcl = (IDictionary) Session[Constants.CATEGORIES_ACLS];
        if (categoriesAcl != null)  // registered user
        {
            if (!categoriesAcl.Contains(category.Id))
            {
                User user = null;
                if (Session.Contains(Constants.USER)) 
                    user = (User) Session[Constants.USER];
                categoriesAcl[category.Id] = category.GetPermissionsHash(user);
            }
            IDictionary categoryAcl = (IDictionary) categoriesAcl[category.Id];
            if (!((bool) categoryAcl[perm]))
                throw new Unauthorized("");
        }
        else    // anonymous user
        {
            if (!category.AnonRole.Can(perm))
            {
                throw new Unauthorized("");
            }
        }
    }

    private void CheckPermissions(Category category, Permission perm, bool layout)
    {
        if ((bool)Session[Constants.IS_ROOT])
            return;

        if (!layout)
            LayoutName = null;
        IDictionary categoriesAcl = (IDictionary) Session[Constants.CATEGORIES_ACLS];
        if (!categoriesAcl.Contains(category.Id))
        {
            User user = null;
            if (Session.Contains(Constants.USER)) 
                user = (User) Session[Constants.USER];
            categoriesAcl[category.Id] = category.GetPermissionsHash(user);
        }
        IDictionary categoryAcl = (IDictionary) categoriesAcl[category.Id];
        if (!((bool) categoryAcl[perm]))
            throw new Unauthorized("");
    }

    /*private ArrayList GetModelsListFromFormattedString(string sourceType, string sourceString)
    {
        if (sourceString == null)
        {
            return null;
        }
        ArrayList list = new ArrayList ();
        string[] Ids = sourceString.TrimEnd(Constants.SEPARATOR).Split(Constants.SEPARATOR);
        foreach (string id in Ids)
        {
            System.Type type = System.Type.GetType(sourceType);
            object model = ActiveRecordMediator.FindByPrimaryKey(type, int.Parse(id));
            list.Add(model);
        }
        return list;
    }*/
    
    /*private ArrayList GetFieldsFromFormattedString(string sourceString)
    {
    	  return GetModelsListFromFormattedString("CastlePortal.Field", sourceString);
    }*/
    
    /*private ArrayList GetAclsFromFormattedString(string sourceString)
    {
        return GetModelsListFromFormattedString("CastlePortal.Acl", sourceString);
    }*/

    private void AddFieldsTemplates(Template template, string edit, string list)
    {

        foreach (FieldTemplate ft in template.Fields)
           ft.Delete();
        template.ListingVisibleFields = new ArrayList();
        template.Fields = new ArrayList();
        if (edit != null)
        {
            string[] options = edit.TrimEnd(':').Split(':');
            string[] ol;
            if (list != null)
                ol = list.TrimEnd(':').Split(':');
            else
                ol = edit.TrimEnd(':').Split(':');
            int i = 1;
            int j = 1;
            // delete current data 
            foreach (string option in options)
            {
                Field field = Field.Find(int.Parse(option));
                FieldTemplate fieldTemplate;
                // if field is within main list
                if ((Array.BinarySearch(ol, option) >= 0) && (Array.BinarySearch(ol, option) <= options.Length))
                {
                    fieldTemplate = new FieldTemplate(field, template, i++, j++);
                    fieldTemplate.Save();
                    template.Fields.Add(fieldTemplate);
                    template.ListingVisibleFields.Add(fieldTemplate);
                }
                else
                {
                    fieldTemplate = new FieldTemplate(field, template, -1, j++);
                    fieldTemplate.Save();
                    template.Fields.Add(fieldTemplate);
                }
            }
        }
    }

    private void SavePortalCategory(int id, string name, string desc, 
                              string info, int ownerId, int parentId, 
                              int tempId, int roleId, string aclString)
    {
        Category category, parentCategory;
        if (parentId < 0)
        {
            parentCategory = null;
        }
        else
        {
            parentCategory = Category.Find(parentId);
        }
        if (id != 0)
        {
            category = Category.Find (id);
            CheckPermissions(category, Permission.Modify);
        }
        else
        {
            if (parentId > 0)
            {
                CheckPermissions(parentCategory, Permission.Create);
            }
            category = new Category();
        }
        User sessionUser = (User) Session[Constants.USER];
        category.Name = name;
        category.Description = desc;
        category.Information = info;
        if (ownerId > 0)
        {
            User tmpUser = User.Find(ownerId);
            if (tmpUser != null)
            {
                category.Owner = tmpUser;
            }   
        }
        else
        {
            if (sessionUser != null)
            {
                category.Owner = sessionUser;
            }
        }
        category.Parent = parentCategory; 
        if (tempId > 0)
        {
            Template tmpTemplate = Template.Find(tempId);
            if (tmpTemplate != null)
            {
                category.Template = tmpTemplate;
            }
        }
        else
        {
            category.Template = Template.GetDefault();
        }
        category.AnonRole = null;
        if (roleId > 0)
        {
            Role tmpRole = Role.Find(roleId);
            if (tmpRole != null)
            {
                category.AnonRole = tmpRole;
            }
        }
        //string aclString = (string) Request.Form["af[]"];
        //ArrayList aux = GetAclsFromFormattedString(aFields);
        if ((aclString != null) && (aclString.Length > 0))
        {
            category.AclSet = new HashedSet();
            string[] acls = aclString.Split(',');
            if (acls != null)
            {
                foreach (string i in acls)
                {
                    Acl acl = Acl.Find(int.Parse(i));
                    if (acl != null)
                    {
                        category.AclSet.Add(acl);
                    }
                }
            }
        }
        else
        {
            Group tempGroup = Group.FindByExactName(
                                  config.GetValue(Constants.ADMIN_GROUP));
            Role tempRole = Role.FindByName(config.GetValue(Constants.ALL_PRIVILEDGES));
            category.AclSet = new HashedSet();
            category.AclSet.Add(Acl.FindByGroupRole (tempGroup.Id, tempRole.Id));
        }

        if (id != 0)
        {
            category.Save();
        }
        else
        {
            category.Create();
            id = category.Id;
#if CACHE
            category.Parent.Children.Add(category);
            category.Parent.Save();
#endif
        }

        User user = null;
        if (Session.Contains(Constants.USER))
        {
            user = sessionUser;  
        } 
        ((IDictionary)Session[Constants.CATEGORIES_ACLS])[category.Id] = category.GetPermissionsHash(user);
    }

    private Category DeletePortalCategory(int id, string confirm)
    {
        Category category;
        if (id > 0)
        {
            category = Category.Find(id);
            if (category == null)
            {
                throw new NotFoundException("Category not found: " + id);
            }
            CheckPermissions(category, Permission.Delete);
        }
        else
        {
            return null;
        }

        if (confirm == "yes")
        {
#if CACHE
            if (category.Parent != null)
            {
                bool contained = false;
                foreach (Category ctgory in category.Parent.Children)
                {
                    if (ctgory.Id == category.Id)
                    {
                        contained = true;
                        category.Parent.Children.Remove(ctgory);
                        break;
                    }
                }
                if (contained)
                {
                    category.Parent.Save();
                }
            }
            foreach(Acl acl in category.AclSet)
            {
                foreach(Container ctgory in acl.Containers)
                {
                    if (ctgory.Id == category.Id)
                    {
                        acl.Containers.Remove(ctgory);
                        break;
                    }
                }
                acl.Save();
            }
#endif
            foreach(Category ctgory in category.Children)
            {
                ctgory.Delete();
            }
            foreach(Content content in category.ContentList)
            {
                
                foreach(DataModel dataModel in content.DataHash.Values)
                {
                    dataModel.Delete();
                }
                content.DataHash.Clear();
                content.Delete();
            }
            /*foreach(Content content in category.ContentListSortedByReverseDate)
            {
                content.Delete();
            }*/
            category.Children.Clear();
            category.ContentList.Clear();
            category.ContentListSortedByReverseDate.Clear();
            category.Delete ();
            Hashtable aclcat = Category.GetHashesByUser((User)Session[Constants.USER]);
            Session[Constants.CATEGORIES_ACLS] = aclcat;
            return null;
        }
        else
        {
            return category;
        }
    }

    private void SavePortalContent (int contentId, int categoryId, string language)
    {
        Category category = Category.Find(categoryId);
        CheckPermissions(category, Permission.Modify);
        if (category == null)
        {
            RedirectToAction(Constants.CATEGORIES);
            return;
        }
        Content content;
        if (contentId != 0)
        {
            content = Content.Find (contentId);
        }
        else
        {
            content = new Content(category);
            Language lang = Language.FindByName(language);
            if (lang != null)
                content.Lang = lang;
            content.Save(); //				r.Save();
        }
        contentId = content.Id;
        // Read form inputs and attached files
        bool emptyForm  = true;
        if (Request.Form.Count > 0 || Request.Files.Count > 0)
        {
            foreach (string input in Request.Form)
            {
                DataModel data;
                if (content.DataHash != null && content.DataHash.Contains(input))
                {
                    data = (DataModel) content.DataHash[input];
                    data.Value = Request.Form[input];
                }
                else
                {
                    Field field = Field.GetByName(input);
                    if (field == null)
                        continue; 			//  invalid field name
                    data = new DataModel(content, field, Request.Form[input]);
                }
                data.Save();
                emptyForm = false;
            }
            foreach (string input in Request.Files.Keys)
            {
                DataModel data;
                System.Web.HttpPostedFile postedFile = Request.Files[input] as System.Web.HttpPostedFile;
                if ((postedFile == null) || (postedFile.FileName.Length == 0) ||
                        (postedFile.ContentLength == 0))
                {
                    logger.Debug("File not uploaded");
                    continue;
                }
                else if (content.DataHash != null && content.DataHash.Contains(input))
                {
                    data = (DataModel) content.DataHash[input];
                    File oldfile = (File) data.GetObjectFromValue();
                    File file = new File( config.GetValue(Constants.MEDIA_FOLDER));
                    file.SaveAttach(postedFile);
                    data.Value = file.Id.ToString();
                    oldfile.RemoveAttach();
                }
                else
                {
                    Field field = Field.GetByName(input);
                    if (field == null)
                        continue;
                    File file = new File(config.GetValue(Constants.MEDIA_FOLDER));
                    file.SaveAttach(postedFile);
                    data = new DataModel(content, field, file.Id.ToString());
#if CACHE
                    content.DataHash[input] = data; // force update cache
#endif

                }
                data.Save();
                emptyForm = false;
            }
            if (emptyForm)
            {
                content.Delete();
            }
            else
            {
                bool isNew = true;
                foreach (Content c in category.ContentList)
                if (c.Id == content.Id)
                    isNew = false;
                if (isNew)
                {
                    category.ContentList.Add(content);
                    category.ContentListSortedByReverseDate.Add(content);
                    category.Save();
                    logger.Debug("save NEW content/category:" + content.Id +","+ category.Id);
                }
                else
                {
                    content.Save();
                    category.Save();
                    logger.Debug("save NOT NEW content/category:" + content.Id +","+ category.Id);
                }
            }
        }
    }


    /*private void sendmail(string from, string to, string subject, string body)
    {
        MailMessage oMsg = new MailMessage();
        oMsg.From = from;
        oMsg.To = to;
        oMsg.Subject = subject;
        oMsg.BodyFormat = MailFormat.Html;
        oMsg.Body = body;

        // attachment
        //String sFile = @"C:\temp\Hello.txt";
        //MailAttachment oAttch = new MailAttachment(sFile, MailEncoding.Base64);
        //oMsg.Attachments.Add(oAttch);

        // TODO: Replace with the name of your remote SMTP server.
        SmtpMail.SmtpServer = "localhost";
        SmtpMail.Send(oMsg);
        oMsg = null;
    }*/





    
    // ************     Public members (Actions): *******************



    public void SetLanguage(string language)
    {
        Response.CreateCookie(Constants.LOCALE_COOKIE, language );
        RedirectToAction("index");
    }

    public void Redir()
    {
        RedirectToAction("index");
    }

    public void Accesible(int accesible, string current)
    {
        Session[Constants.ACCESIBLE] = accesible;
        Response.Redirect(current);
    }

    public void Index ()
    {
        Category category = Category.FindByName(config.GetValue(Constants.FRONTPAGE));
        Hashtable parameters = new Hashtable();
        parameters["Id"] = category.Id;
        RedirectToAction("viewcategory", parameters);
    }
    
    public void ShowSiteMap()
    {
    
    }

    public void Categories(bool layout)
    {
        Commons.CheckSuperUser(Session);
        PropertyBag[Constants.CATEGORIES] = Category.FindByParent(-1);    
        PropertyBag[Constants.PARENT] = -1;
        if (layout == false)
        {
            LayoutName = null;
        }
    }
    
    public void Categories()
    {
        Commons.CheckSuperUser(Session);
        PropertyBag[Constants.CATEGORIES] = Category.FindByParent(-1);    
        PropertyBag[Constants.PARENT] = -1;
    }

    public void Categories(int parent)
    {
        Commons.CheckSuperUser(Session);
        PropertyBag[Constants.CATEGORIES] = Category.FindByParent(parent);  
        PropertyBag[Constants.PARENT] = Category.Find(parent);
    }

    public void CategoriesBlock(int parent)
    {
        //CheckSuperUser(Session);
        PropertyBag[Constants.CATEGORIES] = Category.FindByParent(parent);
        PropertyBag[Constants.PARENT] = Category.Find(parent);
        LayoutName = null;
    }

    public void Templates()
    {
        Commons.CheckSuperUser(Session);
        PropertyBag[Constants.TEMPLATES] = Template.FindAll();
    }

    public void Fields()
    {
        Commons.CheckSuperUser(Session);
        PropertyBag[Constants.FIELDS] = Field.FindAll();
    }


    //
    // Content
    //

    public void NewContent (int id)   // parent category id 
    {
        Category category = Category.Find(id);
        CheckPermissions(category, Permission.Create);
        if (category == null)
        {
            RenderView(Constants.CATEGORY_NOT_FOUND) ;
        }
        else
        {
            PropertyBag[Constants.CATEGORY] = category;
            RenderView("editcontent");
        }
    }

    public void NewContent (int id, bool layout) // id de la category padre
    {
        Category category = Category.Find(id);
        CheckPermissions(category, Permission.Create, layout);
        if (category == null)
        {
            RenderView(Constants.CATEGORY_NOT_FOUND) ;
        }
        else
        {
            PropertyBag[Constants.CATEGORY] = category;
            RenderView("editcontent");
        }
        if (!layout)
            LayoutName = null;
    }

    public void ViewContent ([ARFetch("id")]Content content)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Read);
            PropertyBag[Constants.CONTENT] = content;
            //RenderView(row.Category.Template.TView);
        }
    }

    public void ViewContent ([ARFetch("id")]Content content, bool layout)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Read, layout);
            PropertyBag[Constants.CONTENT] = content;
            //RenderView(row.Category.Template.TView);
        }
        if (!layout)
            LayoutName = null;
    }

    public void EditContent ([ARFetch("id")]Content content)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Modify);
            PropertyBag[Constants.CONTENT] = content;
            //RenderView(row.Category.Template.TEdit);
        }
    }

    public void EditContent ([ARFetch("id")]Content content, bool layout)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Modify, layout);
            PropertyBag[Constants.CONTENT] = content;
        }
        if (!layout)
            LayoutName = null;
    }

    public void CopyContent ([ARFetch("id")]Content content)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Create);
            PropertyBag[Constants.LANGUAGES] = Language.FindAll();
            PropertyBag[Constants.CONTENT] = content;
        }
    }

    public void CopyContent ([ARFetch("id")]Content content, bool layout)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Create, layout);
            PropertyBag[Constants.LANGUAGES] = Language.FindAll();
            PropertyBag[Constants.CONTENT] = content;
        }
        if (!layout)
            LayoutName = null;
    }

    public void PublishContent([ARFetch("id")]Content content)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Publish);
            content.Published = true;
            content.Save();
            PropertyBag[Constants.CONTENT] = content;
            RenderView("viewcontent");
        }
    }
    
    public void PublishContent([ARFetch("id")]Content content, bool layout)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Publish, layout);
            content.Published = true;
            content.Save();
            PropertyBag[Constants.CONTENT] = content;
            RenderView("viewcontent");
        }
        if (!layout)
            LayoutName = null;
    }
    
    public void UnpublishContent([ARFetch("id")]Content content)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Publish);
            content.Published = false;
            content.Save();
            PropertyBag[Constants.CONTENT] = content;
            RenderView("viewcontent");
        }
    }
    
    public void UnpublishContent([ARFetch("id")]Content content, bool layout)
    {
        if (content == null)
        {
            RenderView(Constants.CONTENT_NOT_FOUND) ;
        }
        else
        {
            CheckPermissions(content.Category, Permission.Publish, layout);
            content.Published = false;
            content.Save();
            PropertyBag[Constants.CONTENT] = content;
            RenderView("viewcontent");
        }
        if (!layout)
            LayoutName = null;
    }

    public void SaveContent (int contentId, int categoryId, string language)
    {
        SavePortalContent(contentId, categoryId, language);
        Hashtable parameters = new Hashtable();
        parameters["Id"] = categoryId;
        RedirectToAction("viewcategory", parameters);
    }
    
    public void SaveContent (int contentId, int categoryId, string language, bool layout)
    {
        SavePortalContent(contentId, categoryId, language);
        Hashtable parameters = new Hashtable();
        parameters["Id"] = categoryId;
        parameters["layout"] = layout;
        RedirectToAction("viewcategory", parameters);
    }


    public void SaveCopiedContent (int contentId, int categoryId, int languageId)
    {
        Category category = Category.Find(categoryId);
        CheckPermissions(category, Permission.Modify);
        if (category == null)
        {
            Flash["error"] = "Categoria padre inexistente";
            RedirectToAction(Constants.CATEGORIES);
            return;
        }
        Content originalContent = Content.Find(contentId);
        Content content = new Content(category);
        content.Lang = Language.Find(languageId);
        if (content.Lang == null)
        {
            logger.Error("language is null!!!: " + languageId);
            return;
        }
        content.Save();  // Get an Id
        if (originalContent != null)
        {
            foreach(string key in originalContent.DataHash.Keys) // duplicate content
            {
                DataModel originalDataModel = (DataModel)originalContent.DataHash[key];
                DataModel copyDataModel = new DataModel(content, originalDataModel.Field, 
                                                        originalDataModel.Value);
                content.DataHash[key] = copyDataModel;
                copyDataModel.Save();
            }
            content.Save();
        }
        //contentId = content.Id;
        // Read form inputs and attached files
        bool emptyForm  = true;
        if (Request.Form.Count > 0 || Request.Files.Count > 0)
        {
            foreach (string input in Request.Form)
            {
                DataModel data;
                if (content.DataHash != null && content.DataHash.Contains(input))
                {
                    data = (DataModel) content.DataHash[input];
                    data.Value = Request.Form[input];
                }
                else
                {
                    Field field = Field.GetByName(input);
                    if (field == null)
                        continue; 			//  invalid field name
                    data = new DataModel(content, field, Request.Form[input]);
                }
                data.Save();
                emptyForm = false;
            }
            foreach (string input in Request.Files.Keys)
            {
                DataModel data;
                System.Web.HttpPostedFile postedFile = Request.Files[input] as System.Web.HttpPostedFile;
                if ((postedFile == null) || (postedFile.FileName.Length == 0) ||
                        (postedFile.ContentLength == 0))
                {
                    logger.Debug("File not uploaded");
                    continue;
                }
                else if (content.DataHash != null && content.DataHash.Contains(input))
                {
                    data = (DataModel) content.DataHash[input];
                    File oldfile = (File) data.GetObjectFromValue();
                    File file = new File( config.GetValue(Constants.MEDIA_FOLDER));
                    file.SaveAttach(postedFile);
                    data.Value = file.Id.ToString();
                    oldfile.RemoveAttach();
                }
                else
                {
                    Field field = Field.GetByName(input);
                    if (field == null)
                        continue;
                    File file = new File(config.GetValue(Constants.MEDIA_FOLDER));
                    file.SaveAttach(postedFile);
                    data = new DataModel(content, field, file.Id.ToString());
#if CACHE
                    content.DataHash[input] = data; // force update cache
#endif

                }
                data.Save();
                emptyForm = false;
            }
            if (emptyForm)
            {
                content.Delete();
            }
            else
            {
                bool isNew = true;
                foreach (Content c in category.ContentList)
                if (c.Id == content.Id)
                    isNew = false;
                if (isNew)
                {
                    category.ContentList.Add(content);
                    category.ContentListSortedByReverseDate.Add(content);
                    category.Save();
                    logger.Debug("save NEW content/category:" + content.Id +","+ category.Id);
                }
                else
                {
                    content.Save();
                    category.Save();
                    logger.Debug("save NOT NEW content/category:" + content.Id +","+ category.Id);
                }
            }
            Hashtable parameters = new Hashtable();
            parameters["Id"] = category.Id;
            RedirectToAction("viewcategory", parameters);
        }
    }


    public void SaveCopiedContent (int contentId, int categoryId, int languageId, bool layout)
    {
        SaveCopiedContent(contentId, categoryId, languageId);
        Hashtable parameters = new Hashtable();
        parameters["Id"] = categoryId;
        parameters["layout"] = layout;
        RedirectToAction("viewcategory", parameters);
    }

    public void DeleteContent(int id)
    {
        if (id > 0)
        {
            Content content = Content.Find(id);
            Category c = content.Category;
            CheckPermissions(content.Category, Permission.Delete);
            int i = content.Category.Id;
            c.ContentList.Remove(content);
            c.ContentListSortedByReverseDate.Remove(content);
            c.Save();
            foreach (DataModel d in content.DataHash.Values)
            d.Delete();
            content.Delete();
            Hashtable parameters = new Hashtable();
            parameters["id"] = i;
            RedirectToAction("viewcategory", parameters);
        }
        else
        {
            RedirectToAction("index");
            return;
        }

    }

    public void DeleteContent(int id, bool layout)
    {
        if (!layout)
            LayoutName = null;
        if (id > 0)
        {
            Content content = Content.Find(id);
            Category c = content.Category;
            CheckPermissions(content.Category, Permission.Delete, layout);
            int i = content.Category.Id;
            c.ContentList.Remove(content);
            c.ContentListSortedByReverseDate.Remove(content);
            c.Save();
            foreach (DataModel d in content.DataHash.Values)
            d.Delete();
            content.Delete();
            Hashtable parameters = new Hashtable();
            parameters["id"] = i;
            parameters["layout"] = layout;
            RedirectToAction("viewcategory", parameters);
        }
        else
        {
            RedirectToAction("index");
            return;
        }

    }



    //
    // category
    //

    public void NewCategory (int parent)
    {
        if (parent <= 0)
            RenderView(Constants.CATEGORY_NOT_FOUND);
        else
        {
            // create a child category
            Category parentCategory = Category.Find(parent);
            CheckPermissions(parentCategory, Permission.Create);
            PropertyBag[Constants.PARENT] = parentCategory;
            PropertyBag[Constants.USERS] = User.FindAll ();
            PropertyBag[Constants.TEMPLATES] = Template.FindAllPublic ();
            PropertyBag[Constants.ROLES] = Role.FindAll();
            PropertyBag[Constants.ACLS] = Acl.FindAll();
            LayoutName = null;
            RenderView("editcategory");
        }

    }

    public void NewCategory (int parent, bool layout)
    {
        if (parent <= 0)
            RenderView(Constants.CATEGORY_NOT_FOUND);
        else
        {
            // create a child category
            Category parentCategory = Category.Find(parent);
            CheckPermissions(parentCategory, Permission.Create, layout);

            PropertyBag[Constants.PARENT] = parentCategory;
            PropertyBag[Constants.USERS] = User.FindAll ();
            PropertyBag[Constants.TEMPLATES] = Template.FindAllPublic();
            PropertyBag[Constants.ROLES] = Role.FindAll();
            PropertyBag[Constants.ACLS] = Acl.FindAll();
            LayoutName = null;
            RenderView("editcategory");
        }
        if (!layout)
            LayoutName = null;
    }


    public void ViewCategory (Category category)
    {
        CheckPermissions(category, Permission.Read);
        PropertyBag[Constants.MENU]  = Menu.FindByCategory(category);
        PropertyBag[Constants.CATEGORY]  = category;
       
        //RenderView("viewcategory");
    }

    public void ViewCategory (int id)
    {
        Category category = Category.FindWithContentsByLang(id, Commons.GetCurrentLang(this));
        CheckPermissions(category, Permission.Read);
        PropertyBag[Constants.MENU]  = Menu.FindByCategory(category);
        PropertyBag[Constants.CATEGORY]  = category;
        //RenderView("viewcategory");
    }

    public void ViewCategory (int id, bool layout)
    {
        Category category = Category.FindWithContentsByLang(id, Commons.GetCurrentLang(this));
        CheckPermissions(category, Permission.Read, layout);
        PropertyBag[Constants.MENU]  = Menu.FindByCategory(category);
        PropertyBag[Constants.CATEGORY]  = category;
        if (!layout)
            LayoutName = null;
        //RenderView("viewcategory");
    }

    public void ViewCategoryRaw (int id)
    {
        Category category = Category.Find(id);
        CheckPermissions(category, Permission.Read);
        PropertyBag[Constants.MENU]  = Menu.FindByCategory(category);
        PropertyBag[Constants.CATEGORY]  = category;
        LayoutName = null;
        //RenderView("viewcategory");
    }


    public void ViewCategoryTree (int id)
    {
        Category category = Category.Find(id);
        CheckPermissions(category, Permission.Read);
        if (category != null)
        {
            PropertyBag[Constants.CATEGORIES] = Category.FindByParent(category.Id);
            PropertyBag[Constants.CATEGORY] = category;
            PropertyBag[Constants.PARENT] = category;
        }
        //RenderView("viewcategorytree");
    }

    public void ViewCategoryTree (int id, bool layout)
    {
        Category category = Category.Find(id);
        CheckPermissions(category, Permission.Read, layout);
        if (category != null)
        {
            PropertyBag[Constants.CATEGORIES] = Category.FindByParent(category.Id);
            PropertyBag[Constants.CATEGORY] = category;
            PropertyBag[Constants.PARENT] = category;
        }
        if (!layout)
            LayoutName = null;
        //RenderView("viewcategorytree");
    }

    public void EditCategory (int id)
    {
        Category category = Category.Find(id);
        CheckPermissions(category, Permission.Modify);

        PropertyBag[Constants.CATEGORY] = category;
        PropertyBag[Constants.PARENT] = category.Parent;
        PropertyBag[Constants.USERS] = User.FindAll ();
        PropertyBag[Constants.TEMPLATES] = Template.FindAllPublic ();
        PropertyBag[Constants.ROLES] = Role.FindAll ();
        PropertyBag[Constants.ACLS] = Acl.FindAll();
    }

    public void EditCategory (int id, bool layout)
    {
        Category category = Category.Find(id);
        CheckPermissions(category, Permission.Modify, layout);

        PropertyBag[Constants.CATEGORY] = category;
        PropertyBag[Constants.PARENT] = category.Parent;
        PropertyBag[Constants.USERS] = User.FindAll ();
        PropertyBag[Constants.TEMPLATES] = Template.FindAllPublic ();
        PropertyBag[Constants.ROLES] = Role.FindAll ();
        PropertyBag[Constants.ACLS] = Acl.FindAll();
        if (!layout)
            LayoutName = null;
    }

    public void SaveCategory (int id, string name, string desc, 
                              string info, int ownerId, int parentId, 
                              int tempId, int roleId, string aFields)
    {
        SavePortalCategory(id, name, desc, info, ownerId, parentId, tempId, roleId, Request.Form["af[]"]);
        Hashtable parameters = new Hashtable();
        if (id != 0)
        {
            parameters["id"] = id;
            RedirectToAction("viewcategory", parameters);
        }
        else
        {
            parameters["layout"] = false;
            RedirectToAction(Constants.CATEGORIES, parameters);
        }
    }

    public void SaveCategory (int id, string name, string desc, 
                              string info, int ownerId, int parentId, 
                              int tempId, int roleId, string aFields, bool layout)

    {
        SavePortalCategory(id, name, desc, info, ownerId, parentId, tempId, roleId, Request.Form["af[]"]);
        Hashtable parameters = new Hashtable();
        parameters["layout"] = layout;
        if (id != 0)
        {
            parameters["id"] = id;
            RedirectToAction("viewcategory", parameters);
        }
        else
        {
            RedirectToAction(Constants.CATEGORIES, parameters);
        }
    }

    public void DeleteCategory (int id, string confirm)
    {
        Category category = DeletePortalCategory(id, confirm);
        if (confirm == "yes")
        {
            RedirectToAction("index");
        }
        else
        {
            PropertyBag["f"] = category;
        }
    }

    public void DeleteCategory (int id, string confirm, bool layout)
    {
        Category category = DeletePortalCategory(id, confirm);
        if (confirm == "yes")
        {
            RedirectToAction("index");
        }
        else
        {
            PropertyBag["f"] = category;
        }
        if (!layout)
        {
            LayoutName = null;
        }
    }


    //
    // Templates
    //

    /*public void NewTemplate()
    {
    	if ((!(bool)Session[Constants.IS_ADMIN]) && (!(bool)Session[Constants.IS_ROOT]))
    	{
    		Flash["aviso"] = "No tiene permiso para realizar esa accin";
    		Response.Redirect("portal", "index");
    	}
    	PropertyBag["fields"] = Field.FindAll();
    	RenderView("edittemplate");
    }*/

    // no tiene ViewTemplate

    public void EditTemplate([ARFetch("Id")]Template template)
    {
        Commons.CheckSuperUser(Session);
        if (template == null)
        {
            PropertyBag["fields"] = Field.FindAll();
            //RenderText("El template que pretende editar no existe");
        }
        else
        {
            PropertyBag["template"] = template;
            PropertyBag["fields"] = Field.FindAll();
        }
    }

    public void SaveTemplate(int id, string name, string description, 
            string tlist, string tview, string tedit, string sFields, string sFieldsEdit, bool tpub)
    {
        Commons.CheckSuperUser(Session);
        Template template;
        if (id != 0)
            template = Template.Find (id);
        else
            template = new Template();

        template.Name = name;
        template.Description = description;
        template.Public = tpub;

        string filelist = Constants.DEFAULT_LIST_TEMPLATE;
        string fileview = Constants.DEFAULT_VIEW_TEMPLATE;
        string fileedit = Constants.DEFAULT_EDIT_TEMPLATE;

        if (System.IO.File.Exists(Constants.TEMPLATES_FOLDER + tlist))
        {
            filelist = tlist;
        }
        if (System.IO.File.Exists(Constants.TEMPLATES_FOLDER + tview))
        {
            fileview = tview;
        }
        if (System.IO.File.Exists(Constants.TEMPLATES_FOLDER + tedit))
        {
            fileedit = tedit;
        }

        template.TList = filelist;
        template.TView = fileview;
        template.TEdit = fileedit;
        template.Fields = new ArrayList();

        AddFieldsTemplates(template, sFieldsEdit, sFields);

        template.Save();
        RedirectToAction(Constants.TEMPLATES);
    }

    // no tiene DeleteTemplate

    //
    // Fields
    //

    public void NewField()
    {
        Commons.CheckSuperUser(Session);
        PropertyBag["types"] = Type.FindAll();
        RenderView("editfield");
    }

    // no tiene ViewField

    public void EditField([ARFetch("Id")]Field f)
    {
        Commons.CheckSuperUser(Session);
        if (f != null)
        {
            PropertyBag[Constants.FIELD] = f;
            PropertyBag[Constants.TYPES] = Type.FindAll();
        }
    }

    public void SaveField([ARFetch("Id")]Field field , string name, 
                          string description, [ARFetch("TypeId")] Type type)
    {
        Commons.CheckSuperUser(Session);
        if (field == null)
            field = new Field();

        field.Name = name;
        field.Description = description;
        field.Type = type;
        field.Save();
        RedirectToAction("fields");
    }

    public void DeleteField([ARFetch("Id")]Field field)
    {
        Commons.CheckSuperUser(Session);
        if (field != null)
        {
            try
            {
                field.Delete();
                RedirectToAction("fields");
            }
            catch (Exception e)
            {
                RenderText("No se puede borrar el campo, posiblemente haya datos de este tipo de campo: " + e.Message);
            }
        }
    }

    public void Search(string key)
    {
        User user = null;
        if (Session.Contains(Constants.USER)) 
        {
            user = (User) Session[Constants.USER];
        }
        PropertyBag["key"] = key;
        PropertyBag["catresults"] = Category.SearchByWordAndUser(key, user);
        PropertyBag["dataresults"] = DataModel.SearchByWordAndUser(key, user);
    }


/*    public void sendmail (string name, string subject, string body)
    {
        string b = "<HTML><BODY>";
        b += "<p>Nombre: <strong>"+name+"</strong></p>";
        b += "<p>Asunto: "+subject+"</p>";
        b += "<p>Cuerpo: "+body+"</p>";
        b += "</BODY></HTML>";

        sendmail("plataforma pew <pew@megaserver>",
                 ConfigModel.FindByKey("email").Val,
                 "Formulario de Contacto en Web PEW",
                 b);

    }

    public void incidencia(string url, string fecha, string tipo, string gravedad, string texto)
    {
        if ((url==null) || (fecha==null) ||
                (tipo==null) || (gravedad==null) || (texto==null))
        {
            PropertyBag["url"] = Context.UrlReferrer;
            PropertyBag["fecha"] = System.DateTime.Now;
        }
        else
        {
            string b = "<html><body>";
            b += System.String.Format("<p>Url: <a href='{0}'>{0}</a></p>", url);
            b += System.String.Format("<p>Directorio: {0}</p>", Context.ApplicationPhysicalPath);
            b += System.String.Format("<p>Fecha: {0}</p>", fecha);
            b += System.String.Format("<p>Tipo: {0}</p>", tipo);
            b += System.String.Format("<p>Gravedad: {0}</p>", gravedad);
            b += System.String.Format("<p>Detalle: {0}</p>", texto);
            b += "</body></html>";

            // todo perfecto para enviar
            sendmail("plataforma pew <pew@megaserver>",
                     "pew@listas.shidix.com",
                     "Pew "+tipo+" ("+gravedad+")",
                     b);
            PropertyBag["enviado"] = true;
        }
    }
*/
   

}
}
