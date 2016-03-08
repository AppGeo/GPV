--
--  Copyright 2014 Applied Geographics, Inc.
--
--  Licensed under the Apache License, Version 2.0 (the "License");
--  you may not use this file except in compliance with the License.
--  You may obtain a copy of the License at
--
--      http://www.apache.org/licenses/LICENSE-2.0
--
--  Unless required by applicable law or agreed to in writing, software
--  distributed under the License is distributed on an "AS IS" BASIS,
--  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
--  See the License for the specific language governing permissions and
--  limitations under the License.
--
--  GPV41_Oracle_Upgrade.sql
--
--  Creates the GPV v5.0 configuration tables from an existing set of GPV v4.1 tables.
--  Set the prefixes for both sets of table names by changing the values in the "prefix41 varchar2(10)" 
--  and "prefix50 varchar2(10)" lines below.  Make sure to run GPV50_Oracle_AddConstraints.sql 
--  using the v5.0 prefix to create the necessary constraints on the v5.0 tables.
--

DECLARE 
  prefix41 varchar2(10):= 'GPV41';
  prefix50 varchar2(10):= 'GPV50';

BEGIN 

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Application AS SELECT * FROM ' || prefix41 || 'Application';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ApplicationMapTab AS SELECT * FROM ' || prefix41 || 'ApplicationMapTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ApplicationMarkupCategory AS SELECT * FROM ' || prefix41 || 'ApplicationMarkupCategory';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ApplicationPrintTemplate AS SELECT * FROM ' || prefix41 || 'ApplicationPrintTemplate';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Connection AS SELECT * FROM ' || prefix41 || 'Connection';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'DataTab AS SELECT * FROM ' || prefix41 || 'DataTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ExternalMap AS SELECT * FROM ' || prefix41 || 'ExternalMap';

--delete BaseMapID from Layer

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Layer (' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'LayerName varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'MetaDataURL varchar2(200),' ||
  'KeyField varchar2(50),' ||
  'ZoneField varchar2(50),' ||
  'LevelField varchar2(50),' ||
  'MaxNumberSelected number(5),' ||
  'MaxSelectionArea number(11,3), ' ||
  'MinNearestDistance number(11,3),' ||
  'MaxNearestDistance number(11,3),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix50 || 'Layer (LayerID, LayerName, DisplayName, MetaDataURL, KeyField, ZoneField, LevelField, MaxNumberSelected, MaxSelectionArea, MinNearestDistance, MaxNearestDistance, Active) ' ||
  'SELECT LayerID, LayerName, DisplayName, MetaDataURL, KeyField, ZoneField, LevelField, MaxNumberSelected, MaxSelectionArea, MinNearestDistance, MaxNearestDistance, Active ' ||
  'FROM ' || prefix41 || 'MapTab';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'LayerFunciton AS SELECT * FROM ' || prefix41 || 'LayerFunction';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'LayerProximity AS SELECT * FROM ' || prefix41 || 'LayerProximity';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Level AS SELECT * FROM ' || prefix41 || 'Level';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MailingLabel AS SELECT * FROM ' || prefix41 || 'MailingLabel';

-- delete BaseMapID from MapTab

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MapTab (' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'MapHost varchar2(50) NOT NULL,' ||
  'MapService varchar2(50) NOT NULL,' ||
  'UserName varchar2(50),' ||
  'Password varchar2(50),' ||
  'DataFrame varchar2(50),' ||
  'InteractiveLegend number(1),' ||
  'ShowBaseMapInLegend number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix50 || 'MapTab (MapTabID, DisplayName, MapHost, MapService, UserName, Password, DataFrame, InteractiveLegend, ShowBaseMapInLegend, Active) ' ||
  'SELECT MapTabID, DisplayName, MapHost, MapService, UserName, Password, DataFrame, InteractiveLegend, ShowBaseMapInLegend, Active ' ||
  'FROM ' || prefix41 || 'MapTab';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MapTabLayer AS SELECT * FROM ' || prefix41 || 'MapTabLayer';

-- create MapTabTileLayer table

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MapTabTileLayer (' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'TileLayerID varchar2(50) NOT NULL,' ||
  'Opacity number(1) default 1,' ||
  'ShowInLegend number(1),' ||
  'CheckInLegend number(1),' ||
  'IsOverlay number(1) default 0,' ||
  'SequenceNo number(1) NOT NULL' ||
')';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Markup AS SELECT * FROM ' || prefix41 || 'Markup';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MarkupCategory AS SELECT * FROM ' || prefix41 || 'MarkupCategory';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MarkupGroup AS SELECT * FROM ' || prefix41 || 'MarkupGroup';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MarkupSequence AS SELECT * FROM ' || prefix41 || 'MarkupSequence';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'PrintTemplate AS SELECT * FROM ' || prefix41 || 'PrintTemplate';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'PrintTemplateContent AS SELECT * FROM ' || prefix41 || 'PrintTemplateContent';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Proximity AS SELECT * FROM ' || prefix41 || 'Proximity';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Query AS SELECT * FROM ' || prefix41 || 'Query';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'SavedState AS SELECT * FROM ' || prefix41 || 'SavedState';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Search AS SELECT * FROM ' || prefix41 || 'Search';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'SearchInputField AS SELECT * FROM ' || prefix41 || 'SearchInputField';

-- create TileLayer table

EXECUTE IMMEDIATE  'CREATE TABLE' || prefix50 || 'TileLayer (' ||
  'TileLayerID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'URL varchar2(400) NOT NULL,' ||
  'Opacity number(1) default 1,' ||
  'MetaDataURL varchar2(200),' ||
  'Attribution varchar2(400),' ||
  'ShowLegend number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'UsageTracking AS SELECT * FROM ' || prefix41 || 'UsageTracking';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'User AS SELECT * FROM ' || prefix41 || 'User';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Zone AS SELECT * FROM ' || prefix41 || 'Zone';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ZoneLevel AS SELECT * FROM ' || prefix41 || 'ZoneLevel';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ZoneLevelCombo AS SELECT * FROM ' || prefix41 || 'ZoneLevelCombo';

END;
/


