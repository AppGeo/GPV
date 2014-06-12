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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Hash.aspx.cs" Inherits="Admin_Hash" %>

<html>
<head runat="server">
  <title>Hash Utility</title>
  <script language="JavaScript">
    function clearHash() {
      var tboHash = document.getElementById("tboHash");
      tboHash.value = "";
    }
  </script>
  <style>
    body, td, input
    {
      font-family: Verdana, Arial, Helvetica, Sans-Serif;
      font-size: 11px;
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
  <div>
    <span style="font-weight: bold">Hash Utility</span>
    <div style="margin-left: 10px; margin-top: 10px">
      <div style="margin-bottom: 5px">Generate hash for a password to be stored</div>
      <asp:RadioButton ID="optDatabase" runat="server" GroupName="HashType" Text="in the GPVUser table" Checked="true" /><br />
      <asp:RadioButton ID="optWebConfig" runat="server" GroupName="HashType" Text="in Web.config" />
    </div>
    <table style="margin-left: 10px; margin-top: 10px">
      <tr>
        <td>
      </td>
      </tr>
      <tr>
        <td style="text-align: right">Password</td>
        <td><asp:TextBox ID="tboPassword" runat="server" Width="350px" onkeyup="clearHash()" /></td>
        <td><asp:Button ID="cmdHash" runat="server" Text="Hash" Width="50px" /></td>
      </tr>
      <tr>
        <td style="text-align: right">Hash</td>
        <td><asp:TextBox ID="tboHash" runat="server" Width="350px" ReadOnly="true" /></td>
        <td></td>
      </tr>
    </table>
  </div>
  </form>
</body>
</html>
