<%@ WebHandler Language="C#" Class="CompiledIcons" %>

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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

public class CompiledIcons : IHttpHandler
{
  private int _tileWidth = 24;
  private int _tileHeight = 23;
  
  public void ProcessRequest(HttpContext context)
  {
    string imagePath = context.Server.MapPath("~/Development/Images/UI/Icons");
    
    List<String> files = new List<String>(Directory.GetFiles(imagePath));

    /* show CSS backgound-position selector for the specified file or all files */
    
    string imageFileName = context.Request.QueryString["selectorfor"];

    if (!String.IsNullOrEmpty(imageFileName))
    {
      context.Response.ContentType = "text/plain";

      if (imageFileName == "all")
      {
        for (int i = 0; i < files.Count; ++i)
        {
          context.Response.Write(GetSelector(files, i) + "\n");
        }
      }
      else
      {
        imageFileName = String.Format("{0}\\{1}", imagePath, imageFileName);
        int index = files.IndexOf(imageFileName);

        context.Response.Write(index >= 0 ? GetSelector(files, index) : "Image file not found");
      }
      
      return;
    }

    // if no file specified, generate the compiled icon image
    
    Bitmap bitmap = new Bitmap(files.Count * _tileWidth, _tileHeight, PixelFormat.Format32bppArgb);
    Graphics graphics = Graphics.FromImage(bitmap);
    graphics.Clear(Color.Transparent);

    for (int i = 0; i < files.Count; ++i)
    {
      Bitmap icon = new Bitmap(files[i]);
      graphics.DrawImage(icon, i * _tileWidth, 0);
      icon.Dispose();
    }

    MemoryStream memoryStream = new MemoryStream();
    bitmap.Save(memoryStream, ImageFormat.Png);
    
    context.Response.Cache.SetCacheability(HttpCacheability.Private);
    context.Response.ContentType = "image/png";
    context.Response.BinaryWrite(memoryStream.ToArray());

    graphics.Dispose();
    bitmap.Dispose();
  }

  private string GetSelector(List<String> files, int index)
  {
    string imageFileName = files[index];
    imageFileName = imageFileName.Substring(imageFileName.LastIndexOf("\\") + 1);

    return String.Format("  background-position: {0}px 0px;  /* {1} */", -index * _tileWidth, imageFileName);
  }
  
  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}