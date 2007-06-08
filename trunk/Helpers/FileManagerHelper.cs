// Authors:
//    Beatriz Garcia Garcia <bgarci@shidix.com>
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
public class FileManagerHelper:Castle.MonoRail.Framework.Helpers.AbstractHelper
{
    public string DeleteDirectoryLink(string directory, string icon_path, string border, string action)
    {
        string[] content = System.IO.Directory.GetFileSystemEntries (directory);
				string html;
        if (content.Length == 0)
				    html = "<a href=" + action + "><img src=" + icon_path + " border=" + border + "/></a>";
				else
						html = "";
				return html;
		}
}
}
