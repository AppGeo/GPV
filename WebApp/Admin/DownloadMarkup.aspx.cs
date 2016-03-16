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
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using GeoAPI.Geometries;
using ICSharpCode.SharpZipLib.Zip;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

public partial class Admin_DownloadMarkup : CustomStyledPage
{
  private Configuration _config;

  private void Page_Init(object sender, EventArgs e)
  {
    _config = AppContext.GetConfiguration();
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    Response.Cache.SetCacheability(HttpCacheability.NoCache);

    if (!IsPostBack)
    {
      ddlMarkupCategory.DataSource = _config.MarkupCategory;
      ddlMarkupCategory.DataValueField = "CategoryID";
      ddlMarkupCategory.DataTextField = "DisplayName";
      ddlMarkupCategory.DataBind();
      ddlMarkupCategory.Items.Insert(0, new ListItem("- all categories -", ""));

      calDateFrom.SelectedDate = DateTime.Now.AddMonths(-1).Date;
      calDateFrom.VisibleDate = calDateFrom.SelectedDate;

      calDateTo.SelectedDate = DateTime.Now.Date;
      calDateTo.VisibleDate = calDateTo.SelectedDate;

      tboFilePrefix.Text = GetDefaultFilePrefix();
    }

    //AppContext.AddCustomStyleSheet(head);
  }

  protected void cmdDownload_Click(object sender, EventArgs e)
  {
    StringCollection query = new StringCollection();
    query.Add("a.Deleted = 0 and b.Deleted = 0");

    if (ddlMarkupCategory.SelectedValue.Length > 0)
    {
      query.Add(String.Format("b.CategoryID = '{0}'", ddlMarkupCategory.SelectedValue));
    }

    if (ddlUserName.SelectedValue.Length > 0)
    {
      query.Add(String.Format("b.CreatedBy = '{0}'", ddlUserName.SelectedValue));
    }

    if (optDateRange.Checked)
    {
      DateTime dateFrom = calDateFrom.SelectedDate < calDateTo.SelectedDate ? calDateFrom.SelectedDate : calDateTo.SelectedDate;
      DateTime dateTo = calDateFrom.SelectedDate >= calDateTo.SelectedDate ? calDateFrom.SelectedDate : calDateTo.SelectedDate;
      query.Add(String.Format("'{0}' <= b.DateCreated and b.DateCreated < '{1}'", dateFrom.ToString("MM/dd/yy"), dateTo.AddDays(1).ToString("MM/dd/yy")));
    }

    if (ddlMarkupGroup.SelectedValue.Length > 0)
    {
      query.Add(String.Format("b.DisplayName = '{0}'", ddlMarkupGroup.SelectedValue));
    }

    string baseSql = @"select c.DisplayName as Category, b.DisplayName as [Group], b.CreatedBy as CreatedBy,
          b.DateCreated as DateGroup, a.DateCreated as DateMarkup, a.Color as Color, a.Glow as Glow,
          a.Text as Text, a.Shape as Shape
          from {0}Markup a 
            inner join {0}MarkupGroup b on a.GroupID = b.GroupID
            inner join {0}MarkupCategory c on b.CategoryID = c.CategoryID {1}
          order by b.DateCreated, a.DateCreated
        ";

    Response.Clear();
    Response.ContentType = "application/zip";
    Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}.zip", tboFilePrefix.Text));

    ZipOutputStream zipStream = new ZipOutputStream(Response.OutputStream);

    // polygons

    string where = String.Format(" where {0} and a.Shape like 'POLYGON%'", query.Join(" and "));
    string sql = String.Format(baseSql, WebConfigSettings.ConfigurationTablePrefix, where);
    DataTable table = GetShapeTable(sql, OgcGeometryType.Polygon);

    if (table.Rows.Count > 0)
    {
      table.Columns.Remove("Glow");
      table.Columns.Remove("Text");
      WriteZippedShapefile(zipStream, table, tboFilePrefix.Text + "_poly");
    }

    // lines

    where = String.Format(" where {0} and a.Shape like 'LINESTRING%'", query.Join(" and "));
    sql = String.Format(baseSql, WebConfigSettings.ConfigurationTablePrefix, where);
    table = GetShapeTable(sql, OgcGeometryType.LineString);

    if (table.Rows.Count > 0)
    {
      table.Columns.Remove("Glow");
      table.Columns.Remove("Text");
      WriteZippedShapefile(zipStream, table, tboFilePrefix.Text + "_line");
    }

    // points

    where = String.Format(" where {0} and a.Shape like 'POINT%' and a.Text is null", query.Join(" and "));
    sql = String.Format(baseSql, WebConfigSettings.ConfigurationTablePrefix, where);
    table = GetShapeTable(sql, OgcGeometryType.Point);

    if (table.Rows.Count > 0)
    {
      table.Columns.Remove("Glow");
      table.Columns.Remove("Text");
      WriteZippedShapefile(zipStream, table, tboFilePrefix.Text + "_point");
    }

    // text

    where = String.Format(" where {0} and a.Shape like 'POINT%' and a.Text is not null", query.Join(" and "));
    sql = String.Format(baseSql, WebConfigSettings.ConfigurationTablePrefix, where);
    table = GetShapeTable(sql, OgcGeometryType.Point);

    if (table.Rows.Count > 0)
    {
      WriteZippedShapefile(zipStream, table, tboFilePrefix.Text + "_text");
    }

    zipStream.Finish();
    zipStream.Close();
    Response.End();
  }

  protected void optDateAll_CheckedChanged(object sender, EventArgs e)
  {
    labDateFrom.Enabled = false;
    labDateTo.Enabled = false;
    calDateFrom.Enabled = false;
    calDateTo.Enabled = false;
  }

  protected void optDateRange_CheckedChanged(object sender, EventArgs e)
  {
    labDateFrom.Enabled = true;
    labDateTo.Enabled = true;
    calDateFrom.Enabled = true;
    calDateTo.Enabled = true;
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    string prefix = WebConfigSettings.ConfigurationTablePrefix;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      StringCollection query = new StringCollection();
      query.Add("Deleted = 0");

      // category

      if (ddlMarkupCategory.SelectedValue.Length > 0)
      {
        query.Add(String.Format("CategoryID = '{0}'", ddlMarkupCategory.SelectedValue));
      }

      // user name

      string sql = String.Format("select distinct CreatedBy from {0}MarkupGroup {1} order by CreatedBy",
          prefix, String.Format(" where {0}", query.Join(" and ")));
      DataTable table = new DataTable();
      
      using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
      {
        adapter.Fill(table);
      }

      string currentUser = ddlUserName.Items.Count == 0 ? "" : ddlUserName.SelectedValue;

      ddlUserName.DataSource = table;
      ddlUserName.DataValueField = "CreatedBy";
      ddlUserName.DataBind();
      ddlUserName.Items.Insert(0, new ListItem("- all users -", ""));

      if (table.Select("CreatedBy = '" + currentUser + "'").Length > 0)
      {
        ddlUserName.SelectedValue = currentUser;
        query.Add(String.Format("CreatedBy = '{0}'", currentUser));
      }

      // date

      if (optDateRange.Checked)
      {
        DateTime dateFrom = calDateFrom.SelectedDate < calDateTo.SelectedDate ? calDateFrom.SelectedDate : calDateTo.SelectedDate;
        DateTime dateTo = calDateFrom.SelectedDate >= calDateTo.SelectedDate ? calDateFrom.SelectedDate : calDateTo.SelectedDate;
        query.Add(String.Format("'{0}' <= DateCreated and DateCreated < '{1}'", dateFrom.ToString("MM/dd/yy"), dateTo.AddDays(1).ToString("MM/dd/yy")));
      }

      // markup group

      sql = String.Format("select distinct DisplayName from {0}MarkupGroup {1} order by DisplayName",
          prefix, String.Format(" where {0}", query.Join(" and ")));
      table = new DataTable();

      using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
      {
        adapter.Fill(table);
      }

      string currentGroup = ddlMarkupGroup.Items.Count == 0 ? "" : ddlMarkupGroup.SelectedValue;

      ddlMarkupGroup.DataSource = table;
      ddlMarkupGroup.DataValueField = "DisplayName";
      ddlMarkupGroup.DataBind();
      ddlMarkupGroup.Items.Insert(0, new ListItem("- all groups -", ""));

      if (table.Select("DisplayName = '" + currentGroup + "'").Length > 0)
      {
        ddlMarkupGroup.SelectedValue = currentGroup;
        query.Add(String.Format("DisplayName = '{0}'", currentGroup));
      }

      // count

      string where = String.Format(" where {0}", query.Join(" and "));
      sql = String.Format("select count(*) from {0}MarkupGroup {1}", prefix, where);
      int numGroups;

      using (OleDbCommand command = new OleDbCommand(sql, connection))
      {
        numGroups = Convert.ToInt32(command.ExecuteScalar());
      }

      sql = String.Format("select count(*) from {0}Markup where GroupID in (select GroupID from {0}MarkupGroup {1})", prefix, where);
      int numMarkups;

      using (OleDbCommand command = new OleDbCommand(sql, connection))
      {
        numMarkups = Convert.ToInt32(command.ExecuteScalar());
      }

      labNumberFound.Text = String.Format("{0} {1} in {2} {3} found", numMarkups,
          numMarkups == 1 ? "markup" : "markups", numGroups, numGroups == 1 ? "group" : "groups");

      cmdDownload.Enabled = numGroups != 0 && numMarkups != 0;
    }

    if (String.IsNullOrEmpty(tboFilePrefix.Text))
    {
      tboFilePrefix.Text = GetDefaultFilePrefix();
    }
    else
    {
      char[] c = tboFilePrefix.Text.ToCharArray();

      for (int i = 0; i < c.Length; ++i)
      {
        if (!('a' <= c[i] && c[i] <= 'z') && !('A' <= c[i] && c[i] <= 'Z') && !('0' <= c[i] && c[i] <= '9'))
        {
          c[i] = '_';
        }
      }

      tboFilePrefix.Text = new String(c);
    }
  }

  private string GetDefaultFilePrefix()
  {
    return "markup_" + DateTime.Now.ToString("yyMMdd_hhmm");
  }

  private DataTable GetShapeTable(string sql, OgcGeometryType geometryType)
  {
    DataTable table = null;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      table = new DataTable();

      using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
      {
        adapter.Fill(table);
      }

      table.Columns["Shape"].ColumnName = "ShapeString";

      switch (geometryType)
      {
        case OgcGeometryType.Point: table.Columns.Add("Shape", typeof(IPoint)); break;
        case OgcGeometryType.LineString: table.Columns.Add("Shape", typeof(IMultiLineString)); break;
        case OgcGeometryType.Polygon: table.Columns.Add("Shape", typeof(IMultiPolygon)); break;
      }

      WKTReader wktReader = new WKTReader();

      foreach (DataRow row in table.Rows)
      {
        switch (geometryType)
        {
          case OgcGeometryType.Point:
            row["Shape"] = (IPoint)wktReader.Read((string)row["ShapeString"]);
            break;

          case OgcGeometryType.LineString:
            ILineString lineString = (ILineString)wktReader.Read((string)row["ShapeString"]);
            IMultiLineString multiLineString = new MultiLineString(new ILineString[] { lineString });
            row["Shape"] = multiLineString;
            break;

          case OgcGeometryType.Polygon:
            IPolygon polygon = (IPolygon)wktReader.Read((string)row["ShapeString"]);
            IMultiPolygon multiPolygon = new MultiPolygon(new IPolygon[] { polygon });
            row["Shape"] = multiPolygon;
            break;
        }
      }

      table.Columns.Remove("ShapeString");
    }

    return table;
  }

  private void WriteZippedShapefile(ZipOutputStream zipStream, DataTable table, string baseName)
  {
    ShapeFileWriter writer = new ShapeFileWriter(table);

    MemoryStream memStream = new MemoryStream();
    writer.WriteShp(memStream);
    byte[] buffer = memStream.ToArray();

    ZipEntry entry = new ZipEntry(baseName + ".shp");
    entry.Size = buffer.Length;
    zipStream.PutNextEntry(entry);
    zipStream.Write(buffer, 0, buffer.Length);

    memStream = new MemoryStream();
    writer.WriteShx(memStream);
    buffer = memStream.ToArray();

    entry = new ZipEntry(baseName + ".shx");
    entry.Size = buffer.Length;
    zipStream.PutNextEntry(entry);
    zipStream.Write(buffer, 0, buffer.Length);

    memStream = new MemoryStream();
    writer.WriteDbf(memStream);
    buffer = memStream.ToArray();

    entry = new ZipEntry(baseName + ".dbf");
    entry.Size = buffer.Length;
    zipStream.PutNextEntry(entry);
    zipStream.Write(buffer, 0, buffer.Length);
  }
}
