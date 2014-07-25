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
using AppGeo.Clients;
using AppGeo.Clients.Ags;
using GeoAPI.Geometries;

public class MapTipHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void DefaultMethod()
  {
    string v = Request.Form["visiblelayers"];
    string[] visibleLayers = v == null ? new string[0] : v.Split('\u0001');

    string level = Request.Form["level"];
    double x = Convert.ToDouble(Request.Form["x"]);
    double y = Convert.ToDouble(Request.Form["y"]);
    double distance = Convert.ToDouble(Request.Form["distance"]);
    double scale = Convert.ToDouble(Request.Form["scale"]);

    Configuration config = AppContext.GetConfiguration();
    Configuration.MapTabRow mapTab = config.MapTab.First(o => o.MapTabID == Request.Form["maptab"]);
    CommonDataFrame dataFrame = AppContext.GetDataFrame(mapTab);

    Dictionary<String, Configuration.LayerRow> layers = new Dictionary<String, Configuration.LayerRow>();
    Dictionary<String, Configuration.LayerFunctionRow> layerFunctions = new Dictionary<String, Configuration.LayerFunctionRow>();

    foreach (Configuration.MapTabLayerRow mapTabLayer in mapTab.GetMapTabLayerRows())
    {
      bool isCandidateLayer = mapTab.IsInteractiveLegendNull() || mapTab.InteractiveLegend == 0;

      if (!isCandidateLayer)
      {
        bool shownInLegend = !mapTabLayer.IsShowInLegendNull() && mapTabLayer.ShowInLegend == 1;
        bool checkedInLegend = mapTabLayer.IsCheckInLegendNull() || mapTabLayer.CheckInLegend < 0 || visibleLayers.Any(o => o == mapTabLayer.LayerID);
        isCandidateLayer = !shownInLegend || checkedInLegend;
      }

      if (isCandidateLayer)
      {
        Configuration.LayerRow layer = mapTabLayer.LayerRow;
        Configuration.LayerFunctionRow layerFunction = layer.GetLayerFunctionRows().FirstOrDefault(o => o.FunctionName.ToLower() == "maptip");

        if (layerFunction != null)
        {
          layers.Add(layer.LayerName, layer);
          layerFunctions.Add(layer.LayerName, layerFunction);
        }
      }
    }

    string tipText = null;

    for (int i = 0; i < dataFrame.Layers.Count - 1 && tipText == null; ++i)
    {
      CommonLayer commonLayer = dataFrame.Layers[i];
      string id = null;

      if (layers.ContainsKey(commonLayer.Name) && commonLayer.IsWithinScaleThresholds(scale))
      {
        if (commonLayer.Type == CommonLayerType.Feature)
        {
          Configuration.LayerRow layer = layers[commonLayer.Name];
          string levelQuery = layer.GetLevelQuery(commonLayer, level);

          CommonField keyField = commonLayer.FindField(layer.KeyField);
          DataTable table = commonLayer.GetFeatureTable(keyField.Name, levelQuery, x, y, commonLayer.FeatureType == OgcGeometryType.MultiPolygon ? 0 : distance * scale);

          if (table != null && table.Rows.Count > 0)
          {
            id = table.Rows[table.Rows.Count - 1][0].ToString();
          }
        }

        if (commonLayer.Type == CommonLayerType.Image)
        {
          id = ((AgsLayer)commonLayer).GetRasterValue(x, y);
        }
      }

      if (!String.IsNullOrEmpty(id))
      {
        Configuration.LayerFunctionRow layerFunction = layerFunctions[commonLayer.Name];

        using (OleDbCommand command = layerFunction.GetDatabaseCommand())
        {
          command.Parameters[0].Value = id;

          if (command.Parameters.Count > 1)
          {
            command.Parameters[1].Value = AppUser.GetRole();
          }

          using (OleDbDataReader reader = command.ExecuteReader())
          {
            if (reader.Read())
            {
              StringCollection text = new StringCollection();

              for (int j = 0; j < reader.FieldCount; ++j)
              {
                if (!reader.IsDBNull(j))
                {
                  text.Add(reader.GetValue(j).ToString());
                }
              }

              if (text.Count > 0)
              {
                tipText = text.Join("\n");
              }
            }
          }

          command.Connection.Close();
        }
      }
    }

    if (tipText == null)
    {
      ReturnJson(null);
    }
    else
    {
      Dictionary<String, Object> result = new Dictionary<String, Object>();
      result.Add("tipText", tipText);
      ReturnJson(result);
    }
  }
}