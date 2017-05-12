<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DateTree.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Blog.Units.DateTree" %>
<div class="dateTree">
    <EPiServer:PageTree runat="server" ExpandAll="true" SortOrder="PublishedDescending"
        PageLink="<%# TreeStart %>">
        <TopTemplate>
            <EPiServer:Property runat="server" PropertyName="PageLink" />
        </TopTemplate>
        <ItemHeaderTemplate>
            <li>
        </ItemHeaderTemplate>
        <IndentTemplate>
            <ul>
        </IndentTemplate>
        <ItemTemplate>
            <a href="<%# Container.CurrentPage.LinkURL %>">
                <%# Container.CurrentPage.StartPublish.ToString("MMMM") %></a>
        </ItemTemplate>
        <ItemFooterTemplate>
            </li></ItemFooterTemplate>
        <UnindentTemplate>
            </ul></UnindentTemplate>
    </EPiServer:PageTree>
</div>
