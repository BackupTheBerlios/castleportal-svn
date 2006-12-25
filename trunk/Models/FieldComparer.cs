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
using Castle.ActiveRecord.Queries;
using System;

namespace CastlePortal
{
internal class FieldComparer : System.Collections.IComparer
{
    private string _key;

    public FieldComparer(string key)
    {
        _key = key;
    }

    public int Compare(object x, object y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        Content rx = (Content) x;
        Content ry = (Content) y;
        return string.Compare(rx.GetValueByFieldName(_key), ry.GetValueByFieldName(_key));
    }
}
}
