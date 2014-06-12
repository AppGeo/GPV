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
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.UI;
using GeoAPI.Geometries;
using NetTopologySuite.IO;
using AppGeo.Clients;
using AppGeo.Clients.Transform;
using AppGeo.Clients.ArcIms;

public class MapMaker
{
  private AppState _appState;
  private int _width;
  private int _height;
  private double _resolution;

  private Envelope _extent;
  private AffineTransformation _transform;
  private CoordinateSystem _coordinateSystem = null;

  public MapMaker(AppState appState, int width, int height)
  {
    Initialize(appState, width, height, 1);
  }

  public MapMaker(AppState appState, int width, int height, double resolution)
  {
    Initialize(appState, width, height, resolution);
  }

  private CoordinateSystem CoordinateSystem
  {
    get
    {
      if (_coordinateSystem == null)
      {
        _coordinateSystem = AppSettings.CoordinateSystem;
      }

      return _coordinateSystem;
    }
  }

  private void Initialize(AppState appState, int width, int height, double resolution)
  {
    _appState = appState;
    _width = width;
    _height = height;
    _resolution = resolution;

    if (_width * _resolution > 2048)
    {
      _resolution *= 2048 / (_resolution * _width);
    }

    if (_height * _resolution > 2048)
    {
      _resolution *= 2048 / (_resolution * _height);
    }

    _extent = _appState.Extent;
    _extent.Reaspect(_width, _height);
    _transform = new AffineTransformation(_width, _height, _extent);
  }

  private void DrawCoordinate(Graphics graphics, IPoint p, string[] coordinateModes, Pen pen, System.Drawing.Font font, 
      SolidBrush textBrush, SolidBrush shadowBrush, StringFormat format)
  {
    DrawCross(graphics, p, pen, 10);

    double x = p.Coordinate.X - AppSettings.DatumShiftX;
    double y = p.Coordinate.Y - AppSettings.DatumShiftY;

    if (AppSettings.MapUnits == "feet")
    {
      x *= Constants.MetersPerFoot;
      y *= Constants.MetersPerFoot;
    }

    double lon;
    double lat;
    CoordinateSystem.ToGeodetic(x, y, out lon, out lat);

    double xOffset = 2;

    foreach (string mode in coordinateModes)
    {
      string yText;
      string xText;

      switch (mode)
      {
        case "dms":
          yText = (lat < 0 ? "S " : "N ") + ToDms(lat);
          xText = (lon < 0 ? "W " : "E ") + ToDms(lon);
          break;

        case "dd":
          yText = (lat < 0 ? "S " : "N ") + Math.Abs(lat).ToString("0.000000") + "°";
          xText = (lon < 0 ? "W " : "E ") + Math.Abs(lon).ToString("0.000000") + "°";
          break;

        case "usng":
          yText = "";
          MGRS mgrs = new MGRS();
          mgrs.ToGrid(lon, lat, out xText);
          break;

        default:
          string unit = AppSettings.MapUnits == "feet" ? " ft" : " m";
          yText = "N " + p.Coordinate.Y.ToString("#,##0") + unit;
          xText = "E " + p.Coordinate.X.ToString("#,##0") + unit;
          break;
      }


      DrawText(graphics, p, String.Format("{0}\n{1}", yText, xText), font, textBrush, shadowBrush, xOffset, -3, format);

      SizeF size = graphics.MeasureString((xText.Length > yText.Length ? xText : yText) + "..", font);
      xOffset += size.Width / _resolution;
    }
  }

  private void DrawCoordinates(Graphics graphics)
  {
    if (_appState.Coordinates.Count == 0)
    {
      return;
    }

    Pen pen = new Pen(Color.Black, Convert.ToSingle(_resolution * 2));
    pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;

    System.Drawing.Font font = AppSettings.CoordinatesFont;
    font = new System.Drawing.Font(font.FontFamily, Convert.ToSingle(font.Size * _resolution), font.Style, font.Unit);
    SolidBrush textBrush = new SolidBrush(Color.Black);
    SolidBrush shadowBrush = new SolidBrush(Color.White);

    Configuration config = AppContext.GetConfiguration();
    Configuration.ApplicationRow application = config.Application.FindByApplicationID(_appState.Application);
    
    StringFormat format = new StringFormat();
    format.LineAlignment = StringAlignment.Far;

    string[] modes = application.IsCoordinateModesNull() ? new string[] { "ne" } : application.CoordinateModes.ToLower().Split(',');

    for (int i = 0; i < _appState.Coordinates.Count; ++i)
    {
      IPoint p = new NetTopologySuite.Geometries.Point(_appState.Coordinates[i]);

      if (_appState.CoordinateLabels[i] != "1")
      {
        DrawCross(graphics, p, pen, 10);
        DrawText(graphics, p, _appState.CoordinateLabels[i], font, textBrush, shadowBrush, 2, -3, format);
      }
      else
      {
        DrawCoordinate(graphics, p, modes, pen, font, textBrush, shadowBrush, format);
      }
    }
  }

  private void DrawCross(Graphics graphics, IPoint point, Pen pen, double size)
  {
    point = _transform.ReverseTransform(point);
    size *= 0.5;

    PointF[] points = new PointF[2];
    points[0].X = Convert.ToSingle((point.Coordinate.X - size) * _resolution);
    points[0].Y = Convert.ToSingle(point.Coordinate.Y * _resolution);
    points[1].X = Convert.ToSingle((point.Coordinate.X + size) * _resolution);
    points[1].Y = Convert.ToSingle(point.Coordinate.Y * _resolution);

    graphics.DrawLines(pen, points);

    points[0].X = Convert.ToSingle(point.Coordinate.X * _resolution);
    points[0].Y = Convert.ToSingle((point.Coordinate.Y - size) * _resolution);
    points[1].X = Convert.ToSingle(point.Coordinate.X * _resolution);
    points[1].Y = Convert.ToSingle((point.Coordinate.Y + size) * _resolution);

    graphics.DrawLines(pen, points);
  }

  private void DrawFeatures(Graphics graphics, string layerId, string id, Color color, double opacity, string polygonMode, int penWidth, int dotSize)
  {
    if (String.IsNullOrEmpty(id))
    {
      return;
    }

    StringCollection ids = new StringCollection();
    ids.Add(id);

    DrawFeatures(graphics, layerId, ids, color, opacity, polygonMode, penWidth, dotSize);
  }

  private void DrawFeatures(Graphics graphics, string layerId, StringCollection ids, Color color, double opacity, string polygonMode, int penWidth, int dotSize)
  {
    if (ids.Count == 0)
    {
      return;
    }

    bool drawPolygonOutlines = polygonMode == "outline";

    // get the layer

    Configuration config = AppContext.GetConfiguration();
    Configuration.LayerRow layerRow = config.Layer.FindByLayerID(layerId);

    CommonDataFrame dataFrame = AppContext.GetDataFrame(_appState.MapTab);
    CommonLayer layer = dataFrame.Layers.FirstOrDefault(lyr => String.Compare(lyr.Name, layerRow.LayerName, true) == 0);

    // build the query string and select the features

    CommonField field = layer.FindField(layerRow.KeyField);
    string joinedIds = field.IsNumeric ? ids.Join(",") : String.Format("'{0}'", ids.Join("','"));
    string query = String.Format("{0} in ({1})", field.Name, joinedIds);

    string levelQuery = layerRow.GetLevelQuery(layer, _appState.Level);

    if (!String.IsNullOrEmpty(levelQuery))
    {
      query += " and " + levelQuery;
    }

    CommonField keyField = layer.FindField(layerRow.KeyField);
    DataTable table = layer.GetFeatureTable(String.Format("{0},{1}", layer.GeometryField.Name, keyField.Name), query);

    if (table == null || table.Rows.Count == 0)
    {
      return;
    }

    OgcGeometryType geometryType = ((IGeometry)table.Rows[0][layer.GeometryField.Name]).OgcGeometryType;

    // prepare the temporary image for drawing transparent highlight graphics

    int width = Convert.ToInt32(graphics.VisibleClipBounds.Width);
    int height = Convert.ToInt32(graphics.VisibleClipBounds.Height);

    Bitmap bitMap = new Bitmap(width, height);
    Graphics imageGraphics = Graphics.FromImage(bitMap);
    imageGraphics.Clear(Color.Transparent);

    // prepare the drawing objects

    Brush brush = new SolidBrush(color);
    Pen pen = new Pen(color, Convert.ToSingle(penWidth * _resolution));
    pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
    Pen bufferPen = new Pen(color, Convert.ToSingle(5 * _resolution));
    bufferPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
    bufferPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

    float dot = Convert.ToSingle(dotSize * _resolution);

    // draw each shape in the table

    foreach (DataRow row in table.Rows)
    {
      switch (geometryType)
      {
        case OgcGeometryType.Point:
          IPoint point = (IPoint)row[layer.GeometryField.Name];
          DrawPoint(imageGraphics, point, brush, dot);
          break;

        case OgcGeometryType.MultiPoint:
          IMultiPoint multiPoint = (IMultiPoint)row[layer.GeometryField.Name];
          DrawPoint(imageGraphics, (IPoint)multiPoint[0], brush, dot);
          break;

        case OgcGeometryType.MultiLineString:
          DrawMultiLineString(imageGraphics, (IMultiLineString)row[layer.GeometryField.Name], pen);
          break;

        case OgcGeometryType.MultiPolygon:
          if (drawPolygonOutlines)
          {
            DrawMultiPolygon(imageGraphics, (IMultiPolygon)row[layer.GeometryField.Name], null, null, pen);
          }
          else
          {
            DrawMultiPolygon(imageGraphics, (IMultiPolygon)row[layer.GeometryField.Name], brush, bufferPen);
          }

          break;
      }
    }

    // draw the temporary image containing the highlight graphics on the output image at
    // the specified opacity

    float[][] matrixItems ={
      new float[] {1, 0, 0, 0, 0},
      new float[] {0, 1, 0, 0, 0},
      new float[] {0, 0, 1, 0, 0},
      new float[] {0, 0, 0, Convert.ToSingle(opacity), 0}, 
      new float[] {0, 0, 0, 0, 1}};

    ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

    ImageAttributes imageAtts = new ImageAttributes();
    imageAtts.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

    Rectangle drawRect = new Rectangle(0, 0, width, height);
    graphics.DrawImage(bitMap, drawRect, 0, 0, width, height, GraphicsUnit.Pixel, imageAtts);
  }

  private void DrawMarkup(Graphics graphics)
  {
    if (_appState.MarkupGroups.Count == 0)
    {
      return;
    }

    SolidBrush pointBrush = new SolidBrush(Color.Red);
    SolidBrush polygonBrush = new SolidBrush(Color.FromArgb(128, 255, 192, 192));
    Pen pen = new Pen(Color.Red, Convert.ToSingle(2 * _resolution));
    pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

    float dotSize = Convert.ToSingle(10 * _resolution);

    System.Drawing.Font font = AppSettings.MarkupFont;
    font = new System.Drawing.Font(font.FontFamily, Convert.ToSingle(font.Size * _resolution), font.Style, font.Unit);
    System.Drawing.Font coordinatesFont = AppSettings.CoordinatesFont;
    coordinatesFont = new System.Drawing.Font(coordinatesFont.FontFamily, Convert.ToSingle(coordinatesFont.Size * _resolution), coordinatesFont.Style, coordinatesFont.Unit);
    SolidBrush textBrush = new SolidBrush(Color.FromArgb(192, 0, 0));
    SolidBrush glowBrush = new SolidBrush(Color.Black);

    StringFormat format = new StringFormat();
    format.LineAlignment = StringAlignment.Far;

    Configuration config = AppContext.GetConfiguration();
    Configuration.ApplicationRow application = config.Application.FindByApplicationID(_appState.Application);
    string[] modes = application.IsCoordinateModesNull() ? new string[] { "ne" } : application.CoordinateModes.ToLower().Split(',');

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      foreach (string groupId in _appState.MarkupGroups)
      {
        string sql = String.Format("update {0}MarkupGroup set DateLastAccessed = ? where GroupID = {1}",
            AppSettings.ConfigurationTablePrefix, groupId);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Parameters.Add("@1", OleDbType.Date).Value = DateTime.Now;
          command.ExecuteNonQuery();
        }

        sql = String.Format("select Shape, Color, Glow, Text, Measured from {0}Markup where GroupID = {1} and Deleted = 0",
            AppSettings.ConfigurationTablePrefix, groupId);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          using (OleDbDataReader reader = command.ExecuteReader())
          {
            WKTReader wktReader = new WKTReader();

            while (reader.Read())
            {
              IGeometry geometry = wktReader.Read(reader.GetString(0));
              Color color = ColorTranslator.FromHtml(reader.GetString(1));
              bool isMeasured = !reader.IsDBNull(4) && Convert.ToInt32(reader.GetValue(4)) == 1;
              bool isGlow = !reader.IsDBNull(2);

              if (isGlow)
              {
                glowBrush.Color = ColorTranslator.FromHtml(reader.GetString(2));
              }

              switch (geometry.OgcGeometryType)
              {
                case OgcGeometryType.Point:
                  bool isText = !reader.IsDBNull(3);

                  if (isText)
                  {
                    textBrush.Color = color;
                    DrawText(graphics, (IPoint)geometry, reader.GetString(3), font, textBrush, isGlow ? glowBrush : null, 0, 0, format);
                  }
                  else
                  {
                    if (isMeasured)
                    {
                      pen.Color = color;
                      pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                      textBrush.Color = color;
                      DrawCoordinate(graphics, (IPoint)geometry, modes, pen, coordinatesFont, textBrush, isGlow ? glowBrush : null, format);
                    }
                    else
                    {
                      pointBrush.Color = color;
                      DrawPoint(graphics, (IPoint)geometry, pointBrush, dotSize);
                    }
                  }
                  break;

                case OgcGeometryType.LineString:
                  pen.Color = color;
                  pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                  DrawLineString(graphics, (ILineString)geometry, pen);
                  break;

                case OgcGeometryType.Polygon:
                  pen.Color = color;
                  pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                  polygonBrush.Color = Color.FromArgb(128, color);
                  DrawPolygon(graphics, (IPolygon)geometry, polygonBrush, null, pen);
                  break;
              }

              if (isMeasured && geometry.OgcGeometryType != OgcGeometryType.Point)
              {
                DrawMeasure(graphics, geometry);
              }
            }
          }
        }
      }
    }
  }

  private void DrawMeasure(Graphics graphics, IGeometry geometry)
  {
    string measureUnits = AppSettings.MeasureUnits;
    bool inFeet = measureUnits == "feet" || measureUnits == "both";
    bool inMeters = measureUnits == "meters" || measureUnits == "both";

    System.Drawing.Font font = AppSettings.MeasureFont;
    font = new System.Drawing.Font(font.FontFamily, Convert.ToSingle(font.Size * _resolution), font.Style, font.Unit);
    SolidBrush brush = new SolidBrush(Color.FromArgb(64, 64, 64));
    SolidBrush glowBrush = new SolidBrush(Color.White);

    StringFormat format = new StringFormat();
    format.Alignment = StringAlignment.Center;
    format.LineAlignment = StringAlignment.Center;

    double convert = 1 / (AppSettings.MapUnits == "feet" ? 1 : Constants.MetersPerFoot);
    StringCollection text = new StringCollection();

    switch (geometry.OgcGeometryType)
    {
      case OgcGeometryType.LineString:
        ILineString lineString = (ILineString)geometry;

        double d = lineString.Length * convert;

        if (inFeet)
        {
          text.Add(d < 5280 ? d.ToString("0") + " ft" : (d / 5280).ToString("0.0") + " mi");
        }

        if (inMeters)
        {
          d *= Constants.MetersPerFoot;
          text.Add(d < 1000 ? d.ToString("0") + " m" : (d / 1000).ToString("0.0") + " km");
        }

        IPoint p;
        double angle;

        GetMidpoint(lineString, out p, out angle);

        angle = -(angle * 180 / Math.PI);
        angle = angle < -90 || 90 < angle ? angle + 180 : angle < 0 ? angle + 360 : angle;

        p = _transform.ReverseTransform(p);

        float x = Convert.ToSingle(p.Coordinate.X * _resolution);
        float y = Convert.ToSingle(p.Coordinate.Y * _resolution);

        format.LineAlignment = StringAlignment.Far;

        int[] pos = new int[] { 0, 1, 2, 3, 5, 6, 7, 8, 4 };

        for (int i = 0; i < 9; ++i)
        {
          float offsetX = (pos[i] % 3) - 1;
          float offsetY = Convert.ToSingle(Math.Floor(pos[i] / 3.0)) - 1;

          System.Drawing.Drawing2D.GraphicsState state = graphics.Save();
          graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
          graphics.TranslateTransform(x, y);
          graphics.RotateTransform(Convert.ToSingle(angle));
          graphics.DrawString(text.Join("\n"), font, i < 8 ? glowBrush : brush, Convert.ToSingle(offsetX * _resolution), Convert.ToSingle(offsetY - 3 * _resolution), format);
          graphics.Restore(state);
        }
        break;

      case OgcGeometryType.Polygon:
        IPolygon polygon = (IPolygon)geometry;
        IPoint c = polygon.Centroid;

        if (c != null)
        {
          double a = polygon.Area * convert * convert;
          double acres = a / Constants.SquareFeetPerAcre;

          if (inFeet)
          {
            double squareMile = Constants.FeetPerMile * Constants.FeetPerMile;
            text.Add(a <= squareMile ? a.ToString("0") + " sq ft" : (a / squareMile).ToString("0.00") + " sq mi");
          }

          if (inMeters)
          {
            a *= Constants.MetersPerFoot * Constants.MetersPerFoot;
            text.Add(a <= 100000 ? a.ToString("0") + " sq m" : (a / 1000000).ToString("0.00") + " sq km");
          }

          if (inFeet)
          {
            text.Add(acres.ToString("0.00") + " acres");
          }

          DrawText(graphics, c, text.Join("\n"), font, brush, glowBrush, 0, 0, format);
        }
        break;
    }
  }

  private void DrawLineString(Graphics graphics, ILineString lineString, Pen pen)
  {
    lineString = _transform.ReverseTransform(lineString);
    PointF[] points = new PointF[lineString.Coordinates.Length];

    for (int i = 0; i < lineString.Coordinates.Length; ++i)
    {
      points[i].X = Convert.ToSingle(lineString.Coordinates[i].X * _resolution);
      points[i].Y = Convert.ToSingle(lineString.Coordinates[i].Y * _resolution);
    }

    graphics.DrawLines(pen, points);
  }

  private void DrawMultiLineString(Graphics graphics, IMultiLineString multiLineString, Pen pen)
  {
    foreach (ILineString lineString in multiLineString.Geometries.Cast<ILineString>())
    {
      DrawLineString(graphics, lineString, pen);
    }
  }

  private void DrawMultiPolygon(Graphics graphics, IMultiPolygon multiPolygon, Brush brush)
  {
    DrawMultiPolygon(graphics, multiPolygon, brush, null, null);
  }

  private void DrawMultiPolygon(Graphics graphics, IMultiPolygon multiPolygon, Brush brush, Pen bufferPen)
  {
    DrawMultiPolygon(graphics, multiPolygon, brush, bufferPen, null);
  }

  private void DrawMultiPolygon(Graphics graphics, IMultiPolygon multiPolygon, Brush brush, Pen bufferPen, Pen pen)
  {
    foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
    {
      DrawPolygon(graphics, polygon, brush, bufferPen, pen);
    }
  }

  private void DrawPoint(Graphics graphics, IPoint point, Brush brush, float dotSize)
  {
    point = _transform.ReverseTransform(point);

    float x = Convert.ToSingle(point.Coordinate.X * _resolution);
    float y = Convert.ToSingle(point.Coordinate.Y * _resolution);

    graphics.FillEllipse(brush, x - dotSize / 2, y - dotSize / 2, dotSize, dotSize);
  }

  private void DrawPolygon(Graphics graphics, IPolygon polygon, Brush brush)
  {
    DrawPolygon(graphics, polygon, brush, null, null);
  }

  private void DrawPolygon(Graphics graphics, IPolygon polygon, Brush brush, Pen bufferPen)
  {
    DrawPolygon(graphics, polygon, brush, bufferPen, null);
  }

  private void DrawPolygon(Graphics graphics, IPolygon polygon, Brush brush, Pen bufferPen, Pen pen)
  {
    IPolygon imagePolygon = _transform.ReverseTransform(polygon);

    if (brush != null)
    {
      int numPoints = imagePolygon.ExteriorRing.Coordinates.Length;

      if (imagePolygon.InteriorRings != null && imagePolygon.InteriorRings.Length > 0)
      {
        foreach (ILineString lineString in imagePolygon.InteriorRings)
        {
          numPoints += lineString.Coordinates.Length + 1;
        }
      }

      PointF[] points = new PointF[numPoints];

      for (int i = 0; i < imagePolygon.ExteriorRing.Coordinates.Length; ++i)
      {
        points[i].X = Convert.ToSingle(imagePolygon.ExteriorRing.Coordinates[i].X * _resolution);
        points[i].Y = Convert.ToSingle(imagePolygon.ExteriorRing.Coordinates[i].Y * _resolution);
      }

      if (imagePolygon.InteriorRings != null && imagePolygon.InteriorRings.Length > 0)
      {
        int offset = imagePolygon.ExteriorRing.Coordinates.Length;

        foreach (ILineString lineString in imagePolygon.InteriorRings)
        {
          for (int i = 0; i < lineString.Coordinates.Length; ++i)
          {
            points[i + offset].X = Convert.ToSingle(lineString.Coordinates[i].X * _resolution);
            points[i + offset].Y = Convert.ToSingle(lineString.Coordinates[i].Y * _resolution);
          }

          offset += lineString.Coordinates.Length;

          points[offset].X = Convert.ToSingle(imagePolygon.ExteriorRing.Coordinates[0].X * _resolution);
          points[offset].Y = Convert.ToSingle(imagePolygon.ExteriorRing.Coordinates[0].Y * _resolution);

          ++offset;
        }
      }

      graphics.FillPolygon(brush, points);
    }

    if (pen == null && bufferPen != null && imagePolygon.EnvelopeInternal.Width < 10 && imagePolygon.EnvelopeInternal.Height < 10)
    {
      pen = bufferPen;
    }

    if (pen != null)
    {
      DrawLineString(graphics, polygon.ExteriorRing, pen);

      foreach (ILineString lineString in polygon.InteriorRings)
      {
        DrawLineString(graphics, lineString, pen);
      }
    }
  }

  private void DrawText(Graphics graphics, IPoint point, string text, System.Drawing.Font font, Brush brush, double xOffset, double yOffset, StringFormat format)
  {
    point = _transform.ReverseTransform(point);

    float x = Convert.ToSingle((point.Coordinate.X + xOffset) * _resolution);
    float y = Convert.ToSingle((point.Coordinate.Y + yOffset) * _resolution);

    graphics.DrawString(text, font, brush, x, y, format);
  }

  private void DrawText(Graphics graphics, IPoint point, string text, System.Drawing.Font font, Brush brush, Brush glowBrush, double xOffset, double yOffset, StringFormat format)
  {
    if (glowBrush == null)
    {
      DrawText(graphics, point, text, font, brush, xOffset, yOffset, format);
      return;
    }

    point = _transform.ReverseTransform(point);

    float x = Convert.ToSingle((point.Coordinate.X + xOffset) * _resolution);
    float y = Convert.ToSingle((point.Coordinate.Y + yOffset) * _resolution);
    float r = Convert.ToSingle(_resolution);

    for (int i = -1; i <= 1; ++i)
    {
      for (int j = -1; j <= 1; ++j)
      {
        graphics.DrawString(text, font, glowBrush, x + r * i, y + r * j, format);
      }
    }

    graphics.DrawString(text, font, brush, x, y, format);
  }

  public MapImageData GetImage()
  {
    StringCollection visibleLayers = null;

    if (_appState.VisibleLayers.ContainsKey(_appState.MapTab))
    {
      visibleLayers = _appState.VisibleLayers[_appState.MapTab];
    }

    string keyExtent = _appState.Extent.ToDelimitedString();
    string keySize = _width.ToString() + "," + _height.ToString();
    string keyLayers = visibleLayers != null ? visibleLayers.ToString('|') : "";
    string key = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", _appState.MapTab, _appState.Level, keyExtent, keySize, _resolution, keyLayers);

    CommonImageType imageType = CommonImageType.Png;
    byte[] image = null;

    MapImageData mapImageData = AppContext.ServerImageCache.Retrieve(key);

    if (mapImageData != null)
    {
      imageType = mapImageData.Type;
      image = mapImageData.Image;
    }

    Configuration config = AppContext.GetConfiguration();
    Configuration.MapTabRow mapTab = config.MapTab.FindByMapTabID(_appState.MapTab);
    CommonDataFrame dataFrame = AppContext.GetDataFrame(mapTab);
      
    bool isInteractive = !mapTab.IsInteractiveLegendNull() && mapTab.InteractiveLegend == 1;

    // create the base image if not found in the cache

    if (image == null)
    {
      CommonMap map = dataFrame.GetMap(_width, _height, _extent);

      map.Resolution = _resolution;
      map.ImageType = CommonImageType.Png;

      double pixelSize = map.Extent.Width / _width;

      Dictionary<int, CommonLayer> layerList = new Dictionary<int, CommonLayer>();
      Dictionary<int, String> definitionList = new Dictionary<int, String>();
      List<String> mapTabLayerIds = new List<String>();

      foreach (Configuration.MapTabLayerRow mapTabLayer in mapTab.GetMapTabLayerRows())
      {
        Configuration.LayerRow layer = mapTabLayer.LayerRow;
        mapTabLayerIds.Add(layer.LayerID);

        CommonLayer commonLayer = dataFrame.Layers.FirstOrDefault(lyr => String.Compare(lyr.Name, layer.LayerName, true) == 0);
        int index = dataFrame.Layers.IndexOf(commonLayer);

        bool visibleAtScale = commonLayer.IsWithinScaleThresholds(pixelSize);
        bool shownInLegend = !mapTabLayer.IsShowInLegendNull() && mapTabLayer.ShowInLegend == 1;
        bool checkedInLegend = !isInteractive || mapTabLayer.IsCheckInLegendNull() || mapTabLayer.CheckInLegend < 0 || (visibleLayers != null && visibleLayers.Contains(layer.LayerID));

        bool validLevel = layer.IsLevelFieldNull() || !String.IsNullOrEmpty(_appState.Level);

        if (!layerList.ContainsKey(index) && visibleAtScale && (!shownInLegend || checkedInLegend) && validLevel)
        {
          if (commonLayer.Type == CommonLayerType.Image)
          {
            map.ImageType = CommonImageType.Jpg;
          }

          layerList.Add(index, commonLayer);
          definitionList.Add(index, layer.GetLevelQuery(commonLayer, _appState.Level));
        }
      }

      if (!mapTab.IsBaseMapIDNull())
      {
        foreach (Configuration.LayerRow layer in config.Layer.Where(o => !o.IsBaseMapIDNull() && o.BaseMapID == mapTab.BaseMapID))
        {
          if (!mapTabLayerIds.Contains(layer.LayerID))
          {
            CommonLayer commonLayer = dataFrame.Layers.FirstOrDefault(o => String.Compare(o.Name, layer.LayerName, true) == 0);
            int index = dataFrame.Layers.IndexOf(commonLayer);

            bool visibleAtScale = commonLayer.IsWithinScaleThresholds(pixelSize);

            if (!layerList.ContainsKey(index) && visibleAtScale)
            {
              if (commonLayer.Type == CommonLayerType.Image)
              {
                map.ImageType = CommonImageType.Jpg;
              }

              layerList.Add(index, commonLayer);
              definitionList.Add(index, layer.GetLevelQuery(commonLayer, _appState.Level));
            }
          }
        }
      }

      int[] indexes = new int[layerList.Keys.Count];
      layerList.Keys.CopyTo(indexes, 0);
      List<int> indexList = new List<int>(indexes);
      indexList.Sort();

      for (int i = 0; i < indexList.Count; ++i)
      {
        map.AddLayer(layerList[indexList[i]], definitionList[indexList[i]]);
      }

      imageType = map.ImageType;
      image = map.GetImageBytes();

      AppContext.ServerImageCache.Store(key, new MapImageData(imageType, image));
    }


    // draw the selected feature graphics and markup

    if (_appState.TargetIds.Count > 0 || _appState.SelectionIds.Count > 0 || _appState.MarkupGroups.Count > 0 || _appState.Coordinates.Count > 0)
    {
      Bitmap bitmap = new Bitmap(new MemoryStream(image));
      bitmap.SetResolution(dataFrame.Dpi, dataFrame.Dpi);
      Graphics graphics = Graphics.FromImage(bitmap);

      if (_appState.TargetIds.Count > 0 || _appState.SelectionIds.Count > 0)
      {
        StringCollection targetIds;
        StringCollection filteredIds;
        StringCollection selectionIds;

        PrepareIds(out targetIds, out filteredIds, out selectionIds);

        DrawFeatures(graphics, _appState.TargetLayer, filteredIds, AppSettings.FilteredColor, AppSettings.FilteredOpacity, AppSettings.FilteredPolygonMode, AppSettings.FilteredPenWidth, AppSettings.FilteredDotSize);
        DrawFeatures(graphics, _appState.SelectionLayer, selectionIds, AppSettings.SelectionColor, AppSettings.SelectionOpacity, AppSettings.SelectionPolygonMode, AppSettings.SelectionPenWidth, AppSettings.SelectionDotSize);
        DrawFeatures(graphics, _appState.TargetLayer, targetIds, AppSettings.TargetColor, AppSettings.TargetOpacity, AppSettings.TargetPolygonMode, AppSettings.TargetPenWidth, AppSettings.TargetDotSize);
        DrawFeatures(graphics, _appState.TargetLayer, _appState.ActiveMapId, AppSettings.ActiveColor, AppSettings.ActiveOpacity, AppSettings.ActivePolygonMode, AppSettings.ActivePenWidth, AppSettings.ActiveDotSize);

        IGeometry selectionBuffer = _appState.SelectionManager.GetSelectionBuffer();

        if (selectionBuffer != null)
        {
          Brush bufferBrush = new SolidBrush(Color.FromArgb(Convert.ToInt32(255 * AppSettings.BufferOpacity), AppSettings.BufferColor));
          Pen bufferPen = AppSettings.BufferOutlineOpacity > 0 ? new Pen(new SolidBrush(Color.FromArgb(Convert.ToInt32(255 * AppSettings.BufferOutlineOpacity), AppSettings.BufferOutlineColor)), AppSettings.BufferOutlinePenWidth) : null;
          
          switch (selectionBuffer.OgcGeometryType)
          {
            case OgcGeometryType.Polygon:
              DrawPolygon(graphics, (IPolygon)selectionBuffer, bufferBrush, null, bufferPen);
              break;

            case OgcGeometryType.MultiPolygon:
              DrawMultiPolygon(graphics, (IMultiPolygon)selectionBuffer, bufferBrush, null, bufferPen);
              break;
          }
        }
      }

      graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

      DrawMarkup(graphics);
      DrawCoordinates(graphics);

      MemoryStream memoryStream = new MemoryStream();

      if (imageType == CommonImageType.Jpg)
      {
        ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
        EncoderParameters encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
        bitmap.Save(memoryStream, imageCodecInfo, encoderParameters);
      }
      else
      {
        bitmap.Save(memoryStream, bitmap.RawFormat);
      }

      image = memoryStream.ToArray();
    }

    return new MapImageData(imageType, image);
  }

  private ImageCodecInfo GetEncoderInfo(string mimeType)
  {
    ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

    for (int i = 0; i < encoders.Length; ++i)
    {
      if (encoders[i].MimeType == mimeType)
      {
        return encoders[i];
      }
    }

    return null;
  }

  private void GetMidpoint(ILineString lineString, out IPoint midPoint, out double angle)
  {
    midPoint = null;
    angle = Double.NaN;

    double lineStringLength = lineString.Length;

    if (lineStringLength == 0)
    {
      return;
    }

    double halfLength = lineStringLength * 0.5;

    for (int i = 0; i < lineString.Coordinates.Length - 1; ++i)
    {
      Coordinate p0 = lineString.Coordinates[i];
      Coordinate p1 = lineString.Coordinates[i + 1];

      double dx = p1.X - p0.X;
      double dy = p1.Y - p0.Y;

      double segmentLength = Math.Sqrt(dx * dx + dy * dy);

      if (halfLength <= segmentLength)
      {
        double ratio = halfLength / segmentLength;
        midPoint = new NetTopologySuite.Geometries.Point(p0.X + ratio * dx, p0.Y + ratio * dy);
        angle = Math.Atan2(dy, dx);
        break;
      }
      else
      {
        halfLength -= segmentLength;
      }
    }
  }

  private void PrepareIds(out StringCollection targetIds, out StringCollection filteredIds, out StringCollection selectionIds)
  {
    targetIds = _appState.TargetIds.Clone();
    selectionIds = _appState.SelectionIds.Clone();

    // segregate the target IDs that pass through the query from
    // those that do not

    if (String.IsNullOrEmpty(_appState.Query))
    {
      filteredIds = new StringCollection();
    }
    else
    {
      Configuration config = AppContext.GetConfiguration();
      Configuration.QueryRow query = config.Query.FindByQueryID(_appState.Query);

      using (OleDbCommand command = query.GetDatabaseCommand())
      {
        command.Parameters[0].Value = _appState.TargetIds.Join(",");

        if (command.Parameters.Count > 1)
        {
          command.Parameters[1].Value = AppUser.GetRole();
        }

        using (OleDbDataReader reader = command.ExecuteReader())
        {
          int mapIdColumn = reader.GetOrdinal("MapID");

          filteredIds = targetIds;
          targetIds = new StringCollection();

          while (reader.Read())
          {
            if (!reader.IsDBNull(mapIdColumn))
            {
              string mapId = reader.GetValue(mapIdColumn).ToString();

              filteredIds.Remove(mapId);

              if (!targetIds.Contains(mapId))
              {
                targetIds.Add(mapId);
              }
            }
          }
        }

        command.Connection.Dispose();
      }
    }

    if (targetIds.Count > 0)
    {
      // remove the active ID from the targets

      if (_appState.ActiveMapId.Length > 0)
      {
        targetIds.Remove(_appState.ActiveMapId);
      }

      // remove the selection IDs from the targets if necessary

      if (_appState.TargetLayer == _appState.SelectionLayer && _appState.SelectionIds.Count > 0)
      {
        if (_appState.ActiveMapId.Length > 0)
        {
          selectionIds.Remove(_appState.ActiveMapId);
        }

        foreach (string selectionId in _appState.SelectionIds)
        {
          targetIds.Remove(selectionId);
        }
      }
    }
  }

  private string ToDms(double d)
  {
    double[] v = new double[3];
    double r = Math.Abs(d);

    v[0] = Math.Floor(r);
    r = (r - v[0]) * 60;
    v[1] = Math.Floor(r);
    r = (r - v[1]) * 60;
    v[2] = Math.Round(r * 10) / 10;

    if (v[2] >= 60)
    {
      v[2] -= 60;
      v[1] += 1;
    }

    if (v[1] >= 60)
    {
      v[1] -= 60;
      v[0] += 1;
    }

    return v[0].ToString() + "° " + v[1].ToString("00") + "' " + v[2].ToString("00.0") + "\"";
  }
}
