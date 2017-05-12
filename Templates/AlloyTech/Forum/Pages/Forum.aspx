<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="Forum.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Forum.Pages.Forum" %>

<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
    <div id="MainBodyAreaWide" class="Forum">
        <div runat="server" id="ButtonPanel" class="buttonToolbar">
            <asp:LinkButton ID="AddForumButton" CssClass="linkButton createButton" runat="server"
                Text="<%$ Resources: EPiServer, forum.forumlist.createbutton %>" OnClick="AddForum_Click"
                Visible="false" />
            <asp:LinkButton ID="AddThreadButton" CssClass="linkButton createButton" runat="server"
                Text="<%$ Resources: EPiServer, forum.threadlist.createbutton %>" OnClick="AddThread_Click"
                Visible="false" />
            <asp:LinkButton ID="DeleteForumButton" CssClass="linkButton deleteButton" runat="server"
                Text="<%$ Resources: EPiServer, forum.forumlist.deleteforumbutton %>" OnClick="Delete_Click"
                Visible="false" />
        </div>
        <EPiServer:Property runat="server" PropertyName="PageName" CustomTagName="h1" />
        <div runat="server" id="ErrorPanel" visible="false">
            <p>
                <%= Translate("/forum/forumlist/setuperror") %>
            </p>
            <asp:Button runat="server" ID="SetupStructureButton" CssClass="button" OnClick="SetupStructure_Click"
                Text="<%$ Resources: EPiServer, forum.forumlist.setupbutton %>" />
        </div>
        <asp:PlaceHolder runat="server" ID="InformationPanel">
            <EPiServer:Property runat="server" PropertyName="MainBody" />
        </asp:PlaceHolder>
        <div runat="server" id="ListForumPanel">
            <h2>
                <%= Translate("/forum/forumlist/heading") %>
            </h2>
            <EPiServer:PageList runat="server" ID="ForumList" DataSource="<%# ForumDataSource %>">
                <HeaderTemplate>
                    <table class="forumTable">
                        <thead>
                            <tr>
                                <th scope="col" class="title">
                                    <%= Translate("/forum/forumlist/title") %>
                                </th>
                                <th scope="col" class="lastUpdated">
                                    <%= Translate("/forum/forumlist/updated") %>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="<%# GetStyle(EvenForum) %>">
                        <td>
                            <%# CreatePageLink(Container.CurrentPage, 55, Container.CurrentPage["Icon"] != null ? "forumLink " + Container.CurrentPage["Icon"].ToString() : string.Empty)%>
                        </td>
                        <td>
                            <%# GetLastUpdatedThreadString(Container.CurrentPage)%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody> </table>
                </FooterTemplate>
            </EPiServer:PageList>
        </div>
        <div runat="server" id="NewForumPanel" visible="false" class="createForum">
            <h2>
                <%= Translate("/forum/forumform/heading") %>
            </h2>
            <asp:ValidationSummary runat="server" CssClass="validation" />
            <div>
                <asp:Label runat="server" AssociatedControlID="ForumTitleTextBox" Text="<%$ Resources: EPiServer, forum.forumform.titlelabel %>" />
                <asp:TextBox runat="server" ID="ForumTitleTextBox" TextMode="SingleLine" MaxLength="255"
                    CssClass="title" /><asp:RequiredFieldValidator runat="server" ControlToValidate="ForumTitleTextBox"
                        Text="*" ErrorMessage="<%$ Resources: EPiServer, forum.forumform.titleerror %>" /><br />
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="IconDropDown" Text="<%$ Resources: EPiServer, forum.forumform.iconlabel %>" />
                <asp:DropDownList runat="server" ID="IconDropDown">
                    <asp:ListItem Text="<%$ Resources: EPiServer, forum.forumform.icondropdown.default %>"
                        Value="default" />
                    <asp:ListItem Text="<%$ Resources: EPiServer, forum.forumform.icondropdown.information %>"
                        Value="information" />
                    <asp:ListItem Text="<%$ Resources: EPiServer, forum.forumform.icondropdown.auction %>"
                        Value="auction" />
                </asp:DropDownList>
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="AllowThreadsCheckBox" Text="<%$ Resources: EPiServer, forum.forumform.allowthreadslabel %>" />
                <asp:CheckBox runat="server" ID="AllowThreadsCheckBox" Checked="true" />
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="AllowForumsCheckBox" Text="<%$ Resources: EPiServer, forum.forumform.allowforumslabel %>" />
                <asp:CheckBox runat="server" ID="AllowForumsCheckBox" Checked="true" />
            </div>
            <div class="buttons">
                <asp:Button runat="server" Text="<%$ Resources: EPiServer, forum.forumform.createbutton %>"
                    OnClick="ForumSubmit_Click" CssClass="buttonExt" />
                <asp:Button runat="server" Text="<%$ Resources: EPiServer, button.cancel %>" OnClick="Cancel_Click"
                    CausesValidation="false" CssClass="button" />
            </div>
        </div>
        <div runat="server" id="ListThreadPanel">
            <h2>
                <%= Translate("/forum/threadlist/heading") %>
            </h2>
            <EPiServer:PageList runat="server" ID="ThreadList" Paging="true" PagesPerPagingItem="20"
                PageLinkProperty="ActiveThreadContainer">
                <HeaderTemplate>
                    <table class="forumTable">
                        <thead>
                            <tr>
                                <th scope="col" class="title">
                                    <%= Translate("/forum/threadlist/title") %>
                                </th>
                                <th scope="col" class="replies">
                                    <%= Translate("/forum/threadlist/replies") %>
                                </th>
                                <th scope="col" class="createBy">
                                    <%= Translate("/forum/threadlist/createdby") %>
                                </th>
                                <th scope="col" class="lastUpdated">
                                    <%= Translate("/forum/threadlist/updated") %>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                            <tr class="<%# GetStyle(EvenThread) %>">
                                <td>
                                    <%# CreatePageLink(Container.CurrentPage, 55, "threadLink") %>
                                </td>
                                <td class="replies">
                                    <EPiServer:Property runat="server" PropertyName="NumberOfReplies" />
                                </td>
                                <td>
                                    <EPiServer:Property runat="server" PropertyName="PageCreatedBy" />
                                </td>
                                <td>
                                    <%# GetDateString(Container.CurrentPage) %>
                                </td>
                            </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </EPiServer:PageList>
        </div>
        <div runat="server" id="NewThreadPanel" visible="false" class="createThread">
            <h2>
                <%= Translate("/forum/threadform/heading") %>
            </h2>
            <asp:ValidationSummary runat="server" CssClass="validation" />
            <div>
                <asp:Label runat="server" AssociatedControlID="TitleTextBox" Text="<%$ Resources: EPiServer,forum.threadform.titlelabel %>" />
                <asp:TextBox runat="server" ID="TitleTextBox" TextMode="SingleLine" MaxLength="255"
                    CssClass="title" /><asp:RequiredFieldValidator runat="server" ControlToValidate="TitleTextBox"
                        Text="*" ErrorMessage="<%$ Resources: EPiServer,forum.threadform.titleerror %>" />
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="ContentTextBox" Text="<%$ Resources: EPiServer,forum.threadform.contentlabel %>" />
                <div class="text">
                    <div class="RichTextEditor">
                        <AlloyTech:RichTextEditor ID="ContentTextBox" runat="server" />
                    </div>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorContentTextBox" runat="server"
                        ControlToValidate="ContentTextBox" Text="*" ErrorMessage="<%$ Resources: EPiServer,forum.threadform.contenterror %>"
                        EnableClientScript="false" />
                </div>
            </div>
            <div class="buttons">
                <asp:Button runat="server" ID="CreateThreadButton" Text="<%$ Resources: EPiServer, forum.threadform.createbutton %>"
                    OnClick="ThreadSubmit_Click" CssClass="button" />
                <asp:Button runat="server" Text="<%$ Resources: EPiServer, button.cancel %>" OnClick="Cancel_Click"
                    CausesValidation="false" CssClass="button" />
            </div>
        </div>
    </div>
</asp:Content>
