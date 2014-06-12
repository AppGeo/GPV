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

public partial class Configuration
{
  public partial class ProximityRow
  {
    public ProximityIdentifier ToIdentifier()
    {
      return new ProximityIdentifier(ProximityID, DisplayName);
    }

    public Dictionary<String, Object> ToJsonData()
    {
      Dictionary<String, Object> jsonData = new Dictionary<String, Object>();
      jsonData.Add("name", DisplayName);
      jsonData.Add("sequenceNo", SequenceNo);
      return jsonData;
    }
  }
}

public class ProximityIdentifier : IComparable<ProximityIdentifier>
{
  public string Key = null;
  public string Name = null;

  public ProximityIdentifier(string key, string name)
  {
    Key = key;
    Name = name;
  }

  public int CompareTo(ProximityIdentifier other)
  {
    return Key.CompareTo(other.Key);
  }
}