﻿//  Copyright 2012 Applied Geographics, Inc.
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
    var $container = $("#pnlSelection");
    var $pnlDataDisplay = $("#pnlDataDisplay");
    var config = gpv.configuration;
    var appState = gpv.appState;
    var selection = gpv.selection;
    var service = "Services/SelectionPanel.ashx";

    var action = {
      select: 0,
      findAllWithin: 1,
      findNearest1: 2,
      findNearest2: 3,
      findNearest3: 4,
      findNearest4: 5,
      findNearest5: 6
    };

    var gridFilledHandlers = [];
    var reinitializedHandlers = [];

    // =====  controls  =====

    $("#cmdClearSelection").on("click", clearSelection);

    var $cmdDataPrint = $("#cmdDataPrint").on("click", printData);
    var $cmdMobDataPrint = $("#cmdMobDataPrint").on("click", printData);

    $("#cmdExportData").on("click", function () {
      if (!$(this).hasClass("Disabled")) {
        exportData($(this), "ExportData.ashx");
      }
    });

    $("#cmdMailingLabels").on("click", function () {
      if (!$(this).hasClass("Disabled")) {
        exportData($(this), "MailingLabels.aspx");
      }
    });

    $("#cmdSelectView").on("click", function () {
      gpv.selection.selectByGeometry(gpv.viewer.getExtent());
      $("#pnlQueryGrid").slideDown();
    });

    var $ddlAction = $("#ddlAction").on("change", actionChanged);
    var $ddlProximity = $("#ddlProximity").on("change", proximityChanged);
    var $ddlQuery = $("#ddlQuery").on("change", queryChanged);
    var $ddlSelectionLayer = $("#ddlSelectionLayer").on("change", selectionLayerChanged);
    var $ddlTargetLayer = $("#ddlTargetLayer").on("change", targetLayerChanged);

    var $grdQuery = $("#grdQuery").dataGrid({
      rowClass: "DataGridRow",
      alternateClass: "DataGridRowAlternate",
      selectedClass: "ActiveGridRowSelect",
      selectionChanged: queryGridChanged,
      stringCompare: addressCompare
    });

    var $pnlDataList = $("#pnlDataList").on("click", "a.CommandLink", function (e) {
      var url = $(this).attr("href");
      if (url.substr(0, 12) == "application:") {
        e.preventDefault();
        reinitialize(url);
      }
    });

    // ==== this is for mobile showing details panel data ====
    var $pnlMobDataList = $("#pnlMobDataList").on("click", "a.CommandLink", function (e) {
      var url = $(this).attr("href");
      if (url.substr(0, 12) == "application:") {
        e.preventDefault();
        reinitialize(url);
      }
    });

    var $ddlDataTheme = $("#ddlDataTheme").on("change", function () {
      var dataTab = $("#ddlDataTheme :selected").attr("data-datatab");
      appState.DataTab = dataTab;
      fillDataList();
    });

    // ==== this is for mobile detail panel dropdown selection functionally
    var $ddlMobDataTheme = $("#ddlMobDataTheme").on("change", function () {
      dataTab = $("#ddlMobDataTheme :selected").attr("data-datatab");
      appState.DataTab = dataTab;
      fillDataList();
    });

    // =====  component events

    gpv.on("viewer", "mapTabChanged", mapTabChanged);
    gpv.on("selection", "changed", selectionChanged);

    // =====  private functions  =====

    function actionChanged(e) {
      var previous = appState.Action;
      appState.update({ Action: parseInt($ddlAction.val(), 10) });

      if (!e) {
        if (appState.Action == action.select) {
          appState.update({ SelectionIds: [] });
        }
      }
      else {
        var mapTab = config.mapTab[appState.MapTab];
        var preserveSelection = gpv.settings.preserveOnActionChange == "selection";

        if (preserveSelection && previous > action.select && appState.Action == action.select && hasId(mapTab.target, appState.SelectionLayer)) {
          appState.update({
            TargetLayer: appState.SelectionLayer,
            TargetIds: appState.SelectionIds,
            SelectionIds: []
          });
          $ddlTargetLayer.val(appState.TargetLayer);
          fillSelectionLayer();
          fillQuery();
        }
        else if (preserveSelection && previous == action.select && appState.Action > action.select && hasId(mapTab.selection, appState.TargetLayer)) {
          fillSelectionLayer();
          appState.update({
            SelectionLayer: appState.TargetLayer,
            SelectionIds: appState.TargetIds,
            TargetIds: []
          });
          $ddlSelectionLayer.val(appState.SelectionLayer);
        }
        else {
          if (appState.Action == action.select) {
            appState.update({ SelectionIds: [] });
          }

          fillSelectionLayer();
        }

        fillProximity();

        if (preserveSelection) {
          selection.update();
        }
      }
    }

    function addressCompare(a, b) {
      a = addressParse(a);
      b = addressParse(b);

      for (var i = 0; i < 3; ++i) {
        var r = compare(a[i], b[i]);

        if (r != 0) {
          return r;
        }
      }

      return 0;
    }

    function addressParse(s) {
      var parts = s.split(" ");
      var first = parts.shift();
      var n = parseInt(first, 10);

      if (isNaN(n)) {
        return [s, 0, ""];
      }
      else {
        return [parts.join(" "), n, first.substr(n.toString().length)];
      }
    }

    function clearSelection() {
      if (appState.TargetIds.length > 0 || appState.TargetIds.length > 0) {
        appState.update({
          TargetIds: [],
          SelectionIds: [],
          ActiveMapId: "",
          ActiveDataId: ""
        });
        selection.update();
      }
    }

    function compare(a, b) {
      return a < b ? -1 : a > b ? 1 : 0;
    }

    function exportData($target, url) {
      if (!$target.hasClass("Disabled")) {
        var dataIds = $.map($grdQuery.dataGrid("getIds"), function (v) { return v.d; });

        $("#hdnExportLayer").val(appState.TargetLayer);
        $("#hdnExportIds").val(dataIds.join(","));
        $("#frmExportData").prop("action", url).submit();
      }
    }

    function emptyIfNull(v) {
      return v == null ? "" : v;
    }

    function fillAction(initializing) {
      var list = [];

      if (!appState.TargetLayer) {
        list.push({ id: action.select, name: " " });
      }
      else {
        var mapTab = config.mapTab[appState.MapTab];
        var layer = config.layer[appState.TargetLayer];

        list.push({ id: action.select, name: "Select" });

        if (mapTab.selection.length) {
          if (layer.proximity.length) {
            list.push({ id: action.findAllWithin, name: "Find all" });
          }

          $.each(["one in", "two", "three", "four", "five"], function (i, v) {
            list.push({ id: action["findNearest" + (i + 1)], name: "Find the " + v });
          });
        }
      }

      var changed = gpv.loadOptions($ddlAction, list);

      if (initializing) {
        syncAppState($ddlAction, "Action");
      }
      else if (changed) {
        actionChanged();
      }

      return changed;
    }

    function fillDataList() {
      $cmdDataPrint.addClass("Disabled");
      $cmdMobDataPrint.addClass("Disabled");
      if (!appState.ActiveDataId) {
        $pnlDataList.empty();
        $pnlMobDataList.empty();
      }
      else {
        $.ajax({
          url: service,
          data: {
            m: "GetDataListHtml",
            datatab: appState.DataTab,
            id: appState.ActiveDataId
          },
          type: "POST",
          dataType: "html",
          success: function (html) {
            $pnlDataList.empty().append(html);
            $pnlMobDataList.empty().append(html);
            $cmdDataPrint.removeClass("Disabled").data("printdata", [
              "datatab=", encodeURIComponent(appState.DataTab),
              "&id=", encodeURIComponent(appState.ActiveDataId),
              "&print=1"
            ].join(""));
            $cmdMobDataPrint.removeClass("Disabled").data("printdata", [
              "datatab=", encodeURIComponent(appState.DataTab),
              "&id=", encodeURIComponent(appState.ActiveDataId),
              "&print=1"
            ].join(""));
            $pnlDataDisplay.show();
            $pnlDataDisplay.find("#spnDataTheme").text("Data Set");
            $pnlDataDisplay.find("#ddlDataTheme").show();
            // this is for Detail panel display when device is mobile or other
            if ($(window).width() > 700) {
              if ($pnlDataDisplay.css("right").substring(0, 1) === "-") {
                $pnlDataDisplay.animate({ right: 0, opacity: "1.0" }, 600, function () {
                  $(".DataExit").addClass("DataExitOpen");
                });
                $("#pnlOverview").animate({ right: 290 }, 600);   // shifting his place when detail panel display in device ( not mobile )
                $("div.leaflet-control-attribution.leaflet-control").animate({ right: 322 }, 600);    // shifting his place when detail panel display in device ( not mobile )
              }

            }
            else {
              $("#ddlMobDataTheme").show();
              $("#tabMobDetails").trigger("click");
              $(".MenuItem").removeClass("active");
              $("#tabMobDetails").addClass("active");
            }
          },
          error: function (xhr, status, message) {
            alert(message);
          }
        });
      }
    }

    function fillProximity(initializing) {
      var isFindAll = appState.TargetLayer && appState.Action == action.findAllWithin;
      var isFindNear = appState.TargetLayer && action.findNearest1 <= appState.Action && appState.Action <= action.findNearest5;
      var list = isFindAll ? config.layer[appState.TargetLayer].proximity : isFindNear ? [{ id: "", name: "nearest to the selected" }] : [];

      var changed = gpv.loadOptions($ddlProximity, list);

      if (initializing) {
        syncAppState($ddlProximity, "Proximity");
      }
      else if (changed) {
        proximityChanged();
      }

      return changed;
    }

    function fillQuery(initializing) {
      var list = appState.TargetLayer ? config.layer[appState.TargetLayer].query : [];
      var changed = gpv.loadOptions($ddlQuery, list);

      if (initializing) {
        syncAppState($ddlQuery, "Query");
      }
      else if (changed) {
        queryChanged();
      }

      return changed;
    }

    function fillSelectionLayer(initializing) {
      var list = appState.Action == action.select || !appState.TargetLayer ? [] : config.mapTab[appState.MapTab].selection;
      var changed = gpv.loadOptions($ddlSelectionLayer, list);

      if (initializing) {
        syncAppState($ddlSelectionLayer, "SelectionLayer");
      }
      else if (changed) {
        selectionLayerChanged();
      }

      return changed;
    }

    function fillTargetLayer(initializing) {
      var changed = gpv.loadOptions($ddlTargetLayer, config.mapTab[appState.MapTab].target) && !initializing;

      if (initializing) {
        syncAppState($ddlTargetLayer, "TargetLayer");
      }
      else if (changed) {
        targetLayerChanged();
      }

      return changed;
    }

    function hasId(list, id) {
      return $.grep(list, function (v) { return v.id == id; }).length > 0;
    }

    function initialize() {
      fillTargetLayer(true);
      fillAction(true);
      fillProximity(true);
      fillSelectionLayer(true);
      fillQuery(true);
      setDataTabs();
      fillDataList();
    }

    function mapShape(e) {
      if ($("#optSelect").hasClass("Selected") && appState.TargetLayer.length > 0) {
        map.removeLayer(e.shape);
        var geo = gpv.latLngsToSearchShape(map, e.shape.getLatLngs());
        gpv.selection.selectByGeometry(geo, e.shiftKey ? "add" : e.ctrlKey ? "remove" : "new");
      }
    }

    function mapTabChanged() {
      var changed = fillTargetLayer();

      if (changed) {
        setDataTabs();
        fillDataList();
      }

      changed = fillAction() || changed;
      changed = fillProximity() || changed;
      changed = fillSelectionLayer() || changed;
      changed = fillQuery() || changed;

      if (changed) {
        selection.update();
      }
    }

    function parseQuery(s) {
      var q = {};
      s.replace(/([^?=&]+)(=([^&]*))?/g, function (v0, v1, v2, v3) { q[v1] = v3 || null; });

      $.each(["layerson", "layersoff", "targetids", "targetparams", "selectionids"], function (i, v) {
        if (q.hasOwnProperty(v)) {
          q[v] = q[v] ? q[v].split(",") : [];
        }
      });

      return q;
    }

    function post(args) {
      args.url = service;
      gpv.post(args);
    }

    function printData() {
      if (!$cmdDataPrint.hasClass("Disabled")) {
        event.stopPropagation();
        var data = $cmdDataPrint.data("printdata");
        var windowName = "identify" + (new Date()).getTime();
        var features = "width=700,height=500,menubar=no,titlebar=no,toolbar=no,status=no,scrollbars=no,location=no,resizable=no";
        window.open("Identify.aspx?" + data, windowName, features, true);
      }

      if (!$cmdMobDataPrint.hasClass("Disabled")) {
        event.stopPropagation();
        var data = $cmdMobDataPrint.data("printdata");
        var windowName = "identify" + (new Date()).getTime();
        var features = "width=700,height=500,menubar=no,titlebar=no,toolbar=no,status=no,scrollbars=no,location=no,resizable=no";
        window.open("Identify.aspx?" + data, windowName, features, true);
      }
    }

    function proximityChanged(e) {
      appState.update({ Proximity: emptyIfNull($ddlProximity.val()) });

      if (e) {
        selection.update();
      }
    }

    function queryChanged(e) {
      appState.update({ Query: emptyIfNull($ddlQuery.val()) });

      if (e) {
        selection.update();
      }
    }

    function queryGridChanged(dblClick) {
      var sel = $grdQuery.dataGrid("getSelection");
      appState.update({
        ActiveMapId: sel.length ? sel[0].m : "",
        ActiveDataId: sel.length ? sel[0].d : ""
      });
      fillDataList();

      if (dblClick) {
        gpv.viewer.zoomToActive();
      }
      else {
        gpv.viewer.refreshMap();
      }
    }

    function reinitialize(url) {
      var query = parseQuery(url.substr(12));

      for (var prop in appState) {
        var key = prop.toLowerCase();

        if (key in query) {
          appState[prop] = query[key];
        }
      }

      if ("targetparams" in query) {
        post({
          data: {
            m: "GetTargetIds",
            layer: appState.TargetLayer,
            params: query.targetparams.join(",")
          },
          success: complete
        });
      }
      else {
        complete();
      }

      function complete(result) {
        if (result && result.ids) {
          appState.update({
            TargetIds: result.ids,
            ActiveMapId: "",
            ActiveDataId: ""
          });
        }

        initialize();

        $.each(reinitializedHandlers, function () {
          this(query);
        });

        var scaleBy = query.hasOwnProperty("scaleby") ? parseFloat(query.scaleby) : null;
        selection.update(scaleBy);
      }
    }

    function selectionChanged() {
      if (!appState.Query) {
        appState.update({ Query: $ddlQuery.val() });
      }

      if (appState.Query) {
        post({
          data: {
            m: "GetQueryGridData",
            state: appState.toJson("Application", "Query", "TargetIds")
          },
          success: function (result) {
            var hasRows = false;

            if (result) {
              $grdQuery.dataGrid("load", result);
              var hasRows = result.rows.length > 0;
            }

            if (appState.ActiveDataId.length) {
              $grdQuery.dataGrid("setSelection", function (id) {
                return id.m == appState.ActiveMapId && id.d == appState.ActiveDataId;
              });

              if ($grdQuery.dataGrid("getSelection").length == 0) {
                appState.update({
                  ActiveMapId: "",
                  ActiveDataId: ""
                });
              }
              else {
                fillDataList();
              }
            }

            $("#labSelectionCount").text((hasRows ? result.rows.length : "None") + " selected");

            if (!hasRows) {
              $("#cmdMailingLabels,#cmdExportData").addClass("Disabled");
              // clear detail tab data if exists
              $("#pnlDataList").empty().append('<div class="DataList">' +
              '<p class="dtlPara" style="text-align: center; margin-top: 10px; color: #898989;">' +
              'No Results</p></div>');
              $("#pnlMobDataList").empty().append('<div class="DataList">' +
              '<p class="dtlPara" style="text-align: center; margin-top: 10px; color: #898989;">' +
              'No Results</p></div>');


            }
            else {
              if ($("#pnlSelection").css("display") === "none") {
                gpv.viewer.switchToPanel("Selection");
              }

              post({
                data: {
                  m: "GetLayerProperties",
                  layer: appState.TargetLayer
                },
                success: function (result) {
                  if (result) {
                    $("#cmdMailingLabels").toggleClass("Disabled", !result.supportsMailingLabels);
                    $("#cmdExportData").toggleClass("Disabled", !result.supportsExportData);
                  }
                }
              });
            }

            $.each(gridFilledHandlers, function () {
              this(result);
            });
          }
        });
      }
    }

    function selectionLayerChanged(e) {
      appState.update({ SelectionLayer: emptyIfNull($ddlSelectionLayer.val()) });

      if (e) {
        appState.update({
          TargetIds: [],
          SelectionIds: []
        });
        selection.update();
      }
    }

    function setDataTabs() {
      $ddlDataTheme.empty();
      $ddlMobDataTheme.empty();

      if (appState.TargetLayer) {
        var layer = config.layer[appState.TargetLayer];
        var hasDataTab = $.grep(layer.dataTab, function (t) { return t.id == appState.DataTab; }).length;

        if (!hasDataTab) {
          appState.update({ DataTab: "" });
        }

        $.each(layer.dataTab, function (i, v) {
          if (!hasDataTab && i == 0) {
            appState.update({ DataTab: v.id });
          }

          $("<option value='" + v.name + "' data-datatab='" + v.id + "'>" + v.name + "</option>").prop("selected", appState.DataTab === v.id).appendTo($ddlDataTheme);
          $("<option value='" + v.name + "' data-datatab='" + v.id + "'>" + v.name + "</option>").prop("selected", appState.DataTab === v.id).appendTo($ddlMobDataTheme);
        });
      }
    }

    function setMap(m) {
      map = m;
      map.on("shapedrawn", mapShape);
    }

    function syncAppState($ddl, prop) {
      $ddl.val(appState[prop]);

      if ($ddl.val() != appState[prop]) {
        if ($ddl.find("option").length) {
          $ddl.prop("selectedIndex", 0);
          appState[prop] = $ddl.val();
        }
        else {
          appState[prop] = prop == "Action" ? 0 : "";
        }
      }
    }

    function targetLayerChanged(e) {
      appState.update({
        TargetLayer: emptyIfNull($ddlTargetLayer.val()),
        TargetIds: [],
        ActiveMapId: "",
        ActiveDataId: ""
      });

      if (e) {
        fillAction();
        fillProximity();
        fillSelectionLayer();
        fillQuery();
        setDataTabs();
        fillDataList();
        selection.update();
      }
    }

    // =====  public interface  =====

    gpv.selectionPanel = {
      gridFilled: function (fn) { gridFilledHandlers.push(fn); },
      reinitialize: reinitialize,
      reinitialized: function (fn) { reinitializedHandlers.push(fn); },
      setMap: setMap
    };

    initialize();
    selectionChanged();
  });

  return gpv;
})(GPV || {});
