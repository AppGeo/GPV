--
--  Copyright 2016 Applied Geographics, Inc.
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
--  GPV50_Oracle_Upgrade.sql
--
--  Creates the GPV v5.0 configuration tables from an existing set of GPV v4.1 tables.
--  Set the prefixes for both sets of table names by changing the values in the "prefix41 varchar2(10)"
--  and "prefix50 varchar2(10)" lines below.  Make sure to run GPV50_Oracle_AddConstraints.sql
--  using the v5.0 prefix to create the necessary constraints on the v5.0 tables.
--
--  This script contains default values for the new GPVSetting table.  To transfer your previous 
--  Web.config settings into the new GPVSetting table:
--
--    1) Move the previous content of <appSettings> into the v5.0 Web.config file.
--         - or -
--       Copy the UpdateSettings.ashx file from v5.0 to your previous version.
--
--    2) Navigate to the UpdateSettings.ashx page in a browser.  This will download a SQL
--       script containing update statements for the GPVSetting table.
--
--    3) Make any needed corrections to the SQL script.  Make sure the table name prefix
--       is correct.
--
--    4) Run the SQL script in your database to update the GPVSetting table.
--

DECLARE
  prefix41 varchar2(10):= 'GPV41';
  prefix50 varchar2(10):= 'GPV50';

BEGIN

-- add DefaultFunctionTab, DefaultTool, MetaDescription and MetaKeywords to GPVConfiguration

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Application (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'AuthorizedRoles varchar2(200),' ||
  'FunctionTabs varchar2(50),' ||
  'DefaultFunctionTab varchar2(50),' ||
  'DefaultMapTab varchar2(50),' ||
  'DefaultSearch varchar2(50),' ||
  'DefaultAction varchar2(50),' ||
  'DefaultTargetLayer varchar2(50),' ||
  'DefaultProximity varchar2(50),' ||
  'DefaultSelectionLayer varchar2(50),' ||
  'DefaultLevel varchar2(50),' ||
  'DefaultTool varchar2(50),' ||
  'FullExtent varchar2(50),' ||
  'OverviewMapID varchar2(50), ' ||
  'CoordinateModes varchar2(50),' ||
  'ZoneLevelID varchar2(50),' ||
  'TrackUse number(1),' ||
  'MetaDescription varchar2(200),' ||
  'MetaKeywords varchar2(200),' ||
  'About varchar2(1000),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix50 || 'Application (ApplicationID, DisplayName, AuthorizedRoles, FunctionTabs, DefaultMapTab, DefaultAction, DefaultTargetLayer, DefaultProximity, DefaultSelectionLayer, DefaultLevel, FullExtent, OverviewMapID, CoordinateModes, ZoneLevelID, TrackUse, MetaDescription, MetaKeywords, About, Active) ' ||
  'SELECT ApplicationID, DisplayName, AuthorizedRoles, FunctionTabs, DefaultMapTab, DefaultAction, DefaultTargetLayer, DefaultProximity, DefaultSelectionLayer, DefaultLevel, FullExtent, OverviewMapID, CoordinateModes, ZoneLevelID, TrackUse, MetaDescription, MetaKeywords, About, Active ' ||
  'FROM ' || prefix41 || 'Application';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ApplicationMapTab AS SELECT * FROM ' || prefix41 || 'ApplicationMapTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ApplicationMarkupCategory AS SELECT * FROM ' || prefix41 || 'ApplicationMarkupCategory';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ApplicationPrintTemplate AS SELECT * FROM ' || prefix41 || 'ApplicationPrintTemplate';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Connection AS SELECT * FROM ' || prefix41 || 'Connection';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'DataTab AS SELECT * FROM ' || prefix41 || 'DataTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ExternalMap AS SELECT * FROM ' || prefix41 || 'ExternalMap';

-- delete BaseMapID from Layer

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
  'FROM ' || prefix41 || 'Layer';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'LayerFunction AS SELECT * FROM ' || prefix41 || 'LayerFunction';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'LayerProximity AS SELECT * FROM ' || prefix41 || 'LayerProximity';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Level AS SELECT * FROM ' || prefix41 || 'Level';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MailingLabel AS SELECT * FROM ' || prefix41 || 'MailingLabel';

-- delete BaseMapID and ShowBaseMapInLegend from MapTab

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MapTab (' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'MapHost varchar2(50) NOT NULL,' ||
  'MapService varchar2(50) NOT NULL,' ||
  'UserName varchar2(50),' ||
  'Password varchar2(50),' ||
  'DataFrame varchar2(50),' ||
  'InteractiveLegend number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix50 || 'MapTab (MapTabID, DisplayName, MapHost, MapService, UserName, Password, DataFrame, InteractiveLegend, Active) ' ||
  'SELECT MapTabID, DisplayName, MapHost, MapService, UserName, Password, DataFrame, InteractiveLegend, Active ' ||
  'FROM ' || prefix41 || 'MapTab';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MapTabLayer AS SELECT * FROM ' || prefix41 || 'MapTabLayer';

-- create MapTabTileGroup table

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'MapTabTileGroup (' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'TileGroupID varchar2(50) NOT NULL,' ||
  'CheckInLegend number(1),' ||
  'Opacity number(11,3) default 1,' ||
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

-- create Setting table

EXECUTE IMMEDIATE  'CREATE TABLE ' || prefix || 'Setting (' ||
  'Setting varchar2(50) NOT NULL,' ||
  'Value varchar2(400),' ||
  'Required varchar2(5) DEFAULT ''no'',' ||
  'Note varchar2(100)' ||
')';

-- create TileGroup table

EXECUTE IMMEDIATE  'CREATE TABLE ' || prefix50 || 'TileGroup (' ||
  'TileGroupID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

-- create TileLayer table

EXECUTE IMMEDIATE  'CREATE TABLE ' || prefix50 || 'TileLayer (' ||
  'TileLayerID varchar2(50) NOT NULL,' ||
  'TileGroupID varchar2(50) NOT NULL,' ||
  'URL varchar2(400) NOT NULL,' ||
  'Attribution varchar2(400),' ||
  'Overlay number(1) default 1,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'UsageTracking AS SELECT * FROM ' || prefix41 || 'UsageTracking';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'User AS SELECT * FROM ' || prefix41 || 'User';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'Zone AS SELECT * FROM ' || prefix41 || 'Zone';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ZoneLevel AS SELECT * FROM ' || prefix41 || 'ZoneLevel';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix50 || 'ZoneLevelCombo AS SELECT * FROM ' || prefix41 || 'ZoneLevelCombo';


-- GPVSetting content

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Required, Note) values (''AdminEmail'', null, ''YES'', ''email address'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''DefaultApplication'', null, ''ApplicationID'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Required, Note) values (''FullExtent'', null, ''YES'', ''min X, minY, max X, max Y in MeasureProjection coordinates'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ZoomLevels'', ''19'', ''number > 0'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ShowScaleBar'', ''no'', ''yes or no'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''MapProjection'', null, ''Proj4 string, defaults to Web Mercator'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''MeasureProjection'', null, ''Proj4 string, defaults to MapProjection'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''MeasureUnits'', ''both'', ''feet, meters or both'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ActiveColor'', ''Yellow'', ''HTML color spec'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ActiveOpacity'', ''0.5'', ''0.0 = transparent -> 1.0 = opaque'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ActivePolygonMode'', ''fill'', ''fill or outline'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ActivePenWidth'', ''9'', ''pixels'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ActiveDotSize'', ''13'', ''pixels'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''TargetColor'', ''Orange'', ''HTML color spec'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''TargetOpacity'', ''0.5'', ''0.0 = transparent -> 1.0 = opaque'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''TargetPolygonMode'', ''fill'', ''fill or outline'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''TargetPenWidth'', ''9'', ''pixels'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''TargetDotSize'', ''13'', ''pixels'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SelectionColor'', ''Blue'', ''HTML color spec'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SelectionOpacity'', ''0.5'', ''0.0 = transparent -> 1.0 = opaque'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SelectionPolygonMode'', ''fill'', ''fill or outline'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SelectionPenWidth'', ''9'', ''pixels'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SelectionDotSize'', ''13'', ''pixels'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''FilteredColor'', ''#A0A0A0'', ''HTML color spec'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''FilteredOpacity'', ''0.5'', ''0.0 = transparent -> 1.0 = opaque'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''FilteredPolygonMode'', ''fill'', ''fill or outline'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''FilteredPenWidth'', ''9'', ''pixels'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''FilteredDotSize'', ''13'', ''pixels'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''BufferColor'', ''#A0A0FF'', ''HTML color spec'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''BufferOpacity'', ''0.2'', ''0.0 = transparent -> 1.0 = opaque'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''BufferOutlineColor'', ''#8080DD'', ''HTML color spec'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''BufferOutlineOpacity'', ''0'', ''0.0 = transparent -> 1.0 = opaque'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''BufferOutlinePenWidth'', ''0'', ''pixels'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SwatchTileWidth'', ''20'', ''pixels'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SwatchTileHeight'', ''20'', ''pixels'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''LegendExpanded'', ''yes'', ''yes or no'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''SearchAutoSelect'', ''no'', ''yes or no'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''PreserveOnActionChange'', ''selection'', ''target or selection'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''CustomStyleSheet'', null, ''URL'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ExportFormat'', ''xls'', ''csv (comma-separated value) or xls (Excel)'')';

EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''MarkupTimeout'', ''14'', ''days'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''ServerImageCacheTimeout'', ''60'', ''seconds'')';
EXECUTE IMMEDIATE 'insert into ' || prefix50 || 'Setting (Setting, Value, Note) values (''BrowserImageCacheTimeout'', ''60'', ''seconds'')';

END;
/
