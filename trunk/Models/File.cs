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
using Castle.ActiveRecord;
using System.Collections;
using Iesi.Collections;
using NHibernate.Expression;
using System;

namespace CastlePortal
{
#if CACHE
[ActiveRecord( Cache=CacheEnum.ReadWrite )]
#else
[ActiveRecord]
#endif
public class File : ActiveRecordBase
{
#if DEBUG
//    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
#endif    
    static ConfigManager config = ConfigManager.GetInstance();
    private int _Id;
    private string _Name;
    private string _Filename;
    private string _ContentType;
    private System.DateTime _CreateDate;
    private int _Size;
    private string _Directory;

    public File () {
        _CreateDate = System.DateTime.Now;
    }

    public File(string directory) : this() 
    {
        _Directory = directory;

        string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
        baseDir = System.IO.Path.Combine(baseDir, config.GetValue(Constants.PRIVATE_FOLDER));
        baseDir = System.IO.Path.Combine(baseDir, _Directory);
        System.IO.Directory.CreateDirectory(baseDir);
    }

    [PrimaryKey]
    public int Id
    {
        get { return _Id; }
        set { _Id = value; }
    }

    [Property]
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    [Property]
    public string Filename
    {
        get { return _Filename; }
        set { _Filename = value; }
    }

    [Property]
    public string ContentType
    {
        get { return _ContentType; }
        set { _ContentType = value; }
    }

    [Property]
    public System.DateTime CreateDate
    {
        get { return _CreateDate; }
        set { _CreateDate = value; }
    }

    [Property]
    public int Size
    {
        get { return _Size; }
        set { _Size = value; }
    }

    [Property]
    public string Directory
    {
        get { return _Directory; }
        set { _Directory = value; }
    }

    public string FullPath()
    {
        string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
        baseDir = System.IO.Path.Combine(baseDir, config.GetValue(Constants.PRIVATE_FOLDER));
        baseDir = System.IO.Path.Combine(baseDir, _Directory);
        baseDir = System.IO.Path.Combine(baseDir, _Filename);
        Console.WriteLine("fullpath= " + baseDir);
        return baseDir;
    }

    public static File[] FindAll()
    {
        return (File[]) ActiveRecordBase.FindAll(
                   typeof(File),
                   new Order[] { Order.Asc("Name") }
               );
    }

    public static File Find(int id)
    {
        return (File) ActiveRecordBase.FindByPrimaryKey( typeof(File), id );
    }

    private string RandomString(int size, bool lowerCase)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        System.Random random = new System.Random();
        for(int i=0; i<size; i++)
        {
            int n = System.Convert.ToInt32(System.Math.Floor((26+10) * random.NextDouble()));
            char ch;
            if (n < 26) {
                ch = System.Convert.ToChar(n + 65);
            } else {
                ch = System.Convert.ToChar(n - 26 + 48);
            }
            builder.Append(ch);
        }
        if(lowerCase)
            return builder.ToString().ToLower();
        return builder.ToString();
    }

    public bool SaveAttach(System.Web.HttpPostedFile attach)
    {
        string Formname = RandomString(5, true);

        if (attach != null) {
            _Name = System.Text.RegularExpressions.Regex.Replace(attach.FileName, "^.*[/\\\\]", "");
            _Filename = attach.FileName + "_" + Formname;
            _ContentType = attach.ContentType;
            _Size = attach.ContentLength;
            if (_Id != 0)
                Save();
            else
                Create();
            attach.SaveAs(FullPath());
            return true;
        }
        return false;
    }

    public void RemoveAttach()
    {
        System.IO.File.Move(this.FullPath(), this.FullPath()+".bak");
        this.Delete ();
    }
}

}

