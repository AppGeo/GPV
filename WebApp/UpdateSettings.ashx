<%@ WebHandler Language="C#" Class="ConvertSettings" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;

public class ConvertSettings : IHttpHandler
{
  public void ProcessRequest(HttpContext context)
  {
    context.Response.ContentType = "text/plain";
    context.Response.AddHeader("Content-Disposition", "attachment; filename=UpdateSettings.sql");

    string[] settings = new string[] { "AdminEmail", 
      "AllowShowApps", "DefaultApplication", "FullExtent", "ZoomLevels", "ShowScaleBar", "MeasureUnits", 
      "ActiveColor", "ActiveOpacity", "ActivePolygonMode", "ActivePenWidth", "ActiveDotSize", 
      "TargetColor", "TargetOpacity", "TargetPolygonMode", "TargetPenWidth", "TargetDotSize", 
      "SelectionColor", "SelectionOpacity", "SelectionPolygonMode", "SelectionPenWidth", "SelectionDotSize", 
      "FilteredColor", "FilteredOpacity", "FilteredPolygonMode", "FilteredPenWidth", "FilteredDotSize", 
      "BufferColor", "BufferOpacity", "BufferPolygonMode", "BufferPenWidth", "BufferDotSize", 
      "SwatchTileWidth", "SwatchTileHeight", "LegendExpanded", "PreserveOnActionChange", "CustomStyleSheet",
      "ExportFormat", "MarkupTimeout", "ServerImageCacheTimeout", "BrowserImageCacheTimeout"
    };
  
    string prefix = ConfigurationManager.AppSettings["ConfigTablePrefix"];
    string updateFormat = "update {0}Setting set Value = {1} where Setting = '{2}'\n";
    
    for (int i = 0; i < settings.Length; ++i)
    {
      if (ConfigurationManager.AppSettings.AllKeys.Contains( settings[i]))
      {
        string value = ConfigurationManager.AppSettings[settings[i]];
        value = value == null || value.Trim().Length == 0 ? "null" : String.Format("'{0}'", value.Replace("'", "''"));
        context.Response.Write(String.Format(updateFormat, prefix, value, settings[i]));
      }
    }

    string projection = ConvertProjection();

    if (!String.IsNullOrEmpty(projection))
    {
      context.Response.Write(String.Format(updateFormat, prefix, String.Format("'{0}'", projection), "MapProjection"));
    }
  }

  private string ConvertProjection()
  {
    List<string> parameters = new List<string>();
    string paramFormat = "+{0}={1}";

    // projection name

    string proj = ConfigurationManager.AppSettings["Projection"];
    string spheroid = ConfigurationManager.AppSettings["Spheroid"];

    if (String.IsNullOrEmpty(proj) || (!String.IsNullOrEmpty(spheroid) && spheroid != "GRS80"))
    {
      return null;
    }

    switch (proj.ToLower())
    {
      case "lambertconformalconic":
        proj = "lcc";
        break;

      case "transversemercator":
        proj = "tmerc";
        break;

      default:
        return null;
    }

    parameters.Add(String.Format(paramFormat, "proj", proj));

    // central meridian

    double centralMeridian;

    if (!Double.TryParse(ConfigurationManager.AppSettings["CentralMeridian"], out centralMeridian))
    {
      return null;
    }

    parameters.Add(String.Format(paramFormat, "lon_0", centralMeridian));

    // origin latitude

    double originLatitude;

    if (!Double.TryParse(ConfigurationManager.AppSettings["OriginLatitude"], out originLatitude))
    {
      return null;
    }

    parameters.Add(String.Format(paramFormat, "lat_0", originLatitude));

    // parameters specific to Lambert Conformal Conic

    if (proj == "lcc")
    {
      // first standard parallel

      double standardParallel1;

      if (!Double.TryParse(ConfigurationManager.AppSettings["StandardParallel1"], out standardParallel1))
      {
        return null;
      }

      parameters.Add(String.Format(paramFormat, "lat_1", standardParallel1));

      // second standard parallel

      double standardParallel2;

      if (!Double.TryParse(ConfigurationManager.AppSettings["StandardParallel2"], out standardParallel2))
      {
        return null;
      }

      parameters.Add(String.Format(paramFormat, "lat_2", standardParallel2));
    }

    // parameters specific to Transverse Mercator

    else
    {
      // scale factor

      double scaleFactor;

      if (!Double.TryParse(ConfigurationManager.AppSettings["ScaleFactor"], out scaleFactor))
      {
        return null;
      }

      parameters.Add(String.Format(paramFormat, "k", scaleFactor));
    }

    // false easting

    double falseEasting;

    if (!Double.TryParse(ConfigurationManager.AppSettings["FalseEasting"], out falseEasting))
    {
      return null;
    }

    parameters.Add(String.Format(paramFormat, "x_0", falseEasting));

    // false northing

    double falseNorthing;

    if (!Double.TryParse(ConfigurationManager.AppSettings["FalseNorthing"], out falseNorthing))
    {
      return null;
    }

    parameters.Add(String.Format(paramFormat, "y_0", falseNorthing));

    // ellipsoid and datum

    parameters.Add(String.Format(paramFormat, "ellps", "GRS80"));
    parameters.Add(String.Format(paramFormat, "datum", "NAD83"));

    // units

    string mapUnits = ConfigurationManager.AppSettings["MapUnits"];

    if (String.IsNullOrEmpty(mapUnits))
    {
      return null;
    }

    double toMeter = mapUnits.ToLower() == "feet" ? 0.3048006096012192 : 1;
    parameters.Add(String.Format(paramFormat, "to_meter", toMeter));

    parameters.Add("+no_defs");

    return String.Join(" ", parameters);
  }
  
  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}