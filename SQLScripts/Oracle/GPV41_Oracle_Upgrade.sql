--
--  Copyright 2014 Applied Geographics, Inc.
--
--  Licensed under the Apache License, Version 2.0 (the "License");
--  you may not use this file except in compliance with the License.
--  You may obtain a copy of the License at
--
--      http://www.apache.org/licenses/LICENSE-2.0

--  Unless required by applicable law or agreed to in writing, software
--  distributed under the License is distributed on an "AS IS" BASIS,
--  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
--  See the License for the specific language governing permissions and
--  limitations under the License.
--
--  GPV41_Oracle_Upgrade.sql
--
--  Creates the GPV v4.1 configuration tables from an existing set of GPV v3.1 tables.
--  Set the prefixes for both sets of table names by changing the values in the "prefix31 varchar2(10)" 
--  and "prefix41 varchar2(10)" lines below.  Make sure to run GPV41_Oracle_AddConstraints.sql 
--  using the v4.1 prefix to create the necessary constraints on the v4.1 tables.
--

DECLARE 
  prefix31 varchar2(10):= 'GPV31';
  prefix41 varchar2(10):= 'GPV41';

BEGIN 

-- remove AllowMapTabScroll and AllowDataTabScroll from GPVApplication

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Application (' ||
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

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix41 || 'Application (ApplicationID, DisplayName, AuthorizedRoles, FunctionTabs, DefaultMapTab, ' ||
  'DefaultAction, DefaultTargetLayer, DefaultProximity, DefaultSelectionLayer, DefaultLevel, FullExtent, OverviewMapID, CoordinateModes, ' ||
  'ZoneLevelID, TrackUse, About, Active) ' ||
  'SELECT ApplicationID, DisplayName, AuthorizedRoles, FunctionTabs, DefaultMapTab, DefaultAction, DefaultTargetLayer, DefaultProximity, ' ||
  'DefaultSelectionLayer, DefaultLevel, FullExtent, OverviewMapID, CoordinateModes, ZoneLevelID, TrackUse, About, Active ' ||
  'FROM ' || prefix31 || 'Application';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'ApplicationMapTab AS SELECT * FROM ' || prefix31 || 'ApplicationMapTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'ApplicationMarkupCategory AS SELECT * FROM ' || prefix31 || 'ApplicationMarkupCategory';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'ApplicationPrintTemplate AS SELECT * FROM ' || prefix31 || 'ApplicationPrintTemplate';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Connection AS SELECT * FROM ' || prefix31 || 'Connection';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'DataTab AS SELECT * FROM ' || prefix31 || 'DataTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'ExternalMap AS SELECT * FROM ' || prefix31 || 'ExternalMap';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Layer AS SELECT * FROM ' || prefix31 || 'Layer';

-- rename Function to FunctionName in GPVLayerFunction

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'LayerFunction (' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'FunctionName varchar2(20) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix41 || 'LayerFunction (LayerID, FunctionName, ConnectionID, StoredProc, Active) ' ||
  'SELECT LayerID, Function, ConnectionID, StoredProc, Active ' ||
  'FROM ' || prefix31 || 'LayerFunction';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'LayerProximity AS SELECT * FROM ' || prefix31 || 'LayerProximity';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Level AS SELECT * FROM ' || prefix31 || 'Level';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'MailingLabel AS SELECT * FROM ' || prefix31 || 'MailingLabel';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'MapTab AS SELECT * FROM ' || prefix31 || 'MapTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'MapTabLayer AS SELECT * FROM ' || prefix31 || 'MapTabLayer';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Markup AS SELECT * FROM ' || prefix31 || 'Markup';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'MarkupCategory AS SELECT * FROM ' || prefix31 || 'MarkupCategory';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'MarkupGroup AS SELECT * FROM ' || prefix31 || 'MarkupGroup';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'MarkupSequence AS SELECT * FROM ' || prefix31 || 'MarkupSequence';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'PrintTemplate AS SELECT * FROM ' || prefix31 || 'PrintTemplate';

-- rename TextFont to FontFamily, TextSize to FontSize and TextBold to FontBold in GPVPrintTemplateContent

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'PrintTemplateContent (' ||
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

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix41 || 'PrintTemplateContent (TemplateID, SequenceNo, ContentType, DisplayName, ' ||
  'OriginX, OriginY, Width, Height, FillColor, OutlineColor, OutlineWidth, LegendColumnWidth, Text, TextAlign, TextWrap, ' ||
  'FontFamily, FontSize, FontBold, FileName, Active) ' ||
  'SELECT TemplateID, SequenceNo, ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, TextAlign, TextWrap, TextFont, TextSize, TextBold, FileName, Active ' ||
  'FROM ' || prefix31 || 'PrintTemplateContent';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Proximity AS SELECT * FROM ' || prefix31 || 'Proximity';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Query AS SELECT * FROM ' || prefix31 || 'Query';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'SavedState AS SELECT * FROM ' || prefix31 || 'SavedState';

-- create GPVSearch

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Search (' ||
  'SearchID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

-- create GPVSearchCriteria

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'SearchInputField (' ||
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

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'UsageTracking AS SELECT * FROM ' || prefix31 || 'UsageTracking';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'User AS SELECT * FROM ' || prefix31 || 'User';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'Zone AS SELECT * FROM ' || prefix31 || 'Zone';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'ZoneLevel AS SELECT * FROM ' || prefix31 || 'ZoneLevel';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix41 || 'ZoneLevelCombo AS SELECT * FROM ' || prefix31 || 'ZoneLevelCombo';

END;
/
