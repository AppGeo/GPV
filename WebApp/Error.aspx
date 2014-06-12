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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>

<!DOCTYPE html>

<html>
<head id="head" runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>General Purpose Viewer - Error</title>
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
          <div style="position: absolute; left: 20px; top: 20px; width: 569px; height: 121px">
            Our apologies: an error has unexpectedly occurred while starting the viewer.<br />
            <br />
            Please <a id="lnkEmail" runat="server">notify the administrator</a> of this website.&nbsp;
            Provide a brief description of what you were doing when the problem occurred.&nbsp;
            Thank you!
          </div>
        </div>
      </form>
    </div>
    <div id="pnlFooter"><uc1:Footer id="Footer1" runat="server" /></div>
  </div>
</body>
</html>