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
  $.fn.dateInput = function () {
    this.filter("input[type='text']").each(function () {
      var $this = $(this).on("keydown", function (e) {
        var date = $this.data("date");

        // allow ctrl sequences, backspace, tab, arrow keys, home, end, delete, slash and digits

        if (date.ctrl || e.which == 8 || e.which == 9 || (e.which >= 35 && e.which <= 40) || e.which == 46 || (!date.shift && e.which >= 47 && e.which <= 57) || e.which == 191) {

          // prevent key hold-down repetition

          if (e.which != date.key) {
            date.key = e.which;

            // capture previous value and text selection

            date.previousValue = $this.val();
            date.selectionStart = $this.prop("selectionStart");
            date.selectionEnd = $this.prop("selectionEnd");
            return;
          }
        }

        // store state of shift and allow it through

        if (e.which == 16) {
          date.shift = true;
          return;
        }

        // store state of ctrl and allow it through

        if (e.which == 17) {
          date.ctrl = true;
          return;
        }

        // otherwise prevent key from working

        e.preventDefault();
      }).on("keyup", function (e) {
        var date = $this.data("date");

        if (e.which == date.key) {
          date.key = -1;
        }

        // reset shift state

        if (e.which == 16) {
          date.shift = false;
        }

        // reset ctrl state

        if (e.which == 17) {
          date.ctrl = false;
        }
        
        // if not date after typing or paste, revert

        if (!date.ctrl || e.which == 86) {
          var newValue = $this.val();

          if (!isDate(newValue, true)) {
            $this.val(date.previousValue);
            $this.prop("selectionStart", date.selectionStart);
            $this.prop("selectionEnd", date.selectionEnd);
          }
        }
      }).on("change", function () {
        var date = isDate($this.val());
        $this.val(date && date.length ? date : "");
      }).data("date", { 
        key: -1,
        ctrl: false, 
        shift: false, 
        selectionStart: -1,
        selectionEnd: -1,
        previousValue: "" 
      });
    });

    function isDate(v, partial) {
      var match = /^(?:(\d+)(?:\/(?:(\d+)(?:\/(?:(\d+))?)?)?)?)?$/.exec(v);
      
      if (match) {
        if (!match[0]) {
          return true;
        }

        var m = parseInt(match[1], 10);
        var d = match[2] ? parseInt(match[2], 10): -1;
        var y = match[3] ? parseInt(match[3], 10): -1;

        if (partial || (m >= 1 && d >= 1 && y >= 0)) {
          if (partial && m == 0 && d < 0) {
            return true;
          }
          
          if (1 <= m && m <= 12) {
            if (partial && (d <= 0 || validDay(m, d, 0))) {
              return true;
            }
            
            if (!partial && y >= 0) {
              if (y < 100) {
                var yn = (new Date()).getFullYear();
                y += (Math.floor(yn / 100) - (y < yn ? 0 : 1)) * 100;
              }

              d = validDay(m, d, y);

              if (d) {
                return m + "/" + d + "/" + y;
              }
            }
          }
        }
      }

      return false;
    }

    function isLeapYear(y) {
      return y % 4 == 0 && !(y % 100 == 0 && !(y % 400 == 0));
    }

    function validDay(m, d, y) {
      if (1 <= d && d <= 28) {
        return d;
      }

      if (28 < d && d <= 31) {
        if (m == 1 || m == 3 | m == 5 || m == 7 || m == 8 || m == 10 || m == 12) {
          return d;
        }
        
        if (d <= 30) {
          if (m == 4 || m == 6 || m == 9 || m == 11) {
            return d;
          }

          if (d == 29) {
            return isLeapYear(y) ? 29 : 28;
          }
        }
      }

      return 0;
    }

    return this;
  };
})(jQuery);