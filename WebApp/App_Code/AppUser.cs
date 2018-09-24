//  Copyright 2016 Applied Geographics, Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Security.Principal;
using System.Web;

public static class AppUser
{
  public static bool IsActive
  {
    get
    {
      if (AppAuthentication.Mode == AuthenticationMode.None)
      {
        return false;
      }

      IPrincipal user = HttpContext.Current.User;
      return user.Identity != null && user.Identity.IsAuthenticated;
    }
  }

  public static string Name
  {
    get
    {
      IPrincipal user = HttpContext.Current.User;
      return user.Identity != null && user.Identity.IsAuthenticated ? user.Identity.Name : "";
    }
  }

  public static string GetDisplayName(OleDbConnection connection)
  {
    IPrincipal user = HttpContext.Current.User;
    string displayName = "";

    if (AppAuthentication.Mode != AuthenticationMode.None && user.Identity != null && user.Identity.IsAuthenticated)
    {
      string sql = String.Format("select DisplayName from {0}User where UserName = '{1}'", WebConfigSettings.ConfigurationTablePrefix, Name);
      OleDbCommand command = new OleDbCommand(sql, connection);
      displayName = command.ExecuteScalar() as string;

      if (displayName == null)
      {
        displayName = Name;
      }
    }

    return displayName;
  }

  public static string GetRole()
  {
    string role = null;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      role = GetRole(connection);
    }

    return role;
  }

  private static string GetRole(OleDbConnection connection)
  {
    IPrincipal user = HttpContext.Current.User;
    string role = "public";

    if (user.Identity != null && user.Identity.IsAuthenticated)
    {
      if (AppAuthentication.Mode == AuthenticationMode.None)
      {
        role = "admin";
      }
      else
      {
        string sql = String.Format("select Role from {0}User where UserName = '{1}' and Role is not null",
            WebConfigSettings.ConfigurationTablePrefix, user.Identity.Name);
        OleDbCommand command = new OleDbCommand(sql, connection);
        role = command.ExecuteScalar() as string;

        if (String.IsNullOrEmpty(role))
        {
          role = "private";
        }
      }
    }

    return role;
  }

  public static bool IsInRole(string checkRole)
  {
    bool isInRole = false;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      isInRole = IsInRole(checkRole, connection);
    }

    return isInRole;
  }

  public static bool IsInRole(string checkRole, OleDbConnection connection)
  {
    if (String.IsNullOrEmpty(checkRole))
    {
      checkRole = "public";
    }
    else
    {
      checkRole = checkRole.ToLower();
    }

    string[] userRole = GetRole(connection).ToLower().Split(',').Select(o => o.Trim()).ToArray();

    return userRole.Contains("admin") || checkRole == "public" || (checkRole == "private" && !userRole.Contains("public")) || userRole.Contains(checkRole);
  }

  public static bool RoleIsInList(string roleList)
  {
    bool roleIsInList = false;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      roleIsInList = RoleIsInList(roleList, connection);
    }

    return roleIsInList;
  }

  public static bool RoleIsInList(string roleList, OleDbConnection connection)
  {
    if (String.IsNullOrEmpty(roleList))
    {
      return true;
    }

    List<String> roles = new List<String>(roleList.Split(','));

    foreach (string checkRole in roles)
    {
      if (IsInRole(checkRole.Trim(), connection))
      {
        return true;
      }
    }

    return false;
  }
}
