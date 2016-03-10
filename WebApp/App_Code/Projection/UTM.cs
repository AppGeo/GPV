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

public class UTM : CoordinateSystem
{
	private const double MinLatitude = -80.5;
	private const double MaxLatitude = 84.5;
	private const double MinEasting = 100000;
	private const double MaxEasting = 900000;
	private const double MinNorthing = 0;
	private const double MaxNorthing = 10000000;

	public static int FindZone(double lon, double lat)
	{
		if (lon < -180 || 360 < lon)
		{
			throw new ArgumentException("Longitude out of range");
		}

		if (lat < MinLatitude || MaxLatitude < lat)
		{
			throw new ArgumentException("Latitude out of range");
		}

		double n = lon >= 180 ? lon - 180 : lon + 180;
		int zone = Convert.ToInt32(Math.Floor(n / 6)) + 1;

		if (56 <= lat && lat <= 64 && 3 <= lon && lon <= 6)
		{
			zone = 32;
		}

		if (lat > 72)
		{
			if (0 <= lon && lon <= 9)
			{
				zone = 31;
			}
			if (9 <= lon && lon <= 21)
			{
				zone = 33;
			}
			if (21 <= lon && lon <= 33)
			{
				zone = 35;
			}
			if (33 <= lon && lon <= 42)
			{
				zone = 37;
			}
		}

		return zone;
	}

	public UTM(int zone, Hemisphere hemisphere)
	{
    double centralMeridian = zone * 6.0 - 183;
    double falseNorthing = hemisphere == Hemisphere.North ? 0 : 10000000;
    string proj4Format = "+proj=tmerc +lon_0={0} +lat_0=0 +k=0.9996 +x_0=500000 +y_0={1} +ellps=GRS80 +datum=NAD83 +to_meter=1 +no_defs";

    Projection = ProjectionInfo.FromProj4String(String.Format(proj4Format, centralMeridian, falseNorthing));
	}
}

public enum Hemisphere
{
	North,
	South
}