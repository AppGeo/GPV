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
--  GPV50_SQLServer_LoadPrintTemplates.sql
--
--  Loads the GPV v5.0 PrintTemplate and PrintTemplateContent configuration tables with four 
--  default templates for 8.5" x 11" and 11" X 17" page sizes (tabloid and landscape). You can 
--  set the prefix for the table names by changing the value in the "set @prefix" line below.
--

DECLARE 
  prefix varchar2(10):= 'GPV50';

BEGIN

-- load PrintTemplate

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplate (TemplateID, SequenceNo, TemplateTitle, ' ||
  'PageWidth, PageHeight, AlwaysAvailable, Active) values (''LetterPortrait'', 1,' ||
  '''8.5 x 11 Portrait'', 8.5, 11, 1, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplate (TemplateID, SequenceNo, TemplateTitle, ' ||
  'PageWidth, PageHeight, AlwaysAvailable, Active) values (''LetterLandscape'', 2, ' ||
  '''8.5 x 11 Landscape'', 11, 8.5, 1, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplate (TemplateID, SequenceNo, TemplateTitle, ' ||
  'PageWidth, PageHeight, AlwaysAvailable, Active) values (''TabloidLandscape'', 3,' ||
  '''11 x 17 Landscape'', 17, 11, 1, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplate (TemplateID, SequenceNo, TemplateTitle, ' ||
  'PageWidth, PageHeight, AlwaysAvailable, Active) values (''TabloidPortrait'', 4,' ||
  '''11 x 17 Portrait'', 11, 17, 1, 1) '; 

-- load PrintTemplateContent

-- LetterLandscape

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 1, ''map'', NULL, 0.565, 0.558, ' ||
  '8.063, 7.381, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 2, ''box'', NULL, 0.5, 0.5, ' ||
  '10, 7.5, NULL, ''#000000'', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 3, ''input'', ''Title'', 8.842, 7.5, ' ||
  '1.437, 0.5, NULL, NULL, NULL, NULL, NULL, NULL, ''center'', 12, NULL, 1, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 4, ''image'', NULL, 8.98, 6.4, ' ||
  '1.15, 0.451, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''logo.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 5, ''text'', NULL, 8.671, 0.4, ' ||
  '1.814, 2.5, NULL, NULL, NULL, NULL, NULL, NULL, ''left'', 5, NULL, 1, ''disclaimer.txt'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 6, ''text'', NULL, 8.7, 0.574, ' ||
  '1.2, 0.109, NULL, NULL, NULL, NULL, ''AppGeo GPV Printable Map'', NULL, ''left'', 7, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 7, ''date'', NULL, 9.7, 0.574, ' ||
  '0.75, 0.109, NULL, NULL, NULL, NULL, NULL, NULL, ''right'', 7, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 9, ''image'', NULL, 9.9, 0.75, ' ||
  '0.3, 0.47, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''north.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 10, ''scale'', NULL, 8.842, 0.82, ' ||
  '1, 0.2, NULL, NULL, NULL, NULL, NULL, NULL, ''center'', 8, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 11, ''legend'', NULL, 8.95, 2.9, ' ||
  '1.6, 3.45, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, 1)';

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterLandscape'', 12, ''input'', ''Notes'', 8.671, 2.9, ' ||
  '1.8, 0.4, NULL, NULL, NULL, NULL, NULL, NULL, ''left'', 5, 1, 1, NULL, 1)'; 

-- LetterPortrait

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 1, ''box'', NULL, 0.5, 0.5, ' ||
  '7.5, 10, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 2, ''map'', NULL, 0.5, 2.25, ' ||
  '7.5, 7.95, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 3, ''box'', NULL, 3, 0.5, ' ||
  '2.5, 1.75, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)';

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 4, ''text'', NULL, 0.6, 10.3, ' ||
  '3, 0.25, NULL, NULL, NULL, NULL, ''AppGeo GPV Printable Map'', NULL, ''left'', 10, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 5, ''date'', NULL, 4.9, 10.3, ' ||
  '3, 0.25, NULL, NULL, NULL, NULL, NULL, NULL, ''right'', 10, NULL, NULL, NULL, 1)';

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 6, ''input'', ''Title'', 0.5, 10.275, ' ||
  '7.5, 0.25, NULL, NULL, NULL, NULL, ''AppGeo GPV Printable Map'', NULL, NULL, 13, 1, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 7, ''image'', NULL, 3.675, 0.8, ' ||
  '1.15, 0.451, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''logo.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 8, ''text'', NULL, 5.6, 0.5, ' ||
  '2.3, 1.75, NULL, NULL, NULL, NULL, NULL, NULL, ''left'', 5, NULL, 1, ''disclaimer.txt'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 9, ''legend'', NULL, 0.6, 0.55, ' ||
  '3, 1.64, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 10, ''box'', NULL, 0.45, 0.45, ' ||
  '7.6, 10.1, NULL, ''#000000'', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 11, ''image'', NULL, 2.6, 0.55, ' ||
  '0.3, 0.47, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''north.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''LetterPortrait'', 12, ''scale'', NULL, 1.2, 0.55, ' ||
  '1, 0.2, NULL, NULL, NULL, NULL, NULL, NULL, ''center'', 8, NULL, NULL, NULL, 1)'; 

-- TabloidLandscape

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 1, ''map'', NULL, 0.565, 0.565, ' ||
  '12.465, 9.87, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 2, ''box'', NULL, 0.5, 0.5, ' ||
  '16, 10, NULL, ''#000000'', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 3, ''input'', ''Title'', 13.665, 9.706, ' ||
  '2.221, 0.647, NULL, NULL, NULL, NULL, NULL, NULL, ''center'', 14, NULL, 1, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 4, ''image'', NULL, 14.05, 8.282, ' ||
  '1.488, 0.584, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''logo.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 5, ''text'', NULL, 13.401, 0.85, ' ||
  '2.803, 3.235, NULL, NULL, NULL, NULL, NULL, NULL, ''left'', 7, NULL, 1, ''disclaimer.txt'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 6, ''text'', NULL, 13.445, 0.743, ' ||
  '1.855, 0.141, NULL, NULL, NULL, NULL, ''AppGeo GPV Printable Map'', NULL, ''left'', 9, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 7, ''date'', NULL, 14.991, 0.743, ' ||
  '1.159, 0.141, NULL, NULL, NULL, NULL, NULL, NULL, ''right'', 9, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 9, ''image'', NULL, 15.3, 0.971, ' ||
  '0.388, 0.499, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''north.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 10, ''scale'', NULL, 13.665, 1.061, ' ||
  '1.545, 0.259, NULL, NULL, NULL, NULL, NULL, NULL, ''center'', 10, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 11, ''legend'', NULL, 13.7, 3.7, ' ||
  '2.5, 4.465, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidLandscape'', 12, ''input'', ''Notes'', 13.401, 4, ' ||
  '2.803, 0.4, NULL, NULL, NULL, NULL, NULL, NULL, ''left'', 5, 1, 1, NULL, 1)'; 

-- TabloidPortrait

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 1, ''box'', NULL, 0.5, 0.5, ' ||
  '10, 16, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 2, ''map'', NULL, 0.5, 3.25, ' ||
  '10, 12.75, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 3, ''box'', NULL, 3.882, 0.5, ' ||
  '3.235, 2.75, NULL, ''#000000'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 4, ''text'', NULL, 0.776, 16.2, ' ||
  '3.882, 0.386, NULL, NULL, NULL, NULL, ''AppGeo GPV'', NULL, ''left'', 12, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 5, ''date'', NULL, 6.341, 16.2, ' ||
  '3.882, 0.386, NULL, NULL, NULL, NULL, NULL, NULL, ''right'', 12, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 6, ''input'', ''Title'', 3, 16.1, ' ||
  '5, 0.386, NULL, NULL, NULL, NULL, ''AppGeo GPV Printable Map'', NULL, ''center'', 15, 1, 1, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 7, ''image'', NULL, 4.756, 1.6, ' ||
  '1.488, 0.584, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''logo.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 8, ''text'', NULL, 7.25, 0.75, ' ||
  '2.976, 2.318, NULL, NULL, NULL, NULL, NULL, NULL, ''left'', 7, NULL, 1, ''disclaimer.txt'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 9, ''legend'', NULL, 0.776, 0.55, ' ||
  '3.882, 2.535, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 10, ''box'', NULL, 0.45, 0.45, ' ||
  '10.1, 16.1, NULL, ''#000000'', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 11, ''image'', NULL, 4.5, 0.6, ' ||
  '0.388, 0.499, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, ''north.gif'', 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 12, ''scale'', NULL, 5.2, 0.7, ' ||
  '1.294, 0.309, NULL, NULL, NULL, NULL, NULL, NULL, ''center'', 10, NULL, NULL, NULL, 1)'; 

EXECUTE IMMEDIATE 'insert into ' || prefix || 'PrintTemplateContent (TemplateID, SequenceNo, ' ||
  'ContentType, DisplayName, OriginX, OriginY, Width, Height, FillColor, OutlineColor, ' ||
  'OutlineWidth, LegendColumnWidth, Text, FontFamily, TextAlign, FontSize, FontBold, ' ||
  'TextWrap, FileName, Active) values (''TabloidPortrait'', 13, ''input'', ''Notes'', 4.25, 1.1, ' ||
  '2.6, 0.4, NULL, NULL, NULL, NULL, NULL, NULL, ''center'', 8, NULL, 1, NULL, 1)'; 

END;
/
