<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TagCloud.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Blog.Units.TagCloud" %>

<EPiServer:PageList runat="server" SortOrder="Alphabetical" PageLink="<%# TagStartPage %>">
    <HeaderTemplate><ul class="tagCloud"></HeaderTemplate>
    <ItemTemplate>
        <li><%# RenderTag(Container.CurrentPage) %></li>
    </ItemTemplate>
    <FooterTemplate></ul></FooterTemplate>
</EPiServer:PageList>