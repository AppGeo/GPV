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
using System.Data.OleDb;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

public partial class MailingLabels : System.Web.UI.Page
{
  private const float PointsPerInch = 72;

  protected void Page_Load(object sender, EventArgs e)
  {
    cmdCreate.Enabled = false;

    if (!IsPostBack)
    {
      ViewState["layer"] = Request.Form["layer"];
      ViewState["ids"] = Request.Form["ids"];

      Configuration config = AppContext.GetConfiguration();
      string manufacturer = null;

      IEnumerable<Configuration.MailingLabelRow> mailingLabelRows = config.MailingLabel.OrderBy(o => o);

      foreach (Configuration.MailingLabelRow mailingLabelRow in mailingLabelRows)
      {
        if (mailingLabelRow.Manufacturer != manufacturer)
        {
          ddlManufacturer.Items.Add(mailingLabelRow.Manufacturer);
          manufacturer = mailingLabelRow.Manufacturer;
        }
      }

      if (ddlManufacturer.Items.Count > 0)
      {
        manufacturer = ddlManufacturer.Items[0].Value;

        foreach (Configuration.MailingLabelRow mailingLabelRow in mailingLabelRows.Where(o => o.Manufacturer == manufacturer))
        {
          ddlModelNo.Items.Add(new System.Web.UI.WebControls.ListItem(mailingLabelRow.ModelNo, mailingLabelRow.ID.ToString()));
        }

        Configuration.MailingLabelRow currentModel = mailingLabelRows.First(o => o.ID == Convert.ToInt32(ddlModelNo.Items[0].Value));
        labLabelSize.InnerText = currentModel.LabelSize;
        labLabelsAcross.InnerText = currentModel.LabelsAcross.ToString();

        cmdCreate.Enabled = true;
      }

      JavaScriptSerializer serializer = new JavaScriptSerializer();
      string labelData = serializer.Serialize(mailingLabelRows.Select(o => o.ToJsonData()).ToList());
      string script = "var GPV = (function (gpv) {{ gpv.labelData = {0}; return gpv; }})(GPV || {{}});";

      HtmlGenericControl scriptElem = new HtmlGenericControl("script");
      head.Controls.Add(scriptElem);
      scriptElem.Attributes["type"] = "text/javascript";
      scriptElem.InnerHtml = String.Format(script, labelData);
    }
  }

  protected void cmdCreate_Click(object sender, System.EventArgs e)
  {
    CreateLabels(ddlModelNo.SelectedValue, ddlTextFont.SelectedValue, Convert.ToSingle(ddlTextSize.SelectedValue), hdnPrintDirection.Value == "down");
  }

  private void CreateLabels(string id, string fontName, float textSize, bool columnMajor)
  {
    Configuration config = AppContext.GetConfiguration();

    string layerID = (string)ViewState["layer"];
    Configuration.LayerFunctionRow layerFunction = config.LayerFunction.First(o => o.LayerID == layerID && o.FunctionName == "mailinglabel");
    Configuration.MailingLabelRow mailingLabel = config.MailingLabel.First(o => o.ID == Convert.ToInt32(id));

    Response.Clear();
    Response.ContentType = "application/pdf";
    Response.AddHeader("Content-Disposition", "inline; filename=Labels.pdf");

    float pageWidth = 8.5f * PointsPerInch;
    float pageHeight = 11.0f * PointsPerInch;

    iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(pageWidth, pageHeight);
    pageSize.BackgroundColor = new iTextSharp.text.Color(System.Drawing.Color.White);
    Document document = new Document(pageSize);
    document.SetMargins(0, 0, 0, 0);

    PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
    document.Open();
    PdfContentByte content = writer.DirectContent;

    float textWidth = mailingLabel.dxLabel - 2 * mailingLabel.xOrg;
    float textHeight = mailingLabel.dyLabel - 2 * mailingLabel.yOrg;

    float labelStartX = mailingLabel.xLeft;
    float labelStartY = pageHeight - mailingLabel.yTop - mailingLabel.dyLabel;

    float leading = textSize * 1.2f;

    BaseFont baseFont = BaseFont.CreateFont(fontName, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
    iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, textSize, iTextSharp.text.Font.NORMAL);

    using (OleDbCommand command = layerFunction.GetDatabaseCommand())
    {
      command.Parameters[0].Value = (string)ViewState["ids"];

      if (command.Parameters.Count > 1)
      {
        command.Parameters[1].Value = AppUser.GetRole();
      }

      using (OleDbDataReader reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          List<String> text = new List<String>();

          for (int i = 0; i < reader.FieldCount; ++i)
          {
            if (!reader.IsDBNull(i))
            {
              text.Add(reader.GetValue(i).ToString());
            }
          }

          if (text.Count > 0)
          {
            float originX = labelStartX + mailingLabel.xOrg;
            float originY = labelStartY + mailingLabel.yOrg;

            ColumnText columnText = new ColumnText(content);
            columnText.SetSimpleColumn(originX, originY, originX + textWidth, originY + textHeight, leading, Element.ALIGN_LEFT);
            columnText.AddText(new Phrase(leading, String.Join("\n", text.ToArray()), font));
            columnText.Go();

            if (columnMajor)
            {
              labelStartY -= mailingLabel.dyLabel + mailingLabel.dySpace;

              if (labelStartY <= 0)
              {
                labelStartY = pageHeight - mailingLabel.yTop - mailingLabel.dyLabel;
                labelStartX += mailingLabel.dxLabel + mailingLabel.dxSpace;

                if (labelStartX + mailingLabel.dxLabel > pageWidth)
                {
                  document.NewPage();
                  labelStartX = mailingLabel.xLeft;
                }
              }
            }
            else
            {
              labelStartX += mailingLabel.dxLabel + mailingLabel.dxSpace;

              if (labelStartX + mailingLabel.dxLabel > pageWidth)
              {
                labelStartX = mailingLabel.xLeft;
                labelStartY -= mailingLabel.dyLabel + mailingLabel.dySpace;

                if (labelStartY <= 0)
                {
                  document.NewPage();
                  labelStartY = pageHeight - mailingLabel.yTop - mailingLabel.dyLabel;
                }
              }
            }
          }
        }
      }

      command.Connection.Dispose();
    }

    document.Close();
    Response.End();
  }
}