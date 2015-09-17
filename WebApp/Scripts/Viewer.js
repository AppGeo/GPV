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

    var fullExtent = L.Bounds.fromArray(gpv.configuration.fullExtent);
    var resizeHandle;
    var redrawPost;

    var $mapOverview = $("#mapOverview");
    var $locatorBox = $("#locatorBox");
    var overviewMapHeight = null;
    var overviewMapWidth = null;
    var locatorPanning = false;
    var overviewExtent;

    var mapTabChangedHandlers = [];
    var functionTabChangedHandlers = [];
    var extentChangedHandlers = [];
    var mapRefreshedHandlers = [];

    var panelAnimationTime = 400;

    // =====  controls required prior to map control creation  =====

    var $pnlDataDisplay = $("#pnlDataDisplay");

    // =====  map control  =====

    var maxZoom = gpv.settings.zoomLevels - 1;
    var resolutions = [ 0.25 * Math.pow(2, maxZoom) ];

    for (var i = 0; i < maxZoom; ++i) {
      resolutions.push(resolutions[i] * 0.5);
    }

    var crs = new L.Proj.CRS("GPV:1", gpv.settings.coordinateSystem, {
      resolutions: resolutions
    });

    crs.distance = function (a, b) {
      a = crs.project(a);
      b = crs.project(b);
      var dx = a.x - b.x;
      var dy = a.y - b.y;
      return Math.sqrt(dx * dx + dy * dy) * (gpv.settings.mapUnits === "feet" ? 0.3048 : 1);
    };

    var map = L.map("mapMain", {
      crs: crs,
      doubleClickZoom: false,
      maxZoom: maxZoom,
      drawing: {
        mode: 'off',
        style: {
          color: '#808080',
          weight: 2,
          opacity: 1,
          fill: true,
          fillColor: '#808080',
          fillOpacity: 0.5
        },
        text: {
          className: 'MarkupText',
          color: '#FF0000'
        }
      }
    });

    map.on("click", identify);

    var shingleLayer = L.shingleLayer({ urlBuilder: refreshMap }).on("shingleload", function () {
      gpv.progress.clear();
      updateOverviewExtent();

      $.each(mapRefreshedHandlers, function () {
        this();
      });
    }).addTo(map);

    if (gpv.settings.showScaleBar) {
      L.control.scale({
        imperial: gpv.settings.measureUnits !== "meters",
        metric: gpv.settings.measureUnits !== "feet"
      }).addTo(map);
    }

    gpv.mapTip.setMap(map);
    gpv.selectionPanel.setMap(map);
    gpv.markupPanel.setMap(map);
    gpv.sharePanel.setMap(map);

    if ($("#pnlFunction div.FunctionPanel[style='display: block;']").length === 0) {
      showFunctionMenu();
    }
    else {
      var panel = $("#pnlFunction div.FunctionPanel[style='display: block;']")[0].id.replace("pnl", "");
      showFunctionPanel(panel);
    }

    // =====  control events  =====
    
    $(window).on("resize", function () {
      if (resizeHandle) {
        clearTimeout(resizeHandle);
      }

      resizeHandle = setTimeout(function () {
        resizeHandle = undefined;
        shingleLayer.redraw();
      }, 250);
    });

    $("#cmdFullView").on("click", function () {
      zoomToFullExtent();
    });

    $("#cmdLocation").on("click", function () {
      if (navigator && navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (pos) {
          var latlng = L.latLng(pos.coords.latitude, pos.coords.longitude);
          map.setView(latlng, maxZoom - 2);
        }, showGpsError);
      }
      else {
        showGpsError();
      }
    }).popover({
      content: 'GPS is not enabled on this device',
      delay: { show: 500, hide: 500 },
      placement: 'right',
      trigger: 'manual'
    });

    $("#cmdMenu").on("click", function () {
      var hide = $("#pnlFunctionSidebar").css("left") === "0px";
      $("#pnlFunctionSidebar").animate({ left: hide ? "-400px" : "0px" }, { duration: 800 });
      $("#pnlMapSizer").animate({ left: hide ? "0px" : "400px" }, { 
          duration: 800,
          progress: function () {
            map.invalidateSize();
          },
          complete: function () {
            map.invalidateSize();
            shingleLayer.redraw();
          }
      });
    });

    $("#cmdShowDetails").on("click", function () {
      if ($pnlDataDisplay.css("right").substring(0, 1) === "-") {
        $pnlDataDisplay.show();
        $pnlDataDisplay.animate({ right: 0, opacity: "1.0" }, 600, function () {
          $(".DataExit").addClass("DataExitOpen");
        });
      }
      else {
        $(".DataHeader").trigger("click");
      }
    });

    $("#cmdZoomSelect").on("click", function () {
      zoomToSelection(1.2);
    });

    $(".DataHeader").on("click", function () {
      var width = "-" + $pnlDataDisplay.css("width");
      $pnlDataDisplay.animate({ right: width, opacity: "0" }, 600, function () {
        $(".DataExit").removeClass("DataExitOpen");
        $pnlDataDisplay.hide();
      });
    });

    $(".FunctionHeader").on("click", function () {
      hideFunctionPanel(showFunctionMenu);
    });

    $("#cmdOverview").on("click", function () {
      if ($("#mapOverview").css("background-image") === "none") {
        $("#pnlOverview").removeClass("overviewInitial").addClass("overviewMap");
        $("#iconOverview").addClass('iconOpen');
        overviewMapHeight = $("#pnlOverview").height();
        overviewMapWidth = $("#pnlOverview").width();
        $mapOverview = $("#mapOverview");
        moveAttribution(0.3, -(overviewMapWidth - 30));
        overviewExtent = fullExtent.fit($mapOverview.width(), $mapOverview.height());
        setOverviewMap();
        updateOverviewExtent();
      }
      else {
        if ($("#iconOverview").hasClass("iconOpen")) {
          $("#pnlOverview").animate({ height: "26px", width: "26px" }, 600, function () {
            $("#iconOverview").removeClass('iconOpen');
          });
          moveAttribution(1, 0);
        }
        else {
          $("#pnlOverview").animate({ height: overviewMapHeight + "px", width: overviewMapWidth + "px" }, 600, function () {
            $("#iconOverview").addClass('iconOpen');
            updateOverviewExtent();
          });
          moveAttribution(0.75, -(overviewMapWidth - 30));
        }
      }
    });

    $(".MenuItem").on("click", function(){
      var name = $(this).text();
      
      hideFunctionMenu(function () { showFunctionPanel(name); });

      $.each(functionTabChangedHandlers, function () {
        this(name);
      });
    });

    $("#selectMapTheme li").click(function () {
      $("#selectedTheme").html($(this).html());
      var mapTab = $(this).attr("data-maptab");
      appState.MapTab = mapTab;
      triggerMapTabChanged();
      shingleLayer.redraw();
    });

    $("#selectMapLevel li").click(function () {
      $("#selectedLevel").html($(this).html());
      appState.Level = $(this).attr("data-level");
      shingleLayer.redraw();
    });

    // =====  map tools  =====

    $("#selectMapTools li").click(function () {
      if (!$(this).hasClass('Disabled')) {
        $("#selectedTool").html($(this).html());
      }
    });

    var $MapTool = $(".MapTool");

    $("#optIdentify").on("click", function () {
      gpv.selectTool($(this), map, { cursor: 'default', drawing: { mode: 'off' } });
    });

    $("#optPan").on("click", function () {
      gpv.selectTool($(this), map, { cursor: '', drawing: { mode: 'off' } });
    });


    // =====  component events  =====

    gpv.on("selection", "changed", function (truncated, scaleBy) {
      if (scaleBy) {
        zoomToSelection(scaleBy);
      }
      else {
        shingleLayer.redraw();
      }
    });

    // =====  private functions  =====

    function hideFunctionMenu(callback) {
      $("#pnlFunctionTabs").animate({ left: "-400px", opacity: "0" }, panelAnimationTime, callback);
    }

    function hideFunctionPanel(callback) {
      $("#pnlFunction").animate({ left: "-400px", opacity: "0" }, panelAnimationTime, callback);
    }

    function identify(e) {
      if ($MapTool.filter(".Selected").attr("id") === "optIdentify") {
        var visibleLayers = gpv.legendPanel.getVisibleLayers(appState.MapTab);

        if (visibleLayers.length) {
          var p = map.options.crs.project(e.latlng);

          $.ajax({
            url: "Services/MapIdentify.ashx",
            data: {
              maptab: appState.MapTab,
              visiblelayers: visibleLayers.join("\x01"),
              level: appState.Level,
              x: p.x,
              y: p.y,
              distance: 4,
              scale: map.getProjectedPixelSize()
            },
            type: "POST",
            dataType: "html",
            success: function (html) {
              $("#pnlDataList").empty().append(html);
              $("#cmdDataPrint").removeClass("Disabled").data("printdata", [
                "maptab=", encodeURIComponent(appState.MapTab), 
                "&visiblelayers=", encodeURIComponent(visibleLayers.join("\x01")),
                "&level=", appState.Level, 
                "&x=", p.x, 
                "&y=", p.y, 
                "&distance=4", 
                "&scale=", map.getProjectedPixelSize(),
                "&print=1"
              ].join(""));

              var $pnlDataDisplay = $("#pnlDataDisplay");

              $pnlDataDisplay.show();
              $pnlDataDisplay.find("#spnDataTheme").text("Identify");
              $pnlDataDisplay.find("#ddlDataTheme").hide();

              if ($pnlDataDisplay.css("right").substring(0, 1) === "-") {
                $pnlDataDisplay.animate({ right: 0, opacity: "1.0" }, 600, function () {
                  $(".DataExit").addClass("DataExitOpen");
                });
              }
            },
            error: function (xhr, status, message) {
              alert(message);
            }
          });
        }
      }
    }

    function moveAttribution(interval, xTranslation) {
      interval += "s";
      xTranslation = "translate(" + xTranslation + "px)";

      $("div.leaflet-control-attribution.leaflet-control").css({
        "transition": interval,
        "-webkit-transition": interval,
        "-moz-transition": interval, 
        "transform": xTranslation,
        "-webkit-transform": xTranslation,
        "-moz-transform": xTranslation,
        "-ms-transform": xTranslation
      });
    }

    function refreshMap(size, bbox, callback) {
      var same = sameBox(appState.Extent.bbox, bbox);
      appState.Extent.bbox = bbox;
      appState.VisibleLayers[appState.MapTab] = gpv.legendPanel.getVisibleLayers(appState.MapTab);

      if (!same) {
        $.each(extentChangedHandlers, function () {
          this(bbox);
        });
      }

      if (redrawPost && redrawPost.readyState !== 4) {
        redrawPost.abort();
      }

      gpv.progress.start();

      redrawPost = gpv.post({
        url: "Services/MapImage.ashx",
        data: {
          m: "MakeMapImage",
          state: appState.toJson(),
          width: size.x,
          height: size.y
        }
      }).done(function (url) {
        redrawPost = null;
        callback(url);
      });
    }

    function sameBox(a, b) {
      return a[0] == b[0] && a[1] == b[1] && a[2] == b[2] && a[3] == b[3];
    }

    function setExtent(extent) {
      showLevel();
      map.fitProjectedBounds(L.Bounds.fromArray(extent));
      return map.getProjectedBounds().toArray();
    }

    function showFunctionMenu() {
      $("#pnlFunctionTabs").animate({ left: "12px", opacity: "1.0" }, panelAnimationTime);
      $(".share").hide();
      $(".FunctionExit").removeClass("FunctionExitOpen");
    }

    function showFunctionPanel(name) {
      $(".FunctionPanel").hide();
      $("#pnl" + name).show();
      $("#pnlFunction").animate({ left: "0px", opacity: "1.0" }, panelAnimationTime, function () {
        $(".FunctionExit").addClass("FunctionExitOpen");
      });
    }

    function showLevel() {
      var $li = $("#selectMapLevel li[data-level=\"" + appState.Level + "\"]");
      $("#selectedLevel").html($li.html());
    }

    function switchToPanel(name) {
      if (parseInt($("#pnlFunctionTabs").css("left"), 10) >= 0) {
        hideFunctionMenu(function () { showFunctionPanel(name); });
      }
      else {
        hideFunctionPanel(function () { showFunctionPanel(name); });
      }
    }

    function triggerMapTabChanged() {
      $.each(mapTabChangedHandlers, function () {
        this();
      });
    }

    function zoomToActive() {
      gpv.selection.getActiveExtent(function (bbox) {
        if (bbox) {
          map.fitProjectedBounds(L.Bounds.fromArray(bbox).pad(1.2));
        }
      });
    }

    function zoomToFullExtent() {
      map.fitProjectedBounds(fullExtent);
    }

    function zoomToSelection(scaleBy) {
      gpv.selection.getSelectionExtent(function (bbox) {
        if (bbox) {
          map.fitProjectedBounds(L.Bounds.fromArray(bbox).pad(scaleBy));
        }
      });
    }

    // =====  overvew map  =====

    $("#locatorBox,#locatorBoxFill").mousedown(function (e) {
      e.preventDefault();
    });

    $mapOverview.mousedown(function (e) {
      locatorPanning = true;
      panLocatorBox(e);
    });

    $mapOverview.mousemove(function (e) {
      if (locatorPanning) {
        panLocatorBox(e);
      }
    });

    $mapOverview.mouseup(function (e) {
      if (locatorPanning) {
        panLocatorBox(e);
        locatorPanning = false;

        var x = e.pageX - $mapOverview.offset().left;
        var y = e.pageY - $mapOverview.offset().top;

        x = (x * overviewExtent.getSize().x / $mapOverview.width()) + overviewExtent.min.x;
        y = overviewExtent.max.y - (y * overviewExtent.getSize().y / $mapOverview.height());

        map.panTo(map.options.crs.unproject(L.point(x, y)));
      }
    });

    $mapOverview.mouseleave(function () {
      locatorPanning = false;
    });

    function setOverviewMap() {
      var url = "Services/MapImage.ashx?" + $.param({
        m: "GetOverviewImage",
        application: appState.Application,
        width: $mapOverview.width(),
        height: $mapOverview.height(),
        bbox: overviewExtent.toArray()
      });

      $mapOverview.css("backgroundImage", "url(" + url + ")");
    }

    function panLocatorBox(e) {
      var x = e.pageX - $mapOverview.offset().left;
      var y = e.pageY - $mapOverview.offset().top;
      var left = Math.round(x - $locatorBox.width() * 0.5) - 2;
      var top = Math.round(y - $locatorBox.height() * 0.5) - 2;
      $locatorBox.css({ left: left + "px", top: top + "px" });
    }

    function updateOverviewExtent() {
      if (!$("#iconOverview").hasClass('iconOpen') || locatorPanning) {
        return;
      }

      function toScreenX(x) {
        return Math.round($mapOverview.width() * (x - overviewExtent.min.x) / overviewExtent.getSize().x);
      }

      function toScreenY(y) {
        return Math.round($mapOverview.height() * (overviewExtent.max.y - y) / overviewExtent.getSize().y);
      }

      var extent = map.getProjectedBounds();

      var left = toScreenX(extent.min.x);
      var top = toScreenY(extent.max.y);
      var right = toScreenX(extent.max.x);
      var bottom = toScreenY(extent.min.y);
      var width = $mapOverview.width();
      var height = $mapOverview.height();

      $locatorBox.css({ left: left - 2 + "px", top: top - 2 + "px", width: right - left + "px", height: bottom - top + "px" });
    }

    function showGpsError() {
      $("#cmdLocation").popover('show');

      setTimeout(function () {
        $("#cmdLocation").popover('hide');
      }, 2000);
    }

    // =====  public interface  =====

    gpv.viewer = {
      extentChanged: function (fn) { extentChangedHandlers.push(fn); },
      mapRefreshed: function (fn) { mapRefreshedHandlers.push(fn); },
      getExtent: function () { return map.getProjectedBounds().toArray(); },
      functionTabChanged: function (fn) { functionTabChangedHandlers.push(fn); },
      mapTabChanged: function (fn) { mapTabChangedHandlers.push(fn); },
      refreshMap: function () { showLevel(); shingleLayer.redraw(); },
      setExtent: setExtent,
      switchToPanel: switchToPanel,
      zoomToActive: zoomToActive
    };

    // =====  finish initialization  =====

    map.fitProjectedBounds(L.Bounds.fromArray(gpv.appState.Extent.bbox));

    gpv.loadComplete();
    $MapTool.filter(".Selected").trigger("click");
    triggerMapTabChanged();
  });

  return gpv;
})(GPV || {});