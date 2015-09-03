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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StartViewer.aspx.cs" Inherits="StartViewer" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>

<!DOCTYPE html>

<html>
<head id="head" runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <script type="text/javascript">
      WebFontConfig = {
        google: { families: ['Lora:400,700:latin'] }
      };
      (function () {
        var wf = document.createElement('script');
        wf.src = ('https:' == document.location.protocol ? 'https' : 'http') +
          '://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js';
        wf.type = 'text/javascript';
        wf.async = 'true';
        var s = document.getElementsByTagName('script')[0];
        s.parentNode.insertBefore(wf, s);
      })(); </script>

  <noscript>
    <link href='http://fonts.googleapis.com/css?family=Lora:400,700' rel='stylesheet' type='text/css'>
  </noscript>
  <script type="text/javascript">
    WebFontConfig = {
      google: { families: ['Open+Sans:400,300,700:latin'] }
    };
    (function () {
      var wf = document.createElement('script');
      wf.src = ('https:' == document.location.protocol ? 'https' : 'http') +
        '://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js';
      wf.type = 'text/javascript';
      wf.async = 'true';
      var s = document.getElementsByTagName('script')[0];
      s.parentNode.insertBefore(wf, s);
    })(); </script>

  <noscript>
    <link href='https://fonts.googleapis.com/css?family=Open+Sans:400,300,700' rel='stylesheet' type='text/css'>
  </noscript>
  <title>General Purpose Viewer</title>
  <link href="Styles/Common.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
  <link href="Styles/StartViewer.css" type="text/css" rel="stylesheet" />
</head>
<body>
  <div id="pnlBody">
    <div id="pnlHeader"><uc1:Header id="Header1" runat="server" /></div>
    <div id="pnlContent">
      <form id="Form1" method="post" runat="server">
        <div id="pnlMain" class="MainPanel">
          <asp:Label id="labMessage" CssClass="LabelText" runat="server" Font-Bold="True"/>
          <div id="pnlApplications" runat="server" class="DataGridScroll">
            <asp:DataGrid id="grdApplications" runat="server" CssClass="DataGrid" Visible="False" AutoGenerateColumns="False"
              CellPadding="0" GridLines="None">
              <HeaderStyle CssClass="DataGridHeader"></HeaderStyle>
              <ItemStyle CssClass="DataGridRow"></ItemStyle>
              <AlternatingItemStyle CssClass="DataGridRowAlternate"></AlternatingItemStyle>
              <Columns>
                <asp:HyperLinkColumn DataNavigateUrlField="ApplicationID" DataNavigateUrlFormatString="Viewer.aspx?application={0}"
                  DataTextField="ApplicationID" HeaderText="ID">
                  <HeaderStyle Width="100px"></HeaderStyle>
                </asp:HyperLinkColumn>
                <asp:BoundColumn DataField="DisplayName" HeaderText="Name">
                  <HeaderStyle Width="200px"></HeaderStyle>
                </asp:BoundColumn>
                <asp:BoundColumn DataField="About" HeaderText="Description"/>
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
