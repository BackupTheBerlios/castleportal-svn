// Authors:
//    Alberto Morales <amd77@gulic.org>
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

using Castle.Windsor;
using System;
using System.IO;
using System.Collections;
using System.Diagnostics; // Process
using NLog;

namespace CastlePortal
{
public class LoginRequired:System.Exception
{
    public LoginRequired(string s): base(s)
    {}
    public LoginRequired(): base()
    {}
}

public class TemplateVariableMissing: System.Exception
{
    public TemplateVariableMissing(string s): base(s)
    {}
    public TemplateVariableMissing(string name, string template, string caller): 
        base(name +"/"+ template +"/" + caller)
    {}
    public TemplateVariableMissing(): base()
    {}
    
}

public class Unauthorized:System.Exception
{
    public Unauthorized(string s): base(s)
    {}
    public Unauthorized(): base()
    {}
}

public class TimeOut: System.Exception
{
    public TimeOut(string s): base(s)
    {}
    public TimeOut(): base ()
    {}
}

public class UnPublished: System.Exception
{
    public UnPublished(string s): base (s)
    {}
    public UnPublished(): base ()
    {}
}

}
