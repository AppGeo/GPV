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
    var appState = gpv.appState;
    var service = "Services/Selection.ashx";
    var changedHandlers = [];

    // =====  private functions  =====

    function post(args) {
      args.url = service;
      gpv.post(args);
    }

    function triggerChanged(truncated, scaleBy) {
      $.each(changedHandlers, function () {
        this(truncated, scaleBy);
      });
    }

    // =====  public functions  =====

    function getActiveExtent(callback) {
      if (appState.ActiveMapId) {
        post({
          data: {
            m: "GetActiveExtent",
            state: appState.toJson("MapTab", "TargetLayer", "ActiveMapId")
          },
          success: function (result) {
            callback(result && result.extent ? result.extent : null);
          }
        });
      }
      else {
        callback(null);
      }
    }

    function getSelectionExtent(callback) {
      if (appState.TargetIds.length || appState.SelectionIds.length) {
        post({
          data: {
            m: "GetSelectionExtent",
            state: appState.toJson()
          },
          success: function (result) {
            callback(result && result.extent ? result.extent : null);
          }
        });
      }
      else {
        callback(null);
      }
    }

    function selectByGeometry(geo, mode) {
      post({
        data: {
          m: "SelectFeatures",
          state: appState.toJson(),
          geo: geo.join(","),
          mode: mode ? mode : "new"
        },
        success: function (result) {
          if (result && result.state) {
            appState.update(result.state);
            triggerChanged(result.truncated);
            showMaximumExceeded(result.truncated);
          }
        }
      });
    }

    function showMaximumExceeded(show) {
      if (show) {
        alert("Maximum number of selected features (" + appState.TargetIds.length + ") exceeded");
      }
    }

    function update(scaleBy) {
      post({
        data: {
          m: "SelectFeatures",
          state: appState.toJson()
        },
        success: function (result) {
          if (result && result.state) {
            appState.update(result.state);
            triggerChanged(result.truncated, scaleBy);
            showMaximumExceeded(result.truncated);
          }
        }
      });
    }

    // =====  public interface  =====

    gpv.selection = {
      changed: function (fn) { changedHandlers.push(fn); },
      getActiveExtent: getActiveExtent,
      getSelectionExtent: getSelectionExtent,
      selectByGeometry: selectByGeometry,
      update: update
    };
  });

  return gpv;
})(GPV || {});
