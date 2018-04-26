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
using System.Data.OleDb;
using System.Web;

public class SaveAppStateHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void DefaultMethod()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    string compressedState = appState.ToCompressedString();
    string id = null;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      string sql = String.Format("select count(*) from {0}SavedState where StateID = ?", WebConfigSettings.ConfigurationTablePrefix);

      using (OleDbCommand command = new OleDbCommand(sql, connection))
      {
        command.Parameters.Add("@1", OleDbType.VarWChar);

        string alphabet = "0123456789";

        Random random = new Random();
        char[] chars = new char[12];

        do
        {
          for (int i = 0; i < chars.Length; ++i)
          {
            chars[i] = alphabet[random.Next(alphabet.Length)];
          }

          string tempId = new string(chars);
          command.Parameters[0].Value = tempId;

          if (Convert.ToInt32(command.ExecuteScalar()) == 0)
          {
            id = tempId;
          }
        }
        while (id == null);
      }

      sql = String.Format("insert into {0}SavedState (StateID, DateCreated, DateLastAccessed, State) values (?, ?, ?, ?)", WebConfigSettings.ConfigurationTablePrefix);
      DateTime now = DateTime.Now;

      using (OleDbCommand command = new OleDbCommand(sql, connection))
      {
        command.Parameters.Add("@1", OleDbType.VarWChar).Value = id;
        command.Parameters.Add("@2", OleDbType.Date).Value = now;
        command.Parameters.Add("@3", OleDbType.Date).Value = now;
        command.Parameters.Add("@4", OleDbType.VarWChar).Value = compressedState;
        command.ExecuteNonQuery();
      }
    }

    ReturnJson("id", id);
  }
}