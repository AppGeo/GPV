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

var GPV = (function (gpv) {
  $(function () {
    var $body = $("body");
    var $container = $("#pnlSearch");
    var config = gpv.configuration;
    var appState = gpv.appState;
    var service = "Services/SearchPanel.ashx";
    var initializing = true;

    // =====  controls  =====
    
    var $cmdSearch = $("#cmdSearch").on("click", search);
    var $cmdReset = $("#cmdReset").on("click", reset);
    var $labSearchCount = $("#labSearchCount");
    var $cmdShowAllOnMap = $("#cmdShowAllOnMap").on("click", showOnMap);
    var $cmdShowOnMap = $("#cmdShowOnMap").on("click", showOnMap);
    var $ddlSearch = $("#ddlSearches").on("change", searchChanged);

    $(".Number,.NumberRange").numericInput();
    $(".Date,.DateRange").dateInput().datepicker({ showAnim: "slideDown", changeMonth: true, changeYear: true });
    $(".SearchInputField:has(select)").addClass('customSearch');


    var $grdSearch = $("#grdSearch").dataGrid({
      multiSelect: true,
      rowClass: "DataGridRow",
      alternateClass: "DataGridRowAlternate",
      selectedClass: "DataGridRowSelect",
      selectionChanged: resultGridChanged
    });

    var $input = $container.find(".SearchInputField .Input").on("keyup change", function (e) {
      var hasData = getFilledInputs().length > 0;
      $cmdSearch.toggleClass("Disabled", !hasData);
      $cmdReset.toggleClass("Disabled", !hasData);

      if (e.type == "keyup" && hasData && e.which == 13) {
        search();
      }
    });

    $container.find(".Autocomplete").each(function () {
      var $this = $(this);
      $this.autocomplete({
        serviceUrl: service,
        params: { m: "Autocomplete", criteria: $this.attr("data-id") },
        triggerSelectOnValidInput: false,
        showNoSuggestionNotice: true,
        noSuggestionNotice: 'No matching results'
      });
    });

    // =====  component events
    
    gpv.on("viewer", "mapTabChanged", fillSearches);

    // =====  private functions  =====

    function emptyResultGrid() {
      $grdSearch.dataGrid("empty");
      $cmdShowOnMap.addClass("Disabled");
      $cmdShowAllOnMap.addClass("Disabled");
    }

    function fillSearches() {
      var changed = gpv.loadOptions($ddlSearch, config.mapTab[appState.MapTab].search);

      if (changed) {
        searchChanged();
      }
    }

    function getFilledInputs() {
      return $container.find(".Search:visible").find(".SearchInputField .Input").map(function () {
        if ($(this).val()) {
          return this;
        }
      });
    }
   
    function getSearchResults() {
      gpv.post({
        url: service,
        data: {
          state: appState.toJson("Application", "Search", "SearchCriteria")
        },
        success: function (result) {
          if (result) {
            $grdSearch.dataGrid("load", result);
            $labSearchCount.text((result.rows.length == 0 ? "None" : result.rows.length) + " found");
            $cmdShowAllOnMap.toggleClass("Disabled", result.rows.length == 0);

            if (!initializing && gpv.settings.searchAutoSelect && result.rows.length == 1) {
              showOnMap();
            }
          }
        }
      });
    }

    function initialize() {
      gpv.loadOptions($ddlSearch, config.mapTab[appState.MapTab].search);

      if (!appState.Search && $ddlSearch.val()) {
        appState.Search = $ddlSearch.val();
      }

      if (appState.Search) {
        $ddlSearch.val(appState.Search);
        var $panel = $container.find(".Search").filter("[data-search='" + appState.Search + "']").show();
        var keys = Object.keys(appState.SearchCriteria);

        if (keys.length) {
          Object.keys(appState.SearchCriteria).forEach(function (k) {
            var $input = $panel.find(".SearchInputField .Input[data-id='" + k + "']")
            var v = appState.SearchCriteria[k];

            if ($input.hasClass("DateRange") || $input.hasClass("NumberRange")) {
              $input.filter(".1").val(v[0]);
              $input.filter(".2").val(v[1]);
            }
            else {
              $input.val(v);
            }
          });

          getSearchResults();
        }
      }

      initializing = undefined;
    }

    function reset() {
      if (!$cmdReset.hasClass("Disabled")) {
        $("#pnlSearchScroll").find("input:text").val('');
        $("#pnlSearchScroll").find("select").prop('selectedIndex', 0);
        $cmdSearch.toggleClass("Disabled");
        $cmdReset.toggleClass("Disabled");
        $labSearchCount.text("None Found");
        emptyResultGrid();
      }
    }

    function resultGridChanged(dblClick) {
      if (dblClick) {
        updateTargets($grdSearch.dataGrid("getSelection"));
      }
      else {
        $cmdShowOnMap.toggleClass("Disabled", $grdSearch.dataGrid("getSelection").length == 0);
      }
    }

    function search() {
      if (!$cmdSearch.hasClass("Disabled")) {
        emptyResultGrid();
        var criteria = appState.SearchCriteria = {};

        getFilledInputs().each(function () {
          var $this = $(this);
          var id = $this.attr("data-id");

          if ($this.hasClass("DateRange") || $this.hasClass("NumberRange")) {
            if (!appState.SearchCriteria.hasOwnProperty(id)) {
              criteria[id] = [null, null];
            }

            criteria[id][$this.hasClass("1") ? 0 : 1] = $this.val();
          }
          else {
            criteria[id] = $this.val();
          }
        });

        getSearchResults();
      }
    }

    function searchChanged() {
      var $search = $container.find(".Search").hide();
      appState.Search = "";
      var $opt = $ddlSearch.find("option:selected");

      if ($opt.length) {
        appState.Search = $opt.val();
        $search.filter("[data-search='" + appState.Search + "']").show();
      }

      emptyResultGrid();
      appState.SearchCriteria = {};
    }

    function showOnMap() {
      var ids = $(this)[0].id == "cmdShowOnMap" ? $grdSearch.dataGrid("getSelection") : $grdSearch.dataGrid("getIds");
      updateTargets(ids);
    }

    function updateTargets(ids) {
      var layerID = config.search[appState.Search].layer.id;
      var targetIds = $.map(ids, function (id) { return id.m; }).join(",");
      var url = "application:action=0&selectionlayer=&selectionids=&scaleby=1.2&targetlayer=" + layerID + "&targetids=" + targetIds;

      if (ids.length == 1) {
        url += "&activemapid=" + ids[0].m;
        url += "&activedataid=" + (ids[0].hasOwnProperty("d") ? ids[0].d : ids[0].m);
      }

      gpv.selectionPanel.reinitialize(url);
      $("#tabSelection").trigger("click");
    }

    initialize();
  });

  return gpv;
})(GPV || {});
