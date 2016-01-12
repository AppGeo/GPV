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
    var map;
    var $optIdentify = $("#optIdentify");
    var $mapTip = $("#mapTip");
    var service = "Services/MapTip.ashx";

    var lastPoint = { x: -1, y: -1 };
    var timeoutHandle = null;
    var xhr = null;

    function mapMouseMove(e) {
      if (L.Browser.mobile && L.Browser.touch) {
        return;
      }

      if ($MapTool.filter(".Selected").attr("id") !== "optIdentify") {
        mapMouseOut();
        return;
      }

      var p = e.containerPoint;

      if (p.x != lastPoint.x || p.y != lastPoint.y) {
        lastPoint = p;
        p = map.options.crs.project(e.latlng);

        if (timeoutHandle) {
          clearTimeout(timeoutHandle);
        }

        timeoutHandle = setTimeout(function () {
          timeoutHandle = null;
          xhr = gpv.post({
            url: service,
            data: {
              maptab: gpv.appState.MapTab,
              visiblelayers: gpv.legendPanel.getVisibleLayers(gpv.appState.MapTab).join("\u0001"),
              level: gpv.appState.Level,
              x: p.x,
              y: p.y,
              distance: 4,
              scale: map.getProjectedPixelSize()
            },
            success: function (result) {
              xhr = null;

              if (result && result.tipText) {
                var tipText = result.tipText.split("\n");

                // append a span to the end of each line of text

                $.each(tipText, function (i) { tipText[i] += "<span>&nbsp;</span>"; });

                // set the height and initial location of the tip box and display it

                var height = tipText.length * 15;
                var left = lastPoint.x + 5;
                var top = lastPoint.y - height - 9;

                if (top < 0) {
                  top = lastPoint.y + 5;
                }

                $mapTip.css({ left: left + "px", top: top + "px", width: "2000px", height: height + "px" }).empty().append(tipText.join("<br>")).show();

                // refine the width and location: find the span farthest to the right and set the width to that

                var width = 0;

                $mapTip.find("span").each(function () {
                  width = Math.ceil(Math.max(width, $(this).offset().left - $mapTip.offset().left));
                });

                width += 6;

                // adjust the location so that the tip box does not go beyond the right border of the map

                if (left + width > map.getSize().x) {
                  left = lastPoint.x - width - 20;
                }

                $mapTip.css({ left: left + "px", width: width + "px" }).find("span").remove();
              }
              else {
                $mapTip.hide();
              }
            }
          });
        }, 100);
      }
    }

    function mapMouseOut() {
      if (timeoutHandle) {
        clearTimeout(timeoutHandle);
        timeoutHandle = null;
      }

      if (xhr) {
        xhr.abort();
        xhr = null;
      }

      $mapTip.hide();
    }

    function setMap(m) {
      map = m;
      map.on("mousemove", mapMouseMove);
      map.on("mouseout", mapMouseOut);
    }

    gpv.mapTip = {
      setMap: setMap
    };
  });

  return gpv;
})(GPV || {});
