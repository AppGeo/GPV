//  Copyright 2017 Applied Geographics, Inc.
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
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using AppGeo.Clients;

public class SelectionManager
{
  private AppState _appState = null;
  private AppSettings _appSettings = null;

	public SelectionManager(AppState appState)
	{
    _appState = appState;
    _appSettings = Configuration.GetCurrent().AppSettings;
	}

  private double AdjustBufferDistance(double distance, IGeometry selectionShape)
  {
    if (_appSettings.MapCoordinateSystem.IsWebMercator && !_appSettings.MeasureCoordinateSystem.IsWebMercator)
    {
      IPoint p = selectionShape.Centroid;
      ILineString mapLine = new LineString(new Coordinate[] { p.Coordinate, new Coordinate(p.X + 1, p.Y) });
      ILineString measureLine = _appSettings.MeasureCoordinateSystem.ToProjected(_appSettings.MapCoordinateSystem.ToGeodetic(mapLine));
      distance /= measureLine.Length;
    }

    return distance;
  }

  public Envelope GetExtent(FeatureType featureType)
  {
    Envelope extent = new Envelope();

    string layerId = featureType == FeatureType.Selection ? _appState.SelectionLayer : _appState.TargetLayer;

    Configuration config = AppContext.GetConfiguration();
    Configuration.LayerRow layerRow = config.Layer.FindByLayerID(layerId);

    CommonDataFrame dataFrame = AppContext.GetDataFrame(_appState.MapTab);
    CommonLayer layer = dataFrame.Layers.FirstOrDefault(lyr => String.Compare(lyr.Name, layerRow.LayerName, true) == 0);

    string query = GetQuery(featureType, layerRow, layer);

    if (query != null)
    {
      extent = layer.GetFeatureExtent(query);
    }

    if (!extent.IsNull && extent.Width == 0 && extent.Height == 0)
    {
      extent = new Envelope(new Coordinate(extent.MinX - 50, extent.MinY - 50), new Coordinate(extent.MaxX + 50, extent.MaxY + 50));
    }

    return extent;
  }

  public DataTable GetFeatures(FeatureType featureType)
  {
    return GetFeatures(featureType, null);
  }

  public DataTable GetFeatures(FeatureType featureType, IGeometry spatialConstraint)
  {
    DataTable table = null;

    string layerId = featureType == FeatureType.Selection ? _appState.SelectionLayer : _appState.TargetLayer;

    if (layerId.Length > 0)
    {
      Configuration config = AppContext.GetConfiguration();
      Configuration.LayerRow layerRow = config.Layer.FindByLayerID(layerId);

      CommonDataFrame dataFrame = AppContext.GetDataFrame(_appState.MapTab);
      CommonLayer layer = dataFrame.Layers.FirstOrDefault(lyr => String.Compare(lyr.Name, layerRow.LayerName, true) == 0);

      string query = GetQuery(featureType, layerRow, layer);

      if (query != null)
      {
        CommonField keyField = layer.FindField(layerRow.KeyField);

        if (spatialConstraint == null)
        {
          table = layer.GetFeatureTable(String.Format("{0},{1}", layer.GeometryField.Name, keyField.Name), query);
        }
        else
        {
          table = layer.GetFeatureTable(String.Format("{0},{1}", layer.GeometryField.Name, keyField.Name), query, spatialConstraint);
        }
      }
    }

    return table;
  }

  private StringCollection GetIds(FeatureType featureType)
  {
    switch (featureType)
    {
      case FeatureType.Active:
        StringCollection c = new StringCollection();

        if (_appState.ActiveMapId.Length > 0)
        {
          c.Add(_appState.ActiveMapId);
        }

        return c;

      case FeatureType.Target:
        return _appState.TargetIds;

      case FeatureType.Selection:
        return _appState.SelectionIds;
    }

    return null;
  }

  private string GetQuery(FeatureType featureType, Configuration.LayerRow layerRow, CommonLayer layer)
  {
    return GetQuery(featureType, layerRow, layer, true);
  }

  private string GetQuery(FeatureType featureType, Configuration.LayerRow layerRow, CommonLayer layer, bool inSet)
  {
    StringCollection ids = GetIds(featureType);

    if (ids.Count == 0)
    {
      return null;
    }

    CommonField keyField = layer.FindField(layerRow.KeyField);
    string joinedIds = keyField.IsNumeric ? ids.Join(",") : String.Format("'{0}'", ids.Join("','"));
    return String.Format("{0} {1} ({2})", keyField.Name, inSet ? "in" : "not in", joinedIds);
  }

  public IGeometry GetSelectionBuffer()
  {
    IGeometry selectionBuffer = null;

    if (_appState.Action == Action.FindAllWithin && !String.IsNullOrEmpty(_appState.Proximity))
    {
      Configuration config = AppContext.GetConfiguration();
      Configuration.ProximityRow proximity = config.Proximity.FindByProximityID(_appState.Proximity);

      if (proximity.Distance > 0)
      {
        DataTable table = GetFeatures(FeatureType.Selection);

        if (table != null && table.Rows.Count > 0)
        {
          IGeometry selectionShape = MergeShapes(table);
          double distance = AdjustBufferDistance(proximity.Distance, selectionShape);
          selectionBuffer = selectionShape.Buffer(distance);
        }
      }
    }

    return selectionBuffer;
  }

  private IGeometry MergeShapes(DataTable table)
  {
    if (table.Rows.Count > 0)
    {
      IEnumerable<DataColumn> columns = table.Columns.Cast<DataColumn>();
      DataColumn geometryColumn = columns.First(c => c.DataType.IsSubclassOf(typeof(Geometry)));
      OgcGeometryType geometryType = ((IGeometry)table.Rows[0][geometryColumn]).OgcGeometryType;

      switch (geometryType)
      {
        case OgcGeometryType.Point:
        case OgcGeometryType.MultiPoint:
          List<IPoint> mergeMultiPoint = new List<IPoint>();

          foreach (DataRow row in table.Rows)
          {
            if (geometryType == OgcGeometryType.Point)
            {
              mergeMultiPoint.Add((IPoint)row[geometryColumn]);
            }
            else
            {
              IMultiPoint multiPoint = (IMultiPoint)row[geometryColumn];

              foreach (IPoint p in multiPoint)
              {
                mergeMultiPoint.Add(p);
              }
            }
          }

          return new MultiPoint(mergeMultiPoint.ToArray());

        case OgcGeometryType.MultiLineString:
          List<ILineString> mergeLineString = new List<ILineString>();

          foreach (DataRow row in table.Rows)
          {
            IMultiLineString multiLineString = (MultiLineString)row[geometryColumn];

            foreach (ILineString lineString in multiLineString.Geometries.Cast<ILineString>())
            {
              mergeLineString.Add(lineString);
            }
          }

          return new MultiLineString(mergeLineString.ToArray());

        case OgcGeometryType.MultiPolygon:
          List<IPolygon> mergePolygon = new List<IPolygon>();

          foreach (DataRow row in table.Rows)
          {
            MultiPolygon multiPolygon = (MultiPolygon)row[geometryColumn];

            foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
            {
              mergePolygon.Add(polygon);
            }
          }

          return new MultiPolygon(mergePolygon.ToArray());
      }
    }

    return null;
  }

  public bool SelectTargets()
  {
    if (String.IsNullOrEmpty(_appState.TargetLayer) || String.IsNullOrEmpty(_appState.SelectionLayer))
    {
      return false;
    }

    _appState.TargetIds.Clear();

    if (_appState.SelectionIds.Count == 0)
    {
      return false;
    }

    Configuration config = AppContext.GetConfiguration();
    Configuration.LayerRow targetLayerRow = config.Layer.FindByLayerID(_appState.TargetLayer);

    CommonDataFrame dataFrame = AppContext.GetDataFrame(_appState.MapTab);
    CommonLayer targetLayer = dataFrame.Layers.FirstOrDefault(lyr => String.Compare(lyr.Name, targetLayerRow.LayerName, true) == 0);
    CommonField keyField = targetLayer.FindField(targetLayerRow.KeyField);

    DataTable targetTable = null;
    string filter = "";
    string sort = "";
    bool truncated = false;

    IGeometry selectionShape;

    switch (_appState.Action)
    {
      case Action.FindAllWithin:
        Configuration.ProximityRow proximity = config.Proximity.FindByProximityID(_appState.Proximity);

        targetTable = GetFeatures(FeatureType.Selection);

        if (targetTable.Rows.Count > 0)
        {
          selectionShape = MergeShapes(targetTable);

          if (proximity.Distance > 0)
          {
            double distance = AdjustBufferDistance(proximity.Distance, selectionShape);
            selectionShape = selectionShape.Buffer(distance);
          }

          targetTable = targetLayer.GetFeatureTable(String.Format("{0},{1}", targetLayer.GeometryField.Name, keyField.Name), selectionShape);
        }
        break;

      case Action.FindNearest1:
      case Action.FindNearest2:
      case Action.FindNearest3:
      case Action.FindNearest4:
      case Action.FindNearest5:
        Envelope extent = config.Application.FindByApplicationID(_appState.Application).GetFullExtentEnvelope();

        double minDist = targetLayerRow.IsMinNearestDistanceNull() ? 100 : targetLayerRow.MinNearestDistance;
        double maxDist = targetLayerRow.IsMaxNearestDistanceNull() ? Math.Max(extent.Width, extent.Height) : targetLayerRow.MaxNearestDistance;
        int count = Convert.ToInt32(Enum.GetName(typeof(Action), _appState.Action).Substring(11));

        targetTable = GetFeatures(FeatureType.Selection);

        if (targetTable.Rows.Count > 0)
        {
          selectionShape = MergeShapes(targetTable);

          double distance = minDist;

          do
          {
            targetTable = targetLayer.GetFeatureTable(String.Format("{0},{1}", targetLayer.GeometryField.Name, keyField.Name), selectionShape.Buffer(distance));
            distance *= 1.414213562;
          }
          while ((targetTable == null || targetTable.Rows.Count < count) && distance < maxDist);

          if (targetTable != null)
          {
            targetTable.Columns.Add("Distance", typeof(double));
            targetTable.Columns.Add("Index", typeof(int));
            filter = "Index <= " + count.ToString();
            sort = "Index";

            DataColumn targetShapeColumn = targetTable.Columns.Cast<DataColumn>().First(c => c.DataType.IsSubclassOf(typeof(Geometry)));
            int targetDistanceColumn = targetTable.Columns.IndexOf("Distance");

            DataTable selectionTable = GetFeatures(FeatureType.Selection);
            DataColumn selectionShapeColumn = selectionTable.Columns.Cast<DataColumn>().First(c => c.DataType.IsSubclassOf(typeof(Geometry)));

            foreach (DataRow selectionRow in selectionTable.Rows)
            {
              selectionShape = (Geometry)selectionRow[selectionShapeColumn];

              foreach (DataRow targetRow in targetTable.Rows)
              {
                double d = selectionShape.Distance((Geometry)targetRow[targetShapeColumn]);

                if (targetRow.IsNull(targetDistanceColumn))
                {
                  targetRow[targetDistanceColumn] = d;
                }
                else
                {
                  targetRow[targetDistanceColumn] = Math.Min((double)targetRow[targetDistanceColumn], d);
                }
              }
            }

            DataRow[] targetRows = targetTable.Select("", "Distance");

            for (int i = 0; i < targetRows.Length; ++i)
            {
              targetRows[i]["Index"] = i + 1;
            }
          }
        }
        break;
    }

    if (targetTable != null)
    {
      int maxTargets = targetLayerRow.IsMaxNumberSelectedNull() ? Int32.MaxValue : targetLayerRow.MaxNumberSelected;
      int c = targetTable.Columns.IndexOf(keyField.Name);

      foreach (DataRow row in targetTable.Select(filter, sort))
      {
        if (!row.IsNull(c))
        {
          if (_appState.TargetIds.Count == maxTargets)
          {
            truncated = true;
            break;
          }

          _appState.TargetIds.Add(row[c].ToString());
        }
      }
    }

    return truncated;
  }
}