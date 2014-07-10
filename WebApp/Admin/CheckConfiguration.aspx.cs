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
using System.Drawing;
using System.Linq;
using System.Web.UI.HtmlControls;

public partial class Admin_CheckConfiguration : CustomStyledPage
{
  private const int ColumnCount = 5;

  protected void cmdRecheck_Click(object sender, EventArgs e)
  {
    ViewState["Step"] = 0;
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    if (ViewState["Step"] == null || (int)ViewState["Step"] == 0)
    {
      labMessage.InnerText = "Analyzing configuration, please wait ...";
      cmdRecheck.Visible = false;
      tblReport.Visible = false;

      string script = "$('body').find('*').css('cursor', 'wait'); document.forms[0].submit();";
      ClientScript.RegisterStartupScript(typeof(Admin_CheckConfiguration), "start", script, true);

      ViewState["Step"] = 1;
    }
    else
    {
      cmdRecheck.Visible = true;
      tblReport.Visible = true;

      tblReport.Rows.Clear();

      LayoutColumns();

      int errorCount = WriteWebConfigBlock();

      Configuration config = Configuration.GetCurrent();
      config.CascadeDeactivated();
      config.RemoveDeactivated();
      config.ValidateConfiguration();

      errorCount += WriteReportBlock(config.Application, "ApplicationID", null);
      errorCount += WriteReportBlock(config.ApplicationMapTab, "ApplicationID", "MapTabID");
      errorCount += WriteReportBlock(config.ApplicationMarkupCategory, "ApplicationID", "CategoryID");
      errorCount += WriteReportBlock(config.ApplicationPrintTemplate, "ApplicationID", "TemplateID");
      errorCount += WriteReportBlock(config.Connection, "ConnectionID", null);
      errorCount += WriteReportBlock(config.DataTab, "DataTabID", "LayerID");
      errorCount += WriteReportBlock(config.Layer, "LayerID", null);
      errorCount += WriteReportBlock(config.LayerFunction, "LayerID", "Function");
      errorCount += WriteReportBlock(config.LayerProximity, "LayerID", "ProximityID");
      errorCount += WriteReportBlock(config.Level, "LevelID", "ZoneLevelID");
      errorCount += WriteReportBlock(config.MapTab, "MapTabID", null);
      errorCount += WriteReportBlock(config.MapTabLayer, "MapTabID", "LayerID");
      errorCount += WriteReportBlock(config.MarkupCategory, "CategoryID", null);
      errorCount += WriteReportBlock(config.PrintTemplate, "TemplateID", null);
      errorCount += WriteReportBlock(config.PrintTemplateContent, "TemplateID", "SequenceNo");
      errorCount += WriteReportBlock(config.Proximity, "ProximityID", null);
      errorCount += WriteReportBlock(config.Query, "QueryID", "LayerID");
      errorCount += WriteReportBlock(config.Search, "SearchID", "LayerID");
      errorCount += WriteReportBlock(config.SearchCriteria, "SearchCriteriaID", "SearchID");
      errorCount += WriteReportBlock(config.Zone, "ZoneID", "ZoneLevelID");
      errorCount += WriteReportBlock(config.ZoneLevel, "ZoneLevelID", null);
      errorCount += WriteReportBlock(config.ZoneLevelCombo, "ZoneID,LevelID", "ZoneLevelID");

      labMessage.InnerText = errorCount == 0 ? "No errors found" : errorCount == 1 ? "1 error found" : String.Format("{0} errors found", errorCount);

      IAdminMasterPage master = (IAdminMasterPage)Master;

      if (master.ReloadRequested)
      {
        config.RemoveValidationErrors();
        master.ReloadConfiguration(config);
      }
    }
  }

  private string CheckColor(Color color)
  {
    return color.IsEmpty ? "Not set or not a valid HTML color specification" : null;
  }

  private string CheckGreaterThanZero(int value)
  {
    return value == Int32.MinValue ? "Not set" : value <= 0 ? "Must be greater than 0" : null;
  }

  private string CheckGreaterThanZero(double value)
  {
    return Double.IsNaN(value) ? "Not set" : value <= 0 ? "Must be greater than 0.0" : null;
  }

  private string CheckInRange(double value, double minValue, double maxValue)
  {
    return Double.IsNaN(value) ? "Not set" : value < minValue || maxValue < value ? String.Format("Must be between {0:0.0} and {1:0.0}", minValue, maxValue) : null;
  }

  private string CheckInValues(string value, List<String> validValues)
  {
    if (String.IsNullOrEmpty(value))
    {
      return "Not set";
    }

    if (!validValues.Contains(value))
    {
      validValues = validValues.Select(o => String.Format("\"{0}\"", o)).ToList();
      return "Must be " + String.Join(", ", validValues.Take(validValues.Count - 1).ToArray()) + " or " + validValues[validValues.Count - 1];
    }

    return null;
  }

  private string CheckIsSet(int value)
  {
    return value == Int32.MinValue ? "Not set" : null;
  }

  private string CheckIsSet(double value)
  {
    return Double.IsNaN(value) ? "Not set" : null;
  }

  private void LayoutColumns()
  {
    HtmlTableRow tr = new HtmlTableRow();
    tr.Attributes["class"] = "ReportLayout";
    tblReport.Rows.Add(tr);

    HtmlTableCell td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Width = "20px";
    td.Height = "0px";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Width = "120px";
    td.Height = "0px";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Width = "120px";
    td.Height = "0px";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Width = "50px";
    td.Height = "0px";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.Width = "500px";
    td.Height = "0px";
  }

  private int WriteWebConfigBlock()
  {
    List<String> name = new List<String>();
    List<String> message = new List<String>();

    // check FullExtent and ZoomLevel

    name.Add("FullExtent");
    message.Add(AppSettings.DefaultFullExtent == null ? "Not set or incorrectly specified, must be four comma-separated numbers" : null);

    name.Add("ZoomLevels");
    message.Add(CheckIsSet(AppSettings.ZoomLevels));

    // check MapUnits and MeasureUnits

    name.Add("MapUnits");
    List<String> values = new List<String>(new string[] { "feet", "meters" });
    message.Add(CheckInValues(AppSettings.MapUnits, values));

    name.Add("MeasureUnits");
    values.Add("both");
    message.Add(CheckInValues(AppSettings.MeasureUnits, values));

    // check coordinate system

    string projectionName = AppSettings.GetConfigSetting("Projection");
    name.Add("Projection");
    values = new List<String>(new string[] { "LambertConformalConic", "TransverseMercator" });
    message.Add(CheckInValues(projectionName, values));

    name.Add("Spheroid");
    message.Add(typeof(Spheroid).GetProperty(AppSettings.GetConfigSetting("Spheroid")) == null ? "Not set or not a known spheroid identifier" : null);

    name.Add("CentralMeridian");
    message.Add(CheckInRange(AppSettings.GetConfigDouble("CentralMeridian"), -180, 180));

    name.Add("OriginLatitude");
    message.Add(CheckInRange(AppSettings.GetConfigDouble("OriginLatitude"), -90, 90));

    switch (projectionName)
    {
      case "LambertConformalConic":
        name.Add("StandardParallel1");
        message.Add(CheckInRange(AppSettings.GetConfigDouble("StandardParallel1"), -90, 90));

        name.Add("StandardParallel2");
        message.Add(CheckInRange(AppSettings.GetConfigDouble("StandardParallel2"), -90, 90));
        break;

      case "TransverseMercator":
        name.Add("ScaleFactor");
        message.Add(CheckGreaterThanZero(AppSettings.GetConfigDouble("ScaleFactor")));
        break;
    }

    name.Add("FalseEasting");
    message.Add(CheckIsSet(AppSettings.GetConfigDouble("FalseEasting")));

    name.Add("FalseNorthing");
    message.Add(CheckIsSet(AppSettings.GetConfigDouble("FalseNorthing")));

    // check datum shift

    name.Add("DatumShiftX");
    message.Add(CheckIsSet(AppSettings.GetConfigDouble("DatumShiftX")));

    name.Add("DatumShiftY");
    message.Add(CheckIsSet(AppSettings.GetConfigDouble("DatumShiftY")));

    // check highlight colors, opacities and sizes
    
    values = new List<String>(new string[] { "fill", "outline" });

    foreach (string highlightType in new string[] { "Active", "Target", "Selection", "Filtered" })
    {
      name.Add(highlightType + "Color");
      message.Add(CheckColor(AppSettings.GetConfigColor(highlightType + "Color")));

      name.Add(highlightType + "Opacity");
      message.Add(CheckInRange(AppSettings.GetConfigDouble(highlightType + "Opacity"), 0, 1));

      name.Add(highlightType + "PolygonMode");
      message.Add(CheckInValues(AppSettings.GetConfigSetting(highlightType + "PolygonMode"), values));

      name.Add(highlightType + "PenWidth");
      message.Add(CheckGreaterThanZero(AppSettings.GetConfigInteger(highlightType + "PenWidth")));

      name.Add(highlightType + "DotSize");
      message.Add(CheckGreaterThanZero(AppSettings.GetConfigInteger(highlightType + "DotSize")));
    }

    name.Add("BufferColor");
    message.Add(CheckColor(AppSettings.BufferColor));

    name.Add("BufferOpacity");
    message.Add(CheckInRange(AppSettings.BufferOpacity, 0, 1));

    name.Add("BufferOutlineColor");
    message.Add(CheckColor(AppSettings.BufferOutlineColor));

    name.Add("BufferOutlineOpacity");
    message.Add(CheckInRange(AppSettings.BufferOutlineOpacity, 0, 1));

    name.Add("BufferOutlinePenWidth");
    message.Add(CheckIsSet(AppSettings.BufferOutlinePenWidth));

    name.Add("SwatchTileWidth");
    message.Add(CheckGreaterThanZero(AppSettings.SwatchTileWidth));

    name.Add("SwatchTileHeight");
    message.Add(CheckGreaterThanZero(AppSettings.SwatchTileHeight));

    name.Add("PreserveOnActionChange");
    values = new List<String>(new string[] { "target", "selection" });
    message.Add(CheckInValues(AppSettings.PreserveOnActionChange, values));

    name.Add("ExportFormat");
    values = new List<String>(new string[] { "csv", "xls" });
    message.Add(CheckInValues(AppSettings.ExportFormat, values));

    name.Add("IdentifyPopup");
    values = new List<String>(new string[] { "single", "multiple" });
    message.Add(CheckInValues(AppSettings.IdentifyPopup, values));

    name.Add("IdentifyWindowWidth");
    message.Add(CheckGreaterThanZero(AppSettings.IdentifyWindowWidth));

    name.Add("IdentifyWindowHeight");
    message.Add(CheckGreaterThanZero(AppSettings.IdentifyWindowHeight));

    name.Add("MarkupTimeout");
    message.Add(CheckGreaterThanZero(AppSettings.MarkupTimeout));

    int errorCount = message.Where(o => o != null).Count();
    
    // write header

    HtmlTableRow tr = new HtmlTableRow();
    tblReport.Rows.Add(tr);
    tr.Attributes["class"] = errorCount == 0 ? "ReportHeader Closed" : "ReportHeader Opened";

    HtmlTableCell td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = 3;
    td.Attributes["unselectable"] = "on";
    td.InnerText = "Web.config";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = 2;
    td.Attributes["unselectable"] = "on";

    if (errorCount > 0)
    {
      td.InnerText = String.Format("{0} {1}", errorCount, errorCount == 1 ? "error" : "errors");
    }
    // report on settings

    for (int i = 0; i < name.Count; ++i)
    {
      tr = new HtmlTableRow();
      tblReport.Rows.Add(tr);
      tr.Attributes["class"] = "ReportRow";

      if (errorCount == 0)
      {
        tr.Style["display"] = "none";
      }

      td = new HtmlTableCell();
      tr.Cells.Add(td);

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";
      td.InnerText = name[i];

      td = new HtmlTableCell();
      tr.Cells.Add(td);

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";
      td.InnerText = String.IsNullOrEmpty(message[i]) ? "OK" : "invalid";

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";

      if (!String.IsNullOrEmpty(message[i]))
      {
        td.InnerText = message[i];
      }
    }

    tr = new HtmlTableRow();
    tblReport.Rows.Add(tr);
    tr.Attributes["class"] = "ReportSpacer";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = ColumnCount;
    td.Height = "6px";

    return errorCount;
  }

  private int WriteReportBlock(DataTable table, string idColumns, string linkColumn)
  {
    string sortOrder = idColumns;

    if (linkColumn != null)
    {
      sortOrder += ", " + linkColumn;
    }

    DataRow[] row = table.Select("", sortOrder);
    int errorCount = row.Count(o => !o.IsNull("ValidationError"));

    HtmlTableRow tr = new HtmlTableRow();
    tblReport.Rows.Add(tr);
    tr.Attributes["class"] = errorCount == 0 ? "ReportHeader Closed" : "ReportHeader Opened";

    HtmlTableCell td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = 3;
    td.InnerText = AppSettings.ConfigurationTablePrefix + table.TableName;
    
    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = 2;

    if (row.Length == 0)
    {
      td.InnerText = "(no rows)";
    }
    else if (errorCount > 0)
    {
      td.InnerText += String.Format("{0} {1}", errorCount, errorCount == 1 ? "error" : "errors");
    }

    for (int i = 0; i < row.Length; ++i)
    {
      tr = new HtmlTableRow();
      tblReport.Rows.Add(tr);
      tr.Attributes["class"] = "ReportRow";

      if (errorCount == 0)
      {
        tr.Style["display"] = "none";
      }

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.Align = "right";
      td.VAlign = "top";
      td.InnerText = (i + 1).ToString();

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";

      string[] idColumn = idColumns.Split(',');
      td.InnerText = row[i][idColumn[0]].ToString();

      for (int j = 1; j < idColumn.Length; ++j)
      {
        td.InnerText += ", " + row[i][idColumn[j]].ToString();
      }

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";

      if (linkColumn != null)
      {
        td.InnerText = row[i][linkColumn].ToString();
      }

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";
      td.InnerText = row[i].IsNull("ValidationError") ? "OK" : "invalid";

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";

      if (!row[i].IsNull("ValidationError"))
      {
        td.InnerText = row[i]["ValidationError"].ToString();
      }
    }

    tr = new HtmlTableRow();
    tblReport.Rows.Add(tr);
    tr.Attributes["class"] = "ReportSpacer";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = ColumnCount;
    td.Height = "6px";

    return errorCount;
  }
}
