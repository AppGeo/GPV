
  $(function () {

    var urlParams = getURLParams();
    var functions = urlParams.functiontabs.split(',');

    var winHeight = $(window).height();
    var winWidth = $(window).height();

    if (winHeight > 300 && winWidth > 736) {
      $.each(functions, function () {
        $('.' + this).show();
      });
    }


    function getURLParams() {
      var query = window.location.search.substring(1),
        queryRegex = /([^&=]+)=?([^&]*)/g,
        match,
        decode = function (s) { return decodeURIComponent(s.replace(/\+/g, " ")); },
        params = {};

      while (match = queryRegex.exec(query)) {
        params[decode(match[1])] = decode(match[2]);
      }

      return params;
    }
  });
