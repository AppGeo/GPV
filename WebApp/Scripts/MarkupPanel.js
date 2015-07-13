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
    var map, shingleLayer, currentShape;

    var $container = $("#pnlMarkup");
    var config = gpv.configuration;
    var appState = gpv.appState;
    var isPublic = gpv.settings.isPublic;
    var service = "Services/MarkupPanel.ashx";

    var metersPerFoot = 0.3048;
    var feetPerMile = 5280;
    var squareFeetPerAcre = 43560;

    var measureCoordinates;
    var measureText;

    // =====  controls  =====

    var $colorSelectors = $(".Color").colorSelector({
      enabled: false,
      selectorClass: "ColorSelector",
      disabledClass: "Disabled",
      commandClass: "CommandLink"
    });

    var $chkMarkupLock = $("#chkMarkupLock").on("click", lockMarkupGroup);
    var $chkTextGlow = $("#chkTextGlow");

    $("#cmdDeleteMarkup").on("click", function () {
      if (!$(this).hasClass("Disabled")) {
        deleteMarkupGroup();
      }
    });

    $("#cmdExportMarkup").on("click", function () {
      if (!$(this).hasClass("Disabled")) {
        window.location.href = "ExportMarkup.ashx?app=" + appState.Application + "&group=" + appState.MarkupGroups[0];
      }
    });

    var $cmdNewMarkup = $("#cmdNewMarkup").on("click", function () {
      if (!$(this).hasClass("Disabled")) {
        createMarkupGroup();
      }
    });

    $("#cmdZoomToMarkup").on("click", function () {
      if (!$(this).hasClass("Disabled")) {
        zoomToMarkupGroup();
      }
    });

    var $cmdMarkupColor = $("#cmdMarkupColor").colorSelector("color", "#FF0000").colorSelector("colorChanged", setDrawingColor);
    var $cmdTextGlowColor = $("#cmdTextGlowColor").colorSelector("color", "#FFFFFF");

    $('#btnMarkupColor').on('click', function () {
      $("#cmdMarkupColor").trigger('touchstart');
    });

    $('#btnTextGlowColor').on('click', function () {
      $("#cmdTextGlowColor").trigger('touchstart');
    });

    var $ddlMarkupCategory = $("#ddlMarkupCategory").change(function () {
      if (appState.MarkupGroups.length > 0) {
        appState.MarkupGroups = [];
        $tboMarkupTitle.val("");
        $chkMarkupLock.attr("checked", false);
        enableControls();
        gpv.viewer.refreshMap();
      }

      appState.MarkupCategory = $ddlMarkupCategory.val();
      fillGrid(false);
    });

    var $grdMarkup = $("#grdMarkup").dataGrid({
      multiSelect: true,
      rowClass: "DataGridRow",
      alternateClass: "DataGridRowAlternate",
      selectedClass: "DataGridRowSelect",
      selectionChanged: selectionChanged
    });

    var $tboMarkupTitle = $("#tboMarkupTitle").on("keydown", function (e) {
      if (e.keyCode == 13) {
        updateMarkupGroupTitle();
      }
    }).on("blur", updateMarkupGroupTitle);

    var $tboMarkupUser = $("#tboMarkupUser").on("keyup", function () {
      var name = $(this).val();
      gpv.store("markupUser", name);
      $cmdNewMarkup.toggleClass("Disabled", name.length == 0);
    });

    // =====  map tools  =====

    var $MapTool = $(".MapTool");

    $("#optDeleteMarkup,#optColorPicker,#optPaintBucket").on("click", function () {
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "point" } });
    });

    $("#optDrawArea").on("click", function () {
      var c = getMarkupColor();
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "polygon", style: { color: c, fill: true, fillColor: c } } });
    });

    $("#optDrawCircle").on("click", function () {
      var c = getMarkupColor();
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "circle", style: { color: c, fill: true, fillColor: c } } });
    });

    $("#optDrawPoint,#optDrawCoordinates").on("click", function () {
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "point" } });
    });

    $("#optDrawText").on("click", function () {
      var c = getMarkupColor();
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "text", text: { color: c } } });
    });

    $("#optDrawLength").on("click", function () {
      var c = getMarkupColor();
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "polyline", style: { color: c, fill: false } } });
    });

    $("#optDrawLine").on("click", function () {
      var c = getMarkupColor();
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "polyline", style: { color: c, fill: false } } });
    });

    $("#optDrawPolygon").on("click", function () {
      var c = getMarkupColor();
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: "polygon", style: { color: c, fill: true, fillColor: c } } });
    });

    // =====  private functions  =====

    function addMarkup(e, option) {
      currentShape = e.shape;

      var data = {
        m: "AddMarkup",
        id: appState.MarkupGroups[0],
        shape: toWkt(currentShape),
        color: getMarkupColor()
      };

      if (option == "text") {
        data.text = e.shape.options.value;
      }

      if (option == "measured") {
        data.measured = 1;
      }

      if ((option == "text" || (option == "measured" && currentShape instanceof L.LatLng)) && $chkTextGlow.prop("checked")) {
        data.glow = $cmdTextGlowColor.colorSelector("color");
      }

      post({
        data: data,
        success: function (result) {
          if (result) {
            gpv.viewer.refreshMap();
          }
        }
      });
    }

    function createMarkupGroup() {
      post({
        data: {
          m: "CreateMarkupGroup",
          category: appState.MarkupCategory,
          user: $tboMarkupUser.val()
        },
        success: function (result) {
          if (result) {
            appState.MarkupGroups = [result.id];
            $tboMarkupTitle.val(result.title);
            $chkMarkupLock.attr("checked", result.locked);
            enableControls();
            fillGrid();
            gpv.viewer.refreshMap();
          }
        }
      });
    }

    function deleteMarkup(e) {
      var p = map.options.crs.project(e.shape);

      post({
        data: {
          m: "DeleteMarkup",
          id: appState.MarkupGroups[0],
          x: p.x,
          y: p.y,
          distance: 4,
          scale: map.getProjectedPixelSize()
        },
        success: function (result) {
          if (result) {
            gpv.viewer.refreshMap();
          }
        }
      });
    }

    function deleteMarkupGroup() {
      post({
        data: {
          m: "DeleteMarkupGroup",
          id: appState.MarkupGroups[0]
        },
        success: function (result) {
          if (result) {
            appState.MarkupGroups = [];
            $tboMarkupTitle.val("");
            enableControls();
            $grdMarkup.dataGrid("deleteSelection");
            gpv.viewer.refreshMap();
          }
        }
      });
    }

    function enableControls() {
      var enable = appState.MarkupGroups.length == 1;

      if (!enable) {
        complete();
      }
      else {
        post({
          data: {
            m: "GetMarkupGroupPermissions",
            id: appState.MarkupGroups[0]
          },
          success: function (result) {
            if (result) {
              enable = result.canEdit;
              complete(result.canLock, result.isLocked);
            }
          }
        });
      }

      function complete(canLock, isLocked) {
        var $tools = $(".Button.MarkupTool");

        if (!enable && $tools.filter(".Selected").removeClass("Selected").length) {
          $("#optPan").addClass("Selected");
        }

        if (!isPublic) {
          $chkMarkupLock.attr("disabled", !canLock).attr("checked", isLocked);
        }

        $container.find(".Toggleable").add($tools).toggleClass("Disabled", !enable);
        $tboMarkupTitle.add("#tboMarkupText").attr("disabled", !enable);
        $colorSelectors.colorSelector("enabled", enable);
      }
    }

    function fillGrid() {
      if (!appState.MarkupCategory) {
        $grdMarkup.dataGrid("empty");
      }
      else {
        post({
          data: {
            m: "GetMarkupGroupData",
            category: appState.MarkupCategory
          },
          success: function (result) {
            if (result) {
              $grdMarkup.dataGrid("load", result);

              if (appState.MarkupGroups.length == 1) {
                $grdMarkup.dataGrid("setSelection", function (id) {
                  return id == appState.MarkupGroups[0];
                });
              }
            }
          }
        });
      }
    }

    function floodColors(e) {
      var p = map.options.crs.project(e.shape);

      var data = {
        m: "FillWithColor",
        id: appState.MarkupGroups[0],
        x: p.x,
        y: p.y,
        distance: 4,
        scale: map.getProjectedPixelSize(),
        color: getMarkupColor()
      };

      if ($chkTextGlow.attr("checked")) {
        data.glow = $cmdTextGlowColor.colorSelector("color");
      }

      post({
        data: data,
        success: function (result) {
          if (result) {
            gpv.viewer.refreshMap();
          }
        }
      });
    }

    function getMarkupColor() {
      return $cmdMarkupColor.colorSelector("color");
    }

    function lockMarkupGroup() {
      var data = {
        m: "LockMarkupGroup",
        id: appState.MarkupGroups[0],
        locked: $chkMarkupLock.attr("checked") == "checked"
      };

      post({
        data: data
      });
    }

    function mapShape(e) {
      if (measureCoordinates) {
        measureCoordinates = undefined;
      }

      if (measureText) {
        map.removeLayer(measureText);
        measureText = undefined;
      }

      switch ($MapTool.filter(".Selected").attr("id")) {
        case "optDrawCircle":
        case "optDrawPoint":
        case "optDrawLine":
        case "optDrawPolygon":
          addMarkup(e);
          return;

        case "optDrawCoordinates":
        case "optDrawLength":
        case "optDrawArea":
          addMarkup(e, "measured");
          return;

        case "optDeleteMarkup": deleteMarkup(e); return;
        case "optColorPicker": pickColors(e); return;
        case "optPaintBucket": floodColors(e); return;
        case "optDrawText": addMarkup(e, "text"); return;
      }
    }

    function pickColors(e) {
      var p = map.options.crs.project(e.shape);

      post({
        data: {
          m: "PickColors",
          id: appState.MarkupGroups[0],
          x: p.x,
          y: p.y,
          distance: 4,
          scale: map.getProjectedPixelSize()
        },
        success: function (result) {
          if (result && result.color) {
            $cmdMarkupColor.colorSelector("color", result.color);

            if (result.glow) {
              $chkTextGlow.attr("checked", true);
              $cmdTextGlowColor.colorSelector("color", result.glow);
            }
            else {
              $chkTextGlow.attr("checked", false);
            }
          }
        }
      });
    }

    function post(args) {
      args.url = service;
      gpv.post(args);
    }

    function selectionChanged() {
      appState.MarkupGroups = $grdMarkup.dataGrid("getSelection");
      var numGroups = appState.MarkupGroups.length;
      var markupTitle = numGroups == 1 ? $grdMarkup.dataGrid("getData", appState.MarkupGroups[0])[2] : "";

      $tboMarkupTitle.val(markupTitle);
      enableControls();

      if (numGroups) {
        zoomToMarkupGroup();
      }
      else {
        gpv.viewer.refreshMap();
      }
    }

    function setDrawingColor(c) {
      $.extend(true, map.options.drawing, { style: { color: c, fillColor: c }, text: { color: c } });
    }

    function setMap(m) {
      map = m;
      map.on("shapedrawing", shapeDrawing);
      map.on("shapedrawn", mapShape);

      map.eachLayer(function (layer) {
        if (!shingleLayer) {
          shingleLayer = layer;
        }
      });

      shingleLayer.on("shingleload", function () {
        if (currentShape) {
          map.removeLayer(currentShape);
          currentShape = null;
        }
      });
    }


    function shapeDrawing(e) {
      var currentTool = $MapTool.filter(".Selected").attr("id");

      if (currentTool === 'optDrawLength' || currentTool === 'optDrawArea') {
        var units = gpv.settings.measureUnits;
        var inFeet = units == "feet" || units == "both";
        var inMeters = units == "meters" || units == "both";
        var convert = 1 / (gpv.settings.mapUnits == "feet" ? 1 : metersPerFoot);

        if (!measureCoordinates) {
          measureCoordinates = [];
        }
        
        var c = measureCoordinates;

        var latlngs = e.shape.getLatLngs();
        var lastLatLng = latlngs[latlngs.length - 1];
        c.push(map.options.crs.project(lastLatLng));

        if (measureText) {
          map.removeLayer(measureText);
        }

        var value = [];
        var i, j;

        if (currentTool === 'optDrawLength') {
          if (c.length >= 2) {
            var length = 0;

            for (i = 1; i < c.length; ++i) {
              var dx = c[i].x - c[i - 1].x;
              var dy = c[i].y - c[i - 1].y;
              length += Math.sqrt(dx * dx + dy * dy);
            }

            if (length > 0) {
              length *= convert;

              if (inFeet) {
                value.push(length < feetPerMile ? Math.round(length) + " ft" : (length / feetPerMile).toFixed(1) + " mi");
              }

              if (inMeters) {
                length *= metersPerFoot;
                value.push(length < 1000 ? Math.round(length) + " m" : (length / 1000).toFixed(1) + " km");
              }

              measureText = L.text({ 
                latlng: lastLatLng,
                className: "MeasureText",
                value: value.join("\n")
              }).addTo(map);
            }
          }
        }
        else {
          if (c.length >= 3) {
            var x = 0;
            var y = 0;
            var area = 0;
            var n;

            for (i = 1; i <= c.length; ++i) {
              j = i % c.length;
              n = (c[i - 1].x * c[j].y) - (c[j].x * c[i - 1].y);
              x += (c[i - 1].x + c[j].x) * n;
              y += (c[i - 1].y + c[j].y) * n;
              area += n;
            }

            x /= 3 * area;
            y /= 3 * area;

            area = Math.abs(area * 0.5);

            if (area > 0) {
              area *= convert * convert;
              var acres = area / squareFeetPerAcre;

              if (inFeet) {
                var squareMile = feetPerMile * feetPerMile;
                value.push(area <= squareMile ? Math.round(area) + " sq ft" : (area / squareMile).toFixed(2) + " sq mi");
              }

              if (inMeters) {
                area *= metersPerFoot * metersPerFoot;
                value.push(area <= 100000 ? Math.round(area) + " sq m" : (area / 1000000).toFixed(2) + " sq km");
              }

              if (inFeet) {
                value.push(acres.toFixed(2) + " acres");
              }

              measureText = L.text({ 
                latlng: map.options.crs.unproject(L.point(x, y)),
                className: "MeasureText " + (inFeet ? "Area3" : "Area2"),
                value: value.join("\n")
              }).addTo(map);
            }
          }
        }
      }
    }

    function toWkt(shape) {
      var points;

      if (shape instanceof L.Text) {
        shape = shape.options.latlng;
      }

      if (shape instanceof L.Polyline) {
        points = shape.getLatLngs().map(function (latlng) {
          return map.options.crs.project(latlng);
        });

        if (shape instanceof L.Polygon) {
          points.push(points[0]);
          return "POLYGON((" + toWktCoordinates(points) + "))";
        }
        else {
          return "LINESTRING(" + toWktCoordinates(points) + ")";
        }
      }
      else if (shape instanceof L.LatLng) {
        return "POINT(" + toWktCoordinates([ map.options.crs.project(shape) ]) + ")";
      }
      else if (shape instanceof L.Circle) {
        var sweepAngle = 3;
        var segments = 360 / sweepAngle;
          
        sweepAngle *= Math.PI / 180;
        var cos = Math.cos(sweepAngle);
        var sin = Math.sin(sweepAngle);

        var center = map.options.crs.project(shape.getLatLng());
        var dx = 0;
        var dy = shape.getRadius();
        points = [ L.point(center.x + dx, center.y + dy) ];

        for (var i = 0; i < segments - 1; ++i) {
          var ndx = dx * cos + dy * sin;
          var ndy = dy * cos - dx * sin;
          dx = ndx;
          dy = ndy;
          points.push(L.point(center.x + dx, center.y + dy));
        }

        points.push(points[0]);
        return "POLYGON((" + toWktCoordinates(points) + "))";
      }
    }

    function toWktCoordinates(points) {
      var prec = Math.log(map.getProjectedPixelSize()) / Math.LN10;
      prec = prec >= 0 ? 0 : 0 - Math.floor(prec);
      var c = [];

      for (var i = 0; i < points.length; ++i) {
        c.push(points[i].x.toFixed(prec) + " " + points[i].y.toFixed(prec));
      }

      return c.join(",");
    }

    function updateMarkupGroupTitle() {
      if (appState.MarkupGroups.length == 1) {
        var id = appState.MarkupGroups[0];
        var title = $tboMarkupTitle.val();

        post({
          data: {
            m: "UpdateMarkupGroupTitle",
            id: id,
            title: title
          },
          success: function (result) {
            if (result) {
              var data = $grdMarkup.dataGrid("getData", id);
              data[2] = title;
              $grdMarkup.dataGrid("setData", id, data);
            }
          }
        });
      }
    }

    function zoomToMarkupGroup() {
      if (appState.MarkupGroups.length) {
        post({
          data: {
            m: "GetMarkupExtent",
            ids: appState.MarkupGroups.join(",")
          },
          success: function (result) {
            if (result && result.extent) {
              gpv.viewer.setExtent(result.extent);
            }
            else {
              gpv.viewer.refreshMap();
            }
          }
        });
      }
    }

    // =====  public interface  =====

    gpv.markupPanel = {
      setMap: setMap
    };

    // =====  finish initialization  =====

    fillGrid(true);
    enableControls();
  });

  return gpv;
})(GPV || {});
