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

<div id="pnlMarkupContent">
  <div style="left: 14px; top: 11px; right: auto; bottom: auto; width: 81px; text-align: right">Your Name</div>
  <gpv:Input type="text" id="tboMarkupUser" runat="server" CssClass="Input" style="position: absolute; left: 99px; top: 7px; width: 210px; cursor: default" />
  <div style="left: 14px; top: 34px; right: auto; bottom: auto; width: 81px; text-align: right">Category</div>
  <gpv:Select id="ddlMarkupCategory" runat="server" CssClass="Input" style="position: absolute; left: 99px; top: 31px; width: 216px" />
            
  <div style="left: 14px; top: 57px; right: auto; bottom: auto; width: 81px; text-align: right">Markup Group</div>
  <gpv:Div id="cmdNewMarkup" runat="server" CssClass="CommandLink Disabled" style="left: 104px; top: 57px; right: auto; bottom: auto">New</gpv:Div>  
  <div id="cmdZoomToMarkup" class="CommandLink Disabled Toggleable" style="left: 145px; top: 57px; right: auto; bottom: auto">Zoom To</div>  
  <div id="cmdDeleteMarkup" class="CommandLink Disabled Toggleable" style="left: 213px; top: 57px; right: auto; bottom: auto">Delete</div>  
  <div id="cmdExportMarkup" class="CommandLink Disabled Toggleable" style="left: 267px; top: 57px; right: auto; bottom: auto">To KML</div>  

  <div style="left: 95px; top: 76px; right: auto; bottom: auto; width: 34px; text-align: right">Title</div>
  <input type="text" id="tboMarkupTitle" class="Input" style="position: absolute; left: 132px; top: 73px; width: 176px; cursor: default" disabled="disabled" />

  <gpv:Input type="checkbox" id="chkMarkupLock" runat="server" disabled="disabled" style="position: absolute; left: 315px; top: 73px; display: none" />
  <gpv:Label id="labMarkupLock" runat="server" for="chkMarkupLock" style="position: absolute; left: 335px; top: 76px; display: none">Lock</gpv:Label>

  <gpv:Div id="optDrawPoint" runat="server" CssClass="Button MapTool Disabled" style="left: 13px; top: 104px" Title="Draw Point"></gpv:Div>
  <gpv:Div id="optDrawLine" runat="server" CssClass="Button MapTool Disabled" style="left: 41px; top: 104px" Title="Draw Line"></gpv:Div>
  <gpv:Div id="optDrawPolygon" runat="server" CssClass="Button MapTool Disabled" style="left: 69px; top: 104px" Title="Draw Polygon"></gpv:Div>
  <gpv:Div id="optDrawCircle" runat="server" CssClass="Button MapTool Disabled" style="left: 97px; top: 104px" Title="Draw Circle"></gpv:Div>
  <gpv:Div id="optDrawCoordinates" runat="server" CssClass="Button MapTool Disabled" style="left: 125px; top: 104px" Title="Draw Coordinates"></gpv:Div>
  <gpv:Div id="optDrawLength" runat="server" CssClass="Button MapTool Disabled" style="left: 153px; top: 104px" Title="Draw Measured Length"></gpv:Div>
  <gpv:Div id="optDrawArea" runat="server" CssClass="Button MapTool Disabled" style="left: 181px; top: 104px" Title="Draw Measured Area"></gpv:Div>
  <gpv:Div id="optDeleteMarkup" runat="server" CssClass="Button MapTool Disabled" style="left: 218px; top: 104px" Title="Delete Markup"></gpv:Div>
  <gpv:Div id="optColorPicker" runat="server" CssClass="Button MapTool Disabled" style="left: 246px; top: 104px" Title="Pick Color"></gpv:Div>
  <gpv:Div id="optPaintBucket" runat="server" CssClass="Button MapTool Disabled" style="left: 274px; top: 104px" Title="Fill With Color"></gpv:Div>
  <div id="cmdMarkupColor" class="Button Disabled Color" style="left: 317px; top: 104px; background-color: #808080" title="Markup Color"></div>

  <gpv:Div id="optDrawText" runat="server" CssClass="Button MapTool Disabled" style="left: 13px; top: 133px" Title="Draw Text"></gpv:Div>
  <div style="left: 40px; top: 138px; right: auto; bottom: auto; width: 32px; text-align: right">Text</div>
  <input type="text" id="tboMarkupText" class="Input" style="position: absolute; left: 75px; top: 134px; width: 169px; cursor: default" disabled="disabled" />
  <input type="checkbox" id="chkTextGlow" style="position: absolute; left: 253px; top: 134px" />
  <div style="left: 274px; top: 138px; right: auto; bottom: auto">Glow</div>
  <div id="cmdTextGlowColor" class="Button Disabled Color" style="left: 317px; top: 133px; background-color: #808080" title="Text Glow Color"></div>

  <div id="pnlMarkupGrid">
    <table id="grdMarkup" class="DataGrid"></table>
  </div>
</div>