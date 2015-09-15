//  Copyright 2012 Applied Geographics, Inc.
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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Web.UI.HtmlControls;

public partial class MarkupPanel : System.Web.UI.UserControl
{
  public void Initialize(Configuration config, AppState appState, Configuration.ApplicationRow application)
  {
    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      LoadMarkupCategories(application, appState, connection);

      if (AppAuthentication.Mode != AuthenticationMode.None)
      {
        tboMarkupUser.Attributes["value"] = AppUser.GetDisplayName(connection);
        tboMarkupUser.Attributes["disabled"] = "disabled";
        chkMarkupLock.Style["visibility"] = "visibile";
        labMarkupLock.Style["visibility"] = "visible";
        cmdNewMarkup.Attributes["class"] = "CommandLink";
      }
    }
  }

  private void LoadMarkupCategories(Configuration.ApplicationRow application, AppState appState, OleDbConnection connection)
  {
    bool selected = false;

    foreach (Configuration.ApplicationMarkupCategoryRow link in application.GetApplicationMarkupCategoryRows())
    {
      string roles = link.MarkupCategoryRow.IsAuthorizedRolesNull() ? "public" : link.MarkupCategoryRow.AuthorizedRoles;

      if (AppUser.RoleIsInList(roles, connection))
      {
        HtmlGenericControl option = new HtmlGenericControl("option");
        option.Attributes["value"] = link.CategoryID;
        option.InnerText = link.MarkupCategoryRow.DisplayName;

        if (link.CategoryID == appState.MarkupCategory)
        {
          option.Attributes["selected"] = "selected";
          selected = true;
        }

        ddlMarkupCategory.Controls.Add(option);
      }
    }

    if (!selected)
    {
      appState.MarkupCategory = "";
      appState.MarkupGroups = new StringCollection();

      if (ddlMarkupCategory.Controls.Count > 0)
      {
        appState.MarkupCategory = ((HtmlGenericControl)ddlMarkupCategory.Controls[0]).Attributes["value"];
      }
    }
  }
}