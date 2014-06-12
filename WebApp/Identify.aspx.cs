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
using System.Data;
using System.Data.OleDb;
using System.Linq;
using GeoAPI.Geometries;
using AppGeo.Clients;
using AppGeo.Clients.Ags;

public partial class Identify : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    pnlPostBack.Visible = false;
    autoPrint.Visible = false;

    string mapTabID = Request.QueryString["maptab"];
    double x = ParseDouble(Request.QueryString["x"]);
    double y = ParseDouble(Request.QueryString["y"]);
    double distance = ParseDouble(Request.QueryString["distance"]);

    double lat = ParseDouble(Request.QueryString["lat"]);
    double lon = ParseDouble(Request.QueryString["lon"]);

    bool addSpace = String.IsNullOrEmpty(Request.QueryString["space"]) || Request.QueryString["space"] == "1";

    if (!Double.IsNaN(lat) && !Double.IsNaN(lon))
    {
      AppSettings.CoordinateSystem.ToProjected(lon, lat, out x, out y);

      if (AppSettings.MapUnits == "feet")
      {
        x *= Constants.FeetPerMeter;
        y *= Constants.FeetPerMeter;
      }

      x += AppSettings.DatumShiftX;
      y += AppSettings.DatumShiftY;
    }

    string dataTabID = Request.QueryString["datatab"];
    string id = Request.QueryString["id"];

    if (String.IsNullOrEmpty(Request.Form["postback"]) && String.IsNullOrEmpty(Request.QueryString["noloading"]))
    {
      pnlIdentify.Visible = false;

      if (!String.IsNullOrEmpty(mapTabID))
      {
        if (!Double.IsNaN(x) && !Double.IsNaN(y) && !Double.IsNaN(distance))
        {
          labMessage.InnerText = "Searching...";
          pnlPostBack.Visible = true;
        }
      }
      else if (!String.IsNullOrEmpty(dataTabID) && !String.IsNullOrEmpty(id))
      {
        labMessage.InnerText = "Loading...";
        pnlPostBack.Visible = true;
      }
    }
    else
    {
      labMessage.Visible = false;

      if (!String.IsNullOrEmpty(mapTabID))
      {
        SearchMapTab(mapTabID, x, y, distance, addSpace);
      }
      else if (!String.IsNullOrEmpty(dataTabID))
      {
        ShowTabData(dataTabID, id);
      }
    }
  }

  private double ParseDouble(string s)
  {
    return ParseDouble(s, Double.NaN);
  }

  private double ParseDouble(string s, double defaultValue)
  {
    double d;
    return Double.TryParse(s, out d) ? d : defaultValue;
  }

  private string[] ParseStringArray(string s, char c)
  {
    return !String.IsNullOrEmpty(s) ? s.Split(c) : new string[] { };
  }

  private void SearchMapTab(string mapTabID, double x, double y, double distance, bool addSpace)
  {
    double scale = ParseDouble(Request.QueryString["scale"], 1);
    string[] visibleLayers = ParseStringArray(Request.QueryString["visiblelayers"], '\u0001');
    string levelID = Request.QueryString["level"];

    DataListBuilder dataListBuilder = new DataListBuilder();

    Configuration config = AppContext.GetConfiguration();
    Configuration.MapTabRow mapTab = config.MapTab.First(o => o.MapTabID == mapTabID);
    CommonDataFrame dataFrame = AppContext.GetDataFrame(mapTab);

    Dictionary<String, Configuration.LayerRow> layers = new Dictionary<String, Configuration.LayerRow>();
    Dictionary<String, Configuration.LayerFunctionRow> layerFunctions = new Dictionary<String, Configuration.LayerFunctionRow>();

    bool useDefaultVisible = visibleLayers.Length == 1 && visibleLayers[0] == "*";

    foreach (Configuration.MapTabLayerRow mapTabLayer in mapTab.GetMapTabLayerRows())
    {
      bool isCandidateLayer = mapTab.IsInteractiveLegendNull() || mapTab.InteractiveLegend == 0;

      if (!isCandidateLayer)
      {
        bool shownInLegend = !mapTabLayer.IsShowInLegendNull() && mapTabLayer.ShowInLegend == 1;
        bool checkedInLegend = mapTabLayer.IsCheckInLegendNull() || mapTabLayer.CheckInLegend < 0 || visibleLayers.Any(o => o == mapTabLayer.LayerID);
        bool defaultVisible = useDefaultVisible && !mapTabLayer.IsCheckInLegendNull() && mapTabLayer.CheckInLegend == 1;
        isCandidateLayer = !shownInLegend || checkedInLegend || defaultVisible;
      }

      if (isCandidateLayer)
      {
        Configuration.LayerRow layer = mapTabLayer.LayerRow;
        Configuration.LayerFunctionRow layerFunction = layer.GetLayerFunctionRows().FirstOrDefault(o => o.Function.ToLower() == "identify");

        if (layerFunction != null)
        {
          layers.Add(layer.LayerName, layer);
          layerFunctions.Add(layer.LayerName, layerFunction);
        }
      }
    }

    foreach (CommonLayer commonLayer in dataFrame.Layers)
    {
      DataTable table = null;

      if (layers.ContainsKey(commonLayer.Name) && commonLayer.IsWithinScaleThresholds(scale))
      {
        Configuration.LayerRow layer = layers[commonLayer.Name];

        if (commonLayer.Type == CommonLayerType.Feature)
        {
          CommonField keyField = commonLayer.FindField(layer.KeyField);
          string levelQuery = layer.GetLevelQuery(commonLayer, levelID);
          table = commonLayer.GetFeatureTable(keyField.Name, levelQuery, x, y, commonLayer.FeatureType == OgcGeometryType.MultiPolygon ? 0 : distance * scale);
        }

        if (commonLayer.Type == CommonLayerType.Image && commonLayer is AgsLayer)
        {
          string id = ((AgsLayer)commonLayer).GetRasterValue(x, y);
          table = new DataTable();
          table.Columns.Add("ID");
          table.Rows.Add(id);
        }
      }

      if (table != null && table.Rows.Count > 0)
      {
        Configuration.LayerFunctionRow layerFunction = layerFunctions[commonLayer.Name];

        foreach (DataRow row in table.Rows)
        {
          string id = row[0].ToString();

          using (OleDbCommand command = layerFunction.GetDatabaseCommand())
          {
            command.Parameters[0].Value = id;

            if (command.Parameters.Count > 1)
            {
              command.Parameters[1].Value = AppUser.GetRole();
            }

            using (OleDbDataReader reader = command.ExecuteReader())
            {
              dataListBuilder.AddFromReader(reader, addSpace);
            }

            command.Connection.Dispose();
          }
        }
      }
    }

    pnlContent.Controls.Add(dataListBuilder.BuiltControl);
  }

  private void ShowTabData(string dataTabID, string id)
  {
    bool print = !String.IsNullOrEmpty(Request.QueryString["print"]);

    Configuration config = AppContext.GetConfiguration();
    Configuration.DataTabRow dataTab = config.DataTab.First(o => o.DataTabID == dataTabID);

    using (OleDbCommand command = dataTab.GetDatabaseCommand())
    {
      command.Parameters[0].Value = id;

      if (command.Parameters.Count > 1)
      {
        command.Parameters[1].Value = AppUser.GetRole();
      }

      using (OleDbDataReader reader = command.ExecuteReader())
      {
        DataListBuilder dataListBuilder = new DataListBuilder();
        dataListBuilder.AddFromReader(reader);
        pnlContent.Controls.Add(dataListBuilder.BuiltControl);
      }

      command.Connection.Dispose();
    }

    cmdPrint.Visible = !print;
    autoPrint.Visible = print;

    if (print)
    {
      Title = "Print";
    }
  }
}