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
--  GPV51_Oracle_AddConstraints.sql
--
--  Adds primary key, foreign key and unique constraints to the GPV v5.1 configuration tables.  You can
--  set the prefix for the table names by changing the value in the "prefix varchar2(10)" line below.
--

DECLARE
  prefix varchar2(10):= 'GPV51';

BEGIN

-- add primary key and unique constraints

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Application ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Application PRIMARY KEY ' ||
  '(' ||
  '  ApplicationID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationMapTab ADD ' ||
  'CONSTRAINT ' || prefix || 'ApplicationMapTabUnique UNIQUE ' ||
  '(' ||
  '  ApplicationID,' ||
  '  MapTabID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationMarkupCategory ADD ' ||
  'CONSTRAINT ' || prefix || 'AppMarkupCategoryUnique UNIQUE ' ||
  '(' ||
  '  ApplicationID,' ||
  '  CategoryID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationPrintTemplate ADD ' ||
  'CONSTRAINT ' || prefix || 'AppPrintTemplateUnique UNIQUE ' ||
  '(' ||
  '  ApplicationID,' ||
  '  TemplateID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Connection ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Connection PRIMARY KEY' ||
  '(' ||
  '  ConnectionID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'DataTab ADD ' ||
  'CONSTRAINT PK_' || prefix || 'DataTab PRIMARY KEY' ||
  '(' ||
  '  DataTabID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ExternalMap ADD ' ||
  'CONSTRAINT PK_' || prefix || 'ExternalMap PRIMARY KEY' ||
  '(' ||
  '  DisplayName' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Layer ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Layer PRIMARY KEY ' ||
  '(' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'LayerFunction ADD ' ||
  'CONSTRAINT ' || prefix || 'LayerFunctionUnique UNIQUE ' ||
  '(' ||
  '  LayerID,' ||
  '  FunctionName' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'LayerProximity ADD ' ||
  'CONSTRAINT ' || prefix || 'LayerProximityUnique UNIQUE ' ||
  '(' ||
  '  LayerID,' ||
  '  ProximityID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Level ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Level PRIMARY KEY ' ||
  '(' ||
  '  ZoneLevelID,' ||
  '  LevelID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MapTab ADD ' ||
  'CONSTRAINT PK_' || prefix || 'MapTab PRIMARY KEY ' ||
  '(' ||
  '  MapTabID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MapTabLayer ADD ' ||
  'CONSTRAINT ' || prefix || 'MapTabLayerUnique UNIQUE ' ||
  '(' ||
  '  MapTabID,' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MapTabTileGroup ADD ' ||
'CONSTRAINT ' || prefix || 'MapTabTGroupUnique UNIQUE ' ||
'(' ||
'  MapTabID,' ||
'  TileGroupID' ||
')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Markup ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Markup PRIMARY KEY ' ||
  '(' ||
  '  MarkupID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MarkupCategory ADD ' ||
  'CONSTRAINT PK_' || prefix || 'MarkupCategory PRIMARY KEY ' ||
  '(' ||
  '  CategoryID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MarkupGroup ADD ' ||
  'CONSTRAINT PK_' || prefix || 'MarkupGroup PRIMARY KEY ' ||
  '(' ||
  '  GroupID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'PrintTemplate ADD ' ||
  'CONSTRAINT PK_' || prefix || 'PrintTemplate PRIMARY KEY ' ||
  '(' ||
  '  TemplateID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'PrintTemplateContent ADD ' ||
  'CONSTRAINT ' || prefix || 'PrintTempContUnique UNIQUE ' ||
  '(' ||
  '  TemplateID,' ||
  '  SequenceNo' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Proximity ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Proximity PRIMARY KEY ' ||
  '(' ||
  '  ProximityID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Query ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Query PRIMARY KEY ' ||
  '(' ||
  '  QueryID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'SavedState ADD ' ||
  'CONSTRAINT PK_' || prefix || 'SavedState PRIMARY KEY ' ||
  '(' ||
  '  StateID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Search ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Search PRIMARY KEY ' ||
  '(' ||
  '  SearchID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'SearchInputField ADD ' ||
  'CONSTRAINT PK_' || prefix || 'SearchInputField PRIMARY KEY ' ||
  '(' ||
  '  FieldID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Setting ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Setting PRIMARY KEY ' || 
  '(' ||
  '  Setting' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'TileGroup ADD ' ||
  'CONSTRAINT PK_' || prefix || 'TileGroup PRIMARY KEY ' ||
  '(' ||
  '  TileGroupID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'TileLayer ADD ' ||
  'CONSTRAINT PK_' || prefix || 'TileLayer PRIMARY KEY ' ||
  '(' ||
  '  TileLayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Zone ADD ' ||
  'CONSTRAINT PK_' || prefix || 'Zone PRIMARY KEY ' ||
  '(' ||
  '  ZoneLevelID,' ||
  '  ZoneID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ZoneLevel ADD ' ||
  'CONSTRAINT PK_' || prefix || 'ZoneLevel PRIMARY KEY ' ||
  '(' ||
  '  ZoneLevelID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ZoneLevelCombo ADD ' ||
  'CONSTRAINT ' || prefix || 'ZoneLevelComboUnique UNIQUE ' ||
  '(' ||
  '  ZoneLevelID,' ||
  '  ZoneID,' ||
  '  LevelID' ||
  ')';

-- add foreign key constraints

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Application ADD ' ||
  'CONSTRAINT FK_' || prefix || 'App_ZoneLev FOREIGN KEY ' ||
  '(' ||
  '  ZoneLevelID' ||
  ') REFERENCES ' || prefix || 'ZoneLevel (' ||
  '  ZoneLevelID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationMapTab ADD ' ||
  'CONSTRAINT FK_' || prefix || 'AppMapTab_App FOREIGN KEY ' ||
  '(' ||
  '  ApplicationID' ||
  ') REFERENCES ' || prefix || 'Application (' ||
  '  ApplicationID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationMapTab ADD ' ||
  'CONSTRAINT FK_' || prefix || 'AppMapTab_MapTab FOREIGN KEY ' ||
  '(' ||
  '  MapTabID' ||
  ') REFERENCES ' || prefix || 'MapTab (' ||
  '  MapTabID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationMarkupCategory ADD ' ||
  'CONSTRAINT FK_' || prefix || 'AppMarkCat_App FOREIGN KEY ' ||
  '(' ||
  '  ApplicationID' ||
  ') REFERENCES ' || prefix || 'Application (' ||
  '  ApplicationID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationMarkupCategory ADD ' ||
  'CONSTRAINT FK_' || prefix || 'AppMarkCat_MarkCat FOREIGN KEY ' ||
  '(' ||
  '  CategoryID' ||
  ') REFERENCES ' || prefix || 'MarkupCategory (' ||
  '  CategoryID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationPrintTemplate ADD ' ||
  'CONSTRAINT FK_' || prefix || 'AppPrint_App FOREIGN KEY ' ||
  '(' ||
  '  ApplicationID' ||
  ') REFERENCES ' || prefix || 'Application (' ||
  '  ApplicationID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ApplicationPrintTemplate ADD ' ||
  'CONSTRAINT FK_' || prefix || 'AppPrint_Print FOREIGN KEY ' ||
  '(' ||
  '  TemplateID' ||
  ') REFERENCES ' || prefix || 'PrintTemplate (' ||
  '  TemplateID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'DataTab ADD ' ||
  'CONSTRAINT FK_' || prefix || 'DataTab_Layer FOREIGN KEY ' ||
  '(' ||
  '  LayerID' ||
  ') REFERENCES ' || prefix || 'Layer (' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'DataTab ADD ' ||
  'CONSTRAINT FK_' || prefix || 'DataTab_Conn FOREIGN KEY ' ||
  '(' ||
  '  ConnectionID' ||
  ') REFERENCES ' || prefix || 'Connection (' ||
  '  ConnectionID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'LayerFunction ADD ' ||
  'CONSTRAINT FK_' || prefix || 'LayFunc_Layer FOREIGN KEY ' ||
  '(' ||
  '  LayerID' ||
  ') REFERENCES ' || prefix || 'Layer (' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'LayerFunction ADD ' ||
  'CONSTRAINT FK_' || prefix || 'LayFunc_Conn FOREIGN KEY ' ||
  '(' ||
  '  ConnectionID' ||
  ') REFERENCES ' || prefix || 'Connection (' ||
  '  ConnectionID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'LayerProximity ADD ' ||
  'CONSTRAINT FK_' || prefix || 'LayerProx_Layer FOREIGN KEY ' ||
  '(' ||
  '  LayerID' ||
  ') REFERENCES ' || prefix || 'Layer (' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'LayerProximity ADD ' ||
  'CONSTRAINT FK_' || prefix || 'LayerProx_Prox FOREIGN KEY ' ||
  '(' ||
  '  ProximityID' ||
  ') REFERENCES ' || prefix || 'Proximity (' ||
  '  ProximityID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Level ADD ' ||
  'CONSTRAINT FK_' || prefix || 'Lev_ZoneLev FOREIGN KEY ' ||
  '(' ||
  '  ZoneLevelID' ||
  ') REFERENCES ' || prefix || 'ZoneLevel (' ||
  '  ZoneLevelID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MapTabLayer ADD ' ||
  'CONSTRAINT FK_' || prefix || 'MapTabLayer_Layer FOREIGN KEY ' ||
  '(' ||
  '  LayerID' ||
  ') REFERENCES ' || prefix || 'Layer (' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MapTabLayer ADD ' ||
  'CONSTRAINT FK_' || prefix || 'MapTabLayer_MapTab FOREIGN KEY ' ||
  '(' ||
  '  MapTabID' ||
  ') REFERENCES ' || prefix || 'MapTab (' ||
  '  MapTabID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MapTabTileGroup ADD ' ||
'CONSTRAINT FK_' || prefix || 'MapTabTGroup_TGrp FOREIGN KEY ' ||
'(' ||
'  TileGroupID' ||
') REFERENCES ' || prefix || 'TileGroup (' ||
'  TileGroupID' ||
')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MapTabTileGroup ADD ' ||
  'CONSTRAINT FK_' || prefix || 'MapTabTGroup_MTab FOREIGN KEY ' ||
  '(' ||
  '  MapTabID' ||
  ') REFERENCES ' || prefix || 'MapTab (' ||
  '  MapTabID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Markup ADD ' ||
  'CONSTRAINT FK_' || prefix || 'Mark_MarkGroup FOREIGN KEY ' ||
  '(' ||
  '  GroupID' ||
  ') REFERENCES ' || prefix || 'MarkupGroup (' ||
  '  GroupID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'MarkupGroup ADD ' ||
  'CONSTRAINT FK_' || prefix || 'MarkGroup_MarkCat FOREIGN KEY ' ||
  '(' ||
  '  CategoryID' ||
  ') REFERENCES ' || prefix || 'MarkupCategory (' ||
  '  CategoryID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'PrintTemplateContent ADD ' ||
  'CONSTRAINT FK_' || prefix || 'PrintCont_Print FOREIGN KEY ' ||
  '(' ||
  '  TemplateID' ||
  ') REFERENCES ' || prefix || 'PrintTemplate (' ||
  '  TemplateID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Query ADD ' ||
  'CONSTRAINT FK_' || prefix || 'Query_Layer FOREIGN KEY ' ||
  '(' ||
  '  LayerID' ||
  ') REFERENCES ' || prefix || 'Layer (' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Query ADD ' ||
  'CONSTRAINT FK_' || prefix || 'Query_Conn FOREIGN KEY ' ||
  '(' ||
  '  ConnectionID' ||
  ') REFERENCES ' || prefix || 'Connection (' ||
  '  ConnectionID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Search ADD ' ||
  'CONSTRAINT FK_' || prefix || 'Search_Layer FOREIGN KEY ' ||
  '(' ||
  '  LayerID' ||
  ') REFERENCES ' || prefix || 'Layer (' ||
  '  LayerID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Search ADD ' ||
  'CONSTRAINT FK_' || prefix || 'Search_Conn FOREIGN KEY ' ||
  '(' ||
  '  ConnectionID' ||
  ') REFERENCES ' || prefix || 'Connection (' ||
  '  ConnectionID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'SearchInputField ADD ' ||
  'CONSTRAINT FK_' || prefix || 'SInput_Search FOREIGN KEY ' ||
  '(' ||
  '  SearchID' ||
  ') REFERENCES ' || prefix || 'Search (' ||
  '  SearchID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'SearchInputField ADD ' ||
  'CONSTRAINT FK_' || prefix || 'SInput_Conn FOREIGN KEY ' ||
  '(' ||
  '  ConnectionID' ||
  ') REFERENCES ' || prefix || 'Connection (' ||
  '  ConnectionID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'TileLayer ADD ' ||
  'CONSTRAINT FK_' || prefix || 'TLayer_TGroup FOREIGN KEY ' ||
  '(' ||
  '  TileGroupID' ||
  ') REFERENCES ' || prefix || 'TileGroup (' ||
  '  TileGroupID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'Zone ADD ' ||
  'CONSTRAINT FK_' || prefix || 'Zone_ZoneLev FOREIGN KEY ' ||
  '(' ||
  '  ZoneLevelID' ||
  ') REFERENCES ' || prefix || 'ZoneLevel (' ||
  '  ZoneLevelID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ZoneLevelCombo ADD ' ||
  'CONSTRAINT FK_' || prefix || 'ZoneLevCombo_Zone FOREIGN KEY ' ||
  '(' ||
  '  ZoneLevelID, ZoneID' ||
  ') REFERENCES ' || prefix || 'Zone (' ||
  '  ZoneLevelID, ZoneID' ||
  ')';

EXECUTE IMMEDIATE 'ALTER TABLE ' || prefix || 'ZoneLevelCombo ADD ' ||
  'CONSTRAINT FK_' || prefix || 'ZoneLevCombo_Lev FOREIGN KEY ' ||
  '(' ||
  '  ZoneLevelID, LevelID' ||
  ') REFERENCES ' || prefix || 'Level (' ||
  '  ZoneLevelID, LevelID' ||
  ')';

END;
/
