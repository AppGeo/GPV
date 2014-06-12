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
    var $map = $("#mapMain");
    var $container = $("#pnlMarkup");
    var config = gpv.configuration;
    var appState = gpv.appState;
    var isPublic = gpv.settings.isPublic;
    var service = "Services/MarkupPanel.ashx";

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
      gpv.selectTool($(this), { mode: "drawPoint", pannable: false, drawStyle: { strokeWidth: "0px"} }, "default");
    });

    $("#optDrawArea").on("click", function () {
      var color = getMarkupColor();
      gpv.selectTool($(this), { mode: "measureArea", pannable: false, drawStyle: { strokeWidth: "2px", stroke: color, fill: color} });
    });

    $("#optDrawCircle").on("click", function () {
      var color = getMarkupColor();
      gpv.selectTool($(this), { mode: "dragCircle", pannable: false, drawStyle: { strokeWidth: "2px", stroke: color, fill: color} });
    });

    $("#optDrawPoint,#optDrawCoordinates,#optDrawText").on("click", function () {
      gpv.selectTool($(this), { mode: "drawPoint", pannable: false, drawStyle: { strokeWidth: "0px"} }, "crosshair");
    });

    $("#optDrawLength").on("click", function () {
      gpv.selectTool($(this), { mode: "measureLength", pannable: false, drawStyle: { strokeWidth: "2px", stroke: getMarkupColor()} });
    });

    $("#optDrawLine").on("click", function () {
      gpv.selectTool($(this), { mode: "drawLineString", pannable: false, drawStyle: { strokeWidth: "2px", stroke: getMarkupColor()} });
    });

    $("#optDrawPolygon").on("click", function () {
      var color = getMarkupColor();
      gpv.selectTool($(this), { mode: "drawPolygon", pannable: false, drawStyle: { strokeWidth: "2px", stroke: color, fill: color} });
    });

    // =====  component events  =====

    gpv.on("viewer", "mapShape", mapShape);

    // =====  private functions  =====

    function addMarkup(geo, option) {
      var data = {
        m: "AddMarkup",
        id: appState.MarkupGroups[0],
        shape: toWkt(geo),
        color: getMarkupColor()
      };

      if (option == "text") {
        data.text = $("#tboMarkupText").val();
      }

      if (option == "measured") {
        data.measured = 1;
      }

      if ((option == "text" || (option == "measured" && geo.type == "Point")) && $chkTextGlow.attr("checked")) {
        data.glow = $cmdTextGlowColor.colorSelector("color");
      }

      post({
        data: data,
        success: function (result) {
          if (result) {
            $map.geomap("refresh");
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

    function deleteMarkup(geo) {
      post({
        data: {
          m: "DeleteMarkup",
          id: appState.MarkupGroups[0],
          x: geo.coordinates[0],
          y: geo.coordinates[1],
          distance: 4,
          scale: $map.geomap("option", "pixelSize")
        },
        success: function (result) {
          if (result) {
            $map.geomap("refresh");
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
        var $tools = $container.find(".Button.MapTool");

        if (!enable && $tools.filter(".Selected").removeClass("Selected").length) {
          $("#optZoomIn").addClass("Selected");
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

    function floodColors(geo) {
      var data = {
        m: "FillWithColor",
        id: appState.MarkupGroups[0],
        x: geo.coordinates[0],
        y: geo.coordinates[1],
        distance: 4,
        scale: $map.geomap("option", "pixelSize"),
        color: getMarkupColor()
      };

      if ($chkTextGlow.attr("checked")) {
        data.glow = $cmdTextGlowColor.colorSelector("color");
      }

      post({
        data: data,
        success: function (result) {
          if (result) {
            $map.geomap("refresh");
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

    function mapShape(e, geo) {
      switch ($MapTool.filter(".Selected").attr("id")) {
        case "optDrawCircle":
          if ($.geo.width(geo.bbox) > 0) {
            addMarkup(geo);
          }
          return;

        case "optDrawPoint":
        case "optDrawLine":
        case "optDrawPolygon":
          addMarkup(geo);
          return;

        case "optDrawCoordinates":
        case "optDrawLength":
        case "optDrawArea":
          addMarkup(geo, "measured");
          return;

        case "optDeleteMarkup": deleteMarkup(geo); return;
        case "optColorPicker": pickColors(geo); return;
        case "optPaintBucket": floodColors(geo); return;
        case "optDrawText": addMarkup(geo, "text"); return;
      }
    }

    function pickColors(geo) {
      post({
        data: {
          m: "PickColors",
          id: appState.MarkupGroups[0],
          x: geo.coordinates[0],
          y: geo.coordinates[1],
          distance: 4,
          scale: $map.geomap("option", "pixelSize")
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
      if ($container.find(".Button.MapTool.Selected").length) {
        var drawStyle = $map.geomap("option", "drawStyle");
        drawStyle.color = c;
        $map.geomap("option", "drawStyle", drawStyle);
      }
    }

    function toWkt(geometry) {
      switch (geometry.type) {
        case "Point": return "POINT(" + toWktCoordinates([geometry.coordinates]) + ")";
        case "LineString": return "LINESTRING(" + toWktCoordinates(geometry.coordinates) + ")";
        case "Polygon": return "POLYGON((" + toWktCoordinates(geometry.coordinates[0]) + "))";
      }
    }

    function toWktCoordinates(coords) {
      var prec = Math.log($map.geomap("option", "pixelSize")) / Math.LN10;
      prec = prec >= 0 ? 0 : 0 - Math.floor(prec);
      var c = [];

      for (var i = 0; i < coords.length; ++i) {
        c.push(coords[i][0].toFixed(prec) + " " + coords[i][1].toFixed(prec));
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
    };

    // =====  finish initialization  =====

    fillGrid(true);
    enableControls();
  });

  return gpv;
})(GPV || {});
