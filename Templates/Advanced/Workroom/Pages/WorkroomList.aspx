<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="WorkroomList.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.WorkroomList" %>

<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR WorkroomList">
        <%-- WORKROOM LIST --%>
        <asp:Panel runat="server" ID="WorkroomListPanel">
            <div class="buttonToolbar">
                <asp:LinkButton ID="ShowCreatePanelButton" CssClass="linkButton createButton" OnClick="ToggleView_Click"
                    runat="server" Text="<%$ Resources: EPiServer, workroom.workroomlist.addworkroom %>"
                    Visible="false" CausesValidation="false" />
            </div>
            <h1>
                <%= Translate("/workroom/workroomlist/viewheading") %>
            </h1>
            <EPiServer:PageList runat="server" ID="WorkroomPageList" DataSource='<%# Workrooms %>'
                Paging="true" PagesPerPagingItem="10">
                <HeaderTemplate>
                    <ul class="workroomList">
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href="<%# Container.CurrentPage.LinkURL %>">
                        <img src="<%# Container.CurrentPage["WorkroomIcon"] ?? "../Styles/Images/workroom_default.png" %>"
                            alt="<%# Server.HtmlEncode(Container.CurrentPage.PageName) %>" /><%# Server.HtmlEncode(Container.CurrentPage.PageName) %></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </EPiServer:PageList>
        </asp:Panel>
        <%-- CREATE WORKROOM FORM --%>
        <asp:Panel runat="server" ID="NewWorkroomPanel">
            <h1>
                <%= Translate("/workroom/workroomlist/createheading") %>
            </h1>
            <div class="createWorkroom">
                <asp:ValidationSummary CssClass="validation" runat="server" />
                <div>
                    <asp:Label runat="server" AssociatedControlID="NewWorkroomName" Text="<%$ Resources: EPiServer, workroom.workroomlist.namelabel %>" />
                    <asp:TextBox runat="server" CssClass="name" ID="NewWorkroomName" MaxLength="255" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewWorkroomName" EnableClientScript="true"
                        Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.workroomlist.nameempty %>" />
                    <asp:CustomValidator runat="server" ControlToValidate="NewWorkroomName" Text="*"
                        ErrorMessage="<%$ Resources: EPiServer, workroom.workroomlist.duplicatename %>"
                        ID="WorkroomNameValidator" OnServerValidate="ValidateName" />
                    <asp:RegularExpressionValidator ID="NameValidation" runat="server" ControlToValidate="NewWorkroomName"
                        ValidationExpression="[^/?<>\\:*|&quot;#]*" Text="*" ErrorMessage="<%$ Resources: EPiServer, filemanager.validation.illigalchar %>" />
                </div>
                <div>
                    <asp:Label runat="server" AssociatedControlID="NewWorkroomDescription" Text="<%$ Resources: EPiServer, workroom.workroomlist.descriptionlabel %>" />
                    <div class="richTextEditor">
                        <AlloyTech:RichTextEditor ID="NewWorkroomDescription" runat="server" Advanced="true" />
                    </div>
                </div>
                <div>
                    <asp:Label runat="server" AssociatedControlID="SelectTemplateDropDown" Text="<%$ Resources: EPiServer, workroom.workroomlist.templatelabel %>" />
                    <asp:DropDownList runat="server" ID="SelectTemplateDropDown" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="SelectTemplateDropDown"
                        EnableClientScript="true" Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.workroomlist.templateempty %>" />
                </div>
                <asp:Label runat="server" CssClass="templateDescription" ID="DescriptionLabel" />
                <div class="buttons">
                    <asp:Button ID="ButtonCreate" CssClass="buttonExt" runat="server" Text="<%$ Resources: EPiServer, button.create %>"
                        OnClick="CreateNewWorkroom_Click" />
                    <asp:Button CssClass="button" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>"
                        OnClick="ToggleView_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SecondaryBodyRegion">
    <div id="SecondaryBody">
        <EPiServer:Property runat="server" CustomTagName="h2" PropertyName="PageName" />
        <EPiServer:Property runat="server" PropertyName="MainBody" />
    </div>
</asp:Content>
