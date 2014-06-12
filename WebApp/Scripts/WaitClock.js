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
    var $div = $("#waitClock");
    var $body = $("body");
    var handle = null;
    var index;

    function finish() {
      if (handle) {
        clearTimeout(handle);
        handle = null;
      }

      $div.hide();
      $body.css("cursor", "default");
    }

    function start() {
      if (!handle) {
        $body.css("cursor", "wait");
        index = -1;

        handle = setInterval(function () {
          index = (index + 1) % 10;
          $div.css("background-position-x", -index * 60).show();
        }, 1000);
      }
    }

    gpv.waitClock = {
      start: start,
      finish: finish
    };
  });

  return gpv;
})(GPV || {});
