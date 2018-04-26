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
using System.Data.OleDb;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.IO;

public static class MarkupManager
{
  public static Envelope GetExtent(int[] groupIds)
  {
    Envelope extent = new Envelope();

    string idsString = String.Join(",", groupIds.Select(o => o.ToString()).ToArray());
    string sql = String.Format("select Shape from {0}Markup where GroupID in ({1}) and Deleted = 0", WebConfigSettings.ConfigurationTablePrefix, idsString);

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      using (OleDbCommand command = new OleDbCommand(sql, connection))
      {
        WKTReader wktReader = new WKTReader();

        using (OleDbDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            IGeometry geometry = wktReader.Read(reader.GetString(0));
            extent.ExpandToInclude(geometry.EnvelopeInternal);
          }
        }
      }

      if (!extent.IsNull)
      {
        if (extent.Width == 0 && extent.Height == 0)
        {
          extent = new Envelope(new Coordinate(extent.MinX - 50, extent.MinY - 50), new Coordinate(extent.MaxX + 50, extent.MaxY + 50));
        }

        extent.ScaleBy(1.2);
      }
    }

    return extent;
  }
}