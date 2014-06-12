--
--  © 2004-2009, Applied Geographics, Inc.  All rights reserved.
--
--  GPV31_Oracle_Upgrade.sql
--
--  Creates the GPV v3.1 configuration tables from an existing set of GPV v3.0 tables.
--  Set the prefixes for both sets of table names by changing the values in the "prefix30 varchar2(10)" 
--  and "prefix31 varchar2(10)" lines below.  Make sure to run GPV31_Oracle_AddConstraints.sql 
--  using the v3.1 prefix to create the necessary constraints on the v3.1 tables.
--

DECLARE 
  prefix30 varchar2(10):= 'GPV30';
  prefix31 varchar2(10):= 'GPV31';

BEGIN 

-- add AuthorizedRoles, DefaultLevel, ZoneLevelID and Active to GPVApplication

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Application (' ||
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
  'AllowMapTabScroll number(1),' ||
  'AllowDataTabScroll number(1),' ||
  'ZoneLevelID varchar2(50),' ||
  'TrackUse number(1),' ||
  'About varchar2(1000),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'Application (ApplicationID, DisplayName, FunctionTabs, DefaultMapTab, ' ||
  'DefaultAction, DefaultTargetLayer, DefaultProximity, DefaultSelectionLayer, FullExtent, OverviewMapID, CoordinateModes, ' ||
  'AllowMapTabScroll, AllowDataTabSroll, TrackUse, About, Active) ' ||
  'SELECT ApplicationID, DisplayName, FunctionTabs, DefaultMapTab, DefaultAction, DefaultTargetLayer, DefaultProximity, ' ||
  'DefaultSelectionLayer, FullExtent, OverviewMapID, CoordinateModes, AllowMapTabScroll, AllowDataTabSroll, TrackUse, About, 1 ' ||
  'FROM ' || prefix30 || 'Application';

-- copy tables


EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'ApplicationMapTab (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'ApplicationMapTab (ApplicationID, MapTabID, SequenceNo) ' ||
  'SELECT ApplicationID, MapTabID, SequenceNo ' ||
  'FROM ' || prefix30 || 'ApplicationMapTab';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'ApplicationMarkupCategory (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'CategoryID varchar2(50) NOT NULL,' ||
  'SequenceNo number(1) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'ApplicationMarkupCategory (ApplicationID, CategoryID, SequenceNo) ' ||
  'SELECT ApplicationID, CategoryID, SequenceNo ' ||
  'FROM ' || prefix30 || 'ApplicationMarkupCategory';

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'ApplicationPrintTemplate (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'TemplateID varchar2(50) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'ApplicationPrintTemplate (ApplicationID, TemplateID) ' ||
  'SELECT ApplicationID, TemplateID ' ||
  'FROM ' || prefix30 || 'ApplicationPrintTemplate';

-- add Active to GPVConnection

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Connection (' ||
  'ConnectionID varchar2(50) NOT NULL,' ||
  'ConnectionString varchar2(1000) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'Connection (ConnectionID, ConnectionString, Active) ' ||
  'SELECT ConnectionID, ConnectionString, 1 ' ||
  'FROM ' || prefix30 || 'Connection';

-- add Active to GPVDataTab

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'DataTab (' ||
  'DataTabID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'DataTab (DataTabID, LayerID, DisplayName, ConnectionID, StoredProc, SequenceNo, Active) ' ||
  'SELECT DataTabID, LayerID, DisplayName, ConnectionID, StoredProc, SequenceNo, 1 ' ||
  'FROM ' || prefix30 || 'DataTab';

-- add Active to GPVExternalMap, increase size of URL and SequenceNo

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'ExternalMap (' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'URL varchar2(400) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'ExternalMap (DisplayName, URL, SequenceNo, Active) ' ||
  'SELECT DisplayName, URL, SequenceNo, 1 ' ||
  'FROM ' || prefix30 || 'ExternalMap';

-- add DisplayName, MetaDataURL, ZoneField, LevelField and Active to GPVLayer, fix MaxNumberSelected size, and rename MaxSelectionLayer to MaxSelectionArea

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Layer (' ||
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

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'Layer (LayerID, LayerName, KeyField, MaxNumberSelected, MaxSelectionArea, ' ||
  'MinNearestDistance, MaxNearestDistance, BaseMapID, Active) ' ||
  'SELECT LayerID, LayerName, KeyField, MaxNumberSelected, MaxSelectionLayer, MinNearestDistance, MaxNearestDistance, BaseMapID, 1 ' ||
  'FROM ' || prefix30 || 'Layer';

-- add Active to GPVLayerFunction

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'LayerFunction (' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'Function varchar2(20) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'LayerFunction (LayerID, Function, ConnectionID, StoredProc, Active) ' ||
  'SELECT LayerID, Function, ConnectionID, StoredProc, 1 ' ||
  'FROM ' || prefix30 || 'LayerFunction';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'LayerProximity (' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'ProximityID varchar2(50) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'LayerProximity (LayerID, ProximityID) ' ||
  'SELECT LayerID, ProximityID ' ||
  'FROM ' || prefix30 || 'LayerProximity';

-- create GPVLevel

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Level (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'LevelID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'SequenceNo number(3) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'MailingLabel AS SELECT * FROM ' || prefix30 || 'MailingLabel';

-- add Active to GPVMapTab

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'MapTab (' ||
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

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'MapTab (MapTabID, DisplayName, MapHost, MapService, DataFrame, UserName, Password, InteractiveLegend, BaseMapID, ShowBaseMapInLegend, Active) ' ||
  'SELECT MapTabID, DisplayName, MapHost, MapService, DataFrame, UserName, Password, InteractiveLegend, BaseMapID, ShowBaseMapInLegend, 1 ' ||
  'FROM ' || prefix30 || 'MapTab';

-- add IsExclusive to GPVMapTabLayer

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'MapTabLayer (' ||
  'MapTabID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'AllowTarget number(1),' ||
  'AllowSelection number(1),' ||
  'ShowInLegend number(1),' ||
  'CheckInLegend number(1),' ||
  'IsExclusive number(1),' ||
  'ShowinPrintLegend number(1)' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'MapTabLayer (MapTabID, LayerID, AllowTarget, AllowSelection, ShowInLegend, CheckInLegend, ShowInPrintLegend) ' ||
  'SELECT MapTabID, LayerID, AllowTarget, AllowSelection, ShowInLegend, CheckInLegend, ShowInPrintLegend ' ||
  'FROM ' || prefix30 || 'MapTabLayer';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Markup (' ||
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

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'Markup (MarkupID, GroupID, Shape, Color, Glow, Text, Measured, DateCreated, Deleted) ' ||
  'SELECT MarkupID, GroupID, Shape, Color, Glow, Text, Measured, DateCreated, Deleted ' ||
  'FROM ' || prefix30 || 'Markup';

-- add AuthorizedRoles and Active to GPVMarkupCategory

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'MarkupCategory (' ||
  'CategoryID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'AuthorizedRoles varchar2(200),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'MarkupCategory (CategoryID, DisplayName, Active) ' ||
  'SELECT CategoryID, DisplayName, 1 FROM ' || prefix30 || 'MarkupCategory';

-- add Locked and CreatedByUser to GPVMarkupGroup

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'MarkupGroup (' ||
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

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'MarkupGroup (GroupID, CategoryID, DisplayName, CreatedBy, Locked, DateCreated, DateLastAccessed, Deleted) ' ||
  'SELECT GroupID, CategoryID, DisplayName, CreatedBy, 0, DateCreated, DateLastAccessed, Deleted FROM ' || prefix30 || 'MarkupGroup';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'MarkupSequence AS SELECT * FROM ' || prefix30 || 'MarkupSequence';

-- add Active to GPVPrintTemplate

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'PrintTemplate (' ||
  'TemplateID varchar2(50) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'TemplateTitle varchar2(50) NOT NULL,' ||
  'PageWidth number(7,3) NOT NULL,' ||
  'PageHeight number(7,3) NOT NULL,' ||
  'AlwaysAvailable number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'PrintTemplate (TemplateID, SequenceNo, TemplateTitle, PageWidth, PageHeight, AlwaysAvailable, Active) ' ||
  'SELECT  TemplateID, SequenceNo, TemplateTitle, PageWidth, PageHeight, AlwaysAvailable, 1 FROM ' || prefix30 || 'PrintTemplate';

-- add Active to GPVPrintTemplateContent

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'PrintTemplateContent (' ||
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
  'TextFont varchar2(16),' ||
  'TextAlign varchar2(6),' ||
  'TextSize number(3),' ||
  'TextBold number(1),' ||
  'TextWrap number(1),' ||
  'FileName varchar2(25),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'PrintTemplateContent (TemplateID, SequenceNo, ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, OutlineWidth, LegendColumnWidth, Text, TextFont, TextAlign, TextSize, TextBold, TextWrap, FileName, Active) ' ||
  'SELECT TemplateID, SequenceNo, ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, OutlineWidth, LegendColumnWidth, Text, TextFont, TextAlign, TextSize, TextBold, TextWrap, FileName, 1 FROM ' || prefix30 || 'PrintTemplateContent';

-- add Active to GPVProximity

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Proximity (' ||
  'ProximityID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(150) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Distance number(11,3) NOT NULL,' ||
  'IsDefault number(1),' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'Proximity (ProximityID, DisplayName, SequenceNo, Distance, IsDefault, Active) ' ||
  'SELECT ProximityID, DisplayName, SequenceNo, Distance, IsDefault, 1 FROM ' || prefix30 || 'Proximity';

-- add Active to GPVQuery

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Query (' ||
  'QueryID varchar2(50) NOT NULL,' ||
  'LayerID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50) NOT NULL,' ||
  'ConnectionID varchar2(50),' ||
  'StoredProc varchar2(100) NOT NULL,' ||
  'SequenceNo number(2) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'Query (QueryID, LayerID, DisplayName, ConnectionID, StoredProc, SequenceNo, Active) ' ||
  'SELECT QueryID, LayerID, DisplayName, ConnectionID, StoredProc, SequenceNo, 1 FROM ' || prefix30 || 'Query';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'SavedState AS SELECT * FROM ' || prefix30 || 'SavedState';
  
-- increase the size of the UserAgent column in GPVUsageTracking

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'UsageTracking (' ||
  'ApplicationID varchar2(50) NOT NULL,' ||
  'UrlQuery varchar2(1000) NOT NULL,' ||
  'DateStarted date NOT NULL,' ||
  'UserAgent varchar2(400) NOT NULL,' ||
  'UserHostAddress varchar2(15) NOT NULL,' ||
  'UserHostName varchar2(50) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix31 || 'UsageTracking (ApplicationID, UrlQuery, DateStarted, UserAgent, UserHostAddress, UserHostName) ' ||
  'SELECT ApplicationID, UrlQuery, DateStarted, UserAgent, UserHostAddress, UserHostName ' ||
  'FROM ' || prefix30 || 'UsageTracking';

-- create GPVUser

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'User (' ||
  'UserName varchar2(200) NOT NULL,' ||
  'Password varchar2(40),' ||
  'Role varchar2(25),' ||
  'DisplayName varchar2(50),' ||
  'Active number(1) default 1' ||
')';

-- create GPVZone

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'Zone (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'ZoneID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(50),' ||
  'SequenceNo number(3) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

-- create GPVZoneLevel

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'ZoneLevel (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'ZoneTypeDisplayName varchar2(50),' ||
  'LevelTypeDisplayName varchar2(50),' ||
  'Active number(1) default 1' ||
')';

-- create GPVZoneLevelCombo

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix31 || 'ZoneLevelCombo (' ||
  'ZoneLevelID varchar2(50) NOT NULL,' ||
  'ZoneID varchar2(50) NOT NULL,' ||
  'LevelID varchar2(50) NOT NULL,' ||
  'Active number(1) default 1' ||
')';

END;
/
