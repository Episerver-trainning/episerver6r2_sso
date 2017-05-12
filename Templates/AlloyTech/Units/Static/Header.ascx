<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="False" CodeBehind="Header.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Static.Header" %>

<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/scripts/jquery/jquery-1.4.4.min.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/Scripts/AlloyTech.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/Scripts/Mobile/Mobile.js") %>"></script>


<link type="text/css" rel="stylesheet" media="screen,projection" href="<%= Page.ResolveUrl("~/Templates/AlloyTech/Styles/Default/Styles.css") %>" />
<link type="text/css" rel="stylesheet" media="only screen and (max-width: 480px)"  href="<%= Page.ResolveUrl("~/Templates/AlloyTech/Styles/Mobile/Styles.css") %>" />
<link type="text/css" rel="stylesheet" media="print" href="<%= Page.ResolveUrl("~/Templates/AlloyTech/Styles/Print/Styles.css") %>" />

<asp:PlaceHolder ID="LinkArea" runat="server" />

<asp:PlaceHolder ID="MetadataArea" runat="server" />

<EPiServer:PageList ID="RssList" PageLinkProperty="RssContainer" runat="server">
    <ItemTemplate>
        <%#GetRss(Container.CurrentPage)%>
    </ItemTemplate>
</EPiServer:PageList>