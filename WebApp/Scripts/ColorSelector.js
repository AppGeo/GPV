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

(function ($) {

  $.fn.colorSelector = function (method) {
    var key = "colorSelector";
    var bgc = "backgroundColor";

    function drawButton($button, settings) {
      if (settings.enabled) {
        $button.css(bgc, settings.color).removeClass(settings.disabledClass).addClass(settings.enabledClass);
      }
      else {
        $button.css(bgc, settings.disabledColor).removeClass(settings.enabledClass).addClass(settings.disabledClass);
      }
    }

    function getTouch(e) {
      e = "ontouchend" in document ? e.originalEvent.changedTouches[0] : e;
      return [e.pageX, e.pageY];
    }

    function toHsv(c) {
      var r = /rgb\((\d+),\s*(\d+),\s*(\d+)\)/.exec(c);

      if (r && r.length) {
        c = { r: parseInt(r[1], 10), g: parseInt(r[2], 10), b: parseInt(r[3], 10) };
      }
      else {
        r = /#([\d|A-F]{2})([\d|A-F]{2})([\d|A-F]{2})/i.exec(c);
        c = { r: parseInt(r[1], 16), g: parseInt(r[2], 16), b: parseInt(r[3], 16) };
      }

      var max = c.r > c.g ? (c.r > c.b ? c.r : c.b) : (c.g > c.b ? c.g : c.b);
      var min = c.r < c.g ? (c.r < c.b ? c.r : c.b) : (c.g < c.b ? c.g : c.b);

      var delta = max - min;
      var s = max > 0 ? delta * 100 / max : 0;
      var v = max * 100 / 255;
      var h = 0;

      if (max > 0 && delta > 0) {
        var t = max == c.r ? c.g - c.b : max == c.g ? 2 * delta + c.b - c.r : 4 * delta + c.r - c.g;
        h = (360 + t * 60.0 / delta) % 360;
      }

      return { h: h, s: s, v: v };
    }

    function toRgbString(c) {
      var rh = (c.r < 16 ? "0" : "") + c.r.toString(16);
      var gh = (c.g < 16 ? "0" : "") + c.g.toString(16);
      var bh = (c.b < 16 ? "0" : "") + c.b.toString(16);
      return "#" + rh + gh + bh;
    }

    function toRgb(c) {
      var ns = c.s * 0.01;
      var nv = c.v * 0.01;
      var v = parseInt(nv * 255);

      if (this.s == 0) {
        return toRgbString({ r: v, g: v, b: v });
      }
      else {
        var nh = c.h / 60.0;
        var lh = parseInt(Math.floor(nh));

        var f = nh - lh;
        var p = parseInt((nv * (1.0 - ns)) * 255);
        var q = parseInt((nv * (1.0 - ns * f)) * 255);
        var t = parseInt((nv * (1.0 - ns * (1.0 - f))) * 255);

        switch (lh) {
          case 0:
          case 6: return toRgbString({ r: v, g: t, b: p });
          case 1: return toRgbString({ r: q, g: v, b: p });
          case 2: return toRgbString({ r: p, g: v, b: t });
          case 3: return toRgbString({ r: p, g: q, b: v });
          case 4: return toRgbString({ r: t, g: p, b: v });
          case 5: return toRgbString({ r: v, g: p, b: q });
        }
      }
    }

    var methods = {
      init: function (options) {
        return this.each(function () {
          var $button = $(this);

          var settings = {
            color: $button.css(bgc),
            disabledColor: "#808080",
            enabled: true,
            enabledClass: null,
            disabledClass: null,
            selectorClass: null,
            commandClass: null
          };

          if (options) {
            $.extend(settings, options);
          }

          settings.colorChanged = "colorChanged" in options ? [options.colorChanged] : [];

          $button.data(key, settings);
          drawButton($button, settings);

          var $selector, $saturationBar, $hueSlider, $saturationSlider, $valueSlider, $curColorBox, $newColorBox;
          var surfaceClass = "cs" + (new Date()).valueOf();
          var currentColor = null;
          var sliding = null;

          settings.showSelector = function () {
            if (settings.enabled && !$selector) {
              var selectorClass = settings.selectorClass;
              $selector = $('<div style="position: absolute; width: 134px; height: 130px; z-index: 2" />').addClass(selectorClass).addClass(surfaceClass);
              var p = $button.offset();
              var w = $(window).width();
              var h = $(window).height();
              $selector.offset({ left: p.left - 8, top: p.top + 23 });

              currentColor = toHsv(settings.color);

              makeBar("Hue", "10px", function (i) { return toRgb({ h: i * 3.6, s: 100, v: 100 }); });
              $hueSlider = makeSlider("9px", parseInt(currentColor.h / 3.6));

              $saturationBar = makeBar("Saturation", "30px", function (i) { return "#000000"; });
              $saturationSlider = makeSlider("29px", currentColor.s);
              updateSaturationBar();

              makeBar("Value", "50px", function (i) { return toRgb({ h: 0, s: 0, v: i }); });
              $valueSlider = makeSlider("49px", currentColor.v);

              $curColorBox = makeColorBox("Current Color", "10px").css(bgc, settings.color);
              $newColorBox = makeColorBox("New Color", "60px").css(bgc, settings.color);

              var $controls = $('<div style="position: absolute; left: 5px; bottom: 0" />').appendTo($selector);

              $('<button style="margin: 4px; color: black; text-decoration: none;">OK</button>').addClass(settings.commandClass).appendTo($controls).on("touchstart mousedown", function (e) {
                settings.color = toRgb(currentColor);
                $button.css(bgc, settings.color);

                $.each(settings.colorChanged, function (i, fn) {
                  fn(settings.color);
                });

                disposeSelector(selectorClass);
              });

              $('<button style="margin: 4px; color: black; text-decoration: none;">Cancel</button>').addClass(settings.commandClass).appendTo($controls).on("touchstart mousedown", function (e) {
                disposeSelector(selectorClass);
              });

              $("body").append($selector);
              var $surface = $("." + surfaceClass);

              $surface.on("touchstart mousedown", function (e) {
                var p = getTouch(e);
                var x = p[0] - $selector.offset().left

                if (10 <= x && x <= 110) {
                  var y = p[1] - $selector.offset().top;

                  if (y % 20 > 10) {
                    sliding = y <= 20 ? "h" : y <= 40 ? "s" : "v";
                    updateColor(p[0] - $selector.offset().left);
                  }
                }
              });

              $surface.on("touchmove mousemove", function (e) {
                if (sliding) {
                  var p = getTouch(e);

                  if (p[1] - $selector.offset().top < 70) {
                    updateColor(p[0] - $selector.offset().left);
                  }
                  else {
                    sliding = null;
                  }
                }

                return false;
              });

              $surface.on("touchend touchcancel mouseup", function () { sliding = null; return false; });

              $selector.on("touchleave mouseleave", function () { sliding = null; });
            }
          };

          $button.on("touchstart." + key + " mousedown." + key, settings.showSelector);

          function disposeSelector() {
            $selector.remove();
            $selector = null;
            $saturationBar = null;
            $hueSlider = null;
            $saturationSlider = null;
            $valueSlider = null;
            $surface = null;
            $curColorBox = null;
            $newColorBox = null;
          }

          function makeBar(title, top, colorFunction) {
            var $bar = $('<table cellspacing="0" cellpadding="0" style="position: absolute; left: 10px"><tbody><tr style="height: 10px" /></tbody></table>').addClass(surfaceClass).css("top", top).attr("title", title).appendTo($selector).find("tr");

            for (var i = 0; i <= 100; ++i) {
              $('<td style="width: 1px" />').css(bgc, colorFunction(i)).appendTo($bar);
            }

            return $bar;
          }

          function makeColorBox(title, left) {
            return $('<div style="position: absolute; top: 70px; width: 50px; height: 25px" />').attr("title", title).css("left", left).appendTo($selector);
          }

          function makeSlider(top, pos) {
            return $('<div style="position: absolute; width: 1px; height: 10px; border: solid 1px #404040" />').addClass(surfaceClass).css("left", (pos + 9) + "px").css("top", top).appendTo($selector);
          }

          function updateColor(x) {
            x = x < 10 ? 0 : x > 110 ? 100 : x - 10;

            switch (sliding) {
              case "h":
                currentColor.h = x * 3.6;
                $hueSlider.css("left", (x + 9) + "px");
                updateSaturationBar();
                break;
              case "s":
                currentColor.s = x;
                $saturationSlider.css("left", (x + 9) + "px");
                break;
              case "v":
                currentColor.v = x;
                $valueSlider.css("left", (x + 9) + "px");
                break;
            }

            $newColorBox.css(bgc, toRgb(currentColor));
          }

          function updateSaturationBar() {
            $saturationBar.find("td").each(function (i) {
              $(this).css(bgc, toRgb({ h: currentColor.h, s: i, v: 100 }));
            });
          }
        });
      },

      color: function (c) {
        if (c) {
          return this.each(function () {
            var $button = $(this);
            var settings = $button.data(key);
            settings.color = c;

            if (settings.enabled) {
              $button.css(bgc, settings.color);
            }
          });
        }
        else {
          return $(this).data(key).color;
        }
      },

      colorChanged: function (fn) {
        return this.each(function () {
          $(this).data(key).colorChanged.push(fn);
        });
      },

      enabled: function (en) {
        if (typeof en == "boolean") {
          return this.each(function () {
            var $button = $(this);
            var settings = $button.data(key);
            settings.enabled = en;
            drawButton($button, settings);
          });
        }
        else {
          return $(this).data(key).enabled;
        }
      },

      showSelector: function () {
        return $(this).data(key).showSelector();
      },

      dispose: function () {
        return this.each(function () {
          var $button = $(this);
          $button.removeData(key);
          $button.unbind("." + key);
        });
      }
    };

    if (methods[method]) {
      return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
    }
    else if (typeof method === "object" || !method) {
      return methods.init.apply(this, arguments);
    }
    else {
      $.error("Method " + method + " does not exist in colorSelector");
    }
  };

})(jQuery);
