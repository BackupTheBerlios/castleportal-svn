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

using System;

namespace CastlePortal
{
    public class ChatHelper : Castle.MonoRail.Framework.Helpers.AbstractHelper
    {

        /// <summary>
        /// Write user with a color
        /// </summary>
        /// <param name="user">String with the user that we want get a color</param>
        public string Color(string user)
        {
            string color;
            
            color = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb((int)user.GetHashCode()));
//        color = "FFFFFF";

            return "<b> <font color=\" " + color + "\"> " + user + " </font></b>";
        }
    }
}
