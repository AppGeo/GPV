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

<div id="pnlSearchSizer">
    <div id="pnlSearchCriteriaSizer">
      <div id="pnlSearchCriteria" class="MainPanel">
      <gpv:Select ID="ddlSearches" runat="server" CssClass="Input" Style="position: absolute; width: 60%" ToolTip="Choose a search to perform" />
      </div>
    </div>
    <div id="pnlSearchGridSizer">
      <div id="pnlSearchGrid" class="MainPanel">
        <table id="grdSearch" class="DataGrid"></table>
      </div>
    </div>
  </div>
<div id="pnlSearchDataSizer">
  <div id="pnlSearchDataTabs" class="TabPanel">
    <div id="pnlSearchDataTabScroll" class="TabScroll"></div>
  </div>
  <div id="pnlSearchData" class="MainPanel">
    <div id="cmdSearchDataPrint" class="CommandLink Disabled">Print</div>
    <div id="pnlSearchDataList"></div>
  </div>
</div>
<div id="searchCriteriaDivider"></div>
<div id="searchDivider"></div>
