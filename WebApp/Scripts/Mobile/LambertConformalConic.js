//  Copyright 2012 Applied Geographics, Inc.  Licensed under the Apache License, Version 2.0 http://www.apache.org/licenses/LICENSE-2.0

var GPV = (function (gpv) {
  gpv.coordSys = function (params) {
    var halfPi = 1.5707963267948966192;
    var quarterPi = 0.7853981633974483096;
    var radiansPerDegree = 0.0174532925199432958;
    var degreesPerRadian = 57.295779513082320877;

    var metersPerUnit = params.units == "feet" ? 12.0 / 39.37 : 1;
    var unitsPerMeter = 1 / metersPerUnit;

    var centralMeridian = params.centralMeridian * radiansPerDegree;
    var originLatitude = params.originLatitude * radiansPerDegree;
    var standardParallel1 = params.standardParallel1 * radiansPerDegree;
    var standardParallel2 = params.standardParallel2 * radiansPerDegree;
    var falseEasting = params.falseEasting;
    var falseNorthing = params.falseNorthing;

    var spheroid = null;

    switch (params.spheroid) {
      case "Clarke1866": spheroid = { semiMajorAxis: 6378206.4, eccentricity: 0.0822718542230 }; break;
      case "GRS80": spheroid = { semiMajorAxis: 6378137, eccentricity: 0.0818191910435 }; break;
      case "WGS84": spheroid = { semiMajorAxis: 6378137, eccentricity: 0.0818191908426 }; break;
    }

    var m1 = getM(standardParallel1);
    var m2 = getM(standardParallel2);

    var t0 = getT(originLatitude);
    var t1 = getT(standardParallel1);
    var t2 = getT(standardParallel2);

    var pcN = (Math.log(m1) - Math.log(m2)) / (Math.log(t1) - Math.log(t2));
    var pcF = m1 / (pcN * Math.pow(t1, pcN));
    var pcR0 = getR(t0);

    function getM(lat) {
      var eSqr = spheroid.eccentricity * spheroid.eccentricity;
      return Math.cos(lat) / Math.sqrt(1 - (eSqr * Math.pow(Math.sin(lat), 2)));
    }

    function getR(t) {
      return spheroid.semiMajorAxis * pcF * Math.pow(t, pcN);
    }

    function getT(lat) {
      var sinLat = Math.sin(lat);
      return Math.tan(quarterPi - lat / 2) / Math.pow((1 - spheroid.eccentricity * sinLat) /
        (1 + spheroid.eccentricity * sinLat), spheroid.eccentricity / 2);
    }

    function toProjected(g) {
      var lon = g[0] * radiansPerDegree;
      var lat = g[1] * radiansPerDegree;

      var t = getT(lat);
      var r = getR(t);
      var theta = pcN * (lon - centralMeridian);

      var x = (r * Math.sin(theta) + falseEasting) * unitsPerMeter;
      var y = ((pcR0 - r * Math.cos(theta)) + falseNorthing) * unitsPerMeter;

      return [x, y];
    }

    function toGeodetic(p) {
      p[0] = (p[0] - falseEasting) * metersPerUnit;
      p[1] = (p[1] - falseNorthing) * metersPerUnit;

      var r = Math.sqrt(p[0] * p[0] + Math.pow(pcR0 - p[1], 2));
      var theta;

      if (pcN >= 0) {
        theta = Math.atan2(p[0], pcR0 - p[1]);
      }
      else {
        r = -r;
        theta = Math.atan2(-p[0], p[1] - pcR0);
      }

      var t = Math.pow(r / (spheroid.semiMajorAxis * pcF), 1 / pcN);

      var halfE = spheroid.eccentricity / 2;

      var phi;
      var newPhi = halfPi - 2 * Math.atan(t);
      var i = 0;

      do {
        phi = newPhi;
        var eSinPhi = spheroid.eccentricity * Math.sin(phi);
        newPhi = halfPi - 2 * Math.atan(t * Math.pow((1 - eSinPhi) / (1 + eSinPhi), halfE));
      }
      while (newPhi != phi && ++i < 10);

      var lambda = theta / pcN + centralMeridian;

      var lon = lambda * degreesPerRadian;
      var lat = phi * degreesPerRadian;

      return [lon, lat];
    }

    var lcc = {
      toProjected: toProjected,
      toGeodetic: toGeodetic,
      toUnits: function (meters) { return meters * unitsPerMeter; }
    };

    return lcc;
  };

  return gpv;
})(GPV || {});
