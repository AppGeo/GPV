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

    // =====  control events  =====

    $("#ddlZoneLevelSelect").on("change", function () {
      var id = $(this).find("option:selected").attr("data-table");

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
        appState.update({ Level: level });
        gpv.viewer.refreshMap();
      }
    });

    // =====  component events  =====

    gpv.on("selectionPanel", "gridFilled", showZoneLevelCounts);

    // =====  private functions  =====

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
      $zoneRows.find("td.Value").text("");

      var $zoneLevelRows = $ZoneLevelTable.find("tr.ZoneLevel");
      $zoneLevelRows.find("td.Value").text("");

      $.each(zone, function (z, v) {
        showCount($zoneRows.filter("tr[data-zone='" + z + "']"), v.count);

        $.each(v.level, function (lev, c) {
          showCount($zoneLevelRows.filter("tr[data-zone='" + z + "'][data-level='" + lev + "']"), c);
        });
      });

      var $levelRows = $ZoneLevelTable.find("tr.Level");
      $levelRows.find("td.Value").text("");

      $.each(level, function (lev, c) {
        showCount($levelRows.filter("tr[data-level='" + lev + "']"), c);
      });
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
              appState.update({ Level: level });
            }

            gpv.viewer.setExtent(result.extent);
          }
        }
      });
    }

    // =====  public interface  =====

    gpv.locationPanel = {
    };

  });

  return gpv;
})(GPV || {});
