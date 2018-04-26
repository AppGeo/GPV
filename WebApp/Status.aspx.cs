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
using System.Data;
using AppGeo.Clients;

public partial class Status : CustomStyledPage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    Configuration config = AppContext.GetConfiguration();

    if (Request.QueryString["check"] != null)
    {
      string status = "unknown";

      switch (Request.QueryString["check"].ToLower())
      {
        case "app":
          status = WebConfigSettings.AppIsAvailable ? "up" : "down";
          break;

        case "appmessage":
          status = WebConfigSettings.AppStatusMessage;
          break;

        case "mapservices":
          StringCollection serviceStatus = new StringCollection();

          foreach (Configuration.MapTabRow mapTab in config.MapTab)
          {
            bool isAvailable = AppContext.GetDataFrame(mapTab.MapTabID).Service.IsAvailable;
            serviceStatus.Add(mapTab.MapTabID + ": " + (isAvailable ? "up" : "down"));
          }

          status = serviceStatus.Join("; ");
          break;
      }

      Response.Write(status);
      Response.End();
    }

    labMessage.Text = WebConfigSettings.AppStatusMessage;

    if (WebConfigSettings.AppIsAvailable)
    {
      DataTable table = config.MapTab.Copy();
      table.Columns.Add("Status", typeof(string));

      foreach (DataRow row in table.Rows)
      {
        CommonDataFrame dataFrame = AppContext.GetDataFrame(row["MapTabID"].ToString());
        row["Status"] = dataFrame.Service.IsAvailable ? "up" : "down";
      }

      grdMapServices.DataSource = table;
      grdMapServices.DataBind();
      grdMapServices.Visible = true;
    }
  }
}