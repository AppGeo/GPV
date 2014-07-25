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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI.HtmlControls;
using AppGeo.Clients;
using AppGeo.Clients.Ags;
using AppGeo.Clients.ArcIms;

public static class AppContext
{
	public const string ServerImageCacheKey = "ServerImageCache";
	public const string BrowserImageCacheKey = "BrowserImageCache";

  public static string ConfigurationKey = DateTime.Now.ToString("yyyyMMddhhmmss");

  public static TimedCache<MapImageData> BrowserImageCache
  {
    get
    {
      TimedCache<MapImageData> imageCache;

      if (HttpContext.Current.Cache[BrowserImageCacheKey] == null)
      {
        imageCache = new TimedCache<MapImageData>(AppSettings.BrowserImageCacheTimeout, true);
        CacheInsert(BrowserImageCacheKey, imageCache);
      }
      else
      {
        imageCache = (TimedCache<MapImageData>)HttpContext.Current.Cache[BrowserImageCacheKey];
      }

      return imageCache;
    }
  }

  public static TimedCache<MapImageData> ServerImageCache
  {
    get
    {
      TimedCache<MapImageData> imageCache;

      if (HttpContext.Current.Cache[ServerImageCacheKey] == null)
      {
        imageCache = new TimedCache<MapImageData>(AppSettings.ServerImageCacheTimeout);
        CacheInsert(ServerImageCacheKey, imageCache);
      }
      else
      {
        imageCache = (TimedCache<MapImageData>)HttpContext.Current.Cache[ServerImageCacheKey];
      }

      return imageCache;
    }
  }

  public static void CacheConfiguration(Configuration config)
  {
    string key = "Configuration";

    foreach (DictionaryEntry entry in HttpContext.Current.Cache)
    {
      string entryKey = (string)entry.Key;

      if (entryKey != BrowserImageCacheKey && entryKey != ServerImageCacheKey)
      {
        HttpContext.Current.Cache.Remove(entryKey);
      }
    }

    CacheInsert(key, config);
    ConfigurationKey = DateTime.Now.ToString("yyyyMMddhhmmss");
  }

  private static void CacheInsert(string key, object obj)
  {
    HttpContext.Current.Cache.Insert(key, obj, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
  }

  private static Dictionary<String, CommonDataFrame> GetCachedDataFrames()
  {
    Cache cache = HttpContext.Current.Cache;
    string cacheKey = "DataFrames";
    Dictionary<String, CommonDataFrame> dataFrames = null;

    if (cache[cacheKey] != null)
    {
      dataFrames = (Dictionary<String, CommonDataFrame>)cache[cacheKey];
    }
    else
    {
      dataFrames = new Dictionary<String, CommonDataFrame>();
      CacheInsert(cacheKey, dataFrames);
    }

    return dataFrames;
  }

  private static Dictionary<String, CommonMapService> GetCachedServices()
  {
    Cache cache = HttpContext.Current.Cache;
    string cacheKey = "Services";
    Dictionary<String, CommonMapService> services = null;

    if (cache[cacheKey] != null)
    {
      services = (Dictionary<String, CommonMapService>)cache[cacheKey];
    }
    else
    {
      services = new Dictionary<String, CommonMapService>();
      CacheInsert(cacheKey, services);
    }

    return services;
  }

  public static Configuration GetConfiguration()
  {
    return GetConfiguration(false);
  }

	public static Configuration GetConfiguration(bool forceReload)
	{
		Cache cache = HttpContext.Current.Cache;
		string key = "Configuration";

		Configuration config;

    if (!forceReload && cache[key] != null)
    {
      config = (Configuration)cache[key];
    }
    else
    {
      config = Configuration.GetCurrent();
      config.CascadeDeactivated();
      config.RemoveDeactivated();
      config.ValidateConfiguration();
      config.RemoveValidationErrors();

      CacheConfiguration(config);
    }

		return config;
	}

	public static OleDbConnection GetDatabaseConnection()
	{
		OleDbConnection connection;

		try
		{
			connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
			connection.Open();
		}
		catch (Exception ex)
		{
			throw new AppException("Could not connect to database.", ex);
		}

		return connection;
	}

	public static CommonDataFrame GetDataFrame(string mapTabId)
	{
		Configuration config = GetConfiguration();
		Configuration.MapTabRow mapTab = config.MapTab.FindByMapTabID(mapTabId);
    return GetDataFrame(mapTab);
	}

  public static CommonDataFrame GetDataFrame(Configuration.MapTabRow mapTab)
	{
    Dictionary<String, CommonDataFrame> dataFrames = GetCachedDataFrames();

    string dataFrameKey = mapTab.GetDataFrameKey();
    CommonDataFrame dataFrame = null;

    if (dataFrames.ContainsKey(dataFrameKey))
    {
      dataFrame = dataFrames[dataFrameKey];
    }
    else
    {
      Dictionary<String, CommonMapService> services = GetCachedServices();
      CommonMapService service = null;

      string serviceKey = mapTab.GetServiceKey("AGS");

      if (services.ContainsKey(serviceKey))
      {
        service = services[serviceKey];
      }
      else
      {
        serviceKey = mapTab.GetServiceKey("ArcIMS");

        if (services.ContainsKey(serviceKey))
        {
          service = services[serviceKey];
        }
      }

      if (service == null)
      {
        service = GetService(mapTab, "AGS");

        if (service == null)
        {
          service = GetService(mapTab, "ArcIMS");
        }
      }

      if (service != null)
      {
        dataFrame = mapTab.IsDataFrameNull() ? service.DefaultDataFrame : service.DataFrames.FirstOrDefault(df => String.Compare(df.Name, mapTab.DataFrame, true) == 0);

        if (dataFrame != null)
        {
          dataFrames.Add(dataFrameKey, dataFrame);
        }
      }
    }

    return dataFrame;
	}

  private static CommonHost GetHost(Configuration.MapTabRow mapTab, string type)
  {
    string userName = mapTab.IsUserNameNull() ? "" : mapTab.UserName;
    string password = mapTab.IsPasswordNull() ? "" : mapTab.Password;

    CommonHost host = null;
    
    try
    {
      switch (type)
      {
        case "AGS":
          if (String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(password))
          {
            host = new AgsHost(mapTab.MapHost);
          }
          else
          {
            try
            {
              host = new AgsHost(mapTab.MapHost, userName, password, true);
            }
            catch { }

            if (host == null)
            {
              host = new AgsHost(mapTab.MapHost, userName, password, false);
            }
          }
          break;

        case "ArcIMS": 
          host = new ArcImsHost(mapTab.MapHost, userName, password); 
          break;
      }
    }
    catch { }

    return host;
  }

  private static CommonMapService GetService(Configuration.MapTabRow mapTab, string type)
  {
    Dictionary<String, CommonMapService> services = GetCachedServices();

    string serviceKey = mapTab.GetServiceKey(type);
    CommonMapService service = null;

    if (services.ContainsKey(serviceKey))
    {
      service = services[serviceKey];
    }
    else
    {
      CommonHost host = GetHost(mapTab, type);

      if (host != null)
      {
        try
        {
          service = host.GetMapService(mapTab.MapService);

          ArcImsService arcImsService = service as ArcImsService;

          if (arcImsService != null && !arcImsService.IsArcMap)
          {
            arcImsService.LoadToc(true);
          }

          services.Add(serviceKey, service);
        }
        catch { }
      }
    }

    return service;
  }
}
