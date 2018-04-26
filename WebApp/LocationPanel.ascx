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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LocationPanel.ascx.cs" Inherits="LocationPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>

<div class="FunctionHeader"><span class="glyphicon glyphicon-menu-left FunctionExit" aria-hidden="true"></span>Location</div>

<div id="pnlLocationContent" class="Panel">

  <select id="ddlZoneLevelSelect" class="Input">
    <gpv:Option id="optTabZone" runat="server" style="display: none" data-table="tblZone"></gpv:Option>
    <gpv:Option id="optTabLevel" runat="server" style="display: none" data-table="tblLevel"></gpv:Option>
    <gpv:Option id="optTabLevelByZone" runat="server" style="display: none" data-table="tblLevelByZone"></gpv:Option>
  </select>

  <div id="pnlZoneLevel">
    <gpv:Div id="pnlZoneLevelControl" runat="server" style="display: none">
      <input type="radio" id="optZoneLevelAll" name="zls" class="ZoneLevelSelection" checked="checked" /> <label for="optZoneLevelAll">All</label>&nbsp;&nbsp;
      <input type="radio" id="optZoneLevelSelected" name="zls" class="ZoneLevelSelection" /> <label for="optZoneLevelAll">Containing selection only</label>
    </gpv:Div>
    <div id="pnlZoneLevelScroll">
      <gpv:Table id="tblZone" runat="server" CssClass="ZoneLevelTable" style="display: none">
        <thead>
          <tr>
            <th class="Value">Selected</th>
            <th>Name</th>
          </tr>
        </thead>
      </gpv:Table>
      <gpv:Table id="tblLevel" runat="server" CssClass="ZoneLevelTable" style="display: none">
        <thead>
          <tr>
            <th class="Value">Selected</th>
            <th>Name</th>
          </tr>
        </thead>
      </gpv:Table>
      <gpv:Table id="tblLevelByZone" runat="server" CssClass="ZoneLevelTable" style="display: none">
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