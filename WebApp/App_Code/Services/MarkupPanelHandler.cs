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
using System.Data;
using System.Data.OleDb;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

public class MarkupPanelHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void AddMarkup()
  {
    int groupId = Convert.ToInt32(Request.Form["id"]);
    string shape = Request.Form["shape"];
    string text = Request.Form["text"];
    string color = Request.Form["color"];
    string glow = Request.Form["glow"];
    string measured = Request.Form["measured"];

    int id = Sequences.NextMarkupId;
    DateTime now = DateTime.Now;

    bool success = false;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("insert into {0}Markup (MarkupID, GroupID, Shape, Color, Glow, Text, Measured, DateCreated, Deleted) values (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Integer).Value = id;
          command.Parameters.Add("@2", OleDbType.Integer).Value = groupId;
          command.Parameters.Add("@3", OleDbType.VarWChar).Value = shape;
          command.Parameters.Add("@4", OleDbType.VarWChar).Value = color;
          command.Parameters.Add("@5", OleDbType.VarWChar).Value = glow == null ? (object)DBNull.Value : (object)glow;
          command.Parameters.Add("@6", OleDbType.VarWChar).Value = text == null ? (object)DBNull.Value : (object)text;
          command.Parameters.Add("@7", OleDbType.Integer).Value = measured == null ? (object)DBNull.Value : (object)1F;
          command.Parameters.Add("@8", OleDbType.Date).Value = DateTime.Now;
          command.Parameters.Add("@9", OleDbType.Integer).Value = 0;
          command.ExecuteNonQuery();
        }

        UpdateMarkupGroupLastAccessed(groupId, now, connection);

        success = true;
      }
    }
    catch { }

    ReturnJson(success);
  }

  [WebServiceMethod]
  private void CreateMarkupGroup()
  {
    string category = Request.Form["category"];
    string user = Request.Form["user"];

    int id = Sequences.NextMarkupGroupId;
    string title = "[untitled]";
    bool locked = AppAuthentication.Mode != AuthenticationMode.None;
    DateTime now = DateTime.Now;

    Dictionary<String, object> result = null;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("insert into {0}MarkupGroup (GroupID, CategoryID, DisplayName, CreatedBy, CreatedByUser, Locked, DateCreated, DateLastAccessed, Deleted) values (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Integer).Value = id;
          command.Parameters.Add("@2", OleDbType.VarWChar).Value = category;
          command.Parameters.Add("@3", OleDbType.VarWChar).Value = title;
          command.Parameters.Add("@4", OleDbType.VarWChar).Value = locked ? AppUser.GetDisplayName(connection) : user;
          command.Parameters.Add("@5", OleDbType.VarWChar).Value = locked ? (object)AppUser.Name : (object)DBNull.Value;
          command.Parameters.Add("@6", OleDbType.Integer).Value = locked ? 1 : 0;
          command.Parameters.Add("@7", OleDbType.Date).Value = now;
          command.Parameters.Add("@8", OleDbType.Date).Value = now;
          command.Parameters.Add("@9", OleDbType.Integer).Value = 0;
          command.ExecuteNonQuery();
        }
      }
      
      result = new Dictionary<String, object>();
      result.Add("id", id.ToString());
      result.Add("title", title);
      result.Add("locked", locked);
    }
    catch { }

    ReturnJson(result);
  }

  [WebServiceMethod]
  private void DeleteMarkup()
  {
    int groupId = Convert.ToInt32(Request.Form["id"]);
    double x = Convert.ToDouble(Request.Form["x"]);
    double y = Convert.ToDouble(Request.Form["y"]);
    double distance = Convert.ToDouble(Request.Form["distance"]);
    double scale = Convert.ToDouble(Request.Form["scale"]);

    IPoint p = new Point(x, y);
    bool deleted = false;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("select MarkupID, Shape, Text, Measured from {0}Markup where GroupID = ?", AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Integer).Value = groupId;
          DataTable table = new DataTable();

          using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
          {
            adapter.Fill(table);
          }

          command.Parameters.Clear();

          for (int i = table.Rows.Count - 1; i >= 0; --i)
          {
            DataRow row = table.Rows[i];
            HitType hitType = MarkupHitTest(row, p, distance, scale);

            if (hitType != HitType.None)
            {
              command.CommandText = String.Format("update {0}Markup set Deleted = 1 where MarkupID = {1}", AppSettings.ConfigurationTablePrefix, row["MarkupID"]);
              command.ExecuteNonQuery();

              UpdateMarkupGroupLastAccessed(groupId, connection);

              deleted = true;
              break;
            }
          }
        }
      }
    }
    catch { }

    ReturnJson(deleted);
  }

  [WebServiceMethod]
  private void DeleteMarkupGroup()
  {
    int groupId = Convert.ToInt32(Request.Form["id"]);
    bool deleted = false;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("update {0}Markup set Deleted = 1 where GroupID = ?", AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Integer).Value = groupId;
          command.ExecuteNonQuery();
        }
        
        sql = String.Format("update {0}MarkupGroup set Deleted = 1, DateLastAccessed = ? where GroupID = ?", AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Date).Value = DateTime.Now;
          command.Parameters.Add("@2", OleDbType.Integer).Value = groupId;
          command.ExecuteNonQuery();
        }

        deleted = true;
      }
    }
    catch { }

    ReturnJson(deleted);
  }

  [WebServiceMethod]
  private void FillWithColor()
  {
    int groupId = Convert.ToInt32(Request.Form["id"]);
    double x = Convert.ToDouble(Request.Form["x"]);
    double y = Convert.ToDouble(Request.Form["y"]);
    double distance = Convert.ToDouble(Request.Form["distance"]);
    double scale = Convert.ToDouble(Request.Form["scale"]);

    string markupColor = Request.Form["color"];
    string textGlowColor = Request.Form["glow"];

    Point p = new Point(x, y);
    bool updated = false;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("select MarkupID, Shape, Text, Measured from {0}Markup where GroupID = ?", AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Integer).Value = groupId;
          DataTable table = new DataTable();

          using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
          {
            adapter.Fill(table);
          }

          command.Parameters.Clear();

          for (int i = table.Rows.Count - 1; i >= 0; --i)
          {
            DataRow row = table.Rows[i];
            HitType hitType = MarkupHitTest(row, p, distance, scale);

            if (hitType != HitType.None)
            {
              command.CommandText = String.Format("update {0}Markup set Color = '{1}' where MarkupID = {2}", AppSettings.ConfigurationTablePrefix, markupColor, row["MarkupID"]);
              command.ExecuteNonQuery();

              if (hitType == HitType.Text || hitType == HitType.Coordinate)
              {
                string glow = textGlowColor == null ? "null" : String.Format("'{0}'", textGlowColor);
                command.CommandText = String.Format("update {0}Markup set Glow = {1} where MarkupID = {2}", AppSettings.ConfigurationTablePrefix, glow, row["MarkupID"]);
                command.ExecuteNonQuery();
              }

              UpdateMarkupGroupLastAccessed(groupId, connection);

              updated = true;
              break;
            }
          }
        }
      }
    }
    catch { }

    ReturnJson(updated);
  }

  [WebServiceMethod]
  private void GetMarkupExtent()
  {
    int[] groupIds = Request.Form["ids"].Split(',').Select(o => Convert.ToInt32(o)).ToArray();
    Envelope extent = MarkupManager.GetExtent(groupIds);
    ReturnJson<double[]>("extent", extent.IsNull ? null : extent.ToArray());
  }

  [WebServiceMethod]
  private void GetMarkupGroupData()
  {
    string category = Request.Form["category"];

    Dictionary<String, Object> result = new Dictionary<String, Object>();
    result.Add("headers", new string[] { "Created", "By", "Title" });
    
    List<Dictionary<String, Object>> rows = new List<Dictionary<String, Object>>();
    result.Add("rows", rows);
   
    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      string sql = String.Format("select GroupID, DateCreated, CreatedBy, DisplayName from {0}MarkupGroup where CategoryID = ? and DateLastAccessed >= ? and Deleted = 0 order by DateCreated desc",
          AppSettings.ConfigurationTablePrefix);

      using (OleDbCommand command = new OleDbCommand(sql, connection))
      {
        command.Parameters.Add("@1", OleDbType.VarWChar).Value = category;
        command.Parameters.Add("@2", OleDbType.Date).Value = DateTime.Now.AddDays(0 - AppSettings.MarkupTimeout);

        using (OleDbDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            Dictionary<String, Object> row = new Dictionary<String, Object>();
            row.Add("id", reader.GetInt32(0).ToString());

            List<Object> values = new List<Object>();

            for (int i = 1; i < reader.FieldCount; ++i)
            {
              values.Add(reader.IsDBNull(i) ? null : reader.GetValue(i));
            }

            row.Add("v", values);
            rows.Add(row);
          }
        }
      }
    }

    ReturnJson(result);
  }

  [WebServiceMethod]
  private void GetMarkupGroupPermissions()
  {
    string id = Request.Form["id"];

    bool canEdit = false;
    bool canLock = false;
    bool isLocked = false;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      string sql = String.Format("select CreatedByUser, Locked from {0}MarkupGroup where GroupID = ?", AppSettings.ConfigurationTablePrefix);

      using (OleDbCommand command = new OleDbCommand(sql, connection))
      {
        command.Parameters.Add("@1", OleDbType.VarWChar).Value = id;

        using (OleDbDataReader reader = command.ExecuteReader())
        {
          if (reader.Read())
          {
            canLock = AppUser.IsInRole("admin", connection) || (!reader.IsDBNull(0) && reader.GetString(0) == AppUser.Name);
            isLocked = Convert.ToInt32(reader.GetValue(1)) == 1;
            canEdit = canLock || !isLocked;
          }
        }
      }
    }

    Dictionary<String, bool> result = new Dictionary<string, bool>();
    result.Add("canEdit", canEdit);
    result.Add("canLock", canLock);
    result.Add("isLocked", isLocked);
    ReturnJson(result);
  }

  private HitType MarkupHitTest(DataRow markupRow, IPoint p, double distance, double scale)
  {
    WKTReader wktReader = new WKTReader();
    IGeometry geometry = wktReader.Read(markupRow["Shape"].ToString());

    HitType hitType = HitType.None;

    if (geometry.OgcGeometryType != OgcGeometryType.Point || markupRow.IsNull("Text"))
    {
      if (geometry.Distance(p) <= distance * scale)
      {
        hitType = geometry.OgcGeometryType == OgcGeometryType.Point && !markupRow.IsNull("Measured") ? HitType.Coordinate : HitType.Shape;
      }
    }
    else
    {
      System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new System.Drawing.Bitmap(10, 10));

      Coordinate origin = ((IPoint)geometry).Coordinate;
      System.Drawing.SizeF textSize = graphics.MeasureString(markupRow["Text"].ToString(), AppSettings.MarkupFont);

      Envelope box = new Envelope(new Coordinate(origin.X, origin.Y), new Coordinate(origin.X + textSize.Width * scale, origin.Y + textSize.Height * scale));

      if (box.Contains(p.Coordinate))
      {
        hitType = HitType.Text;
      }
    }

    return hitType;
  }

  [WebServiceMethod]
  private void LockMarkupGroup()
  {
    int groupId = Convert.ToInt32(Request.Form["id"]);
    bool locked = Convert.ToBoolean(Request.Form["locked"]);
    bool success = false;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("update {0}MarkupGroup set Locked = ? where GroupID = ?", AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Integer).Value = locked ? 1 : 0;
          command.Parameters.Add("@2", OleDbType.Integer).Value = groupId;
          command.ExecuteNonQuery();
        }
      }

      success = true;
    }
    catch { }

    ReturnJson(success);
  }

  [WebServiceMethod]
  private void PickColors()
  {
    int groupId = Convert.ToInt32(Request.Form["id"]);
    double x = Convert.ToDouble(Request.Form["x"]);
    double y = Convert.ToDouble(Request.Form["y"]);
    double distance = Convert.ToDouble(Request.Form["distance"]);
    double scale = Convert.ToDouble(Request.Form["scale"]);

    Point p = new Point(x, y);
    Dictionary<String, object> result = null;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("select Shape, Color, Glow, Text, Measured from {0}Markup where GroupID = ?", AppSettings.ConfigurationTablePrefix);
        DataTable table = new DataTable();

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Integer).Value = groupId;

          using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
          {
            adapter.Fill(table);
          }
        }

        for (int i = table.Rows.Count - 1; i >= 0; --i)
        {
          DataRow row = table.Rows[i];
          HitType hitType = MarkupHitTest(row, p, distance, scale);

          if (hitType != HitType.None)
          {
            result = new Dictionary<String, object>();
            result.Add("color", row["Color"]);

            if (hitType == HitType.Text)
            {
              result.Add("glow", row.IsNull("Glow") ? null : row["Glow"]);
            }

            UpdateMarkupGroupLastAccessed(groupId, connection);
            break;
          }
        }
      }
    }
    catch { }

    ReturnJson(result);
  }

  private void UpdateMarkupGroupLastAccessed(int groupId, OleDbConnection connection)
  {
    UpdateMarkupGroupLastAccessed(groupId, DateTime.Now, connection);
  }

  private void UpdateMarkupGroupLastAccessed(int groupId, DateTime date, OleDbConnection connection)
  {
    string sql = String.Format("update {0}MarkupGroup set DateLastAccessed = ? where GroupID = {1}", AppSettings.ConfigurationTablePrefix, groupId);

    using (OleDbCommand command = new OleDbCommand(sql, connection))
    {
      command.Parameters.Add("@1", OleDbType.Date).Value = date;
      command.ExecuteNonQuery();
    }
  }

  [WebServiceMethod]
  private void UpdateMarkupGroupTitle()
  {
    int groupId = Convert.ToInt32(Request.Form["id"]);
    string title = Request.Form["title"];

    DateTime now = DateTime.Now;
    bool success = false;

    try
    {
      using (OleDbConnection connection = AppContext.GetDatabaseConnection())
      {
        string sql = String.Format("update {0}MarkupGroup set DisplayName = ?, DateLastAccessed = ? where GroupID = ?", AppSettings.ConfigurationTablePrefix);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.VarWChar).Value = title;
          command.Parameters.Add("@2", OleDbType.Date).Value = now;
          command.Parameters.Add("@3", OleDbType.Integer).Value = Convert.ToInt32(groupId);

          command.ExecuteNonQuery();
        }
      }

      success = true;
    }
    catch { }

    ReturnJson(success);
  }

  private enum HitType
  {
    None,
    Coordinate,
    Shape,
    Text
  }
}