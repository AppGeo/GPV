//  Copyright 2017 Applied Geographics, Inc.
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

using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

public static class GeometryExtensions
{
  public static Geometry Translate(this Geometry source, double dx, double dy)
  {
    switch (source.OgcGeometryType)
    {
      case OgcGeometryType.Point:
        return Translate((IPoint)source, dx, dy);

      case OgcGeometryType.LineString:
        return Translate((ILineString)source, dx, dy);

      case OgcGeometryType.Polygon:
        return Translate((IPolygon)source, dx, dy);

      case OgcGeometryType.MultiPoint:
        MultiPoint multiPoint = (MultiPoint)source;
        return new MultiPoint(multiPoint.Geometries.Select(p => Translate((IPoint)p, dx, dy)).ToArray());

      case OgcGeometryType.MultiLineString:
        MultiLineString multiLineString = (MultiLineString)source;
        return new MultiLineString(multiLineString.Geometries.Select(ls => Translate((ILineString)ls, dx, dy)).ToArray());

      case OgcGeometryType.MultiPolygon:
        MultiPolygon multiPolygon = (MultiPolygon)source;
        return new MultiPolygon(multiPolygon.Geometries.Select(p => Translate((IPolygon)p, dx, dy)).ToArray());

      default:
        return source;
    }
  }

  public static Coordinate Translate(this Coordinate c, double dx, double dy)
  {
    return new Coordinate(c.X + dx, c.Y + dy);
  }

  private static Coordinate[] Translate(Coordinate[] cc, double dx, double dy)
  {
    return cc.Select(c => c.Translate(dx, dy)).ToArray();
  }

  private static Point Translate(IPoint point, double dx, double dy)
  {
    return new Point(point.Coordinate.Translate(dx, dy));
  }

  private static LineString Translate(ILineString lineString, double dx, double dy)
  {
    return new LineString(Translate(lineString.Coordinates, dx, dy));
  }

  private static Polygon Translate(IPolygon polygon, double dx, double dy)
  {
    ILinearRing exteriorRing = new LinearRing(Translate(polygon.ExteriorRing.Coordinates, dx, dy));
    ILinearRing[] interiorRings = polygon.InteriorRings.Select(r => new LinearRing(Translate(r.Coordinates, dx, dy))).ToArray();
    return new Polygon(exteriorRing, interiorRings);
  }
}