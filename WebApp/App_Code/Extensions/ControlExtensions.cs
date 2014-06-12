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
using System.Web.UI;

public static class ControlExtensions
{
  public static Control FindControl(this Control parent, string id, bool caseSensitive)
  {
    if (caseSensitive)
    {
      return parent.FindControl(id);
    }

    foreach (Control control in parent.Controls)
    {
      if (!String.IsNullOrEmpty(control.ID) && control.ID.Equals(id, StringComparison.OrdinalIgnoreCase))
      {
        return control;
      }
      else
      {
        Control descendant = control.FindControl(id, false);

        if (descendant != null)
        {
          return descendant;
        }
      }
    }

    return null;
  }
}