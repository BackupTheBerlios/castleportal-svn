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
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using CastlePortal;
using NUnit.Framework;
using NVelocity;
using NVelocity.Context;
using NVelocity.App;
using Commons.Collections;

using System.IO;

namespace unittests
{
	
	
	[TestFixture()]
	public class Test2
	{
	    VelocityEngine velocity;
	    
		[TestFixtureSetUp()]
		public void Initialize()
		{
           velocity = new VelocityEngine();
           ExtendedProperties props = new ExtendedProperties();
           velocity.Init(props);
		}
		
		/// <summary>
		/// Load categories from database and fill their templates.
		/// Templates should fire exceptions if something is wrong
		/// </summary>
		[Test]
		public void PopulateTemplatesWithCurrentCategories()
		{
		    Console.WriteLine("*** PopulateTemplatesWithCurrentCategories");
		    Category[] categories = Category.FindAll();
		    foreach (Category category in categories)
		    {
		        CastlePortal.Template template = category.Template;
		        Console.WriteLine("Template:" + template.Id +","+ template.Name);
		        string path = Path.Combine(TestsCommons.TEMPLATESDIR, TestsCommons.GENERALTEMPLATES);
		        path = Path.Combine(path, template.Name);
		        path += TestsCommons.EXTENSION;
		        NVelocity.Template nvtemplate = velocity.GetTemplate(path);
		        VelocityContext context = new VelocityContext();
		        context.Put(TestsCommons.TEMPLATESDIRVAR, TestsCommons.TEMPLATESDIR);
                context.Put(TestsCommons.CATEGORYVAR, category);
                StringWriter writer = new StringWriter();
                nvtemplate.Merge(context, writer);
            }
		}
		
		/// <summary>
		/// Load templates from database. Do not add fields to template
		/// Create a category for every template to try to merge it.
		/// </summary>
		[Test]
		public void MergeEmptyTemplates()
		{
		    Console.WriteLine("*** MergeEmptyTemplates");
		    CastlePortal.Template[] templates = CastlePortal.Template.FindAll();
		    Category category = new Category();
		    foreach (CastlePortal.Template template in templates)
		    {
		        category.Name = "just testing...";
		        category.Template = template;
		        Console.WriteLine("Template:" + template.Id +","+ template.Name);
		        string path = Path.Combine(TestsCommons.TEMPLATESDIR, TestsCommons.GENERALTEMPLATES);
		        path = Path.Combine(path, template.Name);
		        path += TestsCommons.EXTENSION;
		        NVelocity.Template nvtemplate = velocity.GetTemplate(path);
		        VelocityContext context = new VelocityContext();
		        context.Put(TestsCommons.TEMPLATESDIRVAR, TestsCommons.TEMPLATESDIR);
                context.Put(TestsCommons.CATEGORYVAR, category);
                StringWriter writer = new StringWriter();
                nvtemplate.Merge(context, writer);
            }
		}
		
		/// <summary>
		/// Load templates from database. Add some fields to template
		/// Create a category for every template to try to merge it.
		/// </summary>
		[Test]
		public void PopulateAllTemplates()
		{
		    Console.WriteLine("*** PopulateAllTemplates");
		    CastlePortal.Template[] templates = CastlePortal.Template.FindAll();
		    foreach (CastlePortal.Template template in templates)
		    {
		        Category category = new Category();
		        category.Name = "just testing...";
		        category.Template = template;
		        //category.Save();
		        Content content = new Content();
                content.Category = category;
                //content.Save();
		        category.ContentList = new System.Collections.ArrayList();
		        DataModel data = null;
		        if (template.Fields.Count > 0)
		        {
		            //Console.WriteLine("total fields= :" + template.Fields.Count);
		            foreach(CastlePortal.FieldTemplate fieldTemplate in template.Fields)
		            {
		                //Console.WriteLine("FIELD =" + fieldTemplate.Field.Name);
		                string name = fieldTemplate.Field.Name;
		                if ((name != "file") && (name != "image"))
		                {
		                    data = new DataModel(content, fieldTemplate.Field, "just a test field");
		                    content.DataHash[fieldTemplate.Field.Name] = data;
		                }
		                //data.Save();
		            }
		        }
		        category.ContentList.Add(content);
		        Console.WriteLine("Template:" + template.Id +","+ template.Name);
		        string path = Path.Combine(TestsCommons.TEMPLATESDIR, TestsCommons.GENERALTEMPLATES);
		        path = Path.Combine(path, template.Name);
		        path += TestsCommons.EXTENSION;
		        NVelocity.Template nvtemplate = velocity.GetTemplate(path);
		        VelocityContext context = new VelocityContext();
		        context.Put(TestsCommons.TEMPLATESDIRVAR, TestsCommons.TEMPLATESDIR);
                context.Put(TestsCommons.CATEGORYVAR, category);
                StringWriter writer = new StringWriter();
                nvtemplate.Merge(context, writer);
                if (data != null)
                {
                    //data.Delete();
                }
                //content.Delete();
                //category.Delete();
            }
		}
		
		
	}
}