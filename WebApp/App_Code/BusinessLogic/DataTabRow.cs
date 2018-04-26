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
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;

public partial class Configuration
{
  public partial class DataTabRow
  {
    private Configuration Configuration
    {
      get
      {
        return (Configuration)Table.DataSet;
      }
    }

    public OleDbCommand GetDatabaseCommand()
    {
      OleDbConnection connection = IsConnectionIDNull() ? AppContext.GetDatabaseConnection() : ConnectionRow.GetDatabaseConnection();

      if (connection != null)
      {
        OleDbCommand command = new OleDbCommand(StoredProc, connection);
        command.CommandType = CommandType.StoredProcedure;

        if (IsParameterCountNull())
        {
          ParameterCount = Configuration.GetParameterCount(command);
        }

        if (ParameterCount >= 0)
        {
          for (int i = 1; i <= ParameterCount; ++i)
          {
            command.Parameters.AddWithValue(i.ToString(), "0");
          }

          return command;
        }
      }

      return null;
    }

    public Dictionary<String, Object> ToJsonData()
    {
      Dictionary<String, Object> jsonData = new Dictionary<String, Object>();
      jsonData.Add("name", DisplayName);
      jsonData.Add("sequenceNo", SequenceNo);
      return jsonData;
    }
  }
}