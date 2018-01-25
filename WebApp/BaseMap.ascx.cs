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
			foreach(Configuration.TileLayerRow tileLayer in mapTabTileGroupRow.TileGroupRow.GetTileLayerRows())
			{
				bool isOverlay=!tileLayer.IsOverlayNull()&&tileLayer.Overlay==1;
				if(!isOverlay)
				{
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
					radio.Attributes["class"]="LegendCheck RadioCheck";
					radio.Attributes["group"]=mapTabRow.MapTabID;
					radio.Attributes["name"]=mapTabRow.MapTabID;
					radio.Attributes["data-tilegroup"]=tileGroupRow.TileGroupID;
			HtmlGenericControl name=new HtmlGenericControl("span");
			legendHeader.Controls.Add(name);
			name.Attributes["class"]="LegendName";
			name.InnerText=tileGroupRow.DisplayName;
				}
				else
				{


				}

			}
		}
	}
	private void AddTilesForOverlay(Configuration.MapTabRow mapTabRow, AppState appState)
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
			foreach(Configuration.TileLayerRow tileLayer in mapTabTileGroupRow.TileGroupRow.GetTileLayerRows())
			{
				bool isOverlay=!tileLayer.IsOverlayNull()&&tileLayer.Overlay==1;
				if(!isOverlay)
				{

				}
				else
				{
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
			
					HtmlInputCheckBox ckeckBox=new HtmlInputCheckBox();
					visibility.Controls.Add(ckeckBox);
					ckeckBox.Checked=visibleTiles.Contains(tileGroupRow.TileGroupID);
					ckeckBox.Attributes["class"]="LegendCheck OverLaysCheck";
					ckeckBox.Attributes["data-tilegroup"]=tileGroupRow.TileGroupID;
					ckeckBox.Attributes["group"]=mapTabRow.MapTabID;
					ckeckBox.Attributes["name"]=mapTabRow.MapTabID;
			HtmlGenericControl name=new HtmlGenericControl("span");
			legendHeader.Controls.Add(name);
			name.Attributes["class"]="LegendName";
			name.InnerText=tileGroupRow.DisplayName;

				}

			}
		}
	}

	public void Initialize(Configuration config, AppState appState, Configuration.ApplicationRow application)
	{
		HtmlGenericControl parentLegend=new HtmlGenericControl("div");
		pnlBaseMapScroll.Controls.Add(parentLegend);
		HtmlGenericControl legendEntry=new HtmlGenericControl("div");
		parentLegend.Controls.Add(legendEntry);
		legendEntry.Attributes["class"]="LegendEntry";
		HtmlGenericControl legendHeader=new HtmlGenericControl("div");
		legendEntry.Controls.Add(legendHeader);
		legendHeader.Attributes["class"]="LegendHeader";
		HtmlGenericControl name=new HtmlGenericControl("span");
		legendHeader.Controls.Add(name);
		name.Attributes["class"]="LegendName";
		name.InnerText="BaseMaps";

		AddTilesForBaseMap(config, appState, application);
		HtmlGenericControl parentLegend1=new HtmlGenericControl("div");
		pnlBaseMapScroll.Controls.Add(parentLegend1);
		HtmlGenericControl legendEntry1=new HtmlGenericControl("div");
		parentLegend1.Controls.Add(legendEntry1);
		legendEntry1.Attributes["class"]="LegendEntry";
		HtmlGenericControl legendHeader1=new HtmlGenericControl("div");
		legendEntry1.Controls.Add(legendHeader1);
		legendHeader1.Attributes["class"]="LegendHeader";
		HtmlGenericControl name1=new HtmlGenericControl("span");
		legendHeader1.Controls.Add(name1);
		name1.Attributes["class"]="LegendName";
		name1.InnerText="Overlays";
		foreach(Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
		{
			Configuration.MapTabRow mapTabRow=appMapTabRow.MapTabRow;
			AddTilesForOverlay(mapTabRow, appState);
		}
		
	}

	public void AddTilesForBaseMap(Configuration config, AppState appState, Configuration.ApplicationRow application)
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
			radio.Attributes["class"]="LegendCheck RadioCheck";
			radio.Attributes["data-tilegroup"]="None";
			radio.Attributes["group"]=mapTabRow.MapTabID;
			radio.Attributes["name"]=mapTabRow.MapTabID;
			HtmlGenericControl name=new HtmlGenericControl("span");
			legendHeader.Controls.Add(name);
			name.Attributes["class"]="LegendName";
			name.InnerText="None";
		}
	
	}
}
