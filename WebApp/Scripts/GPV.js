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

      $.extend(true, map.options, mapOptions);
    }
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

  gpv.loadComplete = loadComplete;
  gpv.loadOptions = loadOptions;
  gpv.on = on;
  gpv.post = post;
  gpv.selectTool = selectTool;
  gpv.store = store;

  return gpv;
})(GPV || {});
