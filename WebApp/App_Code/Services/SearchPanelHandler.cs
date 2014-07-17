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
using System.Web.Script.Serialization;

public class SearchPanelHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void Autocomplete()
  {
    string id = Request.QueryString["criteria"];
    string text = Request.QueryString["query"];

    Configuration config = AppContext.GetConfiguration();
    Configuration.SearchCriteriaRow searchCriteriaRow = config.SearchCriteria.First(o => o.SearchCriteriaID == id);

    using (OleDbCommand command = searchCriteriaRow.GetDatabaseCommand())
    {
      command.Parameters[0].Value = text;

      if (command.Parameters.Count > 1)
      {
        command.Parameters[1].Value = AppUser.GetRole();
      }

      List<string> values = new List<string>();

      using (OleDbDataReader reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          values.Add(reader.GetString(0));
        }
      }

      command.Connection.Dispose();

      Dictionary<String, Object> result = new Dictionary<String, Object>();
      result.Add("suggestions", values);

      ReturnJson(result);
    }
  }

  [WebServiceMethod]
  private void Search()
  {
    string searchID = Request.Form["search"];

    JavaScriptSerializer serializer = new JavaScriptSerializer();
    Dictionary<String, Object> criteria = serializer.Deserialize<Dictionary<String, Object>>(Request.Form["criteria"]);
    
    Configuration config = AppContext.GetConfiguration();

    List<String> where = new List<String>();
    List<Object> parameters = new List<Object>();

    foreach (string criteriaID in criteria.Keys)
    {
      Configuration.SearchCriteriaRow searchCriteriaRow = config.SearchCriteria.First(o => o.SearchCriteriaID == criteriaID);

      switch (searchCriteriaRow.SearchCriteriaType)
      {
        case "autocomplete":
        case "lookup":
        case "numeric":
        case "text":
          where.Add(searchCriteriaRow.ColumnName + " = ?");
          parameters.Add(criteria[criteriaID]);
          break;

        case "between":
          object[] values = (object[])criteria[criteriaID];

          if (values[0] != null)
          {
            where.Add(searchCriteriaRow.ColumnName + " >= ?");
            parameters.Add(values[0]);
          }

          if (values[1] != null)
          {
            where.Add(searchCriteriaRow.ColumnName + " <= ?");
            parameters.Add(values[1]);
          }
          break;
      }
    }

    Dictionary<String, Object> result = new Dictionary<String, Object>();

    using (OleDbCommand command = config.Search.First(o => o.SearchID == searchID).GetDatabaseCommand())
    {
      command.CommandText = String.Format(command.CommandText, String.Join(" and ", where.ToArray()));

      for (int i = 0; i < parameters.Count; ++i)
      {
        command.Parameters.AddWithValue(i.ToString(), parameters[i]);
      }

      using (OleDbDataReader reader = command.ExecuteReader())
      {
        // get the indexes of the ID columns

        int mapIdColumn = reader.GetColumnIndex("MapID");

        // write the column headers

        List<String> headers = new List<String>();

        for (int i = 0; i < reader.FieldCount; ++i)
        {
          if (i != mapIdColumn)
          {
            headers.Add(reader.GetName(i));
          }
        }

        result.Add("headers", headers);

        // write the data

        List<Dictionary<String, Object>> rows = new List<Dictionary<String, Object>>();

        while (reader.Read())
        {
          if (!reader.IsDBNull(mapIdColumn))
          {
            Dictionary<String, String> id = new Dictionary<String, String>();

            id.Add("m", reader.GetValue(mapIdColumn).ToString());

            List<Object> values = new List<Object>();

            for (int i = 0; i < reader.FieldCount; ++i)
            {
              if (i != mapIdColumn)
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
    }
  }
}
