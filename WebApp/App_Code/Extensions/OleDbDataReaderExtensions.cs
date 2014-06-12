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
using System.Data.OleDb;

public static class OleDbDataReaderExtensions
{
  public static int GetColumnIndex(this OleDbDataReader reader, string columnName)
  {
    int index = -1;

    for (int i = 0; i < reader.FieldCount; ++i)
    {
      if (String.Compare(columnName, reader.GetName(i), true) == 0)
      {
        index = i;
        break;
      }
    }

    return index;
  }
}