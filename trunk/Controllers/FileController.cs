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
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using NHibernate.Expression;

namespace CastlePortal
{
	[Helper (typeof (StringHelper))]
	[DefaultAction("Redir")]
	//[Filter (ExecuteEnum.BeforeAction, typeof (CheckGroupFilter))]
	public class FileController: ARSmartDispatcherController
	{
		// Funcion para redirigir al index del portal en caso de que se haya escrito algun metodo inexistente 
		// del controlador
		public void Redir()
		{
			Response.Redirect("portal" , "index"); 
		}

		public void Delete ([ ARFetch ("Id", Create = false)] File file)
		{
			if (file != null) 
				file.RemoveAttach();

			Response.Redirect ("file", "list");
		}

		public void Get ([ ARFetch ("Id", Create = false)] File file)
		{
			GetDownload(file, false);
		}

		public void Download ([ ARFetch ("Id", Create = false)] File file)
		{
			GetDownload(file, true);
		}

		private void GetDownload (File file, bool download)
		{
			//System.Console.WriteLine("file = " + file.Name);
			//System.Console.WriteLine("file.Directory = " + file.Directory);
			if (file == null) {
				  Response.StatusCode = 500;
				  Response.ContentType = "text/plain";
				  Response.Write ("Id not found!");
			} else if (!System.IO.File.Exists (file.FullPath())) {
				  Response.StatusCode = 500;
				  Response.ContentType = "text/plain";
				  Response.Write ("File " + file.Name + " not found!");
			} else {
				  System.IO.FileStream expFS = System.IO.File.OpenRead (file.FullPath());
				  byte[]barray = new byte[expFS.Length];
				  expFS.Read (barray, 0, barray.Length);

				  Response.Charset = "ASCII";
				  Response.StatusCode = 200;
				  Response.ContentType = file.ContentType;
				  if (download)
					  Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
				  else
					  Response.AppendHeader("Content-Disposition", "inline; filename=\"" + file.Name + "\"");
				  Response.OutputStream.Write (barray, 0, barray.Length);
			}
			CancelView ();
		}

		/*
		public void List()
		{
			PropertyBag["files"] = File.FindAll();
			PropertyBag["category"] = Category.FindAll();
			PropertyBag["filefolder"] = FileFolder.FindAll();
			PropertyBag["message"] = Message.FindAll();
		}
		*/
	}
}
