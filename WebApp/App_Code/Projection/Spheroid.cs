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

public class Spheroid
{
	public static Spheroid Clarke1866
	{
		get
		{
			return new Spheroid(6378206.4, 0.0822718542230);
		}
	}

	public static Spheroid GRS80
	{
		get
		{
			return new Spheroid(6378137, 0.0818191910435);
		}
	}

	public static Spheroid WGS84
	{
		get
		{
			return new Spheroid(6378137, 0.0818191908426);
		}
	}

	private double _semiMajorAxis;
	private double _eccentricity;

	public Spheroid(double semiMajorAxis, double eccentricity)
	{
		_semiMajorAxis = semiMajorAxis;
		_eccentricity = eccentricity;
	}

	public double Eccentricity
	{
		get
		{
			return _eccentricity;
		}
	}

	public double Flattening
	{
		get
		{
			return 1 - Math.Sqrt(1 - _eccentricity * _eccentricity);
		}
	}

	public double SemiMajorAxis
	{
		get
		{
			return _semiMajorAxis;
		}
	}

	public double SemiMinorAxis
	{
		get
		{
			return _semiMajorAxis * Math.Sqrt(1 - _eccentricity * _eccentricity);
		}
	}
}
