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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class SearchPanel : System.Web.UI.UserControl
{
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
      search.Attributes["class"] = "search";
      search.Style["display"] = "none";

      foreach (Configuration.SearchCriteriaRow searchCriteriaRow in searchRow.GetSearchCriteriaRows().OrderBy(o => o.SequenceNo))
      {
        // add UI elements for this criterion
        HtmlGenericControl searchCriteria = new HtmlGenericControl("div");
        searchCriteria.Attributes["data-criteria"] = searchCriteriaRow.SearchCriteriaID;
        searchCriteria.Attributes["class"] = "searchcriteria";

        HtmlGenericControl searchLabel = new HtmlGenericControl("span");
        searchLabel.InnerText = searchCriteriaRow.DisplayName;
        searchCriteria.Controls.Add(searchLabel);
        searchLabel.Attributes["class"] = "Label";

        switch (searchCriteriaRow.SearchCriteriaType)
        {
          case "autocomplete":
            HtmlInputText autoComplete = new HtmlInputText("text");
            searchCriteria.Controls.Add(autoComplete);
            autoComplete.Attributes["class"] = "autocomplete";
            break;

          case "between":
            HtmlInputText minValue = new HtmlInputText("text");
            HtmlInputText maxValue = new HtmlInputText("text");
            HtmlGenericControl betweenText = new HtmlGenericControl("span");
            betweenText.InnerText = " - ";
            minValue.Attributes["class"] = "searchbetween";
            maxValue.Attributes["class"] = "searchbetween";
            searchCriteria.Controls.Add(minValue);
            searchCriteria.Controls.Add(betweenText);
            searchCriteria.Controls.Add(maxValue);
            break;

          case "lookup":
            
            HtmlSelect select = new HtmlSelect();
            searchCriteria.Controls.Add(select);
            select.Attributes["class"] = "searchselect";
            break;

          case "numeric":
            
            HtmlInputText numeric = new HtmlInputText("text");
            searchCriteria.Controls.Add(numeric);
            numeric.Attributes["class"] = "searchnumeric";
            break;

          case "text":
            HtmlInputText text = new HtmlInputText("text");
            searchCriteria.Controls.Add(text);
            text.Attributes["class"] = "searchtext";
            break;
        }

        search.Controls.Add(searchCriteria);
        search.Controls.Add(new HtmlGenericControl("br"));

      }
      pnlSearchScroll.Controls.Add(search);

    }
  }
}