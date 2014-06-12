//  Copyright 2012 Applied Geographics, Inc.  Licensed under the Apache License, Version 2.0 http://www.apache.org/licenses/LICENSE-2.0

$(function () {
  $("#ddlManufacturer").on("change", function () {
    $ddlModelNo.empty();
    var manufacturer = $(this).val();

    $.each(GPV.labelData, function (i, v) {
      if (v.manufacturer == manufacturer) {
        $("<option/>").val(v.id).text(v.modelNo).appendTo($ddlModelNo);
      }
    });

    showModelInfo();
  });

  var $ddlModelNo = $("#ddlModelNo").on("change", showModelInfo);

  var $printDirection = $(".PrintDirection").on("click", function () {
    var $this = $(this);

    if (!$this.hasClass("Selected")) {
      $printDirection.removeClass("Selected");
      $this.addClass("Selected");
      $("#hdnPrintDirection").val($this.attr("data-value"));
    }
  });

  function showModelInfo() {
    var id = parseInt($ddlModelNo.val(), 10);

    var modelData = $.grep(GPV.labelData, function (v) {
      return v.id == id;
    })[0];

    $("#labLabelSize").text(modelData.labelSize);
    $("#labLabelsAcross").text(modelData.labelsAcross);
  }
});