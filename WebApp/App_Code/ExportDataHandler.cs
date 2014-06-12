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
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

public class ExportDataHandler : IHttpHandler
{
  private HttpResponse Response = null;

  public void ProcessRequest(HttpContext context)
  {
    Response = context.Response;
    Response.Clear();

    string layer = context.Request.Form["layer"];
    string ids = context.Request.Form["ids"];

    WriteData(layer, ids);
  }

  private void WriteData(string layerId, string ids)
  {
    Configuration config = AppContext.GetConfiguration();
    Configuration.LayerFunctionRow layerFunction = config.LayerFunction.First(o => o.LayerID == layerId && o.Function == "export");
    DataTable table = new DataTable();

    using (OleDbCommand command = layerFunction.GetDatabaseCommand())
    {
      command.Parameters[0].Value = ids;

      if (command.Parameters.Count > 1)
      {
        command.Parameters[1].Value = AppUser.GetRole();
      }

      using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
      {
        adapter.Fill(table);
      }

      command.Connection.Dispose();
    }
 
    // create a CSV file if specified

    if (AppSettings.ExportFormat == "csv")
    {
      ExportToCsv(table);
    }
    else
    {
      ExportToExcel(table, layerFunction.LayerRow.Name);
    }
   }

  private void ExportToCsv(DataTable table)
  {
    Response.ContentType = "text/csv";
    Response.AddHeader("Content-Disposition", "attachment; filename=Export.csv");

    using (StreamWriter writer = new StreamWriter(Response.OutputStream))
    {
      string[] values = table.Columns.Cast<DataColumn>().Select(o => FormatForCsv(o.ColumnName)).ToArray();
      writer.WriteLine(String.Join(",", values));

      foreach (DataRow row in table.Rows)
      {
        values = row.ItemArray.Select(o => FormatForCsv(o)).ToArray();
        writer.WriteLine(String.Join(",", values));
      }
    }
  }

  private void ExportToExcel(DataTable table, string layerName)
  {
    HSSFWorkbook workbook = new HSSFWorkbook();
 
    HSSFFont font = workbook.CreateFont();
    font.Color = HSSFColor.DARK_BLUE.index;
    font.Boldweight = 2000;

    HSSFCellStyle headerStyle = workbook.CreateCellStyle();
    headerStyle.SetFont(font);

    HSSFCellStyle dateStyle = workbook.CreateCellStyle();
    dateStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("m/d/yy");

    HSSFSheet sheet = workbook.CreateSheet(layerName);
    sheet.CreateFreezePane(0, 1, 0, 1);

    // create the column headers

    int r = 0;

    HSSFRow xRow = sheet.CreateRow(r);

    for (int c = 0; c <= table.Columns.Count - 1; ++c)
    {
      HSSFCell xCell = xRow.CreateCell(c);
      xCell.SetCellValue(table.Columns[c].ColumnName);
      xCell.CellStyle = headerStyle;
    }

    // convert feature attributes to cell values

    foreach (DataRow row in table.Rows)
    {
      r += 1;
      xRow = sheet.CreateRow(r);

      for (int c = 0; c <= table.Columns.Count - 1; ++c)
      {
        HSSFCell xCell = xRow.CreateCell(c);

        if (!row.IsNull(c))
        {
          switch (table.Columns[c].DataType.Name)
          {
            case "Byte":
            case "Int16":
            case "Int32":
            case "Int64":
            case "Single":
            case "Double":
            case "Decimal":
              xCell.SetCellValue(Convert.ToDouble(row[c]));
              break;

            case "DateTime":
              xCell.SetCellValue(Convert.ToDateTime(row[c]));
              xCell.CellStyle = dateStyle;
              break;

            default:
              xCell.SetCellValue(row[c].ToString());
              break;
          }
        }
      }
    }

    // return the Excel workbook

    Response.Clear();
    Response.ContentType = "application/vnd.ms-excel";
    Response.AddHeader("Content-disposition", "attachment; filename=Export.xls");
    workbook.Write(Response.OutputStream);
  }

  private string FormatForCsv(object value)
  {
    if (value == null)
    {
      return "";
    }

    string s = value is DateTime ? ((DateTime)value).ToString("yyyy-MM-dd") : value.ToString();

    bool containsQuote = s.Contains("\"");

    if (containsQuote || s.Contains(",") || s.Contains("\n") || s.StartsWith(" ") || s.EndsWith(" "))
    {
      if (containsQuote)
      {
        s = s.Replace("\"", "\"\"");
      }

      s = String.Format("\"{0}\"", s);
    }

    return s;
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}