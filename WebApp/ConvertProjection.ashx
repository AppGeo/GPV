<%@ WebHandler Language="C#" Class="ConvertProjection" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

public class ConvertProjection : IHttpHandler
{
  public void ProcessRequest(HttpContext context)
  {
    List<string> parameters = new List<string>();
    string paramFormat = "+{0}={1}";
    
    // projection name
    
    string proj = ConfigurationManager.AppSettings["Projection"];

    if (String.IsNullOrEmpty(proj))
    {
      ReturnError(context, "Projection is not defined in Web.config");
      return;
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
        ReturnError(context, "Unknown Projection in Web.config: " + proj);
        return;
    }

    parameters.Add(String.Format(paramFormat, "proj", proj));
    
    // central meridian
    
    double centralMeridian;

    if (!Double.TryParse(ConfigurationManager.AppSettings["CentralMeridian"], out centralMeridian))
    {
      ReturnError(context, "CentralMeridian is not defined or invalid in Web.config");
      return;
    }

    parameters.Add(String.Format(paramFormat, "lon_0", centralMeridian));

    // origin latitude
    
    double originLatitude;

    if (!Double.TryParse(ConfigurationManager.AppSettings["OriginLatitude"], out originLatitude))
    {
      ReturnError(context, "OriginLatitude is not defined or invalid in Web.config");
      return;
    }

    parameters.Add(String.Format(paramFormat, "lat_0", originLatitude));

    // parameters specific to Lambert Conformal Conic
    
    if (proj == "lcc")
    {
      // first standard parallel
      
      double standardParallel1;

      if (!Double.TryParse(ConfigurationManager.AppSettings["StandardParallel1"], out standardParallel1))
      {
        ReturnError(context, "StandardParallel1 is not defined or invalid in Web.config");
        return;
      }

      parameters.Add(String.Format(paramFormat, "lat_1", standardParallel1));

      // second standard parallel

      double standardParallel2;

      if (!Double.TryParse(ConfigurationManager.AppSettings["StandardParallel2"], out standardParallel2))
      {
        ReturnError(context, "StandardParallel2 is not defined or invalid in Web.config");
        return;
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
        ReturnError(context, "ScaleFactor is not defined or invalid in Web.config");
        return;
      }

      parameters.Add(String.Format(paramFormat, "k", scaleFactor));
    }

    // false easting
    
    double falseEasting;

    if (!Double.TryParse(ConfigurationManager.AppSettings["FalseEasting"], out falseEasting))
    {
      ReturnError(context, "FalseEasting is not defined or invalid in Web.config");
      return;
    }

    parameters.Add(String.Format(paramFormat, "x_0", falseEasting));
    
    // false northing
    
    double falseNorthing;

    if (!Double.TryParse(ConfigurationManager.AppSettings["FalseNorthing"], out falseNorthing))
    {
      ReturnError(context, "FalseNorthing is not defined or invalid in Web.config");
      return;
    }

    parameters.Add(String.Format(paramFormat, "y_0", falseNorthing));

    // ellipsoid and datum
    
    parameters.Add(String.Format(paramFormat, "ellps", "GRS80"));
    parameters.Add(String.Format(paramFormat, "datum", "NAD83"));

    // units
    
    string mapUnits = ConfigurationManager.AppSettings["MapUnits"];

    if (String.IsNullOrEmpty(mapUnits))
    {
      ReturnError(context, "MapUnits is not defined in Web.config");
      return;
    }

    double toMeter = mapUnits.ToLower() == "feet" ? 0.3048006096012192 : 1;
    parameters.Add(String.Format(paramFormat, "to_meter", toMeter));

    parameters.Add("+no_defs");
    
    // output to page
    
    string configProj = String.Format("<add key=\"Projection\" value=\"{0}\" />", String.Join(" ", parameters));
    string page = String.Format("<html><body><code>{0}</code></body></html>", context.Server.HtmlEncode(configProj));
      
    context.Response.ContentType = "text/html";
    context.Response.Write(page);
  }

  private void ReturnError(HttpContext context, string message)
  {
    context.Response.ContentType = "text/plain";
    context.Response.Write(message);
  }
  
  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}