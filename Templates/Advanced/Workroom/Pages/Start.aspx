<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="Start.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.Start" %>

<%@ Register TagPrefix="Workroom" TagName="EditableContent" Src="../Units/EditableContent.ascx" %>
<%@ Register TagPrefix="Workroom" TagName="FileList" Src="../Units/FileList.ascx" %>
<%@ Register TagPrefix="Workroom" TagName="NewsList" Src="../Units/NewsList.ascx" %>
<%@ Register TagPrefix="Workroom" TagName="CalendarList" Src="../Units/CalendarList.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR">
        <asp:ValidationSummary runat="server" />
        <Workroom:EditableContent PropertyName="MainBody" runat="server" EditButtonTitle="<%$ Resources: EPiServer, workroom.editdescription %>"
            SaveButtonTitle="<%$ Resources: EPiServer, button.save %>" CancelButtonTitle="<%$ Resources: EPiServer, button.cancel %>" />
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SecondaryBodyRegion">
    <div id="SecondaryBody" class="WR">
        <Workroom:NewsList ID="NewsListUnit" Heading="<%$ Resources: EPiServer, workroom.newslist.heading %>"
            MaxCount="10" PagesPerPagingItem="3" PreviewTextLength="80" runat="server" />
        <Workroom:FileList ID="FileList" RootFolder="<%# FileFolderRoot %>" Heading="<%$ Resources: EPiServer, workroom.filelist.heading %>"
            MaxCount="7" runat="server" />
        <Workroom:CalendarList ID="CalendarList" CalendarContainer='<%# (EPiServer.Core.PageReference)CurrentPage["CalendarContainer"] %>'
            MaxCount="5" runat="server" HideIfEmpty="true" />
    </div>
</asp:Content>
