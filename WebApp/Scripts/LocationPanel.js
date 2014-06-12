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
    var service = "Services/LocationPanel.ashx";

    var $mapOverview = null;
    var fullExtent = gpv.configuration.fullExtent;

    var lastWidth = null;
    var lastHeight = null;
    var lastUrl = null;
    var extentDragging = false;

    // =====  control events  =====

    $("#pnlZoneLevelTabs .Tab").on("click", function () {
      var id = $(this).attr("data-table");

      $(".ZoneLevelTable").each(function () {
        $(this).toggle(this.id == id);
      });
    });

    $(".ZoneLevelSelection").on("click", function () {
      $(".NoSelection").toggle($(this).attr("id") == "optZoneLevelAll");
    });

    var $ZoneLevelTable = $(".ZoneLevelTable tbody").on("click", ".CommandLink", function () {
      var $tr = $(this).closest("tr");
      var zone = $tr.attr("data-zone");
      var level = $tr.attr("data-level");

      if (zone) {
        zoomToZone(zone, level);
      }
      else {
        appState.Level = level;
        gpv.viewer.refreshMap();
      }
    });

    $(".MapTool").on("click", function () {
      if ($mapOverview) {
        $mapOverview.geomap("option", "mode", $(this).attr("id") == "optPan" ? "static" : "dragBox");
      }
    });

    // =====  component events  =====

    gpv.on("resize", "functionResized", refreshOverviewMap);
    gpv.on("resize", "mapResized", refreshOverviewMap);
    gpv.on("viewer", "functionTabChanged", refreshOverviewMap);
    gpv.on("viewer", "extentChanged", drawMainExtent);
    gpv.on("selectionPanel", "gridFilled", showZoneLevelCounts);

    // =====  private functions  =====

    function dragExtentStart(e) {
      extentDragging = $mapOverview.geomap("option", "mode") == "static";
      dragExtent(e);
    }

    function dragExtent(e) {
      var bbox = null;

      if (extentDragging) {
        var offset = $mapOverview.offset();
        var p = $mapOverview.geomap("toMap", [e.pageX - offset.left, e.pageY - offset.top]);
        var bbox = $.geo.recenter(gpv.viewer.getExtent(), p);
        drawMainExtent(bbox);
      }

      return bbox;
    }

    function dragExtentEnd(e) {
      if (extentDragging) {
        gpv.viewer.setExtent(dragExtent(e));
        extentDragging = false;
      }
    }

    function drawMainExtent(bbox) {
      if ($mapOverview) {
        var c = [[[bbox[0], bbox[1]], [bbox[0], bbox[3]], [bbox[2], bbox[3]], [bbox[2], bbox[1]], [bbox[0], bbox[1]]]];
        $mapOverview.geomap("empty").geomap("append", { type: "Polygon", coordinates: c }, { fill: "Red", fillOpacity: 0.4, stroke: "Red" });
      }
    }

    function initialize() {
      if (!$mapOverview) {
        $mapOverview = $("#mapOverview").geomap({
          zoom: 0,
          bboxMax: fullExtent,
          pannable: false,
          services: [
            {
              type: "shingled",
              src: loadOverviewMap
            }
          ],
          drawStyle: { strokeWidth: "2px", stroke: "Gray", fill: "White", fillOpacity: 0.5 },
          tilingScheme: null,
          mode: $("#optPan").hasClass("Selected") ? "static" : "dragBox",
          shape: mapShape
        })
        .on("touchstart mousedown", dragExtentStart)
        .on("touchmove mousemove", dragExtent)
        .on("touchend touchcancel mouseup touchleave mouseleave", dragExtentEnd);

        var handle = setInterval(function () {
          if (gpv.viewer) {
            clearInterval(handle);
            drawMainExtent(gpv.viewer.getExtent());
          }
        }, 10);
      }
    }

    function loadOverviewMap(view) {
      var viewWidth = parseInt(view.width, 10);
      var viewHeight = parseInt(view.height, 10);

      if (viewWidth == lastWidth && viewHeight == lastHeight) {
        return lastUrl;
      } else {
        lastWidth = viewWidth;
        lastHeight = viewHeight;
        lastUrl = "Services/MapImage.ashx?" + $.param({
          m: "GetOverviewImage",
          application: appState.Application,
          width: viewWidth,
          height: viewHeight,
          bbox: view.bbox.toString()
        });

        return lastUrl;
      }
    }

    function mapShape(e, geo) {
      var bbox = geo.bbox;

      if ($.geo.width(bbox) == 0 && $.geo.height(bbox) == 0) {
        bbox = $.geo.recenter(gpv.viewer.getExtent(), $.geo.center(bbox));
      }

      gpv.viewer.setExtent(bbox);
    }

    function post(args) {
      args.url = service;
      gpv.post(args);
    }

    function showCount($tr, c) {
      $tr.removeClass("NoSelection").show().find("td.Value").text(c);
    }

    function showZoneLevelCounts(result) {
      $ZoneLevelTable.find("tr").addClass("NoSelection").toggle($("#optZoneLevelAll:checked").length == 1);

      var zone = {};
      var level = {};

      $.each(result.rows, function () {
        if (this.id.z) {
          if (this.id.z in zone) {
            zone[this.id.z].count += 1;
          }
          else {
            zone[this.id.z] = { count: 1, level: {} };
          }

          if (this.id.l) {
            var zoneLevel = zone[this.id.z].level;

            if (this.id.l in zoneLevel) {
              zoneLevel[this.id.l] += 1;
            }
            else {
              zoneLevel[this.id.l] = 1;
            }
          }
        }

        if (this.id.l) {
          if (this.id.l in level) {
            level[this.id.l] += 1;
          }
          else {
            level[this.id.l] = 1;
          }
        }
      });

      var $zoneRows = $ZoneLevelTable.find("tr.Zone");
      var $zoneLevelRows = $ZoneLevelTable.find("tr.ZoneLevel");

      $.each(zone, function (z, v) {
        showCount($zoneRows.filter("tr[data-zone='" + z + "']"), v.count);

        $.each(v.level, function (lev, c) {
          showCount($zoneLevelRows.filter("tr[data-zone='" + z + "'][data-level='" + lev + "']"), c);
        });
      });

      var $levelRows = $ZoneLevelTable.find("tr.Level");

      $.each(level, function (lev, c) {
        showCount($levelRows.filter("tr[data-level='" + lev + "']"), c);
      });
    }

    function refreshOverviewMap(name) {
      if (!name || name == "Location") {
        if (!$mapOverview) {
          initialize();
        } else {
          $mapOverview.geomap("resize");
        }
      }
    }

    function zoomToZone(zone, level) {
      var data = {
        m: "GetZoneExtent",
        maptab: appState.MapTab,
        zone: zone
      };

      post({
        data: data,
        success: function (result) {
          if (result) {
            if (level) {
              appState.Level = level;
            }

            gpv.viewer.setExtent(result.extent);
          }
        }
      });
    }

    // =====  public interface  =====

    gpv.locationPanel = {
    };

    // =====  finish initialization  =====

    if ($("#tabLocation").hasClass("Selected")) {
      initialize();
    }
  });

  return gpv;
})(GPV || {});
