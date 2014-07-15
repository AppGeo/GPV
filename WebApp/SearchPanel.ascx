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

<div id="pnlSearchCriteriaSizer">
  <div id="pnlSearchCriteria" class="MainPanel">
    <gpv:Select ID="ddlSearches" runat="server" CssClass="Input" Style="position: absolute; width: 60%" ToolTip="Choose a search to perform" />
  </div>
  <gpv:Div id="pnlSearchScroll" runat="server" />
  <div id="cmdSearch" class="CommandLink Disabled" style="position: absolute; top: auto; bottom: 5px; left: 10px; z-index: 2;">Search</div>
</div>
<div id="pnlSearchGridSizer" class="MainPanel">
  <div id="pnlSearchGrid">
    <table id="grdSearch" class="DataGrid"></table>
  </div>
  <div id="pnlSearchCommand">
    <div id="cmdShowOnMap" class="CommandLink Disabled" style="position: absolute; left: 10px;">Show on Map</div>
    <div id="cmdShowAllOnMap" class="CommandLink Disabled" style="position: absolute; left: 100px;">Show All on Map</div>
  </div>
</div>
<div id="searchDivider"></div>
