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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Status.aspx.cs" Inherits="Status" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>

<!DOCTYPE html>

<html>
<head id="head" runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>General Purpose Viewer</title>
  <link href="Styles/Common.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Status.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
</head>
<body>
  <div id="pnlBody">
    <div id="pnlHeader"><uc1:Header id="Header1" runat="server" /></div>
    <div id="pnlContent">
      <form id="Form1" method="post" runat="server">
        <div id="pnlMain" class="MainPanel">
          <asp:Label id="labMessage" CssClass="LabelText" runat="server"/>
          <div id="pnlMapServices">
            <asp:DataGrid id="grdMapServices" runat="server" Visible="False" AutoGenerateColumns="False"
              CellPadding="0" GridLines="None">
              <HeaderStyle Font-Bold="True"></HeaderStyle>
              <Columns>
                <asp:BoundColumn DataField="MapTabID" HeaderText="MapTab">
                  <HeaderStyle Width="150px"></HeaderStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="Status" HeaderText="Map Service Status" />
              </Columns>
            </asp:DataGrid>
          </div>
        </div>
      </form>
    </div>
    <div id="pnlFooter"><uc1:Footer id="Footer1" runat="server" /></div>
  </div>
</body>
</html>