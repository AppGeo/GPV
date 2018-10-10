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
		HtmlGenericControl parentLegend = new HtmlGenericControl("div");
		pnlBaseMapScroll.Controls.Add(parentLegend);

		HtmlGenericControl legendEntry = new HtmlGenericControl("div");
		parentLegend.Controls.Add(legendEntry);
		legendEntry.Attributes["class"] = "LegendEntry";

		HtmlGenericControl legendHeader = new HtmlGenericControl("div");
		legendEntry.Controls.Add(legendHeader);
		legendHeader.Attributes["class"] = "LegendHeader";

		HtmlGenericControl name = new HtmlGenericControl("span");
		legendHeader.Controls.Add(name);
		name.Attributes["class"] = "LegendName";
		name.InnerText = "BaseMaps";

    foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
    {
      Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;
      AddTileGroupsForMapTab(mapTabRow, appState, true);
    }

		HtmlGenericControl parentLegend1 = new HtmlGenericControl("div");
		pnlBaseMapScroll.Controls.Add(parentLegend1);

		HtmlGenericControl legendEntry1 = new HtmlGenericControl("div");
		parentLegend1.Controls.Add(legendEntry1);
		legendEntry1.Attributes["class"] = "LegendEntry";

		HtmlGenericControl legendHeader1 = new HtmlGenericControl("div");
		legendEntry1.Controls.Add(legendHeader1);
		legendHeader1.Attributes["class"] = "LegendHeader";

		HtmlGenericControl name1 = new HtmlGenericControl("span");
		legendHeader1.Controls.Add(name1);
		name1.Attributes["class"] = "LegendName";
		name1.InnerText = "Overlays";

		foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
		{
			Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;
      AddTileGroupsForMapTab(mapTabRow, appState, false);
		}
	}

  private void AddTileGroupsForMapTab(Configuration.MapTabRow mapTabRow, AppState appState, bool asBaseMap)
  {
    StringCollection visibleTiles = appState.VisibleTiles[mapTabRow.MapTabID];

    // create the top level legend control for this map tab

    HtmlGenericControl parentLegend = new HtmlGenericControl("div");
    pnlBaseMapScroll.Controls.Add(parentLegend);
    parentLegend.Attributes["data-maptab"] = mapTabRow.MapTabID;
    parentLegend.Attributes["class"] = "LegendTop";
    parentLegend.Style["display"] = mapTabRow.MapTabID == appState.MapTab ? "block" : "none";

    int n = 0;

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
          radio.Attributes["id"] = string.Format("baseTile{0}", n);
          radio.Checked = visibleTiles.Contains(tileGroupRow.TileGroupID);
          radio.Attributes["class"] = "LegendCheck RadioCheck";
          onOffControl = radio;
        }
        else
        {
          HtmlInputCheckBox checkBox = new HtmlInputCheckBox();
          checkBox.Attributes["id"] = string.Format("overlayTile{0}", n);
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

        n += 1;
      }
    }

    if (asBaseMap)
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
