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
using GeoAPI.Geometries;
using AppGeo.Clients;

public partial class Configuration
{
  public partial class MapTabRow
  {
    public string GetDataFrameKey()
    {
      string userName = IsUserNameNull() ? "" : UserName;
      string password = IsPasswordNull() ? "" : Password;
      string dataFrame = IsDataFrameNull() ? "" : DataFrame;

      return String.Format("{0},{1},{2},{3},{4}", MapHost, userName, password, MapService, dataFrame);
    }

    public string GetHostKey(string type)
    {
      string userName = IsUserNameNull() ? "" : UserName;
      string password = IsPasswordNull() ? "" : Password;

      return String.Format("{0},{1},{2}|{3}", MapHost, userName, password, type);
    }

    public string GetServiceKey(string type)
    {
      string userName = IsUserNameNull() ? "" : UserName;
      string password = IsPasswordNull() ? "" : Password;

      return String.Format("{0},{1},{2},{3}|{4}", MapHost, userName, password, MapService, type);
    }

    public Envelope GetZoneExtent(string zone)
    {
      Envelope extent = new Envelope();
      CommonDataFrame dataFrame = AppContext.GetDataFrame(this);

      foreach (Configuration.MapTabLayerRow mapTabLayer in GetMapTabLayerRows())
      {
        Configuration.LayerRow layer = mapTabLayer.LayerRow;

        if (!layer.IsZoneFieldNull())
        {
          CommonLayer commonLayer = dataFrame.Layers.First(o => o.Name == layer.LayerName);
          CommonField field = commonLayer.FindField(layer.ZoneField);

          string zoneValue = field.IsNumeric ? zone : String.Format("'{0}'", zone);
          extent = commonLayer.GetFeatureExtent(String.Format("{0} = {1}", field.Name, zoneValue));

          if (!extent.IsNull)
          {
            break;
          }
        }
      }

      return extent;
    }

    public Dictionary<String, Object> ToJsonData()
    {
      MapTabLayerRow[] mapTabLayers = GetMapTabLayerRows();
      IEnumerable<MapTabLayerRow> targetMapTabLayers = mapTabLayers.Where(o => !o.IsAllowTargetNull() && o.AllowTarget > 0).OrderBy(o => o, new MapTabLayerComparer(CompareMode.Target));
      IEnumerable<MapTabLayerRow> selectionMapTabLayers = mapTabLayers.Where(o => !o.IsAllowSelectionNull() && o.AllowSelection > 0).OrderBy(o => o, new MapTabLayerComparer(CompareMode.Selection));

      Dictionary<String, Object> jsonData = new Dictionary<String, Object>();
      jsonData.Add("target", targetMapTabLayers.Select(o => o.LayerID).ToArray());
      jsonData.Add("selection", selectionMapTabLayers.Select(o => o.LayerID).ToArray());
      return jsonData;
    }

    private class MapTabLayerComparer : IComparer<MapTabLayerRow>
    {
      private CompareMode _compareMode;

      public MapTabLayerComparer(CompareMode compareMode)
      {
        _compareMode = compareMode;
      }

      public int Compare(MapTabLayerRow x, MapTabLayerRow y)
      {
        int xOrder = _compareMode == CompareMode.Target ? x.AllowTarget : x.AllowSelection;
        int yOrder = _compareMode == CompareMode.Target ? y.AllowTarget : y.AllowSelection;

        int result = xOrder.CompareTo(yOrder);

        if (result == 0)
        {
          string xName = x.LayerRow.IsDisplayNameNull() ? x.LayerRow.LayerName : x.LayerRow.DisplayName;
          string yName = y.LayerRow.IsDisplayNameNull() ? y.LayerRow.LayerName : y.LayerRow.DisplayName;
          result = xName.CompareTo(yName);
        }

        return result;
      }
    }

    private enum CompareMode
    {
      Target = 1,
      Selection = 2
    }
  }
}