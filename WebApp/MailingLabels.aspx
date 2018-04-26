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
  <link href="Styles/bootstrap.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Common.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
  <link href="Styles/MailingLabels.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="Scripts/jquery-2.1.4.min.js"></script>
  <script type="text/javascript" src="Scripts/bootstrap.js"></script>
  <script type="text/javascript" src="Scripts/MailingLabels.js"></script>
</head>
<body>
  <form id="form1" runat="server">
    <div id="pnlMain" class="Panel">
      <span class="Title">Create Mailing Labels</span><br />
      <span class="Label">Manufacturer</span>
      <asp:DropDownList id="ddlManufacturer" runat="server"/><br />
      <span class="Label">Model No</span>
      <asp:DropDownList id="ddlModelNo" runat="server" /><br />
      <span class="Label">Label Size</span>
      <span id="labLabelSize" runat="server" ></span><br />
      <span class="Label">Labels Across</span>
      <span id="labLabelsAcross" runat="server" ></span><br />
      <span class="Label">Print Direction</span>
      <div id="optPrintAcross" runat="server" class="PrintDirection Selected" data-value="across" ></div>
      <div id="optPrintDown" runat="server" class="PrintDirection" data-value="down" ></div><br />
      <span class="Label">Text Font</span>
      <asp:DropDownList id="ddlTextFont" runat="server" >
        <asp:ListItem Value="Courier" />
        <asp:ListItem Value="Helvetica" />
        <asp:ListItem Value="Times-Roman" Selected="True" />
      </asp:DropDownList><br />
      <span class="Label" >Text Size</span>
      <asp:DropDownList id="ddlTextSize" runat="server" >
        <asp:ListItem Value="6" />
        <asp:ListItem Value="7" />
        <asp:ListItem Value="8" Selected="True" />
        <asp:ListItem Value="9" />
        <asp:ListItem Value="10" />
        <asp:ListItem Value="12" />
        <asp:ListItem Value="14" />
        <asp:ListItem Value="16" />
      </asp:DropDownList><br />
      <asp:Button id="cmdCreate" runat="server" Text="Create" OnClick="cmdCreate_Click" CssClass="Button" />
    </div>
    <asp:HiddenField ID="hdnPrintDirection" runat="server" Value="across" />
  </form>
</body>
</html>
