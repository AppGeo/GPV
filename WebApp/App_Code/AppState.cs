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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Web.UI;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using ICSharpCode.SharpZipLib.GZip;

public class AppState
{
  private const char Separator = '\u0001';
  private const char Separator2 = '\u0002';
  private const char Separator3 = '\u0004';
  private const string Key = "AppState";

  private const char VersionMarker = '\u0003';
  private const string CurrentVersion = "5.0";

  public static AppState FromJson(string json)
  {
    return GetJsonSerializer().Deserialize<AppState>(json);
  }

  public static bool IsIn(HttpSessionState session)
  {
    return session[Key] != null;
  }

  public static bool IsIn(StateBag viewState)
  {
    return viewState[Key] != null;
  }

  public static void RemoveFrom(HttpSessionState session)
  {
    if (IsIn(session))
    {
      session.Remove(Key);
    }
  }

  public static void RemoveFrom(StateBag viewState)
  {
    if (IsIn(viewState))
    {
      viewState.Remove(Key);
    }
  }

  public static AppState RestoreFrom(HttpSessionState session)
  {
    return RestoreFrom(session, true);
  }

  public static AppState RestoreFrom(HttpSessionState session, bool remove)
  {
    if (IsIn(session))
    {
      AppState appState = FromString((string)session[Key]);

      if (remove)
      {
        RemoveFrom(session);
      }

      return appState;
    }
    else
    {
      return new AppState();
    }
  }

  public static AppState RestoreFrom(StateBag viewState)
  {
    return RestoreFrom(viewState, true);
  }

  public static AppState RestoreFrom(StateBag viewState, bool remove)
  {
    if (IsIn(viewState))
    {
      AppState appState = FromString((string)viewState[Key]);

      if (remove)
      {
        RemoveFrom(viewState);
      }

      return appState;
    }
    else
    {
      return new AppState();
    }
  }

  private static List<Markup> CoordinateMarkupFromString(string value)
  {
    List<Markup> markup = new List<Markup>();

    if (value.Length > 0)
    {
      string[] values = value.Split(Separator2);

      for (int i = 0; i < values.Length; ++i)
      {
        string[] coords = values[i].Split(',');
        string point = String.Format("POINT({0} {1})", coords[0], coords[1]);
        markup.Add(new Markup(point, "#000000", 1));
      }
    }

    return markup;
  }

  private static T FromJson<T>(string json)
  {
    return GetJsonSerializer().Deserialize<T>(json);
  }

  public static AppState FromString(string stateString)
  {
    Queue values = new Queue(stateString.Split(Separator));
    AppState appState = new AppState();

    string version = ((string)values.Peek())[0] != VersionMarker ? "2.0" : ((string)values.Dequeue()).Substring(1);
    
    int tab;
    FunctionTab functionTabs;

    switch (version)
    {
      case "2.0":
        appState.Application = (string)values.Dequeue();
        appState.MapTab = (string)values.Dequeue();
        appState.TargetLayer = (string)values.Dequeue();
        appState.SelectionLayer = (string)values.Dequeue();
        values.Dequeue();  // skip SelectionDistance
        appState.ActiveMapId = (string)values.Dequeue();
        appState.ActiveDataId = (string)values.Dequeue();
        appState.TargetIds = StringCollection.FromString((string)values.Dequeue());
        appState.SelectionIds = StringCollection.FromString((string)values.Dequeue());
        appState.Query = (string)values.Dequeue();
        appState.DataTab = (string)values.Dequeue();
        appState.MarkupCategory = (string)values.Dequeue();
        appState.MarkupGroups = StringCollection.FromString((string)values.Dequeue());

        tab = Convert.ToInt32((string)values.Dequeue());
        functionTabs = FunctionTab.All;

        switch (tab)
        {
          case 1: functionTabs = FunctionTab.Selection; break;
          case 2: functionTabs = FunctionTab.Markup; break;
          case 3: functionTabs = FunctionTab.None; break;
        }

        appState.FunctionTabs = functionTabs;
        appState.ActiveFunctionTab = functionTabs == FunctionTab.All ? FunctionTab.Selection : functionTabs;
        appState.Extent = ProjectExtent(EnvelopeExtensions.FromDelimitedString((string)values.Dequeue(), Separator2));
        break;

      case "2.1":
        appState.Application = (string)values.Dequeue();
        appState.MapTab = (string)values.Dequeue();
        appState.Action = (Action)(Convert.ToInt32((string)values.Dequeue()));
        appState.TargetLayer = (string)values.Dequeue();
        appState.TargetIds = StringCollection.FromString((string)values.Dequeue());
        appState.ActiveMapId = (string)values.Dequeue();
        appState.ActiveDataId = (string)values.Dequeue();
        appState.Proximity = (string)values.Dequeue();
        appState.SelectionLayer = (string)values.Dequeue();
        appState.SelectionIds = StringCollection.FromString((string)values.Dequeue());
        appState.Query = (string)values.Dequeue();
        appState.DataTab = (string)values.Dequeue();
        appState.MarkupCategory = (string)values.Dequeue();
        appState.MarkupGroups = StringCollection.FromString((string)values.Dequeue());

        tab = Convert.ToInt32((string)values.Dequeue());
        functionTabs = FunctionTab.All;

        switch (tab)
        {
          case 0: functionTabs = FunctionTab.None; break;
          case 1: functionTabs = FunctionTab.Selection; break;
          case 2: functionTabs = FunctionTab.Markup; break;
        }

        appState.FunctionTabs = functionTabs;
        appState.ActiveFunctionTab = functionTabs == FunctionTab.All ? FunctionTab.Selection : functionTabs;
        appState.Extent = ProjectExtent(EnvelopeExtensions.FromDelimitedString((string)values.Dequeue(), Separator2));
        appState.Markup = CoordinateMarkupFromString((string)values.Dequeue());
        break;

      case "2.4":
        appState.Application = (string)values.Dequeue();
        appState.MapTab = (string)values.Dequeue();
        appState.Action = (Action)(Convert.ToInt32((string)values.Dequeue()));
        appState.TargetLayer = (string)values.Dequeue();
        appState.TargetIds = StringCollection.FromString((string)values.Dequeue());
        appState.ActiveMapId = (string)values.Dequeue();
        appState.ActiveDataId = (string)values.Dequeue();
        appState.Proximity = (string)values.Dequeue();
        appState.SelectionLayer = (string)values.Dequeue();
        appState.SelectionIds = StringCollection.FromString((string)values.Dequeue());
        appState.Query = (string)values.Dequeue();
        appState.DataTab = (string)values.Dequeue();
        appState.MarkupCategory = (string)values.Dequeue();
        appState.MarkupGroups = StringCollection.FromString((string)values.Dequeue());
        appState.FunctionTabs = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.ActiveFunctionTab = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.Extent = ProjectExtent(EnvelopeExtensions.FromDelimitedString((string)values.Dequeue(), Separator2));
        appState.Markup = CoordinateMarkupFromString((string)values.Dequeue());
        break;

      case "2.5":
        appState.Application = (string)values.Dequeue();
        appState.MapTab = (string)values.Dequeue();
        appState.Action = (Action)(Convert.ToInt32((string)values.Dequeue()));
        appState.TargetLayer = (string)values.Dequeue();
        appState.TargetIds = StringCollection.FromString((string)values.Dequeue());
        appState.ActiveMapId = (string)values.Dequeue();
        appState.ActiveDataId = (string)values.Dequeue();
        appState.Proximity = (string)values.Dequeue();
        appState.SelectionLayer = (string)values.Dequeue();
        appState.SelectionIds = StringCollection.FromString((string)values.Dequeue());
        appState.Query = (string)values.Dequeue();
        appState.DataTab = (string)values.Dequeue();
        appState.MarkupCategory = (string)values.Dequeue();
        appState.MarkupGroups = StringCollection.FromString((string)values.Dequeue());
        appState.FunctionTabs = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.ActiveFunctionTab = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.Extent = ProjectExtent(EnvelopeExtensions.FromDelimitedString((string)values.Dequeue(), Separator2));
        appState.Markup = CoordinateMarkupFromString((string)values.Dequeue());
        appState.VisibleLayers = LayersFromString((string)values.Dequeue());
        break;

      case "3.1":
        appState.Application = (string)values.Dequeue();
        appState.MapTab = (string)values.Dequeue();
        appState.Action = (Action)(Convert.ToInt32((string)values.Dequeue()));
        appState.TargetLayer = (string)values.Dequeue();
        appState.TargetIds = StringCollection.FromString((string)values.Dequeue());
        appState.ActiveMapId = (string)values.Dequeue();
        appState.ActiveDataId = (string)values.Dequeue();
        appState.Proximity = (string)values.Dequeue();
        appState.SelectionLayer = (string)values.Dequeue();
        appState.SelectionIds = StringCollection.FromString((string)values.Dequeue());
        appState.Query = (string)values.Dequeue();
        appState.DataTab = (string)values.Dequeue();
        appState.MarkupCategory = (string)values.Dequeue();
        appState.MarkupGroups = StringCollection.FromString((string)values.Dequeue());
        appState.FunctionTabs = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.ActiveFunctionTab = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.Extent = ProjectExtent(EnvelopeExtensions.FromDelimitedString((string)values.Dequeue(), Separator2));
        appState.Markup = CoordinateMarkupFromString((string)values.Dequeue());
        appState.VisibleLayers = LayersFromString((string)values.Dequeue());
        appState.Level = (string)values.Dequeue();

        if (values.Count > 0)
        {
          var text = (string)values.Dequeue();

          if (!String.IsNullOrEmpty(text) && text != "1")
          {
            appState.Markup[0].Text = text;
          }
        }
        break;

      case "4.2":
        appState.Application = (string)values.Dequeue();
        appState.MapTab = (string)values.Dequeue();
        appState.Action = (Action)(Convert.ToInt32((string)values.Dequeue()));
        appState.TargetLayer = (string)values.Dequeue();
        appState.TargetIds = StringCollection.FromString((string)values.Dequeue());
        appState.ActiveMapId = (string)values.Dequeue();
        appState.ActiveDataId = (string)values.Dequeue();
        appState.Proximity = (string)values.Dequeue();
        appState.SelectionLayer = (string)values.Dequeue();
        appState.SelectionIds = StringCollection.FromString((string)values.Dequeue());
        appState.Query = (string)values.Dequeue();
        appState.DataTab = (string)values.Dequeue();
        appState.MarkupCategory = (string)values.Dequeue();
        appState.MarkupGroups = StringCollection.FromString((string)values.Dequeue());
        appState.Markup = FromJson<List<Markup>>((string)values.Dequeue());
        appState.FunctionTabs = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.ActiveFunctionTab = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.Extent = ProjectExtent(EnvelopeExtensions.FromDelimitedString((string)values.Dequeue(), Separator2));
        appState.VisibleLayers = LayersFromString((string)values.Dequeue());
        appState.Level = (string)values.Dequeue();
        break;

      case "5.0":
        appState.Application = (string)values.Dequeue();
        appState.MapTab = (string)values.Dequeue();
        appState.Search = (string)values.Dequeue();
        appState.SearchCriteria = FromJson<Dictionary<String, Object>>((string)values.Dequeue());
        appState.Action = (Action)(Convert.ToInt32((string)values.Dequeue()));
        appState.TargetLayer = (string)values.Dequeue();
        appState.TargetIds = StringCollection.FromString((string)values.Dequeue());
        appState.ActiveMapId = (string)values.Dequeue();
        appState.ActiveDataId = (string)values.Dequeue();
        appState.Proximity = (string)values.Dequeue();
        appState.SelectionLayer = (string)values.Dequeue();
        appState.SelectionIds = StringCollection.FromString((string)values.Dequeue());
        appState.Query = (string)values.Dequeue();
        appState.DataTab = (string)values.Dequeue();
        appState.MarkupCategory = (string)values.Dequeue();
        appState.MarkupGroups = StringCollection.FromString((string)values.Dequeue());
        appState.Markup = FromJson<List<Markup>>((string)values.Dequeue());
        appState.FunctionTabs = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.ActiveFunctionTab = (FunctionTab)(Convert.ToInt32((string)values.Dequeue()));
        appState.Extent = EnvelopeExtensions.FromDelimitedString((string)values.Dequeue(), Separator2);
        appState.VisibleLayers = LayersFromString((string)values.Dequeue());
        appState.VisibleTiles = LayersFromString((string)values.Dequeue());
        appState.Level = (string)values.Dequeue();
        break;
    }

    return appState;
  }

  public static AppState FromCompressedString(string stateString)
  {
    byte[] compressedData = Convert.FromBase64String(stateString.Replace("!", "/").Replace("*", "+").Replace("$", "="));
    GZipInputStream zipStream = new GZipInputStream(new MemoryStream(compressedData));

    int size;
    byte[] data = new byte[1024];
    StringBuilder builder = new StringBuilder();

    try
    {
      while ((size = zipStream.Read(data, 0, data.Length)) > 0)
      {
        builder.Append(Encoding.UTF8.GetString(data, 0, size));
      }
    }
    catch (Exception ex)
    {
      throw new AppException("Could not uncompress the provided application state string", ex);
    }
    finally
    {
      zipStream.Close();
    }

    return FromString(builder.ToString());
  }

  private static JavaScriptSerializer GetJsonSerializer()
  {
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    serializer.RegisterConverters(new JavaScriptConverter[] { new GeometryConverter() });
    return serializer;
  }

  private static Dictionary<String, StringCollection> LayersFromString(string value)
  {
    Dictionary<String, StringCollection> dict = new Dictionary<String, StringCollection>();

    if (value.Length > 0)
    {
      string[] layers = value.Split(Separator2);

      for (int i = 0; i < layers.Length; ++i)
      {
        StringCollection values = new StringCollection();
        values.AddRange(layers[i].Split(Separator3));
        string key = values[0];
        values.RemoveAt(0);
        dict.Add(key, values);
      }
    }

    return dict;
  }

  private static Envelope ProjectExtent(Envelope originalExtent)
  {
    AppSettings appSettings = AppContext.AppSettings;
    Envelope projectedExtent = originalExtent;

    if (appSettings.MapCoordinateSystem != null && appSettings.MeasureCoordinateSystem != null && !appSettings.MapCoordinateSystem.Equals(appSettings.MeasureCoordinateSystem))
    {
      projectedExtent = appSettings.MapCoordinateSystem.ToProjected(appSettings.MeasureCoordinateSystem.ToGeodetic(originalExtent));
    }

    return projectedExtent;
  }

  private SelectionManager _selectionManager = null;

  public string Application = "";
  public string MapTab = "";
  public string Level = "";

  public string Search = "";
  public Dictionary<String, Object> SearchCriteria = new Dictionary<String, Object>();

  public Action Action = Action.Select;

  public string TargetLayer = "";
  public StringCollection TargetIds = new StringCollection();
  public string ActiveMapId = "";
  public string ActiveDataId = "";

  public string Proximity = "";

  public string SelectionLayer = "";
  public StringCollection SelectionIds = new StringCollection();

  public string Query = "";
  public string DataTab = "";

  public string MarkupCategory = "";
  public StringCollection MarkupGroups = new StringCollection();
  public List<Markup> Markup = new List<Markup>();

  public FunctionTab FunctionTabs = FunctionTab.None;
  public FunctionTab ActiveFunctionTab = FunctionTab.None;
  public Envelope Extent = null;

  public Dictionary<String, StringCollection> VisibleLayers = new Dictionary<String, StringCollection>();
  public Dictionary<String, StringCollection> VisibleTiles = new Dictionary<String, StringCollection>();

  public AppState()
  {
    _selectionManager = new SelectionManager(this);
  }

  [ScriptIgnore]
  public SelectionManager SelectionManager
  {
    get
    {
      return _selectionManager;
    }
  }

  private string MarkupToJson(List<Markup> markup)
  {
    return GetJsonSerializer().Serialize(markup);
  }

  public void SaveTo(HttpSessionState session)
  {
    session[Key] = ToString();
  }

  public void SaveTo(StateBag viewState)
  {
    viewState[Key] = ToString();
  }

  private String ToJson(object obj)
  {
    return GetJsonSerializer().Serialize(obj);
  }

  private string CoordinatesToString(List<Coordinate> points)
  {
    StringCollection coords = new StringCollection();

    for (int i = 0; i < points.Count; ++i)
    {
      coords.Add(points[i].X.ToString() + "," + points[i].Y.ToString());
    }

    return coords.Join(Separator2.ToString());
  }

  private string LayersToString(Dictionary<String, StringCollection> dict)
  {
    StringCollection layers = new StringCollection();

    foreach (string key in dict.Keys)
    {
      string s = key;

      if (dict[key].Count > 0)
      {
        s += Separator3.ToString() + dict[key].Join(Separator3.ToString());
      }

      layers.Add(s);
    }

    return layers.Join(Separator2.ToString());
  }

  public string ToCompressedString()
  {
    byte[] data = Encoding.UTF8.GetBytes(ToString());

    MemoryStream memoryStream = new MemoryStream();
    GZipOutputStream zipStream = new GZipOutputStream(memoryStream);
    zipStream.Write(data, 0, data.Length);
    zipStream.Close();

    string s = Convert.ToBase64String(memoryStream.ToArray());
    return s.Replace("/", "!").Replace("+", "*").Replace("=", "$");
  }

  public override string ToString()
  {
    StringBuilder builder = new StringBuilder();

    builder.Append(VersionMarker + CurrentVersion + Separator);
    builder.Append(Application + Separator);
    builder.Append(MapTab + Separator);
    builder.Append(Search + Separator);
    builder.Append(ToJson(SearchCriteria) + Separator);
    builder.Append(Action.ToString("d") + Separator);
    builder.Append(TargetLayer + Separator);
    builder.Append(TargetIds.ToString() + Separator);
    builder.Append(ActiveMapId + Separator);
    builder.Append(ActiveDataId + Separator);
    builder.Append(Proximity + Separator);
    builder.Append(SelectionLayer + Separator);
    builder.Append(SelectionIds.ToString() + Separator);
    builder.Append(Query + Separator);
    builder.Append(DataTab + Separator);
    builder.Append(MarkupCategory + Separator);
    builder.Append(MarkupGroups.ToString() + Separator);
    builder.Append(ToJson(Markup) + Separator);
    builder.Append(FunctionTabs.ToString("d") + Separator);
    builder.Append(ActiveFunctionTab.ToString("d") + Separator);
    builder.Append(Extent.ToDelimitedString(Separator2) + Separator);
    builder.Append(LayersToString(VisibleLayers) + Separator);
    builder.Append(LayersToString(VisibleTiles) + Separator);
    builder.Append(Level + Separator);

    return builder.ToString();
  }

  public string ToJson()
  {
    return GetJsonSerializer().Serialize(this);
  }

  private class GeometryConverter : JavaScriptConverter
  {
    public override object Deserialize(IDictionary<String, object> dictionary, Type type, JavaScriptSerializer serializer)
    {
      if (type == typeof(Coordinate))
      {
        double[] coordinates = ((ArrayList)dictionary["coordinates"]).OfType<object>().Select(o => Convert.ToDouble(o)).ToArray();
        return new Coordinate(coordinates[0], coordinates[1]);
      }

      if (type == typeof(Envelope))
      {
        double[] bbox = ((ArrayList)dictionary["bbox"]).OfType<object>().Select(o => Convert.ToDouble(o)).ToArray();
        return new Envelope(new Coordinate(bbox[0], bbox[1]), new Coordinate(bbox[2], bbox[3]));
      }

      return null;
    }

    public override IDictionary<String, object> Serialize(object obj, JavaScriptSerializer serializer)
    {
      Dictionary<String, object> dictionary = new Dictionary<String, object>();

      Envelope envelope = obj as Envelope;

      if (envelope != null)
      {
        dictionary.Add("bbox", new double[] { envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY });
        return dictionary;
      }

      return null;
    }

    public override IEnumerable<Type> SupportedTypes
    {
      get
      {
        return new Type[] { typeof(Envelope) };
      }
    }
  }
}

