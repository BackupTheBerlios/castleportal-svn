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

using Castle.Windsor;
using System;
using System.IO;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using NLog;

namespace CastlePortal
{
public class AdminAPI
{
    static NLog.Logger logger = LogManager.GetCurrentClassLogger();

    public static void AddUser(int gid, int uid, string gindex, string uindex)
    {
        if ((gid!=0) && (uid!=0))
        {
            Group group = Group.Find(gid);
            User user = User.Find(uid);
            group.Users.Add(user);
            group.Save();
#if CACHE
            try
            {
                user.Groups.Add(group);
                user.Save(); 
            }
            catch (ActiveRecordException ex) 
            {
                logger.Error("WEIRD!!!: " + ex.Message + ":" + ex.StackTrace);
            }
#endif
        }
    }

    public static void AddUser(int gid, int uid)
    {
        Group group = Group.Find(gid);
        User user = User.Find(uid);
        group.Users.Add(user);
        group.Save();
#if CACHE
        try
        {
            user.Groups.Add(group);
            user.Save(); 
        }
        catch (ActiveRecordException ex)
        {
            logger.Error("WEIRD!!!: " + ex.Message + ":" + ex.StackTrace);
        }
#endif
    }

    public static void DelUser(int gid, int uid, string gindex, string uindex)
    {
        if ((gid!=0) && (uid!=0))
        {
            Group group = Group.Find(gid);
            User user = User.Find(uid);		
            User selectedUser = user;					
            foreach (User u in group.Users)
            {
                if (u.Name == user.Name)
                    selectedUser = u;
            }
            group.Users.Remove(selectedUser);
            group.Save();
#if CACHE
            selectedUser.Groups.Remove(group);
            selectedUser.Save();
#endif
        }
    }
}
}
