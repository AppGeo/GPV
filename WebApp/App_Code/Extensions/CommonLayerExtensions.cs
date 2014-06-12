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
using AppGeo.Clients;

public static class CommonLayerExtensions
{
  public static CommonField FindField(this CommonLayer layer, string name)
  {
    return layer.Fields.FirstOrDefault(f => String.Compare(f.Name, name, true) == 0 ||
      String.Compare(f.Alias, name, true) == 0 || 
      f.Name.EndsWith("." + name, StringComparison.CurrentCultureIgnoreCase));
  }
}