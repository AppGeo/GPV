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
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

public static class EnvelopeExtensions
{
  public static Envelope FromArray(double[] env)
  {
    return env != null && env.Length == 4 ? new Envelope(new Coordinate(env[0], env[1]), new Coordinate(env[2], env[3])) : new Envelope();
  }

  public static Envelope FromDelimitedString(string env)
  {
    return FromDelimitedString(env, ',');
  }

  public static Envelope FromDelimitedString(string env, char separator)
  {
    if (!String.IsNullOrEmpty(env))
    {
      try
      {
        return FromArray(env.Split(separator).Select(o => Convert.ToDouble(o.Trim())).ToArray());
      }
      catch { }
    }

    return null;
  }

  public static void ScaleBy(this Envelope envelope, double scale)
  {
    double dx = envelope.Width * (scale - 1) * 0.5;
    double dy = envelope.Height * (scale - 1) * 0.5;
    envelope.ExpandBy(dx, dy);
  }

  public static double[] ToArray(this Envelope envelope)
  {
    return new double[] { envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY };
  }

  public static string ToDelimitedString(this Envelope envelope)
  {
    return EnvelopeToDelimitedString(envelope, ',');
  }

  public static string ToDelimitedString(this Envelope envelope, char delimiter)
  {
    return EnvelopeToDelimitedString(envelope, delimiter);
  }

  public static IPolygon ToPolygon(this Envelope envelope)
  {
    ILinearRing ring = new LinearRing(new Coordinate[] {
      new Coordinate(envelope.MinX, envelope.MinY),
      new Coordinate(envelope.MinX, envelope.MaxY),
      new Coordinate(envelope.MaxX, envelope.MaxY),
      new Coordinate(envelope.MaxX, envelope.MinY),
      new Coordinate(envelope.MinX, envelope.MinY)
    });

    return new Polygon(ring);
  }

  private static string EnvelopeToDelimitedString(Envelope envelope, char separator)
  {
    return String.Format("{1}{0}{2}{0}{3}{0}{4}", separator, envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
  }
}