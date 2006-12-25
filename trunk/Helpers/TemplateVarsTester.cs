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
using System.Collections;

namespace CastlePortal
{
public class TemplateVarsTester:Castle.MonoRail.Framework.Helpers.AbstractHelper
{
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    public void IsNotNull(object variable, string name, string template, string caller)
    {
        if (variable != null)
        {
            return;        
        }
        else
        {
            logger.Error("Variable/Template:"+ name + "/" + template + "/" + caller);
            throw new TemplateVariableMissing(name, template, caller);
        }
    }
}
}
