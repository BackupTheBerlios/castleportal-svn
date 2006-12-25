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
public class DataModel : ActiveRecordBase
{
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    private int _id;
    private Field _field;
    private string _fieldName;
    private Content _content;
    private string _value;

    public DataModel () {}

    public DataModel (Content content, Field field, string val)
    {
        _content = content;
        _field = field;
        _fieldName = field.Name;
        _value = val;
    }

    [PrimaryKey]
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [BelongsTo]
    public Field Field
    {
        get { return _field; }
        set { _field = value; }
    }

    [Property]
    public string FieldName
    {
        get { return _fieldName; }
        set { _fieldName= value; }
    }

    [BelongsTo]
    public Content Content
    {
        get { return _content; }
        set { _content = value;}
    }

    [Property(Length=10000)]
    public string Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public bool IsIntegerValue()
    {
        try
        {
            Int32.Parse(_value); // FIXME: this is horrible
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    /// <summary>
    /// The stored value may be the Id of another object which name is the same that Field.Type.Name
    /// </summary>
    public object GetObjectFromValue()
    {
        try
        {
            string possibleModel = Field.Type.Name;
            if ((possibleModel != null) && (possibleModel.Length > 0)) // put first char in uppercase
            {
                char[] tmpName = possibleModel.ToCharArray();
                tmpName[0] = char.ToUpper(possibleModel[0]);
                possibleModel = new string(tmpName);
            }
            System.Type t = System.Type.GetType(Constants.NAMESPACE + "." + possibleModel);
            object o = ActiveRecordMediator.FindByPrimaryKey(t, int.Parse(_value));
            return o;
        }
        catch (Exception ex)
        {
            logger.Error("GetObjectFromValue error:" + ex.Message + ","+ ex.StackTrace);
       Console.WriteLine("GetObjectFromValue error:" + ex.Message + ","+ ex.StackTrace);
            throw ex;
            //return null;
        }
    }

    /// <summary>
    /// The stored value may be the Id of another Content
    /// </summary>
    public Content GetPointedContent(string categoryName)
    {
        try
        {
            Content content = Content.Find(int.Parse(_value));
            return content;
        }
        catch (System.FormatException fex)
        {
            logger.Debug("Pointed content not found {0}", _value);
            throw fex;
            //return null;
        }
    }

    /// <summary>
    /// The stored value may be the Id of another Content
    /// </summary>
    public Content GetPointedContent()
    {
        try
        {
            Content content = Content.Find(int.Parse(_value));
            return content;
        }
        catch (System.FormatException fex)
        {
            logger.Debug("Pointed content not found {0}", _value);
            throw fex;
            //return null;
        }
    }

    public override string ToString()
    {
        return _value;
    }

    public static DataModel Find(int id)
    {
        return (DataModel) ActiveRecordBase.FindByPrimaryKey( typeof(DataModel), id );
    }

    public static DataModel[] GetDataModelsByContent (Content content)
    {
        return (DataModel[]) ActiveRecordBase.FindAll(
                   typeof(DataModel),
                   Expression.Eq("Content", content )
               );
    }

    public static DataModel[] SearchByWord(string word)
    {
        if (word == null)
            return new DataModel[0];

        return (DataModel[]) ActiveRecordBase.FindAll(
                   typeof(DataModel),
                   Expression.Like("Value", "%"+ word +"%" )
               );
    }

    public static ArrayList SearchByWordAndUser(string word, User user)
    {
        DataModel[] results = SearchByWord(word);
        ArrayList visibleResults = new ArrayList();
        foreach (DataModel dataModel in results)
        if (dataModel.Content.Category.CanRead(user))
            visibleResults.Add(dataModel);
        return visibleResults;
    }
}
}
