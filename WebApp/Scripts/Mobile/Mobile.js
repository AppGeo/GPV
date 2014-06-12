//  Copyright 2012 Applied Geographics, Inc.  Licensed under the Apache License, Version 2.0 http://www.apache.org/licenses/LICENSE-2.0

(function () {

  var appConfig = window.sessionStorage.getItem("appConfig") || null,
      queryString = window.location.search.substring(1),
      params = queryString.split("&"),
      xferHint = window.sessionStorage.getItem("xferHint") || "",
      startAppID = window.sessionStorage.getItem("startAppID") || "",
      app = window.sessionStorage.getItem("app") || null,
      startMapTabID = window.sessionStorage.getItem("startMapTabID") || "",
      mapTab = window.sessionStorage.getItem("mapTab") || null,
      map = null,
      bbox = window.sessionStorage.getItem("bbox") || null;

  $.geo.proj = null;

  var forward = false;

  var $locateGraphics;
  var location = null;

  for (var i = 0; i < params.length; i++) {
    var param = params[i].split("=");
    switch (param[0]) {
      case "application":
        startAppID = param[1];
        window.sessionStorage.setItem("startAppID", startAppID);
        forward = true;
        break;

      case "maptab":
        startMapTabID = param[1];
        window.sessionStorage.setItem("startMapTabID", startMapTabID);
        forward = true;
        break;

      case "extent":
        bbox = "[" + param[1] + "]";
        window.sessionStorage.setItem("bbox", bbox);
        break;

      default:
        break;
    }
  }

  if (forward) {
    $.holdReady(true);
    window.location.href = "MobileViewer.aspx";
    return false;
  }

  if (!appConfig) {
    $.holdReady(true);
    $.ajax({
      url: "Services/AppConfig.ashx",
      cache: false,
      dataType: "json",
      success: function (result) {
        appConfig = result;
        window.sessionStorage.setItem("appConfig", JSON.stringify(appConfig));
        $.holdReady(false);
      },
      error: function (xhr) {
        alert(xhr.statusText);
      }
    });
  } else {
    appConfig = JSON.parse(appConfig);
  }

  if (app) {
    app = JSON.parse(app);
  }

  if (mapTab) {
    mapTab = JSON.parse(mapTab);
  }

  $("#appList").bind("pagecreate", function () {
    var appItems = new jQuery(),
        appItem;

    $.each(appConfig.Applications, function () {
      appItem = $('<li><a href="#app">' + this.DisplayName + '</a></li>');
      appItem.children().data("app", this);
      appItems = appItems.add(appItem);

      if (this.ApplicationID == startAppID) {
        app = this;
        window.sessionStorage.setItem("app", JSON.stringify(app));
        window.sessionStorage.removeItem("startAppID");
      }
    });

    if (appItems.length > 0) {
      $(".have-apps").show();
      $("#appList ul").append(appItems);
    } else {
      $(".no-apps").show();
    }

    startAppID = "";

    if (app) {
      $.mobile.changePage($("#app"));
    }
  });

  $("#appList").delegate("a", "click", function () {
    app = $(this).data("app");
    window.sessionStorage.setItem("app", JSON.stringify(app));
  });

  $("#app").bind("pagebeforeshow", function () {
    $(".have-mapTabs").hide();
    $(".no-mapTabs").hide();

    $("#app ul").html("");

    $("#app [data-text='DisplayName']").text(app.DisplayName);
    $("title").text(app.DisplayName);

    var mapTabItems = new jQuery(),
        mapTabItem,
        href = (xferHint === "hotSwitchMapTab" ? "javascript:window.history.back()" : "#mapTab");

    $.each(app.MapTabs, function () {
      mapTabItem = $('<li><a href="' + href + '">' + this.DisplayName + '</a></li>');
      mapTabItem.children().data("mapTab", this);
      mapTabItems = mapTabItems.add(mapTabItem);

      if (this.MapTabID == startMapTabID) {
        mapTab = this;
        window.sessionStorage.setItem("mapTab", JSON.stringify(mapTab));
        window.sessionStorage.removeItem("startMapTabID");
      }
    });

    if (mapTabItems.length > 0) {
      $(".have-mapTabs").show();
      $("#app ul").append(mapTabItems).listview("refresh");
    } else {
      $(".no-mapTabs").show();
    }

    switch (xferHint) {
      case "hotSwitchMapTab":
        xferHint = "";
        break;
    }

    if (startMapTabID) {
      startMapTabID = ""
      $.mobile.changePage($("#mapTab"));
    }
  });

  $("#app").delegate("a", "click", function () {
    mapTab = $(this).data("mapTab");
    window.sessionStorage.setItem("mapTab", JSON.stringify(mapTab));
  });

  $("#mapTab").bind("pagebeforeshow", function () {
    $("#mapTab [data-text='DisplayName']").text(mapTab.DisplayName);
    $("title").text(mapTab.DisplayName);
  });

  $("#mapTab").bind("pagebeforeshow", function () {
    setDesktopLink();
  });

  $(document).bind("pagechange", function (e, mobile) {
    if ($(mobile.toPage).is("#mapTab")) {
      if (map == null) {
        map = $("#mapTab .geomap").geomap({
          tilingScheme: null,
          bbox: bbox ? JSON.parse(bbox) : app.FullExtent,
          mode: "drawPoint",
          services: [
            {
              id: "gpv",
              type: "shingled",
              src: function (view) {
                var mapState = {
                  Application: app.ApplicationID,
                  MapTab: mapTab.MapTabID,
                  Extent: {
                    bbox: view.bbox
                  }
                };

                return "Services/MapImage.ashx?m=GetMapImage&state=" + JSON.stringify(mapState) + "&width=" + view.width + "&height=" + view.height;
              }
            },
            {
              id: "locateGraphics",
              type: "shingled",
              src: function () { return ""; }
            }
          ],
          loadstart: function () {
            $.mobile.showPageLoadingMsg();
          },
          loadend: function () {
            $.mobile.hidePageLoadingMsg();
            showLocation();
          },
          bboxchange: function (e, geo) {
            bbox = "[" + geo.bbox.toString() + "]";
            window.sessionStorage.setItem("bbox", bbox);

            setDesktopLink();
          },
          shape: function (e, geo) {
            sessionStorage.setItem("identifyData", JSON.stringify({
              maptab: mapTab.MapTabID,
              level: "",
              x: geo.coordinates[0],
              y: geo.coordinates[1],
              distance: 20,
              scale: map.geomap("option", "pixelSize"),
              visiblelayers: "*"
            }));

            $.mobile.changePage($("#identify"));

          }
        });

        $locateGraphics = $("#locateGraphics");
      } else {
        map.geomap("resize");
      }
    }
  });

  $(".mapTab-switch").click(function () {
    xferHint = "hotSwitchMapTab";
  });

  $("#cmdLocate").click(function () {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        function (p) {
          var accuracy = p.coords.accuracy;
          var zoomDelta = GPV.coordSys.toUnits(accuracy < 100 ? 100 : accuracy) * 1.05;

          p = GPV.coordSys.toProjected([p.coords.longitude, p.coords.latitude]);
          map.geomap("option", "bbox", [p[0] - zoomDelta, p[1] - zoomDelta, p[0] + zoomDelta, p[1] + zoomDelta]);

          location = {
            geo: { type: "Point", coordinates: p },
            accuracy: GPV.coordSys.toUnits(accuracy)
          };
        },
        function (error) {
        },
        {
          enableHighAccuracy: true,
          maximumAge: 10000
        }
      );
    }

    return false;
  });

  $("#cmdZoomIn").click(function () {
    map.geomap("zoom", +1);
    return false;
  });

  $("#cmdZoomOut").click(function () {
    map.geomap("zoom", -1);
    return false;
  });

  function setDesktopLink() {
    $("#cmdDesktop").attr("href", "Viewer.aspx?application=" + app.ApplicationID + "&maptab=" + mapTab.MapTabID + "&extent=" + (bbox ? JSON.parse(bbox) : app.FullExtent).toString());
  }

  function showLocation() {
    if (location) {
      var coords = [[]];

      for (var a = 0; a <= 360; a += 1) {
        var ar = a * Math.PI / 180;
        var x = location.geo.coordinates[0] + location.accuracy * Math.cos(ar);
        var y = location.geo.coordinates[1] + location.accuracy * Math.sin(ar);
        coords[0].push([x, y]);
      }

      var circle = { type: "Polygon", coordinates: coords };

      $locateGraphics.fadeTo(0, 1).geomap("empty")
        .geomap("append", circle, { color: "#8080FF", fillOpacity: 0.25, strokeWidth: "1px" })
        .geomap("append", location.geo, { width: "0px", height: "0px" }, "<div class='LocateMarker'></div>")
        .delay(1000).fadeOut(2000);

      location = null;
    }
  }

  $("#identify").bind("pageshow", function () {
    $.mobile.showPageLoadingMsg();

    var identifyData = JSON.parse(sessionStorage.getItem("identifyData"));

    $.ajax({
      url: "Identify.aspx?noloading=1&space=0",
      data: identifyData,
      dataType: "html",
      complete: function (xhr) {
        $.mobile.hidePageLoadingMsg();
      },
      success: function (result) {
        var panelTempate = '<div><div class="gpv-collapsible ui-collapsible"><h2 class="ui-collapsible-heading"><a href="#" class="ui-collapsible-heading-toggle ui-btn ui-btn-up-c ui-fullsize ui-btn-icon-left ui-corner-top ui-corner-bottom ui-btn-up-null"><span class="ui-btn-inner ui-corner-top ui-corner-bottom"><span class="ui-btn-text"></span><span class="ui-icon ui-icon-shadow ui-icon-plus"></span></span></a></h2><div class="ui-collapsible-content ui-collapsible-content-collapsed"><p></p></div></div></div>';

        // from jQuery.load:

        // create a dummy div to hold the results
        var loadedHtml = jQuery("<div>")
        // inject the contents of the document in, removing the scripts
        // to avoid any 'Permission Denied' errors in IE
          .append(result.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, ""))

        // locate the specified elements
          .find(".DataList");

        // all links are external
        loadedHtml.find("a").attr("rel", "external");

        // create collapsible content
        var panels = "",
            curPanel = $(panelTempate),
            curPanelContent = curPanel.find("p");

        loadedHtml.find(".RowSetHeader,.RowSetSubheader,.ValueSet").each(function (i) {
          var $this = $(this);
          if ($this.is(".RowSetHeader")) {
            if (i !== 0) {
              panels += curPanel.html();
              curPanel = $(panelTempate);
              curPanelContent = curPanel.find("p");
            }

            curPanel.find(".ui-btn-text").text($this.text());
          } else {
            curPanelContent.append($this);
          }
        });

        panels += curPanel.html();

        $(".identify-content").html(panels);
      },
      error: function (xhr) {
        alert(xhr.statusText);
      }
    });
  });

  $(".identify-content").on("click", ".ui-btn", function (e) {
    $(this).find(".ui-icon").toggleClass("ui-icon-plus ui-icon-minus").end().closest(".ui-collapsible").find(".ui-collapsible-content").toggleClass("ui-collapsible-content-collapsed");
  });

  $("#identify").bind("pagehide", function () {
    $(".identify-content").html('');
  });
})();
