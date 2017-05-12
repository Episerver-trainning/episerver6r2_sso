<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FileList.ascx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Units.FileList" %>
<%@ Import Namespace="EPiServer.Web.Hosting" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>

<div class="WRFileList WRList">
    <h2 id="FileListHeading" runat="server"></h2>
    <asp:Panel runat="server" ID="ErrorPanel" Visible="false">
        <asp:Label runat="server" ID="ErrorMessage" CssClass="error" />
    </asp:Panel>
    <asp:Panel runat="server" ID="FileListPanel">
        <asp:Repeater runat="server" ID="FileListing">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a class='document <%# String.Format("{0}Extension", GetFileExtension((UnifiedFile)Container.DataItem)) %>'
                    href='<%# ((UnifiedFile)Container.DataItem).VirtualPath %>'><%# FormatFileNameLength(((UnifiedFile)Container.DataItem).Name)%></a>
                    <span class="datetime"><%#((UnifiedFile)Container.DataItem).Changed.ToFormattedDateAndTime()%></span>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
</div>
