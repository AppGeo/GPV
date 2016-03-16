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
using System.Linq;
using System.Web;

public class ExternalMapHandler : WebServiceHandler 
{
  [WebServiceMethod]
  private void DefaultMethod()
  {
    AppSettings appSettings = AppContext.AppSettings;

    string name = Request.Form["name"];

    double minx = Convert.ToDouble(Request.Form["minx"]);
    double miny = Convert.ToDouble(Request.Form["miny"]);
    double maxx = Convert.ToDouble(Request.Form["maxx"]);
    double maxy = Convert.ToDouble(Request.Form["maxy"]);
    double pixelSize = Convert.ToDouble(Request.Form["pixelsize"]);

    double x = (minx + maxx) / 2;
    double y = (miny + maxy) / 2;

    double lon;
    double lat;
    appSettings.MapCoordinateSystem.ToGeodetic(x, y, out lon, out lat);

    double minLon;
    double minLat;
    appSettings.MapCoordinateSystem.ToGeodetic(minx, miny, out minLon, out minLat);

    double maxLon;
    double maxLat;
    appSettings.MapCoordinateSystem.ToGeodetic(maxx, maxy, out maxLon, out maxLat);

    string units = appSettings.MapCoordinateSystem.MapUnits;

    if (!appSettings.MapCoordinateSystem.Equals(appSettings.MeasureCoordinateSystem))
    {
      appSettings.MeasureCoordinateSystem.ToProjected(lon, lat, out x, out y);
      appSettings.MeasureCoordinateSystem.ToProjected(minLon, minLat, out minx, out miny);
      appSettings.MeasureCoordinateSystem.ToProjected(maxLon, maxLat, out maxx, out maxy);
      units = appSettings.MeasureCoordinateSystem.MapUnits;
    }

    double xm = x;
    double ym = y;
    double minxm = minx;
    double minym = miny;
    double maxxm = maxx;
    double maxym = maxy;

    if (units == "feet")
    {
      xm *= Constants.MetersPerFoot;
      ym *= Constants.MetersPerFoot;
      minxm *= Constants.MetersPerFoot;
      minym *= Constants.MetersPerFoot;
      maxxm *= Constants.MetersPerFoot;
      maxym *= Constants.MetersPerFoot;
    }

    double xft = x;
    double yft = y;
    double minxft = minx;
    double minyft = miny;
    double maxxft = maxx;
    double maxyft = maxy;

    if (units == "meters")
    {
      xft *= Constants.FeetPerMeter;
      yft *= Constants.FeetPerMeter;
      minxft *= Constants.FeetPerMeter;
      minyft *= Constants.FeetPerMeter;
      maxxft *= Constants.FeetPerMeter;
      maxyft *= Constants.FeetPerMeter;
    }

    if (appSettings.MapCoordinateSystem.MapUnits == "feet")
    {
      pixelSize *= Constants.MetersPerFoot;
    }

    double zoomLevel = (Math.Log(156543.0339280234 / pixelSize) / Math.Log(2));

    Configuration.ExternalMapRow externalMap = Configuration.ExternalMap.First(o => o.DisplayName == name);
    string url = externalMap.URL;

    url = url.Replace("{lat}", lat.ToString("0.0000000"));
    url = url.Replace("{lon}", lon.ToString("0.0000000"));
    url = url.Replace("{minlat}", minLat.ToString("0.0000000"));
    url = url.Replace("{minlon}", minLon.ToString("0.0000000"));
    url = url.Replace("{maxlat}", maxLat.ToString("0.0000000"));
    url = url.Replace("{maxlon}", maxLon.ToString("0.0000000"));
    url = url.Replace("{lev}", zoomLevel.ToString("0"));

    url = url.Replace("{x}", x.ToString("0.00"));
    url = url.Replace("{y}", y.ToString("0.00"));
    url = url.Replace("{minx}", minx.ToString("0.00"));
    url = url.Replace("{miny}", miny.ToString("0.00"));
    url = url.Replace("{maxx}", maxx.ToString("0.00"));
    url = url.Replace("{maxy}", maxy.ToString("0.00"));

    url = url.Replace("{xm}", xm.ToString("0.00"));
    url = url.Replace("{ym}", ym.ToString("0.00"));
    url = url.Replace("{minxm}", minxm.ToString("0.00"));
    url = url.Replace("{minym}", minym.ToString("0.00"));
    url = url.Replace("{maxxm}", maxxm.ToString("0.00"));
    url = url.Replace("{maxym}", maxym.ToString("0.00"));

    url = url.Replace("{xft}", xft.ToString("0.00"));
    url = url.Replace("{yft}", yft.ToString("0.00"));
    url = url.Replace("{minxft}", minxft.ToString("0.00"));
    url = url.Replace("{minyft}", minyft.ToString("0.00"));
    url = url.Replace("{maxxft}", maxxft.ToString("0.00"));
    url = url.Replace("{maxyft}", maxyft.ToString("0.00"));

    ReturnJson("url", url);
  }
}