<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    AutoEventWireup="false" CodeBehind="MembershipManager.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.MembershipManager" %>

<%@ Register TagPrefix="Workroom" TagName="MembershipAdministration" Src="../Units/MembershipAdministration.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
    <div id="MainBodyAreaWide" class="WR">
        <Workroom:MembershipAdministration runat="server" />
    </div>
</asp:Content>
