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
    var $container = $(".LegendScroll");
    var $layerContainer = $("#pnlLayerScroll");
    var $tileContainer = $("#pnlTileScroll");

    // =====  control events  =====

    $layerContainer.find(".LegendExpander").on("click", function (e) {
      var $this = $(this);

      if (!$this.hasClass("Empty")) {
        var expanded = $this.hasClass("Expanded");
        $this.removeClass(expanded ? "Expanded" : "Collapsed").addClass(!expanded ? "Expanded" : "Collapsed");
        var $content = $this.closest(".LegendEntry").find(".LegendContent").eq(0);
        $content.css("display", !expanded ? "block" : "none");

        // expand/collapse all descendents if the shift key is depressed

        if (e.shiftKey) {
          $content.find(".LegendExpander").removeClass(expanded ? "Expanded" : "Collapsed").addClass(!expanded ? "Expanded" : "Collapsed");
          $content.find(".LegendContent").css("display", !expanded ? "block" : "none");
        }
      }
    }).on("selectstart", function () { return false; });

    $layerContainer.find(".LegendCheck").on("click", function () {
      var $this = $(this);
      var isChecked = $this.is(":checked");

      if ($this.is(":checked")) {
        $this.parents(".LegendEntry").each(function (i, e) {
          $(this).find(".LegendCheck").eq(0).prop("checked", true);
        });
      }

      // traverse the descendants in reverse order so that the first radio button in each exclusive group
      // is checked on

      var $children = $this.closest(".LegendEntry").find(".LegendCheck");

      for (var i = $children.length - 1; i >= 0; --i) {
        $children.eq(i).prop("checked", isChecked);
      }
    });

    $tileContainer.find(".LegendCheck").on("click", function () {
      var $this = $(this);
      var isChecked = $this.is(":checked");

      gpv.viewer.toggleTileGroup($this.attr("data-tilegroup"), isChecked);
      gpv.appState.VisibleTiles[gpv.appState.MapTab] = getVisibleTiles(gpv.appState.MapTab);
    });

    $("#cmdRefreshMap").on("click", function () {
      gpv.viewer.refreshMap();
    });

    // =====  component events  =====

    gpv.on("viewer", "mapTabChanged", function () {
      $container.find(".LegendTop").hide().filter('[data-maptab="' + gpv.appState.MapTab + '"]').show();
    });

    gpv.on("selectionPanel", "reinitialized", function (query) {
      reinitializeLayers(query);
    });

    // =====  private functions  =====

    function reinitializeLayers(query) {
      if ("layerson" in query) {
        toggleLayers(query.layerson, true);
      }

      if ("layersoff" in query) {
        toggleLayers(query.layersoff, false);
      }
    }

    function toggleLayers(list, check) {
      var $legend = $layerContainer.find('.LegendTop[data-maptab="' + gpv.appState.MapTab + '"]');

      $.each(list, function () {
        $legend.find('input[data-layer="' + this + '"]').prop("checked", check);
      });
    }

    // =====  public functions  =====

    function getVisibleLayers(mapTabID) {
      var layerIds = [];

      $layerContainer.find(".LegendTop").filter('[data-maptab="' + mapTabID + '"]').find(".LegendCheck:checked").each(function (i, e) {
        var layer = $(this).attr("data-layer");

        if (layer) {
          layerIds.push(layer);
        }
      });

      return layerIds;
    }

    function getVisibleTiles(mapTabID) {
      var tileGroupIds = [];

      $tileContainer.find(".LegendTop").filter('[data-maptab="' + mapTabID + '"]').find(".LegendCheck:checked").each(function (i, e) {
        var tileGroup = $(this).attr("data-tilegroup");

        if (tileGroup) {
          tileGroupIds.push(tileGroup);
        }
      });

      return tileGroupIds;
    }

    // =====  public interface  =====

    gpv.legendPanel = {
      getVisibleLayers: getVisibleLayers,
      getVisibleTiles: getVisibleTiles
    };
  });

  return gpv;
})(GPV || {});
