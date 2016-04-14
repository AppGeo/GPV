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

public static class Constants
{
  public const double MetersPerFoot = 0.3048006096012192;    // US survey foot
  public const double FeetPerMeter = 3.2808333333333333;
  public const double FeetPerMile = 5280;
  public const double SquareFeetPerAcre = 43560;

  public const double BasePixelSize = 156543.0339280234;     // Web Mercator level 0 pixel size in meters
  public const double WebMercatorDelta = 20037508.342787;    // Web Mercator level 1 tile width in meters
}