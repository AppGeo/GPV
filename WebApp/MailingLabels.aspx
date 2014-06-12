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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MailingLabels.aspx.cs" Inherits="MailingLabels" %>

<!DOCTYPE html">

<html>
<head id="head" runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Create Mailing Labels</title>
  <link href="Styles/Common.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
  <link href="Styles/MailingLabels.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="Scripts/jquery-1.7.2.min.js"></script>
  <script type="text/javascript" src="Scripts/MailingLabels.js"></script>
</head>
<body>
  <form id="form1" runat="server">
    <div id="pnlMain" class="MainPanel">
      <div class="Title" style="left: 20px; top: 14px">Create Mailing Labels</div>
      <div class="Label" style="top: 43px">Manufacturer</div>
      <asp:DropDownList id="ddlManufacturer" runat="server" Height="19px" Width="146px" style="left: 139px; top: 40px" />
      <div class="Label" style="top: 63px">Model No</div>
      <asp:DropDownList id="ddlModelNo" runat="server" Height="19px" Width="146px" style="left: 139px; top: 60px" />
      <div class="Label" style="top: 95px">Label Size</div>
      <div id="labLabelSize" runat="server" style="position: absolute; left: 139px; top: 95px"></div>
      <div class="Label" style="top: 116px">Labels Across</div>
      <div id="labLabelsAcross" runat="server" style="position: absolute; left: 139px; top: 116px"></div>
      <div class="Label" style="top: 137px">Print Direction</div>
      <div id="optPrintAcross" runat="server" class="PrintDirection Selected" data-value="across" style="left: 140px; top: 138px"></div>
      <div id="optPrintDown" runat="server" class="PrintDirection" data-value="down" style="left: 187px; top: 138px"></div>
      <div class="Label" style="top: 208px">Text Font</div>
      <asp:DropDownList id="ddlTextFont" runat="server" Width="146px" Height="19px" style="left: 139px; top: 205px">
        <asp:ListItem Value="Courier" />
        <asp:ListItem Value="Helvetica" />
        <asp:ListItem Value="Times-Roman" Selected="True" />
      </asp:DropDownList>
      <div class="Label" style="top: 229px">Text Size</div>
      <asp:DropDownList id="ddlTextSize" runat="server" Width="66px" Height="19px" style="left: 139px; top: 226px">
        <asp:ListItem Value="6" />
        <asp:ListItem Value="7" />
        <asp:ListItem Value="8" Selected="True" />
        <asp:ListItem Value="9" />
        <asp:ListItem Value="10" />
        <asp:ListItem Value="12" />
        <asp:ListItem Value="14" />
        <asp:ListItem Value="16" />
      </asp:DropDownList>
      <asp:Button id="cmdCreate" runat="server" Text="Create" OnClick="cmdCreate_Click" CssClass="Button" style="left: 139px; top: 258px" />
    </div>
    <asp:HiddenField ID="hdnPrintDirection" runat="server" Value="across" />
  </form>
</body>
</html>
