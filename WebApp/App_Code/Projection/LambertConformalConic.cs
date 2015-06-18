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

public class LambertConformalConic : Projection
{
	// coordinate system

	private double _centralMeridian;
	private double _originLatitude;
	private double _standardParallel1;
	private double _standardParallel2;

	private Spheroid _spheroid;

	// projection constants

	private double _pcN;
	private double _pcF;
	private double _pcR0;

	public LambertConformalConic(double centralMeridian, double originLatitude,
			double standardParallel1, double standardParallel2, Spheroid spheroid)
	{
		_centralMeridian = centralMeridian * RadiansPerDegree;
		_originLatitude = originLatitude * RadiansPerDegree;
		_standardParallel1 = standardParallel1 * RadiansPerDegree;
		_standardParallel2 = standardParallel2 * RadiansPerDegree;
		_spheroid = spheroid;

		Initialize();
	}

	private void Initialize()
	{
		double m1 = GetM(_standardParallel1);
		double m2 = GetM(_standardParallel2);

		double t0 = GetT(_originLatitude);
		double t1 = GetT(_standardParallel1);
		double t2 = GetT(_standardParallel2);

		_pcN = (Math.Log(m1) - Math.Log(m2)) / (Math.Log(t1) - Math.Log(t2));
		_pcF = m1 / (_pcN * Math.Pow(t1, _pcN));
		_pcR0 = GetR(t0);
	}

	public override void ToGeodetic(double x, double y, out double lon, out double lat)
	{
		double dy = _pcR0 - y;
		double r = Math.Sqrt(x * x + dy * dy);
		double theta;

		if (_pcN >= 0)
		{
			theta = Math.Atan2(x, _pcR0 - y);
		}
		else
		{
			r = -r;
			theta = Math.Atan2(-x, y - _pcR0);
		}

		double t = Math.Pow(r / (_spheroid.SemiMajorAxis * _pcF), 1 / _pcN);

		double halfE = _spheroid.Eccentricity / 2;

		double phi;
		double newPhi = HalfPi - 2 * Math.Atan(t);
		int i = 0;

		do
		{
			phi = newPhi;
			double eSinPhi = _spheroid.Eccentricity * Math.Sin(phi);
			newPhi = HalfPi - 2 * Math.Atan(t * Math.Pow((1 - eSinPhi) / (1 + eSinPhi), halfE));
		}
		while (newPhi != phi && ++i < 10);

		double lambda = theta / _pcN + _centralMeridian;

		lon = lambda / RadiansPerDegree;
		lat = phi / RadiansPerDegree;
	}

	public override void ToProjected(double lon, double lat, out double x, out double y)
	{
		lon *= RadiansPerDegree;
		lat *= RadiansPerDegree;

		double t = GetT(lat);
		double r = GetR(t);
		double theta = _pcN * (lon - _centralMeridian);

		x = (r * Math.Sin(theta));
		y = (_pcR0 - r * Math.Cos(theta));
	}

  public override string ToProj4String()
  {
    return String.Format("+proj=lcc +lon_0={0} +lat_0={1} +lat_1={2} +lat_2={3} +ellps=GRS80 +datum=NAD83 +towgs84=0,0,0,0,0,0,0 +no_defs",
      _centralMeridian * DegreesPerRadian, _originLatitude * DegreesPerRadian, _standardParallel1 * DegreesPerRadian,
      _standardParallel2 * DegreesPerRadian);
  }

	private double GetM(double latitude)
	{
		double eSqr = _spheroid.Eccentricity * _spheroid.Eccentricity;
		return Math.Cos(latitude) / Math.Sqrt(1 - (eSqr * Math.Pow(Math.Sin(latitude), 2)));
	}

	private double GetR(double t)
	{
		return _spheroid.SemiMajorAxis * _pcF * Math.Pow(t, _pcN);
	}

	private double GetT(double latitude)
	{
		double sinLat = Math.Sin(latitude);
		return Math.Tan(QuarterPi - latitude / 2) /
			  Math.Pow((1 - _spheroid.Eccentricity * sinLat) /
			  (1 + _spheroid.Eccentricity * sinLat), _spheroid.Eccentricity / 2);
	}
}

