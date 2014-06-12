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

$(function () {
  var tabLastX = -1;
  var tabMoved = false;

  $(".TabScroll").on("touchstart mousedown", ".Tab", function (e) {
    if (!("ontouchend" in document)) {
      e.preventDefault();
    }

    tabLastX = GPV.getEventPoint(e)[0];
  }).on("touchmove mousemove", ".Tab", function (e) {
    if (tabLastX >= 0) {
      var $target = $(this);
      $target.css("cursor", "e-resize");

      var $scroll = $target.parent();
      var $panel = $scroll.parent();
      var $lastTab = $scroll.children(":last");

      var p = GPV.getEventPoint(e);
      var dx = p[0] - tabLastX;
      var scroll = $scroll.offset();
      scroll.left += dx;
      scroll.right = $lastTab.offset().left + $lastTab.outerWidth() + dx;
      var panel = $panel.offset();
      panel.right = panel.left + $panel.width();

      if (scroll.left <= panel.left && (scroll.right >= panel.right || dx > 0)) {
        $scroll.offset(scroll);
        tabMoved = true;
      }

      tabLastX = p[0];
    }
  }).on("click", ".Tab", function () {
    var $target = $(this);
    resetMovableTab($target);

    if (!tabMoved && $target.hasClass("Normal")) {
      $target.siblings(".Selected").addClass("Normal").removeClass("Selected");
      $target.addClass("Selected").removeClass("Normal");
    }
    else {
      return false;
    }
  }).on("touchleave mouseleave", ".Tab", function () {
    resetMovableTab($(this));
  }).on("selectstart", function () {
    return false;
  });

  function resetMovableTab($target) {
    $target.css("cursor", $target.hasClass("Normal") ? "pointer" : "default");
    tabLastX = -1;
    tabMoved = false;
  }
});