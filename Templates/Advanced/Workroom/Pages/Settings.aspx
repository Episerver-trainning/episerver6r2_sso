<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="false" Codebehind="Settings.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.Settings" %>
<%@ Register TagPrefix="Workroom" TagName="ChangeVisiblePages" Src="../Units/ChangeVisiblePages.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR">
        <EPiServer:Property runat="server" PropertyName="PageName" CustomTagName="h1" />
        <Workroom:ChangeVisiblePages ID="AvailableFunctions" PageLink="<%# WorkroomStartPageLink %>" runat="server" OnVisibilityChanged="AvailableFunctions_VisibilityChanged" />
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SecondaryBodyRegion" />
