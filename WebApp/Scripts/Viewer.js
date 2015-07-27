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

    var fullExtent = gpv.configuration.fullExtent;
    var resizeHandle;
    var redrawPost;

    var $mapOverview = $("#mapOverview");
    var $locatorBox = $("#locatorBox");
    var overviewMapHeight = $("#pnlOverview").height();
    var overviewMapWidth = $("#pnlOverview").width();

    var locatorPanning = false;

    var mapTabChangedHandlers = [];
    var functionTabChangedHandlers = [];
    var extentChangedHandlers = [];

    var panelAnimationTime = 400;

    // =====  controls required prior to map control creation  =====

    var $ddlExternalMap = $("#ddlExternalMap").on("change", setExternalMap);
    var $pnlDataDisplay = $("#pnlDataDisplay");


    // =====  map control  =====

    // TODO: add support for measurement
    // TODO: add support for markup

    var maxZoom = gpv.settings.zoomLevels - 1;
    var resolutions = [ 0.25 * Math.pow(2, maxZoom) ];

    for (var i = 0; i < maxZoom; ++i) {
      resolutions.push(resolutions[i] * 0.5);
    }

    var crs = new L.Proj.CRS("GPV:1", gpv.settings.coordinateSystem, {
      resolutions: resolutions
    });

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
      showLocatorExtent();
      refreshOverviewExtent();
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

    debugger;
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

    $("#cmdEmail").on("click", function () {
      var $this = $(this)
      gpv.post({
        url: "Services/SaveAppState.ashx",
        data: {
          state: appState.toJson()
        },
        success: function (result) {
          if (result && result.id) {
            var loc = document.location;
            var url = [loc.protocol, "//", loc.hostname, loc.port.length && loc.port != "80" ? ":" + loc.port : "", loc.pathname, "?state=", result.id];
            window.open("mailto:" + $("#tboEmail").val() + "?subject=Map&body=" + encodeURIComponent(url.join("")), "_blank");
            $("#tboEmail").val("");
            $(".share").fadeOut(600);
          }
        }
      });
    });

    $("#cmdExternalMap").on("click", function(e){
      e.preventDefault();
      var url = $(this).attr("href");
      window.open(url, "_blank");
    })

    $("#cmdFullView").on("click", function () {
      zoomToFullExtent();
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

    $("#cmdPrint").on("click", function () {
      var $form = $("#frmPrint");
      $form.find('[name="state"]').val(appState.toJson());
      $form.find('[name="width"]').val(map.getSize().x);
      $form.submit();
    });

    $("#cmdSaveMap").on("click", function () {
      var $form = $("#frmSaveMap");
      $form.find('[name="m"]').val($("#ddlSaveMap").val() == "image" ? "SaveMapImage" : "SaveMapKml");
      $form.find('[name="state"]').val(appState.toJson());
      $form.find('[name="width"]').val(map.getSize().x);
      $form.find('[name="height"]').val(map.getSize().y);
      $form.submit();
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
      zoomToSelection(1.6);
    });

    $(".DataHeader").on("click", function () {
      var width = "-" + $pnlDataDisplay.css("width");
      $pnlDataDisplay.animate({ right: width, opacity: "0" }, 600, function () {
        $(".DataExit").removeClass("DataExitOpen");
        $pnlDataDisplay.hide();
      });
    });

    var $ddlLevel = $("#ddlLevel").on("change", function () {
      appState.Level = $(this).val();
      shingleLayer.redraw();
    });

    $(".FunctionHeader").on("click", function () {
      hideFunctionPanel(showFunctionMenu);
    });

    $("#cmdOverview").on("click", function () {
      if ($("#iconOverview").hasClass("iconOpen")) {
        $("#pnlOverview").animate({ height: "26px", width: "26px" }, 600);
        $("#iconOverview").removeClass('iconOpen');
      }
      else {
        $("#pnlOverview").animate({ height: overviewMapHeight + "px", width: overviewMapWidth + "px" }, 600, function () {
          $("#iconOverview").addClass('iconOpen');
          refreshOverviewExtent();
        });
      }
    });

    $(".MenuItem").on("click", function(){
      var name = $(this).text();
      
      hideFunctionMenu(function () { showFunctionPanel(name); });

      $.each(functionTabChangedHandlers, function () {
        this(name);
      });
    });

    $("#selectedTheme").html($("#selectMapTheme li").first().html());

    $("#selectMapTheme li").click(function () {
      $("#selectedTheme").html($(this).html());
      var mapTab = $(this).attr("data-maptab");
      appState.MapTab = mapTab;
      triggerMapTabChanged();
      shingleLayer.redraw();
    });

    $(".share-type").on("click", function (e) {
      e.preventDefault();
      $(".share").hide();
      var panel = "#pnl" + e.target.id.replace("cmdFor", "");
      $(panel).fadeIn(600);
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

    function refreshMap(size, bbox, callback) {
      var same = sameBox(appState.Extent.bbox, bbox);
      appState.Extent.bbox = bbox;
      appState.VisibleLayers[appState.MapTab] = gpv.legendPanel.getVisibleLayers(appState.MapTab);

      setExternalMap();

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
      $ddlLevel.val(appState.Level); 
      map.fitProjectedBounds(L.Bounds.fromArray(extent)) || shingleLayer.redraw();
      return map.getProjectedBounds().toArray();
    }

    function setExternalMap() {
      var externalName = $ddlExternalMap.val();
      var cmd = $("#cmdExternalMap");
      var extent = appState.Extent.bbox;

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
          pixelSize: map.getProjectedPixelSize()
        },
        success: function (result) {
          if (result && result.url) {
            cmd.attr("href", result.url).removeClass("Disabled");
          }
        }
      });
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
          map.fitProjectedBounds(L.Bounds.fromArray(bbox).pad(1.6)) || shingleLayer.redraw();
        }
      });
    }

    function zoomToFullExtent() {
      map.fitProjectedBounds(L.Bounds.fromArray(fullExtent)) || shingleLayer.redraw();
    }

    function zoomToSelection(scaleBy) {
      gpv.selection.getSelectionExtent(function (bbox) {
        if (bbox) {
          map.fitProjectedBounds(L.Bounds.fromArray(bbox).pad(scaleBy)) || shingleLayer.redraw();
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
      }
    });

    $mapOverview.mouseleave(function () {
      locatorPanning = false;
    });

    function setOverviewMap() {
      var viewWidth = $mapOverview.width();
      var viewHeight = $mapOverview.height();

      url = "Services/MapImage.ashx?" + $.param({
        m: "GetOverviewImage",
        application: appState.Application,
        width: viewWidth,
        height: viewHeight,
        bbox: fullExtent
      });

      $mapOverview.css("backgroundImage", "url(" + url + ")");
    }

    function panLocatorBox(e) {
      var x = e.pageX - $mapOverview.offset().left;
      var y = e.pageY - $mapOverview.offset().top;
      var left = Math.round(x - $locatorBox.width() * 0.5) - 2;
      var top = Math.round(y - $locatorBox.height() * 0.5) - 2;
      $locatorBox.css({ left: left + "px", top: top + "px" });

      x = fullExtent[0] + (x / $mapOverview.width()) * (fullExtent[2] - fullExtent[0]);
      y = fullExtent[3] - (y / $mapOverview.height()) * (fullExtent[3] - fullExtent[1]);
      //TAMC not sure here if this is correct.
      map.setView([x, y]);
      shingleLayer.redraw();
    }

    function showLocatorExtent() {
      var extent = map.getProjectedBounds().toArray();

      function toScreenX(x) {
        return Math.round($mapOverview.width() * (x - fullExtent[0]) / (fullExtent[2] - fullExtent[0]));
      }

      function toScreenY(y) {
        return Math.round($mapOverview.height() * (fullExtent[3] - y) / (fullExtent[3] - fullExtent[1]));
      }

      if (!locatorPanning) {
        var left = toScreenX(extent[0]);
        var top = toScreenY(extent[3]);
        var right = toScreenX(extent[2]);
        var bottom = toScreenY(extent[1]);
        var width = $mapOverview.width();
        var height = $mapOverview.height();

        if (((0 <= left && left <= width) || (0 <= right && right <= width) || (left < 0 && width < right)) &&
          ((0 <= top && top <= height) || (0 <= bottom && bottom <= height) || (top < 0 && height < bottom))) {
          $locatorBox.css({ left: left - 2 + "px", top: top - 2 + "px", width: right - left + "px", height: bottom - top + "px" });
        }
      }
    }

    function refreshOverviewExtent() {
      if ($("#iconOverview").hasClass('iconOpen')) {
        showLocatorExtent();
      }
    }

    // =====  public interface  =====

    gpv.viewer = {
      extentChanged: function (fn) { extentChangedHandlers.push(fn); },
      getExtent: function () { return map.getProjectedBounds().toArray(); },
      functionTabChanged: function (fn) { functionTabChangedHandlers.push(fn); },
      mapTabChanged: function (fn) { mapTabChangedHandlers.push(fn); },
      refreshMap: function () { $ddlLevel.val(appState.Level); shingleLayer.redraw(); },
      setExtent: setExtent,
      switchToPanel: switchToPanel,
      zoomToActive: zoomToActive
    };

    // =====  finish initialization  =====

    zoomToFullExtent();

    gpv.loadComplete();
    $MapTool.filter(".Selected").trigger("click");
    triggerMapTabChanged();
    setOverviewMap();
    

  });

  return gpv;
})(GPV || {});