<%-- 
  Copyright 2015 Applied Geographics, Inc.

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

<div class="FunctionHeader"><span class="glyphicon glyphicon-menu-left FunctionExit" aria-hidden="true"></span>Share</div>
<div id="pnlShareContent" class="Panel">
  <button id="cmdForPrint" class="share-type" title="Create a printable map"><i class="fa fa-print"></i> Print</button>
  <button id="cmdForEmail" title="Get a link to this map for emailing"><i class="fa fa-share-alt"></i> Email Link</button>
  <button id="cmdForExport" class="share-type" title="See this area in other maps"><i class="fa fa-map-marker"></i> Go To</button>
  <button id="cmdForDownload" class="share-type" title="Save this map as an image or KML"><i class="fa fa-external-link"></i> Export</button>
  <div id="pnlPrint" class="share">
    <div class="FunctionLabel">Create a Printable PDF Map</div><br>
    <form id="frmPrint" method="post" action="PrintableMap.ashx" target="print">
      <label for="ddlPrintTemplate">Format</label>
      <gpv:Select id="ddlPrintTemplate" runat="server" name="template" CssClass="Input">
      </gpv:Select>
      <gpv:Div id="pnlPrintInputs" runat="server"></gpv:Div>
      <label>Scale</label>
      <input type="radio" id="optPrintScaleCurrent" name="scalemode" value="scale" checked="checked" />
      <label id="labPrintScaleCurrent" for="optPrintScaleInput" class="PrintOptionLabel">Current</label><br/>
      <label> </label>
      <input type="radio" id="optPrintScaleWidth" name="scalemode" value="width" />
      <label id="labPrintScaleWidth" for="optPrintScaleWidth" class="PrintOptionLabel">Preserve width</label><br/>
      <label> </label>
      <input type="radio" id="optPrintScaleInput" name="scalemode" value="input" />
      <label id="labPrintScaleInput" for="optPrintScaleInput" class="PrintOptionLabel">1" =</label>
      <input type="text" id="tboPrintScale" name="scale" value="100" style="width: 60px" /> ft<br/>
      <input type="hidden" name="state" />
      <input type="hidden" name="width" />
		  <button id="cmdPrint">Create</button>
    </form>
  </div>
  <div id="pnlEmail" class="share">
    <div class="FunctionLabel">Link for Emailing</div><br/>
    <textarea id="lnkEmail" rows="3" readonly="readonly" style="overflow: hidden" ></textarea>
  </div>
  <div id="pnlExport" class="share">
    <div class="FunctionLabel">See Area in Another Map</div><br>
    <gpv:Select id="ddlExternalMap" runat="server" CssClass="Input" style="width: 200px"></gpv:Select>
    <button id="cmdExternalMap">Go</button>
  </div>
  <div id="pnlDownload" class="share">
    <div class="FunctionLabel">Export the Map</div><br>
    <label for="ddlSaveMap">Save as</label>
    <select id="ddlSaveMap" class="Input" style="width: 90px">
      <option value="image">Image</option>
      <option value="kml">KML</option>
    </select>
    <button id="cmdSaveMap">Save</button>
  </div>
</div>
