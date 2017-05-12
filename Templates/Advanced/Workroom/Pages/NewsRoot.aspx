<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="NewsRoot.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.NewsRoot" %>

<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR WRNewsRoot">
        <asp:Panel CssClass="buttonToolbar" runat="server" ID="NewsSectionToolBar" Visible="false">
            <a class="linkButton createButton" id="ShowAddNewsSectionPanelButton" href="javascript:ShowAddNewNewsSectionPanel()">
                <%=Translate("/workroom/newslistpage/addnewssection")%></a>
        </asp:Panel>
        <EPiServer:Property ID="PageNameProperty" CustomTagName="h1" PropertyName="PageName"
            runat="server" />
        <EPiServer:PageList ID="NewsLists" PageLink="<%# CurrentPage.PageLink %>" SortOrder="PublishedDescending"
            runat="server">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <EPiServer:Property PropertyName="PageLink" runat="server" CustomTagName="h2" CssClass="newsSection" />
                    <EPiServer:PageList ID="NewsListing" PageLink="<%# Container.CurrentPage.PageLink %>"
                        SortOrder="PublishedDescending" MaxCount="3" runat="server">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li>
                                <span class="datetime"><%# Container.CurrentPage.GetFormattedPublishDate() %></span>
                                <asp:HyperLink NavigateUrl='<%# Container.CurrentPage.GetNewsItemNavigateUrl() %>'
                                    ToolTip="<%$ Resources: EPiServer, navigation.readmore %>" CssClass="newsItem" runat="server">
                                    <EPiServer:Property PropertyName="PageName" runat="server" />
                                </asp:HyperLink>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </EPiServer:PageList>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </EPiServer:PageList>
        <asp:Panel ID="AddNewNewsSectionPanel" CssClass="hidden" runat="server" Visible="false">
            <div>
                <asp:Label runat="server" Text="<%$ Resources: EPiServer, workroom.newslist.newssectionname %>"
                    AssociatedControlID="NewNewsSectionNameTextBox" /><br />
                <asp:TextBox runat="server" ID="NewNewsSectionNameTextBox" MaxLength="255" />
                <asp:RequiredFieldValidator ID="NewNewsSectionNameRequiredFieldValidator" runat="server"
                    ControlToValidate="NewNewsSectionNameTextBox" ValidationGroup="NewsSectionName"
                    Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.newslistpage.titleempty %>" />
            </div>
            <div class="buttons">
                <asp:Button runat="server" CssClass="buttonExt" ID="AddNewNewsSectionButton" Text="<%$ Resources: EPiServer, button.add %>"
                    OnClick="AddNewNewsSection_Click" ValidationGroup="NewsSectionName" OnClientClick="preventDoubleClick(this);" />
                <input class="button" id="cancelAddNewNewsSectionButton" onclick="javascript:HideAddNewNewsSectionPanel()"
                    type="button" value="<%= Translate("/button/cancel") %>" />
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        var initialValue = "";
        $(document).ready(function() {
            if (window.location.search.search("addmode=true") != -1) {
                ShowAddNewNewsSectionPanel();
            }

        })
        function ShowAddNewNewsSectionPanel() {
            $("#<%=AddNewNewsSectionPanel.ClientID %>").show();
            $("#ShowAddNewsSectionPanelButton").hide();
            initialValue = $("#<%=NewNewsSectionNameTextBox.ClientID %>").val();
            $("#<%=NewNewsSectionNameTextBox.ClientID %>").keydown(NewNewsSectionNameClick);
            $("#<%=NewNewsSectionNameTextBox.ClientID %>").select();
        }
        function HideAddNewNewsSectionPanel() {
            $("#<%=AddNewNewsSectionPanel.ClientID %>").hide();
            $("#ShowAddNewsSectionPanelButton").show();
            $("#<%=NewNewsSectionNameTextBox.ClientID %>").val(initialValue);
            $("#<%=NewNewsSectionNameRequiredFieldValidator.ClientID %>").css("visibility", "hidden");
        }
        function NewNewsSectionNameClick(e) {
            if (e.which == 13) { //Enter
                $("#<%=AddNewNewsSectionButton.ClientID %>").click();
                return false;
            }
            if (e.which == 27) { //Esc
                $("#cancelAddNewNewsSectionButton").click();
                return false;
            }
        }
    </script>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SecondaryBodyRegion" />
