
// Bounds extensions

L.Bounds.fromArray = function (bbox) {
  return L.bounds(L.point(bbox[0], bbox[1]), L.point(bbox[2], bbox[3]));
};

L.Bounds.prototype.pad = function (bufferRatio) {   // (Number) -> Bounds
  var min = this.min;
  var max = this.max;

  var widthBuffer = (max.x - min.x) * bufferRatio;
  var heightBuffer = (max.y - min.y) * bufferRatio,

  min = L.point(min.x - widthBuffer, min.y - heightBuffer);
  max = L.point(max.x + widthBuffer, max.y + heightBuffer);

  return new L.Bounds(min, max);
};

L.Bounds.prototype.toArray = function () {
  return [ this.min.x, this.min.y, this.max.x, this.max.y ];
};


// Map extensions

L.Map.prototype.fitProjectedBounds = function (bounds) {   // (Bounds)
  var sw = this.options.crs.unproject(L.point(bounds.min.x, bounds.min.y));
  var nw = this.options.crs.unproject(L.point(bounds.min.x, bounds.max.y));
  var ne = this.options.crs.unproject(L.point(bounds.max.x, bounds.max.y));
  var se = this.options.crs.unproject(L.point(bounds.max.x, bounds.min.y));

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


// ShingleLayer

L.ShingleLayer = L.Layer.extend({
  options: {
    pane: 'tilePane',
    boundsFormat: 'bbox'  // 'latlng' or 'bbox'
  },

  getEvents: function () {
    var events = {
      dragend: this._update
    };

    if (this._zoomAnimated) {
      events.zoomanim = this._animateZoom;
      events.zoomend = this._zoomEnd;
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
      this._container = L.DomUtil.create('div', 'leaflet-layer');
      this.getPane().appendChild(this._container);
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

  setUrlBuilder: function (urlBuilder, noRedraw) {
    this._urlBuilder = urlBuilder;

    if (!noRedraw) {
      this.redraw();
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
      position = position._add(size._multiplyBy(0.5 - 1 / (scale * 2))).round();

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

    img.onselectstart = img.onmousemove = L.Util.falseFn;
    img.onload = this._shingleOnLoad;

    return img;
  },

  _shingleOnLoad: function () {
    var layer = this.data.layer;
    var zoom = layer._map.getZoom();

    if (this.data.level === zoom) {
      layer._container.appendChild(this);

      for (var i = layer._shingles.length - 1; i >= 0; --i) {
        var img = layer._shingles[i];

        if (img.data.level !== zoom || this.data.reset) {
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
    var layer = this;

    if (layer._zoomHandle) {
      clearTimeout(layer._zoomHandle);
    }

    layer._zoomHandle = setTimeout(function () {
      layer._zoomHandle = undefined;
      layer._update();
    }, 250);
  }
});

L.shingleLayer = function (options) {
  return new L.ShingleLayer(options);
};


