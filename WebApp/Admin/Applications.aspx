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

<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" CodeFile="Applications.aspx.cs" Inherits="Admin_Applications" Title="Applications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div id="pnlApplications" class="DataGridScroll">
    <asp:DataGrid id="grdApplications" runat="server" CssClass="DataGrid" Visible="False" AutoGenerateColumns="False"
      CellPadding="0" GridLines="None">
      <HeaderStyle CssClass="DataGridHeader" />
      <ItemStyle CssClass="DataGridRowAlternate" />
      <AlternatingItemStyle CssClass="DataGridRow" />
      <Columns>
        <asp:HyperLinkColumn DataNavigateUrlField="ApplicationID" DataNavigateUrlFormatString="~/Viewer.aspx?application={0}"
          DataTextField="ApplicationID" Target="GPV" HeaderText="ID">
          <HeaderStyle Width="100px"></HeaderStyle>
        </asp:HyperLinkColumn>
        <asp:BoundColumn DataField="DisplayName" HeaderText="Name">
          <HeaderStyle Width="200px"></HeaderStyle>
        </asp:BoundColumn>
        <asp:BoundColumn DataField="About" HeaderText="Description">
          <HeaderStyle Width="460px"></HeaderStyle>
        </asp:BoundColumn>
      </Columns>
    </asp:DataGrid>
  </div>
</asp:Content>

