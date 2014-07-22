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
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using AppGeo.Clients;
using AppGeo.Clients.Transform;

public class PdfMap
{
	private const float PointsPerInch = 72;
	private const float PixelsPerInch = 96;

	private AppState _appState;
	private string _templateId;
	private List<String> _input;
	private PreserveMode _preserveMode;
	private double _originalWidth;
  private double _pixelSize = -1;

  public PdfMap(AppState appState, string templateId, List<String> input, PreserveMode preserveMode, double originalWidth)
	{
		_appState = appState;
		_templateId = templateId;
    _input = input;
		_preserveMode = preserveMode;
		_originalWidth = originalWidth;
	}

	private void CreatePdfBox(PdfContentByte content, Configuration.PrintTemplateContentRow row)
	{
		CreatePdfBox(content, row, true);
	}

	private void CreatePdfBox(PdfContentByte content, Configuration.PrintTemplateContentRow row, bool allowFill)
	{
		if (row.IsFillColorNull() && row.IsOutlineWidthNull() && row.IsOutlineColorNull())
		{
			return;
		}

		bool hasFill = false;
		bool hasStroke = false;

		content.SetLineWidth((1 / PixelsPerInch) * PointsPerInch);

		if (!row.IsFillColorNull() && allowFill)
		{
			System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(row.FillColor);
			content.SetRGBColorFill(c.R, c.G, c.B);
			hasFill = true;
		}

		if (!row.IsOutlineWidthNull())
		{
			content.SetLineWidth((Convert.ToSingle(row.OutlineWidth) / PixelsPerInch) * PointsPerInch);
			hasStroke = true;
		}

		if (!row.IsOutlineColorNull())
		{
			System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(row.OutlineColor);
			content.SetRGBColorStroke(c.R, c.G, c.B);
			hasStroke = true;
		}

		float originX = Convert.ToSingle(row.OriginX) * PointsPerInch;
		float originY = Convert.ToSingle(row.OriginY) * PointsPerInch;
		float width = Convert.ToSingle(row.Width) * PointsPerInch;
		float height = Convert.ToSingle(row.Height) * PointsPerInch;

		if (hasFill)
		{
			content.Rectangle(originX, originY, width, height);
			content.Fill();
		}

		if (hasStroke)
		{
			content.Rectangle(originX, originY, width, height);
			content.Stroke();
		}
	}

	private void CreatePdfImage(PdfContentByte content, Configuration.PrintTemplateContentRow row)
	{
		float originX = Convert.ToSingle(row.OriginX) * PointsPerInch;
		float originY = Convert.ToSingle(row.OriginY) * PointsPerInch;
		float width = Convert.ToSingle(row.Width) * PointsPerInch;
		float height = Convert.ToSingle(row.Height) * PointsPerInch;

		iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("Images/Print") + "\\" + row.FileName);
		image.SetAbsolutePosition(originX, originY);
		image.ScaleAbsolute(width, height);

		content.AddImage(image);

		CreatePdfBox(content, row, false);
	}

  private bool CreateLayerInLegend(PdfContentByte content, Configuration.MapTabRow mapTab, List<CommonLayer> layerList, LegendProperties properties, CommonLayer layer, float indent)
  {
    if (!layerList.Contains(layer))
    {
      return false;
    }

    float layerHeight = GetLayerHeightInLegend(layerList, properties, layer);

    if (properties.CurrentY < properties.Height && properties.CurrentY - layerHeight < 0)
    {
      if (properties.CurrentColumn == properties.NumColumns)
      {
        return true;
      }

      properties.CurrentX += properties.ColumnWidth + properties.ColumnSpacing;
      properties.CurrentY = properties.Height;
      properties.CurrentColumn += 1;
    }

    int numClasses = GetNumClasses(layer);

    Configuration.LayerRow configLayer = mapTab.GetMapTabLayerRows().Where(o => String.Compare(o.LayerRow.LayerName, layer.Name, true) == 0).Select(o => o.LayerRow).FirstOrDefault();
    string layerName = configLayer != null && !configLayer.IsDisplayNameNull() ? configLayer.DisplayName : layer.Name;

    // write the layer name

    if (layer.Type == CommonLayerType.Group || numClasses > 1)
    {
      properties.CurrentY -= properties.FontSize;
      string name = layerName;

      try
      {
        while (content.GetEffectiveStringWidth(name, false) > properties.ColumnWidth - indent)
        {
          name = name.Substring(0, name.Length - 1);
        }
      }
      catch { }

      content.BeginText();
      content.SetFontAndSize(properties.BaseFont, properties.FontSize);
      content.SetRGBColorFill(0, 0, 0);
      content.ShowTextAligned(PdfContentByte.ALIGN_LEFT, name, properties.OriginX + properties.CurrentX + indent, properties.OriginY + properties.CurrentY + (properties.SwatchHeight - properties.FontSize) / 2, 0);
      content.EndText();
    }

    if (layer.Type == CommonLayerType.Group)
    {
      properties.CurrentY -= properties.LayerSpacing;

      foreach (CommonLayer childLayer in layer.Children)
      {
        CreateLayerInLegend(content, mapTab, layerList, properties, childLayer, indent + 1.5f * properties.FontSize);
      }
    }
    else
    {
      properties.CurrentY -= properties.ClassSpacing;

      foreach (CommonLegendGroup legendGroup in layer.Legend.Groups)
      {
        foreach (CommonLegendClass legendClass in legendGroup.Classes)
        {
          if (!legendClass.ImageIsTransparent)
          {
            properties.CurrentY -= properties.SwatchHeight;

            MemoryStream stream = new MemoryStream(legendClass.Image);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
            float w = properties.SwatchHeight * bitmap.Width / bitmap.Height;

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(legendClass.Image);
            image.SetAbsolutePosition(properties.OriginX + properties.CurrentX + indent, properties.OriginY + properties.CurrentY - properties.SwatchHeight * 0.1f);
            image.ScaleAbsolute(w, properties.SwatchHeight);
            content.AddImage(image);

            string label = numClasses > 1 ? legendClass.Label : layerName;

            try
            {
              while (content.GetEffectiveStringWidth(label, false) > properties.ColumnWidth - properties.SwatchWidth - properties.ClassSpacing)
              {
                label = label.Substring(0, label.Length - 1);
              }
            }
            catch { }

            content.BeginText();
            content.SetFontAndSize(properties.BaseFont, properties.FontSize);
            content.SetRGBColorFill(0, 0, 0);
            content.ShowTextAligned(PdfContentByte.ALIGN_LEFT, label, properties.OriginX + properties.CurrentX + indent + properties.SwatchWidth + properties.ClassSpacing, properties.OriginY + properties.CurrentY + (properties.SwatchHeight - properties.FontSize) / 2, 0);
            content.EndText();

            properties.CurrentY -= properties.ClassSpacing;
          }
        }
      }

      properties.CurrentY -= properties.LayerSpacing - properties.ClassSpacing;
    }

    return false;
  }

	private void CreatePdfLegend(PdfContentByte content, Configuration.PrintTemplateContentRow row)
	{
		CreatePdfBox(content, row);

    LegendProperties properties = new LegendProperties(row);

    if (properties.NumColumns == 0)
    {
      return;
    }

    List<CommonLayer> layerList = GetLegendLayers();

    if (layerList.Count == 0)
    {
      return;
    }

    Configuration config = AppContext.GetConfiguration();
    Configuration.MapTabRow mapTab = config.MapTab.FindByMapTabID(_appState.MapTab);
    CommonDataFrame dataFrame = AppContext.GetDataFrame(mapTab);
    List<CommonLayer> topLayers = dataFrame.TopLevelLayers;

		properties.CurrentX = 0;
    properties.CurrentY = properties.Height;
    properties.CurrentColumn = 1;

    bool full = false;

    for (int i = 0; i < topLayers.Count && !full; ++i)
    {
      full = CreateLayerInLegend(content, mapTab, layerList, properties, topLayers[i], 0);
    }
	}
	
	private void CreatePdfMap(PdfContentByte content, Configuration.PrintTemplateContentRow row)
	{
		int pixelWidth = Convert.ToInt32(row.Width * PixelsPerInch);
		int pixelHeight = Convert.ToInt32(row.Height * PixelsPerInch);

		MapMaker mapMaker = new MapMaker(_appState, pixelWidth, pixelHeight, 2);
		byte[] mapImage = mapMaker.GetImage().Image;

		float originX = Convert.ToSingle(row.OriginX) * PointsPerInch;
		float originY = Convert.ToSingle(row.OriginY) * PointsPerInch;
		float width = Convert.ToSingle(row.Width) * PointsPerInch;
		float height = Convert.ToSingle(row.Height) * PointsPerInch;

		iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(mapImage);
		image.SetAbsolutePosition(originX, originY);
		image.ScaleAbsolute(width, height);

		content.AddImage(image);

		CreatePdfBox(content, row, false);
	}

  private void CreatePdfOverviewMap(PdfContentByte content, Configuration.PrintTemplateContentRow row)
  {
    AppState appState = new AppState();
    appState.Application = _appState.Application;
    Configuration.ApplicationRow application = AppContext.GetConfiguration().Application.First(o => o.ApplicationID == appState.Application);

    appState.MapTab = application.OverviewMapID;
    appState.Extent = application.GetFullExtentEnvelope();

    int pixelWidth = Convert.ToInt32(row.Width * PixelsPerInch);
    int pixelHeight = Convert.ToInt32(row.Height * PixelsPerInch);

    MapMaker mapMaker = new MapMaker(appState, pixelWidth, pixelHeight, 2);
    MapImageData mapImageData = mapMaker.GetImage();

    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(new MemoryStream(mapImageData.Image));
    Transformation trans = new AffineTransformation(pixelWidth * 2, pixelHeight * 2, appState.Extent);
    MapGraphics mapGraphics = MapGraphics.FromImage(bitmap, trans);

    double minSize = (trans.Transform(new Coordinate(1, 0)).X - trans.Transform(new Coordinate(0, 0)).X) * 12;
    Envelope extent = new Envelope(new Coordinate(_appState.Extent.MinX, _appState.Extent.MinY), new Coordinate(_appState.Extent.MaxX, _appState.Extent.MaxY));

    if (extent.Width < minSize)
    {
      extent = new Envelope(new Coordinate(extent.Centre.X - minSize * 0.5, extent.MinY), new Coordinate(extent.Centre.X + minSize * 0.5, extent.MaxY));
    }

    if (extent.Height < minSize)
    {
      extent = new Envelope(new Coordinate(extent.MinX, extent.Centre.Y - minSize * 0.5), new Coordinate(extent.MaxX, extent.Centre.Y + minSize * 0.5));
    }

    System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(64, System.Drawing.Color.Red));
    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 4);

    mapGraphics.FillEnvelope(brush, extent);
    mapGraphics.DrawEnvelope(pen, extent);

    MemoryStream stream = new MemoryStream();
    bitmap.Save(stream, mapImageData.Type == CommonImageType.Png ? System.Drawing.Imaging.ImageFormat.Png : System.Drawing.Imaging.ImageFormat.Jpeg);
    byte[] mapImage = stream.ToArray();

    float originX = Convert.ToSingle(row.OriginX) * PointsPerInch;
    float originY = Convert.ToSingle(row.OriginY) * PointsPerInch;
    float width = Convert.ToSingle(row.Width) * PointsPerInch;
    float height = Convert.ToSingle(row.Height) * PointsPerInch;

    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(mapImage);
    image.SetAbsolutePosition(originX, originY);
    image.ScaleAbsolute(width, height);

    content.AddImage(image);

    CreatePdfBox(content, row, false);
  }

  private void CreatePdfTabData(PdfContentByte content, Configuration.PrintTemplateContentRow row)
  {
    CreatePdfBox(content, row);

    float originX = Convert.ToSingle(row.OriginX) * PointsPerInch;
    float originY = Convert.ToSingle(row.OriginY) * PointsPerInch;
    float width = Convert.ToSingle(row.Width) * PointsPerInch;
    float height = Convert.ToSingle(row.Height) * PointsPerInch;

    string fontFamily = row.IsFontFamilyNull() ? "Helvetica" : row.FontFamily;
    float fontSize = row.IsFontSizeNull() ? 12 : Convert.ToSingle(row.FontSize);

    BaseFont normalFont = BaseFont.CreateFont(fontFamily, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
    BaseFont boldFont = BaseFont.CreateFont(fontFamily + "-Bold", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
  }

	private void CreatePdfText(PdfContentByte content, Configuration.PrintTemplateContentRow row, string text)
	{
		CreatePdfBox(content, row);

		float originX = Convert.ToSingle(row.OriginX) * PointsPerInch;
		float originY = Convert.ToSingle(row.OriginY) * PointsPerInch;
		float width = Convert.ToSingle(row.Width) * PointsPerInch;
		float height = Convert.ToSingle(row.Height) * PointsPerInch;

		string fontFamily = "Helvetica";
		int textAlign = PdfContentByte.ALIGN_CENTER;
		float fontSize = 12;
		int textStyle = iTextSharp.text.Font.NORMAL;
		bool textWrap = false;

		int columnAlign = Element.ALIGN_CENTER;

		if (!row.IsTextWrapNull())
		{
			textWrap = row.TextWrap == 1;
		}

		if (!row.IsFontFamilyNull())
		{
			fontFamily = row.FontFamily;
		}

		if (!row.IsFontBoldNull())
		{
			if (row.FontBold == 1)
			{
				if (textWrap)
				{
					textStyle = Font.BOLD;
				}
				else
				{
					fontFamily += "-Bold";
				}
			}
		}

		if (!row.IsFontSizeNull())
		{
			fontSize = Convert.ToSingle(row.FontSize);
		}

		BaseFont baseFont = BaseFont.CreateFont(fontFamily, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

		if (textWrap)
		{
			if (!row.IsTextAlignNull())
			{
				switch (row.TextAlign)
				{
					case "left":
						columnAlign = Element.ALIGN_LEFT;
						break;
					case "right":
						columnAlign = Element.ALIGN_RIGHT;
						break;
				}
			}

			Font font = new Font(baseFont, fontSize, textStyle);
			content.SetRGBColorFill(0, 0, 0);

			float leading = fontSize * 1.2f;

			ColumnText column = new ColumnText(content);
			column.SetSimpleColumn(originX, originY, originX + width, originY + height, leading, columnAlign);
			column.AddText(new Phrase(leading, text, font));
			column.Go();
		}
		else
		{
			originX += width / 2;

			if (!row.IsTextAlignNull())
			{
				switch (row.TextAlign)
				{
					case "left":
						textAlign = PdfContentByte.ALIGN_LEFT;
						originX -= width / 2;
						break;
					case "right":
						textAlign = PdfContentByte.ALIGN_RIGHT;
						originX += width / 2;
						break;
				}
			}

			content.BeginText();
			content.SetFontAndSize(baseFont, fontSize);
			content.SetRGBColorFill(0, 0, 0);
			content.ShowTextAligned(textAlign, text, originX, originY, 0);
			content.EndText();
		}
	}

  private float GetLayerHeightInLegend(List<CommonLayer> layerList, LegendProperties properties, CommonLayer layer)
  {
    float height = 0;

    if (layer.Type == CommonLayerType.Group)
    {
      height = properties.FontSize + properties.LayerSpacing;

      foreach (CommonLayer childLayer in layer.Children)
      {
        if (layerList.Contains(childLayer))
        {
          height += GetLayerHeightInLegend(layerList, properties, childLayer);
          break;
        }
      }
    }
    else
    {
      int numClasses = GetNumClasses(layer);

      if (numClasses > 1)
      {
        height = properties.FontSize + properties.ClassSpacing;
      }

      height += numClasses * (properties.SwatchHeight + properties.ClassSpacing) - properties.ClassSpacing;
    }

    return height;
  }

  private List<CommonLayer> GetLegendLayers()
  {
    StringCollection visibleLayers = null;

    if (_pixelSize > 0 && _appState.VisibleLayers.ContainsKey(_appState.MapTab))
    {
      visibleLayers = _appState.VisibleLayers[_appState.MapTab];
    }

    Configuration config = AppContext.GetConfiguration();
    Configuration.MapTabRow mapTab = config.MapTab.FindByMapTabID(_appState.MapTab);
    CommonDataFrame dataFrame = AppContext.GetDataFrame(mapTab);

    List<CommonLayer> layerList = new List<CommonLayer>();
    List<String> mapTabLayerIds = new List<String>();

    foreach (Configuration.MapTabLayerRow mapTabLayer in mapTab.GetMapTabLayerRows())
    {
      Configuration.LayerRow layer = mapTabLayer.LayerRow;
      mapTabLayerIds.Add(layer.LayerID);

      CommonLayer commonLayer = dataFrame.Layers.FirstOrDefault(lyr => String.Compare(lyr.Name, layer.LayerName, true) == 0);

      if (commonLayer.Type == CommonLayerType.Feature && !layerList.Contains(commonLayer))
      {
        bool hasClasses = GetNumClasses(commonLayer) > 0;
        bool visibleAtScale = _pixelSize <= 0 || commonLayer.IsWithinScaleThresholds(_pixelSize);
        bool shownInLegend = !mapTabLayer.IsShowInLegendNull() && mapTabLayer.ShowInLegend == 1;
        bool checkedInLegend = mapTabLayer.IsCheckInLegendNull() || mapTabLayer.CheckInLegend < 0 || visibleLayers == null || visibleLayers.Contains(layer.LayerID);
        bool shownInPrintLegend = !(!mapTabLayer.IsShowInPrintLegendNull() && mapTabLayer.ShowInPrintLegend == 0);

        if (hasClasses && visibleAtScale && shownInLegend && checkedInLegend && shownInPrintLegend)
        {
          layerList.Add(commonLayer);

          while (commonLayer.Parent != null)
          {
            commonLayer = commonLayer.Parent;

            if (!layerList.Contains(commonLayer))
            {
              layerList.Add(commonLayer);
            }
          }
        }
      }
    }

    if (!mapTab.IsBaseMapIDNull() && !mapTab.IsShowBaseMapInLegendNull() && mapTab.ShowBaseMapInLegend == 1)
    {
      foreach (Configuration.LayerRow layer in config.Layer.Where(o => !o.IsBaseMapIDNull() && o.BaseMapID == mapTab.BaseMapID))
      {
        if (!mapTabLayerIds.Contains(layer.LayerID))
        {
          CommonLayer commonLayer = dataFrame.Layers.FirstOrDefault(o => String.Compare(o.Name, layer.LayerName, true) == 0);

          if (commonLayer.Type == CommonLayerType.Feature && !layerList.Contains(commonLayer))
          {
            bool hasClasses = GetNumClasses(commonLayer) > 0;
            bool visibleAtScale = _pixelSize <= 0 || commonLayer.IsWithinScaleThresholds(_pixelSize);

            if (hasClasses && visibleAtScale)
            {
              layerList.Add(commonLayer);

              while (commonLayer.Parent != null)
              {
                commonLayer = commonLayer.Parent;

                if (!layerList.Contains(commonLayer))
                {
                  layerList.Add(commonLayer);
                }
              }
            }
          }
        }
      }
    }

    return layerList;
  }

  private int GetNumClasses(CommonLayer layer)
  {
    int numClasses = 0;
    CommonLegend legend = layer.Legend;

    if (legend != null)
    {
      for (int g = 0; g < legend.Groups.Count; ++g)
      {
        for (int c = 0; c < legend.Groups[g].Classes.Count; ++c)
        {
          if (!legend.Groups[g].Classes[c].ImageIsTransparent)
          {
            ++numClasses;
          }
        }
      }
    }

    return numClasses;
  }

	public void Write(HttpResponse response)
	{
		Write(response, true);
	}

	public void Write(HttpResponse response, bool inline)
	{
    response.Clear();
		response.ContentType = "application/pdf";
		response.AddHeader("Content-Disposition", inline ? "inline" : "attachment" + "; filename=Map.pdf");

		// create the PDF document

		Configuration config = AppContext.GetConfiguration();
		Configuration.PrintTemplateRow printTemplate = config.PrintTemplate.First(o => o.TemplateID == _templateId);

		float pageWidth = Convert.ToSingle(printTemplate.PageWidth * PointsPerInch);
		float pageHeight = Convert.ToSingle(printTemplate.PageHeight * PointsPerInch);

		Rectangle pageSize = new Rectangle(pageWidth, pageHeight);
		pageSize.BackgroundColor = new Color(System.Drawing.Color.White);
		Document document = new Document(pageSize);

		PdfWriter writer = PdfWriter.GetInstance(document, response.OutputStream);
		document.Open();
		PdfContentByte content = writer.DirectContent;

		// get the extent of the main map and fit it to the proportions of
		// the map box on the page

		double mapScale = 0;

    Configuration.PrintTemplateContentRow mapElement = printTemplate.GetPrintTemplateContentRows().FirstOrDefault(o => o.ContentType == "map");

    if (mapElement != null)
		{
			if (_preserveMode == PreserveMode.Extent)
			{
        _appState.Extent.Reaspect(mapElement.Width, mapElement.Height);
			}
			else
			{
        IPoint c = new Point(_appState.Extent.Centre);

				double dx;
				double dy;

				if (_preserveMode == PreserveMode.Scale)
				{
					double ratio = _appState.Extent.Width * 96 / _originalWidth;
          dx = mapElement.Width * ratio * 0.5;
          dy = mapElement.Height * ratio * 0.5;
				}
				else
				{
					dx = _appState.Extent.Width * 0.5;
          dy = dx * mapElement.Height / mapElement.Width;
				}

        _appState.Extent = new Envelope(new Coordinate(c.Coordinate.X - dx, c.Coordinate.Y - dy), new Coordinate(c.Coordinate.X + dx, c.Coordinate.Y + dy));
			}

      double conversion = AppSettings.MapUnits == "feet" ? 1 : Constants.FeetPerMeter;
      mapScale = _appState.Extent.Width * conversion / mapElement.Width;

      _pixelSize = _appState.Extent.Width / (mapElement.Width * PixelsPerInch);
		}

    int inputIndex = 0;

		// get the page template elements and draw each one to the page

		foreach (Configuration.PrintTemplateContentRow element in printTemplate.GetPrintTemplateContentRows())
		{
      switch (element.ContentType)
			{
				case "box":
          CreatePdfBox(content, element);
					break;

				case "date":
          CreatePdfText(content, element, DateTime.Now.ToString("MMMM d, yyyy"));
					break;

				case "image":
          CreatePdfImage(content, element);
					break;

				case "legend":
          CreatePdfLegend(content, element);
					break;

				case "map":
          CreatePdfMap(content, element);
					break;

        case "overviewmap":
          CreatePdfOverviewMap(content, element);
          break;

				case "scale":
					if (mapScale > 0)
					{
            CreatePdfText(content, element, "1\" = " + mapScale.ToString("0") + " ft");
					}
					break;

        case "scalefeet":
          if (mapScale > 0)
          {
            CreatePdfText(content, element, mapScale.ToString("0") + " ft");
          }
          break;

        case "tabdata":
          CreatePdfTabData(content, element);
          break;

				case "text":
          if (!element.IsTextNull())
					{
            CreatePdfText(content, element, element.Text);
					}
          else if (!element.IsFileNameNull())
          {
            string fileName = HttpContext.Current.Server.MapPath("Text/Print") + "\\" + element.FileName;

            if (File.Exists(fileName))
            {
              string text = File.ReadAllText(fileName);
              CreatePdfText(content, element, text);
            }
          }
					break;

				case "input":
					if (inputIndex < _input.Count)
					{
            CreatePdfText(content, element, _input[inputIndex]);
            ++inputIndex;
					}
					break;
			}
		}

		document.Close();
		response.End();
	}

  private class LegendProperties
  {
		public float OriginX;
		public float OriginY;
		public float Width;
		public float Height;

    public float FontSize;
    public BaseFont BaseFont;

    public float SwatchHeight;
    public float SwatchWidth;
    public float LayerSpacing;
    public float ClassSpacing;
    public float ColumnSpacing;

    public float ColumnWidth;
    public int NumColumns;

    public float CurrentX;
    public float CurrentY;
    public float CurrentColumn;

    public LegendProperties(Configuration.PrintTemplateContentRow row)
    {
      OriginX = Convert.ToSingle(row.OriginX) * PointsPerInch;
      OriginY = Convert.ToSingle(row.OriginY) * PointsPerInch;
      Width = Convert.ToSingle(row.Width) * PointsPerInch;
      Height = Convert.ToSingle(row.Height) * PointsPerInch;

      string fontFamily = row.IsFontFamilyNull() ? "Helvetica" : row.FontFamily;
      FontSize = row.IsFontSizeNull() ? 12 : Convert.ToSingle(row.FontSize);

      if (!row.IsFontBoldNull() && row.FontBold == 1)
      {
        fontFamily += "-Bold";
      }

      BaseFont = BaseFont.CreateFont(fontFamily, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

      SwatchHeight = FontSize;
      SwatchWidth = FontSize * 1.4f;
      LayerSpacing = FontSize * 0.5f;
      ClassSpacing = FontSize * 0.15f;
      ColumnSpacing = FontSize * 1.4f;

      ColumnWidth = !row.IsLegendColumnWidthNull() ? Convert.ToSingle(row.LegendColumnWidth) * PointsPerInch :
          FontSize * 12 + SwatchWidth;

      NumColumns = Convert.ToInt32(Math.Floor((Width + ColumnSpacing) / (ColumnWidth + ColumnSpacing)));
    }
  }
}

public enum PreserveMode
{
	Extent = 0,
	Scale = 1,
	Width = 2
}