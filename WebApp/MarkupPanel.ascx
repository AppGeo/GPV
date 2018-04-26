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
  <gpv:Button id="cmdNewMarkup" runat="server" class="CommandLink Disabled MarkupGroup" title="Create a new markup group">New</gpv:Button>
  <button id="cmdZoomToMarkup" class="CommandLink Disabled MarkupGroup Toggleable" title="Zoom to the current markup group">Zoom To</button>  
  <button id="cmdDeleteMarkup" class="CommandLink Disabled MarkupGroup Toggleable" title="Delete the current markup group">Delete</button>  
  <button id="cmdExportMarkup" class="CommandLink Disabled MarkupGroup Toggleable" title="Export the current markup group to KML">To KML</button><br />

  <gpv:Input type="checkbox" id="chkMarkupLock" runat="server" disabled="disabled" style="visibility: hidden" />
  <gpv:Label id="labMarkupLock" runat="server" for="chkMarkupLock" style="visibility: hidden" title="Keep others from editing">Lock</gpv:Label>
  <label>Title</label>
  <input type="text" id="tboMarkupTitle" class="Input" disabled="disabled" title="Title of the current markup group"/><br />

  <label>Style</label>
  <button id="btnMarkupColor" style="margin-left: 0" title="Color of new markup"><span id="cmdMarkupColor" class="Button Color"></span> Color</button>
  <button id="btnTextGlowColor" style="float: right; margin-right: 70px" title="Glow color of new text"><span id="cmdTextGlowColor" class="Button Color"></span> Text Glow</button>
  <input type="checkbox" id="chkTextGlow" style="float:right" title="Use glow color on text" />

  <div class="Panel" id="pnlMarkupGrid">
    <table id="grdMarkup" class="DataGrid"></table>
  </div>
</div>