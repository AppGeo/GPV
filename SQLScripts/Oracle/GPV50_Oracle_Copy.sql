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
--  GPV50_Oracle_Copy.sql
--
--  Copies the GPV v5.0 configuration tables.  You can set the source and destination prefixes for
--  the table names by changing the values in the "srcPrefix varchar2(10)" and "desPrefix varchar2(10)"
--  lines below.  Make sure to run GPV50_Oracle_AddConstraints.sql using the destination prefix to
--  create the necessary constraints on the copied tables.
--

DECLARE
  srcPrefix varchar2(10):= 'GPV50';
  desPrefix varchar2(10):= 'GPVx';

BEGIN
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Application AS SELECT * FROM ' || srcPrefix || 'Application';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'ApplicationMapTab AS SELECT * FROM ' || srcPrefix || 'ApplicationMapTab';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'ApplicationMarkupCategory AS SELECT * FROM ' || srcPrefix || 'ApplicationMarkupCategory';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'ApplicationPrintTemplate AS SELECT * FROM ' || srcPrefix || 'ApplicationPrintTemplate';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Connection AS SELECT * FROM ' || srcPrefix || 'Connection';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'DataTab AS SELECT * FROM ' || srcPrefix || 'DataTab';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'ExternalMap AS SELECT * FROM ' || srcPrefix || 'ExternalMap';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Layer AS SELECT * FROM ' || srcPrefix || 'Layer';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'LayerFunction AS SELECT * FROM ' || srcPrefix || 'LayerFunction';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'LayerProximity AS SELECT * FROM ' || srcPrefix || 'LayerProximity';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Level AS SELECT * FROM ' || srcPrefix || 'Level';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'MailingLabel AS SELECT * FROM ' || srcPrefix || 'MailingLabel';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'MapTab AS SELECT * FROM ' || srcPrefix || 'MapTab';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'MapTabLayer AS SELECT * FROM ' || srcPrefix || 'MapTabLayer';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'MapTabTileGroup AS SELECT * FROM ' || srcPrefix || 'MapTabTileGroup';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Markup AS SELECT * FROM ' || srcPrefix || 'Markup';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'MarkupCategory AS SELECT * FROM ' || srcPrefix || 'MarkupCategory';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'MarkupGroup AS SELECT * FROM ' || srcPrefix || 'MarkupGroup';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'MarkupSequence AS SELECT * FROM ' || srcPrefix || 'MarkupSequence';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'PrintTemplate AS SELECT * FROM ' || srcPrefix || 'PrintTemplate';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'PrintTemplateContent AS SELECT * FROM ' || srcPrefix || 'PrintTemplateContent';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Proximity AS SELECT * FROM ' || srcPrefix || 'Proximity';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Query AS SELECT * FROM ' || srcPrefix || 'Query';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'SavedState AS SELECT * FROM ' || srcPrefix || 'SavedState';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Search AS SELECT * FROM ' || srcPrefix || 'Search';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'SearchInputField AS SELECT * FROM ' || srcPrefix || 'SearchInputField';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Setting AS SELECT * FROM ' || srcPrefix || 'Setting';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'TileGroup AS SELECT * FROM ' || srcPrefix || 'TileGroup';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'TileLayer AS SELECT * FROM ' || srcPrefix || 'TileLayer';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'UsageTracking AS SELECT * FROM ' || srcPrefix || 'UsageTracking';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'User AS SELECT * FROM ' || srcPrefix || 'User';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'Zone AS SELECT * FROM ' || srcPrefix || 'Zone';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'ZoneLevel AS SELECT * FROM ' || srcPrefix || 'ZoneLevel';
  EXECUTE IMMEDIATE 'CREATE TABLE ' || desPrefix || 'ZoneLevelCombo AS SELECT * FROM ' || srcPrefix || 'ZoneLevelCombo';
END;
/
