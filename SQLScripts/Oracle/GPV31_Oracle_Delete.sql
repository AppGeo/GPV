--
--  © 2004-2009, Applied Geographics, Inc.  All rights reserved.
--
--  GPV31_Oracle_Delete.sql
--
--  Deletes all GPV v3.1 configuration tables.  You can set the prefix for the table names by 
--  changing the value in the "prefix varchar2(10)" line below.
--

DECLARE 
  prefix varchar2(10):= 'GPV31';

BEGIN 

-- tables not referenced by a foreign key

  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'ApplicationMapTab CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'ApplicationMarkupCategory CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'ApplicationPrintTemplate CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'DataTab CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'ExternalMap CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'LayerFunction CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'LayerProximity CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'MailingLabel CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'MapTabLayer CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Markup CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'MarkupGroup CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'MarkupSequence CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'PrintTemplateContent CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Proximity CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Query CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'SavedState CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'UsageTracking CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'User CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'ZoneLevelCombo CASCADE CONSTRAINTS';

-- tables referenced by a foreign key

  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Application CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Connection CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Layer CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Level CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'MapTab CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'MarkupCategory CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'PrintTemplate CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'Zone CASCADE CONSTRAINTS';
  EXECUTE IMMEDIATE 'DROP TABLE ' || prefix || 'ZoneLevel CASCADE CONSTRAINTS';

END;
/
