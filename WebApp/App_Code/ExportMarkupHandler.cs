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
using System.IO;
using System.Text;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;
using GeoAPI.Geometries;
using NetTopologySuite.IO;

public class ExportMarkupHandler : IHttpHandler
{
  private CoordinateSystem _coordSys = AppSettings.CoordinateSystem;
  private HttpResponse Response = null;

  public void ProcessRequest(HttpContext context)
  {
    Response = context.Response;

    string appId = context.Request.QueryString["app"];
    int groupId;

    if (!String.IsNullOrEmpty(appId) && Int32.TryParse(context.Request.QueryString["group"], out groupId))
    {
      Response.Clear();

      List<String> placemarks = new List<String>();
      Dictionary<String, String> styles = new Dictionary<String, String>();
      string appName = null;
      string groupName = null;

      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("update {0}MarkupGroup set DateLastAccessed = ? where GroupID = ?", AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Date).Value = DateTime.Now;
          command.Parameters.Add("@2", OleDbType.Integer).Value = groupId;
          command.ExecuteNonQuery();

          command.CommandText = String.Format("select Shape, Color, Text from {0}Markup where GroupID = ? and Deleted = 0", AppSettings.ConfigurationTablePrefix);
          command.Parameters.Clear();
          command.Parameters.Add("@1", OleDbType.Integer).Value = groupId;

          WKTReader wktReader = new WKTReader();

          using (OleDbDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              IGeometry geometry = wktReader.Read(reader.GetString(0));
              string coordinates = GetCoordinates(geometry);
              string color = reader.GetString(1);
              bool isText = !reader.IsDBNull(2);

              string styleId = GetStyle(geometry.OgcGeometryType, color, isText, styles);

              switch (geometry.OgcGeometryType)
              {
                case OgcGeometryType.Point:
                  string name = isText ? String.Format("<name>{0}</name>", reader.GetString(2)) : "";
                  placemarks.Add(String.Format("<Placemark>{0}<styleUrl>#{1}</styleUrl><Point>{2}</Point></Placemark>", name, styleId, coordinates));
                  break;

                case OgcGeometryType.LineString:
                  placemarks.Add(String.Format("<Placemark><styleUrl>#{0}</styleUrl><LineString>{1}</LineString></Placemark>", styleId, coordinates));
                  break;

                case OgcGeometryType.Polygon:
                  placemarks.Add(String.Format("<Placemark><styleUrl>#{0}</styleUrl><Polygon><outerBoundaryIs><LinearRing>{1}</LinearRing></outerBoundaryIs></Polygon></Placemark>", styleId, coordinates));
                  break;
              }
            }
          }

          Configuration config = AppContext.GetConfiguration();
          Configuration.ApplicationRow application = config.Application.Select(String.Format("ApplicationID = '{0}'", appId))[0] as Configuration.ApplicationRow;
          appName = application.DisplayName;

          command.CommandText = String.Format("select DisplayName from {0}MarkupGroup where GroupID = ?", AppSettings.ConfigurationTablePrefix);
          groupName = command.ExecuteScalar() as string;
        }
      }

      string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssff");
      string kmzName = String.Format("Markup_{0}.kmz", timeStamp);
      string kmlName = String.Format("Markup_{0}.kml", timeStamp);

      string kml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
          <kml xmlns=""http://earth.google.com/kml/2.2"">
            <Folder>
              <name>{0}</name>
              <Document>
                <name>Markup: {1}</name>
                {2}
                {3}
              </Document>
            </Folder>
          </kml>";

      string[] styleArray = new string[styles.Values.Count];
      styles.Values.CopyTo(styleArray, 0);

      kml = String.Format(kml, appName, groupName, String.Join("", styleArray), String.Join("", placemarks.ToArray()));

      Response.ContentType = "application/vnd.google-earth.kmz";
      Response.AddHeader("Content-Disposition", "attachment; filename=" + kmzName);

      ZipOutputStream zipStream = new ZipOutputStream(Response.OutputStream);

      MemoryStream memoryStream = new MemoryStream();
      byte[] buffer = (new UTF8Encoding()).GetBytes(kml);

      ZipEntry entry = new ZipEntry(kmlName);
      entry.Size = buffer.Length;
      zipStream.PutNextEntry(entry);
      zipStream.Write(buffer, 0, buffer.Length);

      zipStream.Finish();
      Response.End();
    }
  }

  private string GetCoordinates(IGeometry geometry)
  {
    double f = AppSettings.MapUnits == "feet" ? Constants.MetersPerFoot : 1;

    List<Coordinate> points = new List<Coordinate>();
    List<String> coordinates = new List<String>();

    switch (geometry.OgcGeometryType)
    {
      case OgcGeometryType.Point: points.Add(((IPoint)geometry).Coordinate); break;
      case OgcGeometryType.LineString: points.AddRange(((ILineString)geometry).Coordinates); break;
      case OgcGeometryType.Polygon: points.AddRange(((IPolygon)geometry).ExteriorRing.Coordinates); break;
    }

    for (int i = 0; i < points.Count; ++i)
    {
      double lat; double lon;
      _coordSys.ToGeodetic(points[i].X * f, points[i].Y * f, out lon, out lat);
      coordinates.Add(String.Format("{0},{1},0", lon, lat));
    }

    return String.Format("<coordinates>{0}</coordinates>", String.Join(" ", coordinates.ToArray()));
  }

  private string GetStyle(OgcGeometryType geometryType, string color, bool isText, Dictionary<String, String> styles)
  {
    string r = color.ToLower().Substring(1, 2);
    string g = color.ToLower().Substring(3, 2);
    string b = color.ToLower().Substring(5, 2);
    color = b + g + r;

    string styleId = null;

    switch (geometryType)
    {
      case OgcGeometryType.Point:
        if (isText)
        {
          styleId = "T" + color;

          if (!styles.ContainsKey(styleId))
          {
            styles.Add(styleId, String.Format("<Style id=\"{0}\"><IconStyle><scale>0.00001</scale></IconStyle><LabelStyle><color>ff{1}</color></LabelStyle></Style>", styleId, color));
          }
        }
        else
        {
          styleId = "P" + color;

          if (!styles.ContainsKey(styleId))
          {
            styles.Add(styleId, String.Format("<Style id=\"{0}\"><IconStyle><color>ff{1}</color><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon57.png</href></Icon></IconStyle></Style>", styleId, color));
          }
        }
        break;

      case OgcGeometryType.LineString:
        styleId = "L" + color;

        if (!styles.ContainsKey(styleId))
        {
          styles.Add(styleId, String.Format("<Style id=\"{0}\"><LineStyle><color>ff{1}</color><width>2</width></LineStyle></Style>", styleId, color));
        }
        break;

      case OgcGeometryType.Polygon:
        styleId = "A" + color;

        if (!styles.ContainsKey(styleId))
        {
          styles.Add(styleId, String.Format("<Style id=\"{0}\"><PolyStyle><color>80{1}</color></PolyStyle><LineStyle><color>ff{1}</color><width>2</width></LineStyle></Style>", styleId, color));
        }
        break;
    }

    return styleId;
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}