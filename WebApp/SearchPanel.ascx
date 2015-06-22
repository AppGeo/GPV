<%-- 
  Copyright 2012 Applied Geographics, Inc.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchPanel.ascx.cs" Inherits="SearchPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>

<div class="FunctionHeader"><span class="glyphicon glyphicon-menu-left FunctionExit" aria-hidden="true"></span>Search</div>
<div id="pnlSearchInputFieldSizer" class="Panel">
  <div id="pnlSearchInputField" class="Panel">
    <gpv:Select ID="ddlSearches" runat="server" CssClass="Input" ToolTip="Choose a search to perform" />
  </div>
  <gpv:Div id="pnlSearchScroll" runat="server" class="Panel" />
  <div class="searchCommand Panel">
    <button id="cmdSearch" title="Seach">Search</button>
    <button id="cmdReset" title="Reset">Reset</button>
  </div>
</div>
<div id="pnlSearchGridSizer" class="MainPanel Panel">
  <div id="pnlSearchGrid" class="Panel">
    <table id="grdSearch" class="DataGrid"></table>
  </div>
  <div id="pnlSearchCommand" class="searchResultCommand Panel">
    <div id="labSearchCount">None found</div>
    <button id="cmdShowOnMap" class="Disabled" title="Show on Map">Show on Map</button>
    <button id="cmdShowAllOnMap" class="Disabled" title="Show All on Map">Show All on Map</button>
    <%--<div id="cmdShowOnMap" class="CommandLink Disabled">Show on Map</div>
    <div id="cmdShowAllOnMap" class="CommandLink Disabled">Show All on Map</div>--%>
  </div>
</div>
<div id="searchDivider" class="Panel"></div>
