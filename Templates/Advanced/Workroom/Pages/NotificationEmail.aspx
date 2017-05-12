<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="false" CodeBehind="NotificationEmail.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.NotificationEmail" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentRegion" runat="server">
     <EPiServer:Property ID="MailSubject" runat="server" PropertyName="MailSubject" DisplayMissingMessage="true" />
     <EPiServer:Property ID="MailBody" runat="server" PropertyName="MailBody" DisplayMissingMessage="true" />
</asp:Content>
