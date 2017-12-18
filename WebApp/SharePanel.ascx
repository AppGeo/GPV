﻿<%-- 
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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SharePanel.ascx.cs" Inherits="SharePanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>

<div class="topHead">
  <div class="inner">
    <div class="FunctionHeader">
      <span class="topLftTxt ">
        <span class="topTxt">Share</span>
        <a class ="helpIcon" type ="share">
         </a>
      </span>
      <span class="rightCol">
        <a class="prev_arrw FunctionExit" aria-hidden="true"></a>
      </span>
    </div>
  </div>
</div>
<div id="pnlShareContent" class="frm_box">
  <div class="frm_row">
    <div class="threeCol">
      <button id="cmdForPrint" value="Search" class="share-type btn frmBtn btnPrint rightMargin" title="Create a printable map">Print</button>
    </div>
    <div class="threeCol">
      <button id="cmdForExport" value="Search" class="share-type btn frmBtn LocPrint" title="See this area in other maps">Go To</button>
    </div>
    <div class="threeCol">
      <button id="cmdForDownload" value="Search" class="share-type btn frmBtn exprtPrint" title="Save this map as an image or KML">Export</button>
    </div>
  </div>
  <div id="pnlPrint" class="share">
    <div class="share_box">
      <span class="seg_title">Create a Printable PDF Map</span>
      <form id="frmPrint" method="post" action="PrintableMap.ashx" target="print">
        <div class="frm_row">
          <div class="frmLabel">
            Format
          </div>
          <div class="frmField customPrint">
            <gpv:Select ID="ddlPrintTemplate" runat="server" name="template" CssClass="frmSelect">
            </gpv:Select>
          </div>
        </div>
        <gpv:Div ID="pnlPrintInputs" CssClass="frm_row" runat="server"></gpv:Div>
        <div class="frm_row">
          <div class="frmLabel">
            Scale
          </div>
          <div class="frmField">
            <input type="radio" id="optPrintScaleCurrent" class="frmRadio" name="scalemode" value="scale" checked="checked" />
            <label id="labPrintScaleCurrent" for="optPrintScaleInput" class="PrintOptionLabel"><span>Current</span></label>
          </div>
        </div>
        <div class="frm_row topMargin">
          <div class="frmLabel"></div>
          <div class="frmField">
            <input type="radio" id="optPrintScaleWidth" class="frmRadio" name="scalemode" value="width" />
            <label id="labPrintScaleWidth" for="optPrintScaleWidth" class="PrintOptionLabel">Preserve width</label>
          </div>
        </div>
        <div class="frm_row topMargin">
          <div class="frmLabel"></div>
          <div class="frmField">
            <span class="inlineCol">
              <input type="radio" id="optPrintScaleInput" class="frmRadio" name="scalemode" value="input" /></span>
            <span class="inlineCol">
              <label id="labPrintScaleInput" for="optPrintScaleInput">1" =</label></span>
            <span class="inlineCol">
              <input type="text" id="tboPrintScale" name="scale" class="frmInput ratting" placeholder="100" /></span>
            <span class="inlineCol">ft    </span>
          </div>
          <input type="hidden" name="state" />
          <input type="hidden" name="width" />
        </div>
        <div class="frm_row">
          <div class="frmLabel"></div>
          <div class="frmField">
            <button id="cmdPrint" class="btn frmBtn rightMargin">Create</button>
          </div>
        </div>
      </form>
    </div>
  </div>
  <div id="pnlExport" class="share">
    <div class="share_box">
      <span class="seg_title">See Area in Another Map</span>
      <div class="frm_row">
        <div class="twoCol customGoto">
          <gpv:Select ID="ddlExternalMap" runat="server" CssClass="frmSelect"></gpv:Select>
        </div>
        <div class="twoCol">
          <button id="cmdExternalMap" class="btn frmBtn frmBtn2">Go</button>
        </div>
      </div>
    </div>
  </div>
  <div id="pnlDownload" class="share">
    <div class="share_box">
      <span class="seg_title">Export the Map</span>
      <div class="frm_row">
        <div class="threeCol">
          <span>Save as</span>
        </div>
        <div class="threeCol customExport">
          <select id="ddlSaveMap" class="frmSelect">
            <option value="image">Image</option>
            <option value="kml">KML</option>
          </select>
        </div>
        <div class="threeCol">
          <button id="cmdSaveMap" class="btn frmBtn frmBtn2">Save</button>
        </div>
      </div>
    </div>
  </div>
</div>
