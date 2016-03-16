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
using System.Configuration;

public static class WebConfigSettings
{
  public static bool AllowDevScriptCaching
  {
    get
    {
      return GetWebConfigBoolean("AllowDevScriptCaching");
    }
  }

  public static bool AppIsAvailable
  {
    get
    {
      return GetWebConfigBoolean("AppIsAvailable");
    }
  }

  public static String AppStatusMessage
  {
    get
    {
      return GetWebConfigSetting("AppStatusMessage");
    }
  }

  public static string ConfigurationTablePrefix
  {
    get
    {
      string prefix = GetWebConfigSetting("ConfigTablePrefix");

      if (String.IsNullOrEmpty(prefix))
      {
        prefix = "GPV";
      }

      return prefix;
    }
  }

  private static bool GetWebConfigBoolean(string name)
  {
    string value = GetWebConfigSetting(name);
    return value != null && (String.Compare(value, "true", false) == 0 || String.Compare(value, "yes", false) == 0);
  }

  private static string GetWebConfigSetting(string name)
  {
    return ConfigurationManager.AppSettings[name];
  }
}