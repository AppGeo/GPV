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
using GeoAPI.Geometries;

public class MGRS : GridSystem
{
	private const string _zoneLetters = "CDEFGHJKLMNPQRSTUVWX";
	private const string _columnLetters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
	private const string _rowLetters = "ABCDEFGHJKLMNPQRSTUV";

	private int _precision = 5;

	public MGRS() { }

	public MGRS(int precision)
	{
		if (precision < 1 || 5 < precision)
		{
			throw new ArgumentException("Precision must be between 1 and 5");
		}

		_precision = precision;
	}

	public override Coordinate ToGeodetic(string gridValue)
	{
		gridValue = gridValue.Replace(" ", String.Empty).ToUpper();

		if (gridValue.Length < 6)
		{
			throw new ArgumentException("Grid value is too short");
		}

		if (!Char.IsNumber(gridValue[0]))
		{
			throw new ArgumentException("Grid value does not start with a zone number");
		}

		int i = 0;

		if (Char.IsNumber(gridValue[i + 1]))
		{
			++i;
		}

		int zone = Convert.ToInt32(gridValue.Substring(0, i + 1));

		if (zone < 1 || 60 < zone)
		{
			throw new ArgumentException("Invalid zone number, must be between 1 and 60");
		}

		int zoneIndex = _zoneLetters.IndexOf(gridValue[++i]);

		if (zoneIndex < 0)
		{
			throw new ArgumentException("Invalid zone letter");
		}

		int setNumber = (zone - 1) % 6;
		int columnIndex = _columnLetters.IndexOf(gridValue[++i]) - (setNumber % 3) * 8;

		if (columnIndex < 0 || 8 < columnIndex)
		{
			throw new ArgumentException("Invalid column letter");
		}

		int rowIndex = _rowLetters.IndexOf(gridValue[++i]);

		if (rowIndex < 0)
		{
			throw new ArgumentException("Invalid row letter");
		}

		char[] letters = gridValue.Substring(i - 2, 3).ToCharArray();

		for (int j = ++i; j < gridValue.Length; ++j)
		{
			if (!Char.IsNumber(gridValue[j]))
			{
				throw new ArgumentException("Invalid coordinates, contains non-numeric character");
			}
		}

		int coordLength = gridValue.Length - i;

		if (coordLength % 2 == 1)
		{
			throw new ArgumentException("Invalid coordinates, X and Y are of unequal length");
		}

		if (coordLength > 10)
		{
			throw new ArgumentException("Invalid coordinates, too long");
		}

		coordLength /= 2;
		double factor = Math.Pow(10, 5 - coordLength);

		double x = Convert.ToDouble(gridValue.Substring(i, coordLength));
		double y = Convert.ToDouble(gridValue.Substring(i + coordLength, coordLength));

		x += (columnIndex + 1) * 100000;

		int yOffset = setNumber % 2 == 1 ? 1500000 : 0;
		y = (rowIndex * 100000 + yOffset + y) % 2000000;

		double minZoneNorthing = zoneIndex < 10 ? (zoneIndex + 1) * 900000 : (zoneIndex % 10) * 900000;

		while (y < minZoneNorthing)
		{
			y += 2000000;
		}

		Hemisphere hemisphere = letters[0] > 'M' ? Hemisphere.North : Hemisphere.South;
		UTM utm = new UTM(zone, hemisphere);
		return utm.ToGeodetic(new Coordinate(x, y));
	}

	public override string ToGrid(Coordinate g)
	{
		char[] letters = new char[3];

    double zoneLat = g.Y < -80 ? -80 : g.Y > 72 ? 72 : g.Y;
		letters[0] = _zoneLetters[Convert.ToInt32(Math.Floor((zoneLat + 80) / 8))];

    double n = g.X >= 180 ? g.X - 180 : g.X + 180;
		int zone = Convert.ToInt32(Math.Floor(n / 6)) + 1;
    Hemisphere hemisphere = g.Y >= 0 ? Hemisphere.North : Hemisphere.South;

		UTM utm = new UTM(zone, hemisphere);

		Coordinate p = utm.ToProjected(g);
    double x = p.X;
    double y = p.Y;

		double divisor = Math.Pow(10, 5 - _precision);
		x = Math.Round(x / divisor) * divisor;
		y = Math.Round(y / divisor) * divisor;

		int setNumber = (zone - 1) % 6;
		letters[1] = _columnLetters[(setNumber % 3) * 8 + Convert.ToInt32(Math.Floor(x / 100000)) - 1];

		int rowOffset = setNumber % 2 == 1 ? 5 : 0;

		y %= 2000000;
		letters[2] = _rowLetters[(Convert.ToInt32(Math.Floor(y / 100000)) + rowOffset) % _rowLetters.Length];

		x = (x % 100000) / divisor;
		y = (y % 100000) / divisor;

		string f = new String('0', _precision);
		return String.Format("{0}{1} {2} {3}", zone, new String(letters), x.ToString(f), y.ToString(f));
	}
}

