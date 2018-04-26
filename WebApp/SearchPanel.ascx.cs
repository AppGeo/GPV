//  Copyright 2014 Applied Geographics, Inc.
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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class SearchPanel : System.Web.UI.UserControl
{
  private void AddInputControl(HtmlControl parent, HtmlControl control, Configuration.SearchInputFieldRow searchInputFieldRow, string className, string tip)
  {
    parent.Controls.Add(control);
    control.Attributes["class"] = "Input " + className;
    control.Attributes["data-id"] = searchInputFieldRow.FieldID;
    control.Attributes["title"] = tip;
  }

  public void Initialize(Configuration.ApplicationRow application)
  {
    // find all searches for this application

    List<Configuration.SearchRow> searches = new List<Configuration.SearchRow>();

    foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
    {
      Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;

      foreach (Configuration.MapTabLayerRow mapTabLayerRow in mapTabRow.GetMapTabLayerRows().Where(o => !o.IsAllowTargetNull() && o.AllowTarget > 0))
      {
        Configuration.LayerRow layerRow = mapTabLayerRow.LayerRow;

        foreach (Configuration.SearchRow searchRow in layerRow.GetSearchRows())
        {
          if (!searches.Any(o => o.SearchID == searchRow.SearchID))
          {
            searches.Add(searchRow);
          }
        }
      }
    }

    // generate the search interfaces

    foreach (Configuration.SearchRow searchRow in searches)
    {
      // create the panel for this search

      HtmlGenericControl search = new HtmlGenericControl("div");
      search.Attributes["data-search"] = searchRow.SearchID;
      search.Attributes["class"] = "Search";
      search.Style["display"] = "none";

      foreach (Configuration.SearchInputFieldRow searchInputFieldRow in searchRow.GetSearchInputFieldRows().OrderBy(o => o.SequenceNo))
      {
        // add UI elements for this criterion

        HtmlGenericControl searchInputField = new HtmlGenericControl("div");
        search.Controls.Add(searchInputField);
        searchInputField.Attributes["data-criteria"] = searchInputFieldRow.FieldID;
        searchInputField.Attributes["class"] = "SearchInputField";

        HtmlGenericControl searchLabel = new HtmlGenericControl("span");
        searchInputField.Controls.Add(searchLabel);
        searchLabel.InnerText = searchInputFieldRow.DisplayName;
        searchLabel.Attributes["class"] = "Label";

        HtmlGenericControl betweenText;

        switch (searchInputFieldRow.FieldType)
        {
          case "autocomplete":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "Autocomplete", "Enter some text");
            break;

          case "date":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "Date", "Enter a date");
            break;

          case "daterange":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "DateRange 1", "Enter a date");

            betweenText = new HtmlGenericControl("span");
            searchInputField.Controls.Add(betweenText);
            betweenText.InnerText = " - ";

            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "DateRange 2", "Enter a date");
            break;

          case "list":
            HtmlSelect select = CreateSelect(searchInputFieldRow);
            AddInputControl(searchInputField, select, searchInputFieldRow, "List", "Select from a list");
            break;

          case "number":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "Number", "Enter a number");
            break;

          case "numberrange":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "NumberRange 1", "Enter a number");

            betweenText = new HtmlGenericControl("span");
            searchInputField.Controls.Add(betweenText);
            betweenText.InnerText = " - ";

            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "NumberRange 2", "Enter a number");
            break;

          case "text":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "Text", "Enter some text");
            break;

          case "textcontains":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "Text", "Enter some text (contains)");
            break;

          case "textstarts":
            AddInputControl(searchInputField, new HtmlInputText("text"), searchInputFieldRow, "Text", "Enter some text (starts with)");
            break;
        }

        search.Controls.Add(new LiteralControl("<br/>"));
      }
     
      pnlSearchScroll.Controls.Add(search);
    }
  }

  private HtmlSelect CreateSelect(Configuration.SearchInputFieldRow searchInputFieldRow)
  {
    HtmlSelect select = new HtmlSelect();

    using (OleDbCommand command = searchInputFieldRow.GetDatabaseCommand())
    {
      if (command.Parameters.Count > 0)
      {
        command.Parameters[0].Value = AppUser.GetRole();
      }

      using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
      {
        DataTable list = new DataTable();
        adapter.Fill(list);

        select.DataSource = list;
        select.DataValueField = list.Columns[0].ColumnName;
        select.DataBind();
      }

      command.Connection.Dispose();
    }

    select.Items.Insert(0, new ListItem("", ""));
    return select;
  }
}