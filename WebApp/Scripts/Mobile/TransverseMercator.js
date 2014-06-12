//  Copyright 2012 Applied Geographics, Inc.  Licensed under the Apache License, Version 2.0 http://www.apache.org/licenses/LICENSE-2.0

var GPV = (function (gpv) {
  gpv.coordSys = function (params) {
    var twoPi = 6.2831853071795864770;
    var halfPi = 1.5707963267948966192;
    var radiansPerDegree = 0.0174532925199432958;
    var degreesPerRadian = 57.295779513082320877;

    var metersPerUnit = params.units == "feet" ? 12.0 / 39.37 : 1;
    var unitsPerMeter = 1 / metersPerUnit;

    var centralMeridian = params.centralMeridian * radiansPerDegree;
    var originLatitude = params.originLatitude * radiansPerDegree;
    var scaleFactor = params.scaleFactor;
    var falseEasting = params.falseEasting;
    var falseNorthing = params.falseNorthing;

    var spheroid = null;

    switch (params.spheroid) {
      case "Clarke1866": spheroid = { semiMajorAxis: 6378206.4, eccentricity: 0.0822718542230 }; break;
      case "GRS80": spheroid = { semiMajorAxis: 6378137, eccentricity: 0.0818191910435 }; break;
      case "WGS84": spheroid = { semiMajorAxis: 6378137, eccentricity: 0.0818191908426 }; break;
    }

    var es = spheroid.eccentricity * spheroid.eccentricity;
    var ebs = (1.0 / (1.0 - es)) - 1.0;

    var b = spheroid.semiMajorAxis * Math.sqrt(1 - es);
    var tn = (spheroid.semiMajorAxis - b) / (spheroid.semiMajorAxis + b);
    var tn2 = tn * tn;
    var tn3 = tn2 * tn;
    var tn4 = tn3 * tn;
    var tn5 = tn4 * tn;

    var ap = spheroid.semiMajorAxis * (1.0 - tn + 5.0 * (tn2 - tn3) / 4.0 + 81.0 * (tn4 - tn5) / 64.0);
    var bp = 3.0 * spheroid.semiMajorAxis * (tn - tn2 + 7.0 * (tn3 - tn4) / 8.0 + 55.0 * tn5 / 64.0) / 2.0;
    var cp = 15.0 * spheroid.semiMajorAxis * (tn2 - tn3 + 3.0 * (tn4 - tn5) / 4.0) / 16.0;
    var dp = 35.0 * spheroid.semiMajorAxis * (tn3 - tn4 + 11.0 * tn5 / 16.0) / 48.0;
    var ep = 315.0 * spheroid.semiMajorAxis * (tn4 - tn5) / 512.0;

    var tmdo = getTrueMeridianalDistance(originLatitude);

    function getRadiusOfCurvature(lat) {
      var sinLat = Math.sin(lat);
      var denom = Math.sqrt(1 - es * sinLat * sinLat);
      return spheroid.semiMajorAxis * (1 - es) / (denom * denom * denom);
    }

    function getTrueMeridianalDistance(lat) {
      return ap * lat - bp * Math.sin(2 * lat) + cp * Math.sin(4 * lat) - dp * Math.sin(6 * lat) + ep * Math.sin(8 * lat);
    }

    function toProjected(g) {
      var lon = g[0] * radiansPerDegree;
      var lat = g[1] * radiansPerDegree;

      if (lon > Math.PI) {
        lon -= twoPi;
      }

      var dlam = lon - centralMeridian;

      if (dlam > Math.PI) {
        dlam -= twoPi;
      }

      if (dlam < -Math.PI) {
        dlam += twoPi;
      }

      if (Math.abs(dlam) < 2.0e-10) {
        dlam = 0.0;
      }

      var dlam2 = dlam * dlam;
      var dlam3 = dlam2 * dlam;
      var dlam4 = dlam3 * dlam;
      var dlam5 = dlam4 * dlam;
      var dlam6 = dlam5 * dlam;
      var dlam7 = dlam6 * dlam;
      var dlam8 = dlam7 * dlam;

      var s = Math.sin(lat);
      var c = Math.cos(lat);
      var c2 = c * c;
      var c3 = c2 * c;
      var c5 = c3 * c2;
      var c7 = c5 * c2;
      var t = Math.tan(lat);
      var tan2 = t * t;
      var tan3 = tan2 * t;
      var tan4 = tan3 * t;
      var tan5 = tan4 * t;
      var tan6 = tan5 * t;
      var eta = ebs * c2;
      var eta2 = eta * eta;
      var eta3 = eta2 * eta;
      var eta4 = eta3 * eta;

      var sn = spheroid.semiMajorAxis / Math.sqrt(1 - es * s * s);
      var tmd = getTrueMeridianalDistance(lat);

      var t1 = (tmd - tmdo) * scaleFactor;
      var t2 = sn * s * c * scaleFactor / 2.0;
      var t3 = sn * s * c3 * scaleFactor * (5.0 - tan2 + 9.0 * eta + 4.0 * eta2) / 24.0;
      var t4 = sn * s * c5 * scaleFactor * (61.0 - 58.0 * tan2 + tan4 + 270.0 * eta - 330.0 * tan2 * eta + 445.0 * eta2 +
                                324.0 * eta3 - 680.0 * tan2 * eta2 + 88.0 * eta4 - 600.0 * tan2 * eta3 - 192.0 * tan2 * eta4) / 720.0;
      var t5 = sn * s * c7 * scaleFactor * (1385.0 - 3111.0 * tan2 + 543.0 * tan4 - tan6) / 40320.0;

      var y = t1 + dlam2 * t2 + dlam4 * t3 + dlam6 * t4 + dlam8 * t5;

      var t6 = sn * c * scaleFactor;
      var t7 = sn * c3 * scaleFactor * (1.0 - tan2 + eta) / 6.0;
      var t8 = sn * c5 * scaleFactor * (5.0 - 18.0 * tan2 + tan4 + 14.0 * eta - 58.0 * tan2 * eta +
                                13.0 * eta2 + 4.0 * eta3 - 64.0 * tan2 * eta2 - 24.0 * tan2 * eta3) / 120.0;
      var t9 = sn * c7 * scaleFactor * (61.0 - 479.0 * tan2 + 179.0 * tan4 - tan6) / 5040.0;

      var x = dlam * t6 + dlam3 * t7 + dlam5 * t8 + dlam7 * t9;

      x = (x + falseEasting) * unitsPerMeter;
      y = (y + falseNorthing) * unitsPerMeter;

      return [x, y];
    }

    function toGeodetic(p) {
      p[0] = (p[0] * metersPerUnit) - falseEasting;
      p[1] = (p[1] * metersPerUnit) - falseNorthing;

      var tmd = tmdo + p[1] / scaleFactor;

      var sr = getRadiusOfCurvature(0.0);
      var ftphi = tmd / sr;
      var tmdTemp;

      for (var i = 0; i < 5; ++i) {
        tmdTemp = getTrueMeridianalDistance(ftphi);
        sr = getRadiusOfCurvature(ftphi);
        ftphi = ftphi + (tmd - tmdTemp) / sr;
      }

      var s = Math.sin(ftphi);
      var c = Math.cos(ftphi);

      sr = getRadiusOfCurvature(ftphi);
      var sn = spheroid.semiMajorAxis / Math.sqrt(1 - es * s * s);

      var t = Math.tan(ftphi);
      var tan2 = t * t;
      var tan4 = tan2 * tan2;
      var tan6 = tan4 * tan2;
      var eta = ebs * c * c;
      var eta2 = eta * eta;
      var eta3 = eta2 * eta;
      var eta4 = eta3 * eta;

      var sf2 = scaleFactor * scaleFactor;
      var sf3 = sf2 * scaleFactor;
      var sf4 = sf3 * scaleFactor;
      var sf5 = sf4 * scaleFactor;
      var sf6 = sf5 * scaleFactor;
      var sf7 = sf6 * scaleFactor;
      var sf8 = sf7 * scaleFactor;

      var sn3 = sn * sn * sn;
      var sn5 = sn3 * sn * sn;
      var sn7 = sn5 * sn * sn;

      var x = p[0];

      if (Math.abs(x) < 0.0001) {
        x = 0.0;
      }

      var x2 = x * x;
      var x3 = x2 * x;
      var x4 = x3 * x;
      var x5 = x4 * x;
      var x6 = x5 * x;
      var x7 = x6 * x;
      var x8 = x7 * x;

      var t10 = t / (2.0 * sr * sn * sf2);
      var t11 = t * (5.0 + 3.0 * tan2 + eta - 4.0 * eta * eta - 9.0 * tan2 * eta) / (24.0 * sr * sn3 * sf4);
      var t12 = t * (61.0 + 90.0 * tan2 + 46.0 * eta + 45.0 * tan4 - 252.0 * tan2 * eta -
                                3.0 * eta2 + 100.0 * eta3 - 66.0 * tan2 * eta2 - 90.0 * tan4 * eta + 88.0 * eta4 +
                                225.0 * tan4 * eta2 + 84.0 * tan2 * eta3 - 192.0 * tan2 * eta4) / (720.0 * sr * sn5 * sf6);
      var t13 = t * (1385.0 + 3633.0 * tan2 + 4095.0 * tan4 + 1575.0 * tan6) / (40320.0 * sr * sn7 * sf8);

      var phi = ftphi - x2 * t10 + x4 * t11 - x6 * t12 + x8 * t13;

      var t14 = 1.0 / (sn * c * scaleFactor);
      var t15 = (1.0 + 2.0 * tan2 + eta) / (6.0 * sn3 * c * sf3);
      var t16 = (5.0 + 6.0 * eta + 28.0 * tan2 - 3.0 * eta2 + 8.0 * tan2 * eta + 24.0 * tan4 - 4.0 * eta3 +
                                4.0 * tan2 * eta2 + 24.0 * tan2 * eta3) / (120.0 * sn5 * c * sf5);
      var t17 = (61.0 + 662.0 * tan2 + 1320.0 * tan4 + 720.0 * tan6) / (5040.0 * sn7 * c * sf7);
      var dlam = x * t14 - x3 * t15 + x5 * t16 - x7 * t17;

      var lambda = centralMeridian + dlam;

      while (phi > halfPi) {
        phi = Math.PI - phi;
        lambda += Math.PI;

        if (lambda > Math.PI) {
          lambda -= twoPi;
        }
      }

      while (phi < -halfPi) {
        phi = -(phi + Math.PI);
        lambda += Math.PI;

        if (lambda > Math.PI) {
          lambda -= twoPi;
        }
      }

      if (lambda > twoPi) {
        lambda -= twoPi;
      }

      if (lambda < -Math.PI) {
        lambda += twoPi;
      }

      var lon = lambda * degreesPerRadian;
      var lat = phi * degreesPerRadian;

      return [lon, lat];
    }

    var tm = {
      toProjected: toProjected,
      toGeodetic: toGeodetic,
      toUnits: function (meters) { return meters * unitsPerMeter; }
    }

    return tm;
  };

  return gpv;
})(GPV || {});