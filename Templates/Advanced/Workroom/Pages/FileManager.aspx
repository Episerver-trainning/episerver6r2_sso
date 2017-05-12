<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    AutoEventWireup="false" CodeBehind="FileManager.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.FileManager" %>
<%@ Register TagPrefix="FileManager" Namespace="EPiServer.Templates.Advanced.FileManager.Core.WebControls" 
    Assembly="EPiServer.Templates.AlloyTech"  %>

<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
    <div id="MainBodyAreaWide" class="WR">
        <FileManager:FileManagerControl ID="FileManagerControl" runat="server" RootVirtualPath="<%# FileFolderRoot %>" />
    </div>
</asp:Content>
