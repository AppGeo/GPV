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

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DetailsPanel.ascx.cs" Inherits="DetailsPanel" %>
<%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>
<div class="topHead">
  <div class="inner">
    <div class="FunctionHeader">
      <span class="topLftTxt ">
        <span class="topTxt">Details</span>
        <a href="#">
          <img src="Images/faq-icon.png"></a>
      </span>
      <span class="rightCol">
        <a class="prev_arrw FunctionExit" aria-hidden="true"></a>
      </span>
    </div>
  </div>
</div>
<div class="frm_box">
  <div id="pnlData" class="frm_box">
    <div class="frm_row noTopMargin">
      <select id="ddlMobDataTheme" class="frmSelect" title="Data category"></select>
      <span class="dtlPrint"><a id="cmdMobDataPrint" href="#" class="printRght_icon"></a></span>
    </div>
    <div class="frm_box  topMargin">
      <div id="MobDetailContent" class="customScroll">
        <div id="pnlMobDataList" class="data_box dataTable"></div>
      </div>
    </div>
  </div>
</div>
