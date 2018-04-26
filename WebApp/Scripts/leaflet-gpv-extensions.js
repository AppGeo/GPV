//  Copyright 2016 Applied Geographics, Inc.
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

(function () {

  // Bounds extensions

  L.Bounds.fromArray = function (bbox) {
    return L.bounds(L.point(bbox[0], bbox[1]), L.point(bbox[2], bbox[3]));
  };

  L.Bounds.prototype.pad = function (bufferRatio) {   // (Number) -> Bounds
    var min = this.min;
    var max = this.max;

    var widthBuffer = (max.x - min.x) * bufferRatio;
    var heightBuffer = (max.y - min.y) * bufferRatio;

    min = L.point(min.x - widthBuffer, min.y - heightBuffer);
    max = L.point(max.x + widthBuffer, max.y + heightBuffer);

    return new L.Bounds(min, max);
  };

  L.Bounds.prototype.fit = function (w, h) {    //  (Number, Number) -> Bounds
    var newRatio = !h ? w : w / h;
    var dx = (this.max.x - this.min.x) * 0.5;
    var dy = (this.max.y - this.min.y) * 0.5;
    var oldRatio = dx / dy;

    if (newRatio > oldRatio) {
      dx = dy * newRatio;
    }
    else {
      dy = dx / newRatio;
    }

    var cx = (this.min.x + this.max.x) * 0.5;
    var cy = (this.min.y + this.max.y) * 0.5;

    return L.bounds(L.point(cx - dx, cy - dy), L.point(cx + dx, cy + dy));
  };

  L.Bounds.prototype.getCenter = function () {
    return L.point((this.min.x + this.max.x) * 0.5, (this.min.y + this.max.y) * 0.5);
  };

  L.Bounds.prototype.toArray = function () {
    return [ this.min.x, this.min.y, this.max.x, this.max.y ];
  };


  // Map extensions

  L.Polygon = L.Polygon.extend({
    _convertLatLngs: function (latlngs) {
      return L.Polyline.prototype._convertLatLngs.call(this, latlngs);
    },
  });

  function unproject(map, p) {
    return map.options.crs.unproject(p);
  }

  L.Map.prototype.fitProjectedBounds = function (bounds) {   // (Bounds)
    var sw = unproject(this, L.point(bounds.min.x, bounds.min.y));
    var nw = unproject(this, L.point(bounds.min.x, bounds.max.y));
    var ne = unproject(this, L.point(bounds.max.x, bounds.max.y));
    var se = unproject(this, L.point(bounds.max.x, bounds.min.y));

    var minLat = Math.min(Math.min(Math.min(sw.lat, nw.lat), ne.lat), se.lat);
    var minLng = Math.min(Math.min(Math.min(sw.lng, nw.lng), ne.lng), se.lng);
    var maxLat = Math.max(Math.max(Math.max(sw.lat, nw.lat), ne.lat), se.lat);
    var maxLng = Math.max(Math.max(Math.max(sw.lng, nw.lng), ne.lng), se.lng);

    this.fitBounds(L.latLngBounds(L.latLng(minLat, minLng), L.latLng(maxLat, maxLng)));
  }

  L.Map.prototype.getProjectedBounds = function () {   // -> Bounds
    var size = this.getSize();

    var sw = this.containerPointToLatLng(L.point(0, size.y));
    var ne = this.containerPointToLatLng(L.point(size.x, 0));

    var swPoint = this.options.crs.project(sw);
    var nePoint = this.options.crs.project(ne);

    return L.bounds(swPoint, nePoint);
  }

  L.Map.prototype.getProjectedPixelSize = function () {   // -> Number
    return this.getProjectedBounds().getSize().x / this.getSize().x;
  }

  function fireShapeEvent(map, name, e, mode) {
    map.fire(name, { shiftKey: e.originalEvent.shiftKey, ctrlKey: e.originalEvent.ctrlKey, mode: mode, shape: map._drawing.shape });
  }

  function getMode(map) {
    var mode = map.options.drawing && map.options.drawing.mode;

    if (!mode) {
      mode = 'off';
    }

    if (mode !== map._drawing.lastMode && map._drawing.shape) {
      map.removeLayer(map._drawing.shape);
      delete map._drawing.shape;
    }

    map._drawing.lastMode = mode;
    return mode;
  }

  function getShapeOptions(map) {
    return L.extend({ clickable: false, pointerEvents: 'none' }, map.options.drawing.style)
  }

  function initializeDrawing(map) {
    if (!map._drawing) {
      map._drawing = {};
    }

    return map;
  }

  var isMobile = L.Browser.mobile && L.Browser.touch;

  function updateDrawingShape(map, latlng, mode, append) {
    var crs = map.options.crs;
    var g0, p0, p1;

    switch (mode) {
      case 'rectangle':
        g0 = map._drawing.shape.getLatLngs()[0][0];
        p0 = crs.project(g0);
        p1 = crs.project(latlng);

        map._drawing.shape.setLatLngs([
          g0,
          crs.unproject(L.point(p0.x, p1.y)),
          crs.unproject(L.point(p1.x, p1.y)),
          crs.unproject(L.point(p1.x, p0.y)),
          g0
        ]);

        break;

      case 'circle':
        g0 = map._drawing.shape.getLatLng();
        map._drawing.shape.setRadius(g0.distanceTo(latlng));
        break;

      case 'polyline':
      case 'polygon':
        var latlngs = map._drawing.shape.getLatLngs();

        if (!L.Polyline._flat(latlngs)) {
          latlngs = latlngs[0];
        }

        if (append) {
          latlngs.push(latlng);
        }
        else {
          latlngs[latlngs.length - 1] = latlng;
        }

        map._drawing.shape.setLatLngs(latlngs);
        break;
    }
  };

  L.Map.prototype.on('mousedown', function (e) {
    if (!this.options.drawing || e.originalEvent.button !== 0) {
      return;
    }

    var map = initializeDrawing(this);
    var mode = getMode(map);

    switch (mode) {
      case 'rectangle':
        map._drawing.shape = L.polygon([ e.latlng ], getShapeOptions(map)).addTo(map);
        fireShapeEvent(map, 'shapedrawing', e, mode);
        break;

      case 'circle':
        map._drawing.shape = L.circle(e.latlng, 0.01, getShapeOptions(map)).addTo(map);
        break;
    }
  });

  L.Map.prototype.on('mousemove', function (e) {
    if (!this.options.drawing || !this._drawing || !this._drawing.shape) {
      return;
    }

    var map = this;
    var mode = getMode(map);
    var draggable = mode === 'rectangle' || mode === 'circle';
    var stretchable = mode === 'polyline' || mode === 'polygon';

    if ((e.originalEvent.button === 0 && draggable) || (!isMobile && stretchable)) {
      updateDrawingShape(map, e.latlng, mode, false);
      fireShapeEvent(map, 'shapedrawing', e, mode);
    }
  });

  L.Map.prototype.on('mouseup', function (e) {
    if (!this.options.drawing || !this._drawing || !this._drawing.shape) {
      return;
    }

    var map = this;
    var mode = getMode(map);

    if (mode === 'rectangle' || mode === 'circle') {
      updateDrawingShape(map, e.latlng, mode);
      fireShapeEvent(map, 'shapedrawn', e, mode);
      delete map._drawing.shape;
    }
  });

  L.Map.prototype.on('click', function (e) {
    if (!this.options.drawing) {
      return;
    }

    var map = initializeDrawing(this);
    var mode = getMode(map);

    switch (mode) {
      case 'point':
        map._drawing.shape = e.latlng;
        fireShapeEvent(map, 'shapedrawn', e, mode);
        delete map._drawing.shape;
        break;

      case 'polyline':
      case 'polygon':
        if (!map._drawing.dblclickTimeout) {
          if (!map._drawing.shape) {
            var latlngs = isMobile ? [ e.latlng ] :  [ e.latlng, e.latlng ];
            map._drawing.shape = L[mode](latlngs, getShapeOptions(map)).addTo(map);
          }
          else {
            updateDrawingShape(map, e.latlng, mode, true);
          }

          fireShapeEvent(map, 'shapedrawing', e, mode);
        }
        break;

      case 'text':
        var text = L.text({ 
          latlng: e.latlng, 
          className: map.options.drawing.text.className, 
          color: map.options.drawing.text.color, 
          input: true 
        }).addTo(map);

        text.on('enterkey lostfocus', function () {
          map.removeLayer(text);

          if (text.options.value) {
            text = L.text({ 
              latlng: text.options.latlng, 
              className: map.options.drawing.text.className, 
              color: map.options.drawing.text.color,
              value: text.options.value
            }).addTo(map);

            map._drawing.shape = text;
            fireShapeEvent(map, 'shapedrawn', e, mode);
            delete map._drawing.shape;
          }
        });
        break;
    }
  });

  L.Map.prototype.on('dblclick', function (e) {
    if (!this.options.drawing) {
      return;
    }

    var map = initializeDrawing(this);
    var mode = getMode(map);

    if (!map._drawing.dblclickTimeout) {
      if (mode === 'polyline' || mode === 'polygon') {
        fireShapeEvent(map, 'shapedrawn', e, mode);
        delete map._drawing.shape;

        map._drawing.dblclickTimeout = setTimeout(function () {
          delete map._drawing.dblclickTimeout;
        }, 10);
      }
    }
  });

  // TileLayer extension
  
  // add support for Web Mercator extent and tile size in tile URL template

  L.TileLayer.prototype.getTileUrl = function (coords) {
    var d = 20037508.342787;
    var z = this._getZoomForUrl();
    var w = d / Math.pow(2, z - 1);
    var minx = -d + coords.x * w;
    var miny = d - (coords.y + 1) * w;

    var data = {
      r: L.Browser.retina ? '@2x' : '',
      s: this._getSubdomain(coords),
      x: coords.x,
      y: coords.y,
      z: z,
      minx: minx,
      miny: miny,
      maxx: minx + w,
      maxy: miny + w
    };

    if (this._map && !this._map.options.crs.infinite) {
      var invertedY = this._globalTileRange.max.y - coords.y;
      
      if (this.options.tms) {
        data['y'] = invertedY;
      }

      data['-y'] = invertedY;
    }

    return L.Util.template(this._url, L.extend(data, this.options));
  };

  // make sure all CRS's can measure distance

  L.Proj.CRS.prototype.R = L.CRS.Earth.R;
  L.Proj.CRS.prototype.distance = L.CRS.Earth.distance;

  // Web Mercator CRS extension

  L.CRS.EPSG3857.scaleFactorAtLatitude = function (lat) {
    return 1 / Math.cos(lat * Math.PI / 180);
  };

  // Text

  L.Text = L.Layer.extend({
    options: {
      pane: 'overlayPane'
    },

    getEvents: function () {
      var events = {
        viewreset: this._reset
      };

      if (this._zoomAnimated) {
        events.zoomanim = this._animateZoom;
        events.zoomend = this._reset;
      }

      return events;
    },

    initialize: function (options) {
      L.setOptions(this, options);
    },

    onAdd: function (map) {
      if (!this._text) {
        this._map = map

        this._container = L.DomUtil.create('div', 'leaflet-zoom-animated', this.getPane());
        this._container.style.position = 'absolute';
        this._container.style.zIndex = 2;

        if (this.options.input) {
          this._text = L.DomUtil.create('input', this.options.className);
          this._text.setAttribute('type', 'text');
          this._text.setAttribute('value', this.options.value || '');

          L.DomEvent.disableClickPropagation(this._text);

          L.DomEvent.addListener(this._text, 'blur', function (e) {
            this.fire("lostfocus");
          }, this);

          L.DomEvent.addListener(this._text, 'keydown', function (e) {
            if (e.keyCode === 13) {
              L.DomEvent.preventDefault(e);
              this.fire("enterkey");
            }
          }, this);

          L.DomEvent.addListener(this._text, 'keyup', function (e) {
            this.options.value = this._text.value;
          }, this);
        }
        else {
          var lines = (this.options.value || '').split('\n');

          this._text = L.DomUtil.create('div', this.options.className);
          this._text.appendChild(document.createTextNode(lines[0]));

          for (var i = 1; i < lines.length; ++i) {
            this._text.appendChild(L.DomUtil.create('br'));
            this._text.appendChild(document.createTextNode(lines[i]));
          }

          if (this.options.pointerEvents) {
            this._text.style.pointerEvents = this.options.pointerEvents;
          }
        }

        if (this.options.color) {
          this._text.style.color = this.options.color;
        }

        this._text.style.position = 'absolute';
        this._container.appendChild(this._text);

        if (this.options.input) {
          this._text.selectionStart = this._text.selectionEnd = this.options.value ? this.options.value.length : 0;
          this._text.focus();
        }

        this._reset();
      }
    },

    onRemove: function (map) {
      L.DomUtil.remove(this._container);
      this._container = null;
      this._text = null;
    },

    _animateZoom: function (e) {
      var position = this._map._latLngToNewLayerPoint(this.options.latlng, e.zoom, e.center);
      L.DomUtil.setTransform(this._container, position, 1);
    },

    _reset: function () {
      var pos = this._map.latLngToLayerPoint(this.options.latlng);
      L.DomUtil.setPosition(this._container, pos);
    }
  });

  L.text = function (options) {
    return new L.Text(options);
  };


  // ShingleLayer

  L.ShingleLayer = L.Layer.extend({
    options: {
      pane: 'tilePane',
      boundsFormat: 'bbox',  // 'latlng' or 'bbox'
      preserveOnPan: true
    },

    getEvents: function () {
      var events = {
        dragend: this._dragEnd
      };

      if (this._zoomAnimated) {
        events.zoomanim = this._animateZoom;
        events.zoomend = this._zoomEnd;
        events.moveend = this._moveEnd;
      }

      return events;
    },

    initialize: function (options) {
      this._urlBuilder = options.urlBuilder;
      L.setOptions(this, options);
      this._shingles = [];
    },

    onAdd: function (map) {
      if (!this._container) {
        this._container = L.DomUtil.create('div', 'leaflet-layer', this.getPane());

        if (this.options.hasOwnProperty('zIndex')) {
          this._container.style.zIndex = this.options.zIndex;
        }
      }

      this._update(true);
    },

    onRemove: function (map) {
      L.DomUtil.remove(this._container);
      this._container = null;
    },

    redraw: function () {
      if (this._map) {
        this._update(true);
      }

      return this;
    },

    _animateZoom: function (e) {
      var map = this._map;

      for (var i = 0; i < this._shingles.length; ++i) {
        var img = this._shingles[i];
        var scale = Math.pow(2, e.zoom - img.data.level);

        var nw = img.data.bounds.getNorthWest();
        var se = img.data.bounds.getSouthEast();

        var position = map._latLngToNewLayerPoint(nw, e.zoom, e.center);
        var size = map._latLngToNewLayerPoint(se, e.zoom, e.center)._subtract(position);

        L.DomUtil.setTransform(img, position, scale);
      }
    },

    _createImage: function (size) {
      var img = L.DomUtil.create('img', 'leaflet-shingle' + (this._zoomAnimated ? ' leaflet-zoom-animated' : ''));
      img.style.position = 'absolute';
      img.style.width = size.x + 'px';
      img.style.height = size.y + 'px';
      img.galleryimg = 'no';
      img.data = {};

      if (this.options.hasOwnProperty('opacity')) {
        img.style.opacity = this.options.opacity;
      }

      img.onselectstart = img.onmousemove = L.Util.falseFn;
      img.onload = this._shingleOnLoad;
      img.ondragstart = function () { return false; };

      return img;
    },

    _dragEnd: function () {
      this._dragged = true;
    },

    _moveEnd: function () {
      if (!this._dragged && !this._zoomed) {
        this._update(true);
        return;
      }

      delete this._dragged;
      delete this._zoomed;
      var layer = this;

      if (layer._changeHandle) {
        clearTimeout(layer._changeHandle);
      }

      layer._changeHandle = setTimeout(function () {
        delete layer._changeHandle;
        layer._update();
      }, 500);
    },

    _shingleOnLoad: function () {
      var layer = this.data.layer;
      var zoom = layer._map.getZoom();

      if (this.data.level === zoom) {
        layer._container.appendChild(this);

        for (var i = layer._shingles.length - 1; i >= 0; --i) {
          var img = layer._shingles[i];

          if (img.data.level !== zoom || this.data.reset || !layer.options.preserveOnPan) {
            layer._container.removeChild(img);
            layer._shingles.splice(i, 1);
          }
        }

        delete this.data.reset;
        layer._shingles.push(this);
      }

      layer.fire('shingleload');
    },

    _update: function (reset) {
      if (!this._map || !this._urlBuilder) {
        return;
      }

      var map = this._map;
      var size = map.getSize();

      var img = this._createImage(size);
      img.data.layer = this;
      img.data.level = map.getZoom();
      img.data.reset = reset;

      var position = L.point(0, 0);
      var nw = map.containerPointToLatLng(position);
      var se = map.containerPointToLatLng(size);
      var bounds = img.data.bounds = L.latLngBounds(L.latLng(se.lat, nw.lng), L.latLng(nw.lat, se.lng));

      if (this.options.boundsFormat === 'bbox') {
        se = map.options.crs.project(se);
        nw = map.options.crs.project(nw);
        bounds = [ nw.x, se.y, se.x, nw.y ];
      }

      position = map.containerPointToLayerPoint(position);
      L.DomUtil.setPosition(img, position);

      this._urlBuilder(size, bounds, function (url) {
        img.src = url;
      });
    },

    _zoomEnd: function () {
      this._zoomed = true;
    }
  });

  L.shingleLayer = function (options) {
    return new L.ShingleLayer(options);
  };
})();



