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
--  GPV41_Oracle_Create.sql
--
--  Creates the GPV v4.1 configuration tables.  You can set the prefix for the table names by changing 
--  the value in the "prefix varchar2(10)" line below.  Make sure to run the follow scripts after this
--  one using the same prefix:
--
--    GPV41_Oracle_AddConstraints.sql - to create the necessary constraints
--    GPV41_Oracle_LoadMailingLabels.sql - to load the mailing labels table
--


-- make sure SQL Plus does not interpret the ampersand as a substitution character (needed for URLs)

SET define off

DECLARE 
  prefix varchar2(10):= 'GPV41';

BEGIN
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Application (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'AuthorizedRoles varchar2(200),' ||
  'FunctionTabs varchar2(50),' ||
  'DefaultMapTab varchar2(50),' ||
  'DefaultAction varchar2(50),' ||
  'DefaultTargetLayer varchar2(50),' ||
  'DefaultProximity varchar2(50),' ||
  'DefaultSelectionLayer varchar2(50),' ||
  'DefaultLevel varchar2(50),' ||
  'FullExtent varchar2(50),' ||
  'OverviewMapID varchar2(50), ' ||
  'CoordinateModes varchar2(50),' ||
  'ZoneLevelID varchar2(50),' ||
  'TrackUse number(1),' ||
  'About varchar2(1000),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'ApplicationMapTab (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'ApplicationMarkupCategory (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'CategoryID varchar2(50) NOT NULL,' ||
  'SequenceNo number(1) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'ApplicationPrintTemplate (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'TemplateID varchar2(50) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Connection (' ||
  'ConnectionID varchar2(50) NOT NULL,' ||
  'ConnectionString varchar2(1000) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'DataTab (' ||
  'DataTabID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'ExternalMap (' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'URL varchar2(400) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Layer (' ||
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
  'BaseMapID varchar2(50),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'LayerFunction (' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'FunctionName varchar2(20) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'LayerProximity (' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'ProximityID varchar2(50) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Level (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'LevelID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'SequenceNo number(3) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'MailingLabel (' ||
  'ID number(11) NOT NULL,' ||
  'Manufacturer varchar2(50) NOT NULL,' ||
  'ModelNo varchar2(100) NOT NULL,' ||
  'LabelSize varchar2(100) NOT NULL,' ||
  'IsAvailable number(1) NOT NULL,' ||
  'LabelsAcross number(2) NOT NULL,' ||
  'dxLabel number(7,3) NOT NULL,' ||
  'dyLabel number(7,3) NOT NULL,' ||
  'dxSpace number(7,3) NOT NULL,' ||
  'dySpace number(7,3) NOT NULL,' ||
  'xLeft number(7,3) NOT NULL,' ||
  'xRight number(7,3) NOT NULL,' ||
  'yTop number(7,3) NOT NULL,' ||
  'xOrg number(7,3) NOT NULL,' ||
  'yOrg number(7,3) NOT NULL,' ||
  'IsDotMatrix number(1) NOT NULL,' ||
  'IsPortrait number(1) NOT NULL,' ||
  'IsMetric number(1) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'MapTab (' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'MapHost varchar2(50) NOT NULL,' ||
  'MapService varchar2(50) NOT NULL,' ||
  'UserName varchar2(50),' || 
  'Password varchar2(50),' ||
  'DataFrame varchar2(50),' ||
  'InteractiveLegend number(1),' ||
  'BaseMapID varchar2(50),' ||
  'ShowBaseMapInLegend number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'MapTabLayer (' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'AllowTarget number(1),' ||
  'AllowSelection number(1),' ||
  'ShowInLegend number(1),' ||
  'CheckInLegend number(1),' ||
  'IsExclusive number(1),' ||
  'ShowinPrintLegend number(1)' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Markup (' ||
  'MarkupID number(11) NOT NULL,' ||
  'GroupID number(11) NOT NULL,' ||
  'Shape nclob NOT NULL,' ||
  'Color varchar2(25) NOT NULL,' ||
  'Glow varchar2(25),' ||
  'Text varchar2(1000),' ||
  'Measured number(1),' ||
  'DateCreated date NOT NULL,' ||
  'Deleted number(1) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'MarkupCategory (' ||
  'CategoryID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'AuthorizedRoles varchar2(200),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'MarkupGroup (' ||
  'GroupID number(11) NOT NULL,' ||
  'CategoryID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(100) NOT NULL,' ||
  'CreatedBy varchar2(50) NOT NULL,' ||
  'CreatedByUser varchar2(200),' ||
  'Locked number(1) NOT NULL,' ||
  'DateCreated date NOT NULL,' ||
  'DateLastAccessed date NOT NULL,' ||
  'Deleted number(1) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'MarkupSequence (' ||
  'NextGroupID number(11) NOT NULL,' ||
  'NextMarkupID number(11) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'PrintTemplate (' ||
  'TemplateID varchar2(50) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'TemplateTitle varchar2(50) NOT NULL,' ||
  'PageWidth number(7,3) NOT NULL,' ||
  'PageHeight number(7,3) NOT NULL,' ||
  'AlwaysAvailable number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'PrintTemplateContent (' ||
  'TemplateID varchar2(50) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'ContentType varchar2(20) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'OriginX number(7,3) NOT NULL,' ||
  'OriginY number(7,3) NOT NULL,' ||
  'Width number(7,3) NOT NULL,' ||
  'Height number(7,3) NOT NULL,' ||
  'FillColor varchar2(25),' ||
  'OutlineColor varchar2(25),' ||
  'OutlineWidth number(11),' ||
  'LegendColumnWidth decimal,' ||
  'Text varchar2(1000),' ||
  'TextAlign varchar2(6),' ||
  'TextWrap number(1),' ||
  'FontFamily varchar2(16),' ||
  'FontSize number(3),' ||
  'FontBold number(1),' ||
  'FileName varchar2(25),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Proximity (' ||
  'ProximityID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(150) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Distance number(11,3) NOT NULL,' ||
  'IsDefault number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Query (' ||
  'QueryID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'SavedState (' ||
  'StateID varchar2(12) NOT NULL,' ||
  'DateCreated date NOT NULL,' ||
  'DateLastAccessed date NOT NULL,' ||
  'State nclob NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Search (' ||
  'SearchID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'SearchInputField (' ||
  'FieldID varchar2(50) NOT NULL,' ||
  'SearchID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'ColumnName varchar2(50) NOT NULL,' ||
  'FieldType varchar2(50) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100),' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'UsageTracking (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'UrlQuery varchar2(1000) NOT NULL,' ||
  'DateStarted date NOT NULL,' ||
  'UserAgent varchar2(400) NOT NULL,' ||
  'UserHostAddress varchar2(15) NOT NULL,' ||
  'UserHostName varchar2(50) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'User (' ||
  'UserName varchar2(200) NOT NULL,' ||
  'Password varchar2(40),' ||
  'Role varchar2(25),' ||
  'DisplayName varchar2(50),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'Zone (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'ZoneID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'SequenceNo number(3) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'ZoneLevel (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'ZoneTypeDisplayName varchar2(50),' ||
  'LevelTypeDisplayName varchar2(50),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix || 'ZoneLevelCombo (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'ZoneID varchar2(50) NOT NULL,' ||
  'LevelID varchar2(50) NOT NULL,' ||
  'Active number(1) default 1' ||
')';


EXECUTE IMMEDIATE 'insert into ' || prefix || 'ExternalMap (DisplayName, URL, SequenceNo, Active) values (''Google Maps'', ''http://maps.google.com/?ll={lat},{lon}&z={lev}'', 1, 1)';
EXECUTE IMMEDIATE 'insert into ' || prefix || 'ExternalMap (DisplayName, URL, SequenceNo, Active) values (''Bing Maps'', ''http://www.bing.com/maps/?cp={lat}~{lon}&lvl={lev}'', 2, 1)';

EXECUTE IMMEDIATE 'insert into ' || prefix || 'MarkupSequence (NextGroupID, NextMarkupID) values (1, 1)';
END;
/

-- restore the substitution character

SET define on
