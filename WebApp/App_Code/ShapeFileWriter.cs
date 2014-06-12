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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

public class ShapeFileWriter
{
  private DataTable _table;
  private DataColumn _shapeColumn;
  private DBaseWriter _dbf;
  
  private int[] _shapeLength;
  private int[] _numParts;
  private int[] _numPoints;
  
  private int _shapeType;
  private int _fileLength = 50;
  
  private Envelope _extent = new Envelope();
  
  public string ProjectionString = null;
  
  public ShapeFileWriter(DataTable table)
  {
    PrepareTable(table);
  }
  
  private void PrepareTable(DataTable table)
  {
    DataColumn shapeColumn = null;
    
    foreach (DataColumn column in table.Columns)
    {
      if (column.DataType.GetInterfaces().Contains(typeof(IGeometry)))
      {
        shapeColumn = column;
        
        switch (shapeColumn.DataType.Name)
        {
          case "IPoint":
            _shapeType = 1;
            break;
          case "IMultiLineString":
            _shapeType = 3;
            break;
          case "IMultiPolygon":
            _shapeType = 5;
            break;
        }
      }
    }
    
    if (shapeColumn == null)
    {
      throw new ArgumentException("Table does not contain a shape column");
    }
    
    _table = table;
    _shapeColumn = shapeColumn;
    
    _shapeLength = new int[_table.Rows.Count];
    _numParts = new int[_table.Rows.Count];
    _numPoints = new int[_table.Rows.Count];
    
    for (int i = 0; i < _table.Rows.Count; ++i)
    {
      if (_table.Rows[i].IsNull(_shapeColumn))
      {
        _shapeLength[i] = 2;
      }
      else
      {
        IGeometry geometry = (IGeometry)_table.Rows[i][_shapeColumn];
        
        switch (_shapeType)
        {
          case 1:
            _shapeLength[i] = 10;
            break;
            
          case 3:
            IMultiLineString multiLineString = (IMultiLineString)geometry;
            _numParts[i] = multiLineString.Count;
            _numPoints[i] = 0;

            foreach (ILineString lineString in multiLineString.Geometries.Cast<ILineString>())
            {
              _numPoints[i] += lineString.Coordinates.Length;
            }
            
            _shapeLength[i] = 22 + 2 * _numParts[i] + 8 * _numPoints[i];
            break;
            
          case 5:
            IMultiPolygon multiPolygon = (IMultiPolygon)geometry;
            _numParts[i] = multiPolygon.Count;
            _numPoints[i] = 0;

            foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
            {
              _numPoints[i] += polygon.ExteriorRing.Coordinates.Length;
            }
            
            _shapeLength[i] = 22 + 2 * _numParts[i] + 8 * _numPoints[i];
            break;
        }

        _extent.ExpandToInclude(geometry.EnvelopeInternal);
      }
      
      _fileLength += _shapeLength[i] + 4;
    }
    
    _dbf = new DBaseWriter(_table);
  }
  
  public void MakeGeographic()
  {
    ProjectionString = "GEOGCS[\"GCS_WGS_1984\"," + 
      "DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\"," + 
      "6378137,298.257223563]],PRIMEM[\"Greenwich\",0]," + 
      "UNIT[\"Degree\",0.0174532925199433]]";
  }
  
  private byte[] ToBigEndianBytes(int value)
  {
    byte[] bytes = BitConverter.GetBytes(value);
    Array.Reverse(bytes);
    return bytes;
  }
  
  public void WriteDbf(Stream stream)
  {
    _dbf.Write(stream);
  }
  
  private void WriteHeader(BinaryWriter writer, int fileLength)
  {
    writer.Write(ToBigEndianBytes(9994));
    
    for (int i = 0; i < 5; ++i)
    {
      writer.Write((int)0);
    }
    
    writer.Write(ToBigEndianBytes(fileLength));
    writer.Write((int)1000);
    writer.Write(_shapeType);
    
    writer.Write(_extent.MinX);
    writer.Write(_extent.MinY);
    writer.Write(_extent.MaxX);
    writer.Write(_extent.MaxY);
    
    for (int i = 0; i < 4; ++i)
    {
      writer.Write((double)0);
    }
  }
  
  private void WriteIndex(BinaryWriter writer)
  {
    int offset = 50;
    
    for (int i = 0; i < _table.Rows.Count; ++i)
    {
      writer.Write(ToBigEndianBytes(offset));
      writer.Write(ToBigEndianBytes(_shapeLength[i]));
      offset += _shapeLength[i] + 4;
    }
  }
  
  private void WritePrj(Stream stream)
  {
    if (ProjectionString != null)
    {
      ASCIIEncoding ascii = new ASCIIEncoding();
      BinaryWriter writer = new BinaryWriter(stream);
      writer.Write(ascii.GetBytes(ProjectionString));
      writer.Close();
    }
  }
  
  private void WriteShapes(BinaryWriter writer)
  {
    for (int i = 0; i < _table.Rows.Count; ++i)
    {
      writer.Write(ToBigEndianBytes(i + 1));
      writer.Write(ToBigEndianBytes(_shapeLength[i]));
      
      if (_table.Rows[i].IsNull(_shapeColumn))
      {
        writer.Write((int)0);
      }
      else
      {
        writer.Write(_shapeType);

        IGeometry geometry = (IGeometry)_table.Rows[i][_shapeColumn];
        
        Envelope extent;
        int offset;
        
        switch (_shapeType)
        {
          case 1:
            IPoint point = (IPoint)geometry;
            writer.Write(point.Coordinate.X);
            writer.Write(point.Coordinate.Y);
            break;
              
          case 3:
            IMultiLineString multiLineString = (IMultiLineString)geometry;

            extent = multiLineString.EnvelopeInternal;
            writer.Write(extent.MinX);
            writer.Write(extent.MinY);
            writer.Write(extent.MaxX);
            writer.Write(extent.MaxY);
            
            writer.Write(_numParts[i]);
            writer.Write(_numPoints[i]);
            
            offset = 0;
            foreach (ILineString lineString in multiLineString.Geometries.Cast<ILineString>())
            {
              writer.Write(offset);
              offset += lineString.Coordinates.Length;
            }

            foreach (ILineString lineString in multiLineString.Geometries.Cast<ILineString>())
            {
              foreach (Coordinate p in lineString.Coordinates)
              {
                writer.Write(p.X);
                writer.Write(p.Y);
              }
            }
            break;
              
          case 5:
            IMultiPolygon multiPolygon = (IMultiPolygon)geometry;

            extent = multiPolygon.EnvelopeInternal;
            writer.Write(extent.MinX);
            writer.Write(extent.MinY);
            writer.Write(extent.MaxX);
            writer.Write(extent.MaxY);
            
            writer.Write(_numParts[i]);
            writer.Write(_numPoints[i]);
            
            offset = 0;
            foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
            {
              writer.Write(offset);
              offset += polygon.ExteriorRing.Coordinates.Length;
            }

            foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
            {
              foreach (Coordinate p in polygon.ExteriorRing.Coordinates)
              {
                writer.Write(p.X);
                writer.Write(p.Y);
              }
            }
            break;
        }
      }
    }
  }
  
  public void WriteShp(Stream stream)
  {
    BinaryWriter writer = new BinaryWriter(stream);

    WriteHeader(writer, _fileLength);
    WriteShapes(writer);
    
    writer.Close();
  }
  
  public void WriteShx(Stream stream)
  {
    int fileLength = 50 + 4 * _table.Rows.Count;
    
    BinaryWriter writer = new BinaryWriter(stream);

    WriteHeader(writer, fileLength);
    WriteIndex(writer);
    
    writer.Close();
  }
}
