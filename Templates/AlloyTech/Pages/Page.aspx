<%@ Page Language="C#" AutoEventWireup="False" CodeBehind="Page.aspx.cs" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    Inherits="EPiServer.Templates.AlloyTech.Pages.Page" %>

<%@ Register TagPrefix="AlloyTech" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="PageList" Src="~/Templates/AlloyTech/Units/Placeable/PageList.ascx" %>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <AlloyTech:MainBody runat="server" />
        <AlloyTech:PageList PreviewTextLength="200" PageLinkProperty="MainListRoot" MaxCountProperty="MainListCount"
            ShowTopRuler="true" runat="server" />
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="SecondaryBodyRegion" runat="server">
    <div id="SecondaryBody">
        <EPiServer:Property PropertyName="SecondaryBody" DisplayMissingMessage="false" EnableViewState="false"
            runat="server" />
        <AlloyTech:PageList PageLinkProperty="SecondaryListRoot" MaxCountProperty="SecondaryListCount"
            DateProperty="PageStartPublish" runat="server" DisplayDateProperty="SecondaryListDisplayDate"
            ShowTopRuler="true" />
    </div>
</asp:Content>
