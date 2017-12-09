<%-- 
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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Viewer.aspx.cs" Inherits="Viewer" EnableViewState="false" EnableSessionState="true" EnableEventValidation="false" %>

<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SearchPanel" Src="SearchPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SelectionPanel" Src="SelectionPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="LegendPanel" Src="LegendPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="LocationPanel" Src="LocationPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MarkupPanel" Src="MarkupPanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SharePanel" Src="SharePanel.ascx" %>
<%@ Register TagPrefix="uc1" TagName="BaseMapPanel" Src="~/BaseMap.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DetailsPanel" Src="DetailsPanel.ascx" %>

<!DOCTYPE html>

<html>
<head id="head" runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>AppGeo GPV</title>
  <script type="text/javascript"> if (typeof (JSON) == "undefined") { location.href = "Incompatible.htm"; } </script>
  <script type="text/javascript" src="Scripts/WebFonts.js"></script>
</head>
<body>

  <div id="pnlBody" class="container-fluid">
    <div id="pnlHeader" class="Panel">
      <header class="main">
        <div class="inner">
          <div class="leftCol">
            <uc1:Header ID="Header1" runat="server" />
          </div>
          <div class="rightCol">
            <ul class="rightLinks">
              <span id="cmdShowDetails" class="glyphicon glyphicon-option-vertical" title="Show/hide details panel" style="font-size: 22px;"></span>
              <a id="cmdHelp" runat="server" class="CommandLink" title="Show the help pages" href="Help.aspx" target="help"><span class="helpTxt ">Help</span></a>
              <span id="cmdEmail" title="Get a link to this map for emailing"><span class="fa fa-link"></span>Get Link</span>
              <div id="pnlEmail" class="input-group">
                <textarea id="lnkEmail" readonly="readonly" class="form-control custom-control" rows="3"></textarea>
                <span id="cmdEmailClose" class="input-group-addon btn">x</span>
              </div>
            </ul>
          </div>
        </div>
      </header>
    </div>
    <div id="pnlContent" runat="server" class="Panel">
      <div id="pnlFunctionSidebar" runat="server" class="Panel leftNav_panel ">
        <div class="inner">
          <div class="topSection">
            <div class="leftCol">
              <a id="btnHamburger" class="hamburger" href="#"></a>
              <a id="btnHamburgerClose" class="hamburgerClose hidden " href="#"></a>
            </div>
            <div class="rightCol"><a class="pinIco" href="#"></a></div>
          </div>
          <div class="leftLinks">
            <div id="pnlFunctionTabs" runat="server" class="TabPanel ">
              <ul>
                <li id="tabSearch" runat="server" class="MenuItem Normal ">
                  <a href="#">
                    <span class="linkIcon">
                      <img src="Images/search_icon.png"></span>
                    <span class="linkTxt ">Search</span>
                  </a>
                </li>
                <li id="tabSelection" runat="server" class="MenuItem Normal">
                  <a>
                    <span class="linkIcon" title="Selection">
                      <img src="Images/selection_icon.png"></span>
                    <span class="linkTxt ">Selection</span>
                  </a>
                </li>
                <li id="tabMobDetails" runat="server" class="MenuItem Normal">
                  <a>
                    <span class="linkIcon" title="Selection">
                      <img src="Images/details-icon.png"></span>
                    <span class="linkTxt ">Details</span>
                  </a>
                </li>
                <li id="tabLegend" runat="server" class="MenuItem Normal">
                  <a href="#">
                    <span class="linkIcon">
                      <img src="Images/legend-icon.png"></span>
                    <span class="linkTxt ">Maps</span>
                  </a>
                </li>
                <li id="tabLocation" runat="server" class="MenuItem Normal">
                  <a href="#">
                    <span class="linkIcon">
                      <img src="Images/loc-icon.png"></span>
                    <span class="linkTxt ">Location</span>
                  </a>
                </li>
                <li id="tabMarkup" runat="server" class="MenuItem Normal">
                  <a href="#">
                    <span class="linkIcon">
                      <img src="Images/markup-icon.png"></span>
                    <span class="linkTxt ">Draw</span>
                  </a>
                </li>
                <li id="tabShare" runat="server" class="MenuItem Normal">
                  <a href="#">
                    <span class="linkIcon">
                      <img src="Images/share_icon.png"></span>
                    <span class="linkTxt ">Share</span>
                  </a>
                </li>
              </ul>
              <div id="pnlAttribution" class="Panel">
                <span id="spnVersion" runat="server" class="VersionText"></span>&nbsp;&nbsp;
                <a id="logosmall" runat="server" class="VersionText" href="http://www.appgeo.com" target="AppGeo">AppGeo</a>
              </div>
            </div>
          </div>
        </div>
        <div class="">
          <div id="pnlFunction" runat="server" class="left_details">
            <%-- Function tab name with  click to return to main menu --%>
            <div id="pnlSearch" runat="server" class="FunctionPanel Panel" style="display: none">
              <uc1:SearchPanel ID="ucSearchPanel" runat="server" />
            </div>
            <div id="pnlSelection" runat="server" class="FunctionPanel Panel" style="display: none">
              <uc1:SelectionPanel ID="ucSelectionPanel" runat="server" />
            </div>
            <div id="pnlDetails" runat="server" class="FunctionPanel Panel" style="display: none">
              <uc1:DetailsPanel ID="ucDetailsPanel" runat="server" />
            </div>
            <div id="pnlLegend" runat="server" class="FunctionPanel Panel" style="display: none">
              <uc1:LegendPanel ID="ucLegendPanel" runat="server" />
            </div>
            <div id="pnlLocation" runat="server" class="FunctionPanel Panel" style="display: none">
              <uc1:LocationPanel ID="ucLocationPanel" runat="server" />
            </div>
            <div id="pnlMarkup" runat="server" class="FunctionPanel Panel markup" style="display: none">
              <uc1:MarkupPanel ID="ucMarkupPanel" runat="server" />
            </div>
            <div id="pnlShare" runat="server" class="FunctionPanel Panel" style="display: none">
              <uc1:SharePanel ID="ucSharePanel" runat="server" />
            </div>
          </div>
        </div>
      </div>
      <div id="pnlMapSizer" runat="server" class="Panel">
        <div id="pnlMap" class="MainPanel Panel">
          <div id="pnlMapMenus" class="MapMenu">
            <div id="pnlBasemapMenu" class="MapMenu">
              <div class="layerImagePanel baseMapImage">
                <label style="width: 100%; height: 100%;">
                  <input id="pnlBaseMap" style="display: none;" type="checkbox" />
                  <img class="layerImage" src="Images/dropdownMap-icon.jpg" title="Overlays and Basemaps" />
                </label>
              </div>
              <div id="pnlBaseLayer" style="display: none; margin-top: 13px">
                <uc1:BaseMapPanel ID="ucBaseMapPanel" runat="server" />
              </div>
            </div>
            <div id="pnlMapTools" class="MapMenu mapNavigation ">
              <div class="selectCol">
                <button class=" dropdown identify" type="button" id="btnToolMenu" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true" title="Tool">
                  <span id="selectedTool">
                    <img class="flag " src="Images/info-icon.png" alt="" />
                    <span class="txt glyphicon">Identify</span></span>
                </button>
                <ul id="selectMapTools" class="dropdown-menu dropdown" aria-labelledby="btnToolMenu">
                  <li id="optIdentify" runat="server" class="Button MapTool"><a href="#">
                    <img class="flag " src="Images/info-icon.png" alt="" />
                    <span class="txt glyphicon">Identify</span></a></li>
                  <li id="optSelect" runat="server" class="Button MapTool"><a href="#">
                    <img class="flag " src="Images/select-icon.png" alt="" />
                    <span class="txt glyphicon">Select</span></a>
                  </li>
                  <li id="optMarkupTool" runat="server" class="Button MapTool"><a href="#">
                    <img class="flag " src="Images/draw-icon.png" alt="" />
                    <span class="txt glyphicon">Draw</span></a></li>
                </ul>
              </div>
            </div>
            <div id="pnlMapLevels" runat="server" class="MapMenu" style="display: none">
              <button class="frmSelect dropdown-toggle" type="button" id="btnMapLevel" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true" title="Level">
                <span id="selectedLevel" runat="server"></span>
              </button>
              <ul id="selectMapLevel" class="dropdown-menu " aria-labelledby="btnMapLevel">
                <asp:PlaceHolder ID="phlMapLevel" runat="server"></asp:PlaceHolder>
              </ul>
            </div>
          </div>
          <div id="pnlOverview" class="overviewInitial">
            <div id="cmdOverview" class="iconWrapper">
              <span id="iconOverview" class="glyphicon glyphicon-triangle-left" title="Show/hide overview map"></span>
            </div>
            <div id="mapOverview">
              <div id="locatorBox" class="UI" style="width: 1px; height: 1px; left: -10px">
                <div id="locatorBoxFill" class="UI">
                </div>
              </div>
            </div>
          </div>
          <div id="mapMain" runat="server" class="Panel">
          </div>
          <div id="mapTip" style="display: none"></div>
          <div id="progress" style="display: none">
            <div id="progressBar"></div>
          </div>
          <a id="logo" runat="server" href="http://www.appgeo.com" target="_blank"></a>
        </div>
      </div>
      <div id="pnlDataDisplay" class="right_details detailNav">
        <div class="DataHeader topHead">
          <div class="inner">
            <span class="topLftTxt">Details
            </span>
            <span class="rightCol">
              <a id="cmdDataPrint" href="#" class="printRght_icon"></a>
              <a href="#" class=" DataExit nxt_arrw"></a>
            </span>
          </div>
        </div>
        <div id="pnlData" class="frm_box">
          <div class="detailBox">
            <div class="frm_row noTopMargin customDetails">
              <select id="ddlDataTheme" class="frmSelect" title="Data category">
              </select>
            </div>
            <div class="frm_box  topMargin">
              <div id="DetailContent" class="customScroll">
                <div id="pnlDataList" class="data_box dataTable"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div id="pnlFooter" class="Panel mainFooter">
        <uc1:Footer ID="Footer1" runat="server" />
      </div>
    </div>
    <form id="frmSaveMap" method="post" action="Services/MapImage.ashx">
      <input type="hidden" name="m" />
      <input type="hidden" name="state" />
      <input type="hidden" name="width" />
      <input type="hidden" name="height" />
    </form>
  </div>
</body>
</html>
