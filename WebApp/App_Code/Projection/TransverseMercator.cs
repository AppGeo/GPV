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

//
// adapted from tranmerc.c in GEOTRANS (http://earth-info.nga.mil/GandG/geotrans)
//

using System;

public class TransverseMercator : Projection
{
	// constants

	private const double MaxLatitude = 89.99;
	private const double MaxDeltaLongitude = 90.0;
	private const double MinScaleFactor = 0.3;
	private const double MaxScaleFactor = 3.0;

	// coordinate system

	private double _centralMeridian;
	private double _originLatitude;
	private double _scaleFactor;

	private Spheroid _spheroid;

	private double _deltaEasting;
	private double _deltaNorthing;

	// projection constants

	private double _es;
	private double _ebs;

	private double _ap;
	private double _bp;
	private double _cp;
	private double _dp;
	private double _ep;

	private double _tmdo;

	public TransverseMercator(double centralMeridian, double originLatitude, double scaleFactor, Spheroid spheroid)
	{
		if ((originLatitude < -MaxLatitude) || (originLatitude > MaxLatitude))
		{
			throw new ArgumentException("Origin latitude out of range");
		}

		if (centralMeridian > 180)
		{
			centralMeridian -= 360;
		}

		if ((centralMeridian < -180) || (centralMeridian > 180))
		{
			throw new ArgumentException("Origin longitude out of range");
		}

		if ((scaleFactor < MinScaleFactor) || (scaleFactor > MaxScaleFactor))
		{
			throw new ArgumentException("Scale factor out of range");
		}

		_centralMeridian = centralMeridian * RadiansPerDegree;
		_originLatitude = originLatitude * RadiansPerDegree;
		_scaleFactor = scaleFactor;
		_spheroid = spheroid;

		_es = _spheroid.Eccentricity * _spheroid.Eccentricity;
		_ebs = (1.0 / (1.0 - _es)) - 1.0;

		double b = _spheroid.SemiMinorAxis;
		double tn = (_spheroid.SemiMajorAxis - b) / (_spheroid.SemiMajorAxis + b);
		double tn2 = tn * tn;
		double tn3 = tn2 * tn;
		double tn4 = tn3 * tn;
		double tn5 = tn4 * tn;

		_ap = _spheroid.SemiMajorAxis * (1.0 - tn + 5.0 * (tn2 - tn3) / 4.0 + 81.0 * (tn4 - tn5) / 64.0);
		_bp = 3.0 * _spheroid.SemiMajorAxis * (tn - tn2 + 7.0 * (tn3 - tn4) / 8.0 + 55.0 * tn5 / 64.0) / 2.0;
		_cp = 15.0 * _spheroid.SemiMajorAxis * (tn2 - tn3 + 3.0 * (tn4 - tn5) / 4.0) / 16.0;
		_dp = 35.0 * _spheroid.SemiMajorAxis * (tn3 - tn4 + 11.0 * tn5 / 16.0) / 48.0;
		_ep = 315.0 * _spheroid.SemiMajorAxis * (tn4 - tn5) / 512.0;

		double n;
		ToProjected(centralMeridian, MaxLatitude, out n, out _deltaNorthing);
		ToProjected(centralMeridian + MaxDeltaLongitude, 0, out _deltaEasting, out n);

		_tmdo = GetTrueMeridianalDistance(_originLatitude);
	}

	private double GetRadiusOfCurvature(double lat)
	{
		double sinLat = Math.Sin(lat);
		double denom = Math.Sqrt(1 - _es * sinLat * sinLat);
		return _spheroid.SemiMajorAxis * (1 - _es) / (denom * denom * denom);
	}

	private double GetTrueMeridianalDistance(double lat)
	{
		return _ap * lat - _bp * Math.Sin(2 * lat) + _cp * Math.Sin(4 * lat) - _dp * Math.Sin(6 * lat) + _ep * Math.Sin(8 * lat);
	}

	public override void ToProjected(double lon, double lat, out double x, out double y)
	{
		if ((lat < -MaxLatitude) || (lat > MaxLatitude))
		{
			throw new ArgumentException("Latitude out of range");
		}

		if (lon > 180)
		{
			lon -= 360;
		}

		double cm = _centralMeridian * DegreesPerRadian;

		if (lon < (cm - MaxDeltaLongitude) || lon > (cm + MaxDeltaLongitude))
		{
			double tempLon = lon < 0 ? lon + 360 : lon;

			if (cm < 0)
			{
				cm += 360;
			}

			if (tempLon < (cm - MaxDeltaLongitude) || tempLon > (cm + MaxDeltaLongitude))
			{
				throw new ArgumentException("Longitude out of range");
			}
		}

		lon *= RadiansPerDegree;
		lat *= RadiansPerDegree;

		double dlam = lon - _centralMeridian;

		if (dlam > Math.PI)
		{
			dlam -= TwoPi;
		}

		if (dlam < -Math.PI)
		{
			dlam += TwoPi;
		}

		if (Math.Abs(dlam) < 2.0e-10)
		{
			dlam = 0.0;
		}

		double dlam2 = dlam * dlam;
		double dlam3 = dlam2 * dlam;
		double dlam4 = dlam3 * dlam;
		double dlam5 = dlam4 * dlam;
		double dlam6 = dlam5 * dlam;
		double dlam7 = dlam6 * dlam;
		double dlam8 = dlam7 * dlam;

		double s = Math.Sin(lat);
		double c = Math.Cos(lat);
		double c2 = c * c;
		double c3 = c2 * c;
		double c5 = c3 * c2;
		double c7 = c5 * c2;
		double t = Math.Tan(lat);
		double tan2 = t * t;
		double tan3 = tan2 * t;
		double tan4 = tan3 * t;
		double tan5 = tan4 * t;
		double tan6 = tan5 * t;
		double eta = _ebs * c2;
		double eta2 = eta * eta;
		double eta3 = eta2 * eta;
		double eta4 = eta3 * eta;

		double sn = _spheroid.SemiMajorAxis / Math.Sqrt(1 - _es * s * s);
		double tmd = GetTrueMeridianalDistance(lat);

		double t1 = (tmd - _tmdo) * _scaleFactor;
		double t2 = sn * s * c * _scaleFactor / 2.0;
		double t3 = sn * s * c3 * _scaleFactor * (5.0 - tan2 + 9.0 * eta + 4.0 * eta2) / 24.0;
		double t4 = sn * s * c5 * _scaleFactor * (61.0 - 58.0 * tan2 + tan4 + 270.0 * eta - 330.0 * tan2 * eta + 445.0 * eta2 +
				324.0 * eta3 - 680.0 * tan2 * eta2 + 88.0 * eta4 - 600.0 * tan2 * eta3 - 192.0 * tan2 * eta4) / 720.0;
		double t5 = sn * s * c7 * _scaleFactor * (1385.0 - 3111.0 * tan2 + 543.0 * tan4 - tan6) / 40320.0;

		y = t1 + dlam2 * t2 + dlam4 * t3 + dlam6 * t4 + dlam8 * t5;

		double t6 = sn * c * _scaleFactor;
		double t7 = sn * c3 * _scaleFactor * (1.0 - tan2 + eta) / 6.0;
		double t8 = sn * c5 * _scaleFactor * (5.0 - 18.0 * tan2 + tan4 + 14.0 * eta - 58.0 * tan2 * eta +
				13.0 * eta2 + 4.0 * eta3 - 64.0 * tan2 * eta2 - 24.0 * tan2 * eta3) / 120.0;
		double t9 = sn * c7 * _scaleFactor * (61.0 - 479.0 * tan2 + 179.0 * tan4 - tan6) / 5040.0;

		x = dlam * t6 + dlam3 * t7 + dlam5 * t8 + dlam7 * t9;
	}

	public override void ToGeodetic(double x, double y, out double lon, out double lat)
	{
		if ((x < -_deltaEasting) || (x > _deltaEasting))
		{
			throw new ArgumentException("X out of range");
		}

		if ((y < -_deltaNorthing) || (y > _deltaNorthing))
		{
			throw new ArgumentException("Y out of range");
		}

		double tmd = _tmdo + y / _scaleFactor;

		double sr = GetRadiusOfCurvature(0.0);
		double ftphi = tmd / sr;
		double tmdTemp;

		for (int i = 0; i < 5; ++i)
		{
			tmdTemp = GetTrueMeridianalDistance(ftphi);
			sr = GetRadiusOfCurvature(ftphi);
			ftphi = ftphi + (tmd - tmdTemp) / sr;
		}

		double s = Math.Sin(ftphi);
		double c = Math.Cos(ftphi);

		sr = GetRadiusOfCurvature(ftphi);
		double sn = _spheroid.SemiMajorAxis / Math.Sqrt(1 - _es * s * s);

		double t = Math.Tan(ftphi);
		double tan2 = t * t;
		double tan4 = tan2 * tan2;
		double tan6 = tan4 * tan2;
		double eta = _ebs * c * c;
		double eta2 = eta * eta;
		double eta3 = eta2 * eta;
		double eta4 = eta3 * eta;

		double sf2 = _scaleFactor * _scaleFactor;
		double sf3 = sf2 * _scaleFactor;
		double sf4 = sf3 * _scaleFactor;
		double sf5 = sf4 * _scaleFactor;
		double sf6 = sf5 * _scaleFactor;
		double sf7 = sf6 * _scaleFactor;
		double sf8 = sf7 * _scaleFactor;

		double sn3 = sn * sn * sn;
		double sn5 = sn3 * sn * sn;
		double sn7 = sn5 * sn * sn;

		if (Math.Abs(x) < 0.0001)
		{
			x = 0.0;
		}

		double x2 = x * x;
		double x3 = x2 * x;
		double x4 = x3 * x;
		double x5 = x4 * x;
		double x6 = x5 * x;
		double x7 = x6 * x;
		double x8 = x7 * x;

		double t10 = t / (2.0 * sr * sn * sf2);
		double t11 = t * (5.0 + 3.0 * tan2 + eta - 4.0 * eta * eta - 9.0 * tan2 * eta) / (24.0 * sr * sn3 * sf4);
		double t12 = t * (61.0 + 90.0 * tan2 + 46.0 * eta + 45.0 * tan4 - 252.0 * tan2 * eta -
				3.0 * eta2 + 100.0 * eta3 - 66.0 * tan2 * eta2 - 90.0 * tan4 * eta + 88.0 * eta4 +
				225.0 * tan4 * eta2 + 84.0 * tan2 * eta3 - 192.0 * tan2 * eta4) / (720.0 * sr * sn5 * sf6);
		double t13 = t * (1385.0 + 3633.0 * tan2 + 4095.0 * tan4 + 1575.0 * tan6) / (40320.0 * sr * sn7 * sf8);

		lat = ftphi - x2 * t10 + x4 * t11 - x6 * t12 + x8 * t13;

		double t14 = 1.0 / (sn * c * _scaleFactor);
		double t15 = (1.0 + 2.0 * tan2 + eta) / (6.0 * sn3 * c * sf3);
		double t16 = (5.0 + 6.0 * eta + 28.0 * tan2 - 3.0 * eta2 + 8.0 * tan2 * eta + 24.0 * tan4 - 4.0 * eta3 +
				4.0 * tan2 * eta2 + 24.0 * tan2 * eta3) / (120.0 * sn5 * c * sf5);
		double t17 = (61.0 + 662.0 * tan2 + 1320.0 * tan4 + 720.0 * tan6) / (5040.0 * sn7 * c * sf7);
		double dlam = x * t14 - x3 * t15 + x5 * t16 - x7 * t17;

		lon = _centralMeridian + dlam;

		while (lat > HalfPi)
		{
			lat = Math.PI - lat;
			lon += Math.PI;

			if (lon > Math.PI)
			{
				lon -= TwoPi;
			}
		}

		while (lat < -HalfPi)
		{
			lat = -(lat + Math.PI);
			lon += Math.PI;

			if (lon > Math.PI)
			{
				lon -= TwoPi;
			}
		}

		if (lon > TwoPi)
		{
			lon -= TwoPi;
		}

		if (lon < -Math.PI)
		{
			lon += TwoPi;
		}

		lon /= RadiansPerDegree;
		lat /= RadiansPerDegree;
	}

  public override string ToProj4String()
  {
    return String.Format("+proj=tmerc +lon_0={0} +lat_0={1} +k={2} +ellps=GRS80 +datum=NAD83 +towgs84=0,0,0,0,0,0,0 +no_defs",
      _centralMeridian * DegreesPerRadian, _originLatitude * DegreesPerRadian, _scaleFactor);
  }
}
