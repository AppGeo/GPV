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
using System.Web.UI.HtmlControls;

public partial class Admin_ShowDeactivated : CustomStyledPage
{
  private const int ColumnCount = 5;
  
  protected void Page_Init(object sender, EventArgs e)
  {
    tblReport.EnableViewState = false;
  }

  protected void cmdReload_Click(object sender, EventArgs e)
  {
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    LayoutColumns();

    Configuration config = Configuration.GetCurrent();
    config.CascadeDeactivated();

    WriteReportBlock(config.Application, "ApplicationID", null);
    WriteReportBlock(config.ApplicationMapTab, "ApplicationID", "MapTabID");
    WriteReportBlock(config.ApplicationMarkupCategory, "ApplicationID", "CategoryID");
    WriteReportBlock(config.ApplicationPrintTemplate, "ApplicationID", "TemplateID");
    WriteReportBlock(config.Connection, "ConnectionID", null);
    WriteReportBlock(config.DataTab, "DataTabID", "LayerID");
    WriteReportBlock(config.Layer, "LayerID", null);
    WriteReportBlock(config.LayerFunction, "LayerID", "FunctionName");
    WriteReportBlock(config.LayerProximity, "LayerID", "ProximityID");
    WriteReportBlock(config.Level, "LevelID", "ZoneLevelID");
    WriteReportBlock(config.MapTab, "MapTabID", null);
    WriteReportBlock(config.MapTabLayer, "MapTabID", "LayerID");
    WriteReportBlock(config.MarkupCategory, "CategoryID", null);
    WriteReportBlock(config.PrintTemplate, "TemplateID", null);
    WriteReportBlock(config.PrintTemplateContent, "TemplateID", "SequenceNo");
    WriteReportBlock(config.Proximity, "ProximityID", null);
    WriteReportBlock(config.Query, "QueryID", "LayerID");
    WriteReportBlock(config.Search, "SearchID", "LayerID");
    WriteReportBlock(config.SearchInputField, "FieldID", "SearchID");
    WriteReportBlock(config.Zone, "ZoneID", "ZoneLevelID");
    WriteReportBlock(config.ZoneLevel, "ZoneLevelID", null);
    WriteReportBlock(config.ZoneLevelCombo, "ZoneID,LevelID", "ZoneLevelID");
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
  private void WriteReportBlock(DataTable table, string idColumns, string linkColumn)
  {
    string sortOrder = idColumns;

    if (linkColumn != null)
    {
      sortOrder += ", " + linkColumn;
    }

    DataRow[] row = table.Select("Active = 0", sortOrder);

    HtmlTableRow tr = new HtmlTableRow();
    tblReport.Rows.Add(tr);
    tr.Attributes["class"] = row.Length == 0 ? "ReportHeader Closed" : "ReportHeader Opened";

    HtmlTableCell td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = 3;
    td.InnerText = WebConfigSettings.ConfigurationTablePrefix + table.TableName;
    
    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = 2;

    if (row.Length > 0)
    {
      td.InnerText = String.Format("{0} deactivated", row.Length);
    }

    for (int i = 0; i < row.Length; ++i)
    {
      tr = new HtmlTableRow();
      tblReport.Rows.Add(tr);
      tr.Attributes["class"] = "ReportRow";

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
      td.ColSpan = 2;
    }

    tr = new HtmlTableRow();
    tblReport.Rows.Add(tr);
    tr.Attributes["class"] = "ReportSpacer";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.ColSpan = ColumnCount;
    td.Height = "6px";
  }
}
