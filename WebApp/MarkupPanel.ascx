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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarkupPanel.ascx.cs" Inherits="MarkupPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>

<div class="FunctionHeader"><span class="glyphicon glyphicon-menu-left FunctionExit" aria-hidden="true"></span>Markup</div>

<div id="pnlMarkupContent" class="Panel">
  <label for="tboMarkupUser">Your Name</label>
  <gpv:Input type="text" id="tboMarkupUser" runat="server" CssClass="Input" /><br />
  <label for="ddlMarkupCategory">Category</label>
  <gpv:Select id="ddlMarkupCategory" runat="server" CssClass="Input" /><br />
            
  <label>Markup Group</label>
  <button id="cmdNewMarkup" runat="server" class="CommandLink Disabled MarkupGroup" style="margin-left: 0">New</button>
  <button id="cmdZoomToMarkup" class="CommandLink Disabled MarkupGroup Toggleable">Zoom To</button>  
  <button id="cmdDeleteMarkup" class="CommandLink Disabled MarkupGroup Toggleable">Delete</button>  
  <button id="cmdExportMarkup" class="CommandLink Disabled MarkupGroup Toggleable">To KML</button><br />

  <label>Title</label>
  <input type="text" id="tboMarkupTitle" class="Input" disabled="disabled" />

  <gpv:Input type="checkbox" id="chkMarkupLock" runat="server" disabled="disabled" style="display: none" />
  <gpv:Label id="labMarkupLock" runat="server" for="chkMarkupLock" style="display: none">Lock</gpv:Label><br />

  <label>Style</label>
    <button id="btnMarkupColor" style="margin-left: 0"><span id="cmdMarkupColor" class="Button Disabled Color" title="Markup Color"></span> Color</button>
    <button id="btnTextGlowColor" style="float: right; margin-right: 70px" ><span id="cmdTextGlowColor" class="Button Disabled Color" title="Text Glow Color"></span> Text Glow</button>
    <input type="checkbox" id="chkTextGlow" style="float:right" />

  <div class="Panel" id="pnlMarkupGrid">
    <table id="grdMarkup" class="DataGrid"></table>
  </div>
</div>