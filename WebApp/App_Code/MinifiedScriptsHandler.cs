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

public class MinifiedScriptsHandler : IHttpHandler
{
  private static object ScriptsLock = new object();

  public static List<ScriptItem> GetList()
  {
    return new List<ScriptItem>(new ScriptItem[] { 
      new ScriptItem("Scripts/jquery-2.1.4.min.js", false, true),
      new ScriptItem("Scripts/jquery.cookie.min.js", false, true),
      new ScriptItem("Scripts/jquery.autocomplete.min.js", false, true),
      new ScriptItem("Scripts/jquery-ui-datepicker.js", false, true),
      new ScriptItem("Scripts/jquery.mCustomScrollbar.js", false, true),
      new ScriptItem("Scripts/leaflet.js", false, true),
      new ScriptItem("Scripts/proj4-compressed.js", false, true),
      new ScriptItem("Scripts/proj4leaflet.js", false, true),
      new ScriptItem("Scripts/leaflet-gpv-extensions.js", true, true),
      new ScriptItem("Scripts/bootstrap.min.js", false, true),
      new ScriptItem("Scripts/NumericInput.js", true, true),
      new ScriptItem("Scripts/DateInput.js", true, true),
      new ScriptItem("Scripts/ColorSelector.js", true, true),
      new ScriptItem("Scripts/Extensions.js", true, true),
      new ScriptItem("Scripts/DataGrid.js", true, true),
      new ScriptItem("Scripts/GPV.js", true, true),
      new ScriptItem("Scripts/Configuration.js", true, true),
      new ScriptItem("Scripts/AppState.js", true, true),
      new ScriptItem("Scripts/Selection.js", true, true),
      new ScriptItem("Scripts/SearchPanel.js", true, true),
      new ScriptItem("Scripts/BaseLayer.js", true, true),
      new ScriptItem("Scripts/SelectionPanel.js", true, true),
      new ScriptItem("Scripts/LegendPanel.js", true, true),
      new ScriptItem("Scripts/LocationPanel.js", true, true),
      new ScriptItem("Scripts/MarkupPanel.js", true, true),
      new ScriptItem("Scripts/SharePanel.js", true, true),
      new ScriptItem("Scripts/MapTip.js", true, true),
      new ScriptItem("Scripts/Progress.js", true, true),
      new ScriptItem("Scripts/Viewer.js", true, true)
    });
  }

  private static string GetAppPath()
  {
    string path = HttpContext.Current.Server.MapPath("~");
    string remove = "\\Scripts";

    if (path.EndsWith(remove))
    {
      path = path.Substring(0, path.Length - remove.Length);
    }

    return path + "\\";
  }

  public static DateTime GetLastWriteTime()
  {
    string appPath = GetAppPath();
    return GetList().Select(o => (new FileInfo(appPath + o.FileName)).LastWriteTime).Max();
  }

  public void ProcessRequest(HttpContext context)
  {
    const string key = "Scripts";
    Cache cache = context.Cache;

    DateTime lastWriteTime = GetLastWriteTime();
    DatedText scripts = null;

    lock (ScriptsLock)
    {
      if (cache[key] != null)
      {
        scripts = (DatedText)cache[key];

        if (scripts.Date < lastWriteTime)
        {
          scripts = null;
        }
      }

      if (scripts == null)
      {
        scripts = new DatedText(lastWriteTime, GetMinifiedScripts());
        cache.Insert(key, scripts, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
      }
    }

    context.Response.ContentType = "text/javascript";
    context.Response.Write(scripts.Text);
  }

  private string GetMinifiedScripts()
  {
    Minifier minifier = new Minifier();
    StringBuilder roll = new StringBuilder();
    string appPath = GetAppPath();

    foreach (ScriptItem item in GetList())
    {
      string fileName = appPath + item.FileName;
      roll.AppendFormat("// =====  {0}  =====    {1}\n", item.FileName, item.Comment);
      roll.AppendFormat("{0}{1}\n\n", item.Minify ? minifier.MinifyJavaScript(File.ReadAllText(fileName)) : File.ReadAllText(fileName), item.AppendSemiColon ? ";" : "");
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

public class ScriptItem
{
  public string FileName = null;
  public bool Minify = false;
  public bool AppendSemiColon = false;
  public string Comment = null;

  public ScriptItem(string fileName, bool minify) : this(fileName, minify, false, null) { }

  public ScriptItem(string fileName, bool minify, bool appendSemiColon) : this(fileName, minify, appendSemiColon, null) { }

  public ScriptItem(string fileName, bool minify, bool appendSemiColon, string comment)
  {
    FileName = fileName;
    Minify = minify;
    AppendSemiColon = appendSemiColon;
    Comment = comment;
  }
}
