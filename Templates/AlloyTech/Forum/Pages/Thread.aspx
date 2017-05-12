<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="Thread.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Forum.Pages.Thread" %>

<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>
<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
    <div id="MainBodyAreaWide" class="Forum">
        <div runat="server" id="ModeratorPanel" visible="false" class="moderatorPanel">
            <div class="buttonToolbar">
                <asp:HyperLink runat="server" CssClass="linkButton editButton" ID="HyperlinkAddReply"
                    NavigateUrl="#AddReply" Text="<%$ Resources: EPiServer, forum.thread.addreply %>" />
                <asp:LinkButton runat="server" CssClass="linkButton lockButton" ID="LockButton" OnClick="Lock_Click" />
                <asp:LinkButton runat="server" CssClass="linkButton stickyButton" ID="StickyButton"
                    OnClick="Sticky_Click" />
                <asp:LinkButton runat="server" CssClass="linkButton deleteButton" ID="DeleteButton"
                    OnClick="Delete_Click" Text="<%$ Resources: EPiServer, forum.thread.deletethread %>" />
            </div>
            <a href="<%= ForumStartPage.LinkURL %>" class="linkButton upButton">
                <%= Translate("/forum/thread/toforum") %></a>
        </div>
        <EPiServer:Property runat="server" PropertyName="PageName" CustomTagName="h1" />
        <EPiServer:PageList ID="PostList" SortOrder="CreatedAscending" runat="server" PageLinkProperty="PageLink"
            Paging="true" PagesPerPagingItem="20">
            <HeaderTemplate>
                <div class="replyList">
            </HeaderTemplate>
            <ItemTemplate>
                <div id="Reply<%# Container.CurrentPage.PageName %>" class="reply">
                    <EPiServer:Property runat="server" PropertyName="MainBody" CssClass="text" />
                    <div class="info">
                        <asp:Label runat="server" Text="<%$ Resources: EPiServer,forum.thread.postedby %>" />
                        <EPiServer:Property runat="server" PropertyName="PageCreatedBy" ID="PageCreatedByProperty" />
                        <br />
                        <span class="datetime">
                            <%# Container.CurrentPage.Created.ToFormattedDateAndTime()%>
                        </span>
                        <asp:LinkButton runat="server" CssClass="linkButton deleteButton" Text="<%$ Resources: EPiServer,forum.thread.deletereply %>"
                            Visible="<%# IsModerator && !IsFirstReply %>" OnCommand="DeleteReply_Command"
                            CommandArgument="<%# Container.CurrentPage.PageLink %>" OnClientClick="<%# ConfirmScript %>"
                            CausesValidation="false" />
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </EPiServer:PageList>
        <div runat="server" id="ReplyPanel" visible="false">
            <h2>
                <asp:Literal runat="server" Text="<%$ Resources: EPiServer,forum.thread.addreply %>" /></h2>
            <div id="AddReply" class="createReply">
                <asp:ValidationSummary runat="server" CssClass="validation" ValidationGroup="reply" />
                <div>
                    <AlloyTech:RichTextEditor ID="Body" runat="server" />
                    <asp:RequiredFieldValidator ID="ReplyBodyRequiredFieldValidator" runat="server" ControlToValidate="Body"
                        Text="*" ErrorMessage="<%$ Resources: EPiServer,forum.thread.replyempty %>" ValidationGroup="reply"
                        SetFocusOnError="true" EnableClientScript="false" Display="None" />
                </div>
                <asp:Button runat="server" Text="<%$ Resources: EPiServer,forum.thread.reply %>"
                    CssClass="button" ID="ReplyButton" OnClick="Reply_Click" ValidationGroup="reply" />
            </div>
        </div>
    </div>
</asp:Content>
