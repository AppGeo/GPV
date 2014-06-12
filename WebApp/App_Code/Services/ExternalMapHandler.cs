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
using System.Linq;
using System.Web;

public class ExternalMapHandler : WebServiceHandler 
{
  [WebServiceMethod]
  private void DefaultMethod()
  {
    string name = Request.Form["name"];

    double minx = Convert.ToDouble(Request.Form["minx"]);
    double miny = Convert.ToDouble(Request.Form["miny"]);
    double maxx = Convert.ToDouble(Request.Form["maxx"]);
    double maxy = Convert.ToDouble(Request.Form["maxy"]);
    double pixelSize = Convert.ToDouble(Request.Form["pixelsize"]);

    double x = (minx + maxx) / 2;
    double y = (miny + maxy) / 2;

    double xm = x;
    double ym = y;
    double minxm = minx;
    double minym = miny;
    double maxxm = maxx;
    double maxym = maxy;

    if (AppSettings.MapUnits == "feet")
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

    if (AppSettings.MapUnits == "meters")
    {
      xft *= Constants.FeetPerMeter;
      yft *= Constants.FeetPerMeter;
      minxft *= Constants.FeetPerMeter;
      minyft *= Constants.FeetPerMeter;
      maxxft *= Constants.FeetPerMeter;
      maxyft *= Constants.FeetPerMeter;

      pixelSize *= Constants.FeetPerMeter;
    }

    double f = AppSettings.MapUnits == "feet" ? Constants.MetersPerFoot : 1;

    double lon;
    double lat;
    AppSettings.CoordinateSystem.ToGeodetic((x - AppSettings.DatumShiftX) * f, (y - AppSettings.DatumShiftY) * f, out lon, out lat);

    double minLon;
    double minLat;
    AppSettings.CoordinateSystem.ToGeodetic((minx - AppSettings.DatumShiftX) * f, (miny - AppSettings.DatumShiftY) * f, out minLon, out minLat);

    double maxLon;
    double maxLat;
    AppSettings.CoordinateSystem.ToGeodetic((maxx - AppSettings.DatumShiftX) * f, (maxy - AppSettings.DatumShiftY) * f, out maxLon, out maxLat);

    double zoomLevel = (Math.Log(190500 / pixelSize) / Math.Log(2)) + 1;

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