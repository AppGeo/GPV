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
using System.Collections.Generic;
using System.Linq;

public partial class Configuration
{
  public partial class TileLayerRow
  {
    public Dictionary<String, Object> ToJsonData()
    {
      Dictionary<String, Object> jsonData = new Dictionary<String, Object>();
      jsonData.Add("url", URL);
      jsonData.Add("overlay", !IsOverlayNull() && Overlay == 1);

      if (!IsAttributionNull())
      {
        jsonData.Add("attribution", Attribution);
      }

      return jsonData;
    }
  }
}