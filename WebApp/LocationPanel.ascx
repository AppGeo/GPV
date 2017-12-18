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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LocationPanel.ascx.cs" Inherits="LocationPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>
<div class="topHead">
  <div class="inner">
    <div class="FunctionHeader">
      <span class="topLftTxt ">
        <span class="topTxt">Location    </span>
        <a class ="helpIcon" type ="location">
          </a>
      </span>
      <span class="rightCol">
        <a class="prev_arrw FunctionExit" aria-hidden="true"></a>
      </span>
    </div>
  </div>
</div>
<div class="frm_box">
  <div id="pnlLocationContent" class="Panel">
    <div class="frm_row">
      <div class="oneCol customLocation">
        <select id="ddlZoneLevelSelect" class="Input frmSelect">
          <gpv:Option ID="optTabZone" runat="server" Style="display: none" data-table="tblZone"></gpv:Option>
          <gpv:Option ID="optTabLevel" runat="server" Style="display: none" data-table="tblLevel"></gpv:Option>
          <gpv:Option ID="optTabLevelByZone" runat="server" Style="display: none" data-table="tblLevelByZone"></gpv:Option>
        </select>
      </div>
    </div>
    <div id="pnlZoneLevel" class="frm_row">
      <gpv:Div ID="pnlZoneLevelControl" CssClass="radioWrap" runat="server" Style="display: none">
        <input type="radio" id="optZoneLevelAll" name="zls" class="ZoneLevelSelection frmRadio" checked="checked" />
        <label class="radioTxt" for="optZoneLevelAll">All</label>&nbsp;&nbsp;
        <input type="radio" id="optZoneLevelSelected" name="zls" class="ZoneLevelSelection frmRadio" />
        <label class="radioTxt" for="optZoneLevelAll">Containing selection only</label>
      </gpv:Div>
    </div>
    <div id="pnlZoneLevelScroll" class="frm_row customScroll">
      <gpv:Table ID="tblZone" runat="server" CssClass="ZoneLevelTable" Style="display: none">
        <thead>
          <tr>
            <th class="Value">Selected</th>
            <th>Name</th>
          </tr>
        </thead>
      </gpv:Table>
      <gpv:Table ID="tblLevel" runat="server" CssClass="ZoneLevelTable" Style="display: none">
        <thead>
          <tr>
            <th class="Value">Selected</th>
            <th>Name</th>
          </tr>
        </thead>
      </gpv:Table>
      <gpv:Table ID="tblLevelByZone" runat="server" CssClass="ZoneLevelTable" Style="display: none">
        <thead>
          <tr>
            <th class="Value">Selected</th>
            <th>Name</th>
          </tr>
        </thead>
      </gpv:Table>
    </div>
  </div>
</div>
