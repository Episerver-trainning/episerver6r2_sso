<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemSummary.ascx.cs"
    Inherits="EPiServer.Templates.AlloyTech.Blog.Units.ItemSummary" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>
<div class="blogItem">
    <a href="" id="BloggerLink" runat="server" visible="false">
        <img class="bloggerThumbnail" src="" runat="server" id="BloggerImage" alt="" /></a>
    <EPiServer:Property runat="server" PropertyName="PageLink" CustomTagName="h2" PageLink="<%# SelectedPage.PageLink %>" />
    <span class="bloginfo">
        <%= SelectedPage.GetFormattedPublishDate() %></span> 
        <span runat="server" class="bloginfo" visible="<%# ShowWriter %>">
            <EPiServer:Translate runat="server" Text="/blog/item/postedby" />
            <a href="<%= BlogStart.LinkURL %>">
                <EPiServer:Property runat="server" PropertyName="Writer" CssClass="name" PageLink="<%# BlogStart.PageLink %>" />
            </a></span>
    <p>
        <%= SummaryText %></p>
    <span class="bloginfo">
        <EPiServer:Translate runat="server" Text="/blog/item/commentheading" />
        <%= EPiServer.DataFactory.Instance.GetChildren(SelectedPage.PageLink).Count.ToString() %>
    </span>
    <div runat="server" class="tags" visible="<%# Tags.Count > 0 %>">
        <span class="bloginfo">
            <EPiServer:Translate ID="Translate2" runat="server" Text="/blog/item/tags" />
        </span>
        <EPiServer:PageList runat="server" DataSource="<%# Tags %>">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <EPiServer:Property PropertyName="PageLink" runat="server" />
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </EPiServer:PageList>
    </div>
</div>
