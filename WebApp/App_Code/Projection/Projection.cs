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

public abstract class Projection
{
	// math constants

	protected const double TwoPi = 6.2831853071795864769;
	protected const double HalfPi = 1.5707963267948966192;
	protected const double QuarterPi = 0.7853981633974483096;
	protected const double RadiansPerDegree = 0.0174532925199432958;
	protected const double DegreesPerRadian = 57.295779513082320877;

	public abstract void ToGeodetic(double x, double y, out double lon, out double lat);

	public abstract void ToProjected(double lon, double lat, out double x, out double y);

  public abstract string ToProj4String();
}
