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
using GeoAPI.Geometries;

public class ExternalMapHandler : WebServiceHandler 
{
  [WebServiceMethod]
  private void DefaultMethod()
  {
    AppSettings appSettings = AppContext.AppSettings;

    string name = Request.Form["name"];

    Coordinate min = new Coordinate(Convert.ToDouble(Request.Form["minx"]), Convert.ToDouble(Request.Form["miny"]));
    Coordinate max = new Coordinate(Convert.ToDouble(Request.Form["maxx"]), Convert.ToDouble(Request.Form["maxy"]));
    
    Envelope extent = new Envelope(min, max);
    Coordinate center = extent.Centre;

    double pixelSize = Convert.ToDouble(Request.Form["pixelsize"]);

    Envelope gExtent = appSettings.MapCoordinateSystem.ToGeodetic(extent);
    Coordinate gCenter = appSettings.MapCoordinateSystem.ToGeodetic(center);

    string units = appSettings.MapCoordinateSystem.MapUnits;

    if (!appSettings.MapCoordinateSystem.Equals(appSettings.MeasureCoordinateSystem))
    {
      extent = appSettings.MeasureCoordinateSystem.ToProjected(gExtent);
      center = appSettings.MeasureCoordinateSystem.ToProjected(gCenter);
      units = appSettings.MeasureCoordinateSystem.MapUnits;
    }

    double xm = center.X;
    double ym = center.Y;
    double minxm = extent.MinX;
    double minym = extent.MinY;
    double maxxm = extent.MaxX;
    double maxym = extent.MaxY;

    if (units == "feet")
    {
      xm *= Constants.MetersPerFoot;
      ym *= Constants.MetersPerFoot;
      minxm *= Constants.MetersPerFoot;
      minym *= Constants.MetersPerFoot;
      maxxm *= Constants.MetersPerFoot;
      maxym *= Constants.MetersPerFoot;
    }

    double xft = center.X;
    double yft = center.Y;
    double minxft = extent.MinX;
    double minyft = extent.MinY;
    double maxxft = extent.MaxX;
    double maxyft = extent.MaxY;

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

    url = url.Replace("{lat}", gCenter.Y.ToString("0.0000000"));
    url = url.Replace("{lon}", gCenter.X.ToString("0.0000000"));
    url = url.Replace("{minlat}", gExtent.MinY.ToString("0.0000000"));
    url = url.Replace("{minlon}", gExtent.MinX.ToString("0.0000000"));
    url = url.Replace("{maxlat}", gExtent.MaxY.ToString("0.0000000"));
    url = url.Replace("{maxlon}", gExtent.MaxX.ToString("0.0000000"));
    url = url.Replace("{lev}", zoomLevel.ToString("0"));

    url = url.Replace("{x}", center.X.ToString("0.00"));
    url = url.Replace("{y}", center.Y.ToString("0.00"));
    url = url.Replace("{minx}", extent.MinX.ToString("0.00"));
    url = url.Replace("{miny}", extent.MinY.ToString("0.00"));
    url = url.Replace("{maxx}", extent.MaxX.ToString("0.00"));
    url = url.Replace("{maxy}", extent.MaxY.ToString("0.00"));

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