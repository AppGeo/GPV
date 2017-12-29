  ﻿//  Copyright 2016 Applied Geographics, Inc.
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
  using AppGeo.Clients;
  using System.Web.UI.HtmlControls;
  using System.IO;
  using System.Drawing;
  using System.Data;

public partial class LegendPanel : System.Web.UI.UserControl
{
  private void AddLayerToLegend(string mapTabId, List<CommonLayer> configuredLayers, List<LayerProperties> layerProperties, HtmlGenericControl container, CommonLayer layer)
  {
    int i = configuredLayers.IndexOf(layer);

    if (i < 0)
    {
      return;
    }

    int tileWidth = AppContext.AppSettings.SwatchTileWidth;
    int tileHeight = AppContext.AppSettings.SwatchTileHeight;
    bool expanded = AppContext.AppSettings.LegendExpanded;
    bool collapsed = AppContext.AppSettings.LegendCollapsed;

    HtmlGenericControl legendEntry = new HtmlGenericControl("div");
    container.Controls.Add(legendEntry);
    legendEntry.Attributes["class"] = "LegendEntry";

    HtmlGenericControl legendHeader = new HtmlGenericControl("div");
    legendEntry.Controls.Add(legendHeader);
    legendHeader.Attributes["class"] = "LegendHeader";

    HtmlGenericControl expander = new HtmlGenericControl("span");
    legendHeader.Controls.Add(expander);
    // expander.Attributes["class"] = "LegendExpander " + (collapsed ? "Collapsed" : "Expanded");

    if (layerProperties[i].CheckMode != CheckMode.None)
    {
      HtmlGenericControl visibility = new HtmlGenericControl("span");
      legendHeader.Controls.Add(visibility);
      visibility.Attributes["class"] = "LegendVisibility";

      if (layerProperties[i].CheckMode != CheckMode.Empty)
      {
        HtmlControl check = null;

        if (layerProperties[i].IsExclusive)
        {
          HtmlInputRadioButton radio = new HtmlInputRadioButton();
          radio.Checked = layerProperties[i].CheckMode == CheckMode.Checked;
          radio.Name = String.Format("{0}_{1}", mapTabId, layer.Parent.ID);
          check = radio;
          if (radio.Checked == true)
          {
            expander.Attributes["class"] = "LegendExpander Expanded";
          }
          else
          {
            expander.Attributes["class"] = "LegendExpander Collapsed";
          }
        }
        else
        {
          HtmlInputCheckBox checkBox = new HtmlInputCheckBox();
          checkBox.Checked = layerProperties[i].CheckMode == CheckMode.Checked;
          check = checkBox;
          if (checkBox.Checked == true)
          {
            expander.Attributes["class"] = "LegendExpander Expanded";
          }
          else
          {
            expander.Attributes["class"] = "LegendExpander Collapsed";
          }

        }

        visibility.Controls.Add(check);
        check.Attributes["class"] = "LegendCheck";
        check.Attributes["data-layer"] = layerProperties[i].Tag;
      }
    }

    HtmlGenericControl name = new HtmlGenericControl("span");
    legendHeader.Controls.Add(name);
    name.Attributes["class"] = "LegendName";

    if (!String.IsNullOrEmpty(layerProperties[i].MetaDataUrl))
    {
      HtmlAnchor a = new HtmlAnchor();
      name.Controls.Add(a);
      a.HRef = layerProperties[i].MetaDataUrl;
      a.Target = "metadata";
      a.InnerText = layerProperties[i].Name;
      a.Attributes["class"] = "LegendMetadata";
    }
    else
    {
      name.InnerText = layerProperties[i].Name;
    }

    HtmlGenericControl content = new HtmlGenericControl("div");
    content.Attributes["class"] = "LegendContent";
    if (expander.Attributes["class"] == "LegendExpander Collapsed")
    {
      content.Style["display"] = "none";// collapsed ? "none" : "block";
    }
    else
    {
      content.Style["display"] = "block";// expanded ? "block" : "none";
    }


    switch (layer.Type)
    {
      case CommonLayerType.Group:
        if (layer.Children != null)
        {
          foreach (CommonLayer childLayer in layer.Children)
          {
            AddLayerToLegend(mapTabId, configuredLayers, layerProperties, content, childLayer);
          }
        }
        break;

      case CommonLayerType.Feature:
        int layerIndex = layer.DataFrame.Layers.IndexOf(layer);

        if (layer.Legend != null)
        {
          int n = 0;
          string escapedMapTabId = Server.UrlEncode(mapTabId);

          for (int g = 0; g < layer.Legend.Groups.Count; ++g)
          {
            int classCount = layer.Legend.Groups[g].Classes.Count;

            for (int c = 0; c < classCount; ++c)
            {
              if (!layer.Legend.Groups[g].Classes[c].ImageIsTransparent)
              {
                HtmlGenericControl legendClass = new HtmlGenericControl("div");
                content.Controls.Add(legendClass);
                legendClass.Attributes["class"] = "LegendClass";

                HtmlGenericControl legendSwatch = new HtmlGenericControl("span");
                legendClass.Controls.Add(legendSwatch);
                legendSwatch.Attributes["class"] = "LegendSwatch";
                legendSwatch.Style["background"] = String.Format("transparent url(CompiledSwatch.ashx?maptab={0}&c={1}) no-repeat scroll -{2}px -{3}px",
                  escapedMapTabId, AppContext.ConfigurationKey, tileWidth * layerIndex, tileHeight * n);

                using (MemoryStream stream = new MemoryStream(layer.Legend.Groups[g].Classes[c].Image))
                {
                  using (Bitmap swatch = new Bitmap(stream))
                  {
                    legendClass.Style["height"] = String.Format("{0}px", swatch.Height);
                    legendSwatch.Style["width"] = String.Format("{0}px", swatch.Width);
                    legendSwatch.Style["height"] = String.Format("{0}px", swatch.Height);
                  }
                }

                if (classCount > 1 || layer.Legend.Groups.Count > 1)
                {
                  HtmlGenericControl className = new HtmlGenericControl("span");
                  legendClass.Controls.Add(className);
                  className.Attributes["class"] = "LegendClassName";
                  className.InnerText = layer.Legend.Groups[g].Classes[c].Label;
                }
              }

              n += 1;
            }
          }
        }
        break;

      case CommonLayerType.Annotation:
        if (layer.Children != null)
        {
          foreach (CommonLayer childLayer in layer.Children)
          {
            AddLayerToLegend(mapTabId, configuredLayers, layerProperties, content, childLayer);
          }
        }
        break;
    }

    if (content.Controls.Count == 0)
    {
      expander.Attributes["class"] = "LegendExpander Empty";
    }
    else
    {
      legendEntry.Controls.Add(content);
    }
  }

  private void AddLayers(Configuration.MapTabRow mapTabRow, AppState appState)
  {
    CommonDataFrame dataFrame = AppContext.GetDataFrame(mapTabRow);

        bool isInteractive = !mapTabRow.IsInteractiveLegendNull() && mapTabRow.InteractiveLegend == 1;
    CheckMode checkMode = CheckMode.None;

    List<CommonLayer> configuredLayers = new List<CommonLayer>();
    List<LayerProperties> layerProperties = new List<LayerProperties>();
    List<String> mapTabLayerIds = new List<String>();

    string name = null;
    string metaDataUrl = null;

    StringCollection visibleLayers = isInteractive ? appState.VisibleLayers[mapTabRow.MapTabID] : null;

    // find layers attached via MapTabLayer

    foreach (Configuration.MapTabLayerRow mapTabLayerRow in mapTabRow.GetMapTabLayerRows())
    {
      if (!mapTabLayerRow.IsShowInLegendNull() && mapTabLayerRow.ShowInLegend == 1)
      {
        CommonLayer layer = dataFrame.Layers.FirstOrDefault(lyr => String.Compare(lyr.Name, mapTabLayerRow.LayerRow.LayerName, true) == 0);

        name = mapTabLayerRow.LayerRow.IsDisplayNameNull() ? mapTabLayerRow.LayerRow.LayerName : mapTabLayerRow.LayerRow.DisplayName;
        metaDataUrl = mapTabLayerRow.LayerRow.IsMetaDataURLNull() ? null : mapTabLayerRow.LayerRow.MetaDataURL;
        bool isExclusive = mapTabLayerRow.IsIsExclusiveNull() ? false : mapTabLayerRow.IsExclusive == 1;

        string tag = mapTabLayerRow.LayerID;
        mapTabLayerIds.Add(tag);

        if (isInteractive)
        {
          bool layerVisible = visibleLayers != null && visibleLayers.Contains(mapTabLayerRow.LayerID);
          checkMode = mapTabLayerRow.IsCheckInLegendNull() || mapTabLayerRow.CheckInLegend < 0 ? CheckMode.Empty :
              layerVisible ? CheckMode.Checked : CheckMode.Unchecked;
        }

        configuredLayers.Add(layer);
        layerProperties.Add(new LayerProperties(name, tag, checkMode, isExclusive, metaDataUrl));
      }
    }

    // add group layers as necessary

    for (int i = 0; i < configuredLayers.Count; ++i)
    {
      checkMode = !isInteractive ? CheckMode.None : layerProperties[i].CheckMode == CheckMode.Checked ? CheckMode.Checked : CheckMode.Unchecked;
      CommonLayer parent = configuredLayers[i].Parent;

      while (parent != null)
      {
        int index = configuredLayers.IndexOf(parent);

        if (index < 0)
        {
          configuredLayers.Add(parent);
          layerProperties.Add(new LayerProperties(parent.Name, null, checkMode, false, null));
        }
        else
        {
          if (checkMode == CheckMode.Checked && layerProperties[index].CheckMode == CheckMode.Unchecked)
          {
            layerProperties[index].CheckMode = CheckMode.Checked;
          }
        }

        parent = parent.Parent;
      }
    }

    // create the top level legend control for this map tab

    HtmlGenericControl parentLegend = new HtmlGenericControl("div");
    pnlLayerScroll.Controls.Add(parentLegend);
    parentLegend.Attributes["data-maptab"] = mapTabRow.MapTabID;
    parentLegend.Attributes["class"] = "LegendTop";
    parentLegend.Style["display"] = mapTabRow.MapTabID == appState.MapTab ? "block" : "none";

    // add the Legend controls for the configured layers

    foreach (CommonLayer layer in dataFrame.TopLevelLayers)
    {
      AddLayerToLegend(mapTabRow.MapTabID, configuredLayers, layerProperties, parentLegend, layer);
    }
  }

  public void Initialize(Configuration config, AppState appState, Configuration.ApplicationRow application)
  {
    foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
    {
      Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;
      AddLayers(mapTabRow, appState);
    }
  }

  private class LayerProperties
  {
    public bool IsExclusive = false;
    public string MetaDataUrl = null;
    public string Name = null;
    public string Tag = null;
    public CheckMode CheckMode = CheckMode.None;

    public LayerProperties(string name, string tag, CheckMode checkMode, bool isExclusive, string metaDataUrl)
    {
      Name = name;
      Tag = tag;
      CheckMode = checkMode;
      IsExclusive = isExclusive;
      MetaDataUrl = metaDataUrl;
    }
  }

  public void CreateMapThemes(Configuration.ApplicationRow application, AppState _appState)
  {
    // add map tabs

    foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
    {
      HtmlGenericControl li = new HtmlGenericControl("li");
      phlMapTheme.Controls.Add(li);
      li.InnerHtml = appMapTabRow.MapTabRow.DisplayName.Replace(" ", "&nbsp;");
      li.Attributes["data-maptab"] = appMapTabRow.MapTabID;

      if (_appState.MapTab == appMapTabRow.MapTabID)
      {
        selectedTheme.InnerHtml = appMapTabRow.MapTabRow.DisplayName.Replace(" ", "&nbsp;");
      }
    }
  }
}
