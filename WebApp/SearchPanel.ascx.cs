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
  private void AddCriteriaValue(HtmlControl parent, HtmlControl control, Configuration.SearchCriteriaRow searchCriteriaRow, string className)
  {
    parent.Controls.Add(control);
    control.Attributes["class"] = "Input " + className;
    control.Attributes["data-id"] = searchCriteriaRow.SearchCriteriaID;
  }

  public void Initialize(Configuration.ApplicationRow application)
  {
    // find all searches for this application

    List<Configuration.SearchRow> searches = new List<Configuration.SearchRow>();

    foreach (Configuration.ApplicationMapTabRow appMapTabRow in application.GetApplicationMapTabRows())
    {
      Configuration.MapTabRow mapTabRow = appMapTabRow.MapTabRow;

      foreach (Configuration.MapTabLayerRow mapTabLayerRow in mapTabRow.GetMapTabLayerRows().Where(o => !o.IsAllowTargetNull() && o.AllowTarget == 1))
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

      foreach (Configuration.SearchCriteriaRow searchCriteriaRow in searchRow.GetSearchCriteriaRows().OrderBy(o => o.SequenceNo))
      {
        // add UI elements for this criterion
        HtmlGenericControl searchCriteria = new HtmlGenericControl("div");
        search.Controls.Add(searchCriteria);
        searchCriteria.Attributes["data-criteria"] = searchCriteriaRow.SearchCriteriaID;
        searchCriteria.Attributes["class"] = "SearchCriteria";

        HtmlGenericControl searchLabel = new HtmlGenericControl("span");
        searchCriteria.Controls.Add(searchLabel);
        searchLabel.InnerText = searchCriteriaRow.DisplayName;
        searchLabel.Attributes["class"] = "Label";

        switch (searchCriteriaRow.SearchCriteriaType)
        {
          case "autocomplete":
            AddCriteriaValue(searchCriteria, new HtmlInputText("text"), searchCriteriaRow, "Autocomplete");
            break;

          case "between":
            AddCriteriaValue(searchCriteria, new HtmlInputText("text"), searchCriteriaRow, "Between 1");

            HtmlGenericControl betweenText = new HtmlGenericControl("span");
            searchCriteria.Controls.Add(betweenText);
            betweenText.InnerText = " - ";
            
            AddCriteriaValue(searchCriteria, new HtmlInputText("text"), searchCriteriaRow, "Between 2");
            break;

          case "lookup":
            HtmlSelect select = CreateLookup(searchCriteriaRow);
            AddCriteriaValue(searchCriteria, select, searchCriteriaRow, "Lookup");
            break;

          case "numeric":
            AddCriteriaValue(searchCriteria, new HtmlInputText("text"), searchCriteriaRow, "Numeric");
            break;

          case "text":
            AddCriteriaValue(searchCriteria, new HtmlInputText("text"), searchCriteriaRow, "Text");
            break;
        }

        search.Controls.Add(new HtmlGenericControl("br"));
      }

      pnlSearchScroll.Controls.Add(search);
    }
  }

  private HtmlSelect CreateLookup(Configuration.SearchCriteriaRow searchCriteriaRow)
  {
    HtmlSelect select = new HtmlSelect();

    using (OleDbCommand command = searchCriteriaRow.GetDatabaseCommand())
    {
      if (command.Parameters.Count > 0)
      {
        command.Parameters[0].Value = AppUser.GetRole();
      }

      using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
      {
        DataTable lookupList = new DataTable();
        adapter.Fill(lookupList);

        select.DataSource = lookupList;
        select.DataTextField = searchCriteriaRow.SearchCriteriaID;
        select.DataValueField = searchCriteriaRow.SearchCriteriaID;
        select.DataBind();
      }

      command.Connection.Dispose();
    }

    select.Items.Insert(0, new ListItem("", ""));
    return select;
  }
}