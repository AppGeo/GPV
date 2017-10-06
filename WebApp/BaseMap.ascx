<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BaseMap.ascx.cs" Inherits="BaseMap" %>
   <%@ Register TagPrefix="gpv" Assembly="App_Code" Namespace="GPV" %>


<div id="pnlBaseMapContent" class="DropdownMainPanel ">
  <div id="pnlBaseMaps" class="DropdownMain ">    
    <gpv:Div id="pnlBaseMapScroll" runat="server" CssClass="DropdownPanel LegendScroll dropOpt_menu"  />
  </div>
</div>