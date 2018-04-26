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
using System.Data.OleDb;
using System.Linq;

public class SelectionPanelHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void GetDataListHtml()
  {
    string dataTabID = Request.Form["datatab"];
    string id = Request.Form["id"];

    Configuration config = AppContext.GetConfiguration();
    Configuration.DataTabRow dataTab = config.DataTab.First(o => o.DataTabID == dataTabID);
    DataListBuilder dataListBuilder = new DataListBuilder();

    using (OleDbCommand command = dataTab.GetDatabaseCommand())
    {
      command.Parameters[0].Value = id;

      if (command.Parameters.Count > 1)
      {
        command.Parameters[1].Value = AppUser.GetRole();
      }

      using (OleDbDataReader reader = command.ExecuteReader())
      {
        dataListBuilder.AddFromReader(reader);
      }

      command.Connection.Dispose();
    }

    Response.ContentType = "text/html";
    dataListBuilder.RenderToStream(Response.OutputStream);
  }

  [WebServiceMethod]
  private void GetLayerProperties()
  {
    Configuration.LayerRow layer = Configuration.Layer.First(o => o.LayerID == Request.Form["layer"]);

    Dictionary<String, Object> result = new Dictionary<String, Object>();
    result.Add("supportsMailingLabels", layer.GetLayerFunctionRows().Any(o => o.FunctionName.ToLower() == "mailinglabel"));
    result.Add("supportsExportData", layer.GetLayerFunctionRows().Any(o => o.FunctionName.ToLower() == "export"));
    ReturnJson(result);
  }

  [WebServiceMethod]
  private void GetQueryGridData()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    Configuration.ApplicationRow application = Configuration.Application.First(o => o.ApplicationID == appState.Application);
    Configuration.QueryRow query = Configuration.Query.First(o => o.QueryID == appState.Query);

    List<String> zones = new List<String>();
    List<String> levels = new List<String>();

    if (!application.IsZoneLevelIDNull())
    {
      zones = application.ZoneLevelRow.GetZoneRows().Select(o => o.ZoneID).ToList();
      levels = application.ZoneLevelRow.GetLevelRows().Select(o => o.LevelID).ToList();
    }

    Dictionary<String, Object> result = new Dictionary<String, Object>();

    using (OleDbCommand command = query.GetDatabaseCommand())
    {
      command.Parameters[0].Value = appState.TargetIds.Join(",");

      if (command.Parameters.Count > 1)
      {
        command.Parameters[1].Value = AppUser.GetRole();
      }

      using (OleDbDataReader reader = command.ExecuteReader())
      {
        // get the indexes of the ID columns

        int mapIdColumn = reader.GetColumnIndex("MapID");
        int dataIdColumn = reader.GetColumnIndex("DataID");
        int zoneIdColumn = zones.Count > 0 ? reader.GetColumnIndex("ZoneID") : -1;
        int levelIdColumn = levels.Count > 0 ? reader.GetColumnIndex("LevelID") : -1;

        // write the column headers

        List<String> headers = new List<String>();

        for (int i = 0; i < reader.FieldCount; ++i)
        {
          if (i != mapIdColumn && i != dataIdColumn && i != zoneIdColumn && i != levelIdColumn)
          {
            headers.Add(reader.GetName(i));
          }
        }

        result.Add("headers", headers);

        // write the data

        List<Dictionary<String, Object>> rows = new List<Dictionary<String, Object>>();

        while (reader.Read())
        {
          string m = !reader.IsDBNull(mapIdColumn) ? reader.GetValue(mapIdColumn).ToString() : null;
          string d = !reader.IsDBNull(dataIdColumn) ? reader.GetValue(dataIdColumn).ToString() : null;

          if (!String.IsNullOrEmpty(m) && !String.IsNullOrEmpty(d))
          {
            Dictionary<String, String> id = new Dictionary<String, String>();

            id.Add("m", m);

            if (dataIdColumn > -1 && !reader.IsDBNull(dataIdColumn))
            {
              id.Add("d", d);
            }

            if (zoneIdColumn > -1 && !reader.IsDBNull(zoneIdColumn))
            {
              string zoneId = reader.GetValue(zoneIdColumn).ToString();

              if (!String.IsNullOrEmpty(zoneId) && zones.Contains(zoneId))
              {
                id.Add("z", zoneId);
              }
            }

            if (levelIdColumn > -1 && !reader.IsDBNull(levelIdColumn))
            {
              string levelId = reader.GetValue(levelIdColumn).ToString();

              if (!String.IsNullOrEmpty(levelId) && levels.Contains(levelId))
              {
                id.Add("l", levelId);
              }
            }

            List<Object> values = new List<Object>();

            for (int i = 0; i < reader.FieldCount; ++i)
            {
              if (i != mapIdColumn && i != dataIdColumn && i != zoneIdColumn && i != levelIdColumn)
              {
                values.Add(reader.IsDBNull(i) ? null : reader.GetValue(i));
              }
            }

            Dictionary<String, Object> row = new Dictionary<String, Object>();
            row.Add("id", id);
            row.Add("v", values);

            rows.Add(row);
          }
        }

        result.Add("rows", rows);
      }

      command.Connection.Dispose();
    }


    ReturnJson(result);
  }

  [WebServiceMethod]
  private void GetTargetIds()
  {
    Configuration.LayerRow layer = Configuration.Layer.First(o => o.LayerID == Request.Form["layer"]);
    StringCollection data = layer.GetTargetIds(Request.Form["params"]);
    ReturnJson<String[]>("ids", data.Cast<String>().ToArray());
  }
}