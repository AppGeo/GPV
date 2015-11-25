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
  var handlers = [];

  // =====  private functions  =====

  function latLngsToSearchShape(map, latLngs) {
    var p0 = map.options.crs.project(latLngs[0][0]);
    var p1 = map.options.crs.project(latLngs[0][2]);
    var dx = Math.abs(p0.x - p1.x);
    var dy = Math.abs(p0.y - p1.y);
        
    var distance = map.getProjectedPixelSize() * searchDistance();
    var geo;

    if (dx <= distance && dy <= distance) {
      geo = [ (p0.x + p1.x) * 0.5, (p0.y + p1.y) * 0.5, distance ];
    }
    else {
      geo = [ Math.min(p0.x, p1.x), Math.min(p0.y, p1.y), Math.max(p0.x, p1.x), Math.max(p0.y, p1.y) ];
    }

    return geo;
  }

  function loadComplete() {
    $.each(handlers, function (i, v) {
      gpv[v.target][v.event](v.handler);
    });
  }

  function loadOptions($target, list) {
    var previous = $target.val();
    var changed = previous || list.length;
    $target.empty();

    $.each(list, function () {
      var same = this.id == previous;
      changed = changed && !same;
      $("<option/>").val(this.id).text(this.name).prop("selected", same).appendTo($target);
    });

    return changed;
  }

  function on(target, event, handler) {
    handlers.push({
      target: target,
      event: event,
      handler: handler
    });
  }

  function post(args) {
    args.type = "POST";
    args.dataType = "json";
    args.error = function (xhr) {
      var message = xhr.getResponseHeader("GPV-Error");

      if (message) {
        alert(message);
      }
    }

    return $.ajax(args);
  }

  function selectTool($tool, map, mapOptions) {
    if (!$tool.hasClass("Disabled")) {
      $(".MapTool").not($tool.addClass("Selected")).removeClass("Selected");

      mapOptions = mapOptions || {};
      $(map.getPanes().mapPane).css("cursor", mapOptions.cursor || "");
      delete mapOptions.cursor;

      $.each(['dragging', 'touchZoom', 'doubleClickZoom', 'scrollWheelZoom', 'boxZoom'], function (i, handler) {
        var toggle = handler in mapOptions ? mapOptions[handler] : true;
        map[handler][toggle ? 'enable' : 'disable']();
      });

      $.extend(true, map.options, mapOptions);
    }
  }

  function searchDistance() {
    return L.Browser.mobile && L.Browser.touch ? 16 : 4;
  }

  function store(name, value) {
    var s = $.cookie("store");
    s = s ? JSON.parse(s) : {};

    if (arguments.length == 1) {
      return name in s ? s[name] : null;
    }
    else {
      store[name] = value;
      $.cookie("store", JSON.stringify(s), { expires: 60 });
    }
  }

  // =====  public interface  =====

  gpv.latLngsToSearchShape = latLngsToSearchShape;
  gpv.loadComplete = loadComplete;
  gpv.loadOptions = loadOptions;
  gpv.on = on;
  gpv.post = post;
  gpv.selectTool = selectTool;
  gpv.searchDistance = searchDistance;
  gpv.store = store;

  return gpv;
})(GPV || {});
