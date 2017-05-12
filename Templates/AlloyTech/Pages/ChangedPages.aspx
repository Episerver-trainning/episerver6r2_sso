<%@ Page Language="c#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" Codebehind="ChangedPages.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.ChangedPages" %>
<%@ Register TagPrefix="AlloyTech" TagName="ChangedPages" Src="~/Templates/AlloyTech/Units/Placeable/ChangedPages.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
    <div id="MainBodyArea">
        <div id="MainBody">
            <AlloyTech:MainBody runat="server" />
            <AlloyTech:ChangedPages runat="server" />
        </div>
    </div>
</asp:Content>
