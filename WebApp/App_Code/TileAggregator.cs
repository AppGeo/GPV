using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using GeoAPI.Geometries;

public class TileAggregator
{
  public static byte[] GetImageBytes(string tileCacheUrl, Envelope extent, int level, double opacity)
  {
    byte[] imageBytes;

    using (Bitmap bitmap = GetImage(tileCacheUrl, extent, level, opacity))
    using (MemoryStream imageStream = new MemoryStream())
    {
      bitmap.Save(imageStream, ImageFormat.Png);
      imageBytes = imageStream.ToArray();
    }

    return imageBytes;
  }


  public static Bitmap GetImage(string tileCacheUrl, Envelope extent, int level, double opacity)
  {
    // get the pixel size and tile size in meters

    double levelFactor = Math.Pow(2, -level);
    double pixelSize = Constants.BasePixelSize * levelFactor;
    double tileSize = Constants.WebMercatorDelta * levelFactor * 2;

    // get the output image dimensions in pixels and its global pixel origin

    int imageWidth = Convert.ToInt32(extent.Width / pixelSize);
    int imageHeight = Convert.ToInt32(extent.Height / pixelSize);
    int imageOriginX = Convert.ToInt32((extent.MinX + Constants.WebMercatorDelta) / pixelSize);
    int imageOriginY = Convert.ToInt32((Constants.WebMercatorDelta - extent.MaxY) / pixelSize);

    // get the starting/ending row/column numbers

    int tileRowStart = Convert.ToInt32(Math.Floor((Constants.WebMercatorDelta - extent.MaxY) / tileSize));
    int tileRowEnd = Convert.ToInt32(Math.Floor((Constants.WebMercatorDelta - extent.MinY) / tileSize));
    int tileColumnStart = Convert.ToInt32(Math.Floor((extent.MinX + Constants.WebMercatorDelta) / tileSize));
    int tileColumnEnd = Convert.ToInt32(Math.Floor((extent.MaxX + Constants.WebMercatorDelta) / tileSize));

    // loop through the rows and columns creating a thread to fetch each tile image

    List<List<byte[]>> tiles = new List<List<byte[]>>();
    List<Thread> threads = new List<Thread>();

    for (int r = tileRowStart; r <= tileRowEnd; ++r)
    {
      List<byte[]> row = new List<byte[]>();
      tiles.Add(row);

      for (int c = tileColumnStart; c <= tileColumnEnd; ++c)
      {
        row.Add(null);

        string tileUrl = tileCacheUrl.Replace("{s}", "a")
          .Replace("{z}", level.ToString())
          .Replace("{y}", r.ToString())
          .Replace("{x}", c.ToString());

        Thread t = new Thread(new ParameterizedThreadStart(GetTile));
        threads.Add(t);
        t.Start(new TileData(tileUrl, tiles, r - tileRowStart, c - tileColumnStart));
      }
    }

    // wait for all threads to finish

    while (!ThreadsAreFinished(threads))
    {
      Thread.Sleep(10);
    }

    // compile the output image from the tiles

    Bitmap bitmap = new Bitmap(imageWidth, imageHeight);

    using (Graphics graphics = Graphics.FromImage(bitmap))
    {
      graphics.Clear(Color.Transparent);

      for (int r = tileRowStart; r <= tileRowEnd; ++r)
      {
        for (int c = tileColumnStart; c <= tileColumnEnd; ++c)
        {
          byte[] tileBytes = tiles[r - tileRowStart][c - tileColumnStart];

          if (tileBytes != null)
          {
            using (MemoryStream tileStream = new MemoryStream(tileBytes))
            using (Bitmap tileBitmap = new Bitmap(tileStream))
            {
              int x = c * 256 - imageOriginX;
              int y = r * 256 - imageOriginY;

              ColorMatrix matrix = new ColorMatrix();
              matrix.Matrix33 = Convert.ToSingle(opacity);

              ImageAttributes attributes = new ImageAttributes();
              attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

              graphics.DrawImage(tileBitmap, new Rectangle(x, y, 256, 256), 0, 0, 256, 256, GraphicsUnit.Pixel, attributes);
            }
          }
        }
      }
    }

    return bitmap;
  }

  private static void GetTile(object td)
  {
    TileData tileData = (TileData)td;
    string url = tileData.Url;
    List<List<byte[]>> tiles = tileData.Tiles;
    int r = tileData.R;
    int c = tileData.C;

    WebClient webClient = new WebClient();

    try
    {
      tiles[r][c] = webClient.DownloadData(url);
    }
    catch { }
    finally
    {
      webClient.Dispose();
    }
  }

  private static bool ThreadsAreFinished(List<Thread> threads)
  {
    foreach (Thread t in threads)
    {
      if (t.ThreadState == System.Threading.ThreadState.Running)
      {
        return false;
      }
    }

    return true;
  }

  private class TileData
  {
    public string Url;
    public List<List<byte[]>> Tiles;
    public int R;
    public int C;

    public TileData(string url, List<List<byte[]>> tiles, int r, int c)
    {
      Url = url;
      Tiles = tiles;
      R = r;
      C = c;
    }
  }
}