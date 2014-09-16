(function ($) {
  $.fn.numericInput = function () {
    this.filter("input[type='text']").each(function () {
      var $this = $(this).on("keydown", function (e) {
        var numeric = $this.data("numeric");
        var current = numeric.previousValue = $this.val();

        // if starting a paste, capture selection start and length

        if (numeric.ctrl && e.which == 86) {
          numeric.selectionStart = $this.prop("selectionStart");
          numeric.selectionEnd = $this.prop("selectionEnd");
        }

        // allow ctrl sequences, backspace, tab, arrow keys, home, end, digits, minus as the first character and one decimal point

        if (numeric.ctrl || e.which == 8 || e.which == 9 || (e.which >= 35 && e.which <= 40) || (!numeric.shift && e.which >= 48 && e.which <= 57) ||
            (e.which == 189 && $this.prop("selectionStart") == 0) || (e.which == 190 && numeric.previousValue.indexOf(".") < 0)) {

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

          if (newValue.length && newValue != "-" && newValue != "." && newValue != "-." && !isNumeric(newValue)) {
            $this.val(numeric.previousValue);
            $this.prop("selectionStart", numeric.selectionStart);
            $this.prop("selectionEnd", numeric.selectionEnd);
          }
        }
      }).on("change", function () {
        if (!isNumeric($this.val())) {
          $this.val("");
        }
      }).data("numeric", { 
        key: -1,
        ctrl: false, 
        shift: false, 
        selectionStart: -1,
        selectionEnd: -1,
        previousValue: "" 
      });
    });

    function isNumeric(v) {
      return v - parseFloat(v) >= 0;
    }
  };
})(jQuery);