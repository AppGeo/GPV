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
    var map;
    var externalMapState;

    // =====  control events  =====

    $(".share-type").on("click", function (e) {
      $(".share").hide();
      var panel = "#pnl" + e.target.id.replace("cmdFor", "");
      $(panel).fadeIn(600);
    });

    var $cmdExternalMap = $("#cmdExternalMap").on("click", function(e) {
      if (externalMapState === 'posted') {
        externalMapState = 'clicked';
      }
      else {
        e.preventDefault();
        var url = $(this).attr("href");
        window.open(url, "_blank");
      }
    })

    $("#cmdPrint").on("click", function () {
      var $form = $("#frmPrint");
      $form.find('[name="state"]').val(gpv.appState.toJson());
      $form.find('[name="width"]').val(map.getSize().x);
      $form.submit();
    });

    $("#cmdSaveMap").on("click", function () {
      var $form = $("#frmSaveMap");
      $form.find('[name="m"]').val($("#ddlSaveMap").val() == "image" ? "SaveMapImage" : "SaveMapKml");
      $form.find('[name="state"]').val(gpv.appState.toJson());
      $form.find('[name="width"]').val(map.getSize().x);
      $form.find('[name="height"]').val(map.getSize().y);
      $form.submit();
    });

    var $ddlExternalMap = $("#ddlExternalMap").on("change", setExternalMap);

    var $ddlPrintTemplate = $("#ddlPrintTemplate").on("change", function () {
      showPrintTemplateInputs();
      updatePrintScale();
    });

    $("#tboPrintScale").numericInput({ negative: false, decimal: false }).on("keyup", function () {
      $("#optPrintScaleInput").trigger("click");
    });

    // =====  component events  =====

    gpv.on("viewer", "extentChanged", setExternalMap);
    gpv.on("viewer", "mapRefreshed", updatePrintScale);

    gpv.appState.updated(function () {
      $("#pnlEmail").fadeOut(600);
    });

    // =====  private functions  =====

    function showPrintTemplateInputs() {
      $(".printInput").hide()
      $('[data-templateid="' + $ddlPrintTemplate.val() + '"]').fadeIn();
    }
    
    function updatePrintScale() {
      if (map) {
        var extent = map.getProjectedBounds();
        var extentWidth = (extent.max.x - extent.min.x) / (gpv.settings.mapUnits === "feet" ? 1 : 0.3048);
        var mapWidth = map.getSize().x;
        $("#labPrintScaleCurrent").text("Current (1\" = " + Math.round(extentWidth * 96 / mapWidth).format() + " ft)");

        mapWidth = gpv.configuration.printTemplate[$ddlPrintTemplate.val()].mapWidth || 7;
        $("#labPrintScaleWidth").text("Preserve extent width (1\" = " + Math.round(extentWidth / mapWidth).format() + " ft)");
      }
    }

    function setExternalMap() {
      var externalName = $ddlExternalMap.val();
      var extent = gpv.appState.Extent.bbox;

      if (!externalName || !extent) {
        $cmdExternalMap.attr("href", "#").addClass("Disabled");
        return;
      }

      externalMapState = 'posted';

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
            $cmdExternalMap.attr("href", result.url).removeClass("Disabled");
          }
        },
        complete: function () {
          var state = externalMapState;
          externalMapState = undefined;

          if (state === 'clicked' && !$cmdExternalMap.hasClass('Disabled')) {
            $cmdExternalMap.trigger('click');
          }
        }
      });
    }

    // =====  public functions  =====

    function setMap(m) {
      map = m;
    }

    // =====  public interface  =====

    gpv.sharePanel = {
      setMap: setMap
    };

    showPrintTemplateInputs();
  });

  return gpv;
})(GPV || {});
