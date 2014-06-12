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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class LocationPanel : System.Web.UI.UserControl
{
  public void Initialize(Configuration config, AppState appState, Configuration.ApplicationRow application)
  {
    Configuration.ZoneLevelRow zoneLevel = application.ZoneLevelRow;

    if (zoneLevel != null)
    {
      string zoneName = !zoneLevel.IsZoneTypeDisplayNameNull() ? zoneLevel.ZoneTypeDisplayName : "Zone";
      string levelName = !zoneLevel.IsLevelTypeDisplayNameNull() ? zoneLevel.LevelTypeDisplayName : "Level";

      Configuration.ZoneRow[] zones = zoneLevel.GetZoneRows();
      Configuration.LevelRow[] levels = zoneLevel.GetLevelRows();
      bool hasCombos = false;

      HtmlGenericControl zoneBody = new HtmlGenericControl("tbody");
      HtmlGenericControl levelByZoneBody = new HtmlGenericControl("tbody");
      HtmlGenericControl levelBody = new HtmlGenericControl("tbody");

      if (zones.Length > 0 || levels.Length > 0)
      {
        pnlZoneLevelControl.Style["display"] = "block";

        if (zones.Length > 0)
        {
          optTabZone.Text = zoneName;
          optTabZone.Style["display"] = "block";
          optTabZone.CssClass = "Tab Selected";
          tblZone.Controls.Add(zoneBody);
          tblZone.Style["display"] = "block";

          hasCombos = zones.Any(o => o.GetZoneLevelComboRows().Length > 0);

          if (hasCombos)
          {
            optTabLevelByZone.Text = String.Format("{0} by {1}", levelName, zoneName);
            optTabLevelByZone.Style["display"] = "block";
            tblLevelByZone.Controls.Add(levelByZoneBody);
          }
        }

        if (levels.Length > 0)
        {
          optTabLevel.Text = levelName;
          optTabLevel.Style["display"] = "block";
          tblLevel.Controls.Add(levelBody);

          if (zones.Length == 0)
          {
            optTabLevel.CssClass = "Tab Selected";
            tblLevel.Style["display"] = "block";
          }
        }
      }

      foreach (Configuration.ZoneRow zone in zones)
      {
        AddZoneRow(zoneBody, zone);
      }

      foreach (Configuration.LevelRow level in levels)
      {
        AddLevelRow(levelBody, level, null);
      }

      if (hasCombos)
      {
        foreach (Configuration.ZoneRow zone in zones)
        {
          AddZoneRow(levelByZoneBody, zone);

          Configuration.ZoneLevelComboRow[] zoneLevelCombos = zone.GetZoneLevelComboRows().OrderBy(o => o.LevelRowParent.SequenceNo).ToArray();

          foreach (Configuration.ZoneLevelComboRow zoneLevelCombo in zoneLevelCombos)
          {
            AddLevelRow(levelByZoneBody, zoneLevelCombo.LevelRowParent, zoneLevelCombo.ZoneRowParent);
          }
        }
      }
    }
  }

  private void AddLevelRow(HtmlGenericControl tableBody, Configuration.LevelRow level, Configuration.ZoneRow zone)
  {
    HtmlTableRow tr = new HtmlTableRow();
    tableBody.Controls.Add(tr);
    tr.Attributes["class"] = "NoSelection";
    tr.Attributes["data-level"] = level.LevelID;

    if (zone != null)
    {
      tr.Attributes["class"] += " ZoneLevel";
      tr.Attributes["data-zone"] = zone.ZoneID;
    }
    else
    {
      tr.Attributes["class"] += " Level";
    }

    HtmlTableCell td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Attributes["class"] = "Value";
    td.InnerHtml = "&nbsp;";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Attributes["class"] = "CommandLink";
    td.InnerText = level.IsDisplayNameNull() ? level.LevelID : level.DisplayName;
    
    if (zone != null)
    {
      td.Style["padding-left"] = "20px";
    }
  }

  private void AddZoneRow(HtmlGenericControl tableBody, Configuration.ZoneRow zone)
  {
    HtmlTableRow tr = new HtmlTableRow();
    tableBody.Controls.Add(tr);
    tr.Attributes["class"] = "NoSelection Zone";
    tr.Attributes["data-zone"] = zone.ZoneID;

    HtmlTableCell td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Attributes["class"] = "Value";
    td.InnerHtml = "&nbsp;";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Attributes["class"] = "CommandLink";
    td.InnerText = zone.IsDisplayNameNull() ? zone.ZoneID : zone.DisplayName;
  }
}