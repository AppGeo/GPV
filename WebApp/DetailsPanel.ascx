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
            <select id="ddlMobDataTheme" class="frmSelect" title="Data category">  </select>
                <span class="dtlPrint"> <a id="cmdMobDataPrint" href="#" class="printRght_icon"></a></span>
           
        </div>
        <div class="frm_box  topMargin">
            <div id ="MobDetailContent" class="customScroll">
                <div id="pnlMobDataList" class="data_box dataTable"></div>
            </div>
        </div>
    </div>
</div>
