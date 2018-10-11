﻿//  Copyright 2018 Applied Geographics, Inc.
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

using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public partial class BaseMap : UserControl
{
	public void Initialize(Configuration config, AppState appState, Configuration.ApplicationRow application)
	{
    int m = 0;

    foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
    {
      Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;
      AddTileGroupsForMapTab(mapTabRow, appState, m, true);
      m += 1;
    }

    foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
    {
      Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;
      AddTileGroupsForMapTab(mapTabRow, appState, m, false);
      m += 1;
    }
  }

  private void AddTileGroupsForMapTab(Configuration.MapTabRow mapTabRow, AppState appState, int m, bool asBaseMap)
  {
    StringCollection visibleTiles = appState.VisibleTiles[mapTabRow.MapTabID];

    // create the top level legend control for this map tab

    HtmlGenericControl parentLegend = new HtmlGenericControl("div");
    pnlBaseMapScroll.Controls.Add(parentLegend);
    parentLegend.Attributes["data-maptab"] = mapTabRow.MapTabID;
    parentLegend.Attributes["class"] = "LegendTop";
    parentLegend.Style["display"] = mapTabRow.MapTabID == appState.MapTab ? "block" : "none";

    HtmlGenericControl typeEntry = new HtmlGenericControl("div");
    parentLegend.Controls.Add(typeEntry);
    typeEntry.Attributes["class"] = "LegendEntry";

    HtmlGenericControl typeHeader = new HtmlGenericControl("div");
    typeEntry.Controls.Add(typeHeader);
    typeHeader.Attributes["class"] = "LegendHeader";

    HtmlGenericControl typeName = new HtmlGenericControl("span");
    typeHeader.Controls.Add(typeName);
    typeName.Attributes["class"] = "LegendName";
    typeName.InnerText = asBaseMap ? "BaseMaps" : "Overlays";

    int g = 0;

    foreach (Configuration.MapTabTileGroupRow mapTabTileGroupRow in mapTabRow.GetMapTabTileGroupRows())
    {
      Configuration.TileGroupRow tileGroupRow = mapTabTileGroupRow.TileGroupRow;
      bool isBaseMap = tileGroupRow.GetTileLayerRows().Any(t => t.IsOverlayNull() || t.Overlay == 0);

      if (isBaseMap == asBaseMap)
      {
        HtmlGenericControl legendEntry = new HtmlGenericControl("div");
        parentLegend.Controls.Add(legendEntry);
        legendEntry.Attributes["class"] = "LegendEntry";

        HtmlGenericControl legendHeader = new HtmlGenericControl("div");
        legendEntry.Controls.Add(legendHeader);
        legendHeader.Attributes["class"] = "LegendHeader";

        HtmlGenericControl visibility = new HtmlGenericControl("span");
        legendHeader.Controls.Add(visibility);
        visibility.Attributes["class"] = "LegendVisibility";

        HtmlControl onOffControl;

        if (isBaseMap)
        {
          HtmlInputRadioButton radio = new HtmlInputRadioButton();
          radio.Attributes["id"] = string.Format("baseTile{0}-{1}", m, g);
          radio.Checked = visibleTiles.Contains(tileGroupRow.TileGroupID);
          radio.Attributes["class"] = "LegendCheck RadioCheck";
          onOffControl = radio;
        }
        else
        {
          HtmlInputCheckBox checkBox = new HtmlInputCheckBox();
          checkBox.Attributes["id"] = string.Format("overlayTile{0}-{1}", m, g);
          checkBox.Checked = visibleTiles.Contains(tileGroupRow.TileGroupID);
          checkBox.Attributes["class"] = "LegendCheck OverlaysCheck";
          onOffControl = checkBox;
        }

        visibility.Controls.Add(onOffControl);
        onOffControl.Attributes["group"] = mapTabRow.MapTabID;
        onOffControl.Attributes["name"] = mapTabRow.MapTabID;
        onOffControl.Attributes["data-tilegroup"] = tileGroupRow.TileGroupID;

        HtmlGenericControl label = new HtmlGenericControl("label");
        legendHeader.Controls.Add(label);
        label.Attributes["for"] = onOffControl.Attributes["id"];

        HtmlGenericControl name = new HtmlGenericControl("span");
        label.Controls.Add(name);
        name.Attributes["class"] = "LegendName";
        name.InnerText = tileGroupRow.DisplayName;

        g += 1;
      }
    }

    if (g == 0)
    {
      parentLegend.Controls.Remove(typeEntry);
    }
    else if (asBaseMap)
    {
      AddNoneOption(mapTabRow, parentLegend);
    }
  }

  public void AddNoneOption(Configuration.MapTabRow mapTabRow, HtmlGenericControl parentLegend)
	{
		HtmlGenericControl legendEntry = new HtmlGenericControl("div");
		parentLegend.Controls.Add(legendEntry);
		legendEntry.Attributes["class"] = "LegendEntry";

		HtmlGenericControl legendHeader = new HtmlGenericControl("div");
		legendEntry.Controls.Add(legendHeader);
		legendHeader.Attributes["class"] = "LegendHeader";

		HtmlGenericControl visibility = new HtmlGenericControl("span");
		legendHeader.Controls.Add(visibility);
		visibility.Attributes["class"] = "LegendVisibility";

		HtmlInputRadioButton radio = new HtmlInputRadioButton();
		visibility.Controls.Add(radio);
    radio.Attributes["id"] = "baseTileNone";
    radio.Checked = false;
		radio.Attributes["class"] = "LegendCheck RadioCheck";
		radio.Attributes["group"] = mapTabRow.MapTabID;
		radio.Attributes["name"] = mapTabRow.MapTabID;

    HtmlGenericControl label = new HtmlGenericControl("label");
    legendHeader.Controls.Add(label);
    label.Attributes["for"] = radio.Attributes["id"];

    HtmlGenericControl name = new HtmlGenericControl("span");
    label.Controls.Add(name);
		name.Attributes["class"] = "LegendName";
		name.InnerText = "None";
	}
}
