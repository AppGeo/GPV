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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Microsoft.Ajax.Utilities;

public class MinifiedStylesheetsHandler : IHttpHandler
{
  private static object StylesheetsLock = new object();

  public static List<String> GetList()
  {
    return new List<String>(new string[] { 
      "Styles/bootstrap.min.css",
      "Styles/font-awesome.css",
      "Styles/leaflet.css",
      "Styles/jquery-ui-datepicker.css",
      "Styles/Common.css",
      "Styles/Customize.css",
      "Styles/DataList.css",
      "Styles/Viewer.css",
      "Styles/MapTip.css",
      "Styles/Tab.css",
      "Styles/Map.css",
      "Styles/SelectionPanel.css",
      "Styles/SearchPanel.css",
      "Styles/LegendPanel.css",
      "Styles/LocationPanel.css",
      "Styles/MarkupPanel.css",
      "Styles/SharePanel.css",
      "Styles/Mobile.css"
    });
  }

  private static string GetAppPath()
  {
    string path = HttpContext.Current.Server.MapPath("~");
    string remove = "\\Styles";

    if (path.EndsWith(remove))
    {
      path = path.Substring(0, path.Length - remove.Length);
    }

    return path + "\\";
  }

  public static DateTime GetLastWriteTime()
  {
    string appPath = GetAppPath();
    return GetList().Select(o => (new FileInfo(appPath + o)).LastWriteTime).Max();
  }

  public void ProcessRequest(HttpContext context)
  {
    const string key = "Stylesheets";
    Cache cache = context.Cache;

    DateTime lastWriteTime = GetLastWriteTime();
    DatedText stylesheets = null;

    lock (StylesheetsLock)
    {
      if (cache[key] != null)
      {
        stylesheets = (DatedText)cache[key];

        if (stylesheets.Date < lastWriteTime)
        {
          stylesheets = null;
        }
      }

      if (stylesheets == null)
      {
        stylesheets = new DatedText(lastWriteTime, GetMinifiedStylesheets());
        cache.Insert(key, stylesheets, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
      }
    }

    context.Response.ContentType = "text/css";
    context.Response.Write(stylesheets.Text);
  }

  private string GetMinifiedStylesheets()
  {
    Minifier minifier = new Minifier();
    StringBuilder roll = new StringBuilder();
    string appPath = GetAppPath();

    foreach (string item in GetList())
    {
      string fileName = appPath + item;
      roll.AppendFormat("/* =====  {0}  ===== */\n", item);
      roll.Append(minifier.MinifyStyleSheet(File.ReadAllText(fileName)) + "\n\n");
    }

    return roll.ToString();
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }
}