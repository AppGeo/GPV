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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Viewer.aspx.cs" Inherits="Viewer" EnableViewState="false" EnableSessionState="true" EnableEventValidation="false" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SearchPanel" Src="SearchPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SelectionPanel" Src="SelectionPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="LegendPanel" Src="LegendPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="LocationPanel" Src="LocationPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MarkupPanel" Src="MarkupPanel.ascx" %>

<!DOCTYPE html>

<html>
<head id="head" runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>AppGeo GPV</title>
  <script type="text/javascript"> if (typeof(JSON) == "undefined") { location.href = "Incompatible.htm"; } </script></head>
<body>
  <div id="pnlBody" class="container-fluid">
    <div id="pnlHeader" class="Panel">
      <span id="cmdMenu" class="CommandLink" title="Show/hide the menu"><span class="glyphicon glyphicon-menu-hamburger" style="font-size: 22px;"></span></span>
      <uc1:Header ID="Header1" runat="server" />
      <a id="cmdHelp" runat="server" class="CommandLink" title="Show the help pages" href="Help.aspx" target="help"><span class="glyphicon glyphicon-question-sign" style="font-size: 22px;"></span></a>
    </div>
    <div id="pnlContent" runat="server" class="Panel">
      <div id="pnlFunctionSidebar" runat="server" class="Panel">
        <div id="pnlFunctionTabs" class="TabPanel Panel">
          <ul class="TabScroll Menu">
            <li id="tabSearch" runat="server" class="MenuItem Normal" style="display: none">Search</li>
            <li id="tabSelection" runat="server" class="MenuItem Normal" style="display: none">Selection</li>
            <li id="tabLegend" runat="server" class="MenuItem Normal" style="display: none">Legend</li>
            <li id="tabLocation" runat="server" class="MenuItem Normal" style="display: none">Location</li>
            <li id="tabMarkup" runat="server" class="MenuItem Normal" style="display: none">Markup</li>
            <li id="tabShare" runat="server" class="MenuItem Normal">Share</li>
          </ul>
           <div id="pnlAttribution" class="Panel">
            <span id="spnVersion" runat="server" class="VersionText"></span>&nbsp;&nbsp;
            <a class="VersionText" href="http://www.appgeo.com" target="AppGeo">AppGeo</a>
          </div>
        </div>
        <div id="pnlFunction" class="Panel">
          <%-- Fuction tab name with < to click to return to main menu --%>
          <div id="pnlSearch" runat="server" class="FunctionPanel Panel" style="display: none">
            <uc1:SearchPanel ID="ucSearchPanel" runat="server" />
          </div>
          <div id="pnlSelection" runat="server" class="FunctionPanel Panel" style="display: none">
            <uc1:SelectionPanel ID="ucSelectionPanel" runat="server" />
          </div>
          <div id="pnlLegend" runat="server" class="FunctionPanel Panel" style="display: none">
            <uc1:LegendPanel ID="ucLegendPanel" runat="server" />
          </div>
          <div id="pnlLocation" runat="server" class="FunctionPanel Panel" style="display: none">
            <uc1:LocationPanel ID="ucLocationPanel" runat="server" />
          </div>
          <div id="pnlMarkup" runat="server" class="FunctionPanel Panel" style="display: none">
            <uc1:MarkupPanel ID="ucMarkupPanel" runat="server" />
          </div>
          <div id="pnlShare" runat="server" class="FunctionPanel Panel" style="display: none">
            <div class="FunctionHeader"><span class="glyphicon glyphicon-menu-left FunctionExit" aria-hidden="true"></span>Share</div>
            <div id="pnlShareContent" class="Panel">
              <button id="cmdForPrint" class="share-type" title="Printable Map"><i class="fa fa-print"></i> Print</button>
              <button id="cmdForEmail" class="share-type" title="Email This Page"><i class="fa fa-envelope-o"></i> Email</button>
              <button id="cmdForExport" class="share-type" title="Export Map Extent"><i class="fa fa-external-link"></i> Export</button>
              <button id="cmdForDownload" class="share-type" title="Download Map"><i class="fa fa-download"></i> Download</button>
              <div id="pnlPrint" class="share">
                <form id="form1" runat="server">
                  <div class="FunctionLabel">Create Printable PDF Map</div><br>
                  <label for="ddlPrintTemplate">Format</label>
                  <asp:DropDownList id="ddlPrintTemplate" runat="server" Width="216px" AutoPostBack="true" />
		              <button id="cmdCreate" runat="server" Enabled="false" style="left: 139px; top: 48px">Create</button>
                </form>
              </div>
              <div id="pnlEmail" class="share">
                <div class="FunctionLabel">Email a Map</div><br>
                <label for="tboEmail">Email Address</label>
                <input id="tboEmail" />
                <button id="cmdEmail">Send</button>
              </div>
              <div id="pnlExport" class="share">
              <div class="FunctionLabel">Export Map Extent</div><br>
              <select id="ddlExternalMap" runat="server" class="Input" style="width: 200px"></select>
                <button id="cmdExternalMap">Go</button>
              </div>
              <div id="pnlDownload" class="share">
                <div class="FunctionLabel">Download a Map</div><br>
                <label for="ddlSaveMap">Save as</label>
                <select id="ddlSaveMap" runat="server" class="Input" style="width: 90px">
                  <option value="image">as Image</option>
                  <option value="kml">as KML</option>
                </select>
                <button id="cmdSaveMap">Save</button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div id="pnlMapSizer" runat="server" class="Panel">
        <div id="pnlMap" class="MainPanel Panel">

           <div id="cmdFullView" class="Button" title="Full View"><span class="glyphicon glyphicon-globe"></span></div>

           <div id="pnlMapThemes">
            <button class="btn btn-default dropdown-toggle" type="button" id="btnMapTheme" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
              <span id="selectedTheme"></span>
              <span class="caret"></span>
            </button>
            <ul id="selectMapTheme" class="dropdown-menu" aria-labelledby="btnMapTheme">
              <asp:PlaceHolder id="phlMapTheme" runat="server"></asp:PlaceHolder>
            </ul>
          </div>

          <div id="pnlMapLevels">
<%--            <button class="btn btn-default dropdown-toggle" type="button" id="btnMapLevel" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
              <span id="selectedLevel"></span>
              <span class="caret"></span>
            </button>
            <ul id="selectMapLevel" class="dropdown-menu" aria-labelledby="btnMapLevel">
              <asp:PlaceHolder id="phlMapLevel" runat="server"></asp:PlaceHolder>
            </ul>--%>
          <select id="ddlLevel" runat="server" class="Input" style="width: 70px; display: none"></select>
          </div>

          <div id="pnlMapTools">
            <button class="btn btn-default dropdown-toggle" type="button" id="btnToolMenu" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
              <span id="selectedTool"><span class="glyphicon glyphicon-hand-up"></span> Pan</span>
              <span class="caret"></span>
            </button>
            <ul id="selectMapTools" class="dropdown-menu" aria-labelledby="btnToolMenu">
              <li class="dropdown-header">Map Tools</li>
              <li id="optPan" class="Button MapTool"><span class="glyphicon glyphicon-hand-up"></span> Pan</li>
              <li id="optIdentify" class="Button MapTool"><span class="glyphicon glyphicon-info-sign"></span> Identify</li>
              <li class="dropdown-header">Selection Tools</li>
              <li id="optSelect" class="Button MapTool"><span class="select"></span> Select Features</li>
              <li class="dropdown-header">Markup Tools</li>
              <li id="optDrawPoint" class="Button MapTool MarkupTool Disabled"><span class="draw-point"></span> Draw Point</li>
              <li id="optDrawLine" class="Button MapTool MarkupTool Disabled"><span class="draw-line"></span> Draw Line</li>
              <li id="optDrawPolygon" class="Button MapTool MarkupTool Disabled"><span class="draw-polygon"></span> Draw Polygon</li>
              <li id="optDrawCircle" class="Button MapTool MarkupTool Disabled"><span class="draw-circle"></span> Draw Circle</li>
              <li id="optDrawText" class="Button MapTool MarkupTool Disabled"><span class="draw-text"></span> Draw Text</li>
              <li id="optDrawCoordinates" class="Button MapTool MarkupTool Disabled"><span class="draw-coordinates"></span> Draw Coordinates</li>
              <li id="optDrawLength" class="Button MapTool MarkupTool Disabled"><span class="draw-length"></span> Draw Measured Length</li>
              <li id="optDrawArea" class="Button MapTool MarkupTool Disabled"><span class="draw-area"></span> Draw Measured Area</li>
              <li id="optDeleteMarkup" class="Button MapTool MarkupTool Disabled"><span class="delete-markup"></span> Delete Markup</li>
              <li id="optColorPicker" class="Button MapTool MarkupTool Disabled"><span class="color-picker"></span> Pick Color</li>
              <li id="optPaintBucket" class="Button MapTool MarkupTool Disabled"><span class="paint-bucket"></span> Fill With Color</li>
            </ul>
          </div>

<%--            <div id="pnlOverviewSizer">
              <div id="pnlOverview">
                <div id="mapOverview"></div>
              </div>
            </div>--%>

          <div id="mapMain" runat="server" class="Panel">
          </div>
          <div id="mapTip" style="display: none"></div>
          <div id="progress" style="display: none">
            <div id="progressBar"></div>
          </div>
        </div>
      </div>

      <div id="pnlDataDisplay" class="Panel">
        <div class="DataHeader">Details<span class="glyphicon glyphicon-menu-right DataExit" aria-hidden="true"></span></div>
          <div id="pnlData" class="Panel">
          <span class="DataLabel">Data Set</span>
          <select id="ddlDataTheme">
          </select><br>
            <button id="cmdDataPrint" class="Disabled">Print</button>
            <div id="pnlDataList" class="Panel"></div>
          </div>
      </div>

    </div>
    <div id="pnlFooter" class="Panel"><uc1:Footer ID="Footer1" runat="server" /></div>
    <form id="frmSaveMap" method="post" action="Services/MapImage.ashx">
      <input type="hidden" name="m" />
      <input type="hidden" name="state" />
      <input type="hidden" name="width" />
      <input type="hidden" name="height" />
    </form>
    <form id="frmPrint" method="post" action="PrintableMap.aspx" target="print">
      <input type="hidden" name="state" />
      <input type="hidden" name="width" />
    </form>
  </div>
</body>
</html>
