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
using System.Configuration;
using System.Drawing;
using System.Reflection;
using GeoAPI.Geometries;
using System.Collections.Generic;
using System.Web.Script.Serialization;

public static class AppSettings
{
  public static Color ActiveColor
  {
    get
    {
      return GetConfigColor("ActiveColor");
    }
  }

  public static Color ActiveColorUI
  {
    get
    {
      return BlendColors(Color.White, ActiveColor, ActiveOpacity);
    }
  }

  public static double ActiveOpacity
  {
    get
    {
      return GetConfigDouble("ActiveOpacity");
    }
  }

  public static int ActiveDotSize
  {
    get
    {
      return GetConfigInteger("ActiveDotSize");
    }
  }

  public static int ActivePenWidth
  {
    get
    {
      return GetConfigInteger("ActivePenWidth");
    }
  }

  public static string ActivePolygonMode
  {
    get
    {
      string activePolygonMode = GetConfigSetting("ActivePolygonMode");

      if (activePolygonMode != null)
      {
        activePolygonMode = activePolygonMode.ToLower();
      }

      return activePolygonMode;
    }
  }

  public static string AdminEmail
  {
    get
    {
      return GetConfigSetting("AdminEmail");
    }
  }

  public static bool AdminOnlyShowApps
  {
    get
    {
      return GetConfigBoolean("AdminOnlyShowApps");
    }
  }

  public static bool AppIsAvailable
  {
    get
    {
      return GetConfigBoolean("AppIsAvailable");
    }
  }

  public static String AppStatusMessage
  {
    get
    {
      return GetConfigSetting("AppStatusMessage");
    }
  }

  public static int BrowserImageCacheTimeout
  {
    get
    {
      return GetConfigInteger("BrowserImageCacheTimeout");
    }
  }
  
  public static Color BufferColor
  {
    get
    {
      return GetConfigColor("BufferColor");
    }
  }

  public static double BufferOpacity
  {
    get
    {
      return GetConfigDouble("BufferOpacity");
    }
  }

  public static Color BufferOutlineColor
  {
    get
    {
      return GetConfigColor("BufferOutlineColor");
    }
  }

  public static double BufferOutlineOpacity
  {
    get
    {
      return GetConfigDouble("BufferOutlineOpacity");
    }
  }

  public static int BufferOutlinePenWidth
  {
    get
    {
      return GetConfigInteger("BufferOutlinePenWidth");
    }
  }

  public static string ConfigurationTablePrefix
  {
    get
    {
      string prefix = GetConfigSetting("ConfigTablePrefix");

      if (prefix == null)
      {
        prefix = "GPV";
      }

      return prefix;
    }
  }

  public static System.Drawing.Font CoordinatesFont
  {
    get
    {
      return new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
    }
  }

  public static CoordinateSystem CoordinateSystem
  {
    get
    {
      CoordinateSystem coordSys = null;

      try
      {
        string projectionName = GetConfigSetting("Projection");
        string spheroidName = GetConfigSetting("Spheroid");
        double centralMeridian = GetConfigDouble("CentralMeridian");
        double originLatitude = GetConfigDouble("OriginLatitude");

        Spheroid spheroid = (Spheroid)typeof(Spheroid).InvokeMember(spheroidName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, null);
        Projection projection = null;

        switch (projectionName)
        {
          case "LambertConformalConic":
            double standardParallel1 = GetConfigDouble("StandardParallel1");
            double standardParallel2 = GetConfigDouble("StandardParallel2");
            projection = new LambertConformalConic(centralMeridian, originLatitude, standardParallel1, standardParallel2, spheroid);
            break;

          case "TransverseMercator":
            double scaleFactor = GetConfigDouble("ScaleFactor");
            projection = new TransverseMercator(centralMeridian, originLatitude, scaleFactor, spheroid);
            break;
        }

        double falseEasting = GetConfigDouble("FalseEasting");
        double falseNorthing = GetConfigDouble("FalseNorthing");

        coordSys = new CoordinateSystem(projection, falseEasting, falseNorthing);
      }
      catch { }

      return coordSys;
    }
  }

  public static string CustomStyleSheet
  {
    get
    {
      return GetConfigSetting("CustomStyleSheet");
    }
  }

  public static string DataTabLinkWindowName
  {
    get
    {
      return GetConfigSetting("DataTabLinkWindowName");
    }
  }

  public static double DatumShiftX
  {
    get
    {
      return GetConfigDouble("DatumShiftX");
    }
  }

  public static double DatumShiftY
  {
    get
    {
      return GetConfigDouble("DatumShiftY");
    }
  }

  public static Envelope DefaultFullExtent
  {
    get
    {
      if (GetConfigSetting("FullExtent") != null)
      {
        try
        {
          string[] ext = GetConfigSetting("FullExtent").Split(',');
          return new Envelope(new Coordinate(Convert.ToDouble(ext[0]), Convert.ToDouble(ext[1])), new Coordinate(Convert.ToDouble(ext[2]), Convert.ToDouble(ext[3])));
        }
        catch { }
      }

      return null;
    }
  }

  public static string ExportFormat
  {
    get
    {
      return GetConfigSetting("ExportFormat");
    }
  }

  public static Color FilteredColor
  {
    get
    {
      return GetConfigColor("FilteredColor");
    }
  }

  public static double FilteredOpacity
  {
    get
    {
      return GetConfigDouble("FilteredOpacity");
    }
  }

  public static int FilteredDotSize
  {
    get
    {
      return GetConfigInteger("FilteredDotSize");
    }
  }

  public static int FilteredPenWidth
  {
    get
    {
      return GetConfigInteger("FilteredPenWidth");
    }
  }

  public static string FilteredPolygonMode
  {
    get
    {
      string polygonMode = GetConfigSetting("FilteredPolygonMode");

      if (polygonMode != null)
      {
        polygonMode = polygonMode.ToLower();
      }

      return polygonMode;
    }
  }

  public static string IdentifyPopup
  {
    get
    {
      return GetConfigSetting("IdentifyPopup");
    }
  }

  public static int IdentifyWindowHeight
  {
    get
    {
      return GetConfigInteger("IdentifyWindowHeight");
    }
  }

  public static int IdentifyWindowWidth
  {
    get
    {
      return GetConfigInteger("IdentifyWindowWidth");
    }
  }

  public static bool LegendExpanded
  {
    get
    {
      return GetConfigBoolean("LegendExpanded");
    }
  }

  public static string MapUnits
  {
    get
    {
      string mapUnits = GetConfigSetting("MapUnits");

      if (mapUnits != null)
      {
        mapUnits = mapUnits.ToLower();
      }

      return mapUnits;
    }
  }

  public static System.Drawing.Font MarkupFont
  {
    get
    {
      return new System.Drawing.Font("Verdana", 10, System.Drawing.FontStyle.Bold);
    }
  }

  public static int MarkupTimeout
  {
    get
    {
      return GetConfigInteger("MarkupTimeout");
    }
  }

  public static System.Drawing.Font MeasureFont
  {
    get
    {
      return new System.Drawing.Font("Verdana", 11, GraphicsUnit.Pixel);
    }
  }

  public static string MeasureUnits
  {
    get
    {
      string measureUnits = GetConfigSetting("MeasureUnits");

      if (measureUnits != null)
      {
        measureUnits = measureUnits.ToLower();
      }

      return measureUnits;
    }
  }

  public static string PreserveOnActionChange
  {
    get
    {
      string preserveOnActionChange = GetConfigSetting("PreserveOnActionChange");

      if (String.IsNullOrEmpty(preserveOnActionChange))
      {
        preserveOnActionChange = "selection";
      }

      preserveOnActionChange = preserveOnActionChange.ToLower();

      return preserveOnActionChange;
    }
  }

  public static Color SelectionColor
  {
    get
    {
      return GetConfigColor("SelectionColor");
    }
  }

  public static Color SelectionColorUI
  {
    get
    {
      return BlendColors(Color.White, SelectionColor, SelectionOpacity);
    }
  }

  public static double SelectionOpacity
  {
    get
    {
      return GetConfigDouble("SelectionOpacity");
    }
  }

  public static int SelectionDotSize
  {
    get
    {
      return GetConfigInteger("SelectionDotSize");
    }
  }

  public static int SelectionPenWidth
  {
    get
    {
      return GetConfigInteger("SelectionPenWidth");
    }
  }

  public static string SelectionPolygonMode
  {
    get
    {
      string polygonMode = GetConfigSetting("SelectionPolygonMode");

      if (polygonMode != null)
      {
        polygonMode = polygonMode.ToLower();
      }

      return polygonMode;
    }
  }

  public static int ServerImageCacheTimeout
  {
    get
    {
      return GetConfigInteger("ServerImageCacheTimeout");
    }
  }

  public static bool ShowScaleBar
  {
    get
    {
      return GetConfigBoolean("ShowScaleBar");
    }
  }

  public static int SwatchTileHeight
  {
    get
    {
      return GetConfigInteger("SwatchTileHeight");
    }
  }

  public static int SwatchTileWidth
  {
    get
    {
      return GetConfigInteger("SwatchTileWidth");
    }
  }

  public static Color TargetColor
  {
    get
    {
      return GetConfigColor("TargetColor");
    }
  }

  public static Color TargetColorUI
  {
    get
    {
      return BlendColors(Color.White, TargetColor, TargetOpacity);
    }
  }

  public static double TargetOpacity
  {
    get
    {
      return GetConfigDouble("TargetOpacity");
    }
  }

  public static int TargetDotSize
  {
    get
    {
      return GetConfigInteger("TargetDotSize");
    }
  }

  public static int TargetPenWidth
  {
    get
    {
      return GetConfigInteger("TargetPenWidth");
    }
  }

  public static string TargetPolygonMode
  {
    get
    {
      string polygonMode = GetConfigSetting("TargetPolygonMode");

      if (polygonMode != null)
      {
        polygonMode = polygonMode.ToLower();
      }

      return polygonMode;
    }
  }

  public static int ZoomLevels
  {
    get
    {
      return GetConfigInteger("ZoomLevels");
    }
  }

  private static Color BlendColors(Color backColor, Color foreColor, double foreOpacity)
  {
    int r = Convert.ToInt32(backColor.R * (1 - foreOpacity) + foreColor.R * foreOpacity);
    int g = Convert.ToInt32(backColor.G * (1 - foreOpacity) + foreColor.G * foreOpacity);
    int b = Convert.ToInt32(backColor.B * (1 - foreOpacity) + foreColor.B * foreOpacity);
    return Color.FromArgb(r, g, b);
  }

  public static bool GetConfigBoolean(string name)
  {
    string value = GetConfigSetting(name);
    return String.Compare(value, "true", false) == 0 || String.Compare(value, "yes", false) == 0;
  }

  public static Color GetConfigColor(string name)
  {
    Color color = Color.Empty;

    if (GetConfigSetting(name) != null)
    {
      try
      {
        color = ColorTranslator.FromHtml(GetConfigSetting(name));
      }
      catch { }
    }

    return color;
  }

  public static double GetConfigDouble(string name)
  {
    double value = Double.NaN;

    if (GetConfigSetting(name) != null)
    {
      try
      {
        value = Convert.ToDouble(GetConfigSetting(name));
      }
      catch { }
    }

    return value;
  }

  public static int GetConfigInteger(string name)
  {
    int value = Int32.MinValue;

    if (GetConfigSetting(name) != null)
    {
      try
      {
        value = Convert.ToInt32(GetConfigSetting(name));
      }
      catch { }
    }

    return value;
  }

  public static string GetConfigSetting(string name)
  {
    return ConfigurationManager.AppSettings[name];
  }

  public static string ToJson()
  {
    Dictionary<String, Object> jsonData = new Dictionary<String, Object>();
    jsonData.Add("preserveOnActionChange", PreserveOnActionChange);
    jsonData.Add("identifyWindowWidth", IdentifyWindowWidth);
    jsonData.Add("identifyWindowHeight", IdentifyWindowHeight);
    jsonData.Add("identifyPopup", IdentifyPopup);
    jsonData.Add("isPublic", String.IsNullOrEmpty(AppUser.Name));
    jsonData.Add("mapUnits", MapUnits);
    jsonData.Add("measureUnits", MeasureUnits);

    JavaScriptSerializer serializer = new JavaScriptSerializer();
    return serializer.Serialize(jsonData);
  }
}