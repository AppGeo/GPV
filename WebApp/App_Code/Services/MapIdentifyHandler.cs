//  Copyright 2015 Applied Geographics, Inc.
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
using AppGeo.Clients;
using AppGeo.Clients.Ags;
using GeoAPI.Geometries;

public class MapIdentifyHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void DefaultMethod()
  {
    string mapTabID = Request.Form["maptab"];
    string[] visibleLayers = Request.Form["visiblelayers"].Split('\u0001');
    string levelID = Request.Form["level"];
    double x = Convert.ToDouble(Request.Form["x"]);
    double y = Convert.ToDouble(Request.Form["y"]);
    double distance = Convert.ToDouble(Request.Form["distance"]);
    double scale = Convert.ToDouble(Request.Form["scale"]);

    DataListBuilder dataListBuilder = SearchMapTab(mapTabID, visibleLayers, levelID, x, y, distance, scale, false);

    Response.ContentType = "text/html";
    dataListBuilder.RenderToStream(Response.OutputStream);
  }

  public static DataListBuilder SearchMapTab(string mapTabID, string[] visibleLayers, string levelID, double x, double y, 
    double distance, double scale, bool addSpace)
  {
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
        Configuration.LayerFunctionRow layerFunction = layer.GetLayerFunctionRows().FirstOrDefault(o => o.FunctionName.ToLower() == "identify");

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

    return dataListBuilder;
  }
}