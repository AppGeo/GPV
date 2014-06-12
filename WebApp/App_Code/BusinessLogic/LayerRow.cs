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
using AppGeo.Clients;

public partial class Configuration
{
  public partial class LayerRow
  {
    public string Name
    {
      get
      {
        return IsDisplayNameNull() ? LayerName : DisplayName;
      }
    }

    public string GetLevelQuery(CommonLayer layer, string level)
    {
      string query = null;

      if (!IsLevelFieldNull() && !String.IsNullOrEmpty(level))
      {
        CommonField field = layer.FindField(LevelField);
        query = String.Format("{0} = {1}", field.Name, field.IsNumeric ? level : String.Format("'{0}'", level));
      }

      return query;
    }

    public StringCollection GetTargetIds(string parameterValues)
    {
      StringCollection ids = new StringCollection();

      try
      {
        Configuration.LayerFunctionRow layerFunction = GetLayerFunctionRows().First(o => o.Function == "targetparams");

        using (OleDbCommand command = layerFunction.GetDatabaseCommand())
        {
          string[] p = parameterValues.Split(',');

          for (int i = 0; i < command.Parameters.Count - 1; ++i)
          {
            command.Parameters[i].Value = p[i];
          }

          command.Parameters[command.Parameters.Count - 1].Value = AppUser.GetRole();

          using (OleDbDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              ids.Add(reader.GetValue(0).ToString());
            }
          }
        }
      }
      catch { }

      return ids;
    }

    public LayerIdentifier ToIdentifier()
    {
      return new LayerIdentifier(LayerID, Name);
    }

    public Dictionary<String, Object> ToJsonData()
    {
      string[] proximityIDs = GetLayerProximityRows().Select(o => o.ProximityID).ToArray();

      if (proximityIDs.Length == 0)
      {
        Configuration config = (Configuration)this.Table.DataSet;
        proximityIDs = config.Proximity.Cast<ProximityRow>().Where(o => o.IsIsDefaultNull() || o.IsDefault == 1).Select(o => o.ProximityID).ToArray();
      }

      Dictionary<String, Object> jsonData = new Dictionary<String, Object>();
      jsonData.Add("name", IsDisplayNameNull() ? LayerName : DisplayName);
      jsonData.Add("proximity", proximityIDs);
      jsonData.Add("query", GetQueryRows().Select(o => o.QueryID).ToArray());
      jsonData.Add("dataTab", GetDataTabRows().Select(o => o.DataTabID).ToArray());
      return jsonData;
    }
  }
}

public class LayerIdentifier : IComparable<LayerIdentifier>
{
  public string Key = null;
  public string Name = null;

  public LayerIdentifier(string key, string name)
  {
    Key = key;
    Name = name;
  }

  public int CompareTo(LayerIdentifier other)
  {
    return Key.CompareTo(other.Key);
  }
}