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
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using AppGeo.Clients;
using AppGeo.Clients.Ags;
using AppGeo.Clients.ArcIms;
using System.Web.Script.Serialization;

public partial class Configuration
{
  private static Regex _searchSubstitutionRegex = new Regex(@"\s+\{0\}(\s+|$)");

  private static string[] TableNames
  {
    get
    {
      return new string[] {"ZoneLevel", "Application", "Connection", "MapTab", "Layer", "DataTab", "Query", "Search", "SearchInputField", "Proximity", 
				"ApplicationMapTab", "MapTabLayer", "LayerFunction", "LayerProximity", "PrintTemplate", 
				"PrintTemplateContent", "ApplicationPrintTemplate", "MarkupCategory", "ApplicationMarkupCategory", 
        "Zone", "Level", "ZoneLevelCombo", "MailingLabel", "ExternalMap"};
    }
  }

  public static Configuration GetCurrent()
  {
    Configuration config = new Configuration();
    string prefix = AppSettings.ConfigurationTablePrefix;
    
    using (OleDbConnection connection = AppContext.GetDatabaseConnection())
    {
      foreach (string tableName in TableNames)
      {
        try
        {
          DataColumnCollection columns = config.Tables[tableName].Columns;

          string sql = "select * from " + prefix + tableName;

          if (tableName == "MailingLabel")
          {
            sql += " where IsAvailable = 1";
          }

          if (tableName == "ExternalMap" || tableName == "PrintTemplate" || tableName == "Proximity")
          {
            sql += " order by SequenceNo";
          }
          else if (tableName == "ZoneLevelCombo")
          {
            sql = String.Format(@"select a.ZoneLevelID, a.ZoneID, a.LevelID, a.Active from {0}ZoneLevelCombo a
                inner join {0}Zone b on a.ZoneLevelID = b.ZoneLevelID and a.ZoneID = b.ZoneID
                inner join {0}Level c on a.ZoneLevelID = c.ZoneLevelID and a.LevelID = c.LevelID
                order by a.ZoneLevelID, b.SequenceNo, a.ZoneID, c.SequenceNo, a.LevelID", prefix);
          }
          else
          {
            sql += " order by " + columns[0].ColumnName;

            if (columns.Contains("SequenceNo"))
            {
              sql += ", SequenceNo";
            }
          }

          using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
          {
            adapter.Fill(config, tableName);
          }
        }
        catch (Exception ex)
        {
          throw new AppException("Could not load configuration table " + prefix + tableName, ex);
        }
      }
    }

    return config;
  }

  public static int GetParameterCount(OleDbCommand command, bool isSearch)
  {
    int c = -1;

    for (int i = 0; i <= 5; ++i)
    {
      try
      {
        using (OleDbCommand testCommand = new OleDbCommand(command.CommandText, command.Connection))
        {
          testCommand.CommandType = CommandType.StoredProcedure;

          for (int n = 1; n <= i; ++n)
          {
            string testValue = isSearch && i == 1 ? "1 = 0" : "0";
            testCommand.Parameters.AddWithValue(n.ToString(), testValue);
          }

          using (OleDbDataReader reader = testCommand.ExecuteReader())
          {
            c = i;
          }
        }
      }
      catch { }

      if (c > -1)
      {
        break;
      }
    }

    return c;
  }

  public void CascadeDeactivated()
  {
    // normalize Active to 1 or 0

    foreach (Configuration.ApplicationRow application in Application.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      application.Active = 0;
    }

    foreach (Configuration.ApplicationMapTabRow applicationMapTab in ApplicationMapTab)
    {
      applicationMapTab.Active = 1;
    }

    foreach (Configuration.MapTabRow mapTab in MapTab.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      mapTab.Active = 0;
    }

    foreach (Configuration.MapTabLayerRow mapTabLayer in MapTabLayer)
    {
      mapTabLayer.Active = 1;
    }

    foreach (Configuration.LayerRow layer in Layer.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      layer.Active = 0;
    }

    foreach (Configuration.ConnectionRow connection in Connection.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      connection.Active = 0;
    }

    foreach (Configuration.QueryRow query in Query.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      query.Active = 0;
    }

    foreach (Configuration.DataTabRow dataTab in DataTab.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      dataTab.Active = 0;
    }

    foreach (Configuration.SearchRow search in Search.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      search.Active = 0;
    }

    foreach (Configuration.SearchInputFieldRow searchInputField in SearchInputField.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      searchInputField.Active = 0;
    }

    foreach (Configuration.LayerFunctionRow layerFunction in LayerFunction.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      layerFunction.Active = 0;
    }

    foreach (Configuration.LayerProximityRow layerProximity in LayerProximity)
    {
      layerProximity.Active = 1;
    }

    foreach (Configuration.ProximityRow proximity in Proximity.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      proximity.Active = 0;
    }

    foreach (Configuration.ApplicationMarkupCategoryRow applicationMarkupCategory in ApplicationMarkupCategory)
    {
      applicationMarkupCategory.Active = 1;
    }

    foreach (Configuration.MarkupCategoryRow markupCategory in MarkupCategory.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      markupCategory.Active = 0;
    }

    foreach (Configuration.ApplicationPrintTemplateRow applicationPrintTemplate in ApplicationPrintTemplate)
    {
      applicationPrintTemplate.Active = 1;
    }

    foreach (Configuration.PrintTemplateRow printTemplate in PrintTemplate.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      printTemplate.Active = 0;
    }

    foreach (Configuration.PrintTemplateContentRow printTemplateContent in PrintTemplateContent.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      printTemplateContent.Active = 0;
    }

    foreach (Configuration.ZoneLevelRow zoneLevel in ZoneLevel.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      zoneLevel.Active = 0;
    }

    foreach (Configuration.ZoneRow zone in Zone.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      zone.Active = 0;
    }

    foreach (Configuration.LevelRow level in Level.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      level.Active = 0;
    }

    foreach (Configuration.ZoneLevelComboRow zoneLevelCombo in ZoneLevelCombo.Where(o => o.IsActiveNull() || o.Active != 1))
    {
      zoneLevelCombo.Active = 0;
    }

    // cascade deactivations

    foreach (Configuration.ApplicationMapTabRow applicationMapTab in ApplicationMapTab.Where(o => o.ApplicationRow.Active == 0 || o.MapTabRow.Active == 0))
    {
      applicationMapTab.Active = 0;
    }

    foreach (Configuration.MapTabRow mapTab in MapTab)
    {
      bool connectedByAppMapTab = mapTab.GetApplicationMapTabRows().Length > 0;
      bool connectedByOverview = Application.Any(o => !o.IsOverviewMapIDNull() && o.OverviewMapID == mapTab.MapTabID);

      if (connectedByAppMapTab || connectedByOverview)
      {
        bool allAppMapTabInactive = connectedByAppMapTab ? mapTab.GetApplicationMapTabRows().All(o => o.Active == 0) : false;
        bool allOverviewInactive = connectedByOverview ? Application.Where(o => !o.IsOverviewMapIDNull() && o.OverviewMapID == mapTab.MapTabID).All(o => o.Active == 0) : false;
        bool inactive = false;

        if (connectedByAppMapTab && connectedByOverview)
        {
          inactive = allAppMapTabInactive && allOverviewInactive;
        }
        else if (connectedByAppMapTab)
        {
          inactive = allAppMapTabInactive;
        }
        else if (connectedByOverview)
        {
          inactive = allOverviewInactive;
        }

        if (inactive)
        {
          mapTab.Active = 0;
        }
      }
    }

    foreach (Configuration.MapTabLayerRow mapTabLayer in MapTabLayer.Where(o => o.MapTabRow.Active == 0 || o.LayerRow.Active == 0))
    {
      mapTabLayer.Active = 0;
    }

    foreach (Configuration.LayerRow layer in Layer.Where(o => o.GetMapTabLayerRows().Length > 0 && o.GetMapTabLayerRows().All(o2 => o2.Active == 0)))
    {
      layer.Active = 0;
    }

    foreach (Configuration.QueryRow query in Query.Where(o => o.LayerRow.Active == 0 || (!o.IsConnectionIDNull() && o.ConnectionRow.Active == 0)))
    {
      query.Active = 0;
    }

    foreach (Configuration.DataTabRow dataTab in DataTab.Where(o => o.LayerRow.Active == 0 || (!o.IsConnectionIDNull() && o.ConnectionRow.Active == 0)))
    {
      dataTab.Active = 0;
    }

    foreach (Configuration.LayerFunctionRow layerFunction in LayerFunction.Where(o => o.LayerRow.Active == 0 || (!o.IsConnectionIDNull() && o.ConnectionRow.Active == 0)))
    {
      layerFunction.Active = 0;
    }

    foreach (Configuration.LayerProximityRow layerProximity in LayerProximity.Where(o => o.LayerRow.Active == 0 || o.ProximityRow.Active == 0))
    {
      layerProximity.Active = 0;
    }

    foreach (Configuration.ProximityRow proximity in Proximity.Where(o => !o.IsIsDefaultNull() && o.IsDefault == 0 && o.GetLayerProximityRows().Length > 0 && o.GetLayerProximityRows().All(o2 => o2.Active == 0)))
    {
      proximity.Active = 0;
    }

    foreach (Configuration.SearchRow search in Search.Where(o => o.LayerRow.Active == 0 || (!o.IsConnectionIDNull() && o.ConnectionRow.Active == 0)))
    {
      search.Active = 0;
    }

    foreach (Configuration.SearchInputFieldRow searchInputField in SearchInputField.Where(o => o.SearchRow.Active == 0 || (!o.IsConnectionIDNull() && o.ConnectionRow.Active == 0)))
    {
      searchInputField.Active = 0;
    }

    foreach (Configuration.ApplicationMarkupCategoryRow applicationMarkupCategory in ApplicationMarkupCategory.Where(o => o.ApplicationRow.Active == 0 || o.MarkupCategoryRow.Active == 0))
    {
      applicationMarkupCategory.Active = 0;
    }

    foreach (Configuration.MarkupCategoryRow markupCategory in MarkupCategory.Where(o => o.GetApplicationMarkupCategoryRows().Length > 0 && o.GetApplicationMarkupCategoryRows().All(o2 => o2.Active == 0)))
    {
      markupCategory.Active = 0;
    }

    foreach (Configuration.ApplicationPrintTemplateRow applicationPrintTemplate in ApplicationPrintTemplate.Where(o => o.ApplicationRow.Active == 0 || o.PrintTemplateRow.Active == 0))
    {
      applicationPrintTemplate.Active = 0;
    }

    foreach (Configuration.PrintTemplateRow printTemplate in PrintTemplate.Where(o => !o.IsAlwaysAvailableNull() && o.AlwaysAvailable == 0 && o.GetApplicationPrintTemplateRows().Length > 0 && o.GetApplicationPrintTemplateRows().All(o2 => o2.Active == 0)))
    {
      printTemplate.Active = 0;
    }

    foreach (Configuration.PrintTemplateContentRow printTemplateContent in PrintTemplateContent.Where(o => o.PrintTemplateRow.Active == 0))
    {
      printTemplateContent.Active = 0;
    }

    foreach (Configuration.ZoneLevelRow zoneLevel in ZoneLevel.Where(o => o.GetApplicationRows().Length > 0 && o.GetApplicationRows().All(o2 => o2.Active == 0)))
    {
      zoneLevel.Active = 0;
    }

    foreach (Configuration.ZoneRow zone in Zone.Where(o => o.ZoneLevelRow.Active == 0))
    {
      zone.Active = 0;
    }

    foreach (Configuration.LevelRow level in Level.Where(o => o.ZoneLevelRow.Active == 0))
    {
      level.Active = 0;
    }

    foreach (Configuration.ZoneLevelComboRow zoneLevelCombo in ZoneLevelCombo.Where(o => o.ZoneRowParent.Active == 0 || o.LevelRowParent.Active == 0))
    {
      zoneLevelCombo.Active = 0;
    }

    // disconnect deactivated zone/level specs from applications

    foreach (Configuration.ApplicationRow application in Application.Where(o => !o.IsZoneLevelIDNull() && o.ZoneLevelRow.Active == 0))
    {
      application.ZoneLevelID = null;
    }
  }

  public void RemoveDeactivated()
  {
    EnforceConstraints = false;

    for (int i = TableNames.Length - 1; i >= 0; --i)
    {
      DataTable table = Tables[TableNames[i]];
      int activeColumn = table.Columns.IndexOf("Active");

      if (activeColumn >= 0)
      {
        for (int j = table.Rows.Count - 1; j >= 0; --j)
        {
          if (table.Rows[j].IsNull(activeColumn) || (short)table.Rows[j][activeColumn] != 1)
          {
            table.Rows.RemoveAt(j);
          }
        }
      }
    }

    EnforceConstraints = true;
    AcceptChanges();
  }

  public void RemoveValidationErrors()
  {
    EnforceConstraints = false;

    for (int i = TableNames.Length - 1; i >= 0; --i)
    {
      DataTable table = Tables[TableNames[i]];
      int errorColumn = table.Columns.IndexOf("ValidationError");

      if (errorColumn >= 0)
      {
        for (int j = table.Rows.Count - 1; j >= 0; --j)
        {
          if (!table.Rows[j].IsNull(errorColumn))
          {
            table.Rows.RemoveAt(j);
          }
        }
      }
    }

    EnforceConstraints = true;
    AcceptChanges();
  }

  public void ValidateConfiguration()
  {
    ValidateDataSources();
    ValidateMapServices();

    ValidateZoneLevels();

    // for interrelated configuration elements, one element may become
    // invalid if another one does, so iterate through the validation
    // of these until no new validation errors are found

    bool newErrorsFound = false;

    do
    {
      newErrorsFound = ValidateApplications();
      newErrorsFound = ValidateApplicationMapTabs() || newErrorsFound;
      newErrorsFound = ValidateMapTabs() || newErrorsFound;
      newErrorsFound = ValidateMapTabLayers() || newErrorsFound;
      newErrorsFound = ValidateLayers() || newErrorsFound;
      newErrorsFound = ValidateLayerProximities() || newErrorsFound;
      newErrorsFound = ValidateProximities() || newErrorsFound;
      newErrorsFound = ValidateLayerFunctions() || newErrorsFound;
      newErrorsFound = ValidateDataTabs() || newErrorsFound;
      newErrorsFound = ValidateQueries() || newErrorsFound;
      newErrorsFound = ValidateSearches() || newErrorsFound;
      newErrorsFound = ValidateSearchInputField() || newErrorsFound;

      if (newErrorsFound)
      {
        newErrorsFound = ValidateSearches() || newErrorsFound;
        newErrorsFound = ValidateQueries() || newErrorsFound;
        newErrorsFound = ValidateDataTabs() || newErrorsFound;
        newErrorsFound = ValidateLayerFunctions() || newErrorsFound;
        newErrorsFound = ValidateProximities() || newErrorsFound;
        newErrorsFound = ValidateLayerProximities() || newErrorsFound;
        newErrorsFound = ValidateLayers() || newErrorsFound;
        newErrorsFound = ValidateMapTabLayers() || newErrorsFound;
        newErrorsFound = ValidateMapTabs() || newErrorsFound;
        newErrorsFound = ValidateApplicationMapTabs() || newErrorsFound;
      }
    }
    while (newErrorsFound);

    ValidateMarkupCategories();
    ValidatePrintTemplates();
    ValidateZoneLevels();
  }

  private bool ValidateApplications()
  {
    bool newErrorsFound = false;

    // each application

    foreach (Configuration.ApplicationRow application in Application.Where(o => o.IsValidationErrorNull()))
    {
      // must contain as least one valid MapTab

      if (!application.GetApplicationMapTabRows().Any(o => o.IsValidationErrorNull()))
      {
        application.ValidationError = "Does not contain any valid map tabs";
      }

      // DefaultMapTab must be valid

      else if (!application.IsDefaultMapTabNull() && !application.GetApplicationMapTabRows().Any(o => o.IsValidationErrorNull() && o.MapTabRow.IsValidationErrorNull() && o.MapTabRow.MapTabID == application.DefaultMapTab))
      {
        application.ValidationError = "The default map tab is not a valid map tab in this application";
      }

      // DefaultAction must be valid if not null

      else if (!application.IsDefaultActionNull())
      {
        string[] validActions = EnumHelper.ToChoiceArray(typeof(Action));

        if (!validActions.Contains(application.DefaultAction.ToLower()))
        {
          application.ValidationError = String.Format("The default action must be {0} if set", EnumHelper.ToChoiceString(typeof(Action)));
        }
      }

      // if DefaultTargetLayer is not null

      if (application.IsValidationErrorNull() && !application.IsDefaultTargetLayerNull())
      {
        // DefaultMapTab must not be null

        if (application.IsDefaultMapTabNull())
        {
          application.ValidationError = "A default map tab must be provided when a default target layer is defined";
        }

        // and DefaultMapTab must contain DefaultTargetLayer

        else
        {
          Configuration.MapTabRow mapTab = MapTab.First(o => o.MapTabID == application.DefaultMapTab);
          Configuration.MapTabLayerRow link = mapTab.GetMapTabLayerRows().FirstOrDefault(o => o.IsValidationErrorNull() && o.LayerID == application.DefaultTargetLayer);

          if (link == null)
          {
            application.ValidationError = "The default target layer is not a valid layer in the default map tab";
          }
          else if (link.IsAllowTargetNull() || link.AllowTarget <= 0)
          {
            application.ValidationError = "The default target layer is not allowed as a target layer in the default map tab";
          }
        }
      }

      // if DefaultProximity is not null

      if (application.IsValidationErrorNull() && !application.IsDefaultProximityNull())
      {
        Configuration.ProximityRow proximity = Proximity.FirstOrDefault(o => o.ProximityID == application.DefaultProximity);

        // DefaultProximity must exist

        if (proximity == null)
        {
          application.ValidationError = "The default proximity does not exist";
        }

        // DefaultProximity must be valid

        else if (!proximity.IsValidationErrorNull())
        {
          application.ValidationError = "The default proximity is not a valid proximity";
        }

        // DefaultTargetLayer must not be null

        else if (application.IsDefaultTargetLayerNull())
        {
          application.ValidationError = "A default target layer must be provided when a default proximity is defined";
        }

        // DefaultProximity must be linked to the DefaultTargetLayer

        else if (!proximity.IsIsDefaultNull() && proximity.IsDefault == 0 && !proximity.GetLayerProximityRows().Any(o => o.IsValidationErrorNull() && o.LayerID == application.DefaultTargetLayer))
        {
          application.ValidationError = "The default proximity is not linked to the default target layer";
        }
      }

      // if DefaultSelectionLayer is not null

      if (application.IsValidationErrorNull() && !application.IsDefaultSelectionLayerNull())
      {
        // DefaultMapTab must not be null

        if (application.IsDefaultMapTabNull())
        {
          application.ValidationError = "A default map tab must be provided when a default selection layer is provided";
        }

        // and DefaultMapTab must contain DefaultSelectionLayer

        else
        {
          Configuration.MapTabRow mapTab = MapTab.First(o => o.MapTabID == application.DefaultMapTab);
          Configuration.MapTabLayerRow link = mapTab.GetMapTabLayerRows().FirstOrDefault(o => o.IsValidationErrorNull() && o.LayerID == application.DefaultSelectionLayer);

          if (link == null)
          {
            application.ValidationError = "The default selection layer is not a valid layer in the default map tab";
          }
          else if (link.IsAllowSelectionNull() || link.AllowSelection <= 0)
          {
            application.ValidationError = "The default selection layer is not allowed as a selection layer in the default map tab";
          }
        }
      }

      if (application.IsValidationErrorNull() && !application.IsFunctionTabsNull())
      {
        string[] tabs = application.FunctionTabs.ToLower().Split(',');
        string[] validFunctionTabs = EnumHelper.ToChoiceArray(typeof(FunctionTab));

        foreach (string tab in tabs)
        {
          if (!validFunctionTabs.Contains(tab) || ((tab == "all" || tab == "none") && tabs.Length > 1))
          {
            application.ValidationError = "The function tabs must be 'all', 'none', or any combination of 'selection', 'search', 'legend', 'location', 'markup' and 'share' separated by commas if set";
            break;
          }
        }

        if (application.IsValidationErrorNull() && tabs.Contains("location") && application.IsOverviewMapIDNull())
        {
          application.ValidationError = "An overview map ID must be provided when function tabs are 'all' or 'location'";
        }
      }

      if (application.IsValidationErrorNull())
      {
        if (!application.IsFullExtentNull() && EnvelopeExtensions.FromDelimitedString(application.FullExtent) == null)
        {
          application.ValidationError = "Invalid full extent specified, must be four comma-separated numbers";
        }

        else if (!application.IsOverviewMapIDNull() && !MapTab.Any(o => o.MapTabID == application.OverviewMapID))
        {
          application.ValidationError = "The overview map ID does not point to a valid map tab";
        }
      }

      if (application.IsValidationErrorNull() && !application.IsCoordinateModesNull())
      {
        string[] modes = application.CoordinateModes.ToLower().Split(',');
        string[] validCoordinateModes = EnumHelper.ToChoiceArray(typeof(CoordinateMode));

        foreach (string mode in modes)
        {
          if (!validCoordinateModes.Contains(mode))
          {
            application.ValidationError = "The coordinate mode must be any combination of " + EnumHelper.ToChoiceString(typeof(CoordinateMode)) + " separated by commas if set";
            break;
          }
        }
      }

      // zone/level specification must be valid if present

      if (application.IsValidationErrorNull())
      {
        if (application.IsZoneLevelIDNull())
        {
          if (!application.IsDefaultLevelNull())
          {
            application.ValidationError = "ZoneLevelID must be set when DefaultLevel is set";
          }
        }
        else
        {
          if (!application.ZoneLevelRow.IsValidationErrorNull())
          {
            application.ValidationError = "Does not link to a valid zone/level specification";
          }
          else
          {
            if (!application.IsDefaultLevelNull() && !application.ZoneLevelRow.GetLevelRows().Any(o => o.LevelID == application.DefaultLevel))
            {
              application.ValidationError = "The default level does not exist in the zone/level specification";
            }
          }
        }
      }

      newErrorsFound = newErrorsFound || !application.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateApplicationMapTabs()
  {
    bool newErrorsFound = false;

    // each ApplicationMapTab must link to a valid application and valid map tab

    foreach (Configuration.ApplicationMapTabRow link in ApplicationMapTab.Where(o => o.IsValidationErrorNull()))
    {
      if (!link.ApplicationRow.IsValidationErrorNull())
      {
        link.ValidationError = "Does not link to a valid application";
      }

      else if (!link.MapTabRow.IsValidationErrorNull())
      {
        link.ValidationError = "Does not link to a valid map tab";
      }

      newErrorsFound = newErrorsFound || !link.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private void ValidateDataSources()
  {
    // check connections

    foreach (Configuration.ConnectionRow connectionRow in Connection.Where(o => o.IsValidationErrorNull()))
    {
      OleDbConnection connection = connectionRow.GetDatabaseConnection();

      if (connection != null)
      {
        connection.Close();
      }
      else
      {
        connectionRow.ValidationError = "Could not connect to database";
      }
    }

    // check stored procedures

    // === LayerFunction ===

    foreach (Configuration.LayerFunctionRow layerFunction in LayerFunction.Where(o => o.IsValidationErrorNull()))
    {
      if (!layerFunction.IsConnectionIDNull() && !layerFunction.ConnectionRow.IsValidationErrorNull())
      {
        layerFunction.ValidationError = "Does not use a valid database connection";
      }
      else
      {
        using (OleDbCommand command = layerFunction.GetDatabaseCommand())
        {
          if (command != null)
          {
            command.Connection.Dispose();
          }
          else
          {
            layerFunction.ValidationError = "Stored procedure does not exist or is inaccessible";
          }
        }
      }
    }

    // === DataTab ===

    foreach (Configuration.DataTabRow dataTab in DataTab.Where(o => o.IsValidationErrorNull()))
    {
      if (!dataTab.IsConnectionIDNull() && !dataTab.ConnectionRow.IsValidationErrorNull())
      {
        dataTab.ValidationError = "Does not use a valid database connection";
      }
      else
      {
        using (OleDbCommand command = dataTab.GetDatabaseCommand())
        {
          if (command != null)
          {
            command.Connection.Dispose();
          }
          else
          {
            dataTab.ValidationError = "Stored procedure does not exist or is inaccessible";
          }
        }
      }
    }

    // === Query ===

    foreach (Configuration.QueryRow query in Query.Where(o => o.IsValidationErrorNull()))
    {
      if (!query.IsConnectionIDNull() && !query.ConnectionRow.IsValidationErrorNull())
      {
        query.ValidationError = "Does not use a valid database connection";
      }
      else
      {
        using (OleDbCommand command = query.GetDatabaseCommand())
        {
          if (command == null)
          {
            query.ValidationError = "Stored procedure does not exist or is inaccessible";
          }
          else
          {
            OleDbDataReader reader = null;
            bool hasMapId = false;

            try
            {
              using (reader = command.ExecuteReader())
              {
                hasMapId = reader.GetOrdinal("MapID") >= 0;
              }
            }
            catch { }
              
            command.Connection.Dispose();

            if (!hasMapId)
            {
              query.ValidationError = "Stored procedure does not return a MapID column";
            }
          }
        }
      }
    }

    // === Search ===

    foreach (Configuration.SearchRow search in Search.Where(o => o.IsValidationErrorNull()))
    {
      if (!search.IsConnectionIDNull() && !search.ConnectionRow.IsValidationErrorNull())
      {
        search.ValidationError = "Does not use a valid database connection";
      }
      else
      {
        using (OleDbCommand command = search.GetDatabaseCommand())
        {
          if (command == null)
          {
            search.ValidationError = "Stored procedure does not exist or is inaccessible";
          }
        }

        if (search.IsValidationErrorNull())
        {
          using (OleDbCommand command = search.GetSelectCommand())
          {
            if (!_searchSubstitutionRegex.IsMatch(command.CommandText))
            {
              search.ValidationError = "Select statement does not contain a where clause substitution: {0}";
            }
            else
            {
              command.CommandText = String.Format(command.CommandText, "1 = 0");

              OleDbDataReader reader = null;
              bool hasMapId = false;

              try
              {
                using (reader = command.ExecuteReader())
                {
                  hasMapId = reader.GetOrdinal("MapID") >= 0;
                }
              }
              catch { }

              command.Connection.Dispose();

              if (!hasMapId)
              {
                search.ValidationError = "Select statement does not return a MapID column";
              }
            }
          }
        }
      }
    }

    // === SearchInputField ===

    foreach (Configuration.SearchInputFieldRow searchInputField in SearchInputField.Where(o => !o.IsStoredProcNull() && o.IsValidationErrorNull()))
    {
      if (!searchInputField.IsConnectionIDNull() && !searchInputField.ConnectionRow.IsValidationErrorNull())
      {
        searchInputField.ValidationError = "Does not use a valid database connection";
      }
      else
      {
        using (OleDbCommand command = searchInputField.GetDatabaseCommand())
        {
          if (command != null)
          {
            command.Connection.Dispose();
          }
          else
          {
            searchInputField.ValidationError = "Stored procedure does not exist or is inaccessible";
          }
        }
      }
    }
  }

  private bool ValidateDataTabs()
  {
    bool newErrorsFound = false;

    // each data tab

    foreach (Configuration.DataTabRow dataTab in DataTab.Where(o => o.IsValidationErrorNull()))
    {
      // must link to a valid layer

      if (!dataTab.LayerRow.IsValidationErrorNull())
      {
        dataTab.ValidationError = "Is not contained in a valid layer";
      }

      // which has at least one valid query

      else if (!dataTab.LayerRow.GetQueryRows().Any(o => o.IsValidationErrorNull()))
      {
        dataTab.ValidationError = "Layer does not contain any valid queries";
      }

      newErrorsFound = newErrorsFound || !dataTab.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateLayers()
  {
    bool newErrorsFound = false;

    // each layer must be contained in at least one valid map tab

    foreach (Configuration.LayerRow layer in Layer.Where(o => o.IsValidationErrorNull()))
    {
      if (!layer.GetMapTabLayerRows().Any(o => o.IsValidationErrorNull()) &&
          (layer.IsBaseMapIDNull() || !MapTab.Any(o => o.IsValidationErrorNull() && !o.IsBaseMapIDNull() && o.BaseMapID == layer.BaseMapID)))
      {
        layer.ValidationError = "Is not contained in any valid map tab";
      }

      newErrorsFound = newErrorsFound || !layer.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateLayerFunctions()
  {
    string[] validLayerFunctions = new string[] { "maptip", "identify", "mailinglabel", "export", "targetparams" };
    bool newErrorsFound = false;

    // each LayerFunction

    foreach (Configuration.LayerFunctionRow layerFunction in LayerFunction.Where(o => o.IsValidationErrorNull()))
    {
      string functionName = layerFunction.FunctionName.ToLower();

      // must have a valid function type

      if (!validLayerFunctions.Contains(functionName))
      {
        layerFunction.ValidationError = "Invalid function, must be one of 'maptip', 'identify', 'mailinglabel', 'export' or 'targetparams'";
      }

      // must link to a valid layer

      else if (!layerFunction.LayerRow.IsValidationErrorNull())
      {
        layerFunction.ValidationError = "Is not contained in a valid layer";
      }

      // which has a key field specified

      else if (layerFunction.LayerRow.IsKeyFieldNull())
      {
        layerFunction.ValidationError = "A key field must be provided by the layer";
      }

      newErrorsFound = newErrorsFound || !layerFunction.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateLayerProximities()
  {
    bool newErrorsFound = false;

    // each LayerProximity must point to a valid layer and a valid proximity

    foreach (Configuration.LayerProximityRow link in LayerProximity.Where(o => o.IsValidationErrorNull()))
    {
      if (!link.LayerRow.IsValidationErrorNull())
      {
        link.ValidationError = "Does not link to a valid layer";
      }

      else if (!link.ProximityRow.IsValidationErrorNull())
      {
        link.ValidationError = "Does not link to a valid proximity";
      }

      newErrorsFound = newErrorsFound || !link.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private void ValidateMapServices()
  {
    Dictionary<String, CommonHost> mapHosts = new Dictionary<String, CommonHost>();
    Dictionary<String, CommonMapService> mapServices = new Dictionary<String, CommonMapService>();
    Dictionary<String, CommonDataFrame> mapDataFrames = new Dictionary<String, CommonDataFrame>();

    // === MapTab ===

    // load the map hosts

    foreach (Configuration.MapTabRow mapTab in MapTab)
    {
      string userName = mapTab.IsUserNameNull() ? null : mapTab.UserName;
      string password = mapTab.IsPasswordNull() ? null : mapTab.Password;

      string agsHostKey = mapTab.GetHostKey("AGS");

      if (!mapHosts.ContainsKey(agsHostKey))
      {
        AgsHost agsHost = null;

        try
        {
          agsHost = new AgsHost(mapTab.MapHost, userName, password);
        }
        catch { }

        mapHosts.Add(agsHostKey, agsHost);
      }

      string arcImsHostKey = mapTab.GetHostKey("ArcIMS");

      if (!mapHosts.ContainsKey(arcImsHostKey))
      {
        ArcImsHost arcImsHost = null;

        try
        {
          arcImsHost = new ArcImsHost(mapTab.MapHost, userName, password);
        }
        catch { }

        mapHosts.Add(arcImsHostKey, arcImsHost);
      }

      if (mapHosts[agsHostKey] == null && mapHosts[arcImsHostKey] == null)
      {
        mapTab.ValidationError = "Could not connect to the server";
      }
    }

    // load the map services

    foreach (Configuration.MapTabRow mapTab in MapTab.Where(o => o.IsValidationErrorNull()))
    {
      string agsServiceKey = mapTab.GetServiceKey("AGS");

      if (!mapServices.ContainsKey(agsServiceKey))
      {
        AgsMapService agsService = null;
        string agsHostKey = mapTab.GetHostKey("AGS");

        if (mapHosts[agsHostKey] != null)
        {
          try
          {
            agsService = mapHosts[agsHostKey].GetMapService(mapTab.MapService) as AgsMapService;
          }
          catch { }
        }

        mapServices.Add(agsServiceKey, agsService);
      }

      string arcImsServiceKey = mapTab.GetServiceKey("ArcIMS");

      if (!mapServices.ContainsKey(arcImsServiceKey))
      {
        ArcImsService arcImsService = null;
        string arcImsHostKey = mapTab.GetHostKey("ArcIMS");

        if (mapHosts[arcImsHostKey] != null)
        {
          try
          {
            arcImsService = mapHosts[arcImsHostKey].GetMapService(mapTab.MapService) as ArcImsService;
          }
          catch { }
        }

        mapServices.Add(arcImsServiceKey, arcImsService);
      }

      if (mapServices[agsServiceKey] == null && mapServices[arcImsServiceKey] == null)
      {
        mapTab.ValidationError = "Could not find the specified map service on the server";
      }
    }

    // load the dataframes

    foreach (Configuration.MapTabRow mapTab in MapTab.Where(o => o.IsValidationErrorNull()))
    {
      string dataFrameKey = mapTab.GetDataFrameKey();

      if (!mapDataFrames.ContainsKey(dataFrameKey))
      {
        CommonDataFrame dataFrame = null;

        foreach (string type in new string[] { "AGS", "ArcIMS" })
        {
          CommonMapService mapService = mapServices[mapTab.GetServiceKey(type)];

          if (dataFrame == null && mapService != null)
          {
            dataFrame = mapTab.IsDataFrameNull() ? mapService.DefaultDataFrame : mapService.DataFrames.FirstOrDefault(o => String.Compare(o.Name, mapTab.DataFrame, true) == 0);
          }
        }

        mapDataFrames.Add(dataFrameKey, dataFrame);
      }

      if (mapDataFrames[dataFrameKey] == null)
      {
        mapTab.ValidationError = "Could not find the specified data frame in the map service";
      }
    }

    // === Layer ===

    foreach (Configuration.LayerRow layer in Layer)
    {
      // for each valid MapTab linked through MapTabLayer

      foreach (Configuration.MapTabLayerRow link in layer.GetMapTabLayerRows().Where(o => o.MapTabRow.IsValidationErrorNull()))
      {
        // a single layer with the specified name must be present in the MapTab dataframe

        CommonLayer[] commonLayers = mapDataFrames[link.MapTabRow.GetDataFrameKey()].Layers.Where(o => String.Compare(o.Name, layer.LayerName, true) == 0).ToArray();

        if (commonLayers.Length == 0)
        {
          link.ValidationError = String.Format("'{0}' is not a layer in the service/dataframe for map tab '{1}'", layer.LayerName, link.MapTabID);
        }
        else if (commonLayers.Length > 1)
        {
          link.ValidationError = String.Format("More than one layer named '{0}' in the service/dataframe for map tab '{1}'", layer.LayerName, link.MapTabID);
        }
        else
        {
          CommonLayer commonLayer = commonLayers[0];

          if (commonLayer.Type == CommonLayerType.Feature)
          {
            bool needsGeometryField = false;

            // for selectable layers

            if (!layer.IsKeyFieldNull())
            {
              // KeyField must exist in the layer 

              if (commonLayer.Fields == null || commonLayer.FindField(layer.KeyField) == null)
              {
                link.ValidationError = String.Format("Layer '{0}' in the service/dataframe for map tab '{1}' does not contain the key field '{2}'", layer.LayerName, link.MapTabID, layer.KeyField);
              }

              needsGeometryField = true;
            }

            // for zones and levels, the specified fields must be available

            if (!layer.IsZoneFieldNull())
            {
              if (commonLayer.Fields == null || commonLayer.FindField(layer.ZoneField) == null)
              {
                link.ValidationError = String.Format("Layer '{0}' in the service/dataframe for map tab '{1}' does not contain the zone field '{2}'", layer.LayerName, link.MapTabID, layer.ZoneField);
              }

              needsGeometryField = true;
            }

            if (!layer.IsLevelFieldNull() && (commonLayer.Fields == null || commonLayer.FindField(layer.LevelField) == null))
            {
              link.ValidationError = String.Format("Layer '{0}' in the service/dataframe for map tab '{1}' does not contain the level field '{2}'", layer.LayerName, link.MapTabID, layer.LevelField);
            }

            if (needsGeometryField && commonLayer.GeometryField == null)
            {
              link.ValidationError = String.Format("Layer '{0}' in the service/dataframe for map tab '{1}' does not provide a shape field (may be set to hidden in the MXD)", layer.LayerName, link.MapTabID);
            }
          }

          if (commonLayer.Type == CommonLayerType.Image)
          {
            if (!layer.IsKeyFieldNull() && commonLayer is ArcImsLayer)
            {
              link.ValidationError = String.Format("KeyField cannot be set for raster layers in ArcIMS", layer.LayerName, link.MapTabID);
            }
            else
            {
              string[] invalidFunctions = new string[] { "mailinglabel", "export", "targetparams" };

              foreach (Configuration.LayerFunctionRow layerFunction in LayerFunction.Where(o => o.LayerID == link.LayerID && o.IsValidationErrorNull() && invalidFunctions.Contains(o.FunctionName.ToLower())))
              {
                layerFunction.ValidationError = String.Format("Function '{0}' cannot be applied to a raster layer", layerFunction.FunctionName);
              }
            }
          }
        }
      }

      // for each valid MapTab linked via BaseMapID a single layer with the specified name must be present in the MapTab dataframe

      if (!layer.IsBaseMapIDNull())
      {
        foreach (Configuration.MapTabRow mapTab in MapTab.Where(o => o.IsValidationErrorNull() && !o.IsBaseMapIDNull() && o.BaseMapID == layer.BaseMapID))
        {
          int n = mapDataFrames[mapTab.GetDataFrameKey()].Layers.Count<CommonLayer>(o => String.Compare(o.Name, layer.LayerName, true) == 0);

          if (n == 0)
          {
            layer.ValidationError = String.Format("'{0}' is not a layer in the service/dataframe for map tab '{1}'", layer.LayerName, mapTab.MapTabID);
          }
          else if (n > 1)
          {
            layer.ValidationError = String.Format("More than one layer named '{0}' in the service/dataframe for map tab '{1}'", layer.LayerName, mapTab.MapTabID);
          }
        }
      }
    }
  }

  private bool ValidateMapTabs()
  {
    bool newErrorsFound = false;

    // each map tab

    foreach (Configuration.MapTabRow mapTab in MapTab.Where(o => o.IsValidationErrorNull()))
    {
      // must contain as least one valid layer

      if (!mapTab.GetMapTabLayerRows().Any(o => o.IsValidationErrorNull()) &&
          (mapTab.IsBaseMapIDNull() || !Layer.Any(o => o.IsValidationErrorNull() && !o.IsBaseMapIDNull() && o.BaseMapID == mapTab.BaseMapID)))
      {
        mapTab.ValidationError = "Does not contain any valid layers";
      }

      // must be contained in a valid application

      else if (!mapTab.GetApplicationMapTabRows().Any(o => o.IsValidationErrorNull()) && !Application.Any(o => o.IsValidationErrorNull() && !o.IsOverviewMapIDNull() && o.OverviewMapID == mapTab.MapTabID))
      {
        mapTab.ValidationError = "Is not contained in any valid application";
      }

      newErrorsFound = newErrorsFound || !mapTab.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateMapTabLayers()
  {
    bool newErrorsFound = false;

    // each MapTabLayer

    foreach (Configuration.MapTabLayerRow link in MapTabLayer.Where(o => o.IsValidationErrorNull()))
    {
      // must point to a valid MapTab and Layer

      if (!link.MapTabRow.IsValidationErrorNull())
      {
        link.ValidationError = "Does not link to a valid map tab";
      }

      else if (!link.LayerRow.IsValidationErrorNull())
      {
        link.ValidationError = "Does not link to a valid layer";
      }

      // if AllowTarget is true

      if (link.IsValidationErrorNull() && !link.IsAllowTargetNull() && link.AllowTarget > 0)
      {
        // KeyField must be set in the Layer

        if (link.LayerRow.IsKeyFieldNull())
        {
          link.ValidationError = "The layer must have a key field defined when it's allowed to be a target layer";
        }

        // the Layer must have at least one Query

        else if (link.LayerRow.GetQueryRows().Length == 0)
        {
          link.ValidationError = "The layer must have at least one query when it's allowed to be a target layer";
        }

        // the Layer must have at least one DataTab

        else if (link.LayerRow.GetDataTabRows().Length == 0)
        {
          link.ValidationError = "The layer must have at least one data tab when it's allowed to be a target layer";
        }
      }

      // if AllowSelection is true, KeyField must be set in the Layer

      if (link.IsValidationErrorNull() && !link.IsAllowSelectionNull() && link.AllowSelection > 0 && link.LayerRow.IsKeyFieldNull())
      {
        link.ValidationError = "The layer must have a key field defined when it's allowed to be a selection layer";
      }

      newErrorsFound = newErrorsFound || !link.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private void ValidateMarkupCategories()
  {
    // each ApplicationMarkupCategory must point to a valid application

    foreach (Configuration.ApplicationMarkupCategoryRow link in ApplicationMarkupCategory.Where(o => !o.ApplicationRow.IsValidationErrorNull()))
    {
      link.ValidationError = "Does not link to a valid application";
    }

    // each markup category must point to a valid ApplicationMarkupCategory

    foreach (Configuration.MarkupCategoryRow category in MarkupCategory.Where(o => !o.GetApplicationMarkupCategoryRows().Any(o2 => o2.IsValidationErrorNull())))
    {
      category.ValidationError = "Is not contained in any valid application";
    }
  }

  private void ValidatePrintTemplates()
  {
    //string[] validContentTypes = new string[] { "box", "date", "image", "input", "legend", "map", "overviewmap", "scale", "scalefeet", "tabdata", "text" };
    string[] validContentTypes = new string[] { "box", "date", "image", "input", "legend", "map", "overviewmap", "scale", "scalefeet", "text" };

    // each PrintTemplateContent 

    foreach (Configuration.PrintTemplateContentRow printTemplateContent in PrintTemplateContent)
    {
      // must have a valid content type

      if (!validContentTypes.Contains(printTemplateContent.ContentType.ToLower()))
      {
        printTemplateContent.ValidationError = "Unknown content type: " + printTemplateContent.ContentType;
      }

      else
      {
        switch (printTemplateContent.ContentType)
        {
          // if type "input", must have a display name

          case "input":
            if (printTemplateContent.IsDisplayNameNull())
            {
              printTemplateContent.ValidationError = "No display name specified for content type input";
            }
            break;

          // if type "image", file name must point to a valid file

          case "image":
            if (printTemplateContent.IsFileNameNull())
            {
              printTemplateContent.ValidationError = "No file name specified for image";
            }
            else
            {
              string imageDir = HttpContext.Current.Server.MapPath("~/Images/Print");

              if (!File.Exists(imageDir + "\\" + printTemplateContent.FileName))
              {
                printTemplateContent.ValidationError = "Could not find image file in the web site Images/Print directory";
              }
            }
            break;

          // if type "text", text must be provided or file name must point to a valid file

          case "text":
            if (printTemplateContent.IsTextNull())
            {
              if (printTemplateContent.IsFileNameNull())
              {
                printTemplateContent.ValidationError = "No text specified for content type text";
              }
              else
              {
                string textDir = HttpContext.Current.Server.MapPath("~/Text/Print");

                if (!File.Exists(textDir + "\\" + printTemplateContent.FileName))
                {
                  printTemplateContent.ValidationError = "Could not find text file in the web site Text/Print directory";
                }
              }
            }
            break;
        }
      }
    }

    // each ApplicationPrintTemplate must point to a valid Application

    foreach (Configuration.ApplicationPrintTemplateRow link in ApplicationPrintTemplate)
    {
      if (!link.ApplicationRow.IsValidationErrorNull())
      {
        link.ValidationError = "Does not link to a valid application";
      }
    }

    // each PrintTemplate

    foreach (Configuration.PrintTemplateRow printTemplate in PrintTemplate)
    {
      // if not always available, must be contained in at least one valid application

      if (!printTemplate.IsAlwaysAvailableNull() && printTemplate.AlwaysAvailable == 0 && !printTemplate.GetApplicationPrintTemplateRows().Any(o => o.IsValidationErrorNull()))
      {
        printTemplate.ValidationError = "Is not contained in any valid application";
      }

      // must contain at least one valid content item

      else if (!printTemplate.GetPrintTemplateContentRows().Any(o => o.IsValidationErrorNull()))
      {
        printTemplate.ValidationError = "Does not contain any valid content";
      }
    }
  }

  private bool ValidateProximities()
  {
    bool newErrorsFound = false;

    // each proximity

    foreach (Configuration.ProximityRow proximity in Proximity.Where(o => o.IsValidationErrorNull()))
    {
      // must have a distance greater than or equals to zero

      if (proximity.Distance < 0)
      {
        proximity.ValidationError = "Distance must be greater than or equal to zero";
      }

      // if not default, must be contained in a valid layer

      else if (!proximity.IsIsDefaultNull() && proximity.IsDefault == 0 && !proximity.GetLayerProximityRows().Any(o => o.IsValidationErrorNull()))
      {
        proximity.ValidationError = "Is not default and is not contained in a valid layer";
      }

      newErrorsFound = newErrorsFound || !proximity.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateQueries()
  {
    bool newErrorsFound = false;

    // each query

    foreach (Configuration.QueryRow query in Query.Where(o => o.IsValidationErrorNull()))
    {
      // must point to a valid layer

      if (!query.LayerRow.IsValidationErrorNull())
      {
        query.ValidationError = "Is not contained in a valid layer";
      }

      // which has a key field

      else if (query.LayerRow.IsKeyFieldNull())
      {
        query.ValidationError = "A key field must be provided by the layer";
      }

      // and at least one valid data tab

      else if (!query.LayerRow.GetDataTabRows().Any(o => o.IsValidationErrorNull()))
      {
        query.ValidationError = "Layer does not contain any valid data tabs";
      }

      newErrorsFound = newErrorsFound || !query.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateSearchInputField()
  {
    string[] validTypes = new string[] { "autocomplete", "date", "daterange", "list", "number", "numberrange", "text", "textcontains", "textstarts" };
    string[] procedureTypes = new string[] { "autocomplete", "list" };

    bool newErrorsFound = false;

    // each search criteria

    foreach (Configuration.SearchInputFieldRow searchInputField in SearchInputField.Where(o => o.IsValidationErrorNull()))
    {
      // must have a valid type

      if (!validTypes.Contains(searchInputField.FieldType.ToLower()))
      {
        searchInputField.ValidationError = "Unknown content type: " + searchInputField.FieldType;
      }

      // must have a stored procedure when type is "autocomplete" or "list"

      if (procedureTypes.Contains(searchInputField.FieldType.ToLower()) && searchInputField.IsStoredProcNull())
      {
        searchInputField.ValidationError = "No store procedure specified for criteria type " + searchInputField.FieldType;
      }

      newErrorsFound = newErrorsFound || !searchInputField.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private bool ValidateSearches()
  {
    bool newErrorsFound = false;

    // each search

    foreach (Configuration.SearchRow search in Search.Where(o => o.IsValidationErrorNull()))
    {
      // must point to a valid layer

      if (!search.LayerRow.IsValidationErrorNull())
      {
        search.ValidationError = "Is not contained in a valid layer";
      }

      // which has at least one valid query

      else if (!search.LayerRow.GetQueryRows().Any(o => o.IsValidationErrorNull()))
      {
        search.ValidationError = "Layer does not contain any valid queries";
      }

      // must contain at least one valid search criteria

      if (!search.GetSearchInputFieldRows().Any(o => o.IsValidationErrorNull()))
      {
        search.ValidationError = "Does not contain any valid search criteria";
      }

      newErrorsFound = newErrorsFound || !search.IsValidationErrorNull();
    }

    return newErrorsFound;
  }

  private void ValidateZoneLevels()
  {
    // each zone/level spec

    foreach (Configuration.ZoneLevelRow zoneLevel in ZoneLevel.Where(o => o.IsValidationErrorNull()))
    {
      // must be referenced by at least one valid application

      if (!zoneLevel.GetApplicationRows().Any(o => o.IsValidationErrorNull()))
      {
        zoneLevel.ValidationError = "Not referenced by any valid application";

        foreach (Configuration.ZoneRow zone in zoneLevel.GetZoneRows())
        {
          zone.ValidationError = "Is not contained in a valid zone/level specification";
        }

        foreach (Configuration.LevelRow level in zoneLevel.GetLevelRows())
        {
          level.ValidationError = "Is not contained in a valid zone/level specification";
        }
      }

      // must contain zones or levels

      else if (zoneLevel.GetZoneRows().Length == 0 && zoneLevel.GetLevelRows().Length == 0)
      {
        zoneLevel.ValidationError = "No zones or levels provided in this zone/level specification";
      }
    }

    // each zone/level combo must point to a valid zone

    foreach (Configuration.ZoneLevelComboRow zoneLevelCombo in ZoneLevelCombo.Where(o => o.IsValidationErrorNull() && !o.ZoneRowParent.IsValidationErrorNull()))
    {
      zoneLevelCombo.ValidationError = "Is not contained in a valid zone/level specification";
    }
  }
}
