<%@ Control Language="c#" AutoEventWireup="false" CodeBehind="ChangedPages.ascx.cs"
    Inherits="EPiServer.Templates.AlloyTech.Units.Placeable.ChangedPages" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>
<EPiServer:PageList runat="server" ID="pagelist" SortOrder="ChangedDescending" MaxCount="10">
    <HeaderTemplate>
        <div class="pageList">
            <ul>
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <h3>
                <EPiServer:Property PropertyName="PageLink" runat="server" />
            </h3>
            <span>
                <%# Container.CurrentPage.Changed.ToFormattedDateAndTime() %>
            </span>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
     </div>
    </FooterTemplate>
</EPiServer:PageList>
