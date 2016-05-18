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
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using GeoAPI.Geometries;
using NetTopologySuite.IO;

public class UpdateMarkup : IHttpHandler 
{
  public void ProcessRequest (HttpContext context) 
  {
    context.Response.ContentType = "text/plain";
    AppSettings settings = Configuration.GetCurrent().AppSettings;
    
    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      if (!MarkupAlreadyUpdated(connection, context) && CanPerformProjection(settings))
      {
        ProcessMarkup(connection, settings);
        SetMarkupUpdated(connection, context);
      }
    }
  }

  private bool MarkupAlreadyUpdated(OleDbConnection connection, HttpContext context)
  {
    string markupUpdated;
    string sql = String.Format("select Value from {0}Setting where Setting = 'MarkupUpdated'", WebConfigSettings.ConfigurationTablePrefix);

    using (OleDbCommand command = new OleDbCommand(sql, connection))
    {
      markupUpdated = command.ExecuteScalar() as string;
    }

    if (markupUpdated != null)
    {
      context.Response.Write("The markup was already updated on " + markupUpdated);
    }

    return markupUpdated != null;
  }

  private bool CanPerformProjection(AppSettings settings)
  {
    return !settings.MapCoordinateSystem.Equals(settings.MeasureCoordinateSystem);
  }
  
  private void ProcessMarkup(OleDbConnection connection, AppSettings settings)
  {
    string sql = String.Format("select MarkupID, Shape from {0}Markup", WebConfigSettings.ConfigurationTablePrefix);    
    DataTable table = new DataTable();

    using (OleDbCommand command = new OleDbCommand(sql, connection))
    {
      OleDbDataAdapter adapter = new OleDbDataAdapter(command);
      adapter.Fill(table);
      
      object[][] rows = table.Rows.Cast<DataRow>().Select(o => o.ItemArray).ToArray();

      CoordinateSystem mapProjection = settings.MapCoordinateSystem;
      CoordinateSystem measureProjection = settings.MeasureCoordinateSystem;
      WKTReader wktReader = new WKTReader();
      WKTWriter wktWriter = new WKTWriter();

      sql = "update {0}Markup set Shape = '{1}' where MarkupID = {2}";

      foreach (object[] row in rows)
      {
        int id = (int)row[0];
        string shape = (string)row[1];

        IGeometry geometry = wktReader.Read(shape);
        geometry = measureProjection.ToGeodetic(geometry);
        geometry = mapProjection.ToProjected(geometry);
        shape = wktWriter.Write(geometry);

        command.CommandText = String.Format(sql, WebConfigSettings.ConfigurationTablePrefix, shape, id);
        command.ExecuteNonQuery();
      }
    }
  }

  private void SetMarkupUpdated(OleDbConnection connection, HttpContext context)
  {
    string markupUpdated = DateTime.Now.ToString("MMMM d, yyyy");
    string sql = String.Format("insert into {0}Setting (Setting, Value) values ('MarkupUpdated', '{1}')", WebConfigSettings.ConfigurationTablePrefix, markupUpdated);

    using (OleDbCommand command = new OleDbCommand(sql, connection))
    {
      command.ExecuteNonQuery();
    }

    context.Response.Write("The markup has been updated.");
  }
  
  public bool IsReusable {
    get 
    {
      return false;
    }
  }
}