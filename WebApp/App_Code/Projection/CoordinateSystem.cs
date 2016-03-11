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

	public void ToGeodetic(double x, double y, out double lon, out double lat)
	{
    double[] c = new double[] { x, y };
    double[] z = new double[] { 0 };

    Reproject.ReprojectPoints(c, z, Projection, _geodetic, 0, 1);

    lon = c[0];
    lat = c[1];
  }

  public ILineString ToGeodetic(ILineString lineString)
  {
    Coordinate[] points = new Coordinate[lineString.Coordinates.Length];

    for (int i = 0; i < lineString.Coordinates.Length; ++i)
    {
      double lon;
      double lat;
      ToGeodetic(lineString.Coordinates[i].X, lineString.Coordinates[i].Y, out lon, out lat);

      points[i] = new Coordinate(lon, lat);
    }

    return new LineString(points);
  }

  public IPolygon ToGeodetic(IPolygon polygon)
  {
    Coordinate[] points = new Coordinate[polygon.ExteriorRing.Coordinates.Length];

    for (int i = 0; i < polygon.ExteriorRing.Coordinates.Length; ++i)
    {
      double lon;
      double lat;
      ToGeodetic(polygon.ExteriorRing.Coordinates[i].X, polygon.ExteriorRing.Coordinates[i].Y, out lon, out lat);

      points[i] = new Coordinate(lon, lat);
    }

    return new Polygon(new LinearRing(points));
  }

  public void ToProjected(double lon, double lat, out double x, out double y)
	{
    double[] c = new double[] { lon, lat };
    double[] z = new double[] { 0 };

    Reproject.ReprojectPoints(c, z, _geodetic, Projection, 0, 1);

    x = c[0];
    y = c[1];
  }

  public ILineString ToProjected(ILineString lineString)
  {
    Coordinate[] points = new Coordinate[lineString.Coordinates.Length];

    for (int i = 0; i < lineString.Coordinates.Length; ++i)
    {
      double x;
      double y;
      ToProjected(lineString.Coordinates[i].X, lineString.Coordinates[i].Y, out x, out y);

      points[i] = new Coordinate(x, y);
    }

    return new LineString(points);
  }

  public IPolygon ToProjected(IPolygon polygon)
  {
    Coordinate[] points = new Coordinate[polygon.ExteriorRing.Coordinates.Length];

    for (int i = 0; i < polygon.ExteriorRing.Coordinates.Length; ++i)
    {
      double x;
      double y;
      ToProjected(polygon.ExteriorRing.Coordinates[i].X, polygon.ExteriorRing.Coordinates[i].Y, out x, out y);

      points[i] = new Coordinate(x, y);
    }

    return new Polygon(new LinearRing(points));
  }

  public string ToProj4String()
  {
    return Projection.ToProj4String().Trim();
  }
}
