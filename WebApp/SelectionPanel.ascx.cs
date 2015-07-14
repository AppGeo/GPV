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
using System.Drawing;
using GPV;

public partial class SelectionPanel : System.Web.UI.UserControl
{
  public void Initialize(Dictionary<String, String> launchParams)
  {
    ddlTargetLayer.Style["border-left-color"] = ColorTranslator.ToHtml(AppSettings.TargetColorUI);
    ddlSelectionLayer.Style["border-left-color"] = ColorTranslator.ToHtml(AppSettings.SelectionColorUI);

    // activate the selection tool if necessary

    if (!launchParams.ContainsKey("tool") || String.Compare(launchParams["tool"], "select", true) == 0)
    {
      //((Div)FindControl("optSelect")).CssClass = "Button MapTool Selected";
    }
  }
}