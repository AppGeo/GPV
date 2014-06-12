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

<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" CodeFile="CheckConfiguration.aspx.cs" Inherits="Admin_CheckConfiguration" Title="Check Configuration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div id="pnlCheckConfiguration">
    <asp:LinkButton ID="cmdRecheck" runat="server" Text="Recheck" OnClick="cmdRecheck_Click" />
    <div id="labMessage" runat="server" style="left: 80px"></div>
    <div id="pnlCheckConfigScroll">
      <table id="tblReport" runat="server" class="Report">
      </table>
    </div>
  </div>
</asp:Content>

