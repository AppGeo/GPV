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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>

<!DOCTYPE html>

<html>
<head id="head" runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>General Purpose Viewer - Login</title>
  <link href="Styles/Common.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Login.css" type="text/css" rel="stylesheet" />
</head>
<body id="body" runat="server">
  <div id="pnlBody" data-role="page">
    <div id="pnlHeader" data-role="header"><uc1:Header id="Header1" runat="server" Visible="true" /><h1 id="h1" runat="server" visible="false">( mV )</h1></div>
    <div id="pnlContent" data-role="content">
      <form id="Form1" method="post" runat="server">
        <div id="pnlMain" class="MainPanel">
          <div style="margin-left: 20px; margin-top: 20px">
            <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate"
              DisplayRememberMe="False" PasswordLabelText="Password" UserNameLabelText="User Name" TitleText="">
              <TextBoxStyle CssClass="Input" />
              <LabelStyle CssClass="Label" />
              <LoginButtonStyle CssClass="LoginButton" />
              <FailureTextStyle CssClass="Error" />
              <ValidatorTextStyle CssClass="Error" />
            </asp:Login>
          </div>
        </div>
      </form>
    </div>
    <div id="pnlFooter"><uc1:Footer id="Footer1" runat="server" /></div>
  </div>
</body>
</html>
