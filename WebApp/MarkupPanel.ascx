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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarkupPanel.ascx.cs" Inherits="MarkupPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>

<div class="topHead FunctionHeader">
  <%--FunctionHeader class for changing Header Background Color and Font color--%>
  <div class="inner">
    <span class="topLftTxt ">
      <span class="topTxt">Draw</span>
      <a   class ="helpIcon"  type="draw">
         <i class=" fa fa-question-circle" aria-hidden="true"></i>
      </a>
    </span>
    <span class="rightCol">
      <i class="fa fa-angle-left FunctionExit" aria-hidden="true"></i>
    </span>
  </div>
</div>
<div class="drawCntWrap customScroll">
  <div id="pnlMarkupContent" class=" frm_box markup">
    <div class="frm_row ">
      <div class="twoCol">
        <button id="btnQuickSketch" value="Search" class="btn frmBtn tabBtn tab1 active">Quick Sketch</button>
      </div>
      <div class="twoCol">
        <button id="btnCreateMarkup" value="Search" class="btn frmBtn tabBtn tab2">Create Markup</button>
      </div>
    </div>
    <div class="frm_row customDrawTools">
      <dl id="sample" class="dropdown">
        <dt id="btnMarkupToolMenu"><a href="#"><span id="selectedMarkupTool"><span class="imgflag draw-line"></span>Draw Line</span></a></dt>
        <dd>
          <ul id="selectMarkupTools">
            <li id="optDrawTitle" runat="server" clientidmode="static" class=" toolTitle">Draw Tools - </li>
            <li id="optDrawPoint" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-point"></span>Draw Point</a></li>
            <li id="optDrawLine" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-line "></span>Draw Line</a></li>
            <li id="optDrawPolygon" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-polygon"></span>Draw Polygon</a></li>
            <li id="optDrawCircle" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-circle"></span>Draw Circle</a></li>
            <li id="optDrawText" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-text"></span>Draw Text</a></li>
            <li id="optDrawCoordinates" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-coordinates"></span>Draw Coordinates</a></li>
            <li id="optDeleteMarkup" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag delete-markup"></span>Delete Markup</a></li>
            <li id="optColorPicker" runat="server" clientidmode="static" class=" MapTool  "><a href="#"><span class="imgflag color-picker"></span>Pick Color</a></li>
            <li id="optPaintBucket" runat="server" clientidmode="static" class="MapTool "><a href="#"><span class="imgflag paint-bucket"></span>Fill With Color</a></li>
            <li id="optMeasureTitle" runat="server" clientidmode="static" class=" toolTitle">Measure Tools - </li>
            <%--  Measure Tool Start--%>
            <li id="optDrawLength" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-length"></span>Draw Measured Length</a></li>
            <li id="optDrawArea" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-area"></span>Draw Measured Area</a></li>
            <li id="optDrawBearing" runat="server" clientidmode="static" class=" MapTool "><a href="#"><span class="imgflag draw-bearing"></span>Draw Bearing</a></li>
            <%--  Measure Tool End--%>
          </ul>
        </dd>
      </dl>
      <button id="btnMarkupColor" class="btn frmBtn rightMargin btnLarge" title="Color of new markup">
        <span id="cmdMarkupColor" class="Button Color btnSquare"></span><span class="mrkpTxt">Color</span>
      </button>
      <input type="checkbox" id="chkTextGlow" title="Use glow color on text" />
      <button id="btnTextGlowColor" class="btn frmBtn btnLarge" title="Glow color of new text">
        <span id="cmdTextGlowColor" class="Button Color btnSquare grey"></span><span class="mrkpTxt">Text Glow</span>
      </button>
    </div>
    <div id="divMarkupSearch" class="hidden">
      <div class="frm_row topMargin">
        <div class="frmLabel">
          <span>Your Name</span>
        </div>
        <div class="frmField">
          <gpv:Input type="text" id="tboMarkupUser" runat="server" title="User Name" CssClass="frmInput" />
          <br />
        </div>
      </div>
      <div class="frm_row">
        <div class="frmLabel">
          <span>Category</span>
        </div>
        <div class="frmField customCategory">
          <gpv:Select ID="ddlMarkupCategory" runat="server" CssClass="frmSelect" /><br />
        </div>
      </div>
      <div class="frm_row">
        <div class="frmLabel">
          <span>Markup Group</span>
        </div>
        <div class="frmField">
          <div class="fourCol">
            <gpv:Button ID="cmdNewMarkup" runat="server" CssClass="btn frmBtn frmBtnFlex btnControlLock" title="Create a new markup group">New</gpv:Button>
          </div>
          <div class="fourCol">
            <button id="cmdZoomToMarkup" class="btn frmBtn frmBtnFlex btnfix btnControlLock" title="Zoom to the current markup group">Zoom To</button>
          </div>
          <div class="fourCol">
            <button id="cmdDeleteMarkup" class="btn frmBtn frmBtnFlex btnControlLock" title="Delete the current markup group">Delete</button>
          </div>
          <div class="fourCol">
            <button id="cmdExportMarkup" class="btn frmBtn frmBtnFlex btnControlLock" title="Export the current markup group to KML">To KML</button><br />
          </div>
        </div>
      </div>
      <div class="frm_row">
        <div class="frmLabel">
          <span class="checkBoxWrap">
            <gpv:Input type="checkbox" ID="chkMarkupLock" CssClass="frmCheckbox" runat="server" disabled="disabled" Style="visibility: hidden" />
            <gpv:Label ID="labMarkupLock" runat="server" for="chkMarkupLock" Style="visibility: hidden" title="Keep others from editing"><span class="">Title</span></gpv:Label>
          </span>
        </div>
        <div class="frmField">
          <input type="text" id="tboMarkupTitle" class="frmInput btnControlLock" disabled="disabled" placeholder="Title" title="Title of the current markup group" /><br />
        </div>
      </div>
      <div class="frm_row">
        <div class="frmLabel">
          <span class="">Details</span>
        </div>
        <div class="frmField">
          <textarea id="tboMarkupDetails" class="frmTextarea" placeholder="Details" title="Detail of the current markup group"></textarea>
        </div>
      </div>
    </div>
  </div>
  <div class="frm_box septr topMargin hidden" id="divTblWithHead">
    <div class="list_headng noTopMargin divMargin ">
      <span class="divMargin">Markup History </span>
    </div>
    <div class="data_box" id="pnlMarkupGrid">
      <table id="grdMarkup" class="dataTable"></table>
    </div>
  </div>
</div>
