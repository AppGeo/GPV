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
using System.Linq;
using System.Web;
using System.Data.OleDb;

public static class Sequences
{
  public static int NextMarkupGroupId
  {
    get
    {
      return IncrementSequence(AppSettings.ConfigurationTablePrefix + "MarkupSequence", "NextGroupID");
    }
  }

  public static int NextMarkupId
  {
    get
    {
      return IncrementSequence(AppSettings.ConfigurationTablePrefix + "MarkupSequence", "NextMarkupID");
    }
  }

  private static int IncrementSequence(string tableName, string columnName)
  {
    int value = 0;

    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      OleDbTransaction transaction = null;

      try
      {
        transaction = connection.BeginTransaction();

        string sql = String.Format("select {0} from {1}", columnName, tableName);

        using (OleDbCommand command = new OleDbCommand(sql, connection))
        {
          command.Transaction = transaction;
          value = Convert.ToInt32(command.ExecuteScalar());

          command.CommandText = String.Format("update {0} set {1} = {1} + 1", tableName, columnName);
          command.ExecuteNonQuery();

          transaction.Commit();
        }
      }
      catch (Exception ex)
      {
        if (transaction != null)
        {
          try
          {
            transaction.Rollback();
          }
          catch { }
        }

        throw new AppException("Could not get next value for " + columnName, ex);
      }
    }

    return value;
  }
}