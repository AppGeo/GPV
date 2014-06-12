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
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Help : System.Web.UI.Page
{
  private void Page_PreRender(object sender, EventArgs e)
  {
    ddlTarget.Style["background-color"] = ColorTranslator.ToHtml(AppSettings.TargetColorUI);
    ddlSelection.Style["background-color"] = ColorTranslator.ToHtml(AppSettings.SelectionColorUI);

    lnkAdminEmail.HRef = "mailto:" + AppSettings.AdminEmail;
    lnkAdminEmail2.HRef = lnkAdminEmail.HRef;
    pVersion.InnerText = "GPV version " + Version.ToString();

    string applicationID = Request.QueryString["application"];

    if (applicationID == null)
    {
      return;
    }

    Configuration.ApplicationRow application = AppContext.GetConfiguration().Application.FirstOrDefault(o => o.ApplicationID == applicationID);

    if (application == null)
    {
      return;
    }

    pnlAbout.Visible = true;
    labAboutTitle.Text = "About " + application.DisplayName;

    if (!application.IsAboutNull())
    {
      labAboutText.InnerHtml = application.About + "<p/>";
    }

    string tabNames = Request.QueryString["functiontabs"];
    string[] functionTabs = tabNames == null ? new string[] { } : tabNames.Split(',');

    bool hasSelection = functionTabs.Any(o => o.ToLower() == "selection");
    bool hasLegend = functionTabs.Any(o => o.ToLower() == "legend");
    bool hasLocation = functionTabs.Any(o => o.ToLower() == "location");
    bool hasMarkup = functionTabs.Any(o => o.ToLower() == "markup");

    if (hasSelection)
    {
      spnNoSelection.Visible = false;
      spnHasSelection.Visible = true;
      pnlSelection.Visible = true;
    }

    if (hasLegend)
    {
      spnNoLegend.Visible = false;
      spnHasLegend.Visible = true;
    }

    if (hasLocation)
    {
      spnNoLocation.Visible = false;
      spnHasLocation.Visible = true;
    }

    if (hasMarkup)
    {
      spnNoMarkup.Visible = false;
      spnHasMarkup.Visible = true;
      pnlMarkup.Visible = true;

      if (AppAuthentication.Mode != AuthenticationMode.None)
      {
        spnMarkupOpen.Visible = false;
        spnMarkupSecure.Visible = true;
        spnMarkupNameOpen.Visible = false;
        spnMarkupNameSecure.Visible = true;
        liLock.Visible = true;
        spnMarkupSelectOpen.Visible = false;
        spnMarkupSelectSecure.Visible = true;
      }
    }

    bool hasZones = !application.IsZoneLevelIDNull() && application.ZoneLevelRow.GetZoneRows().Length > 0;
    bool hasLevels = !application.IsZoneLevelIDNull() && application.ZoneLevelRow.GetLevelRows().Length > 0;
    bool hasCombos = hasZones && application.ZoneLevelRow.GetZoneRows().Any(o => o.GetZoneLevelComboRows().Length > 0);

    pnlZoneLevel.Visible = hasZones || hasLevels;

    if (hasZones)
    {
      string zoneName = application.ZoneLevelRow.ZoneTypeDisplayName;
      string zoneNameLower = zoneName.ToLower();

      if (String.Compare(zoneName, "Building", true) == 0)
      {
        spnZoneName.InnerText = " (Building in this example and in the current application)";
      }
      else
      {
        spnZoneName.InnerText = String.Format(" (Building in this example, {0} in the current application)", zoneName);
      }

      pZoneTab.Visible = true;
      spnZoneTabName.InnerText = zoneName;
      litZoneName1.Text = zoneNameLower;
      litZoneName2.Text = zoneNameLower;
      litZoneName3.Text = zoneNameLower;
      litZoneLevel.Text = zoneNameLower;
    }

    if (hasLevels)
    {
      string levelName = application.ZoneLevelRow.LevelTypeDisplayName;
      string levelNameLower = levelName.ToLower();

      pMapLevel.Visible = true;
      spnMapLevel.InnerText = levelName;

      if (String.Compare(levelName, "Floor", true) == 0)
      {
        spnLevelName.InnerText = " (Floor in this example and in the current application)";
      }
      else
      {
        spnLevelName.InnerText = String.Format(" (Floor in this example, {0} in the current application)", levelName);
      }

      pnlLevel.Visible = true;
      spnLevel1.InnerText = levelName;
      spnLevel2.InnerText = levelNameLower;
      labLevel.Text = levelName;

      string lev = !application.IsDefaultLevelNull() ? application.DefaultLevel : application.ZoneLevelRow.GetLevelRows()[0].DisplayName;
      ddlLevel.Items.Add(lev);

      pLevelTab.Visible = true;
      spnLevelTabName.InnerText = levelName;

      litLevelName1.Text = levelNameLower;
      litLevelName2.Text = levelNameLower;
      litLevelName3.Text = levelNameLower;
      litZoneLevel.Text = hasZones ? litZoneLevel.Text + " and " + levelNameLower : levelNameLower;
    }

    if (hasCombos)
    {
      pComboTab.Visible = true;
      spnComboTabName.InnerText = application.ZoneLevelRow.LevelTypeDisplayName + " by " + application.ZoneLevelRow.ZoneTypeDisplayName;
    }
  }
}