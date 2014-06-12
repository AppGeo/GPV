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
using System.Web;
using GeoAPI.Geometries;

/// <summary>
/// Summary description for AppConfigHandler
/// </summary>
public class AppConfig : WebServiceHandler
{
  /// <summary>
  /// Return a JSON object of the important parts of the Configuration
  /// </summary>
  [WebServiceMethod]
  private void DefaultMethod()
  {
    ReturnJson(new AppConfigData(AppContext.GetConfiguration()));
  }
}

/// <summary>
/// A smaller/serializable version of the Configuration
/// </summary>
public class AppConfigData
{
  public AppConfigData(Configuration config)
  {
    Applications = new List<ApplicationData>(config.Application.Count);

    foreach (Configuration.ApplicationRow app in config.Application.Where(o => IsUserAuthorized(o)))
    {
      Applications.Add(new ApplicationData(config, app));
    }
  }

  private bool IsUserAuthorized(Configuration.ApplicationRow application)
  {
    string roles = application.IsAuthorizedRolesNull() ? "public" : application.AuthorizedRoles;
    return AppUser.RoleIsInList(roles);
  }

  public class ApplicationData
  {
    public ApplicationData(Configuration config, Configuration.ApplicationRow app)
    {
      ApplicationID = app.ApplicationID;
      DisplayName = app.DisplayName;

      DefaultMapTab = (!app.IsDefaultMapTabNull() ? app.DefaultMapTab : "");
      FullExtent = app.GetFullExtentEnvelope().ToArray();

      MapTabs = new List<MapTabData>(config.ApplicationMapTab.Count);
      foreach (Configuration.ApplicationMapTabRow mapTab in config.ApplicationMapTab.Where(e => e.ApplicationID == ApplicationID))
      {
        var configMapTab = config.MapTab.FirstOrDefault(t => t.MapTabID == mapTab.MapTabID);
        if (configMapTab != null)
        {
          MapTabs.Add(new MapTabData(configMapTab));
        }
      }
    }

    public class MapTabData
    {
      public MapTabData(Configuration.MapTabRow mapTab)
      {
        MapTabID = mapTab.MapTabID;
        DisplayName = mapTab.DisplayName;
      }

      public string MapTabID { get; set; }
      public string DisplayName { get; set; }
    }

    public string ApplicationID { get; set; }
    public string DisplayName { get; set; }
    public string DefaultMapTab { get; set; }
    public double[] FullExtent { get; set; }

    public List<MapTabData> MapTabs { get; set; }
  }

  public List<ApplicationData> Applications { get; set; }
}
