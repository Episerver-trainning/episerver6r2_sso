<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="NewsListPage.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.NewsListPage" %>

<%@ Register TagPrefix="Workroom" TagName="NewsList" Src="../Units/NewsList.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR WRNewsListPage">
        <asp:Panel runat="server" ID="EditNewsSectionButtons" Visible="false" class="buttonToolbar">
            <a class="linkButton createButton" id="AddNewsSectionButton" href="<%= GetPage(CurrentPage.ParentLink).LinkURL.ToString()+"&amp;addmode=true" %>">
                <%=Translate("/workroom/newslistpage/addnewssection")%></a> 
            <a class="linkButton editButton" id="EditNewsSectionNameButton" href="javascript:ShowEditNewsSectionNamePanel()">
                    <%=Translate("/workroom/newslistpage/editnewssection")%></a>
            <asp:LinkButton ID="DeleteNewsSectionLinkButton" CssClass="linkButton deleteButton" runat="server"
                Text="<%$ Resources: EPiServer, workroom.newslistpage.deletenewssection %>" OnClick="DeleteNewsSection_Click" />
        </asp:Panel>
        <EPiServer:Property ID="PageNameProperty" CustomTagName="h1" PropertyName="PageName"
            runat="server" />
        <asp:Panel ID="EditNewsSectionNamePanel" class="hidden" runat="server" Visible="false">
            <br />
            <asp:TextBox runat="server" ID="NewsSectionNameTextBox" CssClass="" MaxLength="255"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="NewsSectionNameTextBox"
                ValidationGroup="NewsSectionName" Text="*"></asp:RequiredFieldValidator>
            <asp:Button runat="server" ID="SaveNewsSectionNameButton" CssClass="buttonExt" Text="<%$ Resources: EPiServer, button.save %>"
                OnClick="SaveNewsSectionNameButton_Click" ValidationGroup="NewsSectionName" />
            <span class="WRbuttonsDiv">&nbsp;</span>
            <input type="button" id="cancelNewsSectionNameButton" class="button" value="<%= Translate("/button/cancel") %>"
                onclick="javascript:HideEditNewsSectionNamePanel()" />
        </asp:Panel>
        <asp:Panel ID="ViewPanel" runat="server" CssClass="viewPanel">
            <div class="buttonToolbar">
                <asp:LinkButton ID="EditButton" CssClass="linkButton editButton" runat="server" Text="<%$ Resources: EPiServer, button.edit %>"
                    OnClick="Edit_Click" />
                <asp:LinkButton ID="DeleteButton" CssClass="linkButton deleteButton" runat="server" Text="<%$ Resources: EPiServer, button.delete %>"
                    OnClick="Delete_Click" />
            </div>
            <EPiServer:Property ID="PageNameProp" PropertyName="PageName" CustomTagName="h2"
                runat="server" />
            <EPiServer:Property ID="MainBodyProp" PropertyName="MainBody" runat="server" />
        </asp:Panel>
        <asp:Panel ID="EditPanel" runat="server" Visible="false" CssClass="editPanel">
            <h2 runat="server" id="EditPanelCaption">
                <asp:Literal Text="<%$ Resources: EPiServer, workroom.newslistpage.editheading %>"
                    runat="server" /></h2>
            <div class="createNews">
                <asp:ValidationSummary CssClass="validation" runat="server" />
                <div>
                    <asp:Label ID="EditPageNameLabel" Text="<%$ Resources: EPiServer, workroom.newslistpage.headinglabel %>"
                        AssociatedControlID="EditPageName" runat="server" /><br />
                    <asp:TextBox ID="EditPageName" runat="server" MaxLength="255" CssClass="name" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="EditPageName" EnableClientScript="true"
                        ValidationGroup="EditNewsItemValidationGroup" Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.newslistpage.titleempty %>" />
                </div>
                <div>
                    <asp:Label ID="EditMainBodyLabel" Text="<%$ Resources: EPiServer, workroom.newslistpage.bodylabel %>"
                        AssociatedControlID="EditMainBody" runat="server" /><br />
                    <AlloyTech:RichTextEditor ID="EditMainBody" runat="server" Advanced="true" />
                </div>
                <div class="buttons">
                    <asp:Button ID="EditButtonSave" CssClass="buttonExt" runat="server" Text="<%$ Resources: EPiServer, button.save %>"
                        OnClick="Save_Click" ValidationGroup="EditNewsItemValidationGroup" />
                    <asp:Button ID="EditButtonCancel" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>"
                        OnClick="Cancel_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        var initialValue = "";
        $(document).ready(function() { $("#NewsContainer  > .containerContent").show() });
        function ShowEditNewsSectionNamePanel() {
            $("#viewNewsSectionName").hide();
            $("#<%=EditNewsSectionNamePanel.ClientID %>").show();
            var sectionNameTextBoxId = "#<%=NewsSectionNameTextBox.ClientID %>";
            initialValue = $(sectionNameTextBoxId).val();
            $(sectionNameTextBoxId).select();
            $(sectionNameTextBoxId).keydown(EditNewsSectionNameKeypress);
            $("#EditNewsSectionNameButton").hide();
        }
        function HideEditNewsSectionNamePanel() {
            $("#viewNewsSectionName").show();
            $("#<%=EditNewsSectionNamePanel.ClientID %>").hide();
            $("#EditNewsSectionNameButton").show();
            $("#<%=NewsSectionNameTextBox.ClientID %>").val(initialValue);
        }
        function EditNewsSectionNameKeypress(e) {
            if (e.which == 13) {// Enter
                $("#<%=SaveNewsSectionNameButton.ClientID %>").click();
                return false;
            }
            if (e.which == 27) { //Esc
                $("#cancelNewsSectionNameButton").click();
                return false;
            }
        }
        
    </script>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SecondaryBodyRegion">
    <div id="SecondaryBody" class="WR">
        <Workroom:NewsList PagesPerPagingItem="5" ID="NewsListUnit" PageLink="<%# CurrentPage.PageLink %>"
            Heading="<%$ Resources: EPiServer, workroom.newslist.heading %>" PreviewTextLength="80"
            OnAddButtonClicked="Add_Click" AddButtonVisible="true" runat="server" />
    </div>
</asp:Content>
