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

var GPV = (function (gpv) {
  $(function () {
    // =====  convert the flat configuration JSON into an object graph  =====

    var config = gpv.configuration;

    // add IDs to proximities, queries and data tabs from keys

    $.each([config.proximity, config.query, config.dataTab], function () {
      $.each(this, function (k, v) {
        v.id = k;
      });
    });

    // make layers directly reference proximities, queries and data tabs

    $.each(config.layer, function (layerID, layer) {
      layer.id = layerID;
      
      layer.proximity = $.map(layer.proximity, function (proximityID) {
        return config.proximity[proximityID];
      });
      sortByProperty(layer.proximity, "sequenceNo");

      layer.query = $.map(layer.query, function (queryID) {
        return config.query[queryID];
      });
      sortByProperty(layer.query, "sequenceNo");

      layer.dataTab = $.map(layer.dataTab, function (dataTabID) {
        return config.dataTab[dataTabID];
      });
      sortByProperty(layer.dataTab, "sequenceNo");
    })

    // make map tabs directly reference layers

    $.each(config.mapTab, function (i, mapTab) {
      $.each(["target", "selection"], function (i, type) {
        mapTab[type] = $.map(mapTab[type], function (layerID) {
          return config.layer[layerID];
        });
      });
    });

    function sortByProperty(array, prop) {
      array.sort(function (a, b) {
        return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : 0;
      });
    }
  });

  return gpv;
})(GPV || {});