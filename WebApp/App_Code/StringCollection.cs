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
using System.Collections.Generic;

public class StringCollection : System.Collections.Specialized.StringCollection
{
	private const char DefaultSeparator = '\u0002';

	public static StringCollection FromString(string value)
	{
		return FromString(value, DefaultSeparator);
	}

	public static StringCollection FromString(string value, char separator)
	{
		StringCollection col = new StringCollection();

		if (value.Length > 0)
		{
			col.AddRange(value.Split(separator));
		}

		return col;
	}

  public StringCollection() { }

  public StringCollection(IEnumerable<String> list)
  {
    AddRange((new List<String>(list)).ToArray());
  }

	public StringCollection Clone()
	{
		StringCollection c = new StringCollection();

		if (Count > 0)
		{
			string[] strings = new string[Count];
			CopyTo(strings, 0);
			c.AddRange(strings);
		}

		return c;
	}

	public string Join(string separator)
	{
		if (Count == 0)
		{
			return "";
		}
		else
		{
			string[] array = new string[Count];
			CopyTo(array, 0);
			return String.Join(separator, array);
		}
	}

	public override string ToString()
	{
		return ToString(DefaultSeparator);
	}

	public string ToString(char separator)
	{
		if (Count == 0)
		{
			return "";
		}
		else
		{
			string[] array = new string[Count];
			CopyTo(array, 0);
			return String.Join(new string(separator, 1), array);
		}
	}

  public bool Truncate(int newCount)
  {
    bool truncated = newCount < Count;

    while (newCount < Count)
    {
      RemoveAt(Count - 1);
    }

    return truncated;
  }
}