<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    AutoEventWireup="False" CodeBehind="Events.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.EventsPage" %>

<%@ Register TagPrefix="AlloyTech" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="XForm" Src="~/Templates/AlloyTech/Units/Placeable/XForm.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="SpeakerLocation" Src="~/Templates/AlloyTech/Units/Placeable/EventLocation.ascx" %>

<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <AlloyTech:MainBody runat="server" />
        <AlloyTech:XForm runat="server" XFormProperty="XForm" HeadingProperty="XFormHeading"
            ShowStatistics="false" />
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="SecondaryBodyRegion" runat="server">
    <div id="SecondaryBody">
        <AlloyTech:SpeakerLocation runat="server" />
        <hr />
        <EPiServer:Property PropertyName="SecondaryBody" EnableViewState="false" runat="server" />
    </div>
</asp:Content>
