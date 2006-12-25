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
	
	// Test1 initializes ActiveRecord engine to be used by all the tests classes.
	// Test1 also contains basic tests samples to load models from db and to fill a template
	
	[TestFixture()]
	public class Test1
	{
	    VelocityEngine velocity;
	    
		[TestFixtureSetUp()]
		public void Initialize()
		{
            ActiveRecordStarter.Initialize( new XmlConfigurationSource("activeRecord.xml"),
              typeof(Acl) ,
              typeof(Category) ,
              typeof(Chat) ,
              typeof(ChatMessage) ,
              typeof(ConfigCombo) ,
              typeof(ConfigModel) ,
              typeof(Container) ,
              typeof(Content) ,
              typeof(DataModel) ,
              typeof(Field) ,
              typeof(FieldTemplate) ,
              typeof(CastlePortal.File) ,
              typeof(Forum) ,
              typeof(ForumFolder) ,
              typeof(ForumMessage) ,
              typeof(Group) ,
              typeof(Menu) ,
              typeof(Role) ,
              typeof(CastlePortal.Template) ,
              typeof(CastlePortal.Type) ,
              typeof(Language),
              typeof(MenuTranslation),
              typeof(TypeTranslation),
              typeof(User)
           );
           
           velocity = new VelocityEngine();
           ExtendedProperties props = new ExtendedProperties();
           velocity.Init(props);
		}
		
		[Test]
		public void LoadCategories()
		{
		    Category.FindAll();
		}
		
		[Test]
		public void LoadTemplates()
		{
		    CastlePortal.Template[] templates = CastlePortal.Template.FindAll();
		    foreach (CastlePortal.Template template in templates)
		    {
		        Console.WriteLine("Template:" + template.Id +","+ template.Name);
		    }
		}
		
		[Test]
		public void FillSampleTemplate()
		{
		    string path = Path.Combine(TestsCommons.TEMPLATESDIR, "sample.vm");
		    NVelocity.Template nvtemplate = velocity.GetTemplate(path);
		    VelocityContext context = new VelocityContext();
		    context.Put("templates", TestsCommons.TEMPLATESDIR);
            context.Put("variable", "12345");
            StringWriter writer = new StringWriter();
            nvtemplate.Merge(context, writer);
            Console.WriteLine(writer.GetStringBuilder().ToString());
		}
		
	}
}
