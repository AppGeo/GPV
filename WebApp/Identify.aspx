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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Identify.aspx.cs" Inherits="Identify" %>

<!DOCTYPE html>

<html>
<head runat="server">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Identify</title>
  <link href="Styles/Common.css" type="text/css" rel="stylesheet" />
  <link href="Styles/Customize.css" type="text/css" rel="stylesheet" />
  <link href="Styles/DataList.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="Scripts/jquery-2.1.4.min.js"></script>
  <script type="text/javascript" src="Scripts/Identify.js"></script>
</head>
<body>
  <div id="labMessage" runat="server" class="Label" style="position: absolute; left: 10px; top: 10px">Invalid parameters, could not perform search.</div>
  <div id="pnlIdentify" runat="server" style="position: absolute; left: 10px; top: 10px; right: 10px; max-width: 600px">
    <div id="cmdPrint" runat="server" class="CommandLink" style="position: absolute; right: 0px; width: 25px">Print</div>
    <div id="pnlContent" runat="server" style="position: absolute; top: 20px"></div>
  </div>
  <div id="pnlPostBack" runat="server">
    <form id="frmPostBack" method="post">
      <input type="hidden" name="postback" value="1" />
    </form>
  </div>
  <div id="autoPrint" runat="server"></div>
</body>
</html>
