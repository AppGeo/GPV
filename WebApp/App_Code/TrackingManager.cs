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
using System.Web;

public static class TrackingManager
{
  public static void TrackUse(HttpRequest request, bool isMobile)
  {
    Dictionary<String, String> launchParams = request.GetNormalizedParameters();

    if (launchParams.Keys.Contains("application"))
    {
      string applicationID = launchParams["application"];
      Configuration.ApplicationRow application = AppContext.GetConfiguration().Application.FindByApplicationID(applicationID);
      applicationID = application.ApplicationID;

      if (!application.IsTrackUseNull() && application.TrackUse == 1)
      {
        string urlQuery = String.Join("&", request.GetNormalizedParameters().Select(o => o.Key + "=" + o.Value).ToArray());

        using (OleDbConnection connection = AppContext.GetDatabaseConnection())
        {
          string sql = String.Format("insert into {0}UsageTracking (ApplicationID, UrlQuery, DateStarted, UserAgent, UserHostAddress, UserHostName) values (?, ?, ?, ?, ?, ?)", AppSettings.ConfigurationTablePrefix);

          using (OleDbCommand command = new OleDbCommand(sql, connection))
          {
            command.Parameters.Add("@1", OleDbType.VarWChar).Value = applicationID + (isMobile && applicationID.Length < 46 ? " [m]" : "");
            command.Parameters.Add("@2", OleDbType.VarWChar).Value = urlQuery.Length < 1000 ? urlQuery : urlQuery.Substring(0, 1000);
            command.Parameters.Add("@3", OleDbType.Date).Value = DateTime.Now;
            command.Parameters.Add("@4", OleDbType.VarWChar).Value = request.UserAgent.Length < 400 ? request.UserAgent : request.UserAgent.Substring(0, 400);
            command.Parameters.Add("@5", OleDbType.VarWChar).Value = request.UserHostAddress;
            command.Parameters.Add("@6", OleDbType.VarWChar).Value = request.UserHostName;
            command.ExecuteNonQuery();
          }
        }
      }
    }
  }
}