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


<div class="topHead FunctionHeader"> <%--FunctionHeader class for changing Header Background Color and Font color--%>
  <div class="inner">
      <span class="topLftTxt ">
        <span class="topTxt">Search</span>
        <a class = "helpIcon" type = "search">
         </a>
      </span>
      <span class="rightCol">
        <a class="prev_arrw FunctionExit" aria-hidden="true"></a>
      </span>
    </div>
  </div>
<div id="pnlSearchFields" class="customScroll ">
  <div id="pnlSearchInputField" class="frm_box">
    <div class="frm_row noTopMargin customSearch">
      <gpv:Select ID="ddlSearches" runat="server" CssClass="frmSelect" ToolTip="Choose a search to perform" />
    </div>
    <gpv:Div ID="pnlSearchScroll" runat="server" class="Panel"></gpv:Div>
  </div>
  <div class="frm_box topMargin alignRight">
    <button id="cmdSearch" title="Search" class="btn frmBtn">Search</button>
    <button id="cmdReset" title="Reset" class="btn frmBtn">Reset</button>
  </div>
  <div id="pnlSearchGrid" class="frm_box horizontalScroll">
    <table id="grdSearch" class="DataGrid dataTable"></table>
      </div>
</div>
<div id="pnlSearchCommand" class="searchCommand Panel septr">
  <div id="labSearchCount">None found</div>
  <button id="cmdShowOnMap" class="btn frmBtn Disabled" title="Show on Map">Show on Map</button>
  <button id="cmdShowAllOnMap" class="btn frmBtn Disabled" title="Show All on Map">Show All on Map</button>
</div>
