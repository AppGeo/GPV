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
    var metersPerFoot = 0.3048;
    var zoomFactor = 1.414213562373095;
    var appState = gpv.appState;

    var $map = $("#mapMain");

    var fullExtent = gpv.configuration.fullExtent;
    var previousExtents = [];
    var zoomPreviousClicked = false;
    var defaultDrawStyle = { width: "0px", height: "0px", strokeWidth: "2px", stroke: "Gray", fill: "White", fillOpacity: 0.5 };

    var mapTabChangedHandlers = [];
    var functionTabChangedHandlers = [];
    var extentChangedHandlers = [];
    var mapShapeHandlers = [];

    // =====  controls required prior to map control creation  =====

    var $tboScale = $("#tboScale");

    if ($tboScale.val()) {
      var initialExtent = appState.Extent.bbox;
      initialExtent = $.geo.scaleBy(initialExtent, parseInt($tboScale.val(), 10) / getScaleFor(initialExtent));
      appState.Extent.bbox = initialExtent;
    }

    var $ddlExternalMap = $("#ddlExternalMap").on("change", setExternalMap);

    // =====  map control  =====

    $.geo.length = function (geo) {
      var c = geo.coordinates;
      var d = 0, dx, dy;

      for (var i = 1; i < c.length; ++i) {
        dx = c[i][0] - c[i - 1][0];
        dy = c[i][1] - c[i - 1][1];
        d += Math.sqrt(dx * dx + dy * dy)
      };

      var t = [];

      if (gpv.settings.measureUnits != "meters") {
        var ft = d / (gpv.settings.mapUnits == "feet" ? 1 : metersPerFoot);
        t.push(d < 5280 ? ft.format() + " ft" : (ft / 5280).format(1) + " mi");
      }

      if (gpv.settings.measureUnits != "feet") {
        var m = d * (gpv.settings.mapUnits == "feet" ? metersPerFoot : 1);
        t.push(m < 1000 ? m.format() + " m" : (m / 1000).format(1) + " km");
      }

      return t.join("<br/>");
    };

    $.geo.area = function (shape) {
      var c = shape.coordinates[0];
      var a = 0;

      if (c.length > 2) {
        var oy = c[0][1] * 2;

        for (var i = 1; i < c.length; ++i) {
          a += (c[i][0] - c[i - 1][0]) * (c[i][1] + c[i - 1][1] - oy);
        }
      }

      if (a != 0) {
        a = Math.abs(a * 0.5);
      }

      var t = [];
      var acres;

      if (gpv.settings.measureUnits != "meters") {
        var u = gpv.settings.mapUnits == "feet" ? 1 : metersPerFoot;
        var sf = a / (u * u);
        t.push(sf <= 27878400 ? sf.format() + " sq ft" : (sf / 27878400).format(2) + " sq mi");
        acres = sf / 43560;
      }

      if (gpv.settings.measureUnits != "feet") {
        var u = gpv.settings.mapUnits == "feet" ? metersPerFoot : 1;
        var sm = a * (u * u);
        t.push(sm <= 1000000 ? sm.format() + " sq m" : (sm / 1000000).format(2) + " sq km");
      }

      if (gpv.settings.measureUnits != "meters") {
        t.push(acres.format(2) + " acres");
      }

      return t.join("<br/>");
    };

    $map.geomap({
      bbox: appState.Extent.bbox,
      bboxMax: fullExtent,
      services: [
        {
          type: "shingled",
          src: refreshMap
        }
      ],
      measureLabels: {
        length: "{{:length!}}",
        area: "{{:area!}}"
      },
      drawStyle: defaultDrawStyle,
      tilingScheme: null,
      shape: mapShape,
      loadstart: gpv.waitClock.start,
      loadend: gpv.waitClock.finish
    });

    // =====  control events  =====

    $("#cmdClearGraphics").on("click", function () {
      appState.Coordinates = [];
      appState.CoordinateLabels = [];
      $map.geomap("refresh");
    });

    $("#cmdEmail").on("click", function () {
      gpv.post({
        url: "Services/SaveAppState.ashx",
        data: {
          state: appState.toJson()
        },
        success: function (result) {
          if (result && result.id) {
            var loc = document.location;
            var url = [loc.protocol, "//", loc.hostname, loc.port.length && loc.port != "80" ? ":" + loc.port : "", loc.pathname, "?state=", result.id];
            loc.href = "mailto:?subject=Map&body=" + encodeURIComponent(url.join(""));
          }
        }
      });
    });

    $("#cmdFullView").on("click", function () {
      $map.geomap("option", "bbox", fullExtent);
    });

    $("#cmdHelp").on("click", function () {
    });

    $("#cmdMobile").on("click", function () {
      window.location.href = "MobileViewer.aspx?application=" + appState.Application + "&maptab=" + appState.MapTab + "&extent=" + appState.Extent.bbox.join(",");
    });

    $("#cmdPrint").on("click", function () {
      var $form = $("#frmPrint");
      $form.find('[name="state"]').val(appState.toJson());
      $form.find('[name="width"]').val($map.width());
      $form.submit();
    });

    $("#cmdSaveMap").on("click", function () {
      var $form = $("#frmSaveMap");
      $form.find('[name="m"]').val($("#ddlSaveMap").val() == "image" ? "SaveMapImage" : "SaveMapKml");
      $form.find('[name="state"]').val(appState.toJson());
      $form.find('[name="width"]').val($map.width());
      $form.find('[name="height"]').val($map.height());
      $form.submit();
    });

    $("#cmdZoomPrevious").on("click", function () {
      if (previousExtents.length) {
        zoomPreviousClicked = true;
        $map.geomap("option", "bbox", previousExtents.pop());
      }
    });

    $("#cmdZoomSelect").on("click", function () {
      zoomToSelection(1.6);
    });

    var $ddlLevel = $("#ddlLevel").on("change", function () {
      appState.Level = $(this).val();
      $map.geomap("refresh");
    });

    $("#pnlMapTabs .Tab").on("click", function () {
      var mapTab = $(this).attr("data-maptab");
      appState.MapTab = mapTab;
      triggerMapTabChanged();
      $map.geomap("refresh");
    });

    $("#pnlFunctionTabs .Tab").on("click", function () {
      $(".FunctionPanel").hide();
      var name = $(this).text();
      $("#pnl" + name).show();

      $.each(functionTabChangedHandlers, function () {
        this(name);
      });
    });

    $tboScale.on("keydown", function (e) {
      if (e.keyCode == 13) {
        setMapScale();
      };
    }).on("keypress", function (e) {
      var keyCode = e.keyCode ? e.keyCode : e.charCode;

      if (!(48 <= keyCode && keyCode <= 57)) {
        return false;
      }
    }).on("blur", setMapScale);

    // =====  map tools  =====

    var $MapTool = $(".MapTool");

    $("#optCoordinates").on("click", function () {
      gpv.selectTool($(this), { mode: "drawPoint", pannable: false, drawStyle: { strokeWidth: "0px"} }, "crosshair");
    });

    $("#optIdentify").on("click", function () {
      gpv.selectTool($(this), { mode: "drawPoint", pannable: false, drawStyle: { strokeWidth: "0px"} }, "default");
    });

    $("#optMeasureArea").on("click", function () {
      gpv.selectTool($(this), { mode: "measureArea", pannable: false, drawStyle: { stroke: "Green", fill: "#C0FFC0", fillOpacity: 0.75} });
    });

    $("#optMeasureLine").on("click", function () {
      gpv.selectTool($(this), { mode: "measureLength", pannable: false, drawStyle: { stroke: "Green"} });
    });

    $("#optPan").on("click", function () {
      gpv.selectTool($(this), { mode: "pan", pannable: true, drawStyle: defaultDrawStyle });
    });

    $("#optZoomIn").on("click", function () {
      gpv.selectTool($(this), { mode: "zoom", pannable: false, drawStyle: defaultDrawStyle });
    });

    // =====  component events  =====

    gpv.on("resize", "mapResized", function () {
      $map.geomap("resize");
    });

    gpv.on("selection", "changed", function (truncated, scaleBy) {
      if (scaleBy) {
        zoomToSelection(scaleBy);
      }
      else {
        $map.geomap("refresh");
      }
    });

    gpv.on("zoomBar", "levelChanged", function (level) {
      var extent = reaspectExtent(fullExtent);
      extent = $.geo.scaleBy(extent, 1 / Math.pow(zoomFactor, level - 1));
      extent = recenterExtent(extent, $.geo.center(appState.Extent.bbox));
      $map.geomap("option", "bbox", extent);
    });

    // =====  private functions  =====

    function getScaleFor(extent) {
      var scale = $.geo.width(reaspectExtent(extent)) * 96 / $map.width();
      return gpv.settings.mapUnits == "meters" ? scale /= metersPerFoot : scale;
    }

    function mapShape(e, geo) {
      switch ($MapTool.filter(".Selected").attr("id")) {
        case "optCoordinates":
          appState.Coordinates.push({ coordinates: geo.coordinates });
          appState.CoordinateLabels.push("1");
          $map.geomap("refresh");
          return;

        case "optIdentify":
          var data = ["maptab=", appState.MapTab, "&visiblelayers=", encodeURIComponent(gpv.legendPanel.getVisibleLayers(appState.MapTab).join("\x01")),
            "&level=", appState.Level, "&x=", geo.coordinates[0], "&y=", geo.coordinates[1], "&distance=4",
            "&scale=", $map.geomap("option", "pixelSize")].join("");

          var windowName = "identify";
          var settings = gpv.settings;

          if (settings.identifyPopup == "multiple") {
            windowName += (new Date()).getTime();
          }

          var features = "width=" + settings.identifyWindowWidth + ",height=" + settings.identifyWindowHeight + ",menubar=no,titlebar=no,toolbar=no,status=no,scrollbars=yes,location=yes,resizable=yes";
          window.open("Identify.aspx?" + data, windowName, features, true);
          return;
      }

      $.each(mapShapeHandlers, function () {
        this(e, geo);
      });
    }

    function reaspectExtent(extent, ratio) {
      if (!ratio) {
        ratio = $map.width() / $map.height();
      }

      return $.geo.reaspect(extent, ratio);
    }

    function recenterExtent(extent, p) {
      var c = $.geo.center(extent);
      var dx = p[0] - c[0];
      var dy = p[1] - c[1];
      return [extent[0] + dx, extent[1] + dy, extent[2] + dx, extent[3] + dy];
    }

    function refreshMap(view) {
      var same = sameBox(appState.Extent.bbox, view.bbox);
      appState.Extent.bbox = view.bbox;
      setZoomBarLevel();
      showMapScale();
      setExternalMap();

      if (!same) {
        if (!zoomPreviousClicked) {
          previousExtents.push(appState.Extent.bbox);
        }

        $.each(extentChangedHandlers, function () {
          this(view.bbox);
        });
      }

      zoomPreviousClicked = false;
      appState.VisibleLayers[appState.MapTab] = gpv.legendPanel.getVisibleLayers(appState.MapTab);

      return gpv.post({
        url: "Services/MapImage.ashx",
        data: {
          m: "MakeMapImage",
          state: appState.toJson(),
          width: parseInt(view.width, 10),
          height: parseInt(view.height, 10)
        }
      });
    }

    function sameBox(a, b) {
      return a[0] == b[0] && a[1] == b[1] && a[2] == b[2] && a[3] == b[3];
    }

    function setExternalMap() {
      var externalName = $ddlExternalMap.val();
      var cmd = $("#cmdExternalMap");
      var extent = reaspectExtent(appState.Extent.bbox);

      if (!externalName || !extent) {
        cmd.attr("href", "#").addClass("Disabled");
        return;
      }

      gpv.post({
        url: "Services/ExternalMap.ashx",
        data: {
          name: externalName,
          minx: extent[0],
          miny: extent[1],
          maxx: extent[2],
          maxy: extent[3],
          pixelSize: $map.geomap("option", "pixelSize")
        },
        success: function (result) {
          if (result && result.url) {
            cmd.attr("href", result.url).removeClass("Disabled");
          }
        }
      });
    }

    function setMapScale() {
      var currentScale = getScaleFor(appState.Extent.bbox)
      var newScale = parseInt($tboScale.val(), 10);

      if (!newScale) {
        $tboScale.val(currentScale);
      }
      else {
        $map.geomap("option", "bbox", $.geo.scaleBy(appState.Extent.bbox, newScale / currentScale));
      }
    }

    function setZoomBarLevel() {
      var ratio = $map.width() / $map.height();
      var extent = reaspectExtent(appState.Extent.bbox, ratio);
      var extentMax = reaspectExtent(fullExtent, ratio);
      var level = Math.round(Math.log($.geo.width(extentMax) / $.geo.width(extent)) / Math.log(zoomFactor)) + 1;
      gpv.zoomBar.setLevel(level);
    }

    function showMapScale() {
      var scale = Math.round(getScaleFor(appState.Extent.bbox))
      $tboScale.val(scale);
      $("#scaleBarText").text(scale + " ft");
    }


    function triggerMapTabChanged() {
      $.each(mapTabChangedHandlers, function () {
        this();
      });
    }

    function zoomToActive() {
      gpv.selection.getActiveExtent(function (bbox) {
        if (bbox) {
          $map.geomap("option", "bbox", $.geo.scaleBy(bbox, 1.6));
        }
      });
    }

    function zoomToSelection(scaleBy) {
      gpv.selection.getSelectionExtent(function (bbox) {
        if (bbox) {
          $map.geomap("option", "bbox", $.geo.scaleBy(bbox, scaleBy));
        }
      });
    }

    // =====  public interface  =====

    gpv.viewer = {
      extentChanged: function (fn) { extentChangedHandlers.push(fn); },
      getExtent: function () { return $map.geomap("option", "bbox"); },
      functionTabChanged: function (fn) { functionTabChangedHandlers.push(fn); },
      mapShape: function (fn) { mapShapeHandlers.push(fn); },
      mapTabChanged: function (fn) { mapTabChangedHandlers.push(fn); },
      refreshMap: function () { $ddlLevel.val(appState.Level); $map.geomap("refresh"); },
      setExtent: function (extent) { $ddlLevel.val(appState.Level); return $map.geomap("option", "bbox", extent); },
      zoomToActive: zoomToActive
    };

    // =====  finish initialization  =====

    gpv.loadComplete();

    $MapTool.filter(".Selected").trigger("click");
    triggerMapTabChanged();
  });

  return gpv;
})(GPV || {});