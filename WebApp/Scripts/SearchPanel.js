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

    var $ddlSearches = $("#ddlSearches").on("change", searchesChanged);


    function fillSearches(initializing) {
      //var changed = loadOptions($ddlSearches, config.mapTab[appState.MapTab].target) && !initializing;

      //if (initializing) {
      //  syncAppState($ddlSearches, "Search");
      //}
      //else if (changed) {
      //  searchesChanged();
      //}

      //return changed;
    }

    function loadOptions($target, list) {
      var previous = $target.val();
      var changed = previous || list.length;
      $target.empty();

      $.each(list, function () {
        var same = this.id == previous;
        changed = changed && !same;
        $("<option/>").val(this.id).text(this.name).prop("selected", same).appendTo($target);
      });

      return changed;
    }

    function searchesChanged(e) {
    }

    function initialize() {
      fillSearches(true);
    }
  });

  return gpv;
})(GPV || {});
