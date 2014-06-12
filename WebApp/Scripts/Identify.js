//  Copyright 2012 Applied Geographics, Inc.  Licensed under the Apache License, Version 2.0 http://www.apache.org/licenses/LICENSE-2.0

$(function () {
  var $frmPostBack = $("#frmPostBack");

  if ($frmPostBack.length) {
    $frmPostBack.attr("action", document.location.href).submit();
  }
  else {
    if ($("#autoPrint").length) {
      window.print();
      setTimeout(function () { window.close(); }, 100);
    }
    else {
      window.focus();

      $("#cmdPrint").on("click", function () {
        window.print();
      });

      $(".Value a").on("click", function (e) {
        if ($(this).attr("href").substr(0, 3) == "app") {
          e.preventDefault();
        }
      });
    }
  }
});