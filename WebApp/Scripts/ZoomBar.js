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
    var $zoomBar = $("#zoomBar");
    var $active = $("#zoomBarActive");
    var mouseIsDown = false;

    var level = 1;
    var maxLevel = parseInt($zoomBar.attr("data-maxlevel"), 10);

    var levelChangedHandlers = [];

    // =====  control events  =====

    $active.on("touchstart mousedown", function (e) {
      mouseIsDown = true;
      moveSlider(e);
    });

    $active.on("touchmove mousemove", function (e) {
      if (mouseIsDown) {
        moveSlider(e);
      }
    });

    $active.on("touchend touchcancel mouseup", function (e) {
      if (mouseIsDown) {
        var v = moveSlider(e);

        if (v != level) {
          level = v;
          triggerLevelChanged();
        }

        mouseIsDown = false;
      }
    });

    $active.on("touchleave mouseleave", function (e) {
      if (mouseIsDown) {
        setSlider(level);
        mouseIsDown = false;
      }
    });

    $("#zoomBarMinus").on("click", function () {
      incrementLevel(-1);
    });

    $("#zoomBarPlus").on("click", function () {
      incrementLevel(1);
    });

    // =====  private functions  =====

    function incrementLevel(v) {
      v += level;

      if (1 <= v && v <= maxLevel) {
        level = v;
        setSlider(level);
        triggerLevelChanged();
      }
    }

    function moveSlider(e) {
      var xOffset = gpv.getEventPoint(e)[0] - $active.offset().left;
      var v = normalize(Math.round((xOffset - 7) / 8) + 1);
      setSlider(v);
      return v;
    }

    function normalize(v) {
      return v < 1 ? 1 : (v < maxLevel ? v : maxLevel);
    }

    function setSlider(v) {
      var offset = $active.offset();
      $("#zoomBarSlider").offset({ left: offset.left + 2 + (v - 1) * 8, top: offset.top });
    }

    function triggerLevelChanged() {
      $.each(levelChangedHandlers, function () {
        this(level);
      });
    }

    // =====  public functions  =====

    function getLevel() {
      return level;
    }

    function setLevel(v) {
      level = normalize(v);
      setSlider(level);
    }

    // =====  public interface  =====

    gpv.zoomBar = {
      getLevel: getLevel,
      levelChanged: function (fn) { levelChangedHandlers.push(fn); },
      setLevel: setLevel
    };
  });

  return gpv;
})(GPV || {});
