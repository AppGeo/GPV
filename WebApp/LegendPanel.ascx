﻿﻿<%-- 
Copyright 2016 Applied Geographics, Inc.

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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LegendPanel.ascx.cs" Inherits="LegendPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>

<div class="topHead FunctionHeader"> <%--FunctionHeader class for changing Header Background Color and Font color--%>
  <div class="inner">
      <span class="topLftTxt ">
        <span class="topTxt">Maps</span>
        <a  class ="helpIcon"  type ="maps"> 
           <i class=" fa fa-question-circle" aria-hidden="true"></i>
          </a>
      </span>
      <span class="rightCol">
        <i class="fa fa-angle-left FunctionExit" aria-hidden="true"></i>
      </span>
    </div>
</div>
<div class="frm_box">
  <div class="frm_row">
    <div class="frmLabel themeTitle">
      Map Themes
    </div>
  </div>
  <div class="frm_row">
    <div id="pnlMapThemes" class="MapMenu customMaps">
      <button class="frmSelect" type="button" id="btnMapTheme" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true" title="Theme">
        <gpv:Span id="selectedTheme" runat="server"></gpv:Span>
      </button>
      <ul id="selectMapTheme" class="dropdown-menu" aria-labelledby="btnMapTheme">
        <asp:PlaceHolder ID="phlMapTheme" runat="server"></asp:PlaceHolder>
      </ul>
    </div>
  </div>
</div>
<div class="frm_box septr">
  <div id="pnlLayers" class="list_headng noTopMargin">Layers</div>
  <button id="cmdRefreshMap" class="btn frmBtn frmBtn2" title="Refresh the map with the selected layers"><i class="fa fa-refresh"></i>Refresh Map</button>
  <div id="pnlLegendContent" class="Panel">
    <div>
      <gpv:Div ID="pnlLayerScroll" runat="server" CssClass="Panel LegendScroll customScroll" />
    </div>
  </div>
  <div id="pnlBaseMaps" runat="server" visible="false">
    <label>Basemaps and Overlays</label>
    <gpv:Div ID="pnlTileScroll" runat="server" CssClass="Panel LegendScroll" />
  </div>
</div>
