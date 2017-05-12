<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HeadingContent.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.HeadingContent" %>
<%@ Register TagPrefix="FileManagerView" TagName="SwitchViewContent" Src="SwitchViewContent.ascx" %>

<div class="toolbar">
    <FileManagerView:SwitchViewContent runat="server" />
    <asp:ValidationSummary class="validator" runat="server" />
</div>
<div class="crumbs">
    <asp:PlaceHolder runat="server" ID="CurrentFolderBreadcrumbs" />
</div>