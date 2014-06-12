<%@ WebHandler Language="C#" Class="CompiledSwatch" %>

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
using System.Web;
using System.Web.Caching;

public class CompiledSwatch : IHttpHandler
{
  public static object SyncRoot = new object();

  public void ProcessRequest(HttpContext context)
  {
    string mapTabId = context.Request.QueryString["maptab"];
    context.Response.Cache.SetCacheability(HttpCacheability.Private);
    context.Response.ContentType = "image/png";
    context.Response.BinaryWrite(GetCompiledSwatches(mapTabId));
  }

  public byte[] GetCompiledSwatches(string mapTabId)
  {
    Configuration config = AppContext.GetConfiguration();
    Configuration.MapTabRow mapTab = config.MapTab.FindByMapTabID(mapTabId);

    lock (SyncRoot)
    {
      Cache cache = HttpContext.Current.Cache;
      string cacheKey = "CompiledSwatches";
      string dataFrameKey = mapTab.GetDataFrameKey();
      byte[] image = null;

      Dictionary<String, byte[]> compiledSwatches = null;

      if (cache[cacheKey] != null)
      {
        compiledSwatches = (Dictionary<String, byte[]>)cache[cacheKey];
      }
      else
      {
        compiledSwatches = new Dictionary<String, byte[]>();
        HttpContext.Current.Cache.Insert(cacheKey, compiledSwatches, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
      }

      if (compiledSwatches.ContainsKey(dataFrameKey))
      {
        image = compiledSwatches[dataFrameKey];
      }
      else
      {
        image = AppContext.GetDataFrame(mapTab).GetCompiledSwatchImage(AppSettings.SwatchTileWidth, AppSettings.SwatchTileHeight);
        compiledSwatches.Add(dataFrameKey, image);
      }

      return image;
    }
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}