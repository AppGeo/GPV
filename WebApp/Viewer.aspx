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
  <div id="pnlBody">
    <div id="pnlHeader"><uc1:Header ID="Header1" runat="server" /></div>
    <div id="pnlContent" runat="server">
      <div id="pnlMapSizer" runat="server">
        <div id="pnlMapTabs" class="TabPanel">
          <div class="TabScroll">
            <asp:PlaceHolder id="plhMapTabs" runat="server"></asp:PlaceHolder>
          </div>
        </div>
        <div id="pnlMap" class="MainPanel">
          <div style="left: 10px; right: 12px; height: 24px">
            <div style="position: relative; float: left; width: 40%; height: 100%">
              <div id="zoomBar" runat="server" style="margin-top: 4px"><span id="zoomBarMinus"></span><span id="zoomBarActive"><span id="zoomBarLeft" class="ZoomBar"></span><asp:PlaceHolder ID="plhZoomBar" runat="server" /><span id="zoomBarRight" class="ZoomBar"></span><span id="zoomBarSlider" class="ZoomBar"></span></span><span id="zoomBarPlus"></span></div>
            </div>
            <div style="position: relative; float: left; width: 20%; text-align: center; padding-top: 5px; height: 16px">
              <a id="cmdHelp" runat="server" class="CommandLink" style="margin-right: 15px" title="Show the help pages" href="Help.aspx" target="help">Help</a> <span id="cmdMobile" class="CommandLink" style="margin-left: 15px" title="Launch the mobile version">Mobile</span>
            </div>
            <div style="position: relative; float: left; width: 40%; text-align: right; padding-top: 3px; height: 18px">
              Scale 1" =
              <input type="text" id="tboScale" runat="server" class="Input" style="width: 45px; cursor: default" />
              ft
            </div>
          </div>
          <div style="left: 10px; top: 28px; right: 10px; height: 25px">
            <div id="cmdFullView" class="Button" title="Full View"></div>
            <div id="cmdZoomPrevious" class="Button" title="Back to Previous Extent"></div>
            <div id="cmdZoomSelect" class="Button" title="Zoom to Selected Features"></div>
            <div id="optZoomIn" runat="server" class="Button MapTool" title="Zoom In"></div>
            <div id="optPan" runat="server" class="Button MapTool" title="Pan"></div>

            <div style="left: 38%">
              <span id="labLevel" runat="server" style="display: none">Level</span>
              <select id="ddlLevel" runat="server" class="Input" style="width: 70px; margin-right: 20px; display: none"></select>
            </div>

            <div id="cmdPrint" class="Button" title="Printable Map"></div>
            <div id="cmdEmail" class="Button" title="Email This Page"></div>
            <div id="optIdentify" runat="server" class="Button MapTool" title="Identify"></div>
            <div id="optCoordinates" runat="server" class="Button MapTool" title="Display Coordinates"></div>
            <div id="cmdClearGraphics" class="Button" title="Erase Coordinates"></div>
            <div id="optMeasureArea" runat="server" class="Button MapTool" title="Measure Area"></div>
            <div id="optMeasureLine" runat="server" class="Button MapTool" title="Measure Distance"></div>
          </div>
          <div id="mapMain" runat="server">
            <div id="mapTip" style="display: none"></div>
          </div>
          <div id="waitClock"></div>
          <div id="pnlScaleBar" runat="server">
            <div id="pnlScaleBarBackground"> </div>
            <div id="scaleBar"></div>
            <div id="scaleBarText"></div>
          </div>
          <div style="left: 10px; top: auto; right: 12px; bottom: 0px; height: 24px">
            <div style="position: relative; float: left; width: 42%; height: 100%">
              <select id="ddlExternalMap" runat="server" class="Input" style="width: 200px"></select>
              <a id="cmdExternalMap" href="#" class="CommandLink Disabled" target="external">Go</a>
            </div>
            <div style="position: relative; float: left; width: 28%; text-align: center; padding-top: 2px; height: 20px">
              <span id="spnVersion" runat="server" class="VersionText"></span>&nbsp;&nbsp;
              <a class="VersionText" href="http://www.appgeo.com" target="AppGeo">AppGeo</a>
            </div>
            <div style="position: relative; float: left; width: 30%; text-align: right; height: 100%">
              <span id="cmdSaveMap" class="CommandLink">Save Map</span>
              <select id="ddlSaveMap" runat="server" class="Input" style="width: 90px">
                <option value="image">as Image</option>
                <option value="kml">as KML</option>
              </select>
            </div>
          </div>
        </div>
      </div>
      <div id="pnlFunctionSizer" runat="server">
        <div id="pnlFunctionTabs" class="TabPanel">
          <div class="TabScroll">
            <div id="tabSelection" runat="server" class="Tab Normal" style="display: none">Selection</div>
            <div id="tabLegend" runat="server" class="Tab Normal" style="display: none">Legend</div>
            <div id="tabLocation" runat="server" class="Tab Normal" style="display: none">Location</div>
            <div id="tabMarkup" runat="server" class="Tab Normal" style="display: none">Markup</div>
          </div>
        </div>
        <div id="pnlFunction">
          <div id="pnlSelection" runat="server" class="FunctionPanel" style="display: none">
            <uc1:SelectionPanel ID="ucSelectionPanel" runat="server" />
          </div>
          <div id="pnlLegend" runat="server" class="MainPanel FunctionPanel" style="display: none">
            <uc1:LegendPanel ID="ucLegendPanel" runat="server" />
          </div>
          <div id="pnlLocation" runat="server" class="FunctionPanel" style="display: none">
            <uc1:LocationPanel ID="ucLocationPanel" runat="server" />
          </div>
          <div id="pnlMarkup" runat="server" class="MainPanel FunctionPanel" style="display: none">
            <uc1:MarkupPanel ID="ucMarkupPanel" runat="server" />
          </div>
        </div>
      </div>
      <div id="contentDivider" runat="server"></div>
    </div>
    <div id="pnlFooter"><uc1:Footer ID="Footer1" runat="server" /></div>
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
