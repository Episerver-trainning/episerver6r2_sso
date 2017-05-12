<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="False" CodeBehind="Document.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.Document" %>
<%@ Register TagPrefix="AlloyTech" TagName="MainBody"  Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="Document"  Src="~/Templates/AlloyTech/Units/Placeable/Document.ascx" %>

<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <AlloyTech:MainBody runat="server" />
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="SecondaryBodyRegion" runat="server">
    <div id="SecondaryBody">
        <AlloyTech:Document runat="server" DisplayMissingMessage="true" />
        <EPiServer:Property PropertyName="SecondaryBody" EnableViewState="false" runat="server" />
    </div>
</asp:Content>