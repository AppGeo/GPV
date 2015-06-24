var GPV = (function (gpv) {
  $(function () {
    var $container = $("#progress");
    var $bar = $container.find("#progressBar");

    var intervalHandle;
    var intervalCount;
    var interval = 50;
    var intervalMax = 60000 / interval;

    function clear() {
      if (intervalHandle) {
        clearInterval(intervalHandle);
        intervalHandle = null;
      }

      $container.hide();
    }

    function start() {
      if (!intervalHandle) {
        $bar.css("width", "0%");
        $container.show();

        intervalCount = 0;

        intervalHandle = setInterval(function () {
          intervalCount = (intervalCount + 1) % intervalMax;
          $bar.css("width", (intervalCount * 100 / intervalMax) + "%");
        }, interval);
      }
    }

    // =====  public interface  =====

    gpv.progress = {
      start: start,
      clear: clear
    };
  });

  return gpv;
})(GPV || {});
