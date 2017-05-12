<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="False" CodeBehind="Submenu.ascx.cs"
    Inherits="EPiServer.Templates.AlloyTech.Units.Static.Submenu" %>
<EPiServer:PageTree ShowRootPage="true" runat="server" ID="Menu">
    <HeaderTemplate>
        <div id="SubMenuArea">
    </HeaderTemplate>
    <IndentTemplate>
        <ul>
    </IndentTemplate>
    <ItemHeaderTemplate>
        <li>
    </ItemHeaderTemplate>
    <TopTemplate>
        <span class="toplevel">
            <EPiServer:Property PropertyName="PageLink" runat="server" />
        </span>
    </TopTemplate>
    <SelectedTopTemplate>
        <span class="toplevel selected">
            <%# Server.HtmlEncode(Container.CurrentPage.PageName) %>
        </span>
    </SelectedTopTemplate>
    <ItemTemplate>
        <EPiServer:Property PropertyName="PageLink" runat="server" />
    </ItemTemplate>
    <SelectedItemTemplate>
        <span class="selected">
            <%# Server.HtmlEncode(Container.CurrentPage.PageName) %>
        </span>
    </SelectedItemTemplate>
    <ItemFooterTemplate>
        </li>
    </ItemFooterTemplate>
    <UnindentTemplate>
        </ul>
    </UnindentTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</EPiServer:PageTree>
