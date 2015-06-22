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

public class CoordinateSystem
{
	protected Projection Projection;
	protected double FalseEasting;
	protected double FalseNorthing;

	protected CoordinateSystem() { }

	public CoordinateSystem(Projection projection, double falseEasting, double falseNorthing)
	{
		Projection = projection;
		FalseEasting = falseEasting;
		FalseNorthing = falseNorthing;
	}

	public void ToGeodetic(double x, double y, out double lon, out double lat)
	{
		x -= FalseEasting;
		y -= FalseNorthing;

		Projection.ToGeodetic(x, y, out lon, out lat);
	}

	public void ToProjected(double lon, double lat, out double x, out double y)
	{
		Projection.ToProjected(lon, lat, out x, out y);

		x += FalseEasting;
		y += FalseNorthing;
	}

  public string ToProj4String()
  {
    return ToProj4String("meters");
	}

  public string ToProj4String(string units)
  {
    double toMeters = units == "meters" ? 1 : Constants.MetersPerFoot;
    double falseEasting = FalseEasting / toMeters;
    double falseNorthing = FalseNorthing / toMeters;

    return String.Format("{0} +x_0={1} +y_0={2} +to_meter={3}", Projection.ToProj4String(), falseEasting,
      falseNorthing, toMeters);
  }
}
