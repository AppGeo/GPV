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
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

public partial class MobileViewer : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    AddProjectionScripts();
    TrackingManager.TrackUse(Request, true);
  }

  private void AddProjectionScripts()
  {
    string projection = ConfigurationManager.AppSettings["Projection"];
    string projectionFile = String.Format("Scripts/Mobile/{0}.js", projection);

    HtmlGenericControl projectionScript = new HtmlGenericControl("script");
    projectionScript.Attributes.Add("src", projectionFile);
    body.Controls.Add(projectionScript);

    JavaScriptSerializer serializer = new JavaScriptSerializer();

    string projectionParams = serializer.Serialize(new {
      centralMeridian = ToNullableDouble(ConfigurationManager.AppSettings["CentralMeridian"]),
      originLatitude = ToNullableDouble(ConfigurationManager.AppSettings["OriginLatitude"]),
      standardParallel1 = ToNullableDouble(ConfigurationManager.AppSettings["StandardParallel1"]),
      standardParallel2 = ToNullableDouble(ConfigurationManager.AppSettings["StandardParallel2"]),
      scaleFactor = ToNullableDouble(ConfigurationManager.AppSettings["ScaleFactor"]),
      falseEasting = ToNullableDouble(ConfigurationManager.AppSettings["FalseEasting"]),
      falseNorthing = ToNullableDouble(ConfigurationManager.AppSettings["FalseNorthing"]),
      spheroid = ConfigurationManager.AppSettings["Spheroid"],
      units = ConfigurationManager.AppSettings["MapUnits"]
    });

    projectionScript = new HtmlGenericControl("script");
    projectionScript.InnerHtml = String.Format("GPV.coordSys = GPV.coordSys({0});", projectionParams);
    body.Controls.Add(projectionScript);
  }

  private double? ToNullableDouble(string s)
  {
    double d;

    if (Double.TryParse(s, out d))
    {
      return (double?)d;
    }

    return null;
  }
}