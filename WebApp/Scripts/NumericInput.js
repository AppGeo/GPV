(function ($) {
  $.fn.numeric = function () {
    this.filter("input[type='text']").each(function () {
      var $this = $(this).on("keydown", function (e) {
        var numeric = $this.data("numeric");
        var current = numeric.oldValue = $this.val();

        // if starting a paste, capture selection start and length

        if (numeric.ctrl && e.which == 86) {
          numeric.selectionStart = $this.prop("selectionStart");
          numeric.selectionEnd = $this.prop("selectionEnd");
        }

        // allow ctrl sequences, backspace, tab, arrow keys, home, end, and digits

        if (numeric.ctrl || e.which == 8 || e.which == 9 || (e.which >= 35 && e.which <= 40) || (!numeric.shift && e.which >= 48 && e.which <= 57)) {
          return;
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

        // allow minus only as the first character

        if (e.which == 189 && $this.prop("selectionStart") == 0) {
          return;
        }

        // allow only one decimal point

        if (e.which == 190 && current.indexOf(".") < 0) {
          return;
        }

        e.preventDefault();
      }).on("keyup", function (e) {
        var numeric = $this.data("numeric");

        // reset shift state

        if (e.which == 16) {
          numeric.shift = false;
        }

        // reset ctrl state

        if (e.which == 17) {
          numeric.ctrl = false;
        }
        
        // if not numeric after paste, revert

        if (e.which == 86 && numeric.ctrl) {
          var newValue = $this.val();

          if (!isNumeric(newValue)) {
            $this.val(numeric.oldValue);
            $this.prop("selectionStart", numeric.selectionStart);
            $this.prop("selectionEnd", numeric.selectionEnd);
          }
        }
      }).on("change", function () {
        if (!isNumeric($this.val())) {
          $this.val("");
        }
      }).data("numeric", { 
        ctrl: false, 
        shift: false, 
        selectionStart: -1,
        selectionEnd: -1,
        current: "" 
      });
    });

    function isNumeric(v) {
      return v - parseFloat(v) >= 0;
    }
  };
})(jQuery);