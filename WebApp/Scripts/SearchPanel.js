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

    var $ddlSearches = $("#ddlSearches").on("change", searchChanged);

    // =====  component events
    
    gpv.on("viewer", "mapTabChanged", fillSearches);

    // =====  private functions  =====

    function fillSearches() {
      var changed = gpv.loadOptions($ddlSearches, config.mapTab[appState.MapTab].search);

      if (changed) {
        searchChanged()
      }
    }

    function searchChanged() {
      // show the criteria for the selected search
      // clear the results grid
    }

    // =====  public interface  =====

    gpv.searchPanel = {
    };

    fillSearches();
  });

  return gpv;
})(GPV || {});
