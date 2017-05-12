<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Delete.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.Delete" %>
<fieldset>
<legend><asp:Literal runat="server" Text="<%$ Resources: EPiServer, filemanager.deletefilesandfoldersheading %>"/></legend>
<asp:PlaceHolder ID="DirectoryConfirmation" runat="server">
    <h3>
        <asp:Literal runat="server" Text="<%$ Resources: EPiServer, filemanager.folders %>" />
    </h3>
    <asp:Label runat="server" CssClass="error" ID="FolderEmptyWarning" Text="<%$ Resources: EPiServer, filemanager.foldernotemptywarning %>" Visible="false" />
    <asp:BulletedList CssClass="documentList" ID="DirectoryList" runat="server" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="FileConfirmation" runat="server">
    <h3>
        <asp:Literal runat="server" Text="<%$ Resources: EPiServer, filemanager.files %>" />
    </h3>
    <asp:BulletedList CssClass="documentList" ID="FileList" runat="server" />
</asp:PlaceHolder>

<p>
    <asp:Button ID="DeleteItems" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, button.delete %>" OnClick="DeleteItems_Click"/>
    <asp:Button ID="CancelDelete" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>" OnClick="CancelDelete_Click" />
</p>
</fieldset>