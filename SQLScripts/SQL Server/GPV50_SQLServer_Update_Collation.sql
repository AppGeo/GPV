/*
  Copyright 2016 Applied Geographics, Inc.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.

  GPV50_SQLServer_Update_Collation.sql

  Creates the GPV v5.0 configuration tables.  You can set the prefix for the table names by changing 
  the value in the "set @prefix" line below.  Make sure to run the follow scripts after this
  one using the same prefix:

    GPV50_SQLServer_AddConstraints.sql - to create the necessary constraints
    GPV50_SQLServer_LoadMailingLabels.sql - to load the mailing labels table

*/

declare @prefix nvarchar(50)
set @prefix = 'GPV50'

declare @sql nvarchar(2000)

set @sql = 'ALTER TABLE ' + @prefix + 'Application 
  ALTER COLUMN ApplicationID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Application 
  ALTER COLUMN ZoneLevelID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ApplicationMapTab 
  ALTER COLUMN ApplicationID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ApplicationMapTab 
  ALTER COLUMN MapTabID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ApplicationMarkupCategory 
  ALTER COLUMN ApplicationID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ApplicationMarkupCategory 
  ALTER COLUMN CategoryID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ApplicationPrintTemplate 
  ALTER COLUMN ApplicationID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ApplicationPrintTemplate 
  ALTER COLUMN TemplateID nvarchar(50) COLLATE Latin1_General_CI_AS  NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Connection 
  ALTER COLUMN ConnectionID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'DataTab 
  ALTER COLUMN DataTabID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'DataTab 
  ALTER COLUMN LayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'DataTab 
  ALTER COLUMN ConnectionID nvarchar(50) COLLATE Latin1_General_CI_AS'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Layer 
  ALTER COLUMN LayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'LayerFunction 
  ALTER COLUMN LayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'LayerFunction 
  ALTER COLUMN FunctionName nvarchar(20) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'LayerFunction 
  ALTER COLUMN ConnectionID nvarchar(50) COLLATE Latin1_General_CI_AS'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'LayerProximity 
  ALTER COLUMN LayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'LayerProximity 
  ALTER COLUMN ProximityID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Level 
  ALTER COLUMN ZoneLevelID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Level 
  ALTER COLUMN LevelID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'MapTab 
  ALTER COLUMN MapTabID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'MapTabLayer 
  ALTER COLUMN MapTabID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'MapTabLayer 
  ALTER COLUMN LayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'MapTabTileGroup 
  ALTER COLUMN MapTabID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'MapTabTileGroup 
  ALTER COLUMN TileGroupID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'MarkupCategory 
  ALTER COLUMN CategoryID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'MarkupGroup 
  ALTER COLUMN CategoryID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'PrintTemplate 
  ALTER COLUMN TemplateID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'PrintTemplateContent 
  ALTER COLUMN TemplateID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Proximity 
  ALTER COLUMN ProximityID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Query 
  ALTER COLUMN QueryID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Query 
  ALTER COLUMN ConnectionID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Query 
  ALTER COLUMN LayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'SavedState 
  ALTER COLUMN StateID nvarchar(12) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Search 
  ALTER COLUMN SearchID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Search 
  ALTER COLUMN LayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Search 
  ALTER COLUMN ConnectionID nvarchar(50) COLLATE Latin1_General_CI_AS'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'SearchInputField 
  ALTER COLUMN FieldID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'SearchInputField 
  ALTER COLUMN SearchID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'SearchInputField 
  ALTER COLUMN ConnectionID nvarchar(50) COLLATE Latin1_General_CI_AS'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'TileGroup 
  ALTER COLUMN TileGroupID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'TileGroup 
  ALTER COLUMN DisplayName nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'TileLayer 
  ALTER COLUMN TileLayerID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'TileLayer 
  ALTER COLUMN TileGroupID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'UsageTracking 
  ALTER COLUMN ApplicationID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Zone 
  ALTER COLUMN ZoneLevelID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'Zone 
  ALTER COLUMN ZoneID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ZoneLevel 
  ALTER COLUMN ZoneLevelID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ZoneLevelCombo 
  ALTER COLUMN ZoneLevelID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ZoneLevelCombo 
  ALTER COLUMN ZoneID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'

exec(@sql)

set @sql = 'ALTER TABLE ' + @prefix + 'ZoneLevelCombo 
  ALTER COLUMN LevelID nvarchar(50) COLLATE Latin1_General_CI_AS NOT NULL'
exec(@sql)
