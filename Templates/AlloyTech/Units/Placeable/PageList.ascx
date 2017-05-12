<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PageList.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Placeable.PageList" %>
<%@ Register TagPrefix="AlloyTech" TagName="Document" Src="~/Templates/AlloyTech/Units/Placeable/Document.ascx" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>
<EPiServer:PageList ID="EPiPageList" runat="server">
    <HeaderTemplate>
        <hr runat="server" visible="<%# ShowTopRuler %>" class="clear" />
        <div class="pageList">
            <h2>
                <%= ListHeading %></h2>
            <ul>
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <h3>
                <EPiServer:Property PropertyName="PageLink" runat="server" />
            </h3>
            <span class="datetime" runat="server" visible="<%# DisplayDate %>">
                <%# Container.CurrentPage.GetFormattedPublishDate() %></span>
            <p>
                <%# Container.CurrentPage.GetPreviewText(PreviewTextLength) %>
            </p>
            <AlloyTech:Document runat="server" DocumentProperty="<%# DocumentLinkProperty %>"
                DocumentPage="<%# Container.CurrentPage %>" Visible="<%# DisplayDocumentLink(Container.CurrentPage) %>" />
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul> </div>
    </FooterTemplate>
</EPiServer:PageList>
