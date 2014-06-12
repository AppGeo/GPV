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
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Admin_MasterPage : System.Web.UI.MasterPage, IAdminMasterPage
{
  private bool _reloadRequested = false;

  public bool ReloadRequested
  {
    get
    {
      return _reloadRequested;
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (Request.QueryString["reload"] != null)
    {
      Configuration config = AppContext.GetConfiguration(true);
      AppContext.CacheConfiguration(config);
      ReloadMapTabs(config);

      Response.ContentType = "application/json";
      Response.Write("{ \"success\": true }");
      Response.End();
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!AppUser.IsInRole("admin"))
    {
      Server.Transfer("NotAuthorized.aspx");
    }

    string pageFileName = Path.GetFileNameWithoutExtension(Request.Path);

    switch (Path.GetFileNameWithoutExtension(Request.Path))
    {
      case "Applications": optTabApplications.CssClass = "Tab Selected"; break;
      case "DownloadMarkup": optTabDownloadMarkup.CssClass = "Tab Selected"; break;
      case "TestStoredProcs": optTabTestStoredProcs.CssClass = "Tab Selected"; break;
      case "CheckConfiguration": optTabCheckConfiguration.CssClass = "Tab Selected"; break;
      case "ShowDeactivated": optTabShowDeactivated.CssClass = "Tab Selected"; break;
    }
  }

  protected void cmdReloadConfiguration_Click(object sender, EventArgs e)
  {
    if (Path.GetFileNameWithoutExtension(Request.Path) == "CheckConfiguration")
    {
      _reloadRequested = true;
    }
    else
    {
      Configuration config = AppContext.GetConfiguration(true);
      ReloadMapTabs(config);
    }
  }

  public void ReloadConfiguration(Configuration config)
  {
    AppContext.CacheConfiguration(config);
    ReloadMapTabs(config);
  }

  private void ReloadMapTabs(Configuration config)
  {
    foreach (Configuration.MapTabRow mapTab in config.MapTab.Rows)
    {
      AppContext.GetDataFrame(mapTab);
    }
  }
}
