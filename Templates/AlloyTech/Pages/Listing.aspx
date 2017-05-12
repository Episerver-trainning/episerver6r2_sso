<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    AutoEventWireup="false" CodeBehind="Listing.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.Listing" %>

<%@ Register TagPrefix="AlloyTech" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="PageList" Src="~/Templates/AlloyTech/Units/Placeable/PageList.ascx" %>
<asp:Content ID="MainRegionContent" ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <AlloyTech:MainBody runat="server" />
        <AlloyTech:PageList ID="PageListControl" PageLinkProperty="ListRoot" MaxCountProperty="ListCount"
            ShowTopRuler="true" DisplayDateProperty="DisplayDate" DateProperty="PageStartPublish"
            PreviewTextLengthProperty="SummaryLength" DisplayDocumentLinkProperty="DisplayDocumentLink"
            DocumentLinkProperty="DocumentInternalPath" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="SecondaryRegionContent" ContentPlaceHolderID="SecondaryBodyRegion"
    runat="server">
    <div id="SecondaryBody">
        <EPiServer:Property runat="server" EnableViewState="false" PropertyName="SecondaryBody" />
        <AlloyTech:PageList PageLinkProperty="SecondaryListRoot" MaxCountProperty="SecondaryListCount"
            DateProperty="PageStartPublish" runat="server" DisplayDateProperty="SecondaryListDisplayDate"
            ShowTopRuler="true" />
    </div>
</asp:Content>
