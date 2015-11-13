//  Copyright 2015 Applied Geographics, Inc.
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
    var $container = $("#progress");
    var $bar = $container.find("#progressBar");

    var intervalHandle;
    var intervalCount;
    var interval = 50;
    var intervalMax = 60000 / interval;

    function clear() {
      if (intervalHandle) {
        clearInterval(intervalHandle);
        intervalHandle = null;
      }

      $container.hide();
    }

    function start() {
      if (!intervalHandle) {
        $bar.css("width", "0%");
        $container.show();

        intervalCount = 0;

        intervalHandle = setInterval(function () {
          intervalCount = (intervalCount + 1) % intervalMax;
          $bar.css("width", (intervalCount * 100 / intervalMax) + "%");
        }, interval);
      }
    }

    // =====  public interface  =====

    gpv.progress = {
      start: start,
      clear: clear
    };
  });

  return gpv;
})(GPV || {});
