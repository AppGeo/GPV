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

    // =====  public interface  =====

    appState.update = function (state) {
      $.extend(appState, state);
    };

    appState.toJson = function () {
      var props = $.makeArray(arguments);

      // if the names of the AppState properties to serialize were not passed as arguments, 
      // use all of the non-function properties of AppState

      if (!props.length) {
        for (var prop in appState) {
          if (typeof (appState[prop]) != "function") {
            props.push(prop);
          }
        }
      }

      // populate a new state object with the specified AppState properties and return it as JSON

      var state = {};

      $.each(props, function (i, p) {
        state[p] = appState[p];
      });

      return JSON.stringify(state);
    };
  });

  return gpv;
})(GPV || {});
