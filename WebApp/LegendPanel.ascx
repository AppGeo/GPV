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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LegendPanel.ascx.cs" Inherits="LegendPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>

<div class="FunctionHeader"><span class="glyphicon glyphicon-menu-left FunctionExit" aria-hidden="true"></span>Legend</div>

<div id="pnlLegendContent" class="Panel">
  <div id="pnlLayers">
    <label>Layers</label>
    <button id="cmdRefreshMap" title="Refresh the map with the selected layers"><i class="fa fa-refresh"></i> Refresh Map</button>
    <gpv:Div id="pnlLayerScroll" runat="server" CssClass="Panel LegendScroll" />
  </div>
  <div id="pnlBaseMaps">
    <label>Basemaps and Overlays</label>
    <gpv:Div id="pnlTileScroll" runat="server" CssClass="Panel LegendScroll" />
  </div>
</div>