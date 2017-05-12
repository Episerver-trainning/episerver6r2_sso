<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NewsList.ascx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Units.NewsList" %>
<%@ Import Namespace="EPiServer.Templates.Advanced.Workroom.Core" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>
<div class="WRNewsList WRList">
    <div class="buttonToolbar">
        <asp:LinkButton ID="AddButton" CssClass="linkButton createButton" runat="server" Text="<%$ Resources: EPiServer, workroom.newslistpage.addnewsitem %>"
            OnClick="AddButton_Click" Visible="<%# Page as EPiServer.Templates.Advanced.Workroom.Pages.NewsListPage != null %>" />
    </div>
    <h2 id="NewsListHeading" runat="server">
    </h2>
    <asp:Panel runat="server" ID="NewsListContainer">
        <EPiServer:PageList ID="NewsListing" SortOrder="PublishedDescending" runat="server">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li class="<%# Container.CurrentPage.PageLink.CompareToIgnoreWorkID(SelectedPageId) && ShowPostBackLinks ? "selected" : "" %>">
                    <h3>
                        <asp:LinkButton OnCommand="LinkButton_Command" CommandArgument='<%# Container.CurrentPage.PageLink.ID %>'
                            ToolTip="<%$ Resources: EPiServer, navigation.readmore %>" runat="server" Visible='<%# ShowPostBackLinks %>'>
		                            <EPiServer:Property PropertyName="PageName" runat="server" />
                        </asp:LinkButton>
                        <asp:HyperLink NavigateUrl='<%# Container.CurrentPage.GetNewsItemNavigateUrl() %>'
                            ToolTip="<%$ Resources: EPiServer, navigation.readmore %>" runat="server" Visible='<%# !ShowPostBackLinks %>'>
		                            <EPiServer:Property PropertyName="PageName" runat="server" />
                        </asp:HyperLink>
                    </h3>
                    <span class="datetime">
                        <%# Container.CurrentPage.GetFormattedPublishDate() %></span>
                    <p>
                        <%# Container.CurrentPage.GetPreviewText(PreviewTextLength) %>
                    </p>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </EPiServer:PageList>
    </asp:Panel>
</div>
