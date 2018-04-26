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

$(function () {
  var jsonDateRegex = /^\/Date\((-?\d+)\)\//;
  var objectTypeRegex = /^\[object (.*)\]$/;

  Date.fromJson = function (s) {
    var match = jsonDateRegex.exec(s);
    return match ? new Date(parseInt(match[1], 10)) : null;
  };

  Date.prototype.format = function () {
    return (this.getMonth() + 1).zeroPad(2) + "/" + this.getDate().zeroPad(2) + "/" + (this.getFullYear() % 100).zeroPad(2);
  };

  Number.prototype.zeroPad = function (n) {
    var s = this.toString();

    while (s.length < n) {
      s = "0" + s;
    }

    return s;
  };

  Number.prototype.format = function (d) {
    var v = this.toFixed(d).split("");
    var n = v.length - (d ? d + 1 : 0);

    for (var i = ((n - 1) % 3) + 1; i < n; i += 4) {
      v.splice(i, 0, ",");
      n += 1;
    }

    return v.join("");
  }

  Object.getType = function (value) {
    return value == null ? "null" : Object.prototype.toString.call(value).match(objectTypeRegex)[1].toLowerCase();
  };

  String.prototype.isNumeric = function (allowSeparators) {
    var s = this;

    if (allowSeparators && this.indexOf(",", 0) > -1) {
      var v = this.split(".");

      if (v.length > 1 && v[1].indexOf(",", 0) > -1) {
        return false;
      }

      v = v[0].split(",");

      if (v[0].length > 3 || !v[0].isNumeric) {
        return false;
      }

      for (var i = 1; i < v.length; ++i) {
        if (v[i].length != 3 || !v[i].isNumeric) {
          return false;
        }
      }

      s = s.replace(/,/g, "");
    }

    return s - parseFloat(s) >= 0;
  };
});