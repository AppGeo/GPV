//  Copyright 2015 Applied Geographics, Inc.
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
using System.Web.UI.HtmlControls;

public partial class SharePanel : System.Web.UI.UserControl
{
  public void Initialize(Configuration config, Configuration.ApplicationRow application)
  {
    foreach (Configuration.PrintTemplateRow template in application.GetPrintTemplates())
    {
      HtmlGenericControl option = new HtmlGenericControl("option");
      option.InnerText = template.TemplateTitle;
      option.Attributes["value"] = template.TemplateID;
      ddlPrintTemplate.Controls.Add(option);

      foreach (Configuration.PrintTemplateContentRow element in template.GetPrintTemplateContentRows().Where(o => o.ContentType == "input"))
      {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.Attributes["data-templateID"] = template.TemplateID;
        div.Attributes["class"] = "printInput";
        div.Style["width"] = "100%";
        div.Style["display"] = "none";

        HtmlGenericControl label = new HtmlGenericControl("label");
        div.Controls.Add(label);
        label.InnerText = element.DisplayName;

        HtmlGenericControl tbo = new HtmlGenericControl("input");
        div.Controls.Add(tbo);
        tbo.Attributes["type"] = "text";
        tbo.Attributes["name"] = String.Format("input_{0}_{1}", template.TemplateID, element.SequenceNo);
        tbo.Attributes["class"] = "Input Text";

        pnlPrintInputs.Controls.Add(div);
      }
    }

    foreach (Configuration.ExternalMapRow externalMap in config.ExternalMap)
    {
      HtmlGenericControl opt = new HtmlGenericControl("option");
      ddlExternalMap.Controls.Add(opt);
      opt.Attributes["value"] = externalMap.DisplayName;
      opt.InnerText = externalMap.DisplayName;
    }
  }
}