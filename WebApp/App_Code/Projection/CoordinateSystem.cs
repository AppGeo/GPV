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

using System;
using DotSpatial.Projections;
using DotSpatial.Projections.Transforms;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

public class CoordinateSystem
{
  private ProjectionInfo _geodetic = KnownCoordinateSystems.Geographic.World.WGS1984;

  protected ProjectionInfo Projection;

  protected CoordinateSystem() { }

	public CoordinateSystem(string proj4String)
	{
    Projection = ProjectionInfo.FromProj4String(proj4String);
	}

  public bool IsWebMercator
  {
    get
    {
      Spheroid spheroid = Projection.GeographicInfo.Datum.Spheroid;

      return Projection.Transform.Proj4Name == "merc" && Projection.Unit.Name == "Meter" &&
        spheroid.EquatorialRadius == 6378137 && spheroid.PolarRadius == 6378137;
    }
  }

  public string MapUnits
  {
    get
    {
      if (Projection.Unit.Name == "Meter")
      {
        return "meters";
      }
      else if (Projection.Unit.Name == "Foot_US" || Projection.Unit.Name == "Foot")
      {
        return "feet";
      }

      return "unknown";
    }
  }

  public override bool Equals(object obj)
  {
    CoordinateSystem other = obj as CoordinateSystem;

    if (other == null)
    {
      return false;
    }

    return Projection.Equals(other.Projection);
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }

	public Coordinate ToGeodetic(Coordinate c)
	{
    double[] p = new double[] { c.X, c.Y };
    double[] z = new double[] { 0 };

    Reproject.ReprojectPoints(p, z, Projection, _geodetic, 0, 1);

    return new Coordinate(p[0], p[1]);
  }

  public Envelope ToGeodetic(Envelope extent)
  {
    Coordinate min = ToGeodetic(new Coordinate(extent.MinX, extent.MinY));
    Coordinate max = ToGeodetic(new Coordinate(extent.MaxX, extent.MaxY));
    return new Envelope(min, max);
  }

  public IGeometry ToGeodetic(IGeometry geometry)
  {
    Coordinate[] points;

    switch (geometry.OgcGeometryType)
    {
      case OgcGeometryType.Point:
        return new Point(ToGeodetic(geometry.Coordinate));

      case OgcGeometryType.LineString:
        ILineString lineString = (ILineString)geometry;
        points = new Coordinate[lineString.Coordinates.Length];

        for (int i = 0; i < lineString.Coordinates.Length; ++i)
        {
          points[i] = ToGeodetic(lineString.Coordinates[i]);
        }

        return new LineString(points);

      case OgcGeometryType.Polygon:
        IPolygon polygon = (IPolygon)geometry;

        points = new Coordinate[polygon.ExteriorRing.Coordinates.Length];

        for (int i = 0; i < polygon.ExteriorRing.Coordinates.Length; ++i)
        {
          points[i] = ToGeodetic(polygon.ExteriorRing.Coordinates[i]);
        }

        return new Polygon(new LinearRing(points));
    }

    return null;
  }

  public ILineString ToGeodetic(ILineString lineString)
  {
    return (ILineString)ToGeodetic((IGeometry)lineString);
  }

  public IPolygon ToGeodetic(IPolygon polygon)
  {
    return (IPolygon)ToGeodetic((IGeometry)polygon);
  }

  public Coordinate ToProjected(Coordinate c)
	{
    double[] p = new double[] { c.X, c.Y };
    double[] z = new double[] { 0 };

    Reproject.ReprojectPoints(p, z, _geodetic, Projection, 0, 1);

    return new Coordinate(p[0], p[1]);
  }

  public Envelope ToProjected(Envelope extent)
  {
    Coordinate min = ToProjected(new Coordinate(extent.MinX, extent.MinY));
    Coordinate max = ToProjected(new Coordinate(extent.MaxX, extent.MaxY));
    return new Envelope(min, max);
  }

  public IGeometry ToProjected(IGeometry geometry)
  {
    Coordinate[] points;

    switch (geometry.OgcGeometryType)
    {
      case OgcGeometryType.Point:
        return new Point(ToProjected(geometry.Coordinate));

      case OgcGeometryType.LineString:
        ILineString lineString = (ILineString)geometry;
        points = new Coordinate[lineString.Coordinates.Length];

        for (int i = 0; i < lineString.Coordinates.Length; ++i)
        {
          points[i] = ToProjected(lineString.Coordinates[i]);
        }

        return new LineString(points);

      case OgcGeometryType.Polygon:
        IPolygon polygon = (IPolygon)geometry;

        points = new Coordinate[polygon.ExteriorRing.Coordinates.Length];

        for (int i = 0; i < polygon.ExteriorRing.Coordinates.Length; ++i)
        {
          points[i] = ToProjected(polygon.ExteriorRing.Coordinates[i]);
        }

        return new Polygon(new LinearRing(points));
    }

    return null;
  }

  public ILineString ToProjected(ILineString lineString)
  {
    return (ILineString)ToProjected((IGeometry)lineString);
  }

  public IPolygon ToProjected(IPolygon polygon)
  {
    return (IPolygon)ToProjected((IGeometry)polygon);
  }

  public string ToProj4String()
  {
    return Projection.ToProj4String().Trim();
  }
}
