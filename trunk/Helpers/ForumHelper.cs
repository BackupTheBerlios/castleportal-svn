// Authors:
//    Héctor Rojas  <hectorrojas@shidix.com>
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

namespace CastlePortal
{
    public class ForumHelper : Castle.MonoRail.Framework.Helpers.AbstractHelper
    {

        public string WriteMessage(ForumMessage message)
        {
            string ret = "";
            for (int i = 0; i < message.Level; i++)
                ret += "&nbsp;&nbsp;&nbsp;";
            ret += message.Title + "<br>";
            for (int i = 0; i < message.Level; i++)
                ret += "&nbsp;&nbsp;&nbsp;";
            ret += message.Owner + "<br>";
            for (int i = 0; i < message.Level; i++)
                ret += "&nbsp;&nbsp&nbsp;";
            ret += message.Body + "<br>";

            ret += "&nbsp;&nbsp&nbsp;";
            ret += "<a href=\"createmessage.html";
            ret += "?idFolderParent=0";
            ret += "&idMessageParent=" + message.Id + "\">Responder</a>";

            return ret;
        }

        public string Shift(int level)
        {
            string ret = "";
            for (int i = 0; i < level; i++)
                ret += "&nbsp;&nbsp;&nbsp;";

            return ret;
        }
    }
}
