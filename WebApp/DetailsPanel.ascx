﻿﻿<%-- 
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
<div class="topHead FunctionHeader"> <%--FunctionHeader class for changing Header Background Color and Font color--%>
  <div class="inner">
      <span class="topLftTxt ">
        <span class="topTxt">Details</span>
        <a href="#">
        </a>
      </span>
      <span class="rightCol">
        <i class="fa fa-angle-left FunctionExit" aria-hidden="true"></i>
      </span>
  </div>
</div>
<div class="frm_box">
  <div id="pnlData" class="frm_box">
    <div class="frm_row noTopMargin customDetails">
      <select id="ddlMobDataTheme" class="frmSelect" title="Data category"></select>
      <span class="dtlPrint">
        <i id="cmdMobDataPrint" class="fa fa-print printRght_icon" aria-hidden="true"></i>
      </span>
    </div>
    <div class="frm_box  topMargin">
      <div id="MobDetailContent" class="customScroll">
        <div id="pnlMobDataList" class="data_box dataTable"></div>
      </div>
    </div>
  </div>
</div>
