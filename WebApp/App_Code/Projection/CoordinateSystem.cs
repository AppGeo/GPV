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

public class CoordinateSystem
{
  private ProjectionInfo _geodetic = KnownCoordinateSystems.Geographic.World.WGS1984;

  protected ProjectionInfo Projection;

  protected CoordinateSystem() { }

	public CoordinateSystem(string proj4String)
	{
    Projection = ProjectionInfo.FromProj4String(proj4String);
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

	public void ToGeodetic(double x, double y, out double lon, out double lat)
	{
    double[] c = new double[] { x, y };
    double[] z = new double[] { 0 };

    Reproject.ReprojectPoints(c, z, Projection, _geodetic, 0, 1);

    lon = c[0];
    lat = c[1];
  }

	public void ToProjected(double lon, double lat, out double x, out double y)
	{
    double[] c = new double[] { lon, lat };
    double[] z = new double[] { 0 };

    Reproject.ReprojectPoints(c, z, _geodetic, Projection, 0, 1);

    x = c[0];
    y = c[1];
  }

  public string ToProj4String()
  {
    return Projection.ToProj4String().Trim();
  }
}
