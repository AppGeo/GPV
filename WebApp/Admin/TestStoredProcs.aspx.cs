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
using System.Web.UI.HtmlControls;

public partial class Admin_TestStoredProcs : CustomStyledPage
{
  protected void cmdRetest_Click(object sender, EventArgs e)
  {
    ViewState["Step"] = 0;
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    if (ViewState["Step"] == null || (int)ViewState["Step"] == 0)
    {
      labMessage.Visible = true;
      cmdRetest.Visible = false;
      tblReport.Visible = false;

      string script = "$('body').find('*').css('cursor', 'wait'); document.forms[0].submit();";
      ClientScript.RegisterStartupScript(typeof(Admin_TestStoredProcs), "start", script, true);

      ViewState["Step"] = 1;
    }
    else
    {
      labMessage.Visible = false;
      cmdRetest.Visible = true;
      tblReport.Visible = true;

      tblReport.Rows.Clear();

      Configuration config = Configuration.GetCurrent();
      config.CascadeDeactivated();
      config.RemoveDeactivated();

      DataTable table = GenerateReportTable(config);
      WriteReportTable(table);
    }
  }

  private DataTable GenerateReportTable(Configuration config)
  {
    OleDbConnection baseConnection = null;
    Dictionary<String, OleDbConnection> connections = new Dictionary<String, OleDbConnection>();
    DataTable reportTable = null;

    // open connections

    try
    {
      baseConnection = AppContext.GetDatabaseConnection();

      foreach (Configuration.ConnectionRow connectionRow in config.Connection.Rows)
      {
        try
        {
          OleDbConnection connection = new OleDbConnection(connectionRow.ConnectionString);
          connection.Open();
          connections.Add(connectionRow.ConnectionID.ToLower(), connection);
        }
        catch { }
      }

      reportTable = new DataTable();
      reportTable.Columns.Add("ConnectionID", typeof(string));
      reportTable.Columns.Add("StoredProc", typeof(string));
      reportTable.Columns.Add("Time", typeof(int));
      reportTable.Columns.Add("Status", typeof(string));
      reportTable.Columns.Add("IsSearch", typeof(bool));

      DataColumn[] uniqueColumns = new DataColumn[] { reportTable.Columns["ConnectionID"], reportTable.Columns["StoredProc"] };
      reportTable.Constraints.Add(new UniqueConstraint(uniqueColumns));

      foreach (DataTable sourceTable in new DataTable[] { config.LayerFunction, config.DataTab, config.Query, config.Search, config.SearchCriteria })
      {
        foreach (DataRow sourceRow in sourceTable.Rows)
        {
          string storedProc = sourceRow["StoredProc"] as String;

          if (!String.IsNullOrEmpty(storedProc))
          {
            DataRow row = reportTable.NewRow();

            if (!sourceRow.IsNull("ConnectionID"))
            {
              row["ConnectionID"] = sourceRow["ConnectionID"];
            }

            row["StoredProc"] = sourceRow["StoredProc"];
            row["IsSearch"] = sourceTable == config.Search;

            try
            {
              reportTable.Rows.Add(row);
            }
            catch { }
          }
        }
      }

      foreach (DataRow row in reportTable.Rows)
      {
        OleDbConnection connection = baseConnection;

        if (!row.IsNull("ConnectionID"))
        {
          string connectionID = row["ConnectionID"].ToString().ToLower();

          if (connections.ContainsKey(connectionID))
          {
            connection = connections[connectionID];
          }
          else
          {
            row["Status"] = "Could not connect to database";
          }
        }

        if (row.IsNull("Status"))
        {
          int executionTime = -1;
          bool isSearch = (bool)row["IsSearch"];

          for (int i = 0; i <= 5; ++i)
          {
            using (OleDbCommand command = new OleDbCommand(row["StoredProc"].ToString(), connection))
            {
              command.CommandType = CommandType.StoredProcedure;

              for (int n = 1; n <= i; ++n)
              {
                string testValue = isSearch && n == 1 ? "1 = 0" : "0";
                command.Parameters.AddWithValue(n.ToString(), testValue);
              }

              long startTime = DateTime.Now.Ticks;

              try
              {
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                  executionTime = Convert.ToInt32((DateTime.Now.Ticks - startTime) / 10000L);
                }
              }
              catch { }
            }

            if (executionTime >= 0)
            {
              break;
            }
          }

          if (executionTime >= 0)
          {
            row["Time"] = executionTime;
          }
          else
          {
            row["Status"] = "Stored procedure failed to execute";
          }
        }
      }
    }
    finally
    {
      if (baseConnection != null)
      {
        baseConnection.Close();
      }

      foreach (string key in connections.Keys)
      {
        connections[key].Close();
      }
    }

    return reportTable;
  }

  private void WriteReportTable(DataTable table)
  {
    HtmlTableRow tr = new HtmlTableRow();
    tblReport.Rows.Add(tr);
    tr.Attributes["class"] = "DataGridHeader";

    HtmlTableCell td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.InnerText = "ConnectionID";
    td.Style["width"] = "120px";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.InnerText = "Stored Proc";
    td.Style["width"] = "240px";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.InnerText = "Time";
    td.Style["width"] = "50px";

    td = new HtmlTableCell();
    tr.Cells.Add(td);
    td.InnerText = "Status";
    td.Style["width"] = "220px";

    string rowClass = "DataGridRow";

    foreach (DataRow row in table.Select("", "ConnectionID, StoredProc"))
    {
      rowClass = rowClass == "DataGridRow" ? "DataGridRowAlternate" : "DataGridRow";

      tr = new HtmlTableRow();
      tblReport.Rows.Add(tr);
      tr.Attributes["class"] = rowClass;

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";

      if (!row.IsNull("ConnectionID"))
      {
        td.InnerText = row["ConnectionID"].ToString();
      }

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";
      td.InnerText = row["StoredProc"].ToString();

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.Align = "right";
      td.VAlign = "top";

      if (!row.IsNull("Time"))
      {
        td.InnerText = row["Time"].ToString() + " ms";
      }

      td = new HtmlTableCell();
      tr.Cells.Add(td);
      td.VAlign = "top";

      if (!row.IsNull("Status"))
      {
        td.InnerText = row["Status"].ToString();
      }
    }
  }
}
