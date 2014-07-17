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

var GPV = (function (gpv) {
  $(function () {
    var $map = $("#mapMain");
    var $container = $("#pnlSearch");
    var config = gpv.configuration;
    var appState = gpv.appState;
    var service = "Services/SearchPanel.ashx";

    // =====  controls  =====
    
    var $cmdSearch = $("#cmdSearch").on("click", search);
    var $ddlSearch = $("#ddlSearches").on("change", searchChanged);

    var $grdSearch = $("#grdSearch").dataGrid({
      multiSelect: true,
      rowClass: "DataGridRow",
      alternateClass: "DataGridRowAlternate",
      selectedClass: "DataGridRowSelect"
    });

    var $input = $container.find(".SearchCriteria .Input").on("keyup change", function () {
      $cmdSearch.toggleClass("Disabled", getFilledInputs().length == 0);
    });

    $container.find(".Autocomplete").each(function () {
      var $this = $(this);
      $this.autocomplete({
        serviceUrl: service,
        params: { m: "Autocomplete", criteria: $this.attr("data-id") },
        triggerSelectOnValidInput: false,
        showNoSuggestionNotice: true,
        noSuggestionNotice: 'No matching results',
      });
    });

    // =====  component events
    
    gpv.on("viewer", "mapTabChanged", fillSearches);

    // =====  private functions  =====

    function fillSearches() {
      var changed = gpv.loadOptions($ddlSearch, config.mapTab[appState.MapTab].search);

      if (changed) {
        searchChanged();
      }
    }

    function getFilledInputs() {
      return $container.find(".Search:visible").find(".SearchCriteria .Input").map(function () {
        if ($(this).val()) {
          return this;
        }
      });
    }

    function search() {
      var criteria = {};
      
      getFilledInputs().each(function () {
        var $this = $(this);
        var id = $this.attr("data-id");

        if ($this.hasClass("Between")) {
          if (!criteria.hasOwnProperty(id)) {
            criteria[id] = [null, null];
          }

          criteria[id][$this.hasClass("1") ? 0 : 1] = $this.val();
        }
        else {
          criteria[id] = $this.val();
        }
      });

      gpv.post({
        url: service,
        data: {
          app: gpv.appState.Application,
          search: $container.find(".Search:visible").attr("data-search"),
          criteria: JSON.stringify(criteria)
        },
        success: function (result) {
          if (result) {
            $grdSearch.dataGrid("load", result);
          }
        }
      });
    }

    function searchChanged() {
      var $search = $container.find(".Search").hide();
      var $opt = $ddlSearch.find("option:selected");

      if ($opt.length) {
        $search.filter("[data-search='" + $opt.val() + "']").show();
      }

      $grdSearch.dataGrid("empty");
    }

    fillSearches();
  });

  return gpv;
})(GPV || {});
