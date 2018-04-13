--
--  Copyright 2018 Applied Geographics, Inc.
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
--  GPV51_Oracle_Upgrade.sql
--
--  Creates the GPV v5.1 configuration tables from an existing set of GPV v5.0 tables.
--  Set the prefixes for both sets of table names by changing the values in the "prefix50 varchar2(10)"
--  and "prefix51 varchar2(10)" lines below.  Make sure to run GPV51_Oracle_AddConstraints.sql
--  using the v5.1 prefix to create the necessary constraints on the v5.1 tables.
--

DECLARE
  prefix50 varchar2(10):= 'GPV50';
  prefix51 varchar2(10):= 'GPV51';

BEGIN

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Application AS SELECT * FROM ' || prefix50 || 'Application';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'ApplicationMapTab AS SELECT * FROM ' || prefix50 || 'ApplicationMapTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'ApplicationMarkupCategory AS SELECT * FROM ' || prefix50 || 'ApplicationMarkupCategory';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'ApplicationPrintTemplate AS SELECT * FROM ' || prefix50 || 'ApplicationPrintTemplate';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Connection AS SELECT * FROM ' || prefix50 || 'Connection';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'DataTab AS SELECT * FROM ' || prefix50 || 'DataTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'ExternalMap AS SELECT * FROM ' || prefix50 || 'ExternalMap';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Layer AS SELECT * FROM ' || prefix50 || 'Layer';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'LayerFunction AS SELECT * FROM ' || prefix50 || 'LayerFunction';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'LayerProximity AS SELECT * FROM ' || prefix50 || 'LayerProximity';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Level AS SELECT * FROM ' || prefix50 || 'Level';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'MailingLabel AS SELECT * FROM ' || prefix50 || 'MailingLabel';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'MapTab AS SELECT * FROM ' || prefix50 || 'MapTab';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'MapTabLayer AS SELECT * FROM ' || prefix50 || 'MapTabLayer';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'MapTabTileGroup AS SELECT * FROM ' || prefix50 || 'MapTabTileGroup';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Markup AS SELECT * FROM ' || prefix50 || 'Markup';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'MarkupCategory AS SELECT * FROM ' || prefix50 || 'MarkupCategory';

-- add Details to MarkupGroup

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'MarkupGroup (' ||
  'GroupID number(11) NOT NULL,' ||
  'CategoryID varchar2(50) NOT NULL,' ||
  'DisplayName varchar2(100) NOT NULL,' ||
  'Details varchar2(1000),' ||
  'CreatedBy varchar2(50) NOT NULL,' ||
  'CreatedByUser varchar2(200),' ||
  'Locked number(1) NOT NULL,' ||
  'DateCreated date NOT NULL,' ||
  'DateLastAccessed date NOT NULL,' ||
  'Deleted number(1) NOT NULL' ||
')';

EXECUTE IMMEDIATE 'INSERT INTO ' || prefix51 || 'MarkupGroup (GroupID, CategoryID, DisplayName, CreatedBy, CreatedByUser, Locked, DateCreated, DateLastAccessed, Deleted) ' ||
  'SELECT GroupID, CategoryID, DisplayName, CreatedBy, CreatedByUser, Locked, DateCreated, DateLastAccessed, Deleted ' ||
  'FROM ' || prefix50 || 'MarkupGroup';

-- copy tables

EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'MarkupSequence AS SELECT * FROM ' || prefix50 || 'MarkupSequence';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'PrintTemplate AS SELECT * FROM ' || prefix50 || 'PrintTemplate';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'PrintTemplateContent AS SELECT * FROM ' || prefix50 || 'PrintTemplateContent';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Proximity AS SELECT * FROM ' || prefix50 || 'Proximity';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Query AS SELECT * FROM ' || prefix50 || 'Query';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'SavedState AS SELECT * FROM ' || prefix50 || 'SavedState';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Search AS SELECT * FROM ' || prefix50 || 'Search';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'SearchInputField AS SELECT * FROM ' || prefix50 || 'SearchInputField';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Setting AS SELECT * FROM ' || prefix50 || 'Setting';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'TileGroup AS SELECT * FROM ' || prefix50 || 'TileGroup';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'TileLayer AS SELECT * FROM ' || prefix50 || 'TileLayer';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'UsageTracking AS SELECT * FROM ' || prefix50 || 'UsageTracking';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'User AS SELECT * FROM ' || prefix50 || 'User';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'Zone AS SELECT * FROM ' || prefix50 || 'Zone';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'ZoneLevel AS SELECT * FROM ' || prefix50 || 'ZoneLevel';
EXECUTE IMMEDIATE 'CREATE TABLE ' || prefix51 || 'ZoneLevelCombo AS SELECT * FROM ' || prefix50 || 'ZoneLevelCombo';

END;
/
