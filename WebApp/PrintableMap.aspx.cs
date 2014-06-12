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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class PrintableMap : System.Web.UI.Page
{
  Dictionary<String, List<int>> _inputFields = new Dictionary<String, List<int>>();

  protected void Page_Init(object sender, EventArgs e)
  {
    Configuration config = AppContext.GetConfiguration();
    int i = 1;

    foreach (Configuration.PrintTemplateRow template in config.PrintTemplate)
    {
      List<int> indexes = new List<int>();

      foreach (Configuration.PrintTemplateContentRow element in template.GetPrintTemplateContentRows().Where(o => o.ContentType == "input"))
      {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.ID = String.Format("pnlInput{0}", i);
        div.Style["width"] = "100%";
        div.Visible = false;
        pnlMain.Controls.Add(div);

        HtmlGenericControl label = new HtmlGenericControl("div");
        label.Attributes["class"] = "Label";
        div.Style["top"] = "3px";
        label.InnerText = element.DisplayName;
        div.Controls.Add(label);

        TextBox tbo = new TextBox();
        tbo.ID = String.Format("tboInput{0}", i);
        tbo.Width = new Unit("216px");
        tbo.Style["left"] = "139px";
        div.Controls.Add(tbo);

        indexes.Add(i);
        ++i;
      }

      if (indexes.Count > 0)
      {
        _inputFields.Add(template.TemplateID, indexes);
      }
    }
  }
  
  protected void Page_Load(object sender, EventArgs e)
  {
    AppState appState = null;

    if (!String.IsNullOrEmpty(Request.Form["state"]) && !String.IsNullOrEmpty(Request.Form["width"]))
    {
      appState = AppState.FromJson(Request.Form["state"]);
      appState.SaveTo(ViewState);
      ViewState["width"] = Convert.ToDouble(Request.Form["width"]);

      Configuration config = AppContext.GetConfiguration();

      foreach (Configuration.PrintTemplateRow template in config.PrintTemplate)
      {
        bool add = template.IsAlwaysAvailableNull() || template.AlwaysAvailable == 1;

        if (!add)
        {
          add = template.GetApplicationPrintTemplateRows().Any(o => o.ApplicationID == appState.Application);
        }

        if (add)
        {
          ddlPrintTemplate.Items.Add(new ListItem(template.TemplateTitle, template.TemplateID));
        }
      }
    }
  }

  protected void cmdCreate_Click(object sender, EventArgs e)
  {
    AppState appState = AppState.RestoreFrom(ViewState);
    double width = (double)ViewState["width"];
    PreserveMode preserveMode = optPreserveScale.Checked ? PreserveMode.Scale : PreserveMode.Width;

    List<String> input = new List<String>();

    if (ddlPrintTemplate.SelectedIndex > -1 && _inputFields.ContainsKey(ddlPrintTemplate.SelectedValue))
    {
      foreach (int i in _inputFields[ddlPrintTemplate.SelectedValue])
      {
        TextBox tbo = FindControl(String.Format("tboInput{0}", i)) as TextBox;
        input.Add(tbo.Text);
      }
    }

    PdfMap pdfMap = new PdfMap(appState, ddlPrintTemplate.SelectedValue, input, preserveMode, width);
    pdfMap.Write(Response);
    Response.End();
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    if (AppState.IsIn(ViewState))
    {
      int top = 62;

      foreach (string key in _inputFields.Keys)
      {
        foreach (int i in _inputFields[key])
        {
          HtmlGenericControl div = FindControl(String.Format("pnlInput{0}", i)) as HtmlGenericControl;
          div.Visible = key == ddlPrintTemplate.SelectedValue;

          if (div.Visible)
          {
            div.Style["top"] = String.Format("{0}px", top);
            top += 20;
          }
        }
      }

      pnlBottom.Style["top"] = String.Format("{0}px", top + 5);
      pnlMain.Style["height"] = String.Format("{0}px", top + 93);

      AppState appState = AppState.RestoreFrom(ViewState, false);
      double extentWidth = appState.Extent.Width;
      double width = (double)ViewState["width"];

      if (AppSettings.MapUnits == "meters")
      {
        extentWidth *= Constants.FeetPerMeter;
      }

      labPreserveScale.InnerText = String.Format("[1\" = {0:#,##0} ft]", extentWidth * 96 / width);
      labPreserveWidth.InnerText = String.Format("[{0:#,##0} ft", extentWidth);

      Configuration.PrintTemplateContentRow mapElement = AppContext.GetConfiguration().PrintTemplateContent.Where(o => o.TemplateID == ddlPrintTemplate.SelectedValue).OrderBy(o => o.SequenceNo).FirstOrDefault(o => String.Compare(o.ContentType, "map", true) == 0);

      if (mapElement != null)
      {
        labPreserveWidth.InnerText += String.Format(" or 1\" = {0:#,##0} ft", extentWidth / mapElement.Width);
      }

      labPreserveWidth.InnerText += "]";

      cmdCreate.Enabled = ddlPrintTemplate.SelectedIndex > -1;
    }
  }
}