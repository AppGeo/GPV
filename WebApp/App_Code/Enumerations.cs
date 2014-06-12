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
using System.Linq;

public enum Action
{
	Select = 0,
	FindAllWithin = 1,
	FindNearest1 = 2,
	FindNearest2 = 3,
	FindNearest3 = 4,
	FindNearest4 = 5,
	FindNearest5 = 6
}

public enum CoordinateMode
{
	DMS = 0,
	DD = 1,
	NE = 2,
	USNG = 3
}

public enum FeatureType
{
	Active = 0,
	Target = 1,
	Selection = 2,
	Filtered = 3
}

public enum FunctionTab
{
  None = 0,
	Legend = 1,
	Selection = 2,
	Markup = 4,
	Location = 8,
	All = 15
}

public enum CheckMode
{
  None,
  Empty,
  Checked,
  Unchecked
}

public enum ExpandMode
{
  Empty,
  Expanded,
  Collapsed
}

public static class EnumHelper
{
  public static string[] ToChoiceArray(Type e)
  {
    return Enum.GetNames(e).Select(o => o.ToLower()).ToArray();
  }

  public static string ToChoiceString(Type e)
  {
    List<String> names = Enum.GetNames(e).Select(o => String.Format("'{0}'", o.ToLower())).ToList();
    names[names.Count - 2] = String.Format("{0} or {1}", names[names.Count - 2], names[names.Count - 1]);
    names.RemoveAt(names.Count - 1);
    return String.Join(", ", names.ToArray());
  }
}
