<%@ WebHandler Language="C#" Class="UpdateMarkup" %>

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
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

public class UpdateMarkup : IHttpHandler 
{
  // at least one of these should be set to true
  
  private bool FixGeometries = true;
  private bool ReprojectGeometries = true;

  private HttpContext _context = null;
  
  private static Regex _lineStringRegex = null;
  private static Regex _polygonRegex = null;
  private static Regex _typeRegex = null;
  private static Regex _spaceRegex = null;

  static UpdateMarkup()
  {
    string real = @"-?\d*\.?\d+";
    string point = real + @"\s*" + real;
    string pointList = point + @"(\s*,\s*" + point + @")*";

    _lineStringRegex = new Regex(@"LINESTRING\s*\((?<p>" + pointList + @")\)");
    _polygonRegex = new Regex(@"POLYGON\s*\(\((?<p>" + pointList + @")\)\)");

    string type = @"(?<t>POINT|LINESTRING|POLYGON)\s*\(";
    _typeRegex = new Regex(type);

    _spaceRegex = new Regex(@"\s+");
  }
  
  public void ProcessRequest (HttpContext context) 
  {
    _context = context;
    _context.Response.ContentType = "text/plain";
    
    AppSettings settings = Configuration.GetCurrent().AppSettings;
    
    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      if (!MarkupAlreadyUpdated(connection) && (!ReprojectGeometries || CanPerformProjection(settings)))
      {
        if (ProcessMarkup(connection, settings))
        {
          SetMarkupUpdated(connection);
        }
      }
    }
  }

  private void AddPoint(List<String> points, double xOffset, double yOffset)
  {
    double[] coords = points[points.Count - 1].Split(' ').Select(o => Convert.ToDouble(o)).ToArray();
    coords[0] += xOffset;
    coords[1] += yOffset;
    string newPoint = String.Join(" ", coords.Select(o => o.ToString()));
    points.Add(newPoint);
  }

  private bool CanPerformProjection(AppSettings settings)
  {
    bool canProject = !settings.MapCoordinateSystem.Equals(settings.MeasureCoordinateSystem);

    if (!canProject)
    {
      _context.Response.Write("No need to update markup; the map projection and measurement projection are the same.");
    }
    
    return canProject;
  }

  private string FixGeometry(string shape)
  {
    Match match = _typeRegex.Match(shape);

    if (match.Success)
    {
      Capture capture = match.Groups["t"].Captures[0];

      switch (capture.Value)
      {
        case "LINESTRING":
          shape = FixLineString(shape);
          break;
        
        case "POLYGON":
          shape = FixPolygon(shape);
          break;
      }
    }

    return shape;
  }

  private string FixLineString(string shape)
  {
    Match match = _lineStringRegex.Match(shape);

    if (match.Success)
    {
      Capture capture = match.Groups["p"].Captures[0];
      List<String> points = new List<String>(capture.Value.Split(',').Select(o => _spaceRegex.Replace(o.Trim(), " ")));

      // make sure the linestring has more than one point

      if (points.Count == 1)
      {
        AddPoint(points, 0.01, 0.01);
        shape = String.Format("LINESTRING({0})", String.Join(",", points));
      }
    }

    return shape;
  }
  
  private string FixPolygon(string shape)
  {
    Match match = _polygonRegex.Match(shape);

    if (match.Success)
    {
      Capture capture = match.Groups["p"].Captures[0];
      List<String> points = new List<String>(capture.Value.Split(',').Select(o => _spaceRegex.Replace(o.Trim(), " ")));

      // make sure the polygon has at least three points

      if (points.Count == 1)
      {
        AddPoint(points, 0.01, 0.01);
      }

      if (points.Count == 2)
      {
        AddPoint(points, 0.01, -0.01);
      }
      
      // make sure the polygon is closed

      if (points[points.Count - 1] != points[0] || points.Count == 3)
      {
        points.Add(points[0]);
        shape = String.Format("POLYGON(({0}))", String.Join(",", points));
      }
    }

    return shape;
  }

  private bool MarkupAlreadyUpdated(OleDbConnection connection)
  {
    string markupUpdated;
    string sql = String.Format("select Value from {0}Setting where Setting = 'MarkupUpdated'", WebConfigSettings.ConfigurationTablePrefix);

    using (OleDbCommand command = new OleDbCommand(sql, connection))
    {
      markupUpdated = command.ExecuteScalar() as string;
    }

    if (markupUpdated != null)
    {
      _context.Response.Write("The markup was already updated on " + markupUpdated);
    }

    return markupUpdated != null;
  }
  
  private bool ProcessMarkup(OleDbConnection connection, AppSettings settings)
  {
    bool success = true;
    string sql = String.Format("select MarkupID, Shape from {0}Markup", WebConfigSettings.ConfigurationTablePrefix);    
    DataTable table = new DataTable();

    bool shiftGeometry = !Double.IsNaN(settings.MarkupShiftX) && !Double.IsNaN(settings.MarkupShiftY);

    using (OleDbTransaction transaction = connection.BeginTransaction())
    using (OleDbCommand command = new OleDbCommand(sql, connection, transaction))
    {
      OleDbDataAdapter adapter = new OleDbDataAdapter(command);
      adapter.Fill(table);
      
      object[][] rows = table.Rows.Cast<DataRow>().Select(o => o.ItemArray).ToArray();

      CoordinateSystem mapProjection = settings.MapCoordinateSystem;
      CoordinateSystem measureProjection = settings.MeasureCoordinateSystem;
      WKTReader wktReader = new WKTReader();
      WKTWriter wktWriter = new WKTWriter();

      sql = "update {0}Markup set Shape = '{1}' where MarkupID = {2}";
      int id = 0;
      
      try
      {
        foreach (object[] row in rows)
        {
          id = (int)row[0];
          string shape = (string)row[1];

          if (FixGeometries)
          {
            shape = FixGeometry(shape);
          }

          if (ReprojectGeometries)
          {
            IGeometry geometry = wktReader.Read(shape);
            geometry = measureProjection.ToGeodetic(geometry);
            geometry = mapProjection.ToProjected(geometry);

            if (shiftGeometry)
            {
              geometry = ((Geometry)geometry).Translate(settings.MarkupShiftX, settings.MarkupShiftY);
            }
            
            shape = wktWriter.Write(geometry);
          }

          command.CommandText = String.Format(sql, WebConfigSettings.ConfigurationTablePrefix, shape, id);
          command.ExecuteNonQuery();
        }

        transaction.Commit();
      }
      catch (Exception ex)
      {
        transaction.Rollback();
        success = false;
        
        _context.Response.Write(String.Format("Error encountered while processing row {0}, markup remains unchanged.\n  {1}", id, ex.Message));
      }
    }

    return success;
  }

  private void SetMarkupUpdated(OleDbConnection connection)
  {
    string markupUpdated = DateTime.Now.ToString("MMMM d, yyyy");
    string sql = String.Format("insert into {0}Setting (Setting, Value) values ('MarkupUpdated', '{1}')", WebConfigSettings.ConfigurationTablePrefix, markupUpdated);

    using (OleDbCommand command = new OleDbCommand(sql, connection))
    {
      command.ExecuteNonQuery();
    }

    _context.Response.Write("The markup has been updated.");
  }
  
  public bool IsReusable {
    get 
    {
      return false;
    }
  }
}