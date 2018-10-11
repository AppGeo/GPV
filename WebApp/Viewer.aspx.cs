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
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using AppGeo.Clients;

public partial class Viewer : CustomStyledPage
{
  private AppState _appState;
  private Configuration _config = null;
  Dictionary<String, List<int>> _inputPrintFields = new Dictionary<String, List<int>>();

  protected void Page_Init(object sender, EventArgs e)
  {
    DateTime lastWriteTime;

    // reference minified stylesheets if not in debug mode

    if (System.Diagnostics.Debugger.IsAttached)
    {
      foreach (string s in MinifiedStylesheetsHandler.GetList())
      {
        head.Controls.Add(MakeStyleReference(s));
      }
    }
    else
    {
      lastWriteTime = MinifiedStylesheetsHandler.GetLastWriteTime();
      head.Controls.Add(MakeStyleReference("Styles/MinifiedStylesheets.ashx" + GetCacheControl(lastWriteTime) + "&ext=.css"));
    }

    // reference minified scripts if not in debug mode

    if (System.Diagnostics.Debugger.IsAttached)
    {
      string query = WebConfigSettings.AllowDevScriptCaching ? "" : GetCacheControl();

      foreach (ScriptItem scriptItem in MinifiedScriptsHandler.GetList())
      {
        head.Controls.Add(MakeScriptReference(scriptItem.FileName + query));
      }
    }
    else
    {
      lastWriteTime = MinifiedScriptsHandler.GetLastWriteTime();
      head.Controls.Add(MakeScriptReference("Scripts/MinifiedScripts.ashx" + GetCacheControl(lastWriteTime) + "&ext=.js"));
    }

    _config = AppContext.GetConfiguration();
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    Response.Cache.SetCacheability(HttpCacheability.NoCache);

    Dictionary<String, String> launchParams = null;

    if (Session["LaunchParams"] != null)
    {
      launchParams = (Dictionary<String, String>)Session["LaunchParams"];
      Session["LaunchParams"] = null;
    }
    else
    {
      launchParams = Request.GetNormalizedParameters();

      if (Request.HttpMethod != "POST" && !launchParams.ContainsKey("keepurl") || (launchParams.Count == 1 && launchParams.ContainsKey("showapps")) ||
      (launchParams.Count == 0 && System.Diagnostics.Debugger.IsAttached))
      {
        Session["LaunchParams"] = launchParams;
        Server.Transfer("StartViewer.aspx", false);
      }
    }

    LoadStateFromLaunchParams(launchParams);

    Configuration.ApplicationRow application = _config.Application.FindByApplicationID(_appState.Application);

    if (!application.IsMetaDescriptionNull())
    {
      AddMetaTag("description", application.MetaDescription);
    }

    if (!application.IsMetaKeywordsNull())
    {
      AddMetaTag("keywords", application.MetaKeywords);
    }

    string toolName = launchParams.ContainsKey("tool") ? launchParams["tool"] : (!application.IsDefaultToolNull() ? application.DefaultTool : null);

    if (String.IsNullOrEmpty(toolName) || toolName.ToLower() == "pan")
    {
      toolName = "identify";
    }

    toolName = toolName.ToLower();

    HtmlControl tool = Page.FindControl("opt" + toolName, false) as HtmlControl;

    if (tool != null)
    {
      tool.Attributes["class"] += " Selected ";

      toolName = toolName == "identify" || toolName == "select" ? "drawline" : "markuptool";
      tool = Page.FindControl("opt" + toolName, false) as HtmlControl;
      tool.Attributes["class"] += " Selected ";
    }

    Title = application.DisplayName;

    SetHelpLink();

    bool isPublic = AppAuthentication.Mode == AuthenticationMode.None;
    ucLegendPanel.Initialize(_config, _appState, application);
    ucBaseMapPanel.Initialize(_config, _appState, application);
    ucSharePanel.Initialize(_config, application);

    if (_appState.FunctionTabs == FunctionTab.None)
    {
      pnlLeft.Style["visibility"] = "hidden";
      pnlMapSizer.Style["left"] = "0px";
    }

    if (_appState.ActiveFunctionTab != FunctionTab.None)
    {
      HtmlControl defaultToolSelect = Page.FindControl("optSelect", false) as HtmlControl;
      HtmlControl defaultToolDraw = Page.FindControl("optMarkupTool", false) as HtmlControl;
      HtmlControl defaultToolIdentify = Page.FindControl("optIdentify", false) as HtmlControl;

      defaultToolSelect.Attributes.Remove("Selected");
      defaultToolDraw.Attributes.Remove("Selected");
      defaultToolIdentify.Attributes.Remove("Selected");

      if (_appState.ActiveFunctionTab == FunctionTab.Selection)
      {
        HtmlControl defaultTool = Page.FindControl("optSelect", false) as HtmlControl;
        defaultToolSelect.Attributes["class"] += " Selected ";
      }
      else if (_appState.ActiveFunctionTab == FunctionTab.Markup)
      {
        HtmlControl defaultTool = Page.FindControl("optMarkupTool", false) as HtmlControl;
        defaultToolDraw.Attributes["class"] += " Selected ";
      }
      else
      {
        HtmlControl defaultTool = Page.FindControl("optIdentify", false) as HtmlControl;
        defaultToolIdentify.Attributes["class"] += " Selected ";
      }
    }

    if ((_appState.FunctionTabs & FunctionTab.Search) == FunctionTab.Search)
    {
      tabSearch.Style["display"] = "block";
      ucSearchPanel.Initialize(application);

      if (_appState.ActiveFunctionTab == FunctionTab.Search)
      {
        pnlSearch.Style["display"] = "block";
        tabSearch.Attributes["class"] += " active";
      }
    }

    if ((_appState.FunctionTabs & FunctionTab.Selection) == FunctionTab.Selection)
    {
      tabSelection.Style["display"] = "block";
      ucSelectionPanel.Initialize(launchParams);

      if (_appState.ActiveFunctionTab == FunctionTab.Selection)
      {
        pnlSelection.Style["display"] += "block";
        tabSelection.Attributes["class"] += " active";
      }
    }

    if ((_appState.FunctionTabs & FunctionTab.Legend) == FunctionTab.Legend)
    {
      tabLegend.Style["display"] = "block";

      if (_appState.ActiveFunctionTab == FunctionTab.Legend)
      {
        pnlLegend.Style["display"] = "block";
        tabLegend.Attributes["class"] += " active";
      }
    }

    if ((_appState.FunctionTabs & FunctionTab.Location) == FunctionTab.Location)
    {
      tabLocation.Style["display"] = "block";
      ucLocationPanel.Initialize(_config, _appState, application);

      if (_appState.ActiveFunctionTab == FunctionTab.Location)
      {
        pnlLocation.Style["display"] = "block";
        tabLocation.Attributes["class"] += " active";
      }
    }

    if ((_appState.FunctionTabs & FunctionTab.Markup) == FunctionTab.Markup)
    {
      tabMarkup.Style["display"] = "block";
      ucMarkupPanel.Initialize(_config, _appState, application);

      if (_appState.ActiveFunctionTab == FunctionTab.Markup)
      {
        pnlMarkup.Style["display"] = "block";
        tabMarkup.Attributes["class"] += " active";
      }
    }

    if ((_appState.FunctionTabs & FunctionTab.Share) == FunctionTab.Share)
    {
      tabShare.Style["display"] = "block";

      if (_appState.ActiveFunctionTab == FunctionTab.Share)
      {
        pnlShare.Style["display"] = "block";
        tabShare.Attributes["class"] += " active";
      }
    }

    ShowLevelSelector(application);

    int? initialZoomLevel = GetInitialZoomLevel(launchParams);

    CreateAppStateScript(application, initialZoomLevel);
    CreateActiveSelectionStyle();

    spnVersion.InnerText = Version.ToString();

    if (!_config.AppSettings.ShowLogo)
    {
      logo.Visible = false;
    }
    else
    {
      logosmall.Visible = false;
    }

    TrackingManager.TrackUse(launchParams);
  }

  private void AddMetaTag(string name, string content)
  {
    HtmlMeta meta = new HtmlMeta();
    meta.Name = name;
    meta.Content = content;
    head.Controls.AddAt(0, meta);
  }

  private void CreateActiveSelectionStyle()
  {
    HtmlGenericControl style = new HtmlGenericControl("style");
    head.Controls.Add(style);
    style.InnerHtml = String.Format(".ActiveGridRowSelect, .ActiveGridRowSelect:hover {{ background-color: {0} }}", ColorTranslator.ToHtml(AppContext.AppSettings.ActiveColorUI));
  }

  private void CreateAppStateScript(Configuration.ApplicationRow application, int? initialZoomLevel)
  {
    string script = "var GPV = (function (gpv) {{ gpv.configuration = {0}; gpv.settings = {1}; gpv.appState = {2}; gpv.initialZoomLevel = {3}; gpv.checkActive = {4}; return gpv; }})(GPV || {{}});";
    string zoomLevel = initialZoomLevel.HasValue ? initialZoomLevel.ToString() : "null";
    string checkActive = (AppAuthentication.Mode != AuthenticationMode.None).ToString().ToLower();

    HtmlGenericControl scriptElem = new HtmlGenericControl("script");
    head.Controls.Add(scriptElem);
    scriptElem.Attributes["type"] = "text/javascript";
    scriptElem.InnerHtml = String.Format(script, application.ToJson(), AppContext.AppSettings.ToJson(), _appState.ToJson(), zoomLevel, checkActive);
  }

  private string GetCacheControl()
  {
    return GetCacheControl(DateTime.Now);
  }

  private string GetCacheControl(DateTime lastWriteTime)
  {
    return String.Format("?t={0:0}", (lastWriteTime - new DateTime(2000, 1, 1)).TotalSeconds);
  }

  private int? GetInitialZoomLevel(Dictionary<string, string> launchParams)
  {
    // assumes zoomlevel was already validated in LoadStateFromLaunchParams

    int? level = null;

    if (AppContext.AppSettings.MapCoordinateSystem.IsWebMercator && launchParams.ContainsKey("zoomlevel"))
    {
      level = Convert.ToInt32(launchParams["zoomlevel"]);
    }

    return level;
  }

  private void LoadStateFromLaunchParams(Dictionary<String, String> launchParams)
  {
    _appState = new AppState();
    AppSettings appSettings = AppContext.AppSettings;

    // recreate application state from a compressed string if provided

    if (launchParams.ContainsKey("state"))
    {
      RestoreState(launchParams["state"]);
      return;
    }

    // otherwise, if no application was specified, show an error

    if (String.IsNullOrEmpty(appSettings.DefaultApplication) && !launchParams.ContainsKey("application"))
    {
      ShowError("An application has not been specified");
    }

    // check settings that were provided in the GPVSetting table

    if (appSettings.AdminEmail == null)
    {
      ShowError("AdminEmail has not been provided in the GPVSetting table");
    }

    if (appSettings.DefaultFullExtent == null)
    {
      ShowError("FullExtent has not been provided or is invalid in the GPVSetting table");
    }

    if (appSettings.MapCoordinateSystem == null)
    {
      ShowError("MapProjection is invalid in the GPVSetting table");
    }

    if (appSettings.MeasureCoordinateSystem == null)
    {
      ShowError("MeasureProjection is invalid in the GPVSetting table");
    }

    // otherwise, load the query string into the application state
    // and validate against the configuration

    // === application ===

    _appState.Application = launchParams.ContainsKey("application") ? launchParams["application"] : appSettings.DefaultApplication;

    Configuration.ApplicationRow application = _config.Application.FindByApplicationID(_appState.Application);
    Configuration.MapTabRow mapTab;

    // the Application must exist

    if (application == null)
    {
      ShowError("Application '" + _appState.Application + "' does not exist");
    }

    // the user must be authorized to run the application

    string roles = application.IsAuthorizedRolesNull() ? "public" : application.AuthorizedRoles;

    if (!AppUser.RoleIsInList(roles))
    {
      ShowError("You are not authorized to view the application '" + _appState.Application + "'");
    }

    _appState.Application = application.ApplicationID;
    _appState.Extent = application.GetFullExtentEnvelope();

    // get the default visible layers for all interactive legends

    foreach (Configuration.ApplicationMapTabRow applicationMapTab in application.GetApplicationMapTabRows())
    {
      mapTab = applicationMapTab.MapTabRow;
      string key = mapTab.MapTabID;
      StringCollection values;

      if (!mapTab.IsInteractiveLegendNull() && mapTab.InteractiveLegend == 1)
      {
        values = new StringCollection();

        foreach (Configuration.MapTabLayerRow mapTabLayer in mapTab.GetMapTabLayerRows())
        {
          if (!mapTabLayer.IsShowInLegendNull() && mapTabLayer.ShowInLegend == 1 &&
          !mapTabLayer.IsCheckInLegendNull() && mapTabLayer.CheckInLegend == 1)
          {
            values.Add(mapTabLayer.LayerID);
          }
        }

        _appState.VisibleLayers.Add(key, values);
      }

      values = new StringCollection();

      foreach (Configuration.MapTabTileGroupRow mapTabTileGroup in mapTab.GetMapTabTileGroupRows())
      {
        if (!mapTabTileGroup.IsCheckInLegendNull() && mapTabTileGroup.CheckInLegend == 1)
        {
          values.Add(mapTabTileGroup.TileGroupID);
        }
      }

      _appState.VisibleTiles.Add(key, values);
    }

    // === maptab ===

    mapTab = null;

    if (launchParams.ContainsKey("maptab"))
    {
      _appState.MapTab = launchParams["maptab"];
      mapTab = _config.MapTab.FindByMapTabID(_appState.MapTab);

      // the MapTab must exist

      if (mapTab == null)
      {
        ShowError("Map tab '" + _appState.MapTab + "' does not exist");
      }

      _appState.MapTab = mapTab.MapTabID;
      string filter = "ApplicationID = '" + application.ApplicationID + "' and MapTabID = '" + mapTab.MapTabID + "'";

      // the Application must contain the MapTab

      if (_config.ApplicationMapTab.Select(filter).Length == 0)
      {
        ShowError("Application '" + application.ApplicationID + "' does not contain map tab '" + mapTab.MapTabID + "'");
      }
    }

    // get the default MapTab from the Application if available

    if (mapTab == null)
    {
      if (!application.IsDefaultMapTabNull())
      {
        mapTab = (Configuration.MapTabRow)(_config.MapTab.Select("MapTabID = '" + application.DefaultMapTab + "'")[0]);
      }
      else
      {
        Configuration.ApplicationMapTabRow applicationMapTab = (Configuration.ApplicationMapTabRow)_config.ApplicationMapTab.Select("ApplicationID = '" + _appState.Application + "'")[0];
        mapTab = applicationMapTab.MapTabRow;
      }

      _appState.MapTab = mapTab.MapTabID;
    }

    // === layers on ===

    if (launchParams.ContainsKey("layerson"))
    {
      StringCollection layersOn = StringCollection.FromString(launchParams["layerson"], ',');
      StringCollection visibleLayers = _appState.VisibleLayers[mapTab.MapTabID];

      foreach (string layerId in layersOn)
      {
        Configuration.MapTabLayerRow mapTabLayer = mapTab.GetMapTabLayerRows().FirstOrDefault(o => o.LayerID == layerId);

        if (mapTabLayer == null)
        {
          ShowError(String.Format("Layer \"{0}\" does not exist in map tab \"{1}\"", layerId, mapTab.MapTabID));
        }
        else if (!mapTabLayer.IsShowInLegendNull() && mapTabLayer.ShowInLegend == 1 &&
        !mapTabLayer.IsCheckInLegendNull() && mapTabLayer.CheckInLegend >= 0 &&
        !visibleLayers.Contains(layerId))
        {
          visibleLayers.Add(layerId);
        }
      }
    }

    // === layers off ===

    if (launchParams.ContainsKey("layersoff"))
    {
      StringCollection layersOff = StringCollection.FromString(launchParams["layersoff"], ',');
      StringCollection visibleLayers = _appState.VisibleLayers[mapTab.MapTabID];

      foreach (string layerId in layersOff)
      {
        Configuration.MapTabLayerRow mapTabLayer = mapTab.GetMapTabLayerRows().FirstOrDefault(o => o.LayerID == layerId);

        if (mapTabLayer == null)
        {
          ShowError(String.Format("Layer \"{0}\" does not exist in map tab \"{1}\"", layerId, mapTab.MapTabID));
        }
        else if (!mapTabLayer.IsShowInLegendNull() && mapTabLayer.ShowInLegend == 1 &&
        !mapTabLayer.IsCheckInLegendNull() && mapTabLayer.CheckInLegend >= 1 &&
        visibleLayers.Contains(layerId))
        {
          visibleLayers.Remove(layerId);
        }
      }
    }

    // === tiles on ===

    if (launchParams.ContainsKey("tileson"))
    {
      StringCollection tilesOn = StringCollection.FromString(launchParams["tileson"], ',');
      StringCollection visibleTiles = _appState.VisibleTiles[mapTab.MapTabID];

      foreach (string groupId in tilesOn)
      {
        Configuration.MapTabTileGroupRow mapTabTileGroup = mapTab.GetMapTabTileGroupRows().FirstOrDefault(o => o.TileGroupID == groupId);

        if (mapTabTileGroup == null)
        {
          ShowError(String.Format("Tile group \"{0}\" does not exist in map tab \"{1}\"", groupId, mapTab.MapTabID));
        }
        else if (!visibleTiles.Contains(groupId))
        {
          visibleTiles.Add(groupId);
        }
      }
    }

    // === tiles off ===

    if (launchParams.ContainsKey("tilesoff"))
    {
      StringCollection tilesOff = StringCollection.FromString(launchParams["tilesoff"], ',');
      StringCollection visibleTiles = _appState.VisibleTiles[mapTab.MapTabID];

      foreach (string groupId in tilesOff)
      {
        Configuration.MapTabTileGroupRow mapTabTileGroup = mapTab.GetMapTabTileGroupRows().FirstOrDefault(o => o.TileGroupID == groupId);

        if (mapTabTileGroup == null)
        {
          ShowError(String.Format("Tile group \"{0}\" does not exist in map tab \"{1}\"", groupId, mapTab.MapTabID));
        }
        else if (visibleTiles.Contains(groupId))
        {
          visibleTiles.Remove(groupId);
        }
      }
    }

    // === level ===

    Configuration.ZoneLevelRow zoneLevel = application.ZoneLevelRow;
    Configuration.LevelRow[] levels = zoneLevel != null ? zoneLevel.GetLevelRows() : new Configuration.LevelRow[0];

    if (!application.IsDefaultLevelNull())
    {
      _appState.Level = application.DefaultLevel;
    }
    else if (levels.Length > 0)
    {
      _appState.Level = levels[0].LevelID;
    }

    if (launchParams.ContainsKey("level"))
    {
      if (levels.Length == 0)
      {
        ShowError("A level was specified but levels have not been configured for this application");
      }
      else if (!levels.Any(o => o.LevelID == launchParams["level"]))
      {
        ShowError(String.Format("Invalid level '{0}' specified", launchParams["level"]));
      }
      else
      {
        _appState.Level = launchParams["level"];
      }
    }

    // === search ===

    if (!application.IsDefaultSearchNull())
    {
      _appState.Search = application.DefaultSearch;
    }

    if (launchParams.ContainsKey("search"))
    {
      // the search must exist

      Configuration.SearchRow search = ((Configuration)mapTab.Table.DataSet).Search.FirstOrDefault(o => String.Compare(o.SearchID, launchParams["search"], true) == 0);

      if (search == null)
      {
        ShowError("No search exists with the specified ID");
      }

      // a MapTab must be specified or available by default

      if (mapTab == null)
      {
        ShowError("When providing a search, a map tab must also be specified or the application must have a default map tab defined");
      }

      // the search must be linked to a layer in the MapTab

      string[] layerIDs = mapTab.GetMapTabLayerRows().Select(o => o.LayerID).ToArray();

      if (!layerIDs.Contains(search.LayerID))
      {
        ShowError("The specified search is not linked to a layer in map tab " + mapTab.MapTabID);
      }

      _appState.Search = search.SearchID;
    }

    // === action ===

    if (!application.IsDefaultActionNull())
    {
      _appState.Action = (Action)Enum.Parse(typeof(Action), application.DefaultAction, true);
    }

    if (launchParams.ContainsKey("action"))
    {
      try
      {
        _appState.Action = (Action)Enum.Parse(typeof(Action), launchParams["action"], true);
      }
      catch
      {
        ShowError("Invalid action '" + launchParams["action"] + "', must be either " + EnumHelper.ToChoiceString(typeof(Action)));
      }
    }

    // === targetlayer ===

    Configuration.LayerRow targetLayer = null;

    if (launchParams.ContainsKey("targetlayer"))
    {
      _appState.TargetLayer = launchParams["targetlayer"];
      targetLayer = _config.Layer.FindByLayerID(_appState.TargetLayer);

      // the Layer must exist

      if (targetLayer == null)
      {
        ShowError("Target layer '" + _appState.TargetLayer + "' does not exist");
      }

      _appState.TargetLayer = targetLayer.LayerID;

      // a MapTab must be specified or available by default

      if (mapTab == null)
      {
        ShowError("When providing a target layer, a map tab must also be specified or the application must have a default map tab defined");
      }

      string filter = "MapTabID = '" + mapTab.MapTabID + "' and LayerID = '" + targetLayer.LayerID + "'";
      DataRow[] rows = _config.MapTabLayer.Select(filter);

      // the MapTab must contain the Layer

      if (rows.Length == 0)
      {
        ShowError("Layer '" + targetLayer.LayerID + "' does not exist in map tab '" + mapTab.MapTabID + "'");
      }

      Configuration.MapTabLayerRow link = (Configuration.MapTabLayerRow)rows[0];

      // the Layer must be an allowed as a target in the MapTab

      if (link.IsAllowTargetNull() || link.AllowTarget <= 0)
      {
        ShowError("Layer '" + targetLayer.LayerID + "' is not allowed to be a target layer for map tab '" + mapTab.MapTabID + "'");
      }
    }

    // get the default target Layer from the Application

    if (targetLayer == null && !application.IsDefaultTargetLayerNull())
    {
      targetLayer = (Configuration.LayerRow)(_config.Layer.Select("LayerID = '" + application.DefaultTargetLayer + "'")[0]);
      _appState.TargetLayer = targetLayer.LayerID;
    }

    // === targetids ===

    if (launchParams.ContainsKey("targetids"))
    {
      if (launchParams.ContainsKey("targetparams"))
      {
        ShowError("When providing target IDs, target parameters are not allowed");
      }

      // action must be Select

      if (_appState.Action != Action.Select)
      {
        ShowError("When providing target IDs, action must be 'select'");
      }

      // a target Layer must be available

      if (targetLayer == null)
      {
        ShowError("When providing target IDs, a target layer must also be specified or the application must have a default target layer defined");
      }

      _appState.TargetIds = StringCollection.FromString(launchParams["targetids"], ',');
    }

    // === targetparams ===

    if (launchParams.ContainsKey("targetparams"))
    {
      // action must be Select

      if (_appState.Action != Action.Select)
      {
        ShowError("When providing target parameters, action must be 'select'");
      }

      // a target Layer must be available

      if (targetLayer == null)
      {
        ShowError("When providing target parameters, a target layer must also be specified or the application must have a default target layer defined");
      }

      if (!targetLayer.GetLayerFunctionRows().Any(o => o.FunctionName == "targetparams"))
      {
        ShowError("The target layer has not been configured to accept target parameters");
      }

      _appState.TargetIds = targetLayer.GetTargetIds(launchParams["targetparams"]);
    }

    // === proximity ===

    Configuration.ProximityRow proximity = null;

    if (launchParams.ContainsKey("proximity"))
    {
      _appState.Proximity = launchParams["proximity"];
      proximity = _config.Proximity.FindByProximityID(_appState.Proximity);

      // the Proximity must exist

      if (proximity == null)
      {
        ShowError(String.Format("Proximity '{0}' does not exist", _appState.Proximity));
      }

      _appState.Proximity = proximity.ProximityID;

      // a target Layer must be available

      if (targetLayer == null)
      {
        ShowError("When providing a proximity, a target layer must also be specified or the application must have a default target layer defined");
      }

      // the Proximity must be valid for the target Layer

      bool invalid = false;
      bool proximityAttachedToLayer = _config.LayerProximity.Select(String.Format("LayerID = '{0}' and ProximityID = '{1}'", targetLayer.LayerID, _appState.Proximity)).Length == 1;

      if (proximity.IsIsDefaultNull() || proximity.IsDefault == 1)
      {
        bool layerHasProximities = _config.LayerProximity.Select(String.Format("LayerID = '{0}'", targetLayer.LayerID)).Length > 0;
        invalid = layerHasProximities && !proximityAttachedToLayer;
      }
      else
      {
        invalid = !proximityAttachedToLayer;
      }

      if (invalid)
      {
        ShowError(String.Format("Proximity '{0}' is not valid for target layer '{1}'", _appState.Proximity, _appState.TargetLayer));
      }
    }

    // get the default Proximity from the Application or configuration

    if (proximity == null && !application.IsDefaultProximityNull())
    {
      proximity = (Configuration.ProximityRow)(_config.Proximity.Select(String.Format("ProximityID = '{0}'", application.DefaultProximity))[0]);
      _appState.Proximity = proximity.ProximityID;
    }

    // === selectionlayer ===

    Configuration.LayerRow selectionLayer = null;

    if (launchParams.ContainsKey("selectionlayer"))
    {
      _appState.SelectionLayer = launchParams["selectionlayer"];
      selectionLayer = _config.Layer.FindByLayerID(_appState.SelectionLayer);

      // the Layer must exist

      if (selectionLayer == null)
      {
        ShowError("Selection layer '" + _appState.SelectionLayer + "' does not exist");
      }

      _appState.SelectionLayer = selectionLayer.LayerID;

      // a MapTab must be specified or available by default

      if (mapTab == null)
      {
        ShowError("When providing a selection layer, a map tab must also be specified or the application must have a default map tab defined");
      }

      // a target Layer must be available

      if (targetLayer == null)
      {
        ShowError("When providing a selection layer, a target layer must also be specified or the application must have a default target layer defined");
      }

      string filter = "MapTabID = '" + mapTab.MapTabID + "' and LayerID = '" + selectionLayer.LayerID + "'";
      DataRow[] rows = _config.MapTabLayer.Select(filter);

      // the MapTab must contain the Layer

      if (rows.Length == 0)
      {
        ShowError("Layer '" + selectionLayer.LayerID + "' does not exist in map tab '" + mapTab.MapTabID + "'");
      }

      Configuration.MapTabLayerRow link = (Configuration.MapTabLayerRow)rows[0];

      // the Layer must be allowed for selection in the MapTab

      if (link.IsAllowSelectionNull() || link.AllowSelection <= 0)
      {
        ShowError("Layer '" + selectionLayer.LayerID + "' is not allowed to be a selection layer for map tab '" + mapTab.MapTabID + "'");
      }
    }

    // get the default selection Layer from the Application

    if (selectionLayer == null && !application.IsDefaultSelectionLayerNull())
    {
      selectionLayer = (Configuration.LayerRow)(_config.Layer.Select("LayerID = '" + application.DefaultSelectionLayer + "'")[0]);
      _appState.SelectionLayer = selectionLayer.LayerID;
    }

    // === selectionids ===

    if (launchParams.ContainsKey("selectionids"))
    {
      if (_appState.TargetIds.Count > 0)
      {
        ShowError("When providing target IDs or target parameters, selection IDs are not allowed");
      }

      // a selection Layer must be available

      if (selectionLayer == null)
      {
        ShowError("When providing selection IDs, a selection layer must also be specified or the application must have a default selection layer defined");
      }

      _appState.SelectionIds = StringCollection.FromString(launchParams["selectionids"], ',');

      if (_appState.Action == Action.FindAllWithin && String.IsNullOrEmpty(_appState.Proximity))
      {
        ShowError("When action is 'findallwithin' and selection IDs are provided, a proximity must also be specified or the application must have a default proximity defined");
      }

      if (_appState.Action != Action.Select)
      {
        _appState.SelectionManager.SelectTargets();
      }
    }

    // zoom to the extent of the target and/or selection features

    if (_appState.TargetIds.Count > 0 || _appState.SelectionIds.Count > 0)
    {
      Envelope extent = new Envelope();

      if (_appState.TargetIds.Count > 0)
      {
        extent.ExpandToInclude(_appState.SelectionManager.GetExtent(FeatureType.Target));
      }

      if (_appState.SelectionIds.Count > 0)
      {
        extent.ExpandToInclude(_appState.SelectionManager.GetExtent(FeatureType.Selection));
      }

      if (!extent.IsNull)
      {
        extent.ScaleBy(1.2);
        _appState.Extent = extent;
      }
    }

    // === activemapid ===

    if (launchParams.ContainsKey("activemapid"))
    {
      string activeMapId = launchParams["activemapid"];

      if (_appState.TargetIds.Contains(activeMapId))
      {
        _appState.ActiveMapId = activeMapId;
      }
    }

    // === activedataid ===

    if (launchParams.ContainsKey("activedataid"))
    {
      _appState.ActiveDataId = launchParams["activedataid"];
    }
    else
    {
      if (_appState.ActiveMapId.Length > 0)
      {
        _appState.ActiveDataId = _appState.ActiveMapId;
      }
    }

    // === query ===

    Configuration.QueryRow query = null;

    if (launchParams.ContainsKey("query"))
    {
      query = _config.Query.FindByQueryID(launchParams["query"]);

      // the Query must exist

      if (query == null)
      {
        ShowError("Query '" + launchParams["query"] + "' does not exist");
      }

      // a target Layer must be available

      if (targetLayer == null)
      {
        ShowError("When providing a query, a target layer must also be specified or the application must have a default target layer defined");
      }

      // the target Layer must contain the Query

      if (query.LayerID != targetLayer.LayerID)
      {
        ShowError("Target layer '" + targetLayer.LayerID + "' does not contain query '" + query.QueryID + "'");
      }
    }
    else if (!String.IsNullOrEmpty(_appState.TargetLayer))
    {
      query = _config.Query.Where(o => o.LayerID == _appState.TargetLayer).OrderBy(o => o.SequenceNo).FirstOrDefault();
    }

    if (query != null)
    {
      _appState.Query = query.QueryID;

      // a single TargetId was specified without ActiveMapId and ActiveDataId, try to fetch a single data ID for it
      // from the query stored procedure; if found set ActiveMapId and ActiveDataId

      if (_appState.TargetIds.Count == 1 && String.IsNullOrEmpty(_appState.ActiveMapId) && String.IsNullOrEmpty(_appState.ActiveDataId))
      {
        string mapId = _appState.TargetIds[0];
        string dataId = null;

        using (OleDbCommand command = query.GetDatabaseCommand())
        {
          command.Parameters[0].Value = mapId;

          if (command.Parameters.Count > 1)
          {
            command.Parameters[1].Value = AppUser.GetRole();
          }

          try
          {
            using (OleDbDataReader reader = command.ExecuteReader())
            {
              if (reader.Read())
              {
                try
                {
                  int dataIdColumn = reader.GetOrdinal("DataID");
                  dataId = reader.GetValue(dataIdColumn).ToString();
                }
                catch
                {
                  dataId = mapId;
                }
              }

              if (reader.Read())
              {
                dataId = null;
              }
            }
          }
          catch { }

          command.Connection.Close();
        }

        if (!String.IsNullOrEmpty(dataId))
        {
          _appState.ActiveMapId = mapId;
          _appState.ActiveDataId = dataId;
        }
      }
    }

    // === datatab ===

    if (launchParams.ContainsKey("datatab"))
    {
      _appState.DataTab = launchParams["datatab"];
      Configuration.DataTabRow dataTab = _config.DataTab.FindByDataTabID(_appState.DataTab);

      // the DataTab must exist

      if (dataTab == null)
      {
        ShowError("Data tab '" + _appState.DataTab + "' does not exist");
      }

      _appState.DataTab = dataTab.DataTabID;

      // a target Layer must be available

      if (targetLayer == null)
      {
        ShowError("When providing a data tab, a target layer must also be specified or the application must have a default target layer defined");
      }

      // the target Layer must contain the DataTab

      if (dataTab.LayerID != targetLayer.LayerID)
      {
        ShowError("Target layer '" + targetLayer.LayerID + "' does not contain data tab '" + dataTab.DataTabID + "'");
      }
    }

    // === function tabs ===

    if (!application.IsFunctionTabsNull())
    {
      _appState.FunctionTabs = (FunctionTab)Enum.Parse(typeof(FunctionTab), application.FunctionTabs, true);
    }

    if (launchParams.ContainsKey("functiontabs"))
    {
      string[] tabs = launchParams["functiontabs"].ToLower().Split(',');
      FunctionTab newTabs = FunctionTab.None;

      foreach (string tab in tabs)
      {
        FunctionTab functionTab = FunctionTab.None;

        try
        {
          functionTab = (FunctionTab)Enum.Parse(typeof(FunctionTab), tab, true);
        }
        catch
        {
          ShowError("Invalid function tab '" + tab + "', must be either " + EnumHelper.ToChoiceString(typeof(FunctionTab)));
        }

        if (functionTab == FunctionTab.All && _appState.FunctionTabs != FunctionTab.All)
        {
          ShowError("Use of all function tabs is not allowed in this application");
        }

        if (functionTab != FunctionTab.None && (functionTab & _appState.FunctionTabs) == FunctionTab.None)
        {
          ShowError("Function tab '" + tab + "', not allowed in this application");
        }

        newTabs |= functionTab;
      }

      _appState.FunctionTabs = newTabs;
    }

    // === active function tab ===

    string activeTab = !application.IsDefaultFunctionTabNull() ? application.DefaultFunctionTab : null;

    if (launchParams.ContainsKey("activefunctiontab"))
    {
      activeTab = launchParams["activefunctiontab"];
    }

    if (!String.IsNullOrEmpty(activeTab))
    {
      FunctionTab functionTab = FunctionTab.None;

      try
      {
        functionTab = (FunctionTab)Enum.Parse(typeof(FunctionTab), activeTab, true);
      }
      catch
      {
        ShowError("Invalid active function tab '" + activeTab + "', must be either " + EnumHelper.ToChoiceString(typeof(FunctionTab)));
      }

      if (functionTab != FunctionTab.None && (functionTab & _appState.FunctionTabs) == FunctionTab.None)
      {
        ShowError("Function tab '" + activeTab + "' cannot be activated because is not present in this application");
      }

      _appState.ActiveFunctionTab = functionTab;
    }

    // === markup category ===

    Configuration.ApplicationMarkupCategoryRow markupCategory = null;

    if (launchParams.ContainsKey("markupcategory"))
    {
      string filter = String.Format("ApplicationID = '{0}' and CategoryID = '{1}'", application.ApplicationID, launchParams["markupcategory"]);
      DataRow[] rows = _config.ApplicationMarkupCategory.Select(filter);

      if (rows.Length > 0)
      {
        markupCategory = (Configuration.ApplicationMarkupCategoryRow)rows[0];
        _appState.MarkupCategory = markupCategory.CategoryID;
      }
      else
      {
        ShowError("Markup category '" + launchParams["markupcategory"] + "' is not available in application '" + application.ApplicationID + "'");
      }
    }
    else
    {
      Configuration.ApplicationMarkupCategoryRow[] markupCategories = application.GetApplicationMarkupCategoryRows();

      if (markupCategories.Length > 0)
      {
        markupCategory = markupCategories[0];
        _appState.MarkupCategory = markupCategories[0].CategoryID;
      }
    }

    // === markup groups ===

    if (launchParams.ContainsKey("markupgroup") || launchParams.ContainsKey("markupgroups"))
    {
      List<String> markupGroups = new List<String>(launchParams.ContainsKey("markupgroups") ? launchParams["markupgroups"].Split(',') : new string[] { launchParams["markupgroup"] });

      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        foreach (string markupGroup in markupGroups)
        {
          int groupId = 0;
          Int32.TryParse(markupGroup, out groupId);

          if (groupId < 1)
          {
            ShowError("Invalid markup group ID specified");
          }

          if (markupCategory == null)
          {
            ShowError("Cannot show the specified markup group; the application does not contain any markup categories");
          }

          string sql = String.Format("select CategoryID from {0}MarkupGroup where GroupID = {1}",
          WebConfigSettings.ConfigurationTablePrefix, groupId);
          string categoryID = null;

          using (OleDbCommand command = new OleDbCommand(sql, connection))
          {
            categoryID = command.ExecuteScalar() as String;
          }

          if (categoryID != markupCategory.CategoryID)
          {
            ShowError(String.Format("Markup group {0} is not a member of markup category '{1}'", markupGroup, markupCategory.CategoryID));
          }
        }

        _appState.MarkupGroups = new StringCollection(markupGroups);
        _appState.Extent = MarkupManager.GetExtent(_appState.MarkupGroups.Cast<String>().Select(o => Convert.ToInt32(o)).ToArray());
      }
    }

    // === zone ===

    if (launchParams.ContainsKey("zone"))
    {
      if (launchParams.ContainsKey("extent"))
      {
        ShowError("When providing a zone, an extent is not allowed");
      }

      if (launchParams.ContainsKey("centerx"))
      {
        ShowError("When providing a zone, a center X is not allowed");
      }

      if (launchParams.ContainsKey("centery"))
      {
        ShowError("When providing a zone, a center Y is not allowed");
      }

      if (launchParams.ContainsKey("centerlat"))
      {
        ShowError("When providing a zone, a center latitude is not allowed");
      }

      if (launchParams.ContainsKey("centerlon"))
      {
        ShowError("When providing a zone, a center longitude is not allowed");
      }

      Configuration.ZoneRow[] zones = zoneLevel != null ? zoneLevel.GetZoneRows() : new Configuration.ZoneRow[0];

      if (zones.Length == 0)
      {
        ShowError("A zone was specified but zones have not been configured for this application");
      }
      else if (!zones.Any(o => o.ZoneID == launchParams["zone"]))
      {
        ShowError(String.Format("Invalid zone '{0}' specified", launchParams["zone"]));
      }

      _appState.Extent = mapTab.GetZoneExtent(launchParams["zone"]);

      if (_appState.Extent.IsNull)
      {
        ShowError("Could not find the specified zone");
      }
      else
      {
        _appState.Extent.ScaleBy(1.2);
      }
    }

    // === extent ===

    if (launchParams.ContainsKey("extent"))
    {
      if (launchParams.ContainsKey("mapscale"))
      {
        ShowError("When providing an extent, a map scale is not allowed");
      }

      if (launchParams.ContainsKey("zoomlevel"))
      {
        ShowError("When providing an extent, a zoom level is not allowed");
      }

      if (launchParams.ContainsKey("centerx"))
      {
        ShowError("When providing an extent, a center X is not allowed");
      }

      if (launchParams.ContainsKey("centery"))
      {
        ShowError("When providing an extent, a center Y is not allowed");
      }

      if (launchParams.ContainsKey("centerlat"))
      {
        ShowError("When providing an extent, a center latitude is not allowed");
      }

      if (launchParams.ContainsKey("centerlon"))
      {
        ShowError("When providing an extent, a center longitude is not allowed");
      }

      if (launchParams.ContainsKey("scaleby"))
      {
        ShowError("When providing an extent, 'scale by' is not allowed");
      }

      try
      {
        string[] ext = launchParams["extent"].Split(',');

        double xmin = Convert.ToDouble(ext[0]);
        double ymin = Convert.ToDouble(ext[1]);
        double xmax = Convert.ToDouble(ext[2]);
        double ymax = Convert.ToDouble(ext[3]);

        _appState.Extent = new Envelope(new Coordinate(xmin, ymin), new Coordinate(xmax, ymax));
      }
      catch
      {
        ShowError("Invalid extent specified");
      }
    }

    // === mapscale ===

    if (launchParams.ContainsKey("mapscale"))
    {
      if (launchParams.ContainsKey("zoomlevel"))
      {
        ShowError("When providing a map scale, a zoom level is not allowed");
      }

      if (launchParams.ContainsKey("scaleby"))
      {
        ShowError("When providing a map scale, 'scale by' is not allowed");
      }

      try
      {
        double mapScale = Convert.ToDouble(launchParams["mapscale"]);

        if (mapScale <= 0)
        {
          throw new Exception();
        }
      }
      catch
      {
        ShowError("Invalid map scale specified");
      }

      // NOTE:  The map scale is passed to the viewer as a JavaScript variable, and not through AppState,
      // because it depends to the size of the map control.  It is passed to directly to the printable map
      // via AppState below.
    }

    // === zoomlevel ===

    if (launchParams.ContainsKey("zoomlevel"))
    {
      if (launchParams.ContainsKey("scaleby"))
      {
        ShowError("When providing a zoom level, 'scale by' is not allowed");
      }

      double level = 0;

      try
      {
        if (appSettings.MapCoordinateSystem.IsWebMercator)
        {
          level = Convert.ToInt32(launchParams["zoomlevel"]);
        }
        else
        {
          level = Convert.ToDouble(launchParams["zoomlevel"]);
        }
      }
      catch
      {
        ShowError("Invalid zoom level specified");
      }

      Envelope extent = application.GetFullExtentEnvelope();
      extent.Translate(_appState.Extent.Centre.X - extent.Centre.X, _appState.Extent.Centre.Y - extent.Centre.Y);

      // NOTE: for Web Mercator the zoom level is passed as a JavaScript variable to the map control

      if (!appSettings.MapCoordinateSystem.IsWebMercator)
      {
        extent.ScaleBy(1 / Math.Pow(2, level));
      }

      _appState.Extent = extent;
    }

    // === centerx ===

    if (launchParams.ContainsKey("centerx"))
    {
      if (launchParams.ContainsKey("scaleby"))
      {
        ShowError("When providing center X, 'scale by' is not allowed");
      }

      if (launchParams.ContainsKey("centerlat"))
      {
        ShowError("When providing center X, a center latitude is not allowed");
      }

      if (launchParams.ContainsKey("centerlon"))
      {
        ShowError("When providing center X, a center longitude is not allowed");
      }

      double x = 0;

      try
      {
        x = Convert.ToDouble(launchParams["centerx"]);
      }
      catch
      {
        ShowError("Invalid center X specified");
      }

      double dx = _appState.Extent.Width / 2;
      _appState.Extent = new Envelope(new Coordinate(x - dx, _appState.Extent.MinY), new Coordinate(x + dx, _appState.Extent.MaxY));
    }

    // === centery ===

    if (launchParams.ContainsKey("centery"))
    {
      if (launchParams.ContainsKey("scaleby"))
      {
        ShowError("When providing center Y, 'scale by' is not allowed");
      }

      if (launchParams.ContainsKey("centerlat"))
      {
        ShowError("When providing center Y, a center latitude is not allowed");
      }

      if (launchParams.ContainsKey("centerlon"))
      {
        ShowError("When providing center Y, a center longitude is not allowed");
      }

      double y = 0;

      try
      {
        y = Convert.ToDouble(launchParams["centery"]);
      }
      catch
      {
        ShowError("Invalid center Y specified");
      }

      double dy = _appState.Extent.Height / 2;
      _appState.Extent = new Envelope(new Coordinate(_appState.Extent.MinX, y - dy), new Coordinate(_appState.Extent.MaxX, y + dy));
    }

    // === centerlat ===

    if (launchParams.ContainsKey("centerlat") || launchParams.ContainsKey("centerlat"))
    {
      if (!launchParams.ContainsKey("centerlon"))
      {
        ShowError("When providing center latitude, a center longitude must also be provided");
      }

      if (!launchParams.ContainsKey("centerlat"))
      {
        ShowError("When providing center longitude, a center latitude must also be provided");
      }

      if (launchParams.ContainsKey("scaleby"))
      {
        ShowError("When providing center latitude/longitude, 'scale by' is not allowed");
      }

      double lat = 0;
      double lon = 0;

      try
      {
        lat = Convert.ToDouble(launchParams["centerlat"]);
      }
      catch
      {
        ShowError("Invalid center latitude specified");
      }

      try
      {
        lon = Convert.ToDouble(launchParams["centerlon"]);
      }
      catch
      {
        ShowError("Invalid center longitude specified");
      }

      Coordinate p = AppContext.AppSettings.MapCoordinateSystem.ToProjected(new Coordinate(lon, lat));

      double dx = _appState.Extent.Width / 2;
      double dy = _appState.Extent.Height / 2;

      _appState.Extent = new Envelope(new Coordinate(p.X - dx, p.Y - dy), new Coordinate(p.X + dx, p.Y + dy));
    }

    // === scaleby ===

    if (launchParams.ContainsKey("scaleby"))
    {
      if (_appState.TargetIds.Count == 0 && _appState.SelectionIds.Count == 0 && _appState.MarkupGroups.Count == 0 && !launchParams.ContainsKey("zone"))
      {
        ShowError("Target IDs, selection IDs, a markup group or a zone must be provided when 'scale by' is specified");
      }

      double scaleBy = Double.NaN;

      try
      {
        scaleBy = Convert.ToDouble(launchParams["scaleby"]);
      }
      catch { }

      if (scaleBy == Double.NaN || scaleBy <= 0)
      {
        ShowError("Invalid 'scale by' value specified, must be greater than zero");
      }

      _appState.Extent.ScaleBy(scaleBy / 1.2);
    }

    // === tool ==

    if (launchParams.ContainsKey("tool"))
    {
      string tool = launchParams["tool"].ToLower();

      if (tool == "pan")
      {
        tool = "identify";
      }

      if (Page.FindControl("opt" + tool, false) == null)
      {
        ShowError("Unknown tool specified");
      }
    }

    if (launchParams.ContainsKey("markcenter"))
    {
      string label = launchParams["markcenter"];

      Coordinate center = _appState.Extent.Centre;
      string point = String.Format("POINT({0} {1})", center.X, center.Y);
      Markup markup = new Markup(point, "#000000", 1);

      if (label != "1")
      {
        markup.Text = label;
      }

      _appState.Markup.Add(markup);
    }

    // === printtemplate and printtitle ===

    string templateID = null;
    string title = "";

    if (launchParams.ContainsKey("printtemplate"))
    {
      templateID = launchParams["printtemplate"];
    }

    if (launchParams.ContainsKey("printtitle"))
    {
      title = launchParams["printtitle"];
    }

    if (!String.IsNullOrEmpty(title) && templateID == null)
    {
      ShowError("When providing a print title, a print template must also be specified");
    }

    if (templateID != null)
    {
      Configuration.PrintTemplateRow printTemplate = _config.PrintTemplate.FindByTemplateID(templateID);

      // the PrintTemplate must exist

      if (printTemplate == null)
      {
        ShowError("Print template '" + templateID + "' does not exist");
      }

      if (!(!printTemplate.IsAlwaysAvailableNull() && printTemplate.AlwaysAvailable == 1))
      {
        string filter = "ApplicationID = '" + application.ApplicationID + "' and TemplateID = '" + templateID + "'";

        // the Application must contain the PrintTemplate

        if (_config.ApplicationPrintTemplate.Select(filter).Length == 0)
        {
          ShowError("Application '" + application.ApplicationID + "' does not contain print template '" + templateID + "'");
        }
      }

      // set the map scale if specified (a 1 inch [96 pixel] square window is assumed)

      if (launchParams.ContainsKey("mapscale"))
      {
        _appState.Extent.ScaleBy(Convert.ToDouble(launchParams["mapscale"]) / _appState.Extent.Width);
      }

      PdfMap pdfMap = new PdfMap(_appState, templateID, new List<String>(new string[] { title }), PreserveMode.Scale, 96);
      pdfMap.Write(Response);
      Response.End();
    }
  }

  private HtmlGenericControl MakeScriptReference(string url)
  {
    HtmlGenericControl scriptReference = new HtmlGenericControl("script");
    scriptReference.Attributes["type"] = "text/javascript";
    scriptReference.Attributes["src"] = url;
    return scriptReference;
  }

  private HtmlLink MakeStyleReference(string url)
  {
    HtmlLink link = new HtmlLink();
    link.Attributes["type"] = "text/css";
    link.Attributes["rel"] = "stylesheet";
    link.Href = url;
    return link;
  }

  private void RestoreState(string state)
  {
    if (state.Length == 12)
    {
      string id = state;
      state = null;

      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = "select State from " + WebConfigSettings.ConfigurationTablePrefix + @"SavedState 
where StateID = ?";

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.VarWChar).Value = id;
          state = command.ExecuteScalar() as string;
        }

        if (state == null)
        {
          ShowError("The map you specified does not exist or is no longer available");
        }

        sql = "update " + WebConfigSettings.ConfigurationTablePrefix + @"SavedState 
set DateLastAccessed = ? where StateID = ?";

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Date).Value = DateTime.Now;
          command.Parameters.Add("@2", OleDbType.VarWChar).Value = id;
          command.ExecuteNonQuery();
        }
      }
    }

    _appState = AppState.FromCompressedString(state);
  }

  private void SetHelpLink()
  {
    List<String> tabNames = new List<String>();

    foreach (string tabName in new string[] { "search", "selection", "legend", "location", "markup", "share" })
    {
      FunctionTab functionTab = (FunctionTab)Enum.Parse(typeof(FunctionTab), tabName, true);

      if ((_appState.FunctionTabs & functionTab) == functionTab)
      {
        tabNames.Add(tabName);
      }
    }

    cmdHelp.HRef = String.Format("Help.aspx?application={0}&functiontabs={1}", _appState.Application, String.Join(",", tabNames.ToArray()));

  }

  private void ShowError(string message)
  {
    Session["StartError"] = message;
    Server.Transfer("StartViewer.aspx");
  }

  private void ShowLevelSelector(Configuration.ApplicationRow application)
  {
    Configuration.ZoneLevelRow zoneLevel = application.ZoneLevelRow;

    if (zoneLevel != null)
    {
      string levelName = !zoneLevel.IsLevelTypeDisplayNameNull() ? zoneLevel.LevelTypeDisplayName : "Level";
      Configuration.LevelRow[] levels = zoneLevel.GetLevelRows();

      if (levels.Length > 0)
      {
        if (String.IsNullOrEmpty(_appState.Level))
        {
          _appState.Level = levels[0].LevelID;
        }

        pnlMapLevels.Style["display"] = "inline-block";
        selectedLevel.Attributes["title"] = levelName;

        foreach (Configuration.LevelRow levelRow in levels)
        {
          string displayName = levelRow.IsDisplayNameNull() ? levelRow.LevelID : levelRow.DisplayName;

          HtmlGenericControl li = new HtmlGenericControl("li");
          phlMapLevel.Controls.Add(li);
          li.InnerHtml = levelName + "&nbsp;" + displayName.Replace(" ", "&nbsp;");
          li.Attributes["data-level"] = levelRow.LevelID;

          if (_appState.Level == levelRow.LevelID)
          {
            selectedLevel.InnerHtml = levelName + "&nbsp;" + displayName.Replace(" ", "&nbsp;");
          }
        }
      }
    }
  }
}