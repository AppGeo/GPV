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
using System.Text;

public class DBaseWriter
{
  private const int MinimumHeaderLength = 33;

  private DataTable _table = null;

  private char[] _fieldType;
  private int[] _fieldLength;
  private int[] _fieldScale;
  private int[] _fieldPrecision;

  private int _headerLength = 0;
  private int _recordLength = 1;

  public DBaseWriter(DataTable table)
  {
    PrepareTable(table);
  }

  public void Write(Stream stream)
  {
    BinaryWriter writer = new BinaryWriter(stream);

    WriteHeader(writer);
    WriteData(writer);

    writer.Close();
  }

  private void PrepareTable(DataTable sourceTable)
  {
    _table = new DataTable();

    _fieldType = new char[sourceTable.Columns.Count];
    _fieldLength = new int[sourceTable.Columns.Count];
    _fieldScale = new int[sourceTable.Columns.Count];
    _fieldPrecision = new int[sourceTable.Columns.Count];

    // define the internal data table (all strings) and
    // get the dBase schema

    int validColumnCount = 0;

    for (int i = 0; i < sourceTable.Columns.Count; ++i)
    {
      DataColumn column = sourceTable.Columns[i];

      string columnName = column.ColumnName.ToUpper();
      if (columnName.Length > 10)
      {
        columnName = columnName.Substring(0, 10);
      }

      _table.Columns.Add(columnName, typeof(string));

      _fieldLength[i] = 1;
      _fieldScale[i] = 0;
      _fieldPrecision[i] = 0;


      switch (column.DataType.Name)
      {
        case "Int16":
        case "Int32":
        case "Int64":
          _fieldType[i] = 'n';
          ++validColumnCount;
          break;

        case "Single":
        case "Double":
        case "Decimal":
          _fieldType[i] = 'N';
          ++validColumnCount;
          break;

        case "DateTime":
          _fieldType[i] = 'D';
          _fieldLength[i] = 8;
          ++validColumnCount;
          break;

        case "String":
          _fieldType[i] = 'C';
          ++validColumnCount;
          break;

        default:
          _fieldType[i] = 'x';
          break;
      }
    }

    // convert each value in the source table to a string
    // in the internal table, determine the character length
    // of each column

    string value;

    foreach (DataRow sourceRow in sourceTable.Rows)
    {
      DataRow newRow = _table.NewRow();

      for (int i = 0; i < sourceTable.Columns.Count; ++i)
      {
        if (_fieldType[i] != 'x')
        {
          value = "";

          switch (_fieldType[i])
          {
            case 'n':
              if (!sourceRow.IsNull(i))
              {
                value = sourceRow[i].ToString();

                if (value.Length > _fieldLength[i])
                {
                  _fieldLength[i] = value.Length;
                }
              }
              break;

            case 'N':
              if (!sourceRow.IsNull(i))
              {
                value = sourceRow[i].ToString();

                int scale;
                int prec = value.IndexOf('.');

                if (prec < 0)
                {
                  scale = value.Length;
                  prec = 0;
                }
                else
                {
                  scale = prec;
                  prec = value.Length - prec - 1;
                }

                if (scale > _fieldScale[i])
                {
                  _fieldScale[i] = scale;
                }

                if (prec > _fieldPrecision[i])
                {
                  _fieldPrecision[i] = prec;
                }

                _fieldLength[i] = _fieldScale[i] + 1 + _fieldPrecision[i];
              }
              break;

            case 'D':
              value = !sourceRow.IsNull(i) ? ((DateTime)sourceRow[i]).ToString("yyyyMMdd") : "        ";
              break;

            case 'C':
              if (!sourceRow.IsNull(i))
              {
                value = sourceRow[i].ToString();
                if (value.Length > 254)
                {
                  value = value.Substring(0, 254);
                }

                if (value.Length > _fieldLength[i])
                {
                  _fieldLength[i] = value.Length;
                }
              }
              break;
          }

          newRow[i] = value;
        }
      }

      _table.Rows.Add(newRow);
    }

    // compute the header length and record length

    _headerLength = MinimumHeaderLength + 32 * validColumnCount;

    for (int i = 0; i < _table.Columns.Count; ++i)
    {
      if (_fieldType[i] != 'x')
      {
        _recordLength += _fieldLength[i];
      }
    }

    // pass through the internal table padding values with 
    // spaces and zeros as necessary

    for (int i = 0; i < _fieldType.Length; ++i)
    {
      if (_fieldType[i] == 'n')
      {
        _fieldType[i] = 'N';
      }

      switch (_fieldType[i])
      {
        case 'N':
          foreach (DataRow row in _table.Rows)
          {
            value = (string)row[i];

            if (_fieldPrecision[i] > 0)
            {
              int prec = value.IndexOf('.');
              if (prec == -1)
              {
                value += ".";
                prec = 0;
              }
              else
              {
                prec = value.Length - prec - 1;
              }

              if (prec < _fieldPrecision[i])
              {
                int totalLength = value.Length + _fieldPrecision[i] - prec;
                value = value.PadRight(totalLength, '0');
              }
            }

            if (value.Length < _fieldLength[i])
            {
              value = value.PadLeft(_fieldLength[i], ' ');
            }

            row[i] = value;
          }
          break;

        case 'C':
          foreach (DataRow row in _table.Rows)
          {
            value = (string)row[i];

            if (value.Length < _fieldLength[i])
            {
              row[i] = value.PadRight(_fieldLength[i], ' ');
            }
          }
          break;
      }
    }
  }

  private void WriteHeader(BinaryWriter writer)
  {
    ASCIIEncoding ascii = new ASCIIEncoding();

    writer.Write((byte)3);

    DateTime date = DateTime.Now;
    writer.Write(Convert.ToByte(date.Year % 100));
    writer.Write(Convert.ToByte(date.Month));
    writer.Write(Convert.ToByte(date.Day));

    writer.Write(_table.Rows.Count);
    writer.Write(Convert.ToInt16(_headerLength));
    writer.Write(Convert.ToInt16(_recordLength));

    for (int i = 0; i < 20; ++i)
    {
      writer.Write((byte)0);
    }

    for (int i = 0; i < _table.Columns.Count; ++i)
    {
      if (_fieldType[i] != 'x')
      {
        string name = _table.Columns[i].ColumnName;
        writer.Write(ascii.GetBytes(name));

        for (int j = name.Length; j < 11; ++j)
        {
          writer.Write((byte)0);
        }

        writer.Write(_fieldType[i]);
        writer.Write((int)0);
        writer.Write(Convert.ToByte(_fieldLength[i]));
        writer.Write(Convert.ToByte(_fieldPrecision[i]));

        for (int j = 0; j < 14; ++j)
        {
          writer.Write((byte)0);
        }
      }
    }

    writer.Write((byte)13);
  }

  private void WriteData(BinaryWriter writer)
  {
    ASCIIEncoding ascii = new ASCIIEncoding();

    foreach (DataRow row in _table.Rows)
    {
      writer.Write((byte)32);

      for (int i = 0; i < _table.Columns.Count; ++i)
      {
        if (_fieldType[i] != 'x')
        {
          writer.Write(ascii.GetBytes((string)row[i]));
        }
      }
    }
  }
}
