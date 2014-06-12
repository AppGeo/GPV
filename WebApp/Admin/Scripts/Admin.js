//  Copyright 2012 Applied Geographics, Inc.  Licensed under the Apache License, Version 2.0 http://www.apache.org/licenses/LICENSE-2.0

$(function () {
  var $all = $("body").find("*");
  var $labRefreshing = $("#labRefreshing");

  $("#cmdReloadConfiguration").on("click", function () {
    $all.css("cursor", "wait");
    $labRefreshing.show();

    $.get("?reload=1&v=" + (new Date()).valueOf(), function (result) {
      $all.css("cursor", "");
      $labRefreshing.hide();
    });
  });

  $(".ReportHeader")
    .on("dragstart selectstart", function (e) { e.preventDefault(); })
    .on("click", function (e) {
      var $this = $(this);

      if (!e.shiftKey) {
        $this.toggleClass("Closed").toggleClass("Opened").nextUntil(".ReportSpacer").toggle($this.hasClass("Opened"));
      }
      else {
        var close = $this.hasClass("Opened");
        $(".ReportHeader").toggleClass("Closed", close).toggleClass("Opened", !close);
        $(".ReportRow").toggle(!close);
      }
    });
});