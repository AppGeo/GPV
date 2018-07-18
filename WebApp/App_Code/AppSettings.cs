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
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using GeoAPI.Geometries;

public class AppSettings
{
  private Dictionary<string, string> _configSetting;
  private CoordinateSystem _mapCoordinateSystem = null;
  private CoordinateSystem _measureCoordinateSystem = null;
  private Envelope _fullExtent = null;

  public AppSettings(Configuration.SettingDataTable settingTable)
  {
    // load settings from table into a dictionary for quick access

    _configSetting = new Dictionary<string, string>();

    foreach (Configuration.SettingRow settingRow in settingTable)
    {
      _configSetting.Add(settingRow.Setting, !settingRow.IsValueNull() ? settingRow.Value : null);
    }

    // load projections

    try
    {
      string proj4String = GetConfigSetting("MapProjection");

      if (String.IsNullOrWhiteSpace(proj4String))
      {
        _mapCoordinateSystem = new CoordinateSystem("+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +no_defs");
      }
      else
      {
        _mapCoordinateSystem = new CoordinateSystem(proj4String);
      }

      proj4String = GetConfigSetting("MeasureProjection");

      if (String.IsNullOrWhiteSpace(proj4String))
      {
        _measureCoordinateSystem = MapCoordinateSystem;
      }
      else
      {
        _measureCoordinateSystem = new CoordinateSystem(proj4String);
      }
    }
    catch { }

    // extract full extent from source setting and project if necessary

    if (_configSetting["FullExtent"] != null)
    {
      try
      {
        _fullExtent = EnvelopeExtensions.FromDelimitedString(_configSetting["FullExtent"]);

        if (_mapCoordinateSystem != null && _measureCoordinateSystem != null && !_mapCoordinateSystem.Equals(_measureCoordinateSystem))
        {
          _fullExtent = _mapCoordinateSystem.ToProjected(_measureCoordinateSystem.ToGeodetic(_fullExtent));
        }
      }
      catch { }
    }
  }

  public Color ActiveColor
  {
    get
    {
      return GetConfigColor("ActiveColor", Color.Yellow);
    }
  }

  public Color ActiveColorUI
  {
    get
    {
      return BlendColors(Color.White, ActiveColor, ActiveOpacity);
    }
  }

  public double ActiveOpacity
  {
    get
    {
      return GetConfigDouble("ActiveOpacity", 0.5);
    }
  }

  public int ActiveDotSize
  {
    get
    {
      return GetConfigInteger("ActiveDotSize", 13);
    }
  }

  public int ActivePenWidth
  {
    get
    {
      return GetConfigInteger("ActivePenWidth", 9);
    }
  }

  public string ActivePolygonMode
  {
    get
    {
      string mode = GetConfigSetting("ActivePolygonMode", "fill").ToLower();

      if (mode != "fill" && mode != "outline")
      {
        mode = "fill";
      }

      return mode;
    }
  }

  public string AdminEmail
  {
    get
    {
      return GetConfigSetting("AdminEmail");
    }
  }

  public bool AllowShowApps
  {
    get
    {
      return GetConfigBoolean("AllowShowApps", false);
    }
  }

  public int BrowserImageCacheTimeout
  {
    get
    {
      return GetConfigInteger("BrowserImageCacheTimeout", 60);
    }
  }
  
  public Color BufferColor
  {
    get
    {
      return GetConfigColor("BufferColor", ColorTranslator.FromHtml("#A0A0FF"));
    }
  }

  public double BufferOpacity
  {
    get
    {
      return GetConfigDouble("BufferOpacity", 0.2);
    }
  }

  public Color BufferOutlineColor
  {
    get
    {
      return GetConfigColor("BufferOutlineColor", ColorTranslator.FromHtml("#8080DD"));
    }
  }

  public double BufferOutlineOpacity
  {
    get
    {
      return GetConfigDouble("BufferOutlineOpacity", 0);
    }
  }

  public int BufferOutlinePenWidth
  {
    get
    {
      return GetConfigInteger("BufferOutlinePenWidth", 0);
    }
  }

  public System.Drawing.Font CoordinatesFont
  {
    get
    {
      return new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
    }
  }

  public string CustomStyleSheet
  {
    get
    {
      return GetConfigSetting("CustomStyleSheet");
    }
  }

  public string DefaultApplication
  {
    get
    {
      return GetConfigSetting("DefaultApplication");
    }
  }

  public Envelope DefaultFullExtent
  {
    get
    {
      return new Envelope(_fullExtent);
    }
  }

  public double DynamicMapImageShiftX
  {
    get
    {
      return GetConfigDouble("DynamicMapImageShiftX");
    }
  }

  public double DynamicMapImageShiftY
  {
    get
    {
      return GetConfigDouble("DynamicMapImageShiftY");
    }
  }

  public string ExportFormat
  {
    get
    {
      string mode = GetConfigSetting("ExportFormat", "xls").ToLower();

      if (mode != "xls" && mode != "csv")
      {
        mode = "xls";
      }

      return mode;
    }
  }

  public Color FilteredColor
  {
    get
    {
      return GetConfigColor("FilteredColor", ColorTranslator.FromHtml("#A0A0A0"));
    }
  }

  public double FilteredOpacity
  {
    get
    {
      return GetConfigDouble("FilteredOpacity", 0.5);
    }
  }

  public int FilteredDotSize
  {
    get
    {
      return GetConfigInteger("FilteredDotSize", 13);
    }
  }

  public int FilteredPenWidth
  {
    get
    {
      return GetConfigInteger("FilteredPenWidth", 9);
    }
  }

  public string FilteredPolygonMode
  {
    get
    {
      string mode = GetConfigSetting("FilteredPolygonMode", "fill").ToLower();

      if (mode != "fill" && mode != "outline")
      {
        mode = "fill";
      }

      return mode;
    }
  }

  public bool ShowLogo
  {
    get
    {
      return GetConfigBoolean("ShowLogo", true);
    }
  }

  public bool LegendExpanded
  {
    get
    {
      return GetConfigBoolean("LegendExpanded", true);
    }
  }

  public CoordinateSystem MapCoordinateSystem
  {
    get
    {
      return _mapCoordinateSystem;
    }
  }

  public string MapUnits
  {
    get
    {
      return MapCoordinateSystem.MapUnits;
    }
  }

  public System.Drawing.Font MarkupFont
  {
    get
    {
      return new System.Drawing.Font("Verdana", 10, System.Drawing.FontStyle.Bold);
    }
  }

  public double MarkupShiftX
  {
    get
    {
      return GetConfigDouble("MarkupShiftX");
    }
  }

  public double MarkupShiftY
  {
    get
    {
      return GetConfigDouble("MarkupShiftY");
    }
  }

  public int MarkupTimeout
  {
    get
    {
      return GetConfigInteger("MarkupTimeout", 14);
    }
  }

  public CoordinateSystem MeasureCoordinateSystem
  {
    get
    {
      return _measureCoordinateSystem;
    }
  }

  public System.Drawing.Font MeasureFont
  {
    get
    {
      return new System.Drawing.Font("Verdana", 11, GraphicsUnit.Pixel);
    }
  }

  public string MeasureUnits
  {
    get
    {
      string measureUnits = GetConfigSetting("MeasureUnits", "feet").ToLower();

      if (measureUnits != "feet" && measureUnits != "meters" && measureUnits != "both")
      {
        measureUnits = "feet";
      }

      return measureUnits;
    }
  }

  public string PreserveOnActionChange
  {
    get
    {
      string mode = GetConfigSetting("PreserveOnActionChange", "selection").ToLower();

      if (mode != "selection" && mode != "target")
      {
        mode = "selection";
      }

      return mode;
    }
  }

  public bool SearchAutoSelect
  {
    get
    {
      return GetConfigBoolean("SearchAutoSelect", false);
    }
  }

  public Color SelectionColor
  {
    get
    {
      return GetConfigColor("SelectionColor", Color.Blue);
    }
  }

  public Color SelectionColorUI
  {
    get
    {
      return BlendColors(Color.White, SelectionColor, SelectionOpacity);
    }
  }

  public double SelectionOpacity
  {
    get
    {
      return GetConfigDouble("SelectionOpacity", 0.5);
    }
  }

  public int SelectionDotSize
  {
    get
    {
      return GetConfigInteger("SelectionDotSize", 13);
    }
  }

  public int SelectionPenWidth
  {
    get
    {
      return GetConfigInteger("SelectionPenWidth", 9);
    }
  }

  public string SelectionPolygonMode
  {
    get
    {
      string mode = GetConfigSetting("SelectionPolygonMode", "fill").ToLower();

      if (mode != "fill" && mode != "outline")
      {
        mode = "fill";
      }

      return mode;
    }
  }

  public double SelectionShiftX
  {
    get
    {
      return GetConfigDouble("SelectionShiftX");
    }
  }

  public double SelectionShiftY
  {
    get
    {
      return GetConfigDouble("SelectionShiftY");
    }
  }

  public int ServerImageCacheTimeout
  {
    get
    {
      return GetConfigInteger("ServerImageCacheTimeout", 60);
    }
  }

  public bool ShowScaleBar
  {
    get
    {
      return GetConfigBoolean("ShowScaleBar", false);
    }
  }

  public int SwatchTileHeight
  {
    get
    {
      return GetConfigInteger("SwatchTileHeight", 20);
    }
  }

  public int SwatchTileWidth
  {
    get
    {
      return GetConfigInteger("SwatchTileWidth", 20);
    }
  }

  public Color TargetColor
  {
    get
    {
      return GetConfigColor("TargetColor", Color.Orange);
    }
  }

  public Color TargetColorUI
  {
    get
    {
      return BlendColors(Color.White, TargetColor, TargetOpacity);
    }
  }

  public double TargetOpacity
  {
    get
    {
      return GetConfigDouble("TargetOpacity", 0.5);
    }
  }

  public int TargetDotSize
  {
    get
    {
      return GetConfigInteger("TargetDotSize", 13);
    }
  }

  public int TargetPenWidth
  {
    get
    {
      return GetConfigInteger("TargetPenWidth", 9);
    }
  }

  public string TargetPolygonMode
  {
    get
    {
      string mode = GetConfigSetting("TargetPolygonMode", "fill").ToLower();

      if (mode != "fill" && mode != "outline")
      {
        mode = "fill";
      }

      return mode;
    }
  }

  public int ZoomLevels
  {
    get
    {
      return GetConfigInteger("ZoomLevels", 19);
    }
  }

  private Color BlendColors(Color backColor, Color foreColor, double foreOpacity)
  {
    int r = Convert.ToInt32(backColor.R * (1 - foreOpacity) + foreColor.R * foreOpacity);
    int g = Convert.ToInt32(backColor.G * (1 - foreOpacity) + foreColor.G * foreOpacity);
    int b = Convert.ToInt32(backColor.B * (1 - foreOpacity) + foreColor.B * foreOpacity);
    return Color.FromArgb(r, g, b);
  }

  private bool GetConfigBoolean(string name, bool defaultValue)
  {
    string value = GetConfigSetting(name);
    return value != null ? (String.Compare(value, "true", false) == 0 || String.Compare(value, "yes", false) == 0) : defaultValue;
  }

  private bool GetConfigBoolean(string name)
  {
    return GetConfigBoolean(name, false);
  }

  private Color GetConfigColor(string name)
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

  private Color GetConfigColor(string name, Color defaultColor)
  {
    Color color = GetConfigColor(name);
    return !color.IsEmpty ? color : defaultColor;
  }

  private double GetConfigDouble(string name)
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

  private double GetConfigDouble(string name, double defaultValue)
  {
    double value = GetConfigDouble(name);
    return !Double.IsNaN(value) ? value : defaultValue;
  }

  private int GetConfigInteger(string name)
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

  private int GetConfigInteger(string name, int defaultValue)
  {
    int value = GetConfigInteger(name);
    return value > Int32.MinValue ? value : defaultValue;
  }

  private string GetConfigSetting(string name)
  {
    return _configSetting.Keys.Contains(name) ? _configSetting[name] : null;
  }

  private string GetConfigSetting(string name, string defaultValue)
  {
    string value = GetConfigSetting(name);
    return !String.IsNullOrEmpty(value) ? value : defaultValue;
  }

  private bool GetWebConfigBoolean(string name)
  {
    string value = GetWebConfigSetting(name);
    return value != null && (String.Compare(value, "true", false) == 0 || String.Compare(value, "yes", false) == 0);
  }

  private string GetWebConfigSetting(string name)
  {
    return ConfigurationManager.AppSettings[name];
  }

  public string ToJson()
  {
    Dictionary<String, Object> jsonData = new Dictionary<String, Object>();
    jsonData.Add("searchAutoSelect", SearchAutoSelect);
    jsonData.Add("showScaleBar", ShowScaleBar);
    jsonData.Add("preserveOnActionChange", PreserveOnActionChange);
    jsonData.Add("isPublic", String.IsNullOrEmpty(AppUser.Name));
    jsonData.Add("mapCrs", !MapCoordinateSystem.IsWebMercator ? MapCoordinateSystem.ToProj4String() : "");
    jsonData.Add("mapUnits", MapUnits);
    jsonData.Add("measureCrs", MeasureCoordinateSystem.ToProj4String());
    jsonData.Add("measureCrsUnits", MeasureCoordinateSystem.MapUnits);
    jsonData.Add("measureUnits", MeasureUnits);
    jsonData.Add("zoomLevels", ZoomLevels);

    JavaScriptSerializer serializer = new JavaScriptSerializer();
    return serializer.Serialize(jsonData);
  }
}