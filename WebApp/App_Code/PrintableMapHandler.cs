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
    double originalWidth = Convert.ToDouble(context.Request.Form["width"]);

    // if the user entered a feet-per-inch scale, compute the pixel width of the map
    // for the scale given the extent width

    if (scaleMode == "input")
    {
      double extentWidth = appState.Extent.Width * (AppContext.AppSettings.MapUnits == "feet" ? 1 : Constants.FeetPerMeter);
      double scale = Convert.ToDouble(context.Request.Form["scale"]);

      originalWidth = extentWidth * 96 / scale;
      scaleMode = "scale";
    }

    PreserveMode preserveMode = (PreserveMode)Enum.Parse(typeof(PreserveMode), scaleMode, true);

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

    PdfMap pdfMap = new PdfMap(appState, templateID, input, preserveMode, originalWidth);
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