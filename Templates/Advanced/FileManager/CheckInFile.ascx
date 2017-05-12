<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CheckInFile.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.CheckInFile" %>

<%@ Register TagPrefix="FileManager" TagName="FileList" Src="FileList.ascx" %>
<fieldset>
    <legend><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: EPiServer, filemanager.switchview.checkin %>" /></legend>
    <p runat="server" id="FileUploadPanel">
        <asp:Label Runat="server" ID="currentFile"/>&nbsp;<asp:Label Runat="server" ID="fileCount"/><br />       
        <asp:Label ID="Label1" runat="server" Text="<%$ Resources: EPiServer, filemanager.choosefile %>" AssociatedControlID="FileUpload" />
        <asp:FileUpload ID="FileUpload" runat="server" />        
        <br />
        <asp:Label ID="labelExceptionMessage" runat="server" Visible="false" CssClass="error" />
    </p>
    <asp:Panel runat="server" ID="CommentsPanel">
        <p>
            <asp:Label ID="Label2" runat="server" Text="<%$ Resources: EPiServer, filemanager.filecomment %>" AssociatedControlID="CheckInComment" />
            <asp:TextBox runat="server" ID="CheckInComment"/>
        </p>
    </asp:Panel>
    <p>
        <asp:Button runat="server" CssClass="button" ID="UploadButton" Text="<%$ Resources: EPiServer, filemanager.button.upload %>" OnClick="CheckInFile_Click" />      
        <asp:Button runat="server" CssClass="button" ID="CancelButton" Text="<%$ Resources: EPiServer, button.cancel %>" OnClick="CancelButton_Click" CausesValidation="false" />
    </p>
</fieldset>

<FileManager:FileList ID="FileList1" runat="server" />