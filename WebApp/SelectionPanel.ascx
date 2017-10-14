﻿<%-- 
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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectionPanel.ascx.cs" Inherits="SelectionPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>


<div class="topHead">
  <div class="inner">
    <div class="FunctionHeader">
      <span class="topLftTxt ">
        <span class="topTxt">Selection</span>
        <a class ="helpIcon" type ="selection">
          <img src="Images/faq-icon.png"></a>
      </span>
      <span class="rightCol">
        <a class="prev_arrw FunctionExit" aria-hidden="true"></a>
      </span>
    </div>
  </div>
</div>
<div id="pnlQuery" class="">
  <div class="upperSelection">Build a Spatial Query</div>
  <div id="pnlSelectionOptions" class="frm_box">
    <div class="frm_row noTopMargin">
      <div class="twoCol arrwAhead customSelect">
        <gpv:Select ID="ddlAction" runat="server" CssClass="Input frmSelect" ToolTip="Action to perform with the Select Features tool to the left" />
        <div class="arrw"></div>
      </div>
      <div class="twoCol customSelect">
        <gpv:Select ID="ddlTargetLayer" runat="server" CssClass="Input frmSelect" ToolTip="Target layer containing features of interest" />
      </div>
    </div>
    <div class="frm_row">
      <div class="twoCol arrwAhead customSelect">
        <gpv:Select ID="ddlProximity" runat="server" CssClass="Input frmSelect" ToolTip="Proximity of target features to selection features" />
        <div class="arrw"></div>
      </div>
      <div class="twoCol customSelect">
        <gpv:Select ID="ddlSelectionLayer" runat="server" CssClass="Input frmSelect" ToolTip="Selection layer with features that will help find target features" />
      </div>
    </div>
    <div class="frm_row customSelect">
      <gpv:Select ID="ddlQuery" runat="server" CssClass="Input frmSelect" ToolTip="Filter which lists only those features meeting certain criteria" />
    </div>
  </div>
  <div class="frm_box septr topMargin">
    <div id="pnlSelectTools" class="frm_row ">
      <button id="cmdSelectView" value="Search" class="btn frmBtn rightMargin">Select All</button>
      <button id="cmdZoomSelect" value="Search" class="btn frmBtn ">Zoom To</button>
      <button id="cmdClearSelection" value="Search" class="btn frmBtn floatRight">Clear</button>
    </div>
    <div class="data_box topMargin  customScroll" id="pnlQueryGrid">
      <table id="grdQuery" class="dataTable"></table>
    </div>
    <div id="pnlQueryCommand topMargin" class="frm_row ">
      <span id="labSelectionCount" class="floatLeft">None selected</span>
      <button id="cmdMailingLabels" class="Disabled btn frmBtn mailingLblBtn" title="Mailing Labels">Mailing Labels</button>
      <button id="cmdExportData" class="Disabled btn frmBtn spreadsheetBtn" title="Spreadsheet">Spreadsheet</button>
    </div>
  </div>
</div>
<div id="selectionDivider"></div>
<form id="frmExportData" method="post" target="export">
  <input id="hdnExportLayer" type="hidden" name="layer" />
  <input id="hdnExportIds" type="hidden" name="ids" />
</form>
