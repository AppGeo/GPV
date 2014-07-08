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
    var $pnlContent = $("#pnlContent")
    var $pnlMapSizer = $("#pnlMapSizer");
    var $pnlFunctionSizer = $("#pnlFunctionSizer");
    var $contentDivider = $("#contentDivider");

    var $pnlSelection = $("#pnlSelection")
    var $pnlQuerySizer = $("#pnlQuerySizer");
    var $pnlDataSizer = $("#pnlDataSizer");
    var $selectionDivider = $("#selectionDivider");

    var $pnlSearch = $("#pnlSearch");
    var $pnlSearchSizer = $("#pnlSearchSizer");
    var $pnlSearchDataSizer = $("#pnlSearchDataSizer");
    var $pnlSearchGridSizer = $("#pnlSearchGridSizer");
    var $pnlSearchCriteriaSizer = $("#pnlSearchCriteriaSizer");
    var $searchDivider = $("#searchDivider");
    var $searchCriteriaDivider = $("#searchCriteriaDivider");

    var $pnlLocation = $("#pnlLocation")
    var $pnlOverviewSizer = $("#pnlOverviewSizer");
    var $pnlZoneLevelSizer = $("#pnlZoneLevelSizer");
    var $locationDivider = $("#locationDivider");

    var mapResizedHandlers = [];
    var functionResizedHandlers = [];

    // =====  control events  =====

    $contentDivider.on("touchstart mousedown", function () {
      var $resizer = $("<div>").addClass("Resizer Main")
        .on("touchmove mousemove", function (e) {
          var x = gpv.getEventPoint(e)[0] - $pnlContent.offset().left;

          if (x - 4 >= parseInt($pnlMapSizer.css("min-width"), 10)) {
            x = $pnlContent.width() - x;

            if (x - 4 >= parseInt($pnlFunctionSizer.css("min-width"), 10)) {
              $pnlMapSizer.css("right", (x + 4) + "px");
              $pnlFunctionSizer.css("width", (x - 4) + "px");
            }
          }
        })
        .on("touchend touchcancel mouseup touchleave mouseleave", resizerFinish)
        .on("selectstart", function () { return false; })
        .appendTo($pnlContent);

      function resizerFinish() {
        $contentDivider.css("right", parseInt($pnlMapSizer.css("right"), 10) - 8 + "px");
        $resizer.remove();
        $resizer = null;

        $.each(mapResizedHandlers, function () {
          this();
        });
      }
    });

    $selectionDivider.on("touchstart mousedown", function () {
      createFunctionResizer($selectionDivider, $pnlSelection, $pnlQuerySizer, $pnlDataSizer);
    });

    $searchDivider.on("touchstart mousedown", function () {
      createFunctionResizer($searchDivider, $pnlSearch, $pnlSearchSizer, $pnlSearchDataSizer);
    });

    $searchCriteriaDivider.on("touchstart mousedown", function () {
      createFunctionResizer($searchCriteriaDivider, $pnlSearchSizer, $pnlSearchCriteriaSizer, $pnlSearchGridSizer);
    });

    $locationDivider.on("touchstart mousedown", function () {
      createFunctionResizer($locationDivider, $pnlLocation, $pnlOverviewSizer, $pnlZoneLevelSizer);
    });

    $("#tabSelection").on("click", function () {
      adjustFunctionSizes($pnlSelection, $pnlQuerySizer, $selectionDivider, $pnlDataSizer);
    });

    $("#tabSearch").on("click", function () {
      adjustFunctionSizes($pnlSearch, $pnlSearchSizer, $searchDivider, $pnlSearchDataSizer);
      adjustFunctionSizes($pnlSearchSizer, $pnlSearchCriteriaSizer, $searchCriteriaDivider, $pnlSearchGridSizer);
    });

    $("#tabLocation").on("click", function () {
      adjustFunctionSizes($pnlLocation, $pnlOverviewSizer, $locationDivider, $pnlZoneLevelSizer);
    });

    $(window).on("resize", function () {
      var x = $pnlContent.width() - $pnlMapSizer.width() - 8;

      if ($pnlFunctionSizer.width() > x) {
        $pnlFunctionSizer.width(x);
        $contentDivider.css("right", x + "px");
        $pnlMapSizer.css("right", x + 8 + "px");
      }

      adjustFunctionSizes($pnlSelection, $pnlQuerySizer, $selectionDivider, $pnlDataSizer);
      adjustFunctionSizes($pnlLocation, $pnlOverviewSizer, $locationDivider, $pnlZoneLevelSizer);
    });

    // =====  private functions  =====

    function adjustFunctionSizes($container, $topPanel, $divider, $bottomPanel) {
      var y = $container.height() - $bottomPanel.height() - 8;

      if ($topPanel.height() > y) {
        $topPanel.height(y);
        $divider.css("top", y + "px")
        $bottomPanel.css("top", y + 8 + "px")
      }
    }

    function createFunctionResizer($target, $container, $topPanel, $bottomPanel) {
      var name = $container.attr("id").substr(3);

      $resizer = $("<div>").addClass("Resizer Function")
        .on("touchmove mousemove", function (e) {
          var y = gpv.getEventPoint(e)[1] - $container.offset().top;

          if (y - 4 >= parseInt($topPanel.css("min-height"), 10)) {
            var h = $container.height() - y;

            if (h - 4 >= parseInt($bottomPanel.css("min-height"), 10)) {
              $topPanel.css("height", (y - 4) + "px");
              $bottomPanel.css("top", (y + 4) + "px");
            }
          }
        })
        .on("touchend touchcancel mouseup touchleave mouseleave", resizerFinish)
        .on("selectstart", function () { return false; })
        .appendTo($container);

      function resizerFinish() {
        $target.css("top", $topPanel.height() + "px");
        $resizer.remove();
        $resizer = null;

        $.each(functionResizedHandlers, function () {
          this(name);
        });
      }
    }

    // =====  public interface  =====

    gpv.resize = {
      mapResized: function (fn) { mapResizedHandlers.push(fn); },
      functionResized: function (fn) { functionResizedHandlers.push(fn); }
    };
  });

  return gpv;
})(GPV || {});
