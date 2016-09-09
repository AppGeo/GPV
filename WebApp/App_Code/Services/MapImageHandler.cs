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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;
using GeoAPI.Geometries;
using AppGeo.Clients;

public class MapImageHandler : WebServiceHandler
{
  [WebServiceMethod]
  private void DefaultMethod()
  {
    string key = Request.QueryString["key"];

    if (String.IsNullOrEmpty(key))
    {
      Response.End();
      return;
    }

    MapImageData mapImageData = AppContext.BrowserImageCache.Retrieve(key);

    if (mapImageData == null)
    {
      Response.End();
      return;
    }

    Response.ContentType = mapImageData.Type == CommonImageType.Png ? "image/png" : "image/jpeg";
    Response.BinaryWrite(mapImageData.Image);
  }

  private MapImageData CompositeMapImage(AppState appState, int width, int height)
  {
    MapMaker mapMaker = new MapMaker(appState, width, height);
    MapImageData mapImageData = mapMaker.GetImage();

    List<Bitmap> baseTiles = GetTileImages(appState, width, height, false);
    List<Bitmap> overlayTiles = GetTileImages(appState, width, height, true);

    if (baseTiles.Count == 0 && overlayTiles.Count == 0)
    {
      return mapImageData;
    }

    using (Bitmap composite = new Bitmap(width, height))
    {
      using (Graphics graphics = Graphics.FromImage(composite))
      {
        graphics.Clear(Color.Transparent);

        foreach (Bitmap tileImage in baseTiles)
        {
          graphics.DrawImage(tileImage, 0, 0);
        }

        using (MemoryStream imageStream = new MemoryStream(mapImageData.Image))
        {
          using (Bitmap mapImage = new Bitmap(imageStream))
          {
            graphics.DrawImage(mapImage, 0, 0);
          }
        }

        foreach (Bitmap tileImage in overlayTiles)
        {
          graphics.DrawImage(tileImage, 0, 0);
        }
      }

      using (MemoryStream imageStream = new MemoryStream())
      {
        composite.Save(imageStream, ImageFormat.Png);
        mapImageData.Image = imageStream.ToArray();
        mapImageData.Type = CommonImageType.Png;
      }
    }

    return mapImageData;
  }

  private string GetImageUrl(AppState appState, int width, int height)
  {
    MapMaker mapMaker = new MapMaker(appState, width, height);
    string key = AppContext.BrowserImageCache.Store(mapMaker.GetImage());
    return "Services/MapImage.ashx?key=" + key;
  }

  private List<Bitmap> GetTileImages(AppState appState, int width, int height, bool overlay)
  {
    List<Bitmap> tileImages = new List<Bitmap>();

    StringCollection visibleTiles = appState.VisibleTiles[appState.MapTab];
    double pixelSize = appState.Extent.Width / width;
    int level = Convert.ToInt32(Math.Log(Constants.BasePixelSize / pixelSize, 2));

    if (visibleTiles.Count > 0)
    {
      Configuration.MapTabRow mapTab = AppContext.GetConfiguration().MapTab.FindByMapTabID(appState.MapTab);

      foreach (Configuration.MapTabTileGroupRow mapTabTileGroup in mapTab.GetMapTabTileGroupRows().Where(o => visibleTiles.Contains(o.TileGroupID)))
      {
        double opacity = mapTabTileGroup.IsOpacityNull() ? 1 : mapTabTileGroup.Opacity;

        foreach (Configuration.TileLayerRow tileLayer in mapTabTileGroup.TileGroupRow.GetTileLayerRows())
        {
          bool isOverlay = !tileLayer.IsOverlayNull() && tileLayer.Overlay == 1;

          if (isOverlay == overlay)
          {
            Bitmap tileImage = TileAggregator.GetImage(tileLayer.URL, appState.Extent, level, opacity);
            tileImages.Add(tileImage);
          }
        }
      }
    }

    return tileImages;
  }

  [WebServiceMethod]
  private void MakeMapImage()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    int width = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["width"])));
    int height = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["height"])));
    ReturnJson(GetImageUrl(appState, width, height));
  }

  /// <summary>
  /// A smaller version that doesn't support the full state but also doesn't require two HTTP requests
  /// </summary>
  [WebServiceMethod]
  private void GetMapImage()
  {
    AppState appState = AppState.FromJson(Request.QueryString["state"]);

    // set the default map layers as visible
    Configuration.MapTabRow mapTab = Configuration.MapTab.FindByMapTabID(appState.MapTab);
    appState.VisibleLayers = new System.Collections.Generic.Dictionary<string,StringCollection>() {
      { appState.MapTab, new StringCollection(mapTab.GetMapTabLayerRows().Where(e => !e.IsCheckInLegendNull() && e.CheckInLegend == 1).Select(e => e.LayerID)) }
    };

    int width = Convert.ToInt32(Request.QueryString["width"]);
    int height = Convert.ToInt32(Request.QueryString["height"]);

    MapMaker mapMaker = new MapMaker(appState, width, height);
    MapImageData mapImageData = mapMaker.GetImage();

    Response.ContentType = mapImageData.Type == CommonImageType.Png ? "image/png" : "image/jpeg";
    Response.BinaryWrite(mapImageData.Image);
  }

  [WebServiceMethod]
  private void MakeOverviewImage()
  {
    AppState appState = new AppState();
    appState.Application = Request.Form["application"];
    Configuration.ApplicationRow application = Configuration.Application.First(o => o.ApplicationID == appState.Application);

    appState.MapTab = application.OverviewMapID;
    appState.Extent = application.GetFullExtentEnvelope();

    int width = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["width"])));
    int height = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["height"])));

    ReturnJson(GetImageUrl(appState, width, height));
  }

  /// <summary>
  /// A smaller version that doesn't support the full state but also doesn't require two HTTP requests
  /// </summary>
  [WebServiceMethod]
  private void GetOverviewImage()
  {
    Configuration.ApplicationRow application = Configuration.Application.First(o => o.ApplicationID == Request.Params["application"]);

    int width = Convert.ToInt32(Request.Params["width"]);
    int height = Convert.ToInt32(Request.Params["height"]);
    string[] bbox = Request.Params["bbox"].Split(',');
    
    AppState appState = new AppState()
    {
      Application = application.ApplicationID,
      MapTab = application.OverviewMapID,
      Extent = new Envelope(new Coordinate(Convert.ToDouble(bbox[0]), Convert.ToDouble(bbox[1])), new Coordinate(Convert.ToDouble(bbox[2]), Convert.ToDouble(bbox[3])))
    };

    MapMaker mapMaker = new MapMaker(appState, width, height);
    MapImageData mapImageData = mapMaker.GetImage();

    Response.ContentType = mapImageData.Type == CommonImageType.Png ? "image/png" : "image/jpeg";
    Response.BinaryWrite(mapImageData.Image);
  }

  [WebServiceMethod]
  private void SaveMapImage()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    int width = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["width"])));
    int height = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["height"])));

    MapImageData mapImageData = CompositeMapImage(appState, width, height);

    Response.ContentType = mapImageData.Type == CommonImageType.Png ? "image/png" : "image/jpeg";
    Response.AddHeader("Content-Disposition", "attachment; filename=Map." + (mapImageData.Type == CommonImageType.Png ? "png" : "jpg"));
    Response.BinaryWrite(mapImageData.Image);
  }

  [WebServiceMethod]
  private void SaveMapKml()
  {
    AppState appState = AppState.FromJson(Request.Form["state"]);
    int width = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["width"])));
    int height = Convert.ToInt32(Math.Round(Convert.ToDouble(Request.Form["height"])));

    MapImageData mapImageData = CompositeMapImage(appState, width, height);

    Configuration.ApplicationRow application = Configuration.Application.Select(String.Format("ApplicationID = '{0}'", appState.Application))[0] as Configuration.ApplicationRow;
    string appName = application.DisplayName;

    DateTime now = DateTime.Now;
    string timeStamp = now.ToString("yyyyMMddHHmmssff");
    string dateNow = now.ToString("MM/dd/yyyy hh:mm tt");

    string kmzName = String.Format("Map_{0}.kmz", timeStamp);
    string kmlName = String.Format("Map_{0}.kml", timeStamp);
    string imageName = String.Format("Map_{0}.", timeStamp) + (mapImageData.Type == CommonImageType.Png ? "png" : "jpg");

    CoordinateSystem coordSys = AppContext.AppSettings.MapCoordinateSystem;

    Coordinate g = coordSys.ToGeodetic(new Coordinate(appState.Extent.MinX, appState.Extent.MinY));
    double minLat = g.Y;
    double maxLat = g.Y;
    double minLon = g.X;
    double maxLon = g.X;

    g = coordSys.ToGeodetic(new Coordinate(appState.Extent.MinX, appState.Extent.MaxY));
    minLat = Math.Min(minLat, g.Y);
    maxLat = Math.Max(maxLat, g.Y);
    minLon = Math.Min(minLon, g.X);
    maxLon = Math.Max(maxLon, g.X);

    g = coordSys.ToGeodetic(new Coordinate(appState.Extent.MaxX, appState.Extent.MaxY));
    minLat = Math.Min(minLat, g.Y);
    maxLat = Math.Max(maxLat, g.Y);
    minLon = Math.Min(minLon, g.X);
    maxLon = Math.Max(maxLon, g.X);

    g = coordSys.ToGeodetic(new Coordinate(appState.Extent.MaxX, appState.Extent.MinY));
    minLat = Math.Min(minLat, g.Y);
    maxLat = Math.Max(maxLat, g.Y);
    minLon = Math.Min(minLon, g.X);
    maxLon = Math.Max(maxLon, g.X);

    Coordinate p = appState.Extent.Centre;
    Coordinate c = coordSys.ToGeodetic(p);
    p.X = appState.Extent.MaxX;
    Coordinate e = coordSys.ToGeodetic(p);

    double rotation = Math.Atan2(e.Y - c.Y, e.X - c.X) * 180 / Math.PI;

    string kml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <kml xmlns=""http://earth.google.com/kml/2.2"">
          <Folder>
            <name>{0}</name>
            <GroundOverlay>
              <name>Map: created {1}</name>
              <Icon>
                <href>{2}</href>
              </Icon>
              <LatLonBox>
                <north>{3}</north>
                <south>{4}</south>
                <east>{5}</east>
                <west>{6}</west>
                <rotation>{7}</rotation>
              </LatLonBox>
            </GroundOverlay>
          </Folder>
        </kml>";

    kml = String.Format(kml, appName, dateNow, imageName, maxLat, minLat, maxLon, minLon, rotation);

    Response.ContentType = "application/vnd.google-earth.kmz";
    Response.AddHeader("Content-Disposition", "attachment; filename=" + kmzName);

    ZipOutputStream zipStream = new ZipOutputStream(Response.OutputStream);

    MemoryStream memoryStream = new MemoryStream();
    byte[] buffer = (new UTF8Encoding()).GetBytes(kml);

    ZipEntry entry = new ZipEntry(kmlName);
    entry.Size = buffer.Length;
    zipStream.PutNextEntry(entry);
    zipStream.Write(buffer, 0, buffer.Length);

    entry = new ZipEntry(imageName);
    entry.Size = mapImageData.Image.Length;
    zipStream.PutNextEntry(entry);
    zipStream.Write(mapImageData.Image, 0, mapImageData.Image.Length);

    zipStream.Finish();
  }
}