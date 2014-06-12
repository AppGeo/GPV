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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintableMap.aspx.cs" Inherits="PrintableMap" %>

<!DOCTYPE html>

<html>
<head runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Create Printable PDF Map</title>
  <link href="Styles/Common.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
  <link href="Styles/PrintableMap.css" type="text/css" rel="stylesheet" />
</head>
<body>
  <form id="form1" runat="server">
    <div id="pnlMain" runat="server" class="MainPanel">
      <div class="Title" style="left: 20px; top: 14px; width: 207px; height: 18px">Create Printable PDF Map</div>
      <div class="Label" style="top: 43px">Format</div>
      <asp:DropDownList id="ddlPrintTemplate" runat="server" Height="19px" Width="216px" AutoPostBack="true" style="left: 139px; top: 40px" />
      <div id="pnlBottom" runat="server" style="left: 0px; top: 67px; width: 100%">
        <div class="Label" style="top: 1px">Preserve</div>
        <input type="radio" id="optPreserveScale" runat="server" name="Preserve" checked="true" style="left: 139px; top: 0px" />
        <label for="optPreserveScale" style="left: 155px; top: 1px">Scale of map window</label>
	  	  <div id="labPreserveScale" runat="server" style="left: 300px; top: 1px"></div>
        <input type="radio" id="optPreserveWidth" runat="server" name="Preserve" style="left: 139px; top: 20px" />
        <label for="optPreserveWidth" style="left: 155px; top: 21px">Width of map window</label>
	  	  <div id="labPreserveWidth" runat="server" style="left: 300px; top: 21px"></div>
		    <asp:Button id="cmdCreate" runat="server" Enabled="false" Text="Create" CssClass="Button" style="left: 139px; top: 48px" onclick="cmdCreate_Click"/>
      </div>
    </div>
  </form>
</body>
</html>
