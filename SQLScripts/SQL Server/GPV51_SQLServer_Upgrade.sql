/*
  Copyright 2018 Applied Geographics, Inc.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.

  GPV51_SQLServer_Upgrade.sql

  Creates the GPV v5.1 configuration tables from an existing set of GPV v5.0 tables.
  Set the prefixes for both sets of table names by changing the values in the "set @prefix50" 
  and "set @prefix51" lines below.  Make sure to run GPV51_SQLServer_AddConstraints.sql 
  using the v5.1 prefix to create the necessary constraints on the v5.1 tables.

*/

declare @prefix50 nvarchar(50)
declare @prefix51 nvarchar(50)

set @prefix50 = 'GPV'
set @prefix51 = 'GPV51'

declare @sql nvarchar(2000)

/* copy tables */

set @sql = 'SELECT * INTO ' + @prefix51 + 'Application FROM ' + @prefix50 + 'Application'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'ApplicationMapTab FROM ' + @prefix50 + 'ApplicationMapTab'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'ApplicationMarkupCategory FROM ' + @prefix50 + 'ApplicationMarkupCategory'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'ApplicationPrintTemplate FROM ' + @prefix50 + 'ApplicationPrintTemplate'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Connection FROM ' + @prefix50 + 'Connection'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'DataTab FROM ' + @prefix50 + 'DataTab'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'ExternalMap FROM ' + @prefix50 + 'ExternalMap'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Layer FROM ' + @prefix50 + 'Layer'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'LayerFunction FROM ' + @prefix50 + 'LayerFunction'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'LayerProximity FROM ' + @prefix50 + 'LayerProximity'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Level FROM ' + @prefix50 + 'Level'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'MailingLabel FROM ' + @prefix50 + 'MailingLabel'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'MapTab FROM ' + @prefix50 + 'MapTab'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'MapTabLayer FROM ' + @prefix50 + 'MapTabLayer'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'MapTabTileGroup FROM ' + @prefix50 + 'MapTabTileGroup'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Markup FROM ' + @prefix50 + 'Markup'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'MarkupCategory FROM ' + @prefix50 + 'MarkupCategory'
exec(@sql)

/* add Details to MarkupGroup */

set @sql = 'CREATE TABLE ' + @prefix51 + 'MarkupGroup (
  GroupID int NOT NULL,
  CategoryID nvarchar(50) COLLATE Latin1_General_CS_AS NOT NULL,
  DisplayName nvarchar(100) NOT NULL,
  Details nvarchar(1000),
  CreatedBy nvarchar(50) NOT NULL,
  CreatedByUser nvarchar(200),
  Locked smallint NOT NULL,
  DateCreated datetime NOT NULL,
  DateLastAccessed datetime NOT NULL,
  Deleted smallint NOT NULL 
)'
exec(@sql)

set @sql = 'INSERT INTO ' + @prefix51 + 'MarkupGroup (GroupID, CategoryID, DisplayName, CreatedBy, CreatedByUser, Locked, DateCreated, DateLastAccessed, Deleted)
  SELECT GroupID, CategoryID, DisplayName, CreatedBy, CreatedByUser, Locked, DateCreated, DateLastAccessed, Deleted
  FROM ' + @prefix50 + 'MarkupGroup'
exec(@sql)

/* copy tables */

set @sql = 'SELECT * INTO ' + @prefix51 + 'MarkupSequence FROM ' + @prefix50 + 'MarkupSequence'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'PrintTemplate FROM ' + @prefix50 + 'PrintTemplate'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'PrintTemplateContent FROM ' + @prefix50 + 'PrintTemplateContent'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Proximity FROM ' + @prefix50 + 'Proximity'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Query FROM ' + @prefix50 + 'Query'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'SavedState FROM ' + @prefix50 + 'SavedState'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Search FROM ' + @prefix50 + 'Search'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'SearchInputField FROM ' + @prefix50 + 'SearchInputField'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Setting FROM ' + @prefix50 + 'Setting'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'TileGroup FROM ' + @prefix50 + 'TileGroup'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'TileLayer FROM ' + @prefix50 + 'TileLayer'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'UsageTracking FROM ' + @prefix50 + 'UsageTracking'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'User FROM ' + @prefix50 + 'User'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'Zone FROM ' + @prefix50 + 'Zone'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'ZoneLevel FROM ' + @prefix50 + 'ZoneLevel'
exec(@sql)

set @sql = 'SELECT * INTO ' + @prefix51 + 'ZoneLevelCombo FROM ' + @prefix50 + 'ZoneLevelCombo'
exec(@sql)
