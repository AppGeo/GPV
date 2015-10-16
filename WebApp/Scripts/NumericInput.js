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

// requires Extensions.js

(function ($) {
  $.fn.numericInput = function (options) {
    var negative = !options || options.negative;
    var decimal = !options || options.decimal;

    this.filter("input[type='text']").each(function () {
      var $this = $(this).on("keydown", function (e) {
        var numeric = $this.data("numeric");
        var current = numeric.previousValue = $this.val();

        // if starting a paste, capture selection start and length

        if (numeric.ctrl && e.which == 86) {
          numeric.selectionStart = $this.prop("selectionStart");
          numeric.selectionEnd = $this.prop("selectionEnd");
        }

        // allow any of the following

        if (numeric.ctrl ||                                      // ctrl sequences
          e.which == 8 ||                                        // backspace
          e.which == 9 ||                                        // tab
          (e.which >= 35 && e.which <= 40) ||                    // arrow keys, home, end
          e.which == 46 ||                                       // delete
          (!numeric.shift && e.which >= 48 && e.which <= 57) ||  // digits from keyboard
          (e.which >= 96 && e.which <= 105) ||                   // digits from keypad
          (negative && (e.which == 189 || e.which == 109) && $this.prop("selectionStart") == 0) ||       // minus as the first character
          (decimal && (e.which == 190 || e.which == 110)  && numeric.previousValue.indexOf(".") < 0)) {  // one decimal point

          // prevent key hold-down repetition

          if (e.which != numeric.key) {
            numeric.key = e.which;

            // capture previous value and text selection

            numeric.previousValue = $this.val();
            numeric.selectionStart = $this.prop("selectionStart");
            numeric.selectionEnd = $this.prop("selectionEnd");
            return;
          }
        }

        // store state of shift and allow it through

        if (e.which == 16) {
          numeric.shift = true;
          return;
        }

        // store state of ctrl and allow it through

        if (e.which == 17) {
          numeric.ctrl = true;
          return;
        }

        // otherwise prevent key from working

        e.preventDefault();
      }).on("keyup", function (e) {
        var numeric = $this.data("numeric");

        if (e.which == numeric.key) {
          numeric.key = -1;
        }

        // reset shift state

        if (e.which == 16) {
          numeric.shift = false;
        }

        // reset ctrl state

        if (e.which == 17) {
          numeric.ctrl = false;
        }
        
        // if not numeric after typing or paste, revert

        if (!numeric.ctrl || e.which == 86) {
          var newValue = $this.val();

          if (newValue.length && newValue != "-" && newValue != "." && newValue != "-." && !newValue.isNumeric()) {
            $this.val(numeric.previousValue);
            $this.prop("selectionStart", numeric.selectionStart);
            $this.prop("selectionEnd", numeric.selectionEnd);
          }
        }
      }).on("change", function () {
        if (!$this.val().isNumeric()) {
          $this.val("");
        }
      }).on("blur", function () {
        var numeric = $this.data("numeric");
        numeric.shift = false;
        numeric.ctrl = false;
      }).data("numeric", { 
        key: -1,
        ctrl: false, 
        shift: false, 
        selectionStart: -1,
        selectionEnd: -1,
        previousValue: "" 
      });
    });

    return this;
  };
})(jQuery);