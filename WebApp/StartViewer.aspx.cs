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
using System.Data;
using System.Data.OleDb;
using System.Web;
using System.Web.Security;

public partial class StartViewer : CustomStyledPage
{
	private Configuration _config;

	private void Page_Init(object sender, EventArgs e)
	{
		_config = AppContext.GetConfiguration();
	}

	protected void Page_Load(object sender, EventArgs e)
  {
		Response.Cache.SetCacheability(HttpCacheability.NoCache);

		if (Session["StartError"] != null)
		{
			labMessage.Text = (string)Session["StartError"];
      pnlApplications.Visible = false;
			Session["StartError"] = null;
			return;
		}

		Dictionary<String, String> launchParams = null;

		if (Session["LaunchParams"] != null)
		{
      launchParams = (Dictionary<String, String>)Session["LaunchParams"];
		}
		else
		{
      launchParams = Request.GetNormalizedParameters();
      Session["LaunchParams"] = launchParams;
		}

    bool isDebugging = launchParams.Count == 0 && System.Diagnostics.Debugger.IsAttached;
    bool hasShowApps = AppSettings.AllowShowApps && launchParams.Count == 1 && launchParams.ContainsKey("showapps");
    bool hasApplication = (!hasShowApps && !String.IsNullOrEmpty(AppSettings.DefaultApplication)) || launchParams.ContainsKey("application") || launchParams.ContainsKey("state");

    if (hasApplication)
		{
			Response.Redirect("Viewer.aspx");
		}

    Session["LaunchParams"] = null;

    if (isDebugging || hasShowApps)
		{
      ShowApplications();
		}
		else
		{
			labMessage.Text = "An application has not been specified";
      labMessage.Font.Bold = false;
		}
  }

	private void ShowApplications()
	{
		labMessage.Text = "Available Applications";

    DataTable table = new DataTable();
    table.Columns.Add("ApplicationID", typeof(string));
    table.Columns.Add("DisplayName", typeof(string));
    table.Columns.Add("About", typeof(string));

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      foreach (Configuration.ApplicationRow application in _config.Application)
      {
        string roles = application.IsAuthorizedRolesNull() ? "public" : application.AuthorizedRoles;

        if (AppUser.RoleIsInList(roles, connection))
        {
          table.Rows.Add(new object[] { application.ApplicationID, application.DisplayName, application.IsAboutNull() ? "" : application.About });
        }
      }
    }

    grdApplications.DataSource = table;
		grdApplications.DataBind();
		grdApplications.Visible = true;
	}
}
