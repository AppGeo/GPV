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

<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" CodeFile="DownloadMarkup.aspx.cs" Inherits="Admin_DownloadMarkup" Title="Download Markup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div style="left: 20px; top: 20px">
      <asp:DropDownList ID="ddlMarkupCategory" runat="server" CssClass="BodyText" Style="left: 108px;
        position: absolute; top: 2px" Width="236px" AutoPostBack="True">
      </asp:DropDownList>
      <div class="BodyText" style="left: 0px; width: 105px; position: absolute; top: 4px;
        height: 16px; text-align: right">
        Markup Category</div>
      <div class="BodyText" style="left: 0px; width: 105px; position: absolute; top: 26px;
        height: 16px; text-align: right">
        User Name</div>
      <asp:DropDownList ID="ddlUserName" runat="server" CssClass="BodyText" Style="left: 108px;
        position: absolute; top: 24px" Width="236px" AutoPostBack="True">
      </asp:DropDownList>
      <div class="BodyText" style="left: 0px; width: 105px; position: absolute; top: 48px;
        height: 16px; text-align: right">
        Dates</div>
      <asp:RadioButton ID="optDateAll" runat="server" CssClass="BodyText" Style="left: 108px;
        position: absolute; top: 47px" Text="All" AutoPostBack="True" Checked="True" GroupName="DateRange"
        OnCheckedChanged="optDateAll_CheckedChanged" />
      <asp:RadioButton ID="optDateRange" runat="server" CssClass="BodyText" Style="left: 146px;
        position: absolute; top: 47px" Text="Range" AutoPostBack="True" GroupName="DateRange"
        OnCheckedChanged="optDateRange_CheckedChanged" />
      <asp:Calendar ID="calDateFrom" runat="server" BackColor="White" CssClass="BodyText"
        DayNameFormat="FirstLetter" Style="left: 108px; position: absolute; top: 82px"
        Enabled="False" Width="146px" EnableTheming="True">
      </asp:Calendar>
      <asp:Calendar ID="calDateTo" runat="server" BackColor="White" CssClass="BodyText"
        DayNameFormat="FirstLetter" Style="left: 263px; position: absolute; top: 82px"
        Enabled="False" Width="146px">
      </asp:Calendar>
      &nbsp;
      <asp:Label ID="labDateTo" runat="server" CssClass="BodyText" Style="left: 264px;
        position: absolute; top: 66px" Text="To" Width="23px" Enabled="False"></asp:Label>
      <asp:Label ID="labDateFrom" runat="server" CssClass="BodyText" Enabled="False" Style="left: 108px;
        position: absolute; top: 66px" Text="From" Width="31px"></asp:Label>
      <asp:LinkButton ID="cmdDownload" runat="server" CssClass="CommandLink" Style="left: 109px;
        position: absolute; top: 306px" OnClick="cmdDownload_Click" Width="195px">Download to Zipped Shapefiles</asp:LinkButton>
      <asp:Label ID="labNumberFound" runat="server" CssClass="BodyText" Style="left: 109px;
        position: absolute; top: 249px" Width="236px"></asp:Label>
      <div class="BodyText" style="left: 0px; width: 105px; position: absolute; top: 229px;
        height: 16px; text-align: right">
        Markup Group</div>
      <asp:DropDownList ID="ddlMarkupGroup" runat="server" CssClass="BodyText" Style="left: 108px;
        position: absolute; top: 227px" Width="236px" AutoPostBack="True">
      </asp:DropDownList>
      <div class="BodyText" style="left: 0px; width: 105px; position: absolute; top: 286px;
        height: 16px; text-align: right">
        File Prefix</div>
      <asp:TextBox ID="tboFilePrefix" runat="server" AutoPostBack="True" CssClass="BodyText"
        Style="left: 108px; position: absolute; top: 283px" Width="236px"></asp:TextBox>
  </div>
</asp:Content>

