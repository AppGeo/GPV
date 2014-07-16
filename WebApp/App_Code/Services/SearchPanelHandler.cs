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

public class SearchPanelHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void Autocomplete()
  {
    string id = Request.Form["criteria"];
    string text = Request.Form["text"];

    Configuration config = AppContext.GetConfiguration();
    Configuration.SearchCriteriaRow searchCriteriaRow = config.SearchCriteria.First(o => o.SearchCriteriaID == id);

    using (OleDbCommand command = searchCriteriaRow.GetDatabaseCommand())
    {
      command.Parameters[0].Value = text;

      if (command.Parameters.Count > 1)
      {
        command.Parameters[1].Value = AppUser.GetRole();
      }

      List<string> results = new List<string>();

      using (OleDbDataReader reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          results.Add(reader.GetString(0));
        }
      }

      command.Connection.Dispose();

      ReturnJson(results);
    }
  }

  [WebServiceMethod]
  private void Search()
  {
  }
}
