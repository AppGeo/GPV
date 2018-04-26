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
using NetTopologySuite.Geometries;
using AppGeo.Clients;

public class SelectionHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void GetActiveExtent()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    Envelope extent = appState.SelectionManager.GetExtent(FeatureType.Active);
    
    ReturnJson<double[]>("extent", extent.IsNull ? null : extent.ToArray());
  }

  [WebServiceMethod]
  private void GetSelectionExtent()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    Envelope extent = new Envelope();

    if (appState.TargetIds.Count > 0)
    {
      extent.ExpandToInclude(appState.SelectionManager.GetExtent(FeatureType.Target));
    }

    if (appState.SelectionIds.Count > 0)
    {
      extent.ExpandToInclude(appState.SelectionManager.GetExtent(FeatureType.Selection));
    }

    ReturnJson<double[]>("extent", extent.IsNull ? null : extent.ToArray());
  }

  [WebServiceMethod]
  private void SelectFeatures()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    bool updated = false;
    bool truncated = false;

    if (Request.Form["geo"] == null)
    {
      appState.SelectionManager.SelectTargets();
      updated = true;
    }
    else
    {
      double[] geo = Request.Form["geo"].Split(',').Select(o => Convert.ToDouble(o)).ToArray();

      CommonDataFrame dataFrame = AppContext.GetDataFrame(appState.MapTab);
      Configuration.LayerRow layer = Configuration.Layer.First(o => o.LayerID == (appState.Action == Action.Select ? appState.TargetLayer : appState.SelectionLayer));
      CommonLayer commonLayer = dataFrame.Layers.FirstOrDefault(o => String.Compare(o.Name, layer.LayerName, true) == 0);
      CommonField keyField = commonLayer.FindField(layer.KeyField);

      string levelQuery = layer.GetLevelQuery(commonLayer, appState.Level);

      DataTable table = null;

      if (geo.Length == 4)
      {
        Envelope box = EnvelopeExtensions.FromArray(geo);

        if (!layer.IsMaxSelectionAreaNull() && layer.MaxSelectionArea > 0 && box.Width * box.Height > layer.MaxSelectionArea)
        {
          throw new AppException("The selection shape was too large; try again with a smaller shape");
        }

        table = commonLayer.GetFeatureTable(keyField.Name, levelQuery, box.ToPolygon());
      }
      else
      {
        table = commonLayer.GetFeatureTable(keyField.Name, levelQuery, geo[0], geo[1], geo[2]);
      }
      
      UpdateMode mode = (UpdateMode)Enum.Parse(typeof(UpdateMode), Request.Form["mode"], true);

      if (table != null && table.Rows.Count > 0)
      {

        if (appState.Action == Action.Select)
        {
          updated = UpdateIds(appState.TargetIds, table, mode);

          if (!layer.IsMaxNumberSelectedNull())
          {
            truncated = appState.TargetIds.Truncate(layer.MaxNumberSelected);
          }

          if (mode != UpdateMode.Remove && table.Rows.Count == 1)
          {
            updated = UpdateActive(appState, table.Rows[0][0].ToString()) || updated;
          }
        }
        else
        {
          updated = UpdateIds(appState.SelectionIds, table, mode);

          if (!layer.IsMaxNumberSelectedNull())
          {
            appState.SelectionIds.Truncate(layer.MaxNumberSelected);
          }

          if (updated)
          {
            truncated = appState.SelectionManager.SelectTargets();
          }
        }
      }
      else if (mode == UpdateMode.New)
      {
        updated = appState.Action == Action.Select ? appState.TargetIds.Count > 0 : appState.SelectionIds.Count > 0;
        appState.TargetIds.Clear();
        appState.SelectionIds.Clear();
        appState.ActiveMapId = "";
        appState.ActiveDataId = "";
      }
    }

    if (!updated)
    {
      ReturnJson(null);
    }

    if (!appState.TargetIds.Contains(appState.ActiveMapId))
    {
      appState.ActiveMapId = "";
      appState.ActiveDataId = "";
    }

    Dictionary<String, Object> state = new Dictionary<String, Object>();
    state.Add("ActiveMapId", appState.ActiveMapId);
    state.Add("ActiveDataId", appState.ActiveDataId);
    state.Add("TargetIds", appState.TargetIds);
    state.Add("SelectionIds", appState.SelectionIds);

    Dictionary<String, Object> result = new Dictionary<String, Object>();
    result.Add("state", state);
    result.Add("truncated", truncated);
    ReturnJson(result);
  }

  private bool UpdateActive(AppState appState, string mapId)
  {
    string dataId = "";
    Configuration.QueryRow query = Configuration.Query.First(o => o.QueryID == appState.Query);

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
          else
          {
            mapId = "";
            dataId = "";
          }
        }
      }
      catch
      {
        mapId = "";
        dataId = "";
      }
       
      command.Connection.Close();
    }

    bool updated = mapId != appState.ActiveMapId || dataId != appState.ActiveDataId;

    if (updated)
    {
      appState.ActiveMapId = mapId;
      appState.ActiveDataId = dataId;
    }

    return updated;
  }

  private bool UpdateIds(StringCollection ids, DataTable table, UpdateMode mode)
  {
    bool updated = false;

    if (mode == UpdateMode.New && ids.Count > 0)
    {
      ids.Clear();
      updated = true;
    }

    foreach (DataRow row in table.Rows)
    {
      if (!row.IsNull(0))
      {
        string value = row[0].ToString();

        if (mode == UpdateMode.Remove)
        {
          if (ids.IndexOf(value) >= 0)
          {
            ids.Remove(value);
            updated = true;
          }
        }
        else
        {
          if (mode == UpdateMode.Add)
          {
            if (ids.IndexOf(value) < 0)
            {
              ids.Add(value);
              updated = true;
            }
          }
          else
          {
            ids.Add(value);
            updated = true;
          }
        }
      }
    }

    return updated;
  }

  private enum UpdateMode
  {
    New,
    Add,
    Remove
  }
}