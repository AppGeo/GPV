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

using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class PrintableMapHandler : IHttpHandler
{
  public void ProcessRequest(HttpContext context)
  {
    HttpResponse response = context.Response;
    response.Clear();

    AppState appState = AppState.FromJson(context.Request.Form["state"]);
    string templateID = context.Request.Form["template"];
    string scaleMode = context.Request.Form["scalemode"];
    double originalWidthPx = Convert.ToDouble(context.Request.Form["width"]);
    
    double? preserveScale = null;

    if (scaleMode == "scale" || scaleMode == "input")
    {
      Coordinate c = appState.Extent.Centre;
      double scaleFactor = AppContext.AppSettings.MapCoordinateSystem.GetScaleFactorAtY(c);
      double conversion = AppContext.AppSettings.MapUnits == "feet" ? 1 : Constants.FeetPerMeter;
      preserveScale = appState.Extent.Width * conversion * 96 / (originalWidthPx * scaleFactor);

      if (scaleMode == "input")
      {
        double s;

        if (Double.TryParse(context.Request.Form["scale"], out s))
        {
          preserveScale = s;
        }
      }
    }

    // read in the user inputs

    List<String> input = new List<String>();
    Configuration.ApplicationRow application = AppContext.GetConfiguration().Application.First(o => o.ApplicationID == appState.Application);
    Configuration.PrintTemplateRow template = application.GetPrintTemplates().First(o => o.TemplateID == templateID);

    foreach (Configuration.PrintTemplateContentRow element in template.GetPrintTemplateContentRows().Where(o => o.ContentType == "input"))
    {
      string fieldName = String.Format("input_{0}_{1}", template.TemplateID, element.SequenceNo);
      input.Add(context.Request.Form[fieldName]);
    }

    // produce the PDF output

    PdfMap pdfMap = new PdfMap(appState, templateID, input, preserveScale);
    pdfMap.Write(response);
    response.End();
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}