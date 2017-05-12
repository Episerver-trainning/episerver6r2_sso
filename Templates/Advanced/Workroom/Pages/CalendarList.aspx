<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="CalendarList.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.CalendarList" %>

<%@ Register TagPrefix="Workroom" TagName="CalendarList" Src="../Units/CalendarList.ascx" %>
<%@ Register TagPrefix="Workroom" TagName="CreatePageBox" Src="../Units/CreatePageBox.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="MainBodyRegion">
    <div id="MainBody" class="WR WRCalendarList">
        <asp:Panel CssClass="buttonToolbar" runat="server" ID="ToolBarSection" Visible="false">
            <a class="linkButton createButton" id="addButton" href="<%=CreateCalendarBox.GetActivateJavaScript(ToolBarSection.ClientID)%>">
                <asp:Literal runat="server" Text="<%$ Resources: EPiServer, workroom.calendarlist.calendaraddnew%>" /></a>
        </asp:Panel>
        <EPiServer:Property PropertyName="PageName" runat="server" CustomTagName="h1" />
        <EPiServer:PageList ID="CalendarPageList" PageLink="<%#CurrentPage.PageLink%>" SortOrder="PublishedDescending"
            runat="server">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <EPiServer:Property PropertyName="PageLink" runat="server" CustomTagName="h2" />
                    <Workroom:CalendarList ViewMode="ListView" CalendarContainer='<%# Container.CurrentPage.PageLink %>'
                        MaxCount="3" runat="server" HideIfEmpty="true" />
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </EPiServer:PageList>
        <Workroom:CreatePageBox runat="server" ID="CreateCalendarBox" LabelText="<%$ Resources: EPiServer, workroom.calendarlist.calendarsectionname %>"
            ButtonOkTitle="<%$ Resources: EPiServer, button.add %>" Visible="false" />
    </div>

    <script type="text/javascript">
             $(document).ready(function() { 
                 if (window.location.search.search("addmode=true") != -1) {
                     <%=CreateCalendarBox.GetActivateJavaScript(ToolBarSection.ClientID)%>;
                 }

             })
    </script>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SecondaryBodyRegion" />
