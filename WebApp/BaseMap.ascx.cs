﻿//  Copyright 2012 Applied Geographics, Inc.
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

public partial class BaseMap:System.Web.UI.UserControl
{
	private void AddTiles(Configuration.MapTabRow mapTabRow, AppState appState)
	{
		StringCollection visibleTiles=appState.VisibleTiles[mapTabRow.MapTabID];
		// create the top level legend control for this map tab
		HtmlGenericControl parentLegend=new HtmlGenericControl("div");
		pnlBaseMapScroll.Controls.Add(parentLegend);
		parentLegend.Attributes["data-maptab"]=mapTabRow.MapTabID;
		parentLegend.Attributes["class"]="LegendTop";
		parentLegend.Style["display"]=mapTabRow.MapTabID==appState.MapTab?"block":"none";
		foreach(Configuration.MapTabTileGroupRow mapTabTileGroupRow in mapTabRow.GetMapTabTileGroupRows())
		{
			Configuration.TileGroupRow tileGroupRow=mapTabTileGroupRow.TileGroupRow;
			HtmlGenericControl legendEntry=new HtmlGenericControl("div");
			parentLegend.Controls.Add(legendEntry);
			legendEntry.Attributes["class"]="LegendEntry";
			HtmlGenericControl legendHeader=new HtmlGenericControl("div");
			legendEntry.Controls.Add(legendHeader);
			legendHeader.Attributes["class"]="LegendHeader";
			HtmlGenericControl visibility=new HtmlGenericControl("span");
			legendHeader.Controls.Add(visibility);
			visibility.Attributes["class"]="LegendVisibility";
			// HtmlInputCheckBox radio = new HtmlInputCheckBox();

			HtmlInputRadioButton radio=new HtmlInputRadioButton();
			visibility.Controls.Add(radio);
			radio.Checked=visibleTiles.Contains(tileGroupRow.TileGroupID);
			radio.Attributes["class"]="LegendCheck";
			radio.Attributes["data-tilegroup"]=tileGroupRow.TileGroupID;
			HtmlGenericControl name=new HtmlGenericControl("span");
			legendHeader.Controls.Add(name);
			name.Attributes["class"]="LegendName";
			name.InnerText=tileGroupRow.DisplayName;
		}
	}

	public void Initialize(Configuration config, AppState appState, Configuration.ApplicationRow application)
	{
		foreach(Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
		{
			Configuration.MapTabRow mapTabRow=appMapTabRow.MapTabRow;
			AddTiles(mapTabRow, appState);
			HtmlGenericControl parentLegend=new HtmlGenericControl("div");
			pnlBaseMapScroll.Controls.Add(parentLegend);
			parentLegend.Attributes["data-maptab"]=mapTabRow.MapTabID;
			parentLegend.Attributes["class"]="LegendTop";
			parentLegend.Style["display"]=mapTabRow.MapTabID==appState.MapTab?"block":"none";
			HtmlGenericControl legendEntry=new HtmlGenericControl("div");
			parentLegend.Controls.Add(legendEntry);
			legendEntry.Attributes["class"]="LegendEntry";
			HtmlGenericControl legendHeader=new HtmlGenericControl("div");
			legendEntry.Controls.Add(legendHeader);
			legendHeader.Attributes["class"]="LegendHeader";
			HtmlGenericControl visibility=new HtmlGenericControl("span");
			legendHeader.Controls.Add(visibility);
			visibility.Attributes["class"]="LegendVisibility";
			HtmlInputRadioButton radio=new HtmlInputRadioButton();
			visibility.Controls.Add(radio);
			radio.Checked=false;
			radio.Attributes["class"]="LegendCheck";
			radio.Attributes["data-tilegroup"]="None";
			HtmlGenericControl name=new HtmlGenericControl("span");
			legendHeader.Controls.Add(name);
			name.Attributes["class"]="LegendName";
			name.InnerText="None";
		}
	}
}
