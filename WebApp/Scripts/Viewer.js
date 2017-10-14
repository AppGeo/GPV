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
    var appState = gpv.appState;
    var fullExtent = L.Bounds.fromArray(gpv.configuration.fullExtent);
    var tileLayers = {};
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
    var panelAnimationTime = 0;

    // =====  controls required prior to map control creation  =====

    var $pnlDataDisplay = $("#pnlDataDisplay");

    // =====  map control  =====

    var maxZoom = gpv.settings.zoomLevels - 1;
    var crs = L.CRS.EPSG3857;
    if (gpv.settings.mapCrs) {
      crs = new L.Proj.CRS("GPV:1", gpv.settings.mapCrs);
      var c = crs.unproject(fullExtent.getCenter());
      var sf = 2 / L.CRS.EPSG3857.scaleFactorAtLatitude(c.lat);
      var isFeet = gpv.settings.mapCrs.indexOf("+to_meter=0.3048") >= 0;
      var resolutions = [(isFeet ? 513591 : 156543) * sf];
      for (var i = 0; i < maxZoom; ++i) {
        resolutions.push(resolutions[i] * 0.5);
      }
      crs = new L.Proj.CRS("GPV:1", gpv.settings.mapCrs, {
        resolutions: resolutions
      });
    }

    var map = L.map("mapMain", {
      crs: crs,
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
    $(document).ready(function () {
      $("#optIdentify").trigger("click");
    });
    var shingleLayer = L.shingleLayer({
      urlBuilder: refreshMap,
      zIndex: 100,
      preserveOnPan: false          // TO DO: reset based on presence of underlay tiles
    }).on("shingleload", function () {
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

    var fullViewTool = L.Control.extend({
      options: {
        position: 'topleft'
      },
      onAdd: function ($map) {
        var button = L.DomUtil.create('div', 'mapButton');
        $(button).attr('id', 'cmdFullView');
        $(button).attr('title', 'Full Extent');
        $(button).html('<span class="glyphicon glyphicon-globe"></span>');
        return button;
      }
    });

    var locationTool = L.Control.extend({
      options: {
        position: 'topleft'
      },
      onAdd: function ($map) {
        var button = L.DomUtil.create('div', 'mapButton');
        $(button).attr('id', 'cmdLocation');
        $(button).attr('title', 'Current Location');
        $(button).html('<span class="glyphicon glyphicon-screenshot"></span>');
        return button;
      }
    });

    map.addControl(new fullViewTool())
    .addControl(new locationTool());
    gpv.mapTip.setMap(map);
    gpv.selectionPanel.setMap(map);
    gpv.markupPanel.setMap(map);
    gpv.sharePanel.setMap(map);

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

    $("#cmdEmail").on("click", function () {
      gpv.post({
        url: "Services/SaveAppState.ashx",
        data: {
          state: gpv.appState.toJson()
        },
        success: function (result) {
          if (result && result.id) {
            var loc = document.location;
            var url = [loc.protocol, "//", loc.hostname, loc.port.length && loc.port != "80" ? ":" + loc.port : "", loc.pathname, "?state=", result.id].join("");
            $lnkEmail.val(url);
            $('#pnlEmail').fadeIn(600);
            selectEmailLink();
          }
        }
      });
    });

    $('#cmdEmailClose').on('click', function (e) {
      e.preventDefault();
      $('#pnlEmail').fadeOut(600);
    });

    var $lnkEmail = $("#lnkEmail").on("mousedown", function (e) {
      if (e.which > 1) {
        e.preventDefault();
        selectEmailLink();
      }
    });

    function selectEmailLink() {
      $lnkEmail.prop("selectionStart", 0).prop("selectionEnd", $lnkEmail.val().length);
    }

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

    //  ==== fort detail panel display ( in large device) ====
    $("#cmdShowDetails").on("click", function () {
      var myw = $pnlDataDisplay.css("right").substring(0, 1);
      if ($pnlDataDisplay.css("right").substring(0, 1) === "-") {
        $pnlDataDisplay.show();
        $pnlDataDisplay.animate({ right: 0, opacity: "1.0" }, 600, function () {
          $(".DataExit").addClass("DataExitOpen");
        });
        $("#pnlOverview").animate({ right: 290 }, 600);   // shift pnlOverview when detail panel show
        $("div.leaflet-control-attribution.leaflet-control").animate({ right: 322 }, 600);
      }
      else {
        $(".DataHeader").trigger("click");
      }
    });

    $("#cmdZoomSelect").on("click", function () {
      zoomToSelection(1.2);
    });

    // ==== detail panel hide ( in large device) ====
    $(".DataHeader").on("click", function () {
      var width = "-" + $pnlDataDisplay.css("width");
      $pnlDataDisplay.animate({ right: width, opacity: "0" }, 800, function () {
        $(".DataExit").removeClass("DataExitOpen");
        $pnlDataDisplay.hide();
      });
      $("#pnlOverview").animate({ right: 5 }, 800);
      $("div.leaflet-control-attribution.leaflet-control").animate({ right: 35 }, 800);
    });

    $("#cmdOverview").on("click", function () {
      if ($("#mapOverview").css("background-image") === "none") {
        $("#pnlOverview").removeClass("overviewInitial").addClass("overviewMap");
        $("#iconOverview").addClass('iconOpen');
        overviewMapHeight = $("#pnlOverview").height();
        overviewMapWidth = $("#pnlOverview").width();
        $("div.leaflet-control-attribution.leaflet-control").css({ right: overviewMapWidth + 10 });
        $mapOverview = $("#mapOverview");
        overviewExtent = fullExtent.fit($mapOverview.width(), $mapOverview.height());
        setOverviewMap();
        updateOverviewExtent();
      }
      else {
        if ($("#iconOverview").hasClass("iconOpen")) {
          $("#pnlOverview").animate({ height: "26px", width: "26px" }, panelAnimationTime, function () {
            $("#iconOverview").removeClass('iconOpen');
          });
          $("div.leaflet-control-attribution.leaflet-control").animate({ right: 35 }, panelAnimationTime);
        }
        else {
          $("#pnlOverview").animate({ height: overviewMapHeight + "px", width: overviewMapWidth + "px" }, panelAnimationTime, function () {
            $("#iconOverview").addClass('iconOpen');
            updateOverviewExtent();
          });
          $("div.leaflet-control-attribution.leaflet-control").animate({ right: overviewMapWidth + 10 }, panelAnimationTime);
        }
      }
    });

    function OpenSelectionTab(name) {
      hideFunctionMenu(function () { showFunctionPanel(name); });
      $.each(functionTabChangedHandlers, function () {
        this(name);
      });
    }

    $("#optSelect").on("click", function () {
      gpv.selectTool($(this), map, { cursor: 'default', dragging: false, boxZoom: false, drawing: { mode: 'rectangle', style: { color: '#c0c0c0', fill: true, fillColor: '#e0e0e0' } } });
      HidePanel();
      $(".MenuItem").removeClass("active");
      $("#tabSelection").addClass("active");
      showFunctionPanel("Selection");   // open selection panel when Select selected in Maptool
      $.each(functionTabChangedHandlers, function () {
        this("Selection");
      });
    });

    $("#selectMapTheme li").click(function () {
      $("#ucLegendPanel_selectedTheme").html($(this).html());
      var mapTab = $(this).attr("data-maptab");
      appState.update({ MapTab: mapTab });
      triggerMapTabChanged();
      shingleLayer.redraw();
      drawTileLayers();
    });

    $("#selectMapLevel li").click(function () {
      $("#selectedLevel").html($(this).html());
      appState.update({ Level: $(this).attr("data-level") });
      shingleLayer.redraw();
    });

    // =====  map tools  =====

    $("#selectMapTools li").not($(".dropdown-header")).click(function () {
      if (!$(this).hasClass('Disabled')) {
        $("#selectedTool").html($(this).html());
      }
    });

    // ==== cusror type selection when Identify select in MapTool ====
    var $MapTool = $(".MapTool");
    var $optIdentify = $("#optIdentify");
    $optIdentify.hasClass("Selected")
    {
      gpv.selectTool($(this), map, { cursor: 'default', drawing: { mode: 'off' } });
    }
    $optIdentify.on("click", function () {
      gpv.selectTool($(this), map, { cursor: 'default', drawing: { mode: 'off' } });
    });

    $("#ucMarkupPanel_optPan").on("click", function () {
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

    function createTileLayers() {
      Object.keys(gpv.configuration.mapTab).forEach(function (m) {
        tileLayers[m] = {};
        gpv.configuration.mapTab[m].tileGroup.forEach(function (tg) {
          var z = -1;
          tileLayers[m][tg.group.id] = tg.group.tileLayer.map(function (tl) {
            z += 1;
            return L.tileLayer(tl.url, {
              zIndex: tl.overlay ? 200 + z : z,
              attribution: tl.attribution,
              opacity: tg.opacity,
              maxZoom: tl.maxZoom || map.options.maxZoom
            });
          });
        });
      });
    }

    function drawTileLayers() {
      map.eachLayer(function (layer) {
        if (layer.constructor === L.TileLayer) {
          map.removeLayer(layer);
        }
      });

      var mapTab = appState.MapTab;

      var visible = gpv.baselayer.getVisibleTiles(mapTab);

      Object.keys(tileLayers[mapTab]).forEach(function (tg) {
        if (visible.indexOf(tg) >= 0) {
          tileLayers[mapTab][tg].forEach(function (tl) {
            tl.addTo(map);
          });
        }
      });
    }

    // ==== for open any Panel ====
    $(".MenuItem").on("click", function () {
      var name = $(this).text();
      var trimName = $.trim(name);
      if (trimName == "Draw") {
        trimName = "Markup";
      }
      if (trimName == "Maps") {
        trimName = "Legend";
      }
      $(".MenuItem").removeClass("active");
      $("#tab" + trimName).addClass("active");
      $(".share").hide();
      hideFunctionMenu(function () {
        showFunctionPanel(trimName);
      });

      $.each(functionTabChangedHandlers, function () {
        this(trimName);
      });
    });

    // ==== for closing any panel
    $(".FunctionHeader").on("click", function () {
      if ($(window).width() < 700) {
        $(".MenuItem").removeClass("active");
        $("#btnHamburger").removeClass("hidden");
        $("#btnHamburgerClose").addClass("hidden");
      }
      hideFunctionPanel(showFunctionMenu);
    });

    // ==== function for hiding all panel
    function HidePanel() {
      $("#pnlMarkup").attr("style", "display:none;");
      $("#pnlShare").attr("style", "display:none;");
      $("#pnlLocation").attr("style", "display:none;");
      $("#pnlLegend").attr("style", "display:none;");
      $("#pnlSelection").attr("style", "display:none;");
      $("#pnlSearch").attr("style", "display:none;");
      $(".share").hide();
    }

    // ==== Hamburger functionally ====

    if ($(window).width() < 700) {    // for small device open panel
      $("#btnHamburger").on("click", function () {
        $("#btnHamburger").addClass("hidden");
        $("#btnHamburgerClose").removeClass("hidden");
        $("#tabSearch").trigger("click");

      });
      $("#btnHamburgerClose").on("click", function () {   // for small device close panel
        $(".MenuItem").removeClass("active");
        $("#btnHamburger").removeClass("hidden");
        $("#btnHamburgerClose").addClass("hidden");
        hideFunctionPanel(showFunctionMenu);
      });
    }
    else {
      $(".hamburger").on("click", function () {   // for large device
        if ($(".leftNav_panel").hasClass("active")) {
          $(".leftNav_panel").removeClass("active");
        }
        else {
          $(".leftNav_panel").addClass("active");
        }
        pnlFunctionTabsWidth = $("#pnlFunctionTabs").width();
        if (($("#pnlFunction").css("opacity")) == "0")
        { pnlFunctionWidth = 0; }
        else
        { pnlFunctionWidth = $("#pnlFunction").width(); }
        $("#pnlFunction").animate({ left: pnlFunctionTabsWidth, opacity: $("#pnlFunction").css("opacity") }, panelAnimationTime, function () {
          $("#pnlMapSizer").animate({ left: parseInt(pnlFunctionTabsWidth) }, {
            duration: panelAnimationTime,
            progress: function () {
              map.invalidateSize();
            },
            complete: function () {
              map.invalidateSize();
              shingleLayer.redraw();
            }
          });
        });
      });
    }

    function showFunctionMenu() {
      $(".share").hide();
      $(".FunctionExit").removeClass("FunctionExitOpen");
    }
    var pnlFuctionLeft, pnlMapSizerLaft, pnlFunctionTabsWidth, pnlFunctionWidth;
    // ==== fuction for Show Panel ====
    function showFunctionPanel(name) {
      if ($(window).width() < 700) {
        if ($("#btnHamburgerClose").hasClass("hidden")) {
          $("#btnHamburgerClose").removeClass("hidden");
          $("#btnHamburger").addClass("hidden");
          $(".leaflet-control-scale leaflet-control").addClass("dimensionDetail");
        }
      }
      $(".FunctionPanel").hide();
      $("#pnl" + name).show();
      if (name == "Selection") {  // When Selection panel open , Select tool selected in pnlMapTools Dropdown 
        gpv.selectTool($(this), map, { cursor: 'default', dragging: false, boxZoom: false, drawing: { mode: 'rectangle', style: { color: '#c0c0c0', fill: true, fillColor: '#e0e0e0' } } });
        $("#optSelect").addClass("Selected");
        $("#selectedTool").html($("#optSelect").html());
      }
      else if (name == "Markup") {     // When Draw panel open , Draw tool selected in pnlMapTools Dropdown 
        $("#ucMarkupPanel_optDrawLine").trigger("click");
        $("#optMarkupTool").addClass("Selected");
        $("#selectedTool").html($("#optMarkupTool").html());
      }
      else {    // When Share ,Search , Map ,Location  panel open , Identify tool selected in pnlMapTools Dropdown 
        $("#optIdentify").addClass("Selected");
        $("#optIdentify").trigger("click");
      }
      pnlFunctionTabsWidth = $("#pnlFunctionTabs").width();
      pnlFunctionWidth = $("#pnlFunction").width();
      $(".FunctionExit").addClass("FunctionExitOpen");
      $("#pnlFunction").css("display", "block");
      $("#pnlFunction").animate({ left: pnlFunctionTabsWidth, opacity: "1.0" }, 800);
      $("#pnlMapSizer").animate({ left: pnlFunctionTabsWidth }, {
        progress: function () {
          map.invalidateSize();
        },
        complete: function () {
          map.invalidateSize();
          shingleLayer.redraw();
          if ($(window).width() > 700) { // for large device
            $("#pnlMapMenus").addClass("pnlMapMenus_option");
            $("#mapMain .leaflet-left").addClass("pnlMapMenus_option");
            $(".leaflet-left .leaflet-control").css("margin-left", "3px");
            $("#pnlMap #logo").addClass("pnlMapMenus_option");
          }
        }
      });
    }

    function hideFunctionMenu(callback) {
      $("#pnlFunctionTabs").animate({ left: "0px", opacity: "1" }, panelAnimationTime, callback);
    }

    // hide (close) panel 
    function hideFunctionPanel(callback) {
      pnlFunctionTabsWidth = $("#pnlFunctionSidebar").width();
      $("#pnlFunction").animate({ left: -355, opacity: "0" }, 800, function () {
        $("#pnlFunction").css("display", "none");
      });
      $("#pnlMapSizer").animate({ left: pnlFunctionTabsWidth }, {
        progress: function () {
          map.invalidateSize();
        },
        complete: function () {
          map.invalidateSize();
          shingleLayer.redraw();
          if ($(window).width() > 700) { // for large device
            $("#pnlMapMenus").removeClass("pnlMapMenus_option");
            $("#mapMain .leaflet-left").removeClass("pnlMapMenus_option");
            $(".leaflet-left .leaflet-control").css("margin-left", "10px");
            $("#pnlMap #logo").removeClass("pnlMapMenus_option");
            $(".leaflet-control-scale leaflet-control").addClass("dimensionDetail");
          }
        }
      });

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
              distance: gpv.searchDistance(),
              scale: map.getProjectedPixelSize()
            },
            type: "POST",
            dataType: "html",
            success: function (html) {
              if (html.length > 28) {
                $("#pnlDataList").empty().append(html);
                $("#pnlMobDataList").empty().append(html);
                $("#cmdDataPrint").removeClass("Disabled").data("printdata", [
                "maptab=", encodeURIComponent(appState.MapTab),
                "&visiblelayers=", encodeURIComponent(visibleLayers.join("\x01")),
                "&level=", appState.Level,
                "&x=", p.x,
                "&y=", p.y,
                "&distance=", gpv.searchDistance(),
                "&scale=", map.getProjectedPixelSize(),
                "&print=1"
                ].join(""));
                $("#cmdMobDataPrint").removeClass("Disabled").data("printdata", [  // for small device
                "maptab=", encodeURIComponent(appState.MapTab),
                "&visiblelayers=", encodeURIComponent(visibleLayers.join("\x01")),
                "&level=", appState.Level,
                "&x=", p.x,
                "&y=", p.y,
                "&distance=", gpv.searchDistance(),
                "&scale=", map.getProjectedPixelSize(),
                "&print=1"
                ].join(""));
              }
              else {
                $("#pnlDataList").empty().append('<div class="DataList">' +
                '<p class="dtlPara" style="text-align: center; margin-top: 10px; color: #898989;">' +
                'No Results</p></div>');
                $("#pnlMobDataList").empty().append('<div class="DataList">' +
                '<p class="dtlPara" style="text-align: center; margin-top: 10px; color: #898989;">' +
                'No Results</p></div>');
              }

              var $pnlDataDisplay = $("#pnlDataDisplay");
              $pnlDataDisplay.show();
              $pnlDataDisplay.find("#spnDataTheme").text("Identify");
              $pnlDataDisplay.find("#ddlDataTheme").hide();
              $("#ddlMobDataTheme").hide();
              if ($("#tabMobDetails").css("display") != "none") {   // for small device trigger tabMobDetails click for showing Detail panel
                $("#tabMobDetails").trigger("click");
                $(".MenuItem").removeClass("active");
                $("#tabMobDetails").addClass("active");
              }
              else {
                if ($pnlDataDisplay.css("right").substring(0, 1) === "-") {   // for large device showing Detail panel
                  $pnlDataDisplay.animate({ right: 0, opacity: "1.0" }, 600, function () {
                    $(".DataExit").addClass("DataExitOpen");
                  });
                  $("#pnlOverview").animate({ right: 288 }, 600);   // shift pnlOverview Control when Detail panel show in large device
                  $("div.leaflet-control-attribution.leaflet-control").animate({ right: 318 }, 800);

                }
              }
            },
            error: function (xhr, status, message) {
              alert(message);
            }
          });
        }
      }
    }

    function refreshMap(size, bbox, callback) {
      var extent = appState.Extent;
      var same = sameBox(extent.bbox, bbox);
      extent.bbox = bbox;
      var layers = appState.VisibleLayers;
      layers[appState.MapTab] = gpv.legendPanel.getVisibleLayers(appState.MapTab);
      appState.update({
        Extent: extent,
        VisibleLayers: layers
      });

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

    var $optMarkupTool = $("#optMarkupTool").on("click", function () {
      $(".MenuItem").removeClass("active");
      $("#tabMarkup").addClass("active");

      HidePanel();
      showFunctionPanel("Markup");
      $.each(functionTabChangedHandlers, function () {
        this("Markup");
      });
    });

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

    function toggleTileGroup(groupId, visible) {
      tileLayers[appState.MapTab][groupId].forEach(function (tl) {
        if (visible) {
          tl.addTo(map);
        }
        else {
          map.removeLayer(tl);
        }
      });
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
        bbox: overviewExtent.toArray().join()
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
    function ClickIdentity() {
      map.on("click", identify);
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
      toggleTileGroup: toggleTileGroup,
      OpenSelectionTab: OpenSelectionTab,
      zoomToActive: zoomToActive
    };

    // =====  finish initialization  =====

    map.fitProjectedBounds(L.Bounds.fromArray(appState.Extent.bbox));

    //need to add title attribute due to bootstrap overwriting title with popover
    $("#cmdLocation").attr("title", "Current Location");
    gpv.loadComplete();
    createTileLayers();
    drawTileLayers();
    triggerMapTabChanged();
    $('input').on('change', function () {
      var dataTile = $(this).attr('data-tilegroup');
      var isChecked = $(this).is(':checked')
      var ele = $(document).find($('input[data-tilegroup= "' + dataTile + '"]'));
      for (var i = 0; i < ele.length; i++) {
        if (isChecked) {
          $(ele[i]).prop('checked', true);
        }
        else {
          $(ele[i]).prop('checked', false);
        }
      }
    });

    var $layerContainer = $("#pnlBaseMap");
    var $baseMapContainer = $('#pnlBaseMaps');
    var itm = document.getElementById('pnlBaseMaps');
    $layerContainer.on('change', function () {
      var isChecked = $layerContainer.is(':checked');
      if (isChecked) {
        $('#pnlBaseLayer').show();
      } else {
        $('#pnlBaseLayer').hide();
      }
    });

    // ==== for custom scrollbar theme ====
    $('.customScroll').mCustomScrollbar({
      theme: "3d-thick"
    })
    //$('.horizontalScroll').mCustomScrollbar({
    //  theme: "3d-thick",
    //  axis: "x"
    //})
  });
  return gpv;
})(GPV || {});
