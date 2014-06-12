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

<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" CodeFile="TestStoredProcs.aspx.cs" Inherits="Admin_TestStoredProcs" Title="Test Stored Procs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div id="pnlTestStoredProcs">
    <div id="labMessage" runat="server">Testing stored procedures, please wait ...</div>
    <asp:LinkButton ID="cmdRetest" runat="server" Text="Retest" OnClick="cmdRetest_Click" />
    <div id="pnlStoredProcsScroll" class="DataGridScroll">
      <table id="tblReport" runat="server" class="DataGrid">
      </table>
    </div>
  </div>
</asp:Content>

